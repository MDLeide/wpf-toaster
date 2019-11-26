using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Defaults
{
    class DefaultToastSettingsProvider
    {
        public static ToastAdornerSettings DefaultToastSettings { get; } = GetDefaultSettings("default");
        public static ToastAdornerSettings DefaultInfoSettings { get; } = GetDefaultSettings("default-info");
        public static ToastAdornerSettings DefaultWarningSettings { get; } = GetDefaultSettings("default-warning");
        public static ToastAdornerSettings DefaultSuccessSettings { get; } = GetDefaultSettings("default-success");
        public static ToastAdornerSettings DefaultErrorSettings { get; } = GetDefaultSettings("default-error");

        static ToastAdornerSettings GetDefaultSettings(string name)
        {
            var settings = new ToastAdornerSettings(name)
            {
                CanUserClose = true,
                CloseOnRightClick = true,
                CloseAfterClickAction = true,
                Lifetime = 2500,
                RefreshLifetimeOnMouseOver = true,
                DynamicLifetime = false,
                LeaveTime = 350
            };

            return settings;
        }
    }
}
