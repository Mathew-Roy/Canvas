using System;
using System.Collections.Generic;
using Library.Canvas.Models;

namespace Library.Canvas.Services
{
    public class SemesterServiceProxy
    {
        private static SemesterServiceProxy? _instance;
        private static object _lock = new object();

        private SemesterServiceProxy()
        {
            Semesters = new List<Semester>
            {
                new Semester { Id = 1, Name = "Fall 2026",
                    StartDate = new DateTime(2026, 8, 24), EndDate = new DateTime(2026, 12, 11) },
                new Semester { Id = 2, Name = "Spring 2026",
                    StartDate = new DateTime(2026, 1, 6), EndDate = new DateTime(2026, 4, 24) }
            };
        }

        public static SemesterServiceProxy Current
        {
            get { lock (_lock) { return _instance ??= new SemesterServiceProxy(); } }
        }

        public List<Semester> Semesters { get; set; }
    }
}