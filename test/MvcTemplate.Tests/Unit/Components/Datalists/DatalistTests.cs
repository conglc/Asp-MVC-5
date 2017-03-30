using Datalist;
using MvcTemplate.Components.Datalists;
using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Tests.Data;
using MvcTemplate.Tests.Objects;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Xunit;
using Xunit.Extensions;

namespace MvcTemplate.Tests.Unit.Components.Datalists
{
    public class MvcDatalistTests : IDisposable
    {
        private MvcDatalist<Role, RoleView> datalist;
        private UrlHelper url;

        public MvcDatalistTests()
        {
            HttpContext.Current = HttpContextFactory.CreateHttpContext();
            url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            datalist = new MvcDatalist<Role, RoleView>(url);
            using (TestingContext context = new TestingContext()) context.DropData();
        }
        public void Dispose()
        {
            HttpContext.Current = null;
        }

        #region Datalist(UrlHelper url)

        [Fact]
        public void Datalist_SetsDialogTitle()
        {
            datalist = new MvcDatalist<Role, RoleView>(url);

            String expected = ResourceProvider.GetDatalistTitle(typeof(RoleView).Name.Replace("View", ""));
            String actual = datalist.Title;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Datalist_SetsDatalistUrl()
        {
            datalist = new MvcDatalist<Role, RoleView>(url);

            String expected = url.Action(typeof(Role).Name, MvcDatalist.Prefix, new { area = "" });
            String actual = datalist.Url;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetColumnHeader(PropertyInfo property)

        [Fact]
        public void GetColumnHeader_ReturnsPropertyTitle()
        {
            String actual = datalist.GetColumnHeader(typeof(RoleView).GetProperty("Title"));
            String expected = ResourceProvider.GetPropertyTitle(typeof(RoleView), "Title");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetColumnHeader_ReturnsRelationPropertyTitle()
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty("Child");

            String actual = datalist.GetColumnHeader(property);
            String expected = "";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetColumnCssClass(PropertyInfo property)

        [Theory]
        [InlineData("EnumField", "text-left")]
        [InlineData("SByteField", "text-right")]
        [InlineData("ByteField", "text-right")]
        [InlineData("Int16Field", "text-right")]
        [InlineData("UInt16Field", "text-right")]
        [InlineData("Int32Field", "text-right")]
        [InlineData("UInt32Field", "text-right")]
        [InlineData("Int64Field", "text-right")]
        [InlineData("UInt64Field", "text-right")]
        [InlineData("SingleField", "text-right")]
        [InlineData("DoubleField", "text-right")]
        [InlineData("DecimalField", "text-right")]
        [InlineData("DateTimeField", "text-center")]

        [InlineData("NullableEnumField", "text-left")]
        [InlineData("NullableSByteField", "text-right")]
        [InlineData("NullableByteField", "text-right")]
        [InlineData("NullableInt16Field", "text-right")]
        [InlineData("NullableUInt16Field", "text-right")]
        [InlineData("NullableInt32Field", "text-right")]
        [InlineData("NullableUInt32Field", "text-right")]
        [InlineData("NullableInt64Field", "text-right")]
        [InlineData("NullableUInt64Field", "text-right")]
        [InlineData("NullableSingleField", "text-right")]
        [InlineData("NullableDoubleField", "text-right")]
        [InlineData("NullableDecimalField", "text-right")]
        [InlineData("NullableDateTimeField", "text-center")]

        [InlineData("StringField", "text-left")]
        [InlineData("Child", "text-left")]
        public void GetColumnCssClass_ReturnsCssClassForPropertyType(String propertyName, String cssClass)
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty(propertyName);

            String actual = datalist.GetColumnCssClass(property);
            String expected = cssClass;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetModels()

        [Fact]
        public void GetModels_FromUnitOfWork()
        {
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            datalist = new MvcDatalist<Role, RoleView>(unitOfWork);
            unitOfWork.Select<Role>().To<RoleView>().Returns(new RoleView[0].AsQueryable());

            Object expected = unitOfWork.Select<Role>().To<RoleView>();
            Object actual = datalist.GetModels();

            Assert.Same(expected, actual);
        }

        #endregion

        #region FilterById(IQueryable<TView> models)

        [Fact]
        public void FilterById_NotInteger_ReturnsEmpty()
        {
            datalist.Filter.Id = "A";

            Assert.Empty(datalist.FilterById(null));
        }

        [Fact]
        public void FilterById_FromCurrentFilter()
        {
            TestingContext context = new TestingContext();
            Role role = ObjectFactory.CreateRole();
            context.Set<Role>().Add(role);
            context.SaveChanges();

            IUnitOfWork unitOfWork = new UnitOfWork(context);
            datalist = new MvcDatalist<Role, RoleView>(unitOfWork);

            datalist.Filter.Id = role.Id.ToString();

            RoleView expected = unitOfWork.Select<Role>().To<RoleView>().Single();
            RoleView actual = datalist.FilterById(null).Single();

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        #endregion
    }
}
