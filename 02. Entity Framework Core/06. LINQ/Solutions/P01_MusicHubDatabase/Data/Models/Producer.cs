using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public Producer()
        {
            this.Albums = new HashSet<Album>();
        }

        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }
        public string Pseudonym  { get; set; }
        public string PhoneNumber { get; set; }


        public virtual ICollection<Album> Albums { get; set; }
    }
}
