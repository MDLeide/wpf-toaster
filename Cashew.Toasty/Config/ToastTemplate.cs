using System.Windows;
using System.Windows.Media;

namespace Cashew.Toasty
{
    /// <summary>
    /// Provides properties that can be used to configure the default <see cref="ToastView"/>.
    /// </summary>
    public class ToastTemplate
    {
        public bool ShowCloseButton { get; set; }

        public Brush CloseButtonStrokeBrush { get; set; }
        public Brush CloseButtonFillBrush { get; set; }

        public bool AutoWidth { get; set; }
        public double Width { get; set; }
        public bool AutoHeight { get; set; }
        public double Height { get; set; }
        public double Opacity { get; set; }

        public ImageSource ImageSource { get; set; }
        public double ImageSize { get; set; }

        public Brush TitleBackgroundBrush { get; set; }
        public Brush TitleForegroundBrush { get; set; }
        public double TitleFontSize { get; set; }
        public FontWeight TitleFontWeight { get; set; }
        public Thickness TitleMargin { get; set; }
        public VerticalAlignment TitleVerticalAlignment { get; set; }
        public HorizontalAlignment TitleHorizontalAlignment { get; set; }

        public Brush MessageBackgroundBrush { get; set; }
        public Brush MessageForegroundBrush { get; set; }
        public double MessageFontSize { get; set; }
        public FontWeight MessageFontWeight { get; set; }
        public Thickness MessageMargin { get; set; }
        public VerticalAlignment MessageVerticalAlignment { get; set; }
        public HorizontalAlignment MessageHorizontalAlignment { get; set; }

        public Thickness InnerBorderThickness { get; set; }
        public Brush InnerBorderBrush { get; set; }
        public double CornerRadius { get; set; }


        public ToastView GetToastView(string title, string message)
        {
            var toastView = new ToastView();

            toastView.IconImageSource = ImageSource;
            toastView.IconImageSize = ImageSize;

            toastView.ShowCloseButton = ShowCloseButton;

            toastView.CloseButtonFillBrush = CloseButtonFillBrush;
            toastView.CloseButtonStrokeBrush = CloseButtonStrokeBrush;

            toastView.Height = AutoHeight ? double.NaN : Height;
            toastView.Width = AutoWidth ? double.NaN : Width;

            toastView.ToastTitle = title;
            toastView.TitleBackgroundBrush = TitleBackgroundBrush;
            toastView.TitleForegroundBrush = TitleForegroundBrush;
            toastView.TitleFontSize = TitleFontSize;
            toastView.TitleFontWeight = TitleFontWeight;
            toastView.TitleMargin = TitleMargin;
            toastView.TitleVerticalAlignment = TitleVerticalAlignment;
            toastView.TitleHorizontalAlignment = TitleHorizontalAlignment;

            toastView.Message = message;
            toastView.MessageBackgroundBrush = MessageBackgroundBrush;
            toastView.MessageForegroundBrush = MessageForegroundBrush;
            toastView.MessageFontSize = MessageFontSize;
            toastView.MessageFontWeight = MessageFontWeight;
            toastView.MessageMargin = MessageMargin;
            toastView.MessageVerticalAlignment = MessageVerticalAlignment;
            toastView.MessageHorizontalAlignment = MessageHorizontalAlignment;

            toastView.InnerBorderThickness = InnerBorderThickness;
            toastView.InnerBorderBrush = InnerBorderBrush;
            toastView.CornerRadius = new CornerRadius(CornerRadius);
            toastView.Opacity = Opacity;

            return toastView;
        }
    }
}