using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cashew.Toasty.Config;

namespace Cashew.Toasty.Defaults
{
    public static class DefaultSettings
    {
        static ToasterSettings _defaultToasterSettings = DefaultToasterSettingsProvider.DefaultToasterSettings;

        public static ToasterSettings DefaultToasterSettings
        {
            get => _defaultToasterSettings;
            set => _defaultToasterSettings = value ?? throw new ArgumentNullException();
        }

        static ToastSettings _defaultToastSettings = DefaultToastSettingsProvider.DefaultToastSettings;
        public static ToastSettings DefaultToastSettings
        {
            get => _defaultToastSettings;
            set => _defaultToastSettings = value ?? throw new ArgumentNullException();
        }

        static ToastTemplate _defaultToastViewTemplate = DefaultToastViewTemplateProvider.DefaultToastTemplate;
        public static ToastTemplate DefaultToastViewTemplate
        {
            get => _defaultToastViewTemplate;
            set => _defaultToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultToastViewFunc =
            (title, message) => DefaultToastViewTemplate.GetToastView(title, message);
        public static Func<string, string, UIElement> GetDefaultToastViewFunc
        {
            get => _getDefaultToastViewFunc;
            set => _getDefaultToastViewFunc = value ?? throw new ArgumentNullException();
        }
    }
}
