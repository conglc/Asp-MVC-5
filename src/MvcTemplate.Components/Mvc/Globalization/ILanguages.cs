using System;

namespace MvcTemplate.Components.Mvc
{
    public interface ILanguages
    {
        Language[] Supported { get; }
        Language Default { get; }
        Language Current { get; set; }

        Language this[String abbreviation] { get; }
    }
}
