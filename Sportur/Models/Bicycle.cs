namespace Sportur.Models
{
    public class Bicycle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int GearCount { get; set; }
        public Brake Brake { get; set; }
        public Bicycle() { }
    }
}
