using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cashew.Toasty
{
    public class ToastAdorner : Adorner
    {
        readonly VisualCollection _visuals;
        readonly ContentPresenter _contentPresenter;


        public ToastAdorner(UIElement adornedElement, UIElement toastView)
            : base(adornedElement)
        {
            ToastView = toastView;
            _visuals = new VisualCollection(this);
            _contentPresenter = new ContentPresenter();
            _contentPresenter.Content = toastView;
            _visuals.Add(_contentPresenter);
        }


        internal event EventHandler CloseRequested;


        public UIElement ToastView { get; }

        protected override int VisualChildrenCount => _visuals.Count;

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            nameof(Top), typeof(double), typeof(ToastAdorner), new PropertyMetadata(default(double)));

        public double Top
        {
            get => (double)GetValue(TopProperty);
            set
            {
                SetValue(TopProperty, value);
                InvalidateArrange();
            }
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            nameof(Left), typeof(double), typeof(ToastAdorner), new PropertyMetadata(default(double)));

        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set
            {
                SetValue(LeftProperty, value);
                InvalidateArrange();
            }
        }


        public void RequestClose()
        {
            CloseRequested?.Invoke(this, new EventArgs());
        }


        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(
                new Rect(
                    Left,
                    Top,
                    finalSize.Width,
                    finalSize.Height));

            return _contentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TopProperty || e.Property == LeftProperty)
                InvalidateArrange();

            base.OnPropertyChanged(e);
        }
    }
}