using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagraph.Models
{
    //•	Id – an integer, Primary Key
    //•	Content – a string with max length 250
    //•	UserId – an integer
    //•	User – a User
    //•	PostId – an integer
    //•	Post – a Post

    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250)]
        [Required]
        public string Content { get; set; }

        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int PostId { get; set; }

        [Required]
        public virtual Post Post { get; set; }
    }
}
