using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class LeaveModel : BaseModel
    {
        [Key]
        public int LeaveId { get; set; }
        public int UserId { get; set; }
        public DateTime LStartDate { get; set; }
        public DateTime LEndDate { get; set; }
        public int Count { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
    }
}
