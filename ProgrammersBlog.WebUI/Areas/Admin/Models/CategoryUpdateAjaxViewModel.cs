using ProgrammersBlog.Entities.Dtos;

namespace ProgrammersBlog.WebUI
{
    public class CategoryUpdateAjaxViewModel
    {
        public CategoryUpdateDto CategoryUpdateDto { get; set; }
        public CategoryDto CategoryDto { get; set; }
        public string CategoryUpdatePartial { get; set; }
    }
}