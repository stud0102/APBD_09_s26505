using APBD_09_s26505.Models;

namespace APBD_09_s26505.Models;

public partial class Student
{
    public string FullName => $"{FirstName} {LastName}";

    public bool HasAcademicEmail()
    {
        return Email.EndsWith("@students.example.edu", StringComparison.OrdinalIgnoreCase);
    }
}