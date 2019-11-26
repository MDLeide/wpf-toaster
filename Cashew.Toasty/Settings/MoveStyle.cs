namespace Cashew.Toasty.Settings
{
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