using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APBD_09_s26505.Data;
using APBD_09_s26505.DTOs;

namespace APBD_09_s26505.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idStudent}/dashboard")]
    public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(int idStudent)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FirstName + " " + s.LastName,
                IsActive = s.IsActive,

                Enrollments = s.Enrollments
                    .Select(e => new StudentEnrollmentDto
                    {
                        CourseId = e.CourseId,
                        CourseCode = e.Course.Code,
                        CourseName = e.Course.Name,
                        Status = e.Status
                    })
                    .ToList(),

                Submissions = s.Submissions
                    .Select(sub => new StudentSubmissionDto
                    {
                        SubmissionId = sub.SubmissionId,
                        AssignmentId = sub.AssignmentId,
                        AssignmentTitle = sub.Assignment.Title,
                        CourseName = sub.Assignment.Course.Name,
                        RepositoryUrl = sub.RepositoryUrl,
                        Status = sub.Status,
                        Score = sub.Score
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            return NotFound($"Student with id {idStudent} was not found.");
        }

        return Ok(student);
    }
}