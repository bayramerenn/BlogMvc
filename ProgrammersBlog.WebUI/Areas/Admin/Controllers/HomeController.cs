using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IArticleService _articleService;
       

        public HomeController(ICategoryService categoryService, ICommentService commentService, IArticleService articleService, UserManager<User> userManager, IImageHelper imageHelper, IMapper mapper) : base(userManager, mapper, imageHelper)
        {
            _categoryService = categoryService;
            _commentService = commentService;
            _articleService = articleService;
          
        }
        [Authorize(Roles = "SuperAdmin,AdminArea.Home.Read")]
        [Area("Admin")]
      
        public async Task<IActionResult> Index()
        {
            var categoriesCountResult = await _categoryService.CountByNonDeletedAsync();
            var articlesCountResult = await _articleService.CountByNonDeletedAsync();
            var commentCountResult = await _commentService.CountByNonDeletedAsync();
            var userCountResult = await UserManager.Users.CountAsync();
            var articlesResult = await _articleService.GetAllAsync(null);
            if (categoriesCountResult.ResultStatus == ResultStatus.Success && articlesCountResult.ResultStatus == ResultStatus.Success
                && commentCountResult.ResultStatus == ResultStatus.Success && userCountResult > -1 && articlesCountResult.ResultStatus == ResultStatus.Success)
            {
                return View( new DashboardViewModel
                {
                    ArticleListDto = articlesResult.Data,
                    ArticlesCount = articlesCountResult.Data,
                    CategoriesCount = categoriesCountResult.Data,
                    CommentsCount = commentCountResult.Data,
                    UsersCount = userCountResult

                });
            }
            return NotFound();
        }
    }
}
