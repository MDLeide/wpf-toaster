namespace Cashew.Toasty.Config
{
    public class ToasterSettings
    {
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
        public Direction EnterFromDirection { get; set; }

        /// <summary>
        /// The area of the window that the toast will enter from.
        /// </summary>
        public Location EnterLocation { get; set; }
        
        /// <summary>
        /// Determines how the toast will enter the screen.
        /// </summary>
        public EnterStyle EnterStyle { get; set; }

        /// <summary>
        /// Determines how the toast should leave the screen when its lifetime has expired.
        /// </summary>
        public LeaveStyle LeaveStyle { get; set; }

        /// <summary>
        /// The direction that the toast will slide if <see cref="LeaveStyle"/> is set
        /// to <see cref="LeaveStyle.SlideOut"/>
        /// </summary>
        public Direction LeaveDirection { get; set; }

        /// <summary>
        /// If true, toasts that are added while another toast is still executing its enter animation
        /// are queued until the animation finishes, resulting in a smoother look.
        /// </summary>
        public bool QueueToasts { get; set; }
    }

    public enum EnterStyle
    {
        /// <summary>
        /// The toast will appear instantly on the screen.
        /// </summary>
        PopIn,
        /// <summary>
        /// The toast will fade from transparent to opaque.
        /// </summary>
        FadeIn,
        /// <summary>
        /// The toast will slide in from a side of the screen.
        /// </summary>
        SlideIn
    }

    public enum MoveStyle
    {
        /// <summary>
        /// New toasts will push existing toasts in the <see cref="ToasterSettings.MoveDirection"/>.
        /// </summary>
        Push,
        /// <summary>
        /// New toasts will stack next to existing toasts depending on the <see cref="ToasterSettings.MoveDirection"/>
        /// </summary>
        Stack
    }
}
