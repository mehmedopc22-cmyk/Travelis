using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;
using System.ComponentModel.DataAnnotations;

namespace WEB.Controllers
{
    public class TaxiController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> Index(string? pickup, string? dropoff, string? companyName, CancellationToken cancellationToken = default)
        {
            List<TaxiCompanyEntity>? apiCompanies;
            try
            {
                apiCompanies = await Utils.CallApiAsync<List<TaxiCompanyEntity>>(
                    _configuration["DefaultApiUrl"] + "taxi/all",
                    HttpMethod.Get,
                    cancellationToken: cancellationToken);
            }
            catch (HttpRequestException)
            {
                ViewData["TaxiApiError"] = true;
                apiCompanies = null;
            }

            List<TaxiCompanyListItemViewModel> model = (apiCompanies ?? [])
                .OrderBy(c => c.Name)
                .Select(c => new TaxiCompanyListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    City = c.City,
                    Country = c.Country,
                    Street = c.Street,
                    PostalCode = c.PostalCode,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    Approved = c.Approved,
                    Status = c.Status
                })
                .ToList();

            string pickupFilter = pickup?.Trim() ?? string.Empty;
            string dropoffFilter = dropoff?.Trim() ?? string.Empty;
            string companyNameFilter = companyName?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(pickupFilter) ||
                !string.IsNullOrWhiteSpace(dropoffFilter) ||
                !string.IsNullOrWhiteSpace(companyNameFilter))
            {
                model = model
                    .Where(c =>
                        (string.IsNullOrWhiteSpace(companyNameFilter) || MatchesCompanyName(c, companyNameFilter)) &&
                        (string.IsNullOrWhiteSpace(pickupFilter) || MatchesAddress(c, pickupFilter)) &&
                        (string.IsNullOrWhiteSpace(dropoffFilter) || MatchesAddress(c, dropoffFilter)))
                    .ToList();
            }

            ViewData["CreateTaxiSuccess"] = TempData["CreateTaxiSuccess"];
            ViewData["CreateTaxiError"] = TempData["CreateTaxiError"];
            ViewData["PickupFilter"] = pickupFilter;
            ViewData["DropoffFilter"] = dropoffFilter;
            ViewData["CompanyNameFilter"] = companyNameFilter;

            return View(model);
        }

        private static bool MatchesAddress(TaxiCompanyListItemViewModel company, string addressFilter)
        {
            if (string.IsNullOrWhiteSpace(addressFilter))
            {
                return false;
            }

            return company.FullAddress.Contains(addressFilter, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesCompanyName(TaxiCompanyListItemViewModel company, string companyNameFilter)
        {
            if (string.IsNullOrWhiteSpace(companyNameFilter))
            {
                return false;
            }

            return company.Name.Contains(companyNameFilter, StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(TaxiCompanyCreateInputModel input, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                TempData["CreateTaxiError"] = "Please fill all required fields.";
                return RedirectToAction(nameof(Index));
            }

            string? adminKey = _configuration["TaxiAdminKey"];
            if (string.IsNullOrWhiteSpace(adminKey))
            {
                TempData["CreateTaxiError"] = "Taxi admin key is not configured in WEB appsettings.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.Equals(input.AdminKey, adminKey, StringComparison.Ordinal))
            {
                TempData["CreateTaxiError"] = "Invalid admin key.";
                return RedirectToAction(nameof(Index));
            }

            TaxiCompanyEntity company = new()
            {
                Id = Guid.Empty,
                Name = input.Name.Trim(),
                Country = input.Country.Trim(),
                City = input.City.Trim(),
                Street = input.Street.Trim(),
                PostalCode = input.PostalCode.Trim(),
                PhoneNumber = input.PhoneNumber?.Trim() ?? string.Empty,
                Email = input.Email.Trim(),
                Status = input.Status,
                Approved = input.Approved
            };

            try
            {
                Dictionary<string, string> headers = new()
                {
                    ["X-Admin-Key"] = input.AdminKey
                };

                await Utils.CallApiAsync<TaxiCompanyEntity>(
                    _configuration["DefaultApiUrl"] + "taxi",
                    HttpMethod.Post,
                    company,
                    headers: headers,
                    cancellationToken: cancellationToken);

                TempData["CreateTaxiSuccess"] = $"Taxi company '{company.Name}' was added successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["CreateTaxiError"] = $"Failed to add taxi company. {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public sealed class TaxiCompanyCreateInputModel
        {
            [Required]
            public string AdminKey { get; set; } = string.Empty;
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Country { get; set; } = string.Empty;
            [Required]
            public string City { get; set; } = string.Empty;
            [Required]
            public string Street { get; set; } = string.Empty;
            [Required]
            public string PostalCode { get; set; } = string.Empty;
            public string? PhoneNumber { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            public int Status { get; set; } = 1;
            public bool Approved { get; set; } = false;
        }
    }
}
