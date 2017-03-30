using System;
using System.Collections.Generic;

namespace MvcTemplate.Components.Extensions
{
    public class JsTree
    {
        public List<JsTreeNode> Nodes { get; set; }
        public List<Int32> SelectedIds { get; set; }

        public JsTree()
        {
            Nodes = new List<JsTreeNode>();
            SelectedIds = new List<Int32>();
        }
    }
}
