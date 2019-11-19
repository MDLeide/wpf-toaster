namespace Cashew.Toasty.Config
{
    public class ToastSettings
    {
        /// <summary>
        /// True if the user can close the toast.
        /// </summary>
        public bool CanUserClose { get; set; }

        /// <summary>
        /// True if right clicking on the toast closes it.
        /// </summary>
        public bool CloseOnRightClick { get; set; }

        /// <summary>
        /// True if the toast should automatically close after executing the click action.
        /// </summary>
        public bool CloseAfterClickAction { get; set; }

        /// <summary>
        /// How long the toast is displayed for, in milliseconds. 0 for infinite lifetime.
        /// </summary>
        public int Lifetime { get; set; }
        /// <summary>
        /// True if the toast's lifetime timer should reset to 0 when the mouse is over the toast.
        /// </summary>
        public bool RefreshLifetimeOnMouseOver { get; set; }
        /// <summary>
        /// True to dynamically calculate the toast's lifetime based on the contents of the message.
        /// </summary>
        public bool DynamicLifetime { get; set; }
        /// <summary>
        /// A time, in milliseconds, that will be added to the dynamic lifetime regardless of message length.
        /// </summary>
        public int DynamicLifetimeBase { get; set; }
        /// <summary>
        /// The number of milliseconds per character to add to the dynamic lifetime.
        /// </summary>
        public int DynamicLifetimeMillisecondsPerCharacter { get; set; }
        /// <summary>
        /// Minimum number of milliseconds for dynamic lifetime.
        /// </summary>
        public int DynamicLifetimeMinimum { get; set; }
        /// <summary>
        /// Maximum number of milliseconds for dynamic lifetime.
        /// </summary>
        public int DynamicLifetimeMaximum { get; set; }

        /// <summary>
        /// How long, in milliseconds, the toast takes to leave the screen. The toast will
        /// start leaving after <see cref="Lifetime"/> - <see cref="LeaveTime"/> have elapsed. Use
        /// 0 for instant removal.
        /// </summary>
        public int LeaveTime { get; set; }
    }
}