using System.ComponentModel.DataAnnotations;

namespace CommandService.Dtos
{
    public class CommandCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string CommandLine { get; set; }
    }
}