using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using MvcTemplate.Resources.Shared;
using NonFactors.Mvc.Grid;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcTemplate.Components.Extensions
{
    public static class MvcGridExtensions
    {
        public static IGridColumn<T> AddActionLink<T>(this IGridColumnsOf<T> columns, String action, String iconClass) where T : class
        {
            if (!IsAuthorizedTo(columns.Grid.HttpContext, action))
                return new GridColumn<T, String>(columns.Grid, model => "");

            return columns
                .Add(model => GetLink(columns.Grid.HttpContext, model, action, iconClass))
                .Css("action-cell")
                .Encoded(false);
        }

        public static IGridColumn<T> AddDateProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T> AddDateProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T> AddBooleanProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean>> expression)
        {
            Func<T, Boolean> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .RenderedAs(model =>
                    valueFor(model)
                        ? Strings.Yes
                        : Strings.No);
        }
        public static IGridColumn<T> AddBooleanProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean?>> expression)
        {
            Func<T, Boolean?> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .RenderedAs(model =>
                    valueFor(model) != null
                        ? valueFor(model) == true
                            ? Strings.Yes
                            : Strings.No
                        : "");
        }
        public static IGridColumn<T> AddDateTimeProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T> AddDateTimeProperty<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T> AddProperty<T, TProperty>(this IGridColumnsOf<T> columns, Expression<Func<T, TProperty>> expression)
        {
            return columns
                .Add(expression)
                .Css(GetCssClassFor<TProperty>())
                .Titled(ResourceProvider.GetPropertyTitle(expression));
        }

        public static IHtmlGrid<T> ApplyDefaults<T>(this IHtmlGrid<T> grid)
        {
            return grid
                .Pageable(pager => { pager.RowsPerPage = 16; })
                .Named(typeof(T).Name.Replace("View", ""))
                .Empty(Strings.NoDataFound)
                .Css("table-hover")
                .Filterable()
                .Sortable();
        }

        private static String GetLink<T>(HttpContextBase context, T model, String action, String iconClass)
        {
            TagBuilder anchor = new TagBuilder("a");
            anchor.AddCssClass(action.ToLower() + "-action");
            anchor.Attributes["href"] = new UrlHelper(context.Request.RequestContext).Action(action, GetRouteFor(model));

            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass(iconClass);

            anchor.InnerHtml = icon.ToString();

            return anchor.ToString();
        }
        private static Boolean IsAuthorizedTo(HttpContextBase context, String action)
        {
            if (Authorization.Provider == null)
                return true;

            Int32? accountId = context.User.Id();
            String area = context.Request.RequestContext.RouteData.Values["area"] as String;
            String controller = context.Request.RequestContext.RouteData.Values["controller"] as String;

            return Authorization.Provider.IsAuthorizedFor(accountId, area, controller, action);
        }
        private static RouteValueDictionary GetRouteFor<T>(T model)
        {
            PropertyInfo key = typeof(T)
                .GetProperties()
                .FirstOrDefault(property => property.IsDefined(typeof(KeyAttribute), false));

            if (key == null)
                throw new Exception($"{typeof(T).Name} type does not have a key property.");

            RouteValueDictionary route = new RouteValueDictionary();
            route[key.Name] = key.GetValue(model);

            return route;
        }
        private static String GetCssClassFor<TProperty>()
        {
            Type type = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
            if (type.IsEnum) return "text-left";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "text-right";
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return "text-center";
                default:
                    return "text-left";
            }
        }
    }
}
