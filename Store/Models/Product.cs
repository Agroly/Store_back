using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Store.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string PhotoFileName { get; set; } // Имя файла фото товара

        [NotMapped]
        public IFormFile Photo { get; set; } // Поле для загрузки фото, не сохраняется в БД
    }
}
