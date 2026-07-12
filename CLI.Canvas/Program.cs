using Library.Canvas.Models;
using Library.Canvas.Services;

namespace CLI.Canvas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Welcome to Canvas ===");
                Console.WriteLine("Are you a student or a teacher?");
                Console.WriteLine("1. Student");
                Console.WriteLine("2. Teacher");
                Console.WriteLine("3. Exit");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        Console.WriteLine("\nYou are logged in as a Student.");
                        break;
                    case "2":
                        TeacherMenu();
                        break;
                    case "3":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void TeacherMenu()
        {
            bool inTeacherMenu = true;
            while (inTeacherMenu)
            {
                Console.WriteLine("\n=== Teacher Menu ===");
                Console.WriteLine("1. Add a new course");
                Console.WriteLine("2. Select an existing course");
                Console.WriteLine("3. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        SelectCourse();
                        break;
                    case "3":
                        inTeacherMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void AddCourse()
        {
            Console.Write("Enter course name: ");
            var name = Console.ReadLine();
            Console.Write("Enter course code: ");
            var code = Console.ReadLine();
            Console.Write("Enter course description: ");
            var description = Console.ReadLine();

            var course = new Course
            {
                Name = name,
                Code = code,
                Description = description
            };

            CourseServiceProxy.Current.Add(course);
            Console.WriteLine($"\nCourse '{name}' added with ID {course.Id}!");
        }

        static void SelectCourse()
        {
            var courses = CourseServiceProxy.Current.Courses;
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses available.");
                return;
            }

            Console.WriteLine("\n=== Available Courses ===");
            courses.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Code} - {c.Name}"));

            Console.Write("\nEnter course ID: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int id))
            {
                var course = courses.FirstOrDefault(c => c.Id == id);
                if (course != null)
                    Console.WriteLine($"\nSelected: {course.Name} - {course.Description}");
                else
                    Console.WriteLine("No course found with that ID.");
            }
            else
            {
                Console.WriteLine("Invalid ID entered.");
            }
        }
    }
}