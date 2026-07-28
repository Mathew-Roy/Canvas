using System;

namespace Library.Canvas.Models
{
    public class Semester
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(4);
    }
}