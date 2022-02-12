using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ProgrammersBlog.WebUI.Extensions
{
    public static class ModelStateDictionaryHelper
    {
        public static void AddModelErrorExtension(this ModelStateDictionary modelState, IEnumerable<IdentityError> identityErrors)
        {
            foreach (var error in identityErrors)
            {
                modelState.AddModelError("", error.Description);
            }

        }
    }
}
