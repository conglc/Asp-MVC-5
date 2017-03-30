using System;
using System.Web.Mvc;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NotTrimmedAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return ModelBinders.Binders.DefaultBinder;
        }
    }
}
