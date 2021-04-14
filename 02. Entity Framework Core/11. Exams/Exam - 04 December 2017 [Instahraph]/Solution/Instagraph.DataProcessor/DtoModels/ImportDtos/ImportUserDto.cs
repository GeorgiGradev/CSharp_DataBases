using System.ComponentModel.DataAnnotations;


namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    public class ImportUserDto
    {
        [MaxLength(30)]
        [Required]
        public string Username { get; set; }

        [MaxLength(30)]
        [Required]
        public string Password { get; set; }

        [Required]
        public string ProfilePicture { get; set; }
    }
}
