using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagraph.Models
{
   public class User
    {
        public User()
        {
            this.Followers = new HashSet<UserFollower>();
            this.UsersFollowing = new HashSet<UserFollower>();
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        [Required]
        public string Username { get; set; }

        [MaxLength(30)]
        [Required]
        public string Password { get; set; }

        [ForeignKey(nameof(ProfilePicture))]
        public int ProfilePictureId { get; set; }

        [Required]
        public virtual Picture ProfilePicture { get; set; }

        public virtual ICollection<UserFollower> Followers  { get; set; }

        public virtual ICollection<UserFollower> UsersFollowing { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
