using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources.Shared;
using MvcTemplate.Tests.Objects;
using NonFactors.Mvc.Grid;
using NSubstitute;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Xunit;

namespace MvcTemplate.Tests.Unit.Components.Extensions
{
    public class MvcGridExtensionsTests : IDisposable
    {
        private IGridColumnsOf<AllTypesView> columns;
        private IHtmlGrid<AllTypesView> html;

        public MvcGridExtensionsTests()
        {
            Grid<AllTypesView> grid = new Grid<AllTypesView>(new AllTypesView[0]);
            HtmlHelper htmlHelper = HtmlHelperFactory.CreateHtmlHelper();
            html = new HtmlGrid<AllTypesView>(htmlHelper, grid);
            columns = new GridColumns<AllTypesView>(grid);
        }
        public void Dispose()
        {
            Authorization.Provider = null;
        }

        #region AddActionLink<T>(this IGridColumnOf<T> columns, String action, String iconClass)

        [Fact]
        public void AddActionLink_Unauthorized_Empty()
        {
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();

            IGridColumn<AllTypesView> actual = columns.AddActionLink("Edit", "fa fa-pencil");

            Assert.Empty(actual.ValueFor(null).ToString());
            Assert.Empty(columns);
        }

        [Fact]
        public void AddActionLink_Authorized_Renders()
        {
            AllTypesView view = new AllTypesView();
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            UrlHelper urlHelper = new UrlHelper(columns.Grid.HttpContext.Request.RequestContext);
            Authorization.Provider.IsAuthorizedFor(Arg.Any<Int32?>(), Arg.Any<String>(), Arg.Any<String>(), "Details").Returns(true);

            IGridColumn<AllTypesView> column = columns.AddActionLink("Details", "fa fa-info");

            String actual = column.ValueFor(new GridRow<AllTypesView>(view)).ToString();
            String expected =
                $"<a class=\"details-action\" href=\"{urlHelper.Action("Details", new {view.Id})}\">" +
                    "<i class=\"fa fa-info\"></i>" +
                "</a>";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddActionLink_NullAuthorization_Renders()
        {
            Authorization.Provider = null;
            AllTypesView view = new AllTypesView();
            UrlHelper urlHelper = new UrlHelper(columns.Grid.HttpContext.Request.RequestContext);

            IGridColumn<AllTypesView> column = columns.AddActionLink("Details", "fa fa-info");

            String actual = column.ValueFor(new GridRow<AllTypesView>(view)).ToString();
            String expected = 
                $"<a class=\"details-action\" href=\"{urlHelper.Action("Details", new {view.Id})}\">" +
                    "<i class=\"fa fa-info\"></i>" +
                "</a>";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddActionLink_NoKey_Throws()
        {
            IGrid<Object> grid = new Grid<Object>(new Object[0]);
            IGridColumnsOf<Object> gridColumns = new GridColumns<Object>(grid);
            gridColumns.Grid.HttpContext = HttpContextFactory.CreateHttpContextBase();

            IGridColumn<Object> column = gridColumns.AddActionLink("Delete", "fa fa-times");

            String actual = Assert.Throws<Exception>(() => column.ValueFor(new GridRow<Object>(new Object()))).Message;
            String expected = "Object type does not have a key property.";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddActionLink_Column()
        {
            IGridColumn<AllTypesView> actual = columns.AddActionLink("Edit", "fa fa-pencil");

            Assert.Equal("action-cell", actual.CssClasses);
            Assert.False(actual.IsEncoded);
            Assert.Single(columns);
        }

        #endregion

        #region AddDateProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, DateTime>> expression)

        [Fact]
        public void AddDateProperty_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView> actual = columns.AddDateProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:d}", actual.Format);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddDateProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, DateTime?>> expression)

        [Fact]
        public void AddDateProperty_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView> actual = columns.AddDateProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:d}", actual.Format);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddBooleanProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, Boolean>> expression)

        [Fact]
        public void AddBooleanProperty_Column()
        {
            Expression<Func<AllTypesView, Boolean>> expression = (model) => model.BooleanField;

            IGridColumn<AllTypesView> actual = columns.AddBooleanProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddBooleanProperty_True()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = true });
            IGridColumn<AllTypesView> column = columns.AddBooleanProperty(model => model.BooleanField);

            String actual = column.ValueFor(row).ToString();
            String expected = Strings.Yes;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBooleanProperty_False()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = false });
            IGridColumn<AllTypesView> column = columns.AddBooleanProperty(model => model.BooleanField);

            String actual = column.ValueFor(row).ToString();
            String expected = Strings.No;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddBooleanProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, Boolean?>> expression)

        [Fact]
        public void AddBooleanProperty_Nullable_Column()
        {
            Expression<Func<AllTypesView, Boolean?>> expression = (model) => model.NullableBooleanField;

            IGridColumn<AllTypesView> actual = columns.AddBooleanProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddBooleanProperty_Nullable()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = null });
            IGridColumn<AllTypesView> column = columns.AddBooleanProperty(model => model.NullableBooleanField);

            String actual = column.ValueFor(row).ToString();

            Assert.Empty(actual);
        }

        [Fact]
        public void AddBooleanProperty_Nullable_True()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = true });
            IGridColumn<AllTypesView> column = columns.AddBooleanProperty(model => model.NullableBooleanField);

            String actual = column.ValueFor(row).ToString();
            String expected = Strings.Yes;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBooleanProperty_Nullable_False()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = false });
            IGridColumn<AllTypesView> column = columns.AddBooleanProperty(model => model.NullableBooleanField);

            String actual = column.ValueFor(row).ToString();
            String expected = Strings.No;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AddDateTimeProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, DateTime>> expression)

        [Fact]
        public void AddDateTimeProperty_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView> actual = columns.AddDateTimeProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:g}", actual.Format);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddDateTimeProperty<T>(this IGridColumnOf<T> columns, Expression<Func<T, DateTime?>> expression)

        [Fact]
        public void AddDateTimeProperty_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView> actual = columns.AddDateTimeProperty(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:g}", actual.Format);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        #endregion

        #region AddProperty<T, TProperty>(this IGridColumnOf<T> columns, Expression<Func<T, TProperty>> expression)

        [Fact]
        public void AddProperty_Column()
        {
            Expression<Func<AllTypesView, AllTypesView>> expression = (model) => model;

            IGridColumn<AllTypesView> actual = columns.AddProperty(expression);

            Assert.Equal("text-left", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Null(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddProperty_SetsCssClassForEnum()
        {
            AssertCssClassFor(model => model.EnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSByte()
        {
            AssertCssClassFor(model => model.SByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForByte()
        {
            AssertCssClassFor(model => model.ByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt16()
        {
            AssertCssClassFor(model => model.Int16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt16()
        {
            AssertCssClassFor(model => model.UInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt32()
        {
            AssertCssClassFor(model => model.Int32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt32()
        {
            AssertCssClassFor(model => model.UInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt64()
        {
            AssertCssClassFor(model => model.Int64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt64()
        {
            AssertCssClassFor(model => model.UInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSingle()
        {
            AssertCssClassFor(model => model.SingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDouble()
        {
            AssertCssClassFor(model => model.DoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDecimal()
        {
            AssertCssClassFor(model => model.DecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForBoolean()
        {
            AssertCssClassFor(model => model.BooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDateTime()
        {
            AssertCssClassFor(model => model.DateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableEnum()
        {
            AssertCssClassFor(model => model.NullableEnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSByte()
        {
            AssertCssClassFor(model => model.NullableSByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableByte()
        {
            AssertCssClassFor(model => model.NullableByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt16()
        {
            AssertCssClassFor(model => model.NullableInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt16()
        {
            AssertCssClassFor(model => model.NullableUInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt32()
        {
            AssertCssClassFor(model => model.NullableInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt32()
        {
            AssertCssClassFor(model => model.NullableUInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt64()
        {
            AssertCssClassFor(model => model.NullableInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt64()
        {
            AssertCssClassFor(model => model.NullableUInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSingle()
        {
            AssertCssClassFor(model => model.NullableSingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDouble()
        {
            AssertCssClassFor(model => model.NullableDoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDecimal()
        {
            AssertCssClassFor(model => model.NullableDecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableBoolean()
        {
            AssertCssClassFor(model => model.NullableBooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDateTime()
        {
            AssertCssClassFor(model => model.NullableDateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForOtherTypes()
        {
            AssertCssClassFor(model => model.StringField, "text-left");
        }

        #endregion

        #region ApplyDefaults<T>(this IHtmlGrid<T> grid)

        [Fact]
        public void ApplyDefaults_Values()
        {
            IGridColumn column = html.Grid.Columns.Add(model => model.ByteField);
            column.IsFilterable = null;
            column.IsSortable = null;

            IGrid actual = html.ApplyDefaults().Grid;

            Assert.Equal(Strings.NoDataFound, actual.EmptyText);
            Assert.Equal("table-hover", actual.CssClasses);
            Assert.Equal(true, column.IsFilterable);
            Assert.Equal(true, column.IsSortable);
            Assert.Equal("AllTypes", actual.Name);
            Assert.NotEmpty(actual.Columns);
        }

        #endregion

        #region Test helpers

        private void AssertCssClassFor<TProperty>(Expression<Func<AllTypesView, TProperty>> property, String cssClasses)
        {
            IGridColumn<AllTypesView> column = columns.AddProperty(property);

            String actual = column.CssClasses;
            String expected = cssClasses;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
