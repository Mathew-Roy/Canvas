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
                    CourseMenu(course);
                else
                    Console.WriteLine("No course found with that ID.");
            }
            else
            {
                Console.WriteLine("Invalid ID entered.");
            }
        }

        static void CourseMenu(Course course)
        {
            bool inCourseMenu = true;
            while (inCourseMenu)
            {
                Console.WriteLine($"\n=== {course.Name} ({course.Code}) ===");
                Console.WriteLine("1. View Assignments");
                Console.WriteLine("2. Add an Assignment");
                Console.WriteLine("3. Delete an Assignment");
                Console.WriteLine("4. Edit an Assignment");
                Console.WriteLine("5. View Modules");
                Console.WriteLine("6. Add a Module");
                Console.WriteLine("7. View Roster");
                Console.WriteLine("8. Delete this Course");
                Console.WriteLine("9. Update Course Description");
                Console.WriteLine("10. Remove Content from a Module");
                Console.WriteLine("11. Modify Content in a Module");
                Console.WriteLine("12. Back to Teacher Menu");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        Console.WriteLine("\n=== Assignments ===");
                        if (course.Assignments.Count == 0)
                            Console.WriteLine("No assignments yet.");
                        else
                            course.Assignments.ForEach(a => Console.WriteLine($"[{a.Id}] {a.Name} - {a.AvailablePoints} pts - Due: {a.DueDate:MM/dd/yyyy}"));
                        break;
                    case "2":
                        AddAssignment(course);
                        break;
                    case "3":
                        DeleteAssignment(course);
                        break;
                    case "4":
                        EditAssignment(course);
                        break;
                    case "5":
                        Console.WriteLine("\n=== Modules ===");
                        if (course.Modules.Count == 0)
                            Console.WriteLine("No modules yet.");
                        else
                            course.Modules.ForEach(m => Console.WriteLine($"[{m.Id}] Module with {m.Content.Count} item(s)"));
                        break;
                    case "6":
                        AddModule(course);
                        break;
                    case "7":
                        Console.WriteLine("\n=== Roster ===");
                        if (course.Roster.Count == 0)
                            Console.WriteLine("No students enrolled.");
                        else
                            course.Roster.ForEach(s => Console.WriteLine($"[{s.Id}] {s.Name} - {s.Classification}"));
                        break;
                    case "8":
                        CourseServiceProxy.Current.Delete(course.Id);
                        Console.WriteLine($"Course '{course.Name}' deleted.");
                        inCourseMenu = false;
                        break;
                    case "9":
                        Console.Write("Enter new description: ");
                        course.Description = Console.ReadLine();
                        Console.WriteLine("Description updated!");
                        break;
                    case "10":
                        RemoveModuleContent(course);
                        break;
                    case "11":
                        ModifyModuleContent(course);
                        break;
                    case "12":
                        inCourseMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void AddAssignment(Course course)
        {
            Console.Write("Enter assignment name: ");
            var name = Console.ReadLine();
            Console.Write("Enter description: ");
            var description = Console.ReadLine();
            Console.Write("Enter available points: ");
            int.TryParse(Console.ReadLine(), out int points);
            Console.Write("Enter due date (MM/DD/YYYY): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime dueDate);

            var assignment = new Assignment
            {
                Id = course.Assignments.Count + 1,
                Name = name,
                Description = description,
                AvailablePoints = points,
                DueDate = dueDate
            };

            course.Assignments.Add(assignment);
            Console.WriteLine($"Assignment '{name}' added!");
        }

        static void DeleteAssignment(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments to delete.");
                return;
            }

            course.Assignments.ForEach(a => Console.WriteLine($"[{a.Id}] {a.Name}"));
            Console.Write("Enter assignment ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
                if (assignment != null)
                {
                    assignment.Submissions.Clear();
                    course.Assignments.Remove(assignment);
                    Console.WriteLine("Assignment and all its submissions deleted!");
                }
                else
                    Console.WriteLine("Assignment not found.");
            }
        }

        static void EditAssignment(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments to edit.");
                return;
            }

            course.Assignments.ForEach(a => Console.WriteLine($"[{a.Id}] {a.Name}"));
            Console.Write("Enter assignment ID to edit: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
                if (assignment != null)
                {
                    Console.Write("Enter new name (or press Enter to keep current): ");
                    var name = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name)) assignment.Name = name;

                    Console.Write("Enter new description (or press Enter to keep current): ");
                    var desc = Console.ReadLine();
                    if (!string.IsNullOrEmpty(desc)) assignment.Description = desc;

                    Console.Write("Enter new points (or press Enter to keep current): ");
                    var pts = Console.ReadLine();
                    if (!string.IsNullOrEmpty(pts))
                    {
                        if (int.TryParse(pts, out int newPoints))
                            assignment.AvailablePoints = newPoints;
                    }

                    Console.WriteLine("Assignment updated!");
                }
                else
                    Console.WriteLine("Assignment not found.");
            }
        }

        static void AddModule(Course course)
        {
            var module = new Module
            {
                Id = course.Modules.Count + 1
            };

            Console.Write("Enter module content (or press Enter to leave empty): ");
            var content = Console.ReadLine();
            if (!string.IsNullOrEmpty(content))
                module.Content.Add(content);

            course.Modules.Add(module);
            Console.WriteLine($"Module added with ID {module.Id}!");
        }

        static void RemoveModuleContent(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules available.");
                return;
            }

            course.Modules.ForEach(m => Console.WriteLine($"[{m.Id}] Module with {m.Content.Count} item(s)"));
            Console.Write("Enter module ID: ");
            if (int.TryParse(Console.ReadLine(), out int moduleId))
            {
                var module = course.Modules.FirstOrDefault(m => m.Id == moduleId);
                if (module == null)
                {
                    Console.WriteLine("Module not found.");
                    return;
                }

                if (module.Content.Count == 0)
                {
                    Console.WriteLine("No content in this module.");
                    return;
                }

                for (int i = 0; i < module.Content.Count; i++)
                    Console.WriteLine($"[{i}] {module.Content[i]}");

                Console.Write("Enter content index to remove: ");
                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index >= 0 && index < module.Content.Count)
                    {
                        module.Content.RemoveAt(index);
                        Console.WriteLine("Content removed!");
                    }
                    else
                        Console.WriteLine("Invalid index.");
                }
            }
        }
        static void ModifyModuleContent(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules available.");
                return;
            }

            course.Modules.ForEach(m => Console.WriteLine($"[{m.Id}] Module with {m.Content.Count} item(s)"));
            Console.Write("Enter module ID: ");
            if (int.TryParse(Console.ReadLine(), out int moduleId))
            {
                var module = course.Modules.FirstOrDefault(m => m.Id == moduleId);
                if (module == null)
                {
                    Console.WriteLine("Module not found.");
                    return;
                }

                if (module.Content.Count == 0)
                {
                    Console.WriteLine("No content in this module.");
                    return;
                }

                for (int i = 0; i < module.Content.Count; i++)
                    Console.WriteLine($"[{i}] {module.Content[i]}");

                Console.Write("Enter content index to modify: ");
                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index >= 0 && index < module.Content.Count)
                    {
                        Console.Write("Enter new content: ");
                        var newContent = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newContent))
                        {
                            module.Content[index] = newContent;
                            Console.WriteLine("Content updated!");
                        }
                        else
                            Console.WriteLine("Content cannot be empty.");
                    }
                    else
                        Console.WriteLine("Invalid index.");
                }
            }
        }
    }
    
}