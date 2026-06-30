namespace APBD_09_s26505.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }

    public List<StudentEnrollmentDto> Enrollments { get; set; } = new();
    public List<StudentSubmissionDto> Submissions { get; set; } = new();
}

public class StudentEnrollmentDto
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public class StudentSubmissionDto
{
    public int SubmissionId { get; set; }
    public int AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string RepositoryUrl { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int? Score { get; set; }
}