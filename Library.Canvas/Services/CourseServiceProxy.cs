using System;
using System.Collections.Generic;
using System.Linq;
using Library.Canvas.Models;
using Library.Canvas.Database;

namespace Library.Canvas.Services
{
    public class CourseServiceProxy
    {
        private static CourseServiceProxy? _instance;
        private static object _instanceLock = new object();

        private CourseServiceProxy()
        {
            Courses = CanvasDbContext.Current.GetCourses();
            if (Courses.Count == 0)
            {
                Courses = Seed();
                CanvasDbContext.Current.SaveCourses(Courses);
            }
        }

        private List<Course> Seed()
        {
            var courses = new List<Course>
            {
                new Course { Id = 1, Code = "COP4870", Name = "Full Stack Development", Description = "Building full stack applications", Term = Term.Fall, Year = 2026, Section = "001" },
                new Course { Id = 2, Code = "COP3330", Name = "Object Oriented Programming", Description = "OOP concepts in C++", Term = Term.Spring, Year = 2026, Section = "001" }
            };

            // Demo data so the MAUI student views have something to show
            var demoStudent = StudentServiceProxy.Current.Students.FirstOrDefault();
            if (demoStudent != null)
            {
                courses[0].Roster.Add(demoStudent);
                courses[0].Announcements.Add("Welcome to the course! Your first assignment is due soon.");

                var essay = new Assignment
                {
                    Id = 1,
                    Name = "Intro Essay",
                    Description = "Write a short introduction.",
                    AvailablePoints = 100,
                    DueDate = new DateTime(2026, 9, 1),
                    GroupId = 1
                };
                essay.Submissions.Add(new Submission
                {
                    Id = 1,
                    StudentId = demoStudent.Id,
                    AssignmentId = 1,
                    Content = "My essay.",
                    SubmissionDate = new DateTime(2026, 8, 15),
                    Grade = 92
                });
                courses[0].Assignments.Add(essay);

                courses[0].AssignmentGroups.Add(new AssignmentGroup
                {
                    Id = 1,
                    Name = "Essays",
                    Weight = 100
                });

                var demoModule = new Module { Id = 1 };
                demoModule.Content.Add(new PageItem { Content = "Welcome to Full Stack Development!" });
                courses[0].Modules.Add(demoModule);
            }

            return courses;
        }

        public void Save() => CanvasDbContext.Current.SaveCourses(Courses);

        public static CourseServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new CourseServiceProxy();
                    return _instance;
                }
            }
        }

        private int _nextId = 3;
        public List<Course> Courses { get; set; }

        public void Add(Course course)
        {
            if (course.Id == 0)
                course.Id = _nextId++;
            Courses.Add(course);
        }

        public void Delete(int id)
        {
            var course = Courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
                Courses.Remove(course);
        }
    }
}