using AutoMapper;
using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Helpers;
using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _userContext;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private IWebHostEnvironment _env;
        public EmployeeController(EmployeeContext context,IMapper mapper,IMailService mailService, IWebHostEnvironment env)
        {
            _userContext = context;
            _mapper = mapper;
            _mailService = mailService;
            _env = env;
        }
        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetAllEmployees()
        {
            var employeeList = await _userContext.UserModels.ToListAsync();
            var mappedEmployeeList = _mapper.Map<List<UserDto>>(employeeList);
            return Ok(new ResponseDto<UserDto>
            {
                StatusCode = 200,
                Message = "Success",
                Result = mappedEmployeeList
            });
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetEmployee(int id)
        {
            var user = await _userContext.UserModels.FindAsync(id);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found!"
                });
            }
            var mappedUser = _mapper.Map<UserDto>(user);
            return Ok(new 
            {
                StatusCode=200,
                Message="Success",
                Result=mappedUser
            });
        }

        // POST api/<EmployeeController>
        [HttpPost("add")]
        public async Task<ActionResult<UserDto>> AddEmployee()
        {
            IFormFileCollection req = Request.Form.Files;
            var files = req;
            var userDtoString = Request.Form["EmployeeDetails"];
            var userDtoObj = JsonConvert.DeserializeObject<UserDto>(userDtoString);
            var uploads = Path.Combine(_env.WebRootPath, "EmployeeImages");

            if (userDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please send employee data to add!"
                });
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Split(";");

                    var filepath = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    userDtoObj.ProfileImageUrl = $"EmployeeImages/{file.FileName}";
                }
            }

            var userObj = _mapper.Map<UserModel>(userDtoObj);
            userObj.CreateDate = DateTime.Now;
            userObj.CreatedBy = 3;
            EncDescPassword.CreateHashPassword(userDtoObj.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userObj.PasswordHash = passwordHash;
            userObj.PasswordSalt = passwordSalt;
            Random r = new Random();
            int randonData = r.Next(1000,9999);
            userObj.UserName = userObj.FirstName+"."+userObj.LastName+randonData;
            await _userContext.UserModels.AddAsync(userObj);

            await _userContext.SaveChangesAsync();
            //mailrequest mailrequest = new mailrequest();
            //mailrequest.toemail = userobj.email;
            //mailrequest.subject = "new user created";
            //mailrequest.body = "hi " + userobj.firstname + " " + userobj.lastname + "," + "<br>" +
            //                    "<p>your profile has been created in our company</p>"
            //                    + "below is your login credentials for accessing the employee portal<br>"
            //                    + "your email : " + "<strong>" + userdtoobj.email + "</strong><br>"
            //                    + "<br>" + "your username : " + "<strong>" + userobj.username + "</strong><br>"
            //                    + "your password : " + "<strong>" + userdtoobj.password + "</strong><br>"

            //                    + "kindly go on below link to reset your password<br>" +
            //                    "<a href='https://localhost:44314/auth/reset'>reset password!</a>" +
            //                    "<br><br>kind regards,<br>sashikumar yadav";

            //await _mailservice.sendemailasync(mailrequest);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Added!"
            });
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("update")]
        public async Task<ActionResult<UserDto>> UpdateEmployee()
        {
            IFormFileCollection req = Request.Form.Files;
            var files = req;
            var userDtoString = Request.Form["EmployeeDetails"];
            var userDtoObj = JsonConvert.DeserializeObject<UserDto>(userDtoString);
            var uploads = Path.Combine(_env.WebRootPath, "EmployeeImages");

            if (userDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please send employee data to update!"
                });
            }
            var isUserExist = await _userContext.UserModels.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userDtoObj.Id);
            if (isUserExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found!"
                });
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Split(";");

                    var filepath = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    userDtoObj.ProfileImageUrl = $"EmployeeImages/{file.FileName}";
                }
            }

            var userObj = _mapper.Map<UserModel>(userDtoObj);
            userObj.UpdateDate = DateTime.Now;
            userObj.UpdatedBy = 1;
            _userContext.Entry(userObj).State = EntityState.Modified;
            await _userContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Updated!"
            });

        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDto>> DeleteEmployee(int id)
        {
            var user = await _userContext.UserModels.FindAsync(id);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Employee not found!"
                });
            }
            _userContext.UserModels.Remove(user);
            await _userContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Deleted!"
            });
        }
    }
}
