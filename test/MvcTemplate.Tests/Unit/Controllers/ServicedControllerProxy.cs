using MvcTemplate.Controllers;
using MvcTemplate.Services;
using System.Web.Mvc;

namespace MvcTemplate.Tests.Unit.Controllers
{
    public class ServicedControllerProxy : ServicedController<IService>
    {
        public ServicedControllerProxy(IService service)
            : base(service)
        {
        }

        public void BaseOnActionExecuting(ActionExecutingContext context)
        {
            OnActionExecuting(context);
        }
    }
}
