using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
