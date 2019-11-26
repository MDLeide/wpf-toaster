using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Defaults
{
    static class DefaultToasterSettingsProvider
    {
        static DefaultToasterSettingsProvider()
        {
            DefaultToasterSettings = new ToasterSettings("default")
            {
                LeaveStyle = LeaveStyle.FadeOut,
                EnterLocation = Location.BottomRight,
                EnterFromDirection = Direction.Down,
                HorizontalAdjustment = -15,
                MoveDuration = 500,
                VerticalPadding = 15,
                QueueToasts = true,
                MoveStyle = MoveStyle.Push,
                MoveDirection = Direction.Up,
                EnterStyle = EnterStyle.SlideIn
            };
        }

        public static ToasterSettings DefaultToasterSettings { get; }
    }
}
