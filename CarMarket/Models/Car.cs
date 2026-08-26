using System.ComponentModel.DataAnnotations;

namespace CarMarket.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Range(0, 10000000)]
        public decimal Price { get; set; }

        [Range(0, 2000000)]
        public int Mileage { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
       
        public string? ImageUrl2 { get; set; }

        public string? ImageUrl3 { get; set; }

        public string? ImageUrl4 { get; set; }
    }
}