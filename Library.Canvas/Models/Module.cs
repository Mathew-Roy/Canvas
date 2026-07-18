namespace Library.Canvas.Models
{
    public class Module
    {
        public int Id { get; set; }
        public List<ModuleItem> Content { get; set; } = new List<ModuleItem>();
    }
}