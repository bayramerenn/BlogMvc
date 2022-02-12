using ProgrammersBlog.Entities.Concrete;
using System.Collections;
using System.Collections.Generic;

namespace ProgrammersBlog.WebUI.Areas.Admin.Models
{
    public class UserWithRolesViewModel
    {
        public User User { get; set; }
        public IList<string> Roles{ get; set; }
    }
}
