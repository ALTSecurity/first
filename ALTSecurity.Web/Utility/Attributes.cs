﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace ALTSecurity.Web.Utility
{
    /// <summary>
    /// Отображение локализированных имен полей
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class DisplayNameLocalizedAttribute : DisplayNameAttribute
    {
        public DisplayNameLocalizedAttribute(Type resourceManagerProvider, string resourceKey)
         : base(LookupResource(resourceManagerProvider, resourceKey))
        {
        }

        internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }
    }
}