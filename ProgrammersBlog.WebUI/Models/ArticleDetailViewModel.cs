using ProgrammersBlog.Entities.Dtos;

namespace ProgrammersBlog.WebUI.Models
{
    public class ArticleDetailViewModel
    {
        public ArticleDto ArticleDto{ get; set; }
        public ArticleDetailRightSideBarViewModel ArticleDetailRightSideBarViewModel { get; set; }
    }
}
