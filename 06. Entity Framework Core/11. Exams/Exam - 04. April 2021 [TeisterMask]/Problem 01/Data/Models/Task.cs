using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.Data.Models
{
    public class Task
    {
        public Task()
        {
            this.EmployeesTasks = new HashSet<EmployeeTask>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public ExecutionType ExecutionType { get; set; }

        [Required]
        public LabelType LabelType { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public virtual Project Project { get; set; }

        public virtual ICollection<EmployeeTask> EmployeesTasks { get; set; }
    }
}
