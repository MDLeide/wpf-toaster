using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cashew.Toasty.Config;

namespace Cashew.Toasty.Defaults
{
    static class DefaultToasterSettingsProvider
    {
        static DefaultToasterSettingsProvider()
        {
            DefaultToasterSettings = new ToasterSettings
            {
                LeaveStyle = LeaveStyle.FadeOut,
                EnterLocation = Location.BottomRight,
                FromDirection = Direction.Bottom,
                HorizontalAdjustment = -15,
                MoveDuration = 500,
                VerticalPadding = 15
            };
        }

        public static ToasterSettings DefaultToasterSettings { get; }
    }
}
