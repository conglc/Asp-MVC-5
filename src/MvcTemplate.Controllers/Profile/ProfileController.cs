using MvcTemplate.Components.Security;
using MvcTemplate.Objects;
using MvcTemplate.Resources.Views.Administration.Accounts.AccountView;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using System.Web.Mvc;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class ProfileController : ValidatedController<IAccountValidator, IAccountService>
    {
        public ProfileController(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectIfAuthorized("Logout", "Auth");

            return View(Service.Get<ProfileEditView>(CurrentAccountId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "Id")] ProfileEditView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectIfAuthorized("Logout", "Auth");

            if (!Validator.CanEdit(profile))
                return View(profile);

            Service.Edit(profile);

            Alerts.AddSuccess(Messages.ProfileUpdated, 4000);

            return RedirectIfAuthorized("Edit");
        }

        [HttpGet]
        public ActionResult Delete()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectIfAuthorized("Logout", "Auth");

            Alerts.AddWarning(Messages.ProfileDeleteDisclaimer);

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Exclude = "Id")] ProfileDeleteView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectIfAuthorized("Logout", "Auth");

            if (!Validator.CanDelete(profile))
            {
                Alerts.AddWarning(Messages.ProfileDeleteDisclaimer);

                return View();
            }

            Service.Delete(CurrentAccountId);

            return RedirectIfAuthorized("Logout", "Auth");
        }
    }
}
