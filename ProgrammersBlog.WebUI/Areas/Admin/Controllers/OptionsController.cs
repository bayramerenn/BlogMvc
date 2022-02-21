using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Helpers.Abstract;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OptionsController : Controller
    {
        private readonly AboutUsPageInfo _aboutUsPageInfo;
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageInfoWriter;
        private readonly IToastNotification _toastNotification;
        private readonly WebSiteInfo _webSiteInfo;
        private readonly IWritableOptions<WebSiteInfo> _webSiteInfoWriter;
        private readonly SmtpSettings _smtpSettings;
        private readonly IWritableOptions<SmtpSettings> _smtpSettingsWriter;
        private readonly ArticleRightSideBarWidgetOptions _articleRightSideBarWidgetOptions;
        private readonly IWritableOptions<ArticleRightSideBarWidgetOptions> _articleRightSideBarWidgetOptionsWriter;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public OptionsController(IOptionsSnapshot<AboutUsPageInfo> aboutUsPageInfo, IWritableOptions<AboutUsPageInfo> aboutUsPageInfoWrite, IToastNotification toastNotification, IOptionsSnapshot<WebSiteInfo> webSiteInfo, IWritableOptions<WebSiteInfo> webSiteInfoWriter, IOptionsSnapshot<SmtpSettings> smtpSettings, IWritableOptions<SmtpSettings> smtpSettingsWriter, IOptionsSnapshot<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptions, IWritableOptions<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptionsWriter, ICategoryService categoryService, IMapper mapper)
        {
            _aboutUsPageInfo = aboutUsPageInfo.Value;
            _aboutUsPageInfoWriter = aboutUsPageInfoWrite;
            _toastNotification = toastNotification;
            _webSiteInfo = webSiteInfo.Value;
            _webSiteInfoWriter = webSiteInfoWriter;
            _smtpSettings = smtpSettings.Value;
            _smtpSettingsWriter = smtpSettingsWriter;
            _articleRightSideBarWidgetOptions = articleRightSideBarWidgetOptions.Value;
            _articleRightSideBarWidgetOptionsWriter = articleRightSideBarWidgetOptionsWriter;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult About()
        {
            return View(_aboutUsPageInfo);
        }

        [HttpPost]
        public IActionResult About(AboutUsPageInfo aboutUsPageInfo)
        {
            if (ModelState.IsValid)
            {
                _aboutUsPageInfoWriter.Update(x =>
                {
                    x.Header = aboutUsPageInfo.Header;
                    x.Content = aboutUsPageInfo.Content;
                    x.SeoDescription = aboutUsPageInfo.SeoDescription;
                    x.SeoAuthor = aboutUsPageInfo.SeoAuthor;
                    x.SeoTags = aboutUsPageInfo.SeoTags;
                });
                _toastNotification.AddSuccessToastMessage("Hakkımızda Sayfa İçerikleri Başarıyla Güncellenmiştir.", new ToastrOptions
                {
                    Title = "Başarılı İşleme!"
                });
                return View(aboutUsPageInfo);
            }
            return View(aboutUsPageInfo);
        }

        [HttpGet]
        public IActionResult GeneralSettings()
        {
            return View(_webSiteInfo);
        }

        [HttpPost]
        public IActionResult GeneralSettings(WebSiteInfo webSiteInfo)
        {
            if (ModelState.IsValid)
            {
                _webSiteInfoWriter.Update(x =>
                {
                    x.MenuTitle = webSiteInfo.MenuTitle;
                    x.Title = webSiteInfo.Title;
                    x.SeoDescription = webSiteInfo.SeoDescription;
                    x.SeoAuthor = webSiteInfo.SeoAuthor;
                    x.SeoTags = webSiteInfo.SeoTags;
                });
                _toastNotification.AddSuccessToastMessage("Sitenizin Genel Ayarları Başarıyla Güncellenmiştir.", new ToastrOptions
                {
                    Title = "Başarılı İşleme!"
                });
                return View(webSiteInfo);
            }
            return View(webSiteInfo);
        }

        [HttpGet]
        public IActionResult EmailSettings()
        {
            return View(_smtpSettings);
        }

        [HttpPost]
        public IActionResult EmailSettings(SmtpSettings smtpSettings)
        {
            if (ModelState.IsValid)
            {
                _smtpSettingsWriter.Update(x =>
                {
                    x.Server = smtpSettings.Server;
                    x.SenderName = smtpSettings.SenderName;
                    x.Password = smtpSettings.Password;
                    x.SenderEmail = smtpSettings.SenderEmail;
                    x.Port = smtpSettings.Port;
                    x.Username = smtpSettings.Username;
                });
                _toastNotification.AddSuccessToastMessage("Sitenizin E-Posta Ayarları Başarıyla Güncellenmiştir.", new ToastrOptions
                {
                    Title = "Başarılı İşleme!"
                });
                return View(smtpSettings);
            }
            return View(smtpSettings);
        }

        [HttpGet]
        public async Task<IActionResult> ArticleRightSideBarWidgetSettings()
        {
            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            var articleRightSideBarWidgetOptionsViewModel = _mapper.Map<ArticleRightSideBarWidgetOptionsViewModel>(_articleRightSideBarWidgetOptions);
            articleRightSideBarWidgetOptionsViewModel.Categories = categories.Data.Categories;
            return View(articleRightSideBarWidgetOptionsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ArticleRightSideBarWidgetSettings(ArticleRightSideBarWidgetOptionsViewModel articleRightSideBarWidgetOptions)
        {
            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            articleRightSideBarWidgetOptions.Categories = categories.Data.Categories;
            if (ModelState.IsValid)
            {
                _articleRightSideBarWidgetOptionsWriter.Update(x =>
                {
                    x.Header = articleRightSideBarWidgetOptions.Header;
                    x.TakeSize = articleRightSideBarWidgetOptions.TakeSize;
                    x.CategoryId = articleRightSideBarWidgetOptions.CategoryId;
                    x.FilterBy = articleRightSideBarWidgetOptions.FilterBy;
                    x.OrderBy = articleRightSideBarWidgetOptions.OrderBy;
                    x.IsAscending = articleRightSideBarWidgetOptions.IsAscending;
                    x.StartAt = articleRightSideBarWidgetOptions.StartAt;
                    x.EndAt = articleRightSideBarWidgetOptions.EndAt;
                    x.MaxViewCount = articleRightSideBarWidgetOptions.MaxViewCount;
                    x.MinViewCount = articleRightSideBarWidgetOptions.MinViewCount;
                    x.MaxCommentCount = articleRightSideBarWidgetOptions.MaxCommentCount;
                    x.MinCommentCount = articleRightSideBarWidgetOptions.MinCommentCount;
                });
                _toastNotification.AddSuccessToastMessage("Makale Sayfalarımızın Widget Ayarları Başarıyla Güncellenmiştir.", new ToastrOptions
                {
                    Title = "Başarılı İşleme!"
                });
                return View(articleRightSideBarWidgetOptions);
            }
           
            return View(articleRightSideBarWidgetOptions);
        }
    }
}