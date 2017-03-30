using Datalist;
using MvcTemplate.Components.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class RoleView : BaseView
    {
        [Required]
        [DatalistColumn]
        [StringLength(128)]
        public String Title { get; set; }

        public JsTree Permissions { get; set; }

        public RoleView()
        {
            Permissions = new JsTree();
        }
    }
}
