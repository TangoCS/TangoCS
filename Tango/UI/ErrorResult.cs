using System;

namespace Tango.UI
{
    public interface IErrorResult
    {
        string OnError(Exception e, int errorId);
    }

    public class ErrorResult : IErrorResult
    {
        public string OnError(Exception e, int errorId)
        {
            return e.ToString().Replace(Environment.NewLine, "<br/>");
        }
    }
}
