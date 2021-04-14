using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagraph.Models
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Caption { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        [ForeignKey(nameof(Picture))]
        public int PictureId { get; set; }

        [Required]
        public virtual Picture Picture { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
