using SoftJail.Data.Models.Enums;
using SoftJail.DataProcessor.ImportDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SoftJail.Data.Models
{
    public class Officer
    {
        public Officer()
        {
            this.OfficerPrisoners = new HashSet<OfficerPrisoner>();
        }


        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string FullName { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335M")]
        public decimal Salary { get; set; }

        [Required]
        [Range(0,3)]
        public  Position Position { get; set; }

        [Required]
        [Range(0,4)]
        public Weapon Weapon { get; set; }

        [Required]
        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<OfficerPrisoner> OfficerPrisoners { get; set; }
        public IList<OfficerPrisonerImportDto> Prisoners { get; internal set; }
    }
}
