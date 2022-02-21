using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Attributes;
using ProgrammersBlog.WebUI.Models;
using System;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ArticleRightSideBarWidgetOptions _articleRightSideBarWidgetOptions;

        public ArticleController(IArticleService articleService, IOptionsSnapshot<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptions)
        {
            _articleService = articleService;
            _articleRightSideBarWidgetOptions = articleRightSideBarWidgetOptions.Value;
        }
        [HttpGet]
        public async Task<IActionResult> Search(string keyword,int currentPage = 1,int pageSize = 5,bool isAscending = false)
        {
            var searchResult = await _articleService.SearchAsync(keyword, currentPage, pageSize, isAscending);
            if (searchResult.ResultStatus == ResultStatus.Success)
                return View(new ArticleSearchViewModel
                {
                    ArticleListDto = searchResult.Data,
                    Keyword = keyword,
                });
            return NotFound();
        }
        [HttpGet]
        [ViewCountFilter]
        public async Task<IActionResult> Detail(int articleId)
        {
            var articleResult =await _articleService.GetAsync(articleId);
            if (articleResult.ResultStatus == ResultStatus.Success)
            {
                var userArticles = await _articleService.GetAllByUserIdOnFilter(
                    articleResult.Data.Article.UserId,
                    filterBy: _articleRightSideBarWidgetOptions.FilterBy,
                    orderBy: _articleRightSideBarWidgetOptions.OrderBy,
                    isAscending: _articleRightSideBarWidgetOptions.IsAscending,
                    takeSize: _articleRightSideBarWidgetOptions.TakeSize,
                    categoryId: _articleRightSideBarWidgetOptions.CategoryId,
                    startAt: _articleRightSideBarWidgetOptions.StartAt,
                    endAt: _articleRightSideBarWidgetOptions.EndAt,
                    minViewCount: _articleRightSideBarWidgetOptions.MinViewCount,
                    maxViewCount: _articleRightSideBarWidgetOptions.MaxViewCount,
                    minCommentCount: _articleRightSideBarWidgetOptions.MinCommentCount,
                    maxCommentCount: _articleRightSideBarWidgetOptions.MaxCommentCount);
                //await _articleService.IncreaseViewCountAsync(articleId);
                return View(new ArticleDetailViewModel
                {
                    ArticleDto = articleResult.Data,
                    ArticleDetailRightSideBarViewModel = new ArticleDetailRightSideBarViewModel
                    {
                        ArticleListDto = userArticles.Data,
                        Header = _articleRightSideBarWidgetOptions.Header,
                        User = articleResult.Data.Article.User
                    }
                });
            }
            return NotFound();
        }
    }
}
