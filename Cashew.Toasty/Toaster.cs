using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cashew.Toasty.Config;
using Cashew.Toasty.Defaults;
using Timer = System.Timers.Timer;

namespace Cashew.Toasty
{
    class ToastQueueEntry
    {
        public ToastAdorner Adorner { get; set; }
        public ToastSettings Settings { get; set; }
        public Action ClickAction { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }

    public class Toaster
    {
        readonly List<ToastAdorner> _adorners = new List<ToastAdorner>();
        readonly AdornerLayer _adornerLayer;
        readonly ToasterSettings _settings;
        readonly Func<string, string, UIElement> _getDefaultView;
        readonly ConcurrentQueue<ToastQueueEntry> _queue = new ConcurrentQueue<ToastQueueEntry>();
        readonly object _queueLock = new object();

        bool _queueIsProcessing;

        
        public Toaster(Window window, ToasterSettings settings = null, Func<string, string, UIElement> getDefaultView = null, ToastSettings defaultToastSettings = null)
        {
            _getDefaultView = getDefaultView ?? DefaultSettings.GetDefaultToastViewFunc;
            _adornerLayer = GetWindowAdornerLayer(window, out var elementToAdorn);
            _settings = settings ?? DefaultSettings.DefaultToasterSettings;
            DefaultToastSettings = defaultToastSettings ?? DefaultSettings.DefaultToastSettings;
            ElementToAdorn = elementToAdorn;
            Window = window;
        }


        public event EventHandler IsEmpty;


        public ToastSettings DefaultToastSettings { get; set; }

        public Window Window { get; }
        public UIElement ElementToAdorn { get; }

        Duration MoveDuration => new Duration(new TimeSpan(0, 0, 0, 0, _settings.MoveDuration));
        int MoveTime => _settings.MoveDuration;
        MoveStyle MoveStyle => _settings.MoveStyle;
        Direction MoveDirection => _settings.MoveDirection;
        double VerticalPadding => _settings.VerticalPadding;
        double HorizontalPadding => _settings.HorizontalPadding;
        double VerticalAdjustment => _settings.VerticalAdjustment;
        double HorizontalAdjustment => _settings.HorizontalAdjustment;
        EnterStyle EnterStyle => _settings.EnterStyle;
        Direction EnterFromDirection => _settings.EnterFromDirection;
        Location EnterLocation => _settings.EnterLocation;
        LeaveStyle LeaveStyle => _settings.LeaveStyle;
        Direction LeaveDirection => _settings.LeaveDirection;
        bool QueueToasts => _settings.QueueToasts;



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

            if (QueueToasts)
            {
                AddToQueue(adorner, toastSettings, clickAction, title, message);
                ProcessQueue();
            }
            else
            {
                Configure(adorner, toastSettings, clickAction, title, message);
                Add(adorner);
            }
        }


        delegate void RemoveDelegate(ToastAdorner adorner);
        delegate void ActionDelegate();


        void AddToQueue(ToastAdorner adorner, ToastSettings toastSettings, Action clickAction, string title, string message)
        {
            var queueEntry = new ToastQueueEntry()
            {
                Adorner = adorner,
                Settings = toastSettings,
                ClickAction = clickAction,
                Title = title,
                Message = message
            };

            _queue.Enqueue(queueEntry);
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

        void ProcessQueue()
        {
            lock (_queueLock)
            {
                if (_queueIsProcessing)
                    return;
                _queueIsProcessing = true;
            }

            Task.Factory.StartNew(() =>
            {
                while (_queue.TryDequeue(out var entry))
                {
                    var entryCopy = entry;
                    void ConfigureAndAdd()
                    {
                        Configure(entryCopy.Adorner, entryCopy.Settings, entryCopy.ClickAction, entryCopy.Title, entryCopy.Message);
                        Add(entryCopy.Adorner);
                    }

                    Application.Current?.Dispatcher?.Invoke((ActionDelegate) ConfigureAndAdd);
                    Thread.Sleep(MoveTime);
                }

                lock (_queueLock)
                {
                    _queueIsProcessing = false;
                }
            });
        }

        #region Toast Configuration

        void Configure(ToastAdorner adorner, ToastSettings settings, Action clickAction, string title, string message)
        {
            ConfigureClose(adorner, settings);
            ConfigureClickAction(adorner, settings, clickAction);
            var lifetime = settings.DynamicLifetime ? GetDynamicLifetime(settings, title, message) : settings.Lifetime;
            ConfigureRemove(adorner, settings, lifetime);
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

        void ConfigureRemove(ToastAdorner adorner, ToastSettings settings, int lifetime)
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
        }

        void ConfigureLeave(ToastAdorner adorner, ToastSettings settings, int lifetime)
        {
            GetLeaveActions(adorner, settings, lifetime, out var leaveAction, out var cancelAction);
            ConfigureLeaveActions(adorner, settings, lifetime, leaveAction, cancelAction);
        }

        void GetLeaveActions(ToastAdorner adorner, ToastSettings settings, int lifetime, out Action leaveAction, out Action cancelAction)
        {
            if (settings.LeaveTime <= 0 || lifetime - settings.LeaveTime <= 0)
            {
                leaveAction = null;
                cancelAction = null;
                return;
            }

            var elapsedStopwatch = new Stopwatch();
            double originalOpacity = adorner.Opacity;
            double originalLeft = adorner.Left;
            double originalTop = adorner.Top;
            
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
                        var elapsed = (int)elapsedStopwatch.ElapsedMilliseconds;
                        elapsedStopwatch.Reset();
                        CancelSlide(adorner, originalLeft, originalTop, elapsed);
                    };
                    break;
                case LeaveStyle.None:
                    leaveAction = null;
                    cancelAction = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ConfigureLeaveActions(ToastAdorner adorner, ToastSettings settings, int lifetime, Action leaveAction, Action cancelAction)
        {
            var leaveTimer = new Timer(lifetime - settings.LeaveTime);
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
                    leaveTimer = new Timer(lifetime - settings.LeaveTime);
                    leaveTimer.Elapsed += (sender, args) =>
                    {
                        leaveTimer?.Dispose();
                        leaveTimer = null;
                        leaveAction?.Invoke();
                    };
                    leaveTimer.Start();
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
            
            Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
        }
        
        void CancelFade(ToastAdorner adorner, double originalOpacity)
        {
            void Animate() { adorner.BeginAnimation(UIElement.OpacityProperty, null); }
            Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
            adorner.Opacity = originalOpacity;
        }

        void Slide(ToastAdorner adorner, ToastSettings settings)
        {
            double targetTop = 0;
            double targetLeft = 0;

            void SetTargets()
            {
                targetTop = adorner.Top;
                targetLeft = adorner.Left;

                if (LeaveDirection == Direction.Left)
                    targetLeft = -adorner.ActualWidth;
                else if (LeaveDirection == Direction.Right)
                    targetLeft = GetWidth();
                else if (LeaveDirection == Direction.Up)
                    targetTop = -adorner.ActualHeight;
                else if (LeaveDirection == Direction.Down)
                    targetTop = GetHeight();
                else
                    throw new ArgumentOutOfRangeException();
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) SetTargets);
            
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

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) Animate);
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

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) Animate);
        }

        #endregion // Toast Configuration
        
        #region Movement

        void MoveForward()
        {
            if (EnterFromDirection == Direction.Up || EnterFromDirection == Direction.Down)
                MoveVertical(EnterFromDirection == Direction.Up);
            else
                MoveHorizontal(EnterFromDirection == Direction.Left);
        }

        void MoveBackwards(int startingIndex)
        {
            if (EnterFromDirection == Direction.Up || EnterFromDirection == Direction.Down)
                MoveVertical(EnterFromDirection == Direction.Up, startingIndex);
            else
                MoveHorizontal(EnterFromDirection == Direction.Left, startingIndex);
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
                Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
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
                targetTop += moveDown ? VerticalPadding : -VerticalPadding; 
                var targetTopCopy = targetTop;
                
                if (i > startingIndex)
                    continue;

                var index = i;
                void Animate()
                {
                    var animation = new DoubleAnimation(_adorners[index].Top, targetTopCopy, MoveDuration);
                    _adorners[index].BeginAnimation(ToastAdorner.TopProperty, animation);
                }
                Application.Current?.Dispatcher?.Invoke((ActionDelegate) Animate);
            }
        }

        #endregion

        #region Position and Dimensions

        double GetInitialLeft(ToastAdorner adorner)
        {
            double left = 0;

            void GetLeft()
            {
                if (EnterLocation == Location.Top && EnterFromDirection == Direction.Up ||
                    EnterLocation == Location.Bottom && EnterFromDirection == Direction.Down)
                    left = GetWidth() / 2 - adorner.ActualWidth / 2;

                if (EnterLocation == Location.TopLeft)
                {
                    if (EnterFromDirection == Direction.Left)
                        left = -adorner.ActualWidth;
                    else if (EnterFromDirection != Direction.Up)
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.BottomLeft)
                {
                    if (EnterFromDirection == Direction.Left)
                        left = -adorner.ActualWidth;
                    else if (EnterFromDirection != Direction.Down)
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.TopRight)
                {
                    if (EnterFromDirection == Direction.Right)
                        left = GetWidth();
                    else if (EnterFromDirection == Direction.Up)
                        left = GetWidth() - adorner.ActualWidth;
                    else
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.BottomRight)
                {
                    if (EnterFromDirection == Direction.Right)
                        left = GetWidth();
                    else if (EnterFromDirection == Direction.Down)
                        left = GetWidth() - adorner.ActualWidth;
                    else
                        throw new InvalidOperationException();
                }

                left += HorizontalAdjustment;
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) GetLeft);
            return left;

        }

        double GetInitialTop(ToastAdorner adorner)
        {
            double top = 0;

            void GetTop()
            {
                if (EnterLocation == Location.Left && EnterFromDirection == Direction.Left ||
                    EnterLocation == Location.Right && EnterFromDirection == Direction.Right)
                    top = GetHeight() / 2 - adorner.ActualHeight / 2;

                if (EnterLocation == Location.TopLeft)
                {
                    if (EnterFromDirection == Direction.Up)
                        top = -adorner.ActualHeight;
                    else if (EnterFromDirection != Direction.Left)
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.TopRight)
                {
                    if (EnterFromDirection == Direction.Up)
                        top = -adorner.ActualHeight;
                    else if (EnterFromDirection != Direction.Right)
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.BottomLeft)
                {
                    if (EnterFromDirection == Direction.Left)
                        top = GetHeight() - adorner.ActualHeight;
                    else if (EnterFromDirection == Direction.Down)
                        top = GetHeight();
                    else
                        throw new InvalidOperationException();
                }

                if (EnterLocation == Location.BottomRight)
                {
                    if (EnterFromDirection == Direction.Right)
                        top = GetHeight() - adorner.ActualHeight;
                    else if (EnterFromDirection == Direction.Down)
                        top = GetHeight();
                    else
                        throw new InvalidOperationException();
                }

                top += VerticalAdjustment;
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) GetTop);
            return top;
        }

        double GetHeight()
        {
            if (ElementToAdorn is FrameworkElement frameworkElement)
                return _adornerLayer.ActualHeight - frameworkElement.Margin.Left;
            return 0;
        }

        double GetWidth()
        {
            double width = 0;
            if (ElementToAdorn is FrameworkElement frameworkElement)
                width = _adornerLayer.ActualWidth - frameworkElement.Margin.Top;
            return width;
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