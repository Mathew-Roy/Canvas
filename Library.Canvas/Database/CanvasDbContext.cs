using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Library.Canvas.Models;
using System.Linq;
using Library.Canvas.Services;

namespace Library.Canvas.Database
{
    public class CanvasDbContext
    {
        private const string ConnectionString =
            "Server=localhost;Database=Canvas;Trusted_Connection=True;TrustServerCertificate=True;";

        private static CanvasDbContext? _instance;
        public static CanvasDbContext Current => _instance ??= new CanvasDbContext();

        // ---------- Students ----------
        public List<Student> GetStudents()
        {
            var students = new List<Student>();
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Classification, Code FROM Students", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    Id = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Classification = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Code = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return students;
        }

        public void SaveStudents(List<Student> students)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = new SqlCommand("DELETE FROM Students", conn, tx))
                del.ExecuteNonQuery();

            foreach (var s in students)
            {
                using var ins = new SqlCommand(
                    "INSERT INTO Students (Id, Name, Classification, Code) VALUES (@id, @name, @class, @code)",
                    conn, tx);
                ins.Parameters.AddWithValue("@id", s.Id);
                ins.Parameters.AddWithValue("@name", (object?)s.Name ?? DBNull.Value);
                ins.Parameters.AddWithValue("@class", (object?)s.Classification ?? DBNull.Value);
                ins.Parameters.AddWithValue("@code", (object?)s.Code ?? DBNull.Value);
                ins.ExecuteNonQuery();
            }
            tx.Commit();
        }

        public List<Course> GetCourses()
        {
            var courses = new List<Course>();
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();

            using (var cmd = new SqlCommand(
                "SELECT Id, Code, Name, Description, Term, Year, Section, GradeA, GradeB, GradeC, GradeD FROM Courses", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        Id = reader.GetInt32(0),
                        Code = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Term = (Term)reader.GetInt32(4),
                        Year = reader.GetInt32(5),
                        Section = reader.IsDBNull(6) ? null : reader.GetString(6),
                        GradeA = reader.GetInt32(7),
                        GradeB = reader.GetInt32(8),
                        GradeC = reader.GetInt32(9),
                        GradeD = reader.GetInt32(10)
                    });
                }
            }

            var students = StudentServiceProxy.Current.Students;

            foreach (var course in courses)
            {
                using (var cmd = new SqlCommand("SELECT StudentId FROM Enrollments WHERE CourseId=@cid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        int sid = r.GetInt32(0);
                        var st = students.FirstOrDefault(s => s.Id == sid);
                        if (st != null) course.Roster.Add(st);
                    }
                }

                using (var cmd = new SqlCommand("SELECT Id, Name, Weight FROM AssignmentGroups WHERE CourseId=@cid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        course.AssignmentGroups.Add(new AssignmentGroup
                        {
                            Id = r.GetInt32(0),
                            Name = r.IsDBNull(1) ? null : r.GetString(1),
                            Weight = r.GetDouble(2)
                        });
                }

                using (var cmd = new SqlCommand(
                    "SELECT Id, Name, Description, AvailablePoints, DueDate, GroupId, IsQuiz, Question FROM Assignments WHERE CourseId=@cid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        course.Assignments.Add(new Assignment
                        {
                            Id = r.GetInt32(0),
                            Name = r.IsDBNull(1) ? null : r.GetString(1),
                            Description = r.IsDBNull(2) ? null : r.GetString(2),
                            AvailablePoints = r.GetInt32(3),
                            DueDate = r.GetDateTime(4),
                            GroupId = r.IsDBNull(5) ? (int?)null : r.GetInt32(5),
                            IsQuiz = r.GetBoolean(6),
                            Question = r.IsDBNull(7) ? null : r.GetString(7)
                        });
                }

                foreach (var a in course.Assignments)
                {
                    using var cmd = new SqlCommand(
                        "SELECT Id, StudentId, Content, SubmissionDate, Grade, Feedback, AttachedFileName, AttachedFilePath FROM Submissions WHERE CourseId=@cid AND AssignmentId=@aid", conn);
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    cmd.Parameters.AddWithValue("@aid", a.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        a.Submissions.Add(new Submission
                        {
                            Id = r.GetInt32(0),
                            StudentId = r.GetInt32(1),
                            AssignmentId = a.Id,
                            Content = r.IsDBNull(2) ? null : r.GetString(2),
                            SubmissionDate = r.GetDateTime(3),
                            Grade = r.IsDBNull(4) ? (double?)null : r.GetDouble(4),
                            Feedback = r.IsDBNull(5) ? null : r.GetString(5),
                            AttachedFileName = r.IsDBNull(6) ? null : r.GetString(6),
                            AttachedFilePath = r.IsDBNull(7) ? null : r.GetString(7)
                        });
                }

                var moduleIds = new List<int>();
                using (var cmd = new SqlCommand("SELECT Id FROM Modules WHERE CourseId=@cid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) moduleIds.Add(r.GetInt32(0));
                }
                foreach (var mid in moduleIds)
                {
                    var module = new Module { Id = mid };
                    using (var cmd = new SqlCommand(
                        "SELECT ItemType, Content, FileName, FilePath FROM ModuleItems WHERE CourseId=@cid AND ModuleId=@mid", conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", course.Id);
                        cmd.Parameters.AddWithValue("@mid", mid);
                        using var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string type = r.GetString(0);
                            string? content = r.IsDBNull(1) ? null : r.GetString(1);
                            string? fn = r.IsDBNull(2) ? null : r.GetString(2);
                            string? fp = r.IsDBNull(3) ? null : r.GetString(3);
                            if (type == "file") module.Content.Add(new FileItem { FileName = fn, FilePath = fp });
                            else if (type == "assignment") module.Content.Add(new AssignmentItem { Assignment = new Assignment { Name = content } });
                            else module.Content.Add(new PageItem { Content = content });
                        }
                    }
                    course.Modules.Add(module);
                }

                using (var cmd = new SqlCommand("SELECT Text FROM Announcements WHERE CourseId=@cid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", course.Id);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        course.Announcements.Add(r.IsDBNull(0) ? "" : r.GetString(0));
                }
            }

            return courses;
        }

        public void SaveCourses(List<Course> courses)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            foreach (var table in new[] { "Announcements", "ModuleItems", "Modules", "Submissions", "Assignments", "AssignmentGroups", "Enrollments", "Courses" })
                using (var del = new SqlCommand($"DELETE FROM {table}", conn, tx))
                    del.ExecuteNonQuery();

            foreach (var c in courses)
            {
                using (var ins = new SqlCommand(
                    @"INSERT INTO Courses (Id, Code, Name, Description, Term, Year, Section, GradeA, GradeB, GradeC, GradeD)
                      VALUES (@id,@code,@name,@desc,@term,@year,@section,@ga,@gb,@gc,@gd)", conn, tx))
                {
                    ins.Parameters.AddWithValue("@id", c.Id);
                    ins.Parameters.AddWithValue("@code", (object?)c.Code ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@name", (object?)c.Name ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@desc", (object?)c.Description ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@term", (int)c.Term);
                    ins.Parameters.AddWithValue("@year", c.Year);
                    ins.Parameters.AddWithValue("@section", (object?)c.Section ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@ga", c.GradeA);
                    ins.Parameters.AddWithValue("@gb", c.GradeB);
                    ins.Parameters.AddWithValue("@gc", c.GradeC);
                    ins.Parameters.AddWithValue("@gd", c.GradeD);
                    ins.ExecuteNonQuery();
                }

                foreach (var s in c.Roster)
                    using (var ins = new SqlCommand("INSERT INTO Enrollments (CourseId, StudentId) VALUES (@cid,@sid)", conn, tx))
                    { ins.Parameters.AddWithValue("@cid", c.Id); ins.Parameters.AddWithValue("@sid", s.Id); ins.ExecuteNonQuery(); }

                foreach (var g in c.AssignmentGroups)
                    using (var ins = new SqlCommand("INSERT INTO AssignmentGroups (Id, CourseId, Name, Weight) VALUES (@id,@cid,@name,@w)", conn, tx))
                    {
                        ins.Parameters.AddWithValue("@id", g.Id);
                        ins.Parameters.AddWithValue("@cid", c.Id);
                        ins.Parameters.AddWithValue("@name", (object?)g.Name ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@w", g.Weight);
                        ins.ExecuteNonQuery();
                    }

                foreach (var a in c.Assignments)
                {
                    using (var ins = new SqlCommand(
                        @"INSERT INTO Assignments (Id, CourseId, Name, Description, AvailablePoints, DueDate, GroupId, IsQuiz, Question)
                          VALUES (@id,@cid,@name,@desc,@pts,@due,@gid,@quiz,@q)", conn, tx))
                    {
                        ins.Parameters.AddWithValue("@id", a.Id);
                        ins.Parameters.AddWithValue("@cid", c.Id);
                        ins.Parameters.AddWithValue("@name", (object?)a.Name ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@desc", (object?)a.Description ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@pts", a.AvailablePoints);
                        ins.Parameters.AddWithValue("@due", a.DueDate);
                        ins.Parameters.AddWithValue("@gid", (object?)a.GroupId ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@quiz", a.IsQuiz);
                        ins.Parameters.AddWithValue("@q", (object?)a.Question ?? DBNull.Value);
                        ins.ExecuteNonQuery();
                    }

                    foreach (var sub in a.Submissions)
                        using (var ins = new SqlCommand(
                            @"INSERT INTO Submissions (CourseId, AssignmentId, StudentId, Content, SubmissionDate, Grade, Feedback, AttachedFileName, AttachedFilePath)
                              VALUES (@cid,@aid,@sid,@content,@date,@grade,@fb,@fn,@fp)", conn, tx))
                        {
                            ins.Parameters.AddWithValue("@cid", c.Id);
                            ins.Parameters.AddWithValue("@aid", a.Id);
                            ins.Parameters.AddWithValue("@sid", sub.StudentId);
                            ins.Parameters.AddWithValue("@content", (object?)sub.Content ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@date", sub.SubmissionDate);
                            ins.Parameters.AddWithValue("@grade", (object?)sub.Grade ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@fb", (object?)sub.Feedback ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@fn", (object?)sub.AttachedFileName ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@fp", (object?)sub.AttachedFilePath ?? DBNull.Value);
                            ins.ExecuteNonQuery();
                        }
                }

                foreach (var m in c.Modules)
                {
                    using (var ins = new SqlCommand("INSERT INTO Modules (Id, CourseId) VALUES (@id,@cid)", conn, tx))
                    { ins.Parameters.AddWithValue("@id", m.Id); ins.Parameters.AddWithValue("@cid", c.Id); ins.ExecuteNonQuery(); }

                    foreach (var item in m.Content)
                    {
                        string itemType; string? content = null, fn = null, fp = null;
                        if (item is FileItem fi) { itemType = "file"; fn = fi.FileName; fp = fi.FilePath; }
                        else if (item is AssignmentItem ai) { itemType = "assignment"; content = ai.Assignment?.Name; }
                        else if (item is PageItem pi) { itemType = "page"; content = pi.Content; }
                        else { itemType = "page"; content = item.Display(); }

                        using var ins = new SqlCommand(
                            "INSERT INTO ModuleItems (CourseId, ModuleId, ItemType, Content, FileName, FilePath) VALUES (@cid,@mid,@type,@content,@fn,@fp)", conn, tx);
                        ins.Parameters.AddWithValue("@cid", c.Id);
                        ins.Parameters.AddWithValue("@mid", m.Id);
                        ins.Parameters.AddWithValue("@type", itemType);
                        ins.Parameters.AddWithValue("@content", (object?)content ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@fn", (object?)fn ?? DBNull.Value);
                        ins.Parameters.AddWithValue("@fp", (object?)fp ?? DBNull.Value);
                        ins.ExecuteNonQuery();
                    }
                }

                foreach (var ann in c.Announcements)
                    using (var ins = new SqlCommand("INSERT INTO Announcements (CourseId, Text) VALUES (@cid,@text)", conn, tx))
                    { ins.Parameters.AddWithValue("@cid", c.Id); ins.Parameters.AddWithValue("@text", (object?)ann ?? DBNull.Value); ins.ExecuteNonQuery(); }
            }

            tx.Commit();
        }
    }
}