namespace APBD_09_s26505.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Credits { get; set; }
    public int AssignmentsCount { get; set; }
}