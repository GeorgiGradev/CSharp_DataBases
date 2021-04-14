using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [Key]
        public int SongId { get; set; }
        [Required]
        public virtual Song Song { get; set; }

        [Key]
        public int PerformerId { get; set; }
        [Required]
        public virtual Performer Performer { get; set; }
    }
}
