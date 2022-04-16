namespace PlatformService.Settings.Models
{
    public class DatabaseVariables
    {
        public string InMemoryDb { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DbName { get; set; }
        public string ConnectionString => $"Server={Server},{Port}; Initial Catalog={DbName}; User ID={User}; Password={Password}";
    }
}