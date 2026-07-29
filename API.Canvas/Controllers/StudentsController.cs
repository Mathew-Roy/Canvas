using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace API.Canvas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        // GET api/students
        [HttpGet]
        public ActionResult<List<Student>> GetAll()
        {
            return StudentServiceProxy.Current.Students;
        }

        // GET api/students/5
        [HttpGet("{id}")]
        public ActionResult<Student> Get(int id)
        {
            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            return student;
        }

        // POST api/students
        [HttpPost]
        public ActionResult<Student> Create(Student student)
        {
            var students = StudentServiceProxy.Current.Students;
            student.Id = students.Any() ? students.Max(s => s.Id) + 1 : 1;
            students.Add(student);
            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }

        // PUT api/students/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Student updated)
        {
            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();

            student.Name = updated.Name;
            student.Classification = updated.Classification;
            return NoContent();
        }

        // DELETE api/students/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var students = StudentServiceProxy.Current.Students;
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();

            students.Remove(student);
            return NoContent();
        }
    }
}