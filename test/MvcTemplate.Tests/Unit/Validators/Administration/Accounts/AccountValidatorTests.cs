using MvcTemplate.Components.Alerts;
using MvcTemplate.Components.Security;
using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using MvcTemplate.Resources.Views.Administration.Accounts.AccountView;
using MvcTemplate.Tests.Data;
using MvcTemplate.Validators;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Tests.Unit.Validators
{
    public class AccountValidatorTests : IDisposable
    {
        private AccountValidator validator;
        private TestingContext context;
        private Account account;
        private IHasher hasher;

        public AccountValidatorTests()
        {
            context = new TestingContext();
            hasher = Substitute.For<IHasher>();
            hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(true);

            context.DropData();
            SetUpData();

            validator = new AccountValidator(new UnitOfWork(context), hasher);
            validator.CurrentAccountId = account.Id;
        }
        public void Dispose()
        {
            validator.Dispose();
            context.Dispose();
        }

        #region CanRecover(AccountRecoveryView view)

        [Fact]
        public void CanRecover_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanRecover(ObjectFactory.CreateAccountRecoveryView()));
        }

        [Fact]
        public void CanRecover_ValidAccount()
        {
            Assert.True(validator.CanRecover(ObjectFactory.CreateAccountRecoveryView()));
        }

        #endregion

        #region CanReset(AccountResetView view)

        [Fact]
        public void CanReset_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanReset(ObjectFactory.CreateAccountResetView()));
        }

        [Fact]
        public void CanReset_ExpiredToken_ReturnsFalse()
        {
            account.RecoveryTokenExpirationDate = DateTime.Now.AddMinutes(-5);
            context.SaveChanges();

            Boolean canReset = validator.CanReset(ObjectFactory.CreateAccountResetView());
            Alert alert = validator.Alerts.Single();

            Assert.False(canReset);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validations.ExpiredToken, alert.Message);
        }

        [Fact]
        public void CanReset_ValidAccount()
        {
            Assert.True(validator.CanRecover(ObjectFactory.CreateAccountRecoveryView()));
        }

        #endregion

        #region CanLogin(AccountLoginView view)

        [Fact]
        public void CanLogin_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanLogin(ObjectFactory.CreateAccountLoginView()));
        }

        [Fact]
        public void CanLogin_NoAccount_ReturnsFalse()
        {
            hasher.VerifyPassword(null, null).Returns(false);
            AccountLoginView view = new AccountLoginView();

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validations.IncorrectAuthentication, alert.Message);
        }

        [Fact]
        public void CanLogin_IncorrectPassword_ReturnsFalse()
        {
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            AccountLoginView view = ObjectFactory.CreateAccountLoginView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validations.IncorrectAuthentication, alert.Message);
        }

        [Fact]
        public void CanLogin_LockedAccount_ReturnsFalse()
        {
            AccountLoginView view = ObjectFactory.CreateAccountLoginView();
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validations.LockedAccount, alert.Message);
        }

        [Fact]
        public void CanLogin_IsCaseInsensitive()
        {
            AccountLoginView view = ObjectFactory.CreateAccountLoginView();
            view.Username = view.Username.ToUpper();

            Assert.True(validator.CanLogin(view));
        }

        [Fact]
        public void CanLogin_ValidAccount()
        {
            Assert.True(validator.CanLogin(ObjectFactory.CreateAccountLoginView()));
        }

        #endregion

        #region CanCreate(AccountCreateView view)

        [Fact]
        public void CanCreate_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanCreate(ObjectFactory.CreateAccountCreateView(1)));
        }

        [Fact]
        public void CanCreate_UsedUsername_ReturnsFalse()
        {
            AccountCreateView view = ObjectFactory.CreateAccountCreateView(1);
            view.Username = account.Username.ToLower();

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueUsername, validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_UsedEmail_ReturnsFalse()
        {
            AccountCreateView view = ObjectFactory.CreateAccountCreateView(1);
            view.Email = account.Email.ToUpper();

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueEmail, validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_ValidAccount()
        {
            Assert.True(validator.CanCreate(ObjectFactory.CreateAccountCreateView(1)));
        }

        #endregion

        #region CanEdit(AccountEditView view)

        [Fact]
        public void CanEdit_Account_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectFactory.CreateAccountEditView(account.Id)));
        }

        [Fact]
        public void CanEdit_Account_UsedUsername_ReturnsFalse()
        {
            Account takenAccount = ObjectFactory.CreateAccount(1);
            context.Set<Account>().Add(takenAccount);
            context.SaveChanges();

            AccountEditView view = ObjectFactory.CreateAccountEditView(account.Id);
            view.Username = takenAccount.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueUsername, validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameUsername()
        {
            AccountEditView view = ObjectFactory.CreateAccountEditView(account.Id);
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Account_UsedEmail_ReturnsFalse()
        {
            Account usedEmailAccount = ObjectFactory.CreateAccount(1);
            context.Set<Account>().Add(usedEmailAccount);
            context.SaveChanges();

            AccountEditView view = ObjectFactory.CreateAccountEditView(account.Id);
            view.Email = usedEmailAccount.Email;

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueEmail, validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameEmail()
        {
            AccountEditView view = ObjectFactory.CreateAccountEditView(account.Id);
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidAccount()
        {
            Assert.True(validator.CanEdit(ObjectFactory.CreateAccountEditView(account.Id)));
        }

        #endregion

        #region CanEdit(ProfileEditView view)

        [Fact]
        public void CanEdit_Profile_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectFactory.CreateProfileEditView()));
        }

        [Fact]
        public void CanEdit_Profile_IncorrectPassword_ReturnsFalse()
        {
            ProfileEditView view = ObjectFactory.CreateProfileEditView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.IncorrectPassword, validator.ModelState["Password"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_UsedUsername_ReturnsFalse()
        {
            Account takenAccount = ObjectFactory.CreateAccount(1);
            context.Set<Account>().Add(takenAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectFactory.CreateProfileEditView();
            view.Username = takenAccount.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueUsername, validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameUsername()
        {
            ProfileEditView view = ObjectFactory.CreateProfileEditView();
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Profile_UsedEmail_ReturnsFalse()
        {
            Account usedEmailAccount = ObjectFactory.CreateAccount(1);
            context.Set<Account>().Add(usedEmailAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectFactory.CreateProfileEditView();
            view.Email = usedEmailAccount.Email;

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.UniqueEmail, validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameEmail()
        {
            ProfileEditView view = ObjectFactory.CreateProfileEditView();
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidProfile()
        {
            Assert.True(validator.CanEdit(ObjectFactory.CreateProfileEditView()));
        }

        #endregion

        #region CanDelete(ProfileDeleteView view)

        [Fact]
        public void CanDelete_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanDelete(ObjectFactory.CreateProfileDeleteView()));
        }

        [Fact]
        public void CanDelete_IncorrectPassword_ReturnsFalse()
        {
            ProfileDeleteView view = ObjectFactory.CreateProfileDeleteView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canDelete = validator.CanDelete(view);

            Assert.False(canDelete);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validations.IncorrectPassword, validator.ModelState["Password"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanDelete_ValidProfile()
        {
            Assert.True(validator.CanDelete(ObjectFactory.CreateProfileDeleteView()));
        }

        #endregion

        #region Test helpers

        private void SetUpData()
        {
            account = ObjectFactory.CreateAccount();
            account.RoleId = account.Role.Id;
            account.IsLocked = false;

            context.Set<Account>().Add(account);
            context.SaveChanges();
        }

        #endregion
    }
}
