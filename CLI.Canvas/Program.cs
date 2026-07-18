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
                        SelectStudentProxy();
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
                Console.WriteLine("3. View courses by semester");
                Console.WriteLine("4. Back to Main Menu");
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
                        ViewCoursesBySemester();
                    break;
                    case "4":
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

            Term term;
            while (true)
            {
                Console.Write("Enter term (Spring, Summer, Fall, Winter): ");
                if (Enum.TryParse(Console.ReadLine(), true, out term))
                    break;
                Console.WriteLine("Invalid term. Try again.");
            }

            Console.Write("Enter year (e.g. 2026): ");
            int.TryParse(Console.ReadLine(), out int year);

            Console.Write("Enter section (e.g. 001): ");
            var section = Console.ReadLine();

            var course = new Course
            {
                Name = name,
                Code = code,
                Description = description,
                Term = term,
                Year = year,
                Section = section
            };

            CourseServiceProxy.Current.Add(course);
            Console.WriteLine($"\nCourse '{name}' ({course.Semester}, Section {course.Section}) added with ID {course.Id}!");
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
            courses.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Code} - {c.Name} ({c.Semester}, Sec {c.Section})"));

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

        static void ViewCoursesBySemester()
        {
            var courses = CourseServiceProxy.Current.Courses;
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses available.");
                return;
            }

            var grouped = courses
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Term)
                .GroupBy(c => c.Semester);

            Console.WriteLine("\n=== Courses by Semester ===");
            foreach (var group in grouped)
            {
                Console.WriteLine($"\n{group.Key}:");
                foreach (var c in group)
                    Console.WriteLine($"  [{c.Id}] {c.Code} - {c.Name} (Sec {c.Section})");
            }
        }

        static void CourseMenu(Course course)
        {
            bool inCourseMenu = true;
            while (inCourseMenu)
            {
                Console.WriteLine($"\n=== {course.Name} ({course.Code}) - {course.Semester}, Section {course.Section} ===");
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
                Console.WriteLine("12. Enroll a Student");
                Console.WriteLine("13. Unenroll a Student");
                Console.WriteLine("14. Grade a Submission");
                Console.WriteLine("15. Back to Teacher Menu");
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
                        EnrollStudent(course);
                        break;
                    case "13":
                        UnenrollStudent(course);
                        break;
                    case "14":
                        GradeSubmission(course);
                        break;
                    case "15":
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
        static void SelectStudentProxy()
        {
            var students = StudentServiceProxy.Current.Students;
            if (students.Count == 0)
            {
                Console.WriteLine("No students available.");
                return;
            }

            Console.WriteLine("\n=== Select Student ===");
            students.ForEach(s => Console.WriteLine($"[{s.Id}] {s.Name} - {s.Classification} ({s.Code})"));

            Console.Write("\nEnter student ID to proxy as: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int id))
            {
                var student = students.FirstOrDefault(s => s.Id == id);
                if (student != null)
                {
                    Console.WriteLine($"\nLogged in as {student.Name} ({student.Classification})");
                    StudentMenu(student);
                }
                else
                    Console.WriteLine("No student found with that ID.");
            }
            else
                Console.WriteLine("Invalid ID entered.");
        }

        static void StudentMenu(Student student)
        {
            bool inStudentMenu = true;
            while (inStudentMenu)
            {
                Console.WriteLine($"\n=== Student Menu - {student.Name} ===");
                Console.WriteLine("1. View My Courses");
                Console.WriteLine("2. Submit an Assignment");
                Console.WriteLine("3. View My Submissions");
                Console.WriteLine("4. Unenroll from a Course");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        ViewStudentCourses(student);
                        break;
                    case "2":
                        SubmitAssignment(student);
                        break;
                    case "3":
                        ViewSubmissions(student);
                        break;
                    case "4":
                        StudentUnenroll(student);
                        break;
                    case "5":
                        inStudentMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void SubmitAssignment(Student student)
        {
            var courses = CourseServiceProxy.Current.Courses
                .Where(c => c.Roster.Any(s => s.Id == student.Id))
                .ToList();

            if (courses.Count == 0)
            {
                Console.WriteLine("You are not enrolled in any courses.");
                return;
            }

            Console.WriteLine("\n=== Your Courses ===");
            courses.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Code} - {c.Name}"));
            Console.Write("Enter course ID: ");

            if (int.TryParse(Console.ReadLine(), out int courseId))
            {
                var course = courses.FirstOrDefault(c => c.Id == courseId);
                if (course == null)
                {
                    Console.WriteLine("Course not found.");
                    return;
                }

                if (course.Assignments.Count == 0)
                {
                    Console.WriteLine("No assignments in this course.");
                    return;
                }

                Console.WriteLine("\n=== Assignments ===");
                course.Assignments.ForEach(a => Console.WriteLine($"[{a.Id}] {a.Name} - Due: {a.DueDate:MM/dd/yyyy}"));
                Console.Write("Enter assignment ID: ");

                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                {
                    var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
                    if (assignment == null)
                    {
                        Console.WriteLine("Assignment not found.");
                        return;
                    }

                    Console.Write("Enter your submission content: ");
                    var content = Console.ReadLine();

                    var submission = new Submission
                    {
                        Id = assignment.Submissions.Count + 1,
                        StudentId = student.Id,
                        AssignmentId = assignment.Id,
                        Content = content,
                        SubmissionDate = DateTime.Now
                    };

                    assignment.Submissions.Add(submission);
                    Console.WriteLine("Submission submitted successfully!");
                }
            }
        }

        static void ViewSubmissions(Student student)
        {
            var courses = CourseServiceProxy.Current.Courses
                .Where(c => c.Roster.Any(s => s.Id == student.Id))
                .ToList();

            bool found = false;
            foreach (var course in courses)
            {
                foreach (var assignment in course.Assignments)
                {
                    var mySubmissions = assignment.Submissions
                        .Where(s => s.StudentId == student.Id)
                        .ToList();

                    if (mySubmissions.Count > 0)
                    {
                        found = true;
                        Console.WriteLine($"\n{course.Name} - {assignment.Name}:");
                        mySubmissions.ForEach(s => Console.WriteLine($"  Submitted: {s.SubmissionDate:MM/dd/yyyy} - {s.Content}"));
                    }
                }
            }

            if (!found)
                Console.WriteLine("You have no submissions yet.");
        }
        static void EnrollStudent(Course course)
        {
            var students = StudentServiceProxy.Current.Students;
            if (students.Count == 0)
            {
                Console.WriteLine("No students available.");
                return;
            }

            Console.WriteLine("\n=== Available Students ===");
            students.ForEach(s => Console.WriteLine($"[{s.Id}] {s.Name} - {s.Classification}"));

            Console.Write("Enter student ID to enroll: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var student = students.FirstOrDefault(s => s.Id == id);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }
                if (course.Roster.Any(s => s.Id == id))
                {
                    Console.WriteLine("Student is already enrolled.");
                    return;
                }
                course.Roster.Add(student);
                Console.WriteLine($"{student.Name} enrolled successfully!");
            }
            else
                Console.WriteLine("Invalid ID.");
        }

        static void UnenrollStudent(Course course)
        {
            if (course.Roster.Count == 0)
            {
                Console.WriteLine("No students enrolled.");
                return;
            }

            Console.WriteLine("\n=== Enrolled Students ===");
            course.Roster.ForEach(s => Console.WriteLine($"[{s.Id}] {s.Name} - {s.Classification}"));

            Console.Write("Enter student ID to unenroll: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var student = course.Roster.FirstOrDefault(s => s.Id == id);
                if (student != null)
                {
                    course.Roster.Remove(student);
                    Console.WriteLine($"{student.Name} unenrolled successfully!");
                }
                else
                    Console.WriteLine("Student not found in roster.");
            }
            else
                Console.WriteLine("Invalid ID.");
        }
        static void GradeSubmission(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments in this course.");
                return;
            }

            course.Assignments.ForEach(a => Console.WriteLine($"[{a.Id}] {a.Name} - {a.Submissions.Count} submission(s)"));
            Console.Write("Enter assignment ID to grade: ");

            if (int.TryParse(Console.ReadLine(), out int assignmentId))
            {
                var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
                if (assignment == null)
                {
                    Console.WriteLine("Assignment not found.");
                    return;
                }

                if (assignment.Submissions.Count == 0)
                {
                    Console.WriteLine("No submissions for this assignment.");
                    return;
                }

                assignment.Submissions.ForEach(s => Console.WriteLine($"[{s.Id}] Student {s.StudentId} - {s.Content}"));
                Console.Write("Enter submission ID to grade: ");

                if (int.TryParse(Console.ReadLine(), out int submissionId))
                {
                    var submission = assignment.Submissions.FirstOrDefault(s => s.Id == submissionId);
                    if (submission == null)
                    {
                        Console.WriteLine("Submission not found.");
                        return;
                    }

                    Console.Write($"Enter grade (0-{assignment.AvailablePoints}): ");
                    if (int.TryParse(Console.ReadLine(), out int grade))
                    {
                        Console.WriteLine($"Submission graded {grade}/{assignment.AvailablePoints}!");
                    }
                }
            }
        }
        static void StudentUnenroll(Student student)
        {
            var courses = CourseServiceProxy.Current.Courses
                .Where(c => c.Roster.Any(s => s.Id == student.Id))
                .ToList();

            if (courses.Count == 0)
            {
                Console.WriteLine("You are not enrolled in any courses.");
                return;
            }

            courses.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Code} - {c.Name}"));
            Console.Write("Enter course ID to unenroll from: ");

            if (int.TryParse(Console.ReadLine(), out int courseId))
            {
                var course = courses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                {
                    var studentInRoster = course.Roster.FirstOrDefault(s => s.Id == student.Id);
                    if (studentInRoster != null)
                    {
                        course.Roster.Remove(studentInRoster);
                        Console.WriteLine($"You have been unenrolled from {course.Name}.");
                    }
                }
                else
                    Console.WriteLine("Course not found.");
            }
        }
        static void ViewStudentCourses(Student student)
        {
            var courses = CourseServiceProxy.Current.Courses
                .Where(c => c.Roster.Any(s => s.Id == student.Id))
                .ToList();

            if (courses.Count == 0)
            {
                Console.WriteLine("You are not enrolled in any courses.");
                return;
            }

            Console.WriteLine("\n=== Your Courses ===");
            courses.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Code} - {c.Name}"));

            Console.Write("\nEnter course ID to view details (or 0 to go back): ");
            if (int.TryParse(Console.ReadLine(), out int courseId) && courseId != 0)
            {
                var course = courses.FirstOrDefault(c => c.Id == courseId);
                if (course != null)
                    StudentCourseView(student, course);
                else
                    Console.WriteLine("Course not found.");
            }
        }

        static void StudentCourseView(Student student, Course course)
        {
            bool inCourseView = true;
            while (inCourseView)
            {
                Console.WriteLine($"\n=== {course.Name} ({course.Code}) ===");
                Console.WriteLine("1. View Assignments");
                Console.WriteLine("2. View Course Schedule");
                Console.WriteLine("3. View Modules");
                Console.WriteLine("4. View Other Students");
                Console.WriteLine("5. Back to My Courses");
                Console.Write("\nEnter your choice: ");

                var selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        Console.WriteLine("\n=== Assignments ===");
                        if (course.Assignments.Count == 0)
                            Console.WriteLine("No assignments yet.");
                        else
                            course.Assignments.ForEach(a => Console.WriteLine(
                                $"[{a.Id}] {a.Name} - {a.AvailablePoints} pts - Due: {a.DueDate:MM/dd/yyyy}\n    {a.Description}"));
                        break;
                    case "2":
                        Console.WriteLine("\n=== Course Schedule ===");
                        if (course.Assignments.Count == 0)
                            Console.WriteLine("No assignments scheduled.");
                        else
                        {
                            var sorted = course.Assignments.OrderBy(a => a.DueDate).ToList();
                            sorted.ForEach(a => Console.WriteLine(
                                $"Due {a.DueDate:MM/dd/yyyy} - {a.Name} ({a.AvailablePoints} pts)"));
                        }
                        break;
                    case "3":
                        Console.WriteLine("\n=== Modules ===");
                        if (course.Modules.Count == 0)
                            Console.WriteLine("No modules yet.");
                        else
                        {
                            foreach (var module in course.Modules)
                            {
                                Console.WriteLine($"\nModule {module.Id}:");
                                if (module.Content.Count == 0)
                                    Console.WriteLine("  No content yet.");
                                else
                                    module.Content.ForEach(c => Console.WriteLine($"  - {c}"));
                            }
                        }
                        break;
                    case "4":
                        Console.WriteLine("\n=== Students in this Course ===");
                        if (course.Roster.Count == 0)
                            Console.WriteLine("No students enrolled.");
                        else
                            course.Roster.ForEach(s => Console.WriteLine(
                                $"[{s.Id}] {s.Name} - {s.Classification}"));
                        break;
                    case "5":
                        inCourseView = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
    
}