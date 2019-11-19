using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cashew.Toasty.Config;

namespace Cashew.Toasty.Defaults
{
    class DefaultToastSettingsProvider
    {
        static DefaultToastSettingsProvider()
        {
            DefaultToastSettings = new ToastSettings()
            {
                CanUserClose = true,
                CloseOnRightClick = true,
                CloseAfterClickAction = true,
                Lifetime = 2500,
                RefreshLifetimeOnMouseOver = true,
                DynamicLifetime = false,
                LeaveTime = 350
            };
        }

        public static ToastSettings DefaultToastSettings { get; }
    }
}
