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
using Cashew.Toasty.Defaults;
using Cashew.Toasty.Settings;
using Timer = System.Timers.Timer;

namespace Cashew.Toasty
{
    public class Toaster
    {
        readonly List<ToastAdorner> _adorners = new List<ToastAdorner>();
        readonly AdornerLayer _adornerLayer;
        readonly ToasterSettings _settings;
        readonly Func<string, string, UIElement> _getDefaultView;
        readonly ConcurrentQueue<ToastQueueEntry> _queue = new ConcurrentQueue<ToastQueueEntry>();
        readonly object _queueLock = new object();

        bool _queueIsProcessing;

        
        public Toaster(Window window, ToasterSettings settings = null, Func<string, string, UIElement> getDefaultView = null, ToastAdornerSettings defaultToastSettings = null)
        {
            _getDefaultView = getDefaultView ?? DefaultSettings.GetDefaultToastViewFunc;
            _adornerLayer = GetWindowAdornerLayer(window, out var elementToAdorn);
            _settings = settings ?? DefaultSettings.DefaultToasterSettings;
            DefaultToastSettings = defaultToastSettings ?? DefaultSettings.DefaultToastSettings;
            ElementToAdorn = elementToAdorn;
            Window = window;
        }


        public event EventHandler IsEmpty;


        public ToastAdornerSettings DefaultToastSettings { get; set; }

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
                    EnterLocation == Location.TopLeft)
                    return HorizontalAlignment.Left;
                if (EnterLocation == Location.BottomRight ||
                    EnterLocation == Location.TopRight)
                    return HorizontalAlignment.Right;
                return HorizontalAlignment.Center;
            }
        }

        VerticalAlignment VerticalAlignment
        {
            get
            {
                if (EnterLocation == Location.TopLeft ||
                    EnterLocation == Location.TopRight)
                    return VerticalAlignment.Top;
                if (EnterLocation == Location.BottomLeft ||
                    EnterLocation == Location.BottomRight)
                    return VerticalAlignment.Bottom;
                return VerticalAlignment.Center;
            }
        }

        public ToastAdorner Show(
            string message,
            string title,
            ToastAdornerSettings toastSettings = null,
            UIElement toastView = null,
            Action clickAction = null)
        {
            if (toastSettings == null && DefaultToastSettings == null)
                throw new ArgumentNullException(nameof(toastSettings), "Toast settings cannot be null if DefaultToastSettings has not been set.");
            
            if (toastSettings == null)
                toastSettings = DefaultToastSettings;

            if (!toastSettings.CanUserClose && toastSettings.Lifetime <= 0)
                throw new InvalidOperationException("Toast is configured to disallow user closing and to never expire.");

            ToastAdorner adorner = null;
            void CreateAdorner()
            {
                adorner = new ToastAdorner(ElementToAdorn, toastView ?? _getDefaultView(title, message));
            }
            Application.Current?.Dispatcher?.Invoke((ActionDelegate) CreateAdorner);
            
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

            return adorner;
        }


        delegate void RemoveDelegate(ToastAdorner adorner);
        delegate void ActionDelegate();


        void AddToQueue(ToastAdorner adorner, ToastAdornerSettings toastSettings, Action clickAction, string title, string message)
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
            toast.Loaded += (s, e) => { Enter(toast); };

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

        void Configure(ToastAdorner adorner, ToastAdornerSettings settings, Action clickAction, string title, string message)
        {
            ConfigureClose(adorner, settings);
            ConfigureClickAction(adorner, settings, clickAction);
            var lifetime = settings.DynamicLifetime ? GetDynamicLifetime(settings, title, message) : settings.Lifetime;
            ConfigureRemove(adorner, settings, lifetime);
            ConfigureLeave(adorner, settings, lifetime);
        }

        void ConfigureClose(ToastAdorner adorner, ToastAdornerSettings settings)
        {
            if (!settings.CanUserClose)
                return;

            adorner.CloseRequested += (s, e) => Remove(adorner);
            if (settings.CloseOnRightClick)
                adorner.ToastView.MouseRightButtonUp += (s, e) => Remove(adorner);
        }

        void ConfigureClickAction(ToastAdorner adorner, ToastAdornerSettings settings, Action clickAction)
        {
            adorner.ToastView.MouseLeftButtonUp += (s, e) =>
            {
                clickAction();
                if (settings.CloseAfterClickAction)
                    Remove(adorner);
            };
        }

        int GetDynamicLifetime(ToastAdornerSettings settings, string title, string message)
        {
            var lifetime =
                (
                    (string.IsNullOrEmpty(title) ? 0 : title.Length) +
                    (string.IsNullOrEmpty(message) ? 0 : message.Length)
                ) * settings.DynamicLifetimeMillisecondsPerCharacter +
                settings.DynamicLifetimeBase;

            if (lifetime < settings.DynamicLifetimeMinimum)
                lifetime = settings.DynamicLifetimeMinimum;
            if (settings.DynamicLifetimeMaximum > 0 && lifetime > settings.DynamicLifetimeMaximum)
                lifetime = settings.DynamicLifetimeMaximum;
            return lifetime;
        }

        void ConfigureRemove(ToastAdorner adorner, ToastAdornerSettings settings, int lifetime)
        {
            if (lifetime <= 0)
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

        #endregion // Toast Configuration

        #region Leave
        
        void ConfigureLeave(ToastAdorner adorner, ToastAdornerSettings settings, int lifetime)
        {
            GetLeaveActions(adorner, settings, lifetime, out var leaveAction, out var cancelAction);
            ConfigureLeaveActions(adorner, settings, lifetime, leaveAction, cancelAction);
        }

        void GetLeaveActions(ToastAdorner adorner, ToastAdornerSettings settings, int lifetime, out Action leaveAction, out Action cancelAction)
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
            var isLeaving = false;

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
                        isLeaving = true;
                    };
                    cancelAction = () =>
                    {
                        if (!isLeaving)
                            return;
                        isLeaving = false;
                        var elapsed = (int) elapsedStopwatch.ElapsedMilliseconds;
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

        void ConfigureLeaveActions(ToastAdorner adorner, ToastAdornerSettings settings, int lifetime, Action leaveAction, Action cancelAction)
        {
            if (lifetime - settings.LeaveTime <= 0)
                return;

            var leaveTimer = new Timer(lifetime - settings.LeaveTime);
            leaveTimer.Elapsed += (s, e) =>
            {
                leaveTimer?.Dispose();
                leaveTimer = null;

                void Leave()
                {
                    leaveAction?.Invoke();
                }

                Application.Current?.Dispatcher?.Invoke((ActionDelegate) Leave);
            };

            if (settings.RefreshLifetimeOnMouseOver)
            {
                adorner.MouseEnter += (s, e) =>
                {
                    leaveTimer?.Dispose();
                    leaveTimer = null;

                    void Cancel()
                    {
                        cancelAction?.Invoke();
                    }

                    Application.Current?.Dispatcher?.Invoke((ActionDelegate) Cancel);
                };

                adorner.MouseLeave += (s, e) =>
                {
                    leaveTimer = new Timer(lifetime - settings.LeaveTime);
                    leaveTimer.Elapsed += (sender, args) =>
                    {
                        leaveTimer?.Dispose();
                        leaveTimer = null;

                        void Leave()
                        {
                            leaveAction?.Invoke();
                        }

                        Application.Current?.Dispatcher?.Invoke((ActionDelegate) Leave);
                    };
                    leaveTimer.Start();
                };
            }

            leaveTimer.AutoReset = false;
            leaveTimer.Start();
        }

        void Fade(double originalOpacity, ToastAdornerSettings settings, ToastAdorner adorner)
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

        void Slide(ToastAdorner adorner, ToastAdornerSettings settings)
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

            Application.Current?.Dispatcher?.Invoke((ActionDelegate)SetTargets);

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

            Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
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
                    GetCancelTime(elapsedTime));
                adorner.BeginAnimation(horizontal ? ToastAdorner.LeftProperty : ToastAdorner.TopProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
        }
        
        #endregion

        #region Movement

        void Enter(ToastAdorner toast)
        {
            toast.Top = GetEntryTop(toast);
            toast.Left = GetEntryLeft(toast);

            if (EnterStyle == EnterStyle.PopIn || EnterStyle == EnterStyle.FadeIn)
            {
                if (EnterLocation == Location.TopLeft ||
                    EnterLocation == Location.TopRight)
                    toast.Top += toast.ActualHeight + VerticalPadding;

                if (EnterLocation == Location.BottomLeft ||
                    EnterLocation == Location.BottomRight)
                    toast.Top -= toast.ActualHeight + VerticalPadding;

                if (EnterLocation == Location.TopLeft ||
                    EnterLocation == Location.BottomLeft)
                    toast.Left += toast.ActualWidth + HorizontalPadding;

                if (EnterLocation == Location.TopRight ||
                    EnterLocation == Location.BottomRight)
                    toast.Left -= toast.ActualWidth + HorizontalPadding;
            }

            if (EnterStyle == EnterStyle.FadeIn)
            {
                toast.Opacity = 0;

                void Animate()
                {
                    var animation = new DoubleAnimation(
                        0,
                        1,
                        MoveDuration,
                        FillBehavior.Stop);

                    toast.BeginAnimation(UIElement.OpacityProperty, animation);
                }

                Application.Current?.Dispatcher?.Invoke((ActionDelegate) Animate);
            }

            if (EnterStyle == EnterStyle.SlideIn)
            {
                double from;
                double target;
                if (EnterFromDirection == Direction.Left)
                {
                    from = toast.Left;
                    target = toast.Left + toast.ActualWidth + HorizontalPadding + HorizontalAdjustment;
                }
                else if (EnterFromDirection == Direction.Right)
                {
                    from = toast.Left;
                    target = GetWidth() - toast.ActualWidth - HorizontalPadding + HorizontalAdjustment;
                }
                else if (EnterFromDirection == Direction.Up)
                {
                    from = toast.Top;
                    target = toast.Top + toast.ActualHeight + VerticalPadding + VerticalAdjustment;
                }
                else // Direction.Down
                {
                    from = toast.Top;
                    target = GetHeight() - toast.ActualHeight - VerticalPadding + VerticalAdjustment;
                }

                var property =
                    EnterFromDirection == Direction.Left || EnterFromDirection == Direction.Right
                        ? ToastAdorner.LeftProperty
                        : ToastAdorner.TopProperty;

                var animation = new DoubleAnimation(
                    from,
                    target,
                    MoveDuration);

                void Animate()
                {
                    toast.BeginAnimation(property, animation);
                }

                Application.Current?.Dispatcher?.Invoke((ActionDelegate) Animate);
            }
            
            if (MoveStyle == MoveStyle.Stack)
            {
                if (MoveDirection == Direction.Left)
                    foreach (var adorner in _adorners)
                        toast.Left -= adorner.ActualWidth + HorizontalPadding;
                else if (MoveDirection == Direction.Right)
                    foreach (var adorner in _adorners)
                        toast.Left += adorner.ActualWidth + HorizontalPadding;
                else if (MoveDirection == Direction.Up)
                    foreach (var adorner in _adorners)
                        toast.Top -= adorner.ActualHeight + VerticalPadding;
                else if (MoveDirection == Direction.Down)
                    foreach (var adorner in _adorners)
                        toast.Top += adorner.ActualHeight + VerticalPadding;
            }

            if (MoveStyle == MoveStyle.Stack)
                _adorners.Insert(0, toast);
            else
                _adorners.Add(toast);

            if (MoveStyle == MoveStyle.Push)
                MoveForward();
        }

        // called after the adorner is added to the collection
        void MoveForward()
        {
            if (MoveDirection == Direction.Up || MoveDirection == Direction.Down)
                AdjustVertical(1);
            else
                AdjustHorizontal(1);
        }

        // called after the adorner is removed from the collection
        void MoveBackwards(int startingIndex)
        {
            if (MoveDirection == Direction.Up || MoveDirection == Direction.Down)
                AdjustVertical(0);
            else
                AdjustHorizontal(0);
        }

        void AdjustVertical(int skip)
        {
            if (!_adorners.Any())
                return;

            var targetTop = GetAdjustTop();
            var count = 0;

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                if (MoveDirection == Direction.Up)
                    targetTop -= _adorners[i].ActualHeight + VerticalPadding;
                else
                    targetTop += _adorners[i].ActualHeight + VerticalPadding;

                var targetTopCopy = targetTop;

                count++;
                if (count <= skip)
                    continue;

                var index = i;
                void Animate()
                {
                    var animation = new DoubleAnimation(_adorners[index].Top, targetTopCopy, MoveDuration);
                    _adorners[index].BeginAnimation(ToastAdorner.TopProperty, animation);
                }
                Application.Current?.Dispatcher?.Invoke((ActionDelegate)Animate);
            }
        }

        void AdjustHorizontal(int skip)
        {
            if (!_adorners.Any())
                return;

            var targetLeft = GetAdjustLeft();
            var count = 0;

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                if (MoveDirection == Direction.Right)
                    targetLeft += _adorners[i].ActualWidth + HorizontalPadding;
                else
                    targetLeft -= _adorners[i].ActualWidth + HorizontalPadding;

                var targetLeftCopy = targetLeft;

                count++;
                if (count <= skip)
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
        
        #endregion

        #region Position and Dimensions

        double GetAdjustTop()
        {
            double top = 0;

            void GetTop()
            {
                var adorner = _adorners.Last();

                if (EnterLocation == Location.TopLeft || EnterLocation == Location.TopRight)
                {
                    //if (EnterFromDirection == Direction.Up)
                    top = -adorner.ActualHeight;
                    // else if (EnterFromDirection == Direction.Down)
                    if (EnterFromDirection == Direction.Down)
                        throw new InvalidOperationException();
                }
                
                if (EnterLocation == Location.BottomLeft || EnterLocation == Location.BottomRight)
                {
                    if (EnterFromDirection == Direction.Left || EnterFromDirection == Direction.Right || EnterFromDirection == Direction.Down)
                        top = GetHeight();
                    else
                        throw new InvalidOperationException();
                }
                
                top += VerticalAdjustment;
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate)GetTop);
            return top;
        }

        double GetAdjustLeft()
        {
            double left = 0;

            void GetLeft()
            {
                var adorner = _adorners.Last();

                if (EnterLocation == Location.TopLeft || EnterLocation == Location.BottomLeft)
                        left -= adorner.ActualWidth;

                if (EnterLocation == Location.TopRight || EnterLocation == Location.BottomRight)
                    left = GetWidth();

                left += HorizontalAdjustment;
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) GetLeft);
            return left;
        }

        double GetEntryLeft(ToastAdorner adorner)
        {
            double left = 0;

            void GetLeft()
            {
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

        double GetEntryTop(ToastAdorner adorner)
        {
            double top = 0;

            void GetTop()
            {
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
            UIElement element = window;
            AdornerLayer layer = null;

            void GetWindowLayer()
            {
                layer = AdornerLayer.GetAdornerLayer(window);
                if (layer != null)
                    return;

                if (window.Content is UIElement ui)
                {
                    element = ui;
                    if (ui is Visual v)
                        layer = AdornerLayer.GetAdornerLayer(v);
                }
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate) GetWindowLayer);
            uiElement = element;
            return layer;
        }

        Duration GetCancelTime(int ms)
        {
            return new Duration(new TimeSpan(0, 0, 0, 0, ms));
        }

        Duration GetLeaveTime(ToastAdornerSettings settings)
        {
            return new Duration(new TimeSpan(0, 0, 0, 0, settings.LeaveTime));
        }
    }
}