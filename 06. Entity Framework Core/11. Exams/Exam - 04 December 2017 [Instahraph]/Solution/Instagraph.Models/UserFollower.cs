using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagraph.Models
{ 
    public class UserFollower
    {
        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int FollowerId { get; set; }

        [Required]
        public virtual User Follower { get; set; }
    }
}
 