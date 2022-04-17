using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommandService.Models
{
    public class Platform
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}