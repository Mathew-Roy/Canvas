using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Library.Canvas.Models;

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

        // ---------- Courses ----------
        public List<Course> GetCourses()
        {
            var courses = new List<Course>();
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT Id, Code, Name, Description, Term, Year, Section, GradeA, GradeB, GradeC, GradeD FROM Courses", conn);
            using var reader = cmd.ExecuteReader();
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
            return courses;
        }

        public void SaveCourses(List<Course> courses)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = new SqlCommand("DELETE FROM Courses", conn, tx))
                del.ExecuteNonQuery();

            foreach (var c in courses)
            {
                using var ins = new SqlCommand(
                    @"INSERT INTO Courses (Id, Code, Name, Description, Term, Year, Section, GradeA, GradeB, GradeC, GradeD)
                      VALUES (@id, @code, @name, @desc, @term, @year, @section, @ga, @gb, @gc, @gd)",
                    conn, tx);
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
            tx.Commit();
        }
    }
}