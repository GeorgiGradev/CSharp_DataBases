using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Album
    {
        public Album()
        {
            this.Songs = new HashSet<Song>();
        }

        [Key]
        public int Id { get; set; }
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }


        public int? ProducerId { get; set; }
        public virtual Producer Producer { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

        //TODO
        [NotMapped]
        public decimal Price => this.Songs.Sum(x => x.Price);
    }
}
