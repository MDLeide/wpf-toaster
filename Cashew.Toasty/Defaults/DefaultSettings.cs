using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Defaults
{
    public static class DefaultSettings
    {

        #region Default Toaster Settings
        static ToasterSettings _defaultToasterSettings = DefaultToasterSettingsProvider.DefaultToasterSettings;
        public static ToasterSettings DefaultToasterSettings
        {
            get => _defaultToasterSettings;
            set => _defaultToasterSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Toast Settings
        static ToastAdornerSettings _defaultToastSettings = DefaultToastSettingsProvider.DefaultToastSettings;
        public static ToastAdornerSettings DefaultToastSettings
        {
            get => _defaultToastSettings;
            set => _defaultToastSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Info Toast Settings
        static ToastAdornerSettings _defaultInfoToastSettings = DefaultToastSettingsProvider.DefaultInfoSettings;
        public static ToastAdornerSettings DefaultInfoToastSettings
        {
            get => _defaultInfoToastSettings;
            set => _defaultInfoToastSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Warning Toast Settings
        static ToastAdornerSettings _defaultWarningToastSettings = DefaultToastSettingsProvider.DefaultWarningSettings;
        public static ToastAdornerSettings DefaultWarningToastSettings
        {
            get => _defaultWarningToastSettings;
            set => _defaultWarningToastSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Success Toast Settings
        static ToastAdornerSettings _defaultSuccessToastSettings = DefaultToastSettingsProvider.DefaultSuccessSettings;
        public static ToastAdornerSettings DefaultSuccessToastSettings
        {
            get => _defaultSuccessToastSettings;
            set => _defaultSuccessToastSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Error Toast Settings
        static ToastAdornerSettings _defaultErrorToastSettings = DefaultToastSettingsProvider.DefaultErrorSettings;
        public static ToastAdornerSettings DefaultErrorToastSettings
        {
            get => _defaultErrorToastSettings;
            set => _defaultErrorToastSettings = value ?? throw new ArgumentNullException();
        }
        #endregion

        #region Default Template

        static ToastViewTemplate _defaultToastViewTemplate = DefaultToastViewTemplateProvider.DefaultToastTemplate;
        public static ToastViewTemplate DefaultToastViewTemplate
        {
            get => _defaultToastViewTemplate;
            set => _defaultToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultToastViewFunc =
            (title, message) => DefaultToastViewTemplate.GetToastView(message, title);
        public static Func<string, string, UIElement> GetDefaultToastViewFunc
        {
            get => _getDefaultToastViewFunc;
            set => _getDefaultToastViewFunc = value ?? throw new ArgumentNullException();
        } 

        #endregion

        #region Default Info Template

        static ToastViewTemplate _defaultInfoToastViewTemplate = DefaultToastViewTemplateProvider.DefaultInfoTemplate;
        public static ToastViewTemplate DefaultInfoToastViewTemplate
        {
            get => _defaultInfoToastViewTemplate;
            set => _defaultInfoToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultInfoToastViewFunc =
            (title, message) => DefaultInfoToastViewTemplate.GetToastView(message, title);
        public static Func<string, string, UIElement> GetDefaultInfoToastViewFunc
        {
            get => _getDefaultInfoToastViewFunc;
            set => _getDefaultInfoToastViewFunc = value ?? throw new ArgumentNullException();
        }

        #endregion

        #region Default Warning Template

        static ToastViewTemplate _defaultWarningToastViewTemplate = DefaultToastViewTemplateProvider.DefaultWarningTemplate;
        public static ToastViewTemplate DefaultWarningToastViewTemplate
        {
            get => _defaultWarningToastViewTemplate;
            set => _defaultWarningToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultWarningToastViewFunc =
            (title, message) => DefaultWarningToastViewTemplate.GetToastView(message, title);
        public static Func<string, string, UIElement> GetDefaultWarningToastViewFunc
        {
            get => _getDefaultWarningToastViewFunc;
            set => _getDefaultWarningToastViewFunc = value ?? throw new ArgumentNullException();
        }

        #endregion

        #region Default Success Template

        static ToastViewTemplate _defaultSuccessToastViewTemplate = DefaultToastViewTemplateProvider.DefaultSuccessTemplate;
        public static ToastViewTemplate DefaultSuccessToastViewTemplate
        {
            get => _defaultSuccessToastViewTemplate;
            set => _defaultSuccessToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultSuccessToastViewFunc =
            (title, message) => DefaultSuccessToastViewTemplate.GetToastView(message, title);
        public static Func<string, string, UIElement> GetDefaultSuccessToastViewFunc
        {
            get => _getDefaultSuccessToastViewFunc;
            set => _getDefaultSuccessToastViewFunc = value ?? throw new ArgumentNullException();
        }

        #endregion

        #region Default Error Template

        static ToastViewTemplate _defaultErrorToastViewTemplate = DefaultToastViewTemplateProvider.DefaultErrorTemplate;
        public static ToastViewTemplate DefaultErrorToastViewTemplate
        {
            get => _defaultErrorToastViewTemplate;
            set => _defaultErrorToastViewTemplate = value ?? throw new ArgumentNullException();
        }

        static Func<string, string, UIElement> _getDefaultErrorToastViewFunc =
            (title, message) => DefaultErrorToastViewTemplate.GetToastView(message, title);
        public static Func<string, string, UIElement> GetDefaultErrorToastViewFunc
        {
            get => _getDefaultErrorToastViewFunc;
            set => _getDefaultErrorToastViewFunc = value ?? throw new ArgumentNullException();
        }

        #endregion
    }
}
