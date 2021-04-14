namespace VaporStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Game
    {
        public Game()
        {
            this.Purchases = new HashSet<Purchase>();
            this.GameTags = new HashSet<GameTag>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        [ForeignKey("Developer")]
        public int DeveloperId { get; set; }

        [Required]
        public virtual Developer Developer { get; set; }
        
        [Required]
        [ForeignKey("Genre")]
        public int GenreId { get; set; }
      
        [Required]
        public virtual Genre Genre { get; set; }
        
        public virtual ICollection<Purchase> Purchases { get; set; }
        
        [Required]
        public virtual ICollection<GameTag> GameTags { get; set; }

    }
}
