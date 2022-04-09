using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class UserModel : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int DepartmentId { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int RoleId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime StartDate { get; set; }
    }
}
