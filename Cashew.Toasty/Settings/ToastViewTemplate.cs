using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cashew.Toasty.Properties;
using Brush = System.Windows.Media.Brush;

namespace Cashew.Toasty.Settings
{
    /// <summary>
    /// Provides properties that can be used to configure the default <see cref="ToastView"/>.
    /// </summary>
    public class ToastViewTemplate
    {
        public double Opacity { get; set; }

        #region Close Button

        public bool ShowCloseButton { get; set; }
        public Brush CloseButtonStrokeBrush { get; set; }
        public Brush CloseButtonFillBrush { get; set; }

        #endregion

        #region Dimensions

        public bool AutoWidth { get; set; }
        public double Width { get; set; }
        public bool AutoHeight { get; set; }
        public double Height { get; set; }

        #endregion

        #region Image

        public StandardImages? StandardImage { get; set; }
        public ImageSource ImageSource { get; set; }
        public double ImageSize { get; set; }

        #endregion

        #region Title

        public Brush TitleBackgroundBrush { get; set; }
        public Brush TitleForegroundBrush { get; set; }
        public double TitleFontSize { get; set; }
        public FontWeight TitleFontWeight { get; set; }
        public Thickness TitleMargin { get; set; }
        public VerticalAlignment TitleVerticalAlignment { get; set; }
        public HorizontalAlignment TitleHorizontalAlignment { get; set; }

        #endregion

        #region Message

        public Brush MessageBackgroundBrush { get; set; }
        public Brush MessageForegroundBrush { get; set; }
        public double MessageFontSize { get; set; }
        public FontWeight MessageFontWeight { get; set; }
        public Thickness MessageMargin { get; set; }
        public VerticalAlignment MessageVerticalAlignment { get; set; }
        public HorizontalAlignment MessageHorizontalAlignment { get; set; }

        #endregion

        #region Border

        public Thickness InnerBorderThickness { get; set; }
        public Brush InnerBorderBrush { get; set; }
        public double CornerRadius { get; set; }

        #endregion

        public ToastView GetToastView(string message, string title)
        {
            var toastView = new ToastView();

            if (StandardImage.HasValue)
            {
                switch (StandardImage)
                {
                    case StandardImages.WarningBlack:
                        toastView.IconImageSource = ImageFromBitmap(Resources.warning_black);
                        break;
                    case StandardImages.WarningWhite:
                        toastView.IconImageSource = ImageFromBitmap(Resources.warning_white);
                        break;
                    case StandardImages.ErrorBlack:
                        toastView.IconImageSource = ImageFromBitmap(Resources.error_black);
                        break;
                    case StandardImages.ErrorWhite:
                        toastView.IconImageSource = ImageFromBitmap(Resources.error_white);
                        break;
                    case StandardImages.SuccessWhite:
                        toastView.IconImageSource = ImageFromBitmap(Resources.success_white);
                        break;
                    case StandardImages.SuccessBlack:
                        toastView.IconImageSource = ImageFromBitmap(Resources.success_black);
                        break;
                    case StandardImages.InfoWhite:
                        toastView.IconImageSource = ImageFromBitmap(Resources.info_white);
                        break;
                    case StandardImages.InfoBlack:
                        toastView.IconImageSource = ImageFromBitmap(Resources.info_black);
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                toastView.IconImageSource = ImageSource;
            }
            
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

        static BitmapImage ImageFromBitmap(Bitmap bitmap)
        {
            using (var mem = new MemoryStream())
            {
                bitmap.Save(mem, ImageFormat.Png);
                mem.Position = 0;

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = mem;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}