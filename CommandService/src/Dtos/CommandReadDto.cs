namespace CommandService.Dtos
{
    public class CommandReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CommandLine { get; set; }
        public int PlatformId { get; set; }
    }
}