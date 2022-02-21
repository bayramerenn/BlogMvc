using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Helpers.Abstract;
using ProgrammersBlog.WebUI.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly AboutUsPageInfo _aboutUsPageInfo;
        private readonly IMailService _mailService;
        private readonly IToastNotification _toastNotification;
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageIngoWriter;

        public HomeController(IArticleService articleService, IOptionsSnapshot<AboutUsPageInfo> options, IMailService mailService, IToastNotification toastNotification, IWritableOptions<AboutUsPageInfo> aboutUsPageIngoWriter)
        {
            _articleService = articleService;
            _aboutUsPageInfo = options.Value;
            _mailService = mailService;
            _toastNotification = toastNotification;
            _aboutUsPageIngoWriter = aboutUsPageIngoWriter;
        }

        [Route("index")]
        [Route("anasayfa")]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, int currentPage = 1, int PageSize = 5, bool isAscending = false)
        {
            var articlesResult = await (categoryId == null
                ? _articleService.GetAllByPagingAsync(null, currentPage, PageSize, isAscending)
                : _articleService.GetAllByPagingAsync(categoryId.Value, currentPage, PageSize, isAscending));
            return View(articlesResult.Data);
        }

        [Route("yakinda")]
        [HttpGet]
        public IActionResult CommingSoon()
        {
            return View();
        }

        [Route("hakkinda")]
        [Route("hakkimizda")]
        [HttpGet]
        public IActionResult About()
        {
            return View(_aboutUsPageInfo);
        }

        [Route("iletisim")]
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("iletisim")]
        [HttpPost]
        public IActionResult Contact(EmailSendDto emailSendDto)
        {
            if (ModelState.IsValid)
            {
                var result = _mailService.SendContactEmail(emailSendDto);
                _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                {
                    Title = "Başarılı İşlem!"
                });
                return View();
            }
            return View(emailSendDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}