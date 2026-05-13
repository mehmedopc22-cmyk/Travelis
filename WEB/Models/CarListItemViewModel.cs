namespace WEB.Models
{
    public class CarListItemViewModel
    {
        public int Id { get; set; }

        public string Brand { get; set; } // Марка (напр. BMW)

        public string Model { get; set; } // Модел (напр. X5)

        public decimal PricePerDay { get; set; } // Цена на ден

        public string ImageUrl { get; set; } // Път към снимката (напр. /images/cars/bmw.jpg)

        public string Transmission { get; set; } // Автоматик или Ръчна

        public string FuelType { get; set; } // Бензин, Дизел, Електрическа

        public int Year { get; set; } // Година на производство

        public bool IsAvailable { get; set; } // Дали е свободна в момента
    }
}