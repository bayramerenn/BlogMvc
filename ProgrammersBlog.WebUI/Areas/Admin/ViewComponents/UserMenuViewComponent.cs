using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.ViewComponents
{
    public class UserMenuViewComponent:ViewComponent
    {

        private readonly UserManager<User> _userManager;

        public UserMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new UserViewModel
            {
                User = user,
            });
        }
    }
}
