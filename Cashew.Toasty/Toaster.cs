using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cashew.Toasty.Config;
using Cashew.Toasty.Defaults;

namespace Cashew.Toasty
{
    public class Toaster
    {
        readonly List<ToastAdorner> _adorners = new List<ToastAdorner>();
        readonly AdornerLayer _adornerLayer;
        readonly ToasterSettings _settings;
        readonly Func<string, string, UIElement> _getDefaultView;

        
        public Toaster(Window window, ToasterSettings settings = null, Func<string, string, UIElement> getDefaultView = null, ToastSettings defaultToastSettings = null)
        {
            _getDefaultView = getDefaultView ?? DefaultSettings.GetDefaultToastViewFunc;
            _adornerLayer = GetWindowAdornerLayer(window, out var elementToAdorn);
            _settings = settings ?? DefaultSettings.DefaultToasterSettings;
            DefaultToastSettings = defaultToastSettings ?? DefaultSettings.DefaultToastSettings;
            ElementToAdorn = elementToAdorn;
            Window = window;
            MoveDuration = new Duration(new TimeSpan(0, 0, 0, 0, _settings.MoveDuration));
        }


        public event EventHandler IsEmpty;


        public ToastSettings DefaultToastSettings { get; set; }

        public Window Window { get; }
        public UIElement ElementToAdorn { get; }

        Duration MoveDuration { get; }
        double VerticalPadding => _settings.VerticalPadding;
        double HorizontalPadding => _settings.HorizontalPadding;
        double VerticalAdjustment => _settings.VerticalAdjustment;
        double HorizontalAdjustment => _settings.HorizontalAdjustment;
        Direction FromDirection => _settings.FromDirection;
        Location EnterLocation => _settings.EnterLocation;


        public void Show(
            string title, 
            string message, 
            ToastSettings toastSettings = null,
            UIElement toastView = null,
            Action clickAction = null)
        {
            if (toastSettings == null && DefaultToastSettings == null)
                throw new ArgumentNullException(nameof(toastSettings), "Toast settings cannot be null if DefaultToastSettings has not been set.");
            
            if (toastSettings == null)
                toastSettings = DefaultToastSettings;

            if (!toastSettings.CanUserClose && toastSettings.Lifetime <= 0)
                throw new InvalidOperationException("Toast is configured to disallow user closing and to never expire.");

            var adorner = new ToastAdorner(ElementToAdorn, toastView ?? _getDefaultView(title, message));
            ToastConfigurator.Configure(adorner, Window, toastSettings, title, message, Remove, clickAction);
            Add(adorner);
        }

        void Add(ToastAdorner toast)
        {
            toast.Loaded += (s, e) =>
            {
                toast.Top = GetInitialTop(toast);
                toast.Left = GetInitialLeft(toast);
                MoveForward();
            };
            
            _adorners.Add(toast);
            _adornerLayer.Add(toast);
        }

        void Remove(ToastAdorner toast)
        {
            var index = _adorners.IndexOf(toast) - 1;

            if (!_adorners.Remove(toast))
                return;

            RemoveDelegate del = _adornerLayer.Remove;
            Application.Current?.Dispatcher?.Invoke(del, toast);

            if (!_adorners.Any())
            {
                IsEmpty?.Invoke(this, new EventArgs());
                return;
            }

            if (index < 0)
                return;

            MoveBackwards(index);
        }
        

        AdornerLayer GetWindowAdornerLayer(Window window, out UIElement uiElement)
        {
            uiElement = window;
            var layer = AdornerLayer.GetAdornerLayer(window);
            if (layer != null)
                return layer;

            if (window.Content is UIElement ui)
            {
                uiElement = ui;
                if (ui is Visual v)
                    layer = AdornerLayer.GetAdornerLayer(v);
            }

            return layer;
        }

        delegate void RemoveDelegate(ToastAdorner adorner);
        delegate void AnimateDelegate();

        void MoveForward()
        {
            if (FromDirection == Direction.Top || FromDirection == Direction.Bottom)
                MoveVertical(FromDirection == Direction.Top);
            else
                MoveHorizontal(FromDirection == Direction.Left);
        }

        void MoveBackwards(int startingIndex)
        {
            if (FromDirection == Direction.Top || FromDirection == Direction.Bottom)
                MoveVertical(FromDirection == Direction.Top, startingIndex);
            else
                MoveHorizontal(FromDirection == Direction.Left, startingIndex);
        }

        void MoveHorizontal(bool moveRight)
        {
            MoveHorizontal(moveRight, _adorners.Count - 1);
        }

        void MoveHorizontal(bool moveRight, int startingIndex)
        {
            if (!_adorners.Any())
                return;

            var targetLeft = GetInitialLeft(_adorners.Last());

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                targetLeft += _adorners[i].ActualWidth * (moveRight ? 1 : -1);
                var targetLeftCopy = targetLeft;
                targetLeft += moveRight ? HorizontalPadding : -HorizontalPadding;

                if (i > startingIndex)
                    continue;

                var index = i;
                void Animate()
                {
                    var animation = new DoubleAnimation(_adorners[index].Left, targetLeftCopy, MoveDuration);
                    _adorners[index].BeginAnimation(ToastAdorner.LeftProperty, animation);
                }
                Application.Current?.Dispatcher?.Invoke((AnimateDelegate) Animate);
            }
        }

        void MoveVertical(bool moveDown)
        {
            MoveVertical(moveDown, _adorners.Count - 1);
        }

        void MoveVertical(bool moveDown, int startingIndex)
        {
            if (!_adorners.Any())
                return;

            var targetTop = GetInitialTop(_adorners.Last());

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                targetTop += _adorners[i].ActualHeight * (moveDown ? 1 : -1);
                var targetTopCopy = targetTop;
                targetTop += moveDown ? VerticalPadding : -VerticalPadding;
                
                if (i > startingIndex)
                    continue;

                var index = i;
                void Animate()
                {
                    var animation = new DoubleAnimation(_adorners[index].Top, targetTopCopy, MoveDuration);
                    _adorners[index].BeginAnimation(ToastAdorner.TopProperty, animation);
                }
                Application.Current?.Dispatcher?.Invoke((AnimateDelegate) Animate);
            }
        }
        
        double GetInitialLeft(ToastAdorner adorner)
        {
            if (EnterLocation == Location.Top && FromDirection == Direction.Top ||
                EnterLocation == Location.Bottom && FromDirection == Direction.Bottom)
                return GetWidth() / 2 - adorner.ActualWidth / 2 + HorizontalAdjustment;
            
            if (EnterLocation == Location.TopLeft)
            {
                if (FromDirection == Direction.Left)
                    return -adorner.ActualWidth + HorizontalAdjustment;
                if (FromDirection == Direction.Top)
                    return HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.BottomLeft)
            {
                if (FromDirection == Direction.Left)
                    return -adorner.ActualWidth + HorizontalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.TopRight)
            {
                if (FromDirection == Direction.Right)
                    return GetWidth() + HorizontalAdjustment;
                if (FromDirection == Direction.Top)
                    return GetWidth() - adorner.ActualWidth + HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.BottomRight)
            {
                if (FromDirection == Direction.Right)
                    return GetWidth() + HorizontalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return GetWidth() - adorner.ActualWidth + HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            throw new InvalidOperationException();
        }

        double GetInitialTop(ToastAdorner adorner)
        {
            if (EnterLocation == Location.Left && FromDirection == Direction.Left ||
                EnterLocation == Location.Right && FromDirection == Direction.Right)
                return GetHeight() / 2 - adorner.ActualHeight / 2 + VerticalAdjustment;

            if (EnterLocation == Location.TopLeft)
            {
                if (FromDirection == Direction.Left)
                    return VerticalAdjustment;
                if (FromDirection == Direction.Top)
                    return -adorner.ActualHeight + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.TopRight)
            {
                if (FromDirection == Direction.Right)
                    return VerticalAdjustment;
                if (FromDirection == Direction.Top)
                    return -adorner.ActualHeight + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.BottomLeft)
            {
                if (FromDirection == Direction.Left)
                    return GetHeight() - adorner.ActualHeight + VerticalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return GetHeight() + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (EnterLocation == Location.BottomRight)
            {
                if (FromDirection == Direction.Right)
                    return GetHeight() - adorner.ActualHeight + VerticalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return GetHeight() + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            throw new InvalidOperationException();
        }

        double GetHeight()
        {
            if (ElementToAdorn is FrameworkElement frameworkElement)
                return _adornerLayer.ActualHeight - frameworkElement.Margin.Left;
            return 0;
        }

        double GetWidth()
        {
            if (ElementToAdorn is FrameworkElement frameworkElement)
                return _adornerLayer.ActualWidth - frameworkElement.Margin.Top;
            return 0;
        }
    }
}