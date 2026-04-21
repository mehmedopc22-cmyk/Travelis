using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    [Authorize(Roles = "HotelAdmin")]
    public class HotelAdminController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            List<HotelEntity>? hotels = await Utils.CallApiAsync<List<HotelEntity>>(
                $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/hotel-admin/hotels",
                HttpMethod.Get,
                HttpContext,
                cancellationToken: cancellationToken);

            return View(hotels ?? []);
        }

        public async Task<IActionResult> Manage(Guid hotelId, CancellationToken cancellationToken)
        {
            HotelEntity? hotel = await Utils.CallApiAsync<HotelEntity>(
                $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/hotel-admin/hotels/{hotelId}",
                HttpMethod.Get,
                HttpContext,
                cancellationToken: cancellationToken);

            List<ImageEntity>? images = await Utils.CallApiAsync<List<ImageEntity>>(
                $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/hotel-admin/hotels/{hotelId}/images",
                HttpMethod.Get,
                HttpContext,
                cancellationToken: cancellationToken);

            if (hotel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new HotelAdminManageViewModel
            {
                Hotel = hotel,
                Images = images ?? []
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHotel(HotelEntity hotel, CancellationToken cancellationToken)
        {
            await Utils.CallApiAsync<object>(
                $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/hotel-admin/hotels/{hotel.Id}",
                HttpMethod.Put,
                HttpContext,
                hotel,
                cancellationToken: cancellationToken);

            TempData["HotelAdminMessage"] = "Информацията за хотела беше обновена.";
            return RedirectToAction(nameof(Manage), new { hotelId = hotel.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHotelImage(Guid hotelId, List<IFormFile>? hotelImages, CancellationToken cancellationToken)
        {
            List<IFormFile> selectedImages = (hotelImages ?? [])
                .Where(image => image.Length > 0)
                .ToList();

            if (selectedImages.Count == 0)
            {
                TempData["HotelAdminError"] = "Избери изображение за хотела.";
                return RedirectToAction(nameof(Manage), new { hotelId });
            }

            try
            {
                List<string> imageNames = [];

                foreach (IFormFile hotelImage in selectedImages)
                {
                    imageNames.Add(await SaveUploadedImageAsync(hotelImage, "hotels", cancellationToken));
                }

                await Utils.CallApiAsync<List<ImageEntity>>(
                    $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/hotel-admin/hotels/{hotelId}/images/batch",
                    HttpMethod.Post,
                    HttpContext,
                    new ImageUploadBatchRequestDTO
                    {
                        ImageNames = imageNames
                    },
                    cancellationToken: cancellationToken);

                TempData["HotelAdminMessage"] = "Снимката беше добавена към хотела.";
                if (selectedImages.Count > 1)
                {
                    TempData["HotelAdminMessage"] = $"{selectedImages.Count} images were added to the hotel.";
                }
            }
            catch (Exception ex)
            {
                TempData["HotelAdminError"] = ex.Message;
            }

            return RedirectToAction(nameof(Manage), new { hotelId });
        }

        private async Task<string> SaveUploadedImageAsync(IFormFile image, string subfolder, CancellationToken cancellationToken)
        {
            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Allowed image formats are JPG, PNG and WEBP.");
            }

            string relativeDirectory = Path.Combine("uploads", subfolder);
            string absoluteDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeDirectory);
            Directory.CreateDirectory(absoluteDirectory);

            string fileName = $"{Guid.NewGuid():N}{extension}";
            string absolutePath = Path.Combine(absoluteDirectory, fileName);

            await using FileStream stream = System.IO.File.Create(absolutePath);
            await image.CopyToAsync(stream, cancellationToken);

            return fileName;
        }

        public static string BuildHotelImageUrl(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return string.Empty;
            }

            if (imageName.StartsWith("/", StringComparison.Ordinal) || imageName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imageName;
            }

            return "/" + Path.Combine("uploads", "hotels", imageName).Replace("\\", "/");
        }
    }
}
