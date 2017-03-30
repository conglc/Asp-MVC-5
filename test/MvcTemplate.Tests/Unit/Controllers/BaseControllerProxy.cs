using MvcTemplate.Controllers;
using System;
using System.Web.Mvc;

namespace MvcTemplate.Tests.Unit.Controllers
{
    public class BaseControllerProxy : BaseController
    {
        public RedirectToRouteResult BaseRedirectToAction(String action)
        {
            return RedirectToAction(action);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, Object route)
        {
            return RedirectToAction(action, route);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, String controller)
        {
            return RedirectToAction(action, controller);
        }
        public RedirectToRouteResult BaseRedirectToAction(String action, String controller, Object route)
        {
            return RedirectToAction(action, controller, route);
        }

        public IAsyncResult BaseBeginExecuteCore(AsyncCallback callback, Object state)
        {
            return BeginExecuteCore(callback, state);
        }
        public void BaseOnAuthorization(AuthorizationContext context)
        {
            OnAuthorization(context);
        }
        public void BaseOnActionExecuted(ActionExecutedContext context)
        {
            OnActionExecuted(context);
        }
    }
}
