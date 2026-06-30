using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APBD_09_s26505.Data;
using APBD_09_s26505.DTOs;

namespace APBD_09_s26505.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public CoursesController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses([FromQuery] bool activeOnly = true)
    {
        var query = _context.Courses
            .AsNoTracking()
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        var courses = await query
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Code = c.Code,
                Name = c.Name,
                Credits = c.Credits,
                AssignmentsCount = c.Assignments.Count
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{idCourse}/assignments")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetCourseAssignments(
        int idCourse,
        [FromQuery] bool publishedOnly = true)
    {
        var courseExists = await _context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.CourseId == idCourse);

        if (!courseExists)
        {
            return NotFound($"Course with id {idCourse} was not found.");
        }

        var query = _context.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == idCourse);

        if (publishedOnly)
        {
            query = query.Where(a => a.IsPublished);
        }

        var assignments = await query
            .Select(a => new AssignmentDto
            {
                AssignmentId = a.AssignmentId,
                Title = a.Title,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints,
                IsPublished = a.IsPublished,
                SubmissionsCount = a.Submissions.Count
            })
            .ToListAsync();

        return Ok(assignments);
    }
}