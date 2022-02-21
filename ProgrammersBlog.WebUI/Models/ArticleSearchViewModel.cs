using ProgrammersBlog.Entities.Dtos;

namespace ProgrammersBlog.WebUI.Models
{
    public class ArticleSearchViewModel
    {
        public ArticleListDto ArticleListDto{ get; set; }
        public string Keyword { get; set; }
    }
}
