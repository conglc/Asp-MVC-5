using MvcTemplate.Components.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcTemplate.Tests.Unit.Components.Mvc
{
    public class DisplayNameMetadataProviderProxy : DisplayNameMetadataProvider
    {
        public ModelMetadata BaseCreateMetadata(IEnumerable<Attribute> attributes, Type container, Func<Object> model, Type type, String property)
        {
            return CreateMetadata(attributes, container, model, type, property);
        }
    }
}
