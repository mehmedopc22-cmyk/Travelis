using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Policy;
using WEB.Models; // Увери се, че това съвпада с името на твоя проект

namespace WEB.Controllers
{
    public class CarsController : Controller
    {
        // 1. Начална страница
        public IActionResult Index() => View();

        // 2. Списък с коли (Grid)
        public IActionResult Grid(string location, DateTime? startDate, DateTime? endDate, string driverAge, string residence)
        {
            ViewBag.SelectedLocation = string.IsNullOrEmpty(location) ? "София (SOF)" : location;
            ViewBag.DateRange = (startDate.HasValue && endDate.HasValue)
                ? $"{startDate.Value:dd MMM} - {endDate.Value:dd MMM}"
                : "Изберете дати";

            // ТРЯБВА ДА ГИ ЗАПАЗИШ ВЪВ VIEWBAG, ЗА ДА ГИ ПРЕДАДЕШ КЪМ DETAILS
            ViewBag.DriverAge = driverAge;
            ViewBag.Residence = residence;
            ViewBag.Location = location;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View();
        }

        // 3. Детайли за колата - приема данните от Grid и ги предава към Details страницата
        public IActionResult Details(string location, DateTime? startDate, DateTime? endDate, string driverAge, string residence)
        {
            // Пращаме ВСИЧКИ данни към View-то чрез ViewBag
            ViewBag.Location = location;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.DriverAge = driverAge;
            ViewBag.Residence = residence;

            return View();
        }

        // 4. Форма за резервация (GET) - Тук се попълват автоматично данните
        [HttpGet]
        public IActionResult Booking(string location, DateTime? startDate, DateTime? endDate, string driverAge, string residence)
        {
            // Създаваме модела с данните от URL адреса
            var model = new BookingViewModel
            {
                PickUpLocation = location,
                // Ако няма дати в URL, слагаме днешната и след 3 дни
                PickUpDate = startDate ?? DateTime.Now,
                DropOffDate = endDate ?? DateTime.Now.AddDays(3), // <-- Трябваше запетая тук
                DriverAge = driverAge,
                Residence = residence
            };

            return View(model);
        }

        // 5. Обработка на резервацията (POST) - Когато натиснеш финалния бутон
        [HttpPost]
        public IActionResult Booking(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Тук записваш в базата или пращаш имейл
                TempData["SuccessMessage"] = "Резервацията на " + model.DriverName + " е приета!";
                return RedirectToAction("Summary");
            }

            // Ако има грешка в данните, връщаме формата с попълнените полета
            return View(model);
        }

        // 6. Обобщение
        public IActionResult Summary() => View();

        // 7. Успех
        public IActionResult Success() => View();

        // Други страници
        public IActionResult List() => View();
        public IActionResult Policies() => View();
    }
}