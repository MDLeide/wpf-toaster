using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cashew.Toasty
{
    /// <summary>
    /// Interaction logic for ToastView.xaml
    /// </summary>
    public partial class ToastView : UserControl
    {
        public ToastView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }


        public EventHandler CloseButtonClicked;


        #region Close 

        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(
            nameof(ShowCloseButton), typeof(bool), typeof(ToastView), new PropertyMetadata(default(bool)));

        public bool ShowCloseButton
        {
            get => (bool) GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        #endregion

        #region Icon

        public static readonly DependencyProperty IconImageSizeProperty = DependencyProperty.Register(
            nameof(IconImageSize), typeof(double), typeof(ToastView), new PropertyMetadata(default(double)));

        public double IconImageSize
        {
            get => (double) GetValue(IconImageSizeProperty);
            set => SetValue(IconImageSizeProperty, value);
        }

        public static readonly DependencyProperty IconImageSourceProperty = DependencyProperty.Register(
            nameof(IconImageSource), typeof(ImageSource), typeof(ToastView),
            new PropertyMetadata(default(ImageSource)));

        public ImageSource IconImageSource
        {
            get => (ImageSource) GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        #endregion

        #region Inner Border

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(ToastView), new PropertyMetadata(default(CornerRadius)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty InnerBorderThicknessProperty = DependencyProperty.Register(
            nameof(InnerBorderThickness), typeof(Thickness), typeof(ToastView),
            new PropertyMetadata(default(Thickness)));

        public Thickness InnerBorderThickness
        {
            get => (Thickness) GetValue(InnerBorderThicknessProperty);
            set => SetValue(InnerBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty InnerBorderBrushProperty = DependencyProperty.Register(
            nameof(InnerBorderBrush), typeof(Brush), typeof(ToastView), new PropertyMetadata(default(Brush)));

        public Brush InnerBorderBrush
        {
            get => (Brush) GetValue(InnerBorderBrushProperty);
            set => SetValue(InnerBorderBrushProperty, value);
        }

        #endregion

        #region Close Button

        public static readonly DependencyProperty CloseButtonStrokeBrushProperty = DependencyProperty.Register(
            nameof(CloseButtonStrokeBrush), typeof(Brush), typeof(ToastView),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush CloseButtonStrokeBrush
        {
            get => (Brush) GetValue(CloseButtonStrokeBrushProperty);
            set => SetValue(CloseButtonStrokeBrushProperty, value);
        }

        public static readonly DependencyProperty CloseButtonFillBrushProperty = DependencyProperty.Register(
            nameof(CloseButtonFillBrush), typeof(Brush), typeof(ToastView),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush CloseButtonFillBrush
        {
            get => (Brush) GetValue(CloseButtonFillBrushProperty);
            set => SetValue(CloseButtonFillBrushProperty, value);
        }

        #endregion

        #region Title

        public static readonly DependencyProperty ToastTitleProperty = DependencyProperty.Register(
            nameof(ToastTitle), typeof(string), typeof(ToastView), new PropertyMetadata(default(string)));

        public string ToastTitle
        {
            get => (string) GetValue(ToastTitleProperty);
            set => SetValue(ToastTitleProperty, value);
        }

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(
            nameof(TitleFontSize), typeof(double), typeof(ToastView), new PropertyMetadata(default(double)));

        public double TitleFontSize
        {
            get => (double) GetValue(TitleFontSizeProperty);
            set => SetValue(TitleFontSizeProperty, value);
        }

        public static readonly DependencyProperty TitleFontWeightProperty = DependencyProperty.Register(
            nameof(TitleFontWeight), typeof(FontWeight), typeof(ToastView), new PropertyMetadata(default(FontWeight)));

        public FontWeight TitleFontWeight
        {
            get => (FontWeight) GetValue(TitleFontWeightProperty);
            set => SetValue(TitleFontWeightProperty, value);
        }

        public static readonly DependencyProperty TitleBackgroundBrushProperty = DependencyProperty.Register(
            nameof(TitleBackgroundBrush), typeof(Brush), typeof(ToastView), new PropertyMetadata(default(Brush)));

        public Brush TitleBackgroundBrush
        {
            get => (Brush) GetValue(TitleBackgroundBrushProperty);
            set => SetValue(TitleBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty TitleForegroundBrushProperty = DependencyProperty.Register(
            nameof(TitleForegroundBrush), typeof(Brush), typeof(ToastView), new PropertyMetadata(default(Brush)));

        public Brush TitleForegroundBrush
        {
            get => (Brush) GetValue(TitleForegroundBrushProperty);
            set => SetValue(TitleForegroundBrushProperty, value);
        }

        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register(
            nameof(TitleMargin), typeof(Thickness), typeof(ToastView), new PropertyMetadata(default(Thickness)));

        public Thickness TitleMargin
        {
            get => (Thickness) GetValue(TitleMarginProperty);
            set => SetValue(TitleMarginProperty, value);
        }

        public static readonly DependencyProperty TitleHorizontalAlignmentProperty = DependencyProperty.Register(
            nameof(TitleHorizontalAlignment), typeof(HorizontalAlignment), typeof(ToastView),
            new PropertyMetadata(default(HorizontalAlignment)));

        public HorizontalAlignment TitleHorizontalAlignment
        {
            get => (HorizontalAlignment) GetValue(TitleHorizontalAlignmentProperty);
            set => SetValue(TitleHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty TitleVerticalAlignmentProperty = DependencyProperty.Register(
            nameof(TitleVerticalAlignment), typeof(VerticalAlignment), typeof(ToastView),
            new PropertyMetadata(default(VerticalAlignment)));

        public VerticalAlignment TitleVerticalAlignment
        {
            get => (VerticalAlignment) GetValue(TitleVerticalAlignmentProperty);
            set => SetValue(TitleVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty TitleTextWrappingProperty = DependencyProperty.Register(
            nameof(TitleTextWrapping), typeof(TextWrapping), typeof(ToastView),
            new PropertyMetadata(default(TextWrapping)));

        public TextWrapping TitleTextWrapping
        {
            get => (TextWrapping) GetValue(TitleTextWrappingProperty);
            set => SetValue(TitleTextWrappingProperty, value);
        }

        #endregion

        #region Message

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(string), typeof(ToastView), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageFontSizeProperty = DependencyProperty.Register(
            nameof(MessageFontSize), typeof(double), typeof(ToastView), new PropertyMetadata(default(double)));

        public double MessageFontSize
        {
            get => (double) GetValue(MessageFontSizeProperty);
            set => SetValue(MessageFontSizeProperty, value);
        }

        public static readonly DependencyProperty MessageFontWeightProperty = DependencyProperty.Register(
            nameof(MessageFontWeight), typeof(FontWeight), typeof(ToastView),
            new PropertyMetadata(default(FontWeight)));

        public FontWeight MessageFontWeight
        {
            get => (FontWeight) GetValue(MessageFontWeightProperty);
            set => SetValue(MessageFontWeightProperty, value);
        }

        public static readonly DependencyProperty MessageBackgroundBrushProperty = DependencyProperty.Register(
            nameof(MessageBackgroundBrush), typeof(Brush), typeof(ToastView), new PropertyMetadata(default(Brush)));

        public Brush MessageBackgroundBrush
        {
            get => (Brush) GetValue(MessageBackgroundBrushProperty);
            set => SetValue(MessageBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty MessageForegroundBrushProperty = DependencyProperty.Register(
            nameof(MessageForegroundBrush), typeof(Brush), typeof(ToastView), new PropertyMetadata(default(Brush)));

        public Brush MessageForegroundBrush
        {
            get => (Brush) GetValue(MessageForegroundBrushProperty);
            set => SetValue(MessageForegroundBrushProperty, value);
        }

        public static readonly DependencyProperty MessageMarginProperty = DependencyProperty.Register(
            nameof(MessageMargin), typeof(Thickness), typeof(ToastView), new PropertyMetadata(default(Thickness)));

        public Thickness MessageMargin
        {
            get => (Thickness) GetValue(MessageMarginProperty);
            set => SetValue(MessageMarginProperty, value);
        }

        public static readonly DependencyProperty MessageHorizontalAlignmentProperty = DependencyProperty.Register(
            nameof(MessageHorizontalAlignment), typeof(HorizontalAlignment), typeof(ToastView),
            new PropertyMetadata(default(HorizontalAlignment)));

        public HorizontalAlignment MessageHorizontalAlignment
        {
            get => (HorizontalAlignment) GetValue(MessageHorizontalAlignmentProperty);
            set => SetValue(MessageHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty MessageVerticalAlignmentProperty = DependencyProperty.Register(
            nameof(MessageVerticalAlignment), typeof(VerticalAlignment), typeof(ToastView),
            new PropertyMetadata(default(VerticalAlignment)));

        public VerticalAlignment MessageVerticalAlignment
        {
            get => (VerticalAlignment) GetValue(MessageVerticalAlignmentProperty);
            set => SetValue(MessageVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty MessageTextWrappingProperty = DependencyProperty.Register(
            nameof(MessageTextWrapping), typeof(TextWrapping), typeof(ToastView),
            new PropertyMetadata(default(TextWrapping)));

        public TextWrapping MessageTextWrapping
        {
            get => (TextWrapping) GetValue(MessageTextWrappingProperty);
            set => SetValue(MessageTextWrappingProperty, value);
        }

        #endregion

        void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseButtonClicked?.Invoke(this, new EventArgs());
        }
    }
}

