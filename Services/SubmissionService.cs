using Microsoft.EntityFrameworkCore;
using APBD_09_s26505.Data;
using APBD_09_s26505.DTOs;
using APBD_09_s26505.Models;

namespace APBD_09_s26505.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string? Error, int? StatusCode, SubmissionDto? Data)> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) ||
            !dto.RepositoryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Repository URL cannot be empty and must start with https://.", 400, null);
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);

        if (student is null)
            return (false, "Student was not found.", 404, null);

        if (!student.IsActive)
            return (false, "Student is not active.", 400, null);

        var assignment = await _context.Assignments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId);

        if (assignment is null)
            return (false, "Assignment was not found.", 404, null);

        if (!assignment.IsPublished)
            return (false, "Assignment is not published.", 400, null);

        var isEnrolled = await _context.Enrollments.AnyAsync(e =>
            e.StudentId == dto.StudentId &&
            e.CourseId == assignment.CourseId &&
            (e.Status == "Active" || e.Status == "Completed"));

        if (!isEnrolled)
            return (false, "Student is not enrolled in this course.", 400, null);

        var alreadySubmitted = await _context.Submissions.AnyAsync(s =>
            s.StudentId == dto.StudentId &&
            s.AssignmentId == dto.AssignmentId);

        if (alreadySubmitted)
            return (false, "Student has already submitted this assignment.", 409, null);

        var now = DateTime.Now;

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = now,
            Status = assignment.IsOverdue(now) ? "Late" : "Submitted"
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        var result = new SubmissionDto
        {
            SubmissionId = submission.SubmissionId,
            StudentId = student.StudentId,
            StudentFullName = student.FullName,
            AssignmentId = assignment.AssignmentId,
            AssignmentTitle = assignment.Title,
            RepositoryUrl = submission.RepositoryUrl,
            SubmittedAt = submission.SubmittedAt,
            Status = submission.Status,
            Score = submission.Score,
            Feedback = submission.Feedback
        };

        return (true, null, 201, result);
    }

    public async Task<(bool Success, string? Error, int? StatusCode, SubmissionDto? Data)> GradeSubmissionAsync(
        int idSubmission,
        GradeSubmissionDto dto)
    {
        if (dto.Score < 0)
            return (false, "Score cannot be lower than 0.", 400, null);

        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.SubmissionId == idSubmission);

        if (submission is null)
            return (false, "Submission was not found.", 404, null);

        if (dto.Score > submission.Assignment.MaxPoints)
            return (false, "Score cannot be higher than assignment max points.", 400, null);

        // Change Tracker: pobieramy encję, zmieniamy właściwości i zapisujemy
        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync();

        var result = new SubmissionDto
        {
            SubmissionId = submission.SubmissionId,
            StudentId = submission.StudentId,
            StudentFullName = submission.Student.FullName,
            AssignmentId = submission.AssignmentId,
            AssignmentTitle = submission.Assignment.Title,
            RepositoryUrl = submission.RepositoryUrl,
            SubmittedAt = submission.SubmittedAt,
            Status = submission.Status,
            Score = submission.Score,
            Feedback = submission.Feedback
        };

        return (true, null, 200, result);
    }

    public async Task<(bool Success, string? Error, int? StatusCode)> DeleteSubmissionAsync(int idSubmission)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == idSubmission);

        if (submission is null)
            return (false, "Submission was not found.", 404);

        if (submission.Status == "Graded")
            return (false, "Graded submission cannot be deleted.", 400);

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();

        return (true, null, 204);
    }
}