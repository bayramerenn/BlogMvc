using ProgrammersBlog.Entities.Dtos;
using System;

namespace ProgrammersBlog.WebUI.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

