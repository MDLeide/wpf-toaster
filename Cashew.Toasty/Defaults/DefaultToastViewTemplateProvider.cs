using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cashew.Toasty.Config;
using Cashew.Toasty.Properties;

namespace Cashew.Toasty.Defaults
{
    static class DefaultToastViewTemplateProvider
    {
        static DefaultToastViewTemplateProvider()
        {
            DefaultToastTemplate = new ToastTemplate
            {
                ImageSize = 25,
                ShowCloseButton = true,
                CloseButtonStrokeBrush = new SolidColorBrush(Colors.White),
                CloseButtonFillBrush = new SolidColorBrush(Colors.White),
                AutoWidth = false,
                AutoHeight = true,
                Width = 325,
                TitleBackgroundBrush = new SolidColorBrush(Colors.Green),
                TitleForegroundBrush = new SolidColorBrush(Colors.White),
                TitleFontSize = 15,
                TitleFontWeight = FontWeights.Bold,
                TitleMargin = new Thickness(8, 8, 8, 4),
                TitleVerticalAlignment = VerticalAlignment.Center,
                TitleHorizontalAlignment = HorizontalAlignment.Left,
                MessageBackgroundBrush = new SolidColorBrush(Colors.Green),
                MessageForegroundBrush = new SolidColorBrush(Colors.White),
                MessageFontSize = 12,
                MessageFontWeight = FontWeights.Normal,
                MessageMargin = new Thickness(8, 4, 8, 8),
                MessageVerticalAlignment = VerticalAlignment.Center,
                MessageHorizontalAlignment = HorizontalAlignment.Left,
                InnerBorderThickness = new Thickness(0),
                InnerBorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                CornerRadius = 3,
                Opacity = 1,
                ImageSource = ImageFromBitmap(Resources.final_info_icon_white)
            };
        }
        
        public static ToastTemplate DefaultToastTemplate { get; }

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
