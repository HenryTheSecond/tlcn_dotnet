using Microsoft.AspNetCore.Mvc.Filters;

namespace tlcn_dotnet.CustomException
{
    public class ControllerExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
        }
    }
}
