using AutoMapper;
using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.WebUI.Helpers.Abstract;

namespace ProgrammersBlog.WebUI.Automapper
{
    public class UserProfile : Profile
    {
        public UserProfile(IImageHelper imageHelper)
        {
            CreateMap<User, UserAddDto>();
            CreateMap<UserAddDto, User>().ForMember(dest 
                => dest.Picture,opt => opt.MapFrom(x
                => imageHelper.Upload(x.UserName,x.PictureFile,PictureType.User,null)));
            CreateMap<User, UserUpdateDto>().ReverseMap();
        }
    }
}
