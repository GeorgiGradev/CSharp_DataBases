namespace VaporStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using VaporStore.Data.Models.Enums;

    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public PurchaseType Type { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string ProductKey { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [ForeignKey("Card")]
        public int CardId { get; set; }

        [Required]
        public virtual Card Card { get; set; }

        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }

        [Required]
        public virtual Game Game { get; set; }
    }
}
