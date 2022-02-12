using AutoMapper;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.WebUI.Areas.Admin.Models;

namespace ProgrammersBlog.WebUI.Automapper
{
    public class ViewModelProfile:Profile
    {
        public ViewModelProfile()
        {
            CreateMap<ArticleAddViewModel,ArticleAddDto>().ReverseMap();
        }
    }
}
