using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    public class CarsController : Controller
    {
        // 1. Начална страница за коли
        public IActionResult Index() => View();

        // 2. Списък с коли (Grid изглед)
        public IActionResult Grid() => View();

        // 3. Списък с коли (List изглед)
        public IActionResult List() => View();

        // 4. Детайли за конкретна кола
        public IActionResult Details() => View();

        // 5. Условия и политики
        public IActionResult Policies() => View();

        // 6. Форма за резервация
        public IActionResult Booking() => View();

        // 7. Обобщение на резервацията (Summary)
        public IActionResult Summary() => View();

        // 8. Успешна резервация
        public IActionResult Success() => View();
    }
}