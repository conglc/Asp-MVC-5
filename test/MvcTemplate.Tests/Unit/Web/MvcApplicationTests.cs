using LightInject.Mvc;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Controllers;
using MvcTemplate.Tests.Objects;
using MvcTemplate.Web;
using NSubstitute;
using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Xunit;
using Xunit.Extensions;

namespace MvcTemplate.Tests.Unit.Web
{
    public class MvcApplicationTests : IDisposable
    {
        private MvcApplication application;

        public MvcApplicationTests()
        {
            application = Substitute.ForPartsOf<MvcApplication>();
            application.When(app => app.RegisterAreas()).DoNotCallBase();
            application.When(app => app.RegisterSiteMapProvider()).DoNotCallBase();
            application.When(app => app.RegisterGlobalizationLanguages()).DoNotCallBase();

            DependencyResolver.SetResolver(Substitute.For<IDependencyResolver>());
            ModelValidatorProviders.Providers.Clear();
            RouteTable.Routes.LowercaseUrls = false;
            ModelMetadataProviders.Current = null;
            GlobalizationManager.Languages = null;
            Authorization.Provider = null;
            GlobalFilters.Filters.Clear();
            ModelBinders.Binders.Clear();
            ViewEngines.Engines.Clear();
            BundleTable.Bundles.Clear();
            MvcSiteMap.Provider = null;
            RouteTable.Routes.Clear();
        }
        public void Dispose()
        {
            application.Dispose();
        }

        #region Application_Start()

        [Fact]
        public void Application_Start_RegistersSecureResponseConfiguration()
        {
            application.Application_Start();

            application.Received().RegisterSecureResponseConfiguration();
        }

        [Fact]
        public void Application_Start_RegistersCurrentDependencyResolver()
        {
            application.Application_Start();

            application.Received().RegisterCurrentDependencyResolver();
        }

        [Fact]
        public void Application_Start_RegistersGlobalizationLanguages()
        {
            application.Application_Start();

            application.Received().RegisterGlobalizationLanguages();
        }

        [Fact]
        public void Application_Start_RegistersModelMetadataProvider()
        {
            application.Application_Start();

            application.Received().RegisterModelMetadataProvider();
        }

        [Fact]
        public void Application_Start_RegistersDataTypeValidator()
        {
            application.Application_Start();

            application.Received().RegisterDataTypeValidator();
        }

        [Fact]
        public void Application_Start_RegistersSiteMapProvider()
        {
            application.Application_Start();

            application.Received().RegisterSiteMapProvider();
        }

        [Fact]
        public void Application_Start_RegistersAuthorization()
        {
            application.Application_Start();

            application.Received().RegisterAuthorization();
        }

        [Fact]
        public void Application_Start_RegistersModelBinders()
        {
            application.Application_Start();

            application.Received().RegisterModelBinders();
        }

        [Fact]
        public void Application_Start_RegistersViewEngine()
        {
            application.Application_Start();

            application.Received().RegisterViewEngine();
        }

        [Fact]
        public void Application_Start_RegistersAdapters()
        {
            application.Application_Start();

            application.Received().RegisterAdapters();
        }

        [Fact]
        public void Application_Start_RegistersBundles()
        {
            application.Application_Start();

            application.Received().RegisterBundles();
        }

        [Fact]
        public void Application_Start_RegistersAreas()
        {
            application.Application_Start();

            application.Received().RegisterAreas();
        }

        [Fact]
        public void Application_Start_RegistersRoute()
        {
            application.Application_Start();

            application.Received().RegisterRoute();
        }

        #endregion

        #region RegisterSecureResponseConfiguration()

        [Fact]
        public void RegisterSecureResponseConfiguration_SuppressesXFrameOptionsHeader()
        {
            AntiForgeryConfig.SuppressXFrameOptionsHeader = false;

            application.RegisterSecureResponseConfiguration();

            Assert.True(AntiForgeryConfig.SuppressXFrameOptionsHeader);
        }

        [Fact]
        public void RegisterSecureResponseConfiguration_DisablesMvcResponseHeader()
        {
            MvcHandler.DisableMvcResponseHeader = false;

            application.RegisterSecureResponseConfiguration();

            Assert.True(MvcHandler.DisableMvcResponseHeader);
        }

        #endregion

        #region RegisterCurrentDependencyResolver()

        [Fact]
        public void RegisterCurrentDependencyResolver_Implementation()
        {
            Assert.IsNotType<LightInjectMvcDependencyResolver>(DependencyResolver.Current);

            application.RegisterCurrentDependencyResolver();

            Assert.IsType<LightInjectMvcDependencyResolver>(DependencyResolver.Current);
        }

        #endregion

        #region RegisterGlobalizationLanguages()

        [Fact]
        public void RegisterGlobalizationLanguages_Implementation()
        {
            ILanguages languages = Substitute.For<ILanguages>();
            DependencyResolver.Current.GetService<ILanguages>().Returns(languages);

            application = Substitute.ForPartsOf<MvcApplication>();
            application.RegisterGlobalizationLanguages();

            ILanguages actual = GlobalizationManager.Languages;
            ILanguages expected = languages;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region RegisterModelMetadataProvider()

        [Fact]
        public void RegisterModelMetadataProvider_Implementation()
        {
            application.RegisterModelMetadataProvider();

            Assert.IsType<DisplayNameMetadataProvider>(ModelMetadataProviders.Current);
        }

        #endregion

        #region RegisterDataTypeValidator()

        [Fact]
        public void RegisterDataTypeValidator_RemovesClientDataTypeModelValidatorProvider()
        {
            ClientDataTypeModelValidatorProvider provider = new ClientDataTypeModelValidatorProvider();
            ModelValidatorProviders.Providers.Add(provider);

            application.RegisterDataTypeValidator();

            Assert.False(ModelValidatorProviders.Providers.Contains(provider));
        }

        [Fact]
        public void RegisterDataTypeValidator_Provider()
        {
            application.RegisterDataTypeValidator();

            ModelValidatorProviderCollection providers = ModelValidatorProviders.Providers;
            Type type = typeof(DataTypeValidatorProvider);

            Assert.Single(providers.Select(provider => provider.GetType()), type);
        }

        #endregion

        #region RegisterSiteMapProvider()

        [Fact]
        public void RegisterSiteMapProvider_Implementation()
        {
            IMvcSiteMapProvider siteMap = Substitute.For<IMvcSiteMapProvider>();
            DependencyResolver.Current.GetService<IMvcSiteMapProvider>().Returns(siteMap);

            application = Substitute.ForPartsOf<MvcApplication>();
            application.RegisterSiteMapProvider();

            IMvcSiteMapProvider actual = MvcSiteMap.Provider;
            IMvcSiteMapProvider expected = siteMap;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region RegisterAuthorization()

        [Fact]
        public void RegisterAuthorization_RegistersAuthorization()
        {
            IAuthorizationProvider authorization = Substitute.For<IAuthorizationProvider>();
            DependencyResolver.Current.GetService<IAuthorizationProvider>().Returns(authorization);

            application.RegisterAuthorization();

            IAuthorizationProvider actual = Authorization.Provider;
            IAuthorizationProvider expected = authorization;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RegisterAuthorization_RefreshesAuthorization()
        {
            IAuthorizationProvider authorization = Substitute.For<IAuthorizationProvider>();
            DependencyResolver.Current.GetService<IAuthorizationProvider>().Returns(authorization);

            application.RegisterAuthorization();

            Authorization.Provider.Received().Refresh();
        }

        #endregion

        #region RegisterModelBinders()

        [Fact]
        public void RegisterModelBinders_TrimmingModelBinder()
        {
            Assert.Null(ModelBinders.Binders[typeof(String)]);

            application.RegisterModelBinders();

            IModelBinder actual = ModelBinders.Binders[typeof(String)];

            Assert.IsType<TrimmingModelBinder>(actual);
        }

        #endregion

        #region RegisterViewEngine()

        [Fact]
        public void RegisterViewEngine_RemovesUnnecessaryViewEngines()
        {
            IViewEngine engine = Substitute.For<IViewEngine>();
            ViewEngines.Engines.Add(engine);

            application.RegisterViewEngine();

            Assert.False(ViewEngines.Engines.Contains(engine));
        }

        [Fact]
        public void RegisterViewEngine_Implementation()
        {
            application.RegisterViewEngine();

            IViewEngine actual = ViewEngines.Engines.Single();

            Assert.IsType<ViewEngine>(actual);
        }

        #endregion

        #region RegisterAdapters()

        [Theory]
        [InlineData("Range", typeof(RangeAdapter))]
        [InlineData("Digits", typeof(DigitsAdapter))]
        [InlineData("EqualTo", typeof(EqualToAdapter))]
        [InlineData("Integer", typeof(IntegerAdapter))]
        [InlineData("Required", typeof(RequiredAdapter))]
        [InlineData("MinValue", typeof(MinValueAdapter))]
        [InlineData("MaxValue", typeof(MaxValueAdapter))]
        [InlineData("FileSize", typeof(FileSizeAdapter))]
        [InlineData("MinLength", typeof(MinLengthAdapter))]
        [InlineData("GreaterThan", typeof(GreaterThanAdapter))]
        [InlineData("EmailAddress", typeof(EmailAddressAdapter))]
        [InlineData("StringLength", typeof(StringLengthAdapter))]
        public void RegisterAdapters_RegistersAdapter(String property, Type adapter)
        {
            DataAnnotationsModelValidatorProvider annotations = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = new DataAnnotationsModelMetadataProvider()
                .GetMetadataForProperty(null, typeof(AdaptersModel), property);

            application.RegisterAdapters();

            Assert.Single(annotations.GetValidators(metadata, new ControllerContext()), validator => validator.GetType() == adapter);
        }

        #endregion

        #region RegisterBundles()

        [Fact]
        public void RegisterBundles_RegistersBundles()
        {
            IBundleConfig config = Substitute.For<IBundleConfig>();
            DependencyResolver.Current.GetService<IBundleConfig>().Returns(config);

            application.RegisterBundles();

            config.Received().RegisterBundles(BundleTable.Bundles);
        }

        #endregion

        #region RegisterRoute()

        [Fact]
        public void RegisterRoute()
        {
            IRouteConfig config = Substitute.For<IRouteConfig>();
            DependencyResolver.Current.GetService<IRouteConfig>().Returns(config);

            application.RegisterRoute();

            config.Received().RegisterRoutes(RouteTable.Routes);
        }

        #endregion
    }
}
