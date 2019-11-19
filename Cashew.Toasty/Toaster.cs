using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
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
        bool InvertStacking => _settings.InvertStacking;
        LeaveStyle LeaveStyle => _settings.LeaveStyle;
        Direction LeaveDirection => _settings.LeaveDirection;

        HorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (EnterLocation == Location.BottomLeft || 
                    EnterLocation == Location.TopLeft ||
                    EnterLocation == Location.Left)
                    return HorizontalAlignment.Left;
                if (EnterLocation == Location.BottomRight ||
                    EnterLocation == Location.TopRight ||
                    EnterLocation == Location.Right)
                    return HorizontalAlignment.Right;
                return HorizontalAlignment.Center;
            }
        }

        VerticalAlignment VerticalAlignment
        {
            get
            {
                if (EnterLocation == Location.TopLeft ||
                    EnterLocation == Location.Top ||
                    EnterLocation == Location.TopRight)
                    return VerticalAlignment.Top;
                if (EnterLocation == Location.BottomLeft ||
                    EnterLocation == Location.Bottom ||
                    EnterLocation == Location.BottomRight)
                    return VerticalAlignment.Bottom;
                return VerticalAlignment.Center;
            }
        }

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
            Configure(adorner, toastSettings, clickAction, title, message);
            Add(adorner);
        }


        delegate void RemoveDelegate(ToastAdorner adorner);
        delegate void AnimateDelegate();
        

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


        #region Toast Configuration

        void Configure(ToastAdorner adorner, ToastSettings settings, Action clickAction, string title, string message)
        {
            ConfigureClose(adorner, settings);
            ConfigureClickAction(adorner, settings, clickAction);
            var lifetime = settings.DynamicLifetime ? GetDynamicLifetime(settings, title, message) : settings.Lifetime;
            ConfigureLeave(adorner, settings, lifetime);
        }

        void ConfigureClose(ToastAdorner adorner, ToastSettings settings)
        {
            if (!settings.CanUserClose)
                return;

            adorner.CloseRequested += (s, e) => Remove(adorner);
            if (settings.CloseOnRightClick)
                adorner.ToastView.MouseRightButtonUp += (s, e) => Remove(adorner);
        }

        void ConfigureClickAction(ToastAdorner adorner, ToastSettings settings, Action clickAction)
        {
            adorner.ToastView.MouseLeftButtonUp += (s, e) =>
            {
                clickAction();
                if (settings.CloseAfterClickAction)
                    Remove(adorner);
            };
        }

        int GetDynamicLifetime(ToastSettings settings, string title, string message)
        {
            var lifetime =
                (
                    (string.IsNullOrEmpty(title) ? 0 : title.Length) +
                    (string.IsNullOrEmpty(message) ? 0 : message.Length)
                ) * settings.DynamicLifetimeMillisecondsPerCharacter +
                settings.DynamicLifetimeBase;

            if (lifetime < settings.DynamicLifetimeMinimum)
                lifetime = settings.DynamicLifetimeMinimum;
            if (lifetime > settings.DynamicLifetimeMaximum)
                lifetime = settings.DynamicLifetimeMaximum;
            return lifetime;
        }

        void ConfigureLeave(ToastAdorner adorner, ToastSettings settings, int lifetime)
        {
            if (settings.Lifetime <= 0)
                return;

            var lifetimeTimer = new Timer(lifetime);
            lifetimeTimer.Elapsed += (s, e) =>
            {
                Remove(adorner);
                lifetimeTimer.Dispose();
            };

            if (settings.RefreshLifetimeOnMouseOver)
            {
                adorner.MouseEnter += (s, e) => lifetimeTimer.Stop();
                adorner.MouseLeave += (s, e) => lifetimeTimer.Start();
            }

            lifetimeTimer.AutoReset = false;
            lifetimeTimer.Start();

            if (settings.LeaveTime <= 0 || settings.Lifetime - settings.LeaveTime <= 0)
                return;

            Action leaveAction = null;
            Action cancelAction = null;

            var elapsedStopwatch = new Stopwatch();
            var originalOpacity = adorner.Opacity;
            var originalLeft = adorner.Left;
            var originalTop = adorner.Top;

            switch (LeaveStyle)
            {
                case LeaveStyle.FadeOut:
                    leaveAction = () => Fade(originalOpacity, settings, adorner);
                    cancelAction = () => CancelFade(adorner, originalOpacity);
                    break;
                case LeaveStyle.SlideOut:
                    leaveAction = () =>
                    {
                        originalLeft = adorner.Left;
                        originalTop = adorner.Top;
                        Slide(adorner, settings);
                        elapsedStopwatch.Start();
                    };
                    cancelAction = () =>
                    {
                        var elapsed = (int) elapsedStopwatch.ElapsedMilliseconds;
                        elapsedStopwatch.Reset();
                        CancelSlide(adorner, originalLeft, originalTop, elapsed);
                    };
                    break;
                case LeaveStyle.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var leaveTimer = new Timer(settings.Lifetime - settings.LeaveTime);
            leaveTimer.Elapsed += (s, e) =>
            {
                leaveTimer?.Dispose();
                leaveTimer = null;
                leaveAction?.Invoke();
            };

            if (settings.RefreshLifetimeOnMouseOver)
            {
                adorner.MouseEnter += (s, e) =>
                {
                    leaveTimer?.Dispose();
                    leaveTimer = null;
                    cancelAction?.Invoke();
                };

                adorner.MouseLeave += (s, e) =>
                {
                    leaveTimer = new Timer(settings.Lifetime - settings.LeaveTime);
                    leaveTimer.Elapsed += (sender, args) =>
                    {
                        leaveTimer?.Dispose();
                        leaveTimer = null;
                        leaveTimer.Start();
                    };
                };
            }

            leaveTimer.AutoReset = false;
            leaveTimer.Start();
        }
        
        void Fade(double originalOpacity, ToastSettings settings, ToastAdorner adorner)
        {
            void Animate()
            {
                var animation = new DoubleAnimation(
                    originalOpacity,
                    0,
                    GetLeaveTime(settings),
                    FillBehavior.Stop);

                adorner.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
        }
        
        void CancelFade(ToastAdorner adorner, double originalOpacity)
        {
            void Animate() { adorner.BeginAnimation(UIElement.OpacityProperty, null); }
            Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
            adorner.Opacity = originalOpacity;
        }

        void Slide(ToastAdorner adorner, ToastSettings settings)
        {
            var targetTop = adorner.Top;
            var targetLeft = adorner.Left;

            if (LeaveDirection == Direction.Left)
                targetLeft = -adorner.ActualWidth;
            else if (LeaveDirection == Direction.Right)
                targetLeft = GetWidth();
            else if (LeaveDirection == Direction.Top)
                targetTop = -adorner.ActualHeight;
            else if (LeaveDirection == Direction.Bottom)
                targetTop = GetHeight();
            else
                throw new ArgumentOutOfRangeException();

            void Animate()
            {
                var horizontal = LeaveDirection == Direction.Left || LeaveDirection == Direction.Right;
                var animation = new DoubleAnimation(
                    horizontal ? adorner.Left : adorner.Top,
                    horizontal ? targetLeft : targetTop,
                    GetLeaveTime(settings),
                    FillBehavior.Stop);

                adorner.BeginAnimation(horizontal ? ToastAdorner.LeftProperty : ToastAdorner.TopProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((AnimateDelegate) Animate);
        }

        void CancelSlide(ToastAdorner adorner, double originalLeft, double originalTop, int elapsedTime)
        {
            void Animate()
            {
                var horizontal = LeaveDirection == Direction.Left || LeaveDirection == Direction.Right;
                adorner.BeginAnimation(horizontal ? ToastAdorner.LeftProperty : ToastAdorner.TopProperty, null);
                var animation = new DoubleAnimation(
                    horizontal ? adorner.Left : adorner.Top,
                    horizontal ? originalLeft : originalTop,
                    GetCancelTime(elapsedTime),
                    FillBehavior.Stop);
                adorner.BeginAnimation(horizontal ? ToastAdorner.LeftProperty : ToastAdorner.TopProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((AnimateDelegate) Animate);
        }

        #endregion // Toast Configuration
        
        #region Movement

        void MoveForward()
        {
            if (FromDirection == Direction.Top || FromDirection == Direction.Bottom)
            {
                if (InvertStacking)
                    MoveHorizontal(HorizontalAlignment == HorizontalAlignment.Left);
                else
                    MoveVertical(FromDirection == Direction.Top);
            }
            else
                MoveHorizontal(FromDirection == Direction.Left);
        }

        void MoveBackwards(int startingIndex)
        {
            if (FromDirection == Direction.Top || FromDirection == Direction.Bottom)
            {
                if (InvertStacking)
                    MoveHorizontal(HorizontalAlignment == HorizontalAlignment.Left, startingIndex);
                else
                    MoveVertical(FromDirection == Direction.Top, startingIndex);
            }
            else
            {
                if (InvertStacking)
                    MoveVertical(VerticalAlignment == VerticalAlignment.Top, startingIndex);
                else
                    MoveHorizontal(FromDirection == Direction.Left, startingIndex);
            }
        }

        void MoveHorizontal(bool moveRight)
        {
            MoveHorizontal(moveRight, _adorners.Count - 1);
        }

        void MoveHorizontal(bool moveRight, int startingIndex)
        {
            MoveHorizontal(moveRight, startingIndex, 0);
        }

        void MoveHorizontal(bool moveRight, int startingIndex, int lastIndex)
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
                Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
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

        #endregion

        #region Position and Dimensions

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

        #endregion

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


        Duration GetCancelTime(int ms)
        {
            return new Duration(new TimeSpan(0, 0, 0, 0, ms));
        }

        Duration GetLeaveTime(ToastSettings settings)
        {
            return new Duration(new TimeSpan(0, 0, 0, 0, settings.LeaveTime));
        }
    }
}