namespace Cashew.Toasty.Config
{
    public enum LeaveStyle
    {
        /// <summary>
        /// The toast will simply disappear when its lifetime expires.
        /// </summary>
        None,
        /// <summary>
        /// The toast will slide off the screen when its lifetime expires.
        /// </summary>
        SlideOut,
        /// <summary>
        /// The toast will fade out when its lifetime expires.
        /// </summary>
        FadeOut
    }
}