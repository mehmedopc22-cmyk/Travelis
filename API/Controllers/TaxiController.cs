using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("taxi")]
    public class TaxiController(ITaxiCompanyDAO taxiCompanyDao, IConfiguration configuration) : ControllerBase
    {
        private readonly ITaxiCompanyDAO _taxiCompanyDao = taxiCompanyDao;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("all")]
        public ActionResult<IEnumerable<TaxiCompanyEntity>> GetAll()
        {
            try
            {
                IEnumerable<TaxiCompanyEntity> companies = _taxiCompanyDao.SelectAll();
                return Ok(companies);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:guid}")]
        public ActionResult<TaxiCompanyEntity> GetById(Guid id)
        {
            try
            {
                TaxiCompanyEntity? company = _taxiCompanyDao.SelectById(id);
                if (company is null)
                {
                    return NotFound();
                }

                return Ok(company);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<TaxiCompanyEntity> Create([FromBody] JsonElement payload)
        {
            try
            {
                string? configuredAdminKey = _configuration["TaxiAdminKey"];
                if (string.IsNullOrWhiteSpace(configuredAdminKey))
                {
                    return StatusCode(500, "TaxiAdminKey is not configured.");
                }

                if (!Request.Headers.TryGetValue("X-Admin-Key", out var providedAdminKey) ||
                    !string.Equals(providedAdminKey.ToString(), configuredAdminKey, StringComparison.Ordinal))
                {
                    return Unauthorized("Only administrators can add taxi companies.");
                }

                TaxiCompanyCreateRequest? request = TryParseCreateRequest(payload);
                if (request is null)
                {
                    return BadRequest("Invalid payload. Use either { ...fields } or { \"company\": { ...fields } }.");
                }

                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Country) ||
                    string.IsNullOrWhiteSpace(request.City) ||
                    string.IsNullOrWhiteSpace(request.Street) ||
                    string.IsNullOrWhiteSpace(request.PostalCode) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Missing required fields: name, country, city, street, postalCode, email.");
                }

                TaxiCompanyEntity company = new()
                {
                    Id = request.Id ?? Guid.Empty,
                    Name = request.Name,
                    Country = request.Country,
                    City = request.City,
                    Street = request.Street,
                    PostalCode = request.PostalCode,
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Email = request.Email,
                    Status = request.Status ?? 1,
                    Approved = request.Approved ?? false
                };

                TaxiCompanyEntity insertedCompany = _taxiCompanyDao.Insert(company);
                return Ok(insertedCompany);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static TaxiCompanyCreateRequest? TryParseCreateRequest(JsonElement payload)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                if (payload.ValueKind == JsonValueKind.Object &&
                    payload.TryGetProperty("company", out JsonElement companyElement))
                {
                    return companyElement.Deserialize<TaxiCompanyCreateRequest>(options);
                }

                return payload.Deserialize<TaxiCompanyCreateRequest>(options);
            }
            catch
            {
                return null;
            }
        }

        private sealed class TaxiCompanyCreateRequest
        {
            public Guid? Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
            public string? PhoneNumber { get; set; }
            public string Email { get; set; } = string.Empty;
            public int? Status { get; set; }
            public bool? Approved { get; set; }
        }
    }
}
