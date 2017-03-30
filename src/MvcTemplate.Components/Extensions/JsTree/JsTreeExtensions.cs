using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace MvcTemplate.Components.Extensions
{
    public static class JsTreeExtensions
    {
        public static MvcHtmlString JsTreeFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, JsTree>> expression)
        {
            String name = ExpressionHelper.GetExpressionText(expression) + ".SelectedIds";
            JsTree model = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model as JsTree;

            return new MvcHtmlString(HiddenIdsFor(name, model) + JsTreeFor(name, model));
        }

        private static void Add(TagBuilder tree, List<JsTreeNode> nodes)
        {
            if (nodes.Count == 0) return;

            TagBuilder branch = new TagBuilder("ul");
            StringBuilder nodeBuilder = new StringBuilder();

            foreach (JsTreeNode node in nodes)
            {
                TagBuilder item = new TagBuilder("li");
                String id = node.Id.ToString();
                item.InnerHtml = node.Title;
                item.Attributes["id"] = id;

                Add(item, node.Nodes);
                nodeBuilder.Append(item);
            }

            branch.InnerHtml = nodeBuilder.ToString();
            tree.InnerHtml += branch.ToString();
        }
        private static String HiddenIdsFor(String name, JsTree model)
        {
            StringBuilder inputs = new StringBuilder();
            TagBuilder input = new TagBuilder("input");
            input.Attributes["type"] = "hidden";
            input.Attributes["name"] = name;

            foreach (Int32 id in model.SelectedIds)
            {
                input.Attributes["value"] = id.ToString();
                inputs.Append(input.ToString(TagRenderMode.SelfClosing));
            }

            TagBuilder ids = new TagBuilder("div");
            ids.AddCssClass("js-tree-view-ids");
            ids.InnerHtml = inputs.ToString();

            return ids.ToString();
        }
        private static String JsTreeFor(String name, JsTree model)
        {
            TagBuilder tree = new TagBuilder("div");
            tree.AddCssClass("js-tree-view");
            tree.Attributes["for"] = name;

            Add(tree, model.Nodes);

            return tree.ToString();
        }
    }
}
