﻿using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;

namespace ProgrammersBlog.Entities.Dtos
{
    public partial class UserListDto : DtoGetBase
    {
        public IList<User> Users { get; set; }
        
    }
}