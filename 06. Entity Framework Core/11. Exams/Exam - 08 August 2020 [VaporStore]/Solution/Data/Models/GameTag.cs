﻿namespace VaporStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GameTag
    {
        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }

        public virtual Game Game { get; set; }

        [Required]
        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
