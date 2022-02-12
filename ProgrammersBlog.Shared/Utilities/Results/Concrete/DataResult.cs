using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplextTypes;
using System;

namespace ProgrammersBlog.Shared.Utilities.Results.Concrete
{
    public class DataResult<T> : IDataResult<T>
    {
        public DataResult(ResultStatus resultStatus, T data)
        {
            ResultStatus = resultStatus;
            Data = data;
        }

        public DataResult(ResultStatus resultStatus, string message)
        {
            ResultStatus = resultStatus;
            Message = message;
        }

        public DataResult(ResultStatus resultStatus, string message, T data) : this(resultStatus, data)
        {
            Message = message;
        }

        public DataResult(ResultStatus resultStatus, string message, T data,Exception exception) : this(resultStatus,message, data)
        {
            Exception = exception;
        }

        public T Data { get; }

        public ResultStatus ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}