using AutoMapper;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<DepartmentModel, DepartmentDto>();
            CreateMap<DepartmentDto, DepartmentModel>();
            CreateMap<RoleDto, RoleModel>();
            CreateMap<RoleModel, RoleDto>();
            CreateMap <UserDto, UserModel> ();
            CreateMap<UserModel, UserDto>();
            CreateMap<LeaveDto,LeaveModel>();
            CreateMap<LeaveModel, LeaveDto>();
        }
    }
}
