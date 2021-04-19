using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFood.Models
{
    public class Item
    {
        public Item()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Name { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
