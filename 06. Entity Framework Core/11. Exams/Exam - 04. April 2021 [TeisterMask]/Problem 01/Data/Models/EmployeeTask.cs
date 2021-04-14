using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeisterMask.Data.Models
{
    public class EmployeeTask
    {
        public int EmployeeId { get; set; }

        [Required]
        public virtual Employee Employee { get; set; }

        public virtual int TaskId { get; set; }

        [Required]
        public virtual Task Task { get; set; }
    }
}
