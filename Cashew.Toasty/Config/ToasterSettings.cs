using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cashew.Toasty
{
    public class ToasterSettings
    {
        /// <summary>
        /// Length of time, in milliseconds, for a toast to move into position when first created, or
        /// when another toast has entered or left.
        /// </summary>
        public int MoveDuration { get; set; }

        /// <summary>
        /// Vertical padding used to separate toasts when they are stacked vertically.
        /// </summary>
        public double VerticalPadding { get; set; }

        /// <summary>
        /// Horizontal padding used to separate toasts when they are stack horizontally.
        /// </summary>
        public double HorizontalPadding { get; set; }

        /// <summary>
        /// A value used to adjust the vertical position of all toasts. You can use this to account
        /// for window borders, raising a horizontal toast from the bottom of the window a bit, etc.
        /// </summary>
        public double VerticalAdjustment { get; set; }

        /// <summary>
        /// A value used to adjust the horizontal position of all toasts. You can use this to account
        /// for window borders, pushing a vertical toast in from the side of the window, etc.
        /// </summary>
        public double HorizontalAdjustment { get; set; }

        /// <summary>
        /// The direction that the toast will enter from. For example, if it is set to <see cref="Direction.Right"/>,
        /// the toast will slide to the left from the right side of the screen.
        /// </summary>
        public static Direction FromDirection { get; set; }

        /// <summary>
        /// The area of the window that the toast will enter from.
        /// </summary>
        public static Location EnterLocation { get; set; }
    }
}
