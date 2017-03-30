using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Xunit;

namespace MvcTemplate.Tests.Unit.Resources
{
    public class ResourcesTests
    {
        [Fact]
        public void Resources_HasAllPermissionAreaTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Area.Titles.ResourceManager;
            using (Context context = new Context())
            {
                String[] areas = context
                    .Set<Permission>()
                    .Select(permission => permission.Area)
                    .Distinct()
                    .ToArray();

                foreach (String area in areas)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(area)),
                        $"Permission area '{area}', does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionControllerTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Controller.Titles.ResourceManager;
            using (Context context = new Context())
            {
                String[] controllers = context
                    .Set<Permission>()
                    .Select(permission => permission.Area + permission.Controller)
                    .Distinct()
                    .ToArray();

                foreach (String controller in controllers)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(controller)),
                        $"Permission controller '{controller}', does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionActionTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Action.Titles.ResourceManager;
            using (Context context = new Context())
            {
                String[] actions = context
                    .Set<Permission>()
                    .Select(permission => permission.Area + permission.Controller + permission.Action)
                    .Distinct()
                    .ToArray();

                foreach (String action in actions)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(action)),
                        $"Permission action '{action}', does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasEquivalents()
        {
            IEnumerable<CultureInfo> languages = new[] { new CultureInfo("en-GB"), new CultureInfo("lt-LT") };
            IEnumerable<Type> types = Assembly
                .Load("MvcTemplate.Resources")
                .GetTypes()
                .Where(type => type.Namespace.StartsWith("MvcTemplate.Resources."));

            foreach (Type type in types)
            {
                IEnumerable<String> keys = new String[0];
                ResourceManager manager = new ResourceManager(type);

                foreach (ResourceSet set in languages.Select(language => manager.GetResourceSet(language, true, true)))
                {
                    keys = keys.Union(set.Cast<DictionaryEntry>().Select(resource => resource.Key.ToString()));
                    keys = keys.Distinct();
                }

                foreach (CultureInfo language in languages)
                {
                    ResourceSet set = manager.GetResourceSet(language, true, true);
                    foreach (String key in keys)
                        Assert.True((set.GetObject(key) ?? "").ToString() != "",
                            $"{type.FullName}, does not have translation for '{key}' in {language.EnglishName} language.");
                }
            }
        }
    }
}
