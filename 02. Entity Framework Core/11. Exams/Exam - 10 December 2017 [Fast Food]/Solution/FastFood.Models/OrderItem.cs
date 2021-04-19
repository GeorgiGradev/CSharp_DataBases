using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastFood.Models
{
    public class OrderItem
    {
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [Required]
        public virtual Order Order { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [Required]
        public virtual Item Item { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
