using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : BaseController
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper, IImageHelper imageHelper, SignInManager<User> signInManager) : base(userManager, mapper, imageHelper)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var roles = await _roleManager.Roles.ToListAsync();
            return View(new RoleListDto
            {
                Roles = roles
            });
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleListDto = JsonSerializer.Serialize(new RoleListDto
            {
                Roles = roles
            }, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            return Json(roleListDto);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<IActionResult> Assign(int userId)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var roles = await _roleManager.Roles.ToListAsync();
            var userRole = await UserManager.GetRolesAsync(user);
            UserRoleAssignDto userRoleAssingDto = new UserRoleAssignDto
            {
                UserId = user.Id,
                UserName = user.UserName,
            };
            foreach (var role in roles)
            {
                var roleAssignDto = new RoleAssignDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    HasRole = userRole.Contains(role.Name)
                };
                userRoleAssingDto.RoleAssignDtos.Add(roleAssignDto);
            }

            var roleListDto = JsonSerializer.Serialize(new RoleListDto
            {
                Roles = roles
            }, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            return PartialView("_RoleAssignPartial", userRoleAssingDto);
        }

        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleAssignDto userRoleAssignDto)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(userRoleAssignDto.UserId.ToString());
                foreach (var roleAssignDto in userRoleAssignDto.RoleAssignDtos)
                {
                    if (roleAssignDto.HasRole)
                    {
                        await UserManager.AddToRoleAsync(user, roleAssignDto.RoleName);
                    }
                    else
                    {
                        await UserManager.RemoveFromRoleAsync(user,roleAssignDto.RoleName);
                    }
                }
                await UserManager.UpdateSecurityStampAsync(user);
                
                var userRoleAssignAjaxViewModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
                {
                    UserDto = new UserDto
                    {
                        Message = $"{user.UserName} kullanıcısına ait rol atama işlemi başarıyla sonuçlanmıştır.",
                        ResultStatus = ResultStatus.Success,
                        User = user
                    },
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial",userRoleAssignDto),
                    UserRoleAssignDto = userRoleAssignDto
                });
                return Json(userRoleAssignAjaxViewModel);
            }
            var userRoleAssignErrorAjaxViewModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
            {
                
                RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", userRoleAssignDto),
                UserRoleAssignDto = userRoleAssignDto
            });
            return Json(userRoleAssignErrorAjaxViewModel);

        }
    }
}