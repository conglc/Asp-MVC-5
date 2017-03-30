using MvcTemplate.Controllers;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using System.Web.Mvc;

namespace MvcTemplate.Tests.Unit.Controllers
{
    public class ValidatedControllerProxy : ValidatedController<IValidator, IService>
    {
        protected ValidatedControllerProxy(IValidator validator, IService service)
            : base(validator, service)
        {
        }

        public void BaseOnActionExecuting(ActionExecutingContext context)
        {
            OnActionExecuting(context);
        }
    }
}
