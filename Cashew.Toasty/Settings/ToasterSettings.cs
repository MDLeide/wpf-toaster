using System;

namespace Cashew.Toasty.Settings
{
    [Serializable]
    public class ToasterSettings
    {
        public ToasterSettings() { }

        public ToasterSettings(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        /// If true, toasts that are added while another toast is still executing its enter animation
        /// are queued until the animation finishes, resulting in a smoother look.
        /// </summary>
        public bool QueueToasts { get; set; }

        #region Positioning

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

        #endregion

        #region Movement

        /// <summary>
        /// Length of time, in milliseconds, for a toast to move into position when first created, or
        /// when another toast has entered or left.
        /// </summary>
        public int MoveDuration { get; set; }

        /// <summary>
        /// Determines how existing toasts move when a new toast is added.
        /// </summary>
        public MoveStyle MoveStyle { get; set; }

        /// <summary>
        /// Determines the direction that existing toast move when a new toast is added.
        /// </summary>
        public Direction MoveDirection { get; set; }

        #endregion

        #region Enter

        /// <summary>
        /// The direction that the toast will enter from. For example, if it is set to <see cref="Direction.Right"/>,
        /// the toast will slide to the left from the right side of the screen.
        /// </summary>
        public Direction EnterFromDirection { get; set; }

        /// <summary>
        /// The area of the window that the toast will enter from.
        /// </summary>
        public Location EnterLocation { get; set; }
        
        /// <summary>
        /// Determines how the toast will enter the screen.
        /// </summary>
        public EnterStyle EnterStyle { get; set; }

        #endregion

        #region Leave

        /// <summary>
        /// Determines how the toast should leave the screen when its lifetime has expired.
        /// </summary>
        public LeaveStyle LeaveStyle { get; set; }

        /// <summary>
        /// The direction that the toast will slide if <see cref="LeaveStyle"/> is set
        /// to <see cref="LeaveStyle.SlideOut"/>
        /// </summary>
        public Direction LeaveDirection { get; set; }

        #endregion

        public ToasterSettings Clone(string newName)
        {
            var settings = new ToasterSettings(newName);

            settings.QueueToasts = QueueToasts;
            settings.EnterFromDirection = EnterFromDirection;
            settings.LeaveDirection = LeaveDirection;
            settings.MoveDirection = MoveDirection;
            settings.HorizontalAdjustment = HorizontalAdjustment;
            settings.HorizontalPadding = HorizontalPadding;
            settings.VerticalAdjustment = VerticalAdjustment;
            settings.VerticalPadding = VerticalPadding;
            settings.EnterStyle = EnterStyle;
            settings.MoveDuration = MoveDuration;
            settings.LeaveStyle = LeaveStyle;
            settings.EnterLocation = EnterLocation;
            settings.MoveStyle = MoveStyle;

            return settings;
        }
    }
}
