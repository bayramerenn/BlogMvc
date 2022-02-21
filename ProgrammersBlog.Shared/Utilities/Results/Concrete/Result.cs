using ProgrammersBlog.Shared.Entities.Concrete;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Utilities.Results.Concrete
{
    public class Result : IResult
    {
        public Result(ResultStatus resultStatus)
        {
            ResultStatus = resultStatus;
        }
        
        public Result(ResultStatus resultStatus, IEnumerable<ValidationError> validationErrors) : this(resultStatus)
        {
           
            ValidationErrors = validationErrors;
        }

        public Result(ResultStatus resultStatus, string message) : this(resultStatus)
        {
            Message = message;
          
        }
        public Result(ResultStatus resultStatus, string message, IEnumerable<ValidationError> validationErrors) : this(resultStatus, validationErrors)
        {
            Message = message;

        }

        public Result(ResultStatus resultStatus, string message, Exception exception) : this(resultStatus, message)
        {
            Exception = exception;
            
        }
        public Result(ResultStatus resultStatus, string message, Exception exception, IEnumerable<ValidationError> validationErrors) : this(resultStatus, message, validationErrors)
        {
            Exception = exception;

        }

        public ResultStatus ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public IEnumerable<ValidationError> ValidationErrors{ get; }
  
    }
}
