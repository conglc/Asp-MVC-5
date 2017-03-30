using System;
using System.Web.Mvc;

namespace MvcTemplate.Components.Mvc
{
    public class TrimmingModelBinder : IModelBinder
    {
        public Object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = GetValue(controllerContext.Controller, bindingContext);

            return value?.AttemptedValue?.Trim();
        }

        private ValueProviderResult GetValue(ControllerBase controller, ModelBindingContext context)
        {
            if (!controller.ValidateRequest || !context.ModelMetadata.RequestValidationEnabled)
            {
                IUnvalidatedValueProvider provider = context.ValueProvider as IUnvalidatedValueProvider;
                if (provider != null) return provider.GetValue(context.ModelName, true);
            }

            return context.ValueProvider.GetValue(context.ModelName);
        }
    }
}
