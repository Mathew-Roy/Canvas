using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.Canvas.Models;
using Library.Canvas.Services;

namespace API.Canvas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        // GET api/courses
        [HttpGet]
        public ActionResult<List<Course>> GetAll()
        {
            return CourseServiceProxy.Current.Courses;
        }

        // GET api/courses/5
        [HttpGet("{id}")]
        public ActionResult<Course> Get(int id)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            return course;
        }

        // POST api/courses
        [HttpPost]
        public ActionResult<Course> Create(Course course)
        {
            CourseServiceProxy.Current.Add(course);
            return CreatedAtAction(nameof(Get), new { id = course.Id }, course);
        }

        // PUT api/courses/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Course updated)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            course.Name = updated.Name;
            course.Code = updated.Code;
            course.Description = updated.Description;
            course.Term = updated.Term;
            course.Year = updated.Year;
            course.Section = updated.Section;
            return NoContent();
        }

        // DELETE api/courses/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            CourseServiceProxy.Current.Delete(id);
            return NoContent();
        }
    }
}