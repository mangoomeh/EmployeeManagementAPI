using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Helpers;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeContext _employeeContext;
        private readonly IConfiguration _configuration;
        public AuthController(EmployeeContext employeeContext, IConfiguration configuration)
        {
            _employeeContext = employeeContext;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody]LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Login and Password fields is empty!"
                    });
                }
                var user = await _employeeContext.UserModels.FirstOrDefaultAsync(a => a.Email == loginDto.Email);
                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "User Not Found!"
                    });
                }
                else
                {
                    if (!EncDescPassword.VerifyHashPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Wrong Password!"
                        });
                        
                    }
                    string token = CreateJwtToken(user);
                    return Ok(new
                    {
                        Status = 200,
                        Message = "Login Success!",
                        Token = token
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string CreateJwtToken(UserModel user)
        {
            List<Claim> claimsList = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("Role", user.RoleId.ToString()),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("veryveryverysecure"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claimsList,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpPost("reset")]
        public async Task<ActionResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Fields is empty!"
                    });
                }
                else
                {
                    var user = await _employeeContext.UserModels.FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
                    if (user == null)
                    {
                        return NotFound(new
                        {
                            Status = 404,
                            Message = "User Not Found!"
                        });
                    }
                    else
                    {
                        if (!EncDescPassword.VerifyHashPassword(resetPasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                        {
                            return BadRequest(new
                            {
                                Status = 400,
                                Message = "Wrong Password!"
                            });

                        }
                        EncDescPassword.CreateHashPassword(resetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        _employeeContext.Entry(user).State = EntityState.Modified;
                        await _employeeContext.SaveChangesAsync();
                        return Ok(new
                        {
                            StatusCode = 200,
                            Message = "Password Updated"
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
