using System;

namespace Library.Canvas.Models
{
    public class Comment
    {
        public string? Author { get; set; }
        public string? Text { get; set; }
        public DateTime PostedAt { get; set; }
        public string Display => $"{Author}: {Text}";
    }
}