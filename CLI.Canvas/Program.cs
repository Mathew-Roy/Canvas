using Library.Canvas.Models;

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
                Console.WriteLine("1. Add a Course");
                Console.WriteLine("2. Delete a Course");
                Console.WriteLine("3. Update Course Description");
                Console.WriteLine("4. Add an Assignment");
                Console.WriteLine("5. Delete an Assignment");
                Console.WriteLine("6. Edit an Assignment");
                Console.WriteLine("7. Add a Module");
                Console.WriteLine("8. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        Console.WriteLine("Adding a course...");
                        break;
                    case "2":
                        Console.WriteLine("Deleting a course...");
                        break;
                    case "3":
                        Console.WriteLine("Updating course description...");
                        break;
                    case "4":
                        Console.WriteLine("Adding an assignment...");
                        break;
                    case "5":
                        Console.WriteLine("Deleting an assignment...");
                        break;
                    case "6":
                        Console.WriteLine("Editing an assignment...");
                        break;
                    case "7":
                        Console.WriteLine("Adding a module...");
                        break;
                    case "8":
                        inTeacherMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}