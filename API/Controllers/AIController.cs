using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("ai")]
    public class AIController(
        IHotelDAO hotelDAO,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("hotels/filter")]
        public async Task<ActionResult<AiHotelFilterResponseDTO>> FilterHotels(
            AiHotelFilterRequestDTO request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SelectedLocation))
            {
                return BadRequest("Location is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt is required.");
            }

            HotelFilterRequestDTO locationFilter = new()
            {
                Destination = request.SelectedLocation,
                SortBy = "recommended"
            };

            List<HotelEntity> candidates = _hotelDAO.SelectFiltered(locationFilter).ToList();
            AiHotelSelectionResult selection = await RankHotelsWithLocalModelAsync(request, candidates, cancellationToken);

            return Ok(new AiHotelFilterResponseDTO
            {
                AppliedFilters = locationFilter,
                Hotels = selection.Hotels,
                AiResponse = selection.Explanation
            });
        }

        private async Task<AiHotelSelectionResult> RankHotelsWithLocalModelAsync(
            AiHotelFilterRequestDTO request,
            List<HotelEntity> candidates,
            CancellationToken cancellationToken)
        {
            if (candidates.Count == 0)
            {
                return new AiHotelSelectionResult([], "Няма намерени хотели за избраната локация.");
            }

            string endpoint = _configuration["AI:LocalModel:Endpoint"] ?? string.Empty;
            string model = _configuration["AI:LocalModel:Model"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model))
            {
                return new AiHotelSelectionResult(candidates, "AI услугата не е конфигурирана, затова показвам всички хотели за избраната локация.");
            }

            var candidatePayload = candidates.Select(hotel => new
            {
                id = hotel.Id,
                hotel.Name,
                hotel.Country,
                hotel.City,
                hotel.Street,
                hotel.PostalCode,
                hotel.PhoneNumber,
                hotel.Email,
                hotel.Approved,
                hotel.Status,
                rating = (decimal?)null
            });

            var localModelRequest = new
            {
                model,
                temperature = 0.1,
                stream = false,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = """
You help rank and filter hotels for Travelis.
You will receive a selected location, a user prompt, and candidate hotels from that location.
Each candidate includes a rating field. If rating is null, do not invent a rating.
Return only JSON in this exact shape:
{
  "hotelIds": ["guid-1", "guid-2"],
  "explanation": "short user-facing explanation in Bulgarian"
}
Only include hotel IDs from the candidates list. Put the best matches first. If every candidate is relevant, return all candidate IDs in ranked order.
Keep explanation concise and mention why these hotels match the prompt. If ratings are null, do not mention ratings.
"""
                    },
                    new
                    {
                        role = "user",
                        content = JsonSerializer.Serialize(new
                        {
                            selectedLocation = request.SelectedLocation,
                            prompt = request.Prompt,
                            hotels = candidatePayload
                        })
                    }
                }
            };

            using HttpRequestMessage httpRequest = new(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(localModelRequest),
                    Encoding.UTF8,
                    "application/json")
            };

            string? apiKey = _configuration["AI:LocalModel:ApiKey"];

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }

            using HttpClient httpClient = _httpClientFactory.CreateClient();
            using HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AiHotelSelectionResult(candidates, "AI услугата не върна успешен отговор, затова показвам всички хотели за избраната локация.");
            }

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            string? content = ExtractAssistantContent(responseBody);

            if (string.IsNullOrWhiteSpace(content))
            {
                return new AiHotelSelectionResult(candidates, "AI услугата не върна обяснение, затова показвам всички хотели за избраната локация.");
            }

            AiHotelSelectionPayload selectionPayload = ExtractSelectionPayload(content);

            if (selectionPayload.HotelIds.Count == 0)
            {
                return new AiHotelSelectionResult(candidates, selectionPayload.Explanation);
            }

            Dictionary<Guid, HotelEntity> hotelsById = candidates.ToDictionary(hotel => hotel.Id);
            List<HotelEntity> rankedHotels = [];

            foreach (Guid hotelId in selectionPayload.HotelIds)
            {
                if (hotelsById.TryGetValue(hotelId, out HotelEntity? hotel))
                {
                    rankedHotels.Add(hotel);
                }
            }

            return new AiHotelSelectionResult(
                rankedHotels.Count == 0 ? candidates : rankedHotels,
                selectionPayload.Explanation);
        }

        private static string? ExtractAssistantContent(string responseBody)
        {
            JsonDocument document;

            try
            {
                document = JsonDocument.Parse(responseBody);
            }
            catch (JsonException)
            {
                return null;
            }

            using (document)
            {
                JsonElement root = document.RootElement;

                if (!root.TryGetProperty("choices", out JsonElement choices) || choices.GetArrayLength() == 0)
                {
                    return null;
                }

                JsonElement firstChoice = choices[0];

                if (!firstChoice.TryGetProperty("message", out JsonElement message)
                    || !message.TryGetProperty("content", out JsonElement content))
                {
                    return null;
                }

                return content.GetString();
            }
        }

        private static AiHotelSelectionPayload ExtractSelectionPayload(string content)
        {
            int start = content.IndexOf('{');
            int end = content.LastIndexOf('}');

            if (start < 0 || end <= start)
            {
                return new AiHotelSelectionPayload([], content.Trim());
            }

            string json = content[start..(end + 1)];

            try
            {
                using JsonDocument document = JsonDocument.Parse(json);

                List<Guid> ids = [];

                if (document.RootElement.TryGetProperty("hotelIds", out JsonElement hotelIds)
                    && hotelIds.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement hotelId in hotelIds.EnumerateArray())
                    {
                        if (Guid.TryParse(hotelId.GetString(), out Guid parsedId))
                        {
                            ids.Add(parsedId);
                        }
                    }
                }

                string explanation = "AI филтърът подреди хотелите според заявката ти.";

                if (document.RootElement.TryGetProperty("explanation", out JsonElement explanationElement)
                    && explanationElement.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(explanationElement.GetString()))
                {
                    explanation = explanationElement.GetString()!;
                }

                return new AiHotelSelectionPayload(ids, explanation);
            }
            catch (JsonException)
            {
                return new AiHotelSelectionPayload([], content.Trim());
            }
        }

        private sealed record AiHotelSelectionPayload(List<Guid> HotelIds, string Explanation);

        private sealed record AiHotelSelectionResult(List<HotelEntity> Hotels, string Explanation);
    }
}
