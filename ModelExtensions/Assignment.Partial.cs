using APBD_09_s26505.Models;

namespace APBD_09_s26505.Models;

public partial class Assignment
{
    public bool IsOverdue(DateTime now)
    {
        return DueDate < now;
    }
}