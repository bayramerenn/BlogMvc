using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplextTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using ProgrammersBlog.WebUI.Extensions;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IImageHelper _imageHelper;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IImageHelper imageHelper, IMapper mapper)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _imageHelper = imageHelper;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _articleService.GetAllByNonDeletedAsync();
            if (result.ResultStatus == ResultStatus.Success)
            {
                return View(result.Data);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var result = await _categoryService.GetAllByNonDeletedAsync();
            if (result.ResultStatus == ResultStatus.Success)
            {
                return View(new ArticleAddViewModel
                {
                    Categories = result.Data.Categories,
                });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ArticleAddViewModel articleAddViewModel)
        {
            var resultCategories = await _categoryService.GetAllByNonDeletedAsync();

            if (ModelState.IsValid)
            {
                var articleAddDto = _mapper.Map<ArticleAddDto>(articleAddViewModel);
                var imageResult = await _imageHelper.Upload(articleAddViewModel.Title, articleAddViewModel.ThumbnailFile, PictureType.Post);
                articleAddDto.Thumbnail = imageResult.Data.FullName;
                var result = await _articleService.AddAsync(articleAddDto, "Bayram EREN");
                if (result.ResultStatus == ResultStatus.Success)
                {
                    TempData.Add("SuccessMessage", result.Message);
                    return RedirectToAction("Index","Article");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,result.Message);
                    articleAddViewModel.Categories = resultCategories.Data.Categories;
                    return View(articleAddViewModel);
                }
            }
            articleAddViewModel.Categories = resultCategories.Data.Categories;
            return View(articleAddViewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllArticles()
        {
            var result = await _articleService.GetAllByNonDeletedAsync();
           
                var articles = JsonSerializer.Serialize(result.Data,new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented= true
                });
                return Json(articles);
          
        
        }
    }
}
