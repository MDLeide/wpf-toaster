using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cashew.Toasty
{
    // todo: synch thread access

    public static class Toaster
    {
        static readonly Dictionary<Window, ElementToaster> Managers =
            new Dictionary<Window, ElementToaster>();


        public static int MoveDuration { get; set; } = 250;
        public static double VerticalPadding { get; set; } = 15;
        public static double VerticalAdjustment { get; set; } = 0;
        public static double HorizontalPadding { get; set; } = 15;
        public static double HorizontalAdjustment { get; set; } = 0;
        public static Direction FromDirection { get; set; } = Direction.Right;
        public static Location ToastLocation { get; set; } = Location.BottomRight;


        public static void Show(
            string title, 
            string message,
            Window window, 
            ToastSettings config,
            UIElement toastView)
        {
            if (!config.CanUserClose && config.Lifetime <= 0)
                throw new InvalidOperationException("Toast is configured to disallow user closing and to never expire.");

            if (!Managers.ContainsKey(window))
                Managers.Add(window, GetManager(window));

            var adorner = new ToastAdorner(Managers[window].ElementToAdorn, toastView);
            Configure(adorner, window, config, title, message);
            Managers[window].Add(adorner);
        }


        static void Configure(ToastAdorner adorner, Window window, ToastSettings config, string title, string message)
        {
            ConfigureClose(adorner, window, config);
            ConfigureClickAction(adorner, window, config);
            ConfigureLifetime(adorner, window, config, title, message);
            ConfigureFade(adorner, config);
        }

        static void ConfigureClose(ToastAdorner adorner, Window window, ToastSettings config)
        {
            if (!config.CanUserClose)
                return;

            adorner.CloseRequested += (s, e) => Remove(adorner, window);
            if (config.CloseOnRightClick)
                adorner.ToastView.MouseRightButtonUp += (s, e) => Remove(adorner, window);
        }

        static void ConfigureClickAction(ToastAdorner adorner, Window window, ToastSettings config)
        {
            if (config.ClickAction != null)
                adorner.ToastView.MouseLeftButtonUp += (s, e) =>
                {
                    config.ClickAction();
                    if (config.CloseAfterClickAction)
                        Remove(adorner, window);
                };
        }

        static void ConfigureLifetime(
            ToastAdorner adorner, 
            Window window, 
            ToastSettings config, 
            string title,
            string message)
        {
            var lifetime = config.Lifetime;

            if (config.DynamicLifetime)
            {
                lifetime =
                    (
                        (string.IsNullOrEmpty(title) ? 0 : title.Length) +
                        (string.IsNullOrEmpty(message) ? 0 : message.Length)
                    ) * config.DynamicLifetimeMillisecondsPerCharacter +
                    config.DynamicLifetimeBase;

                if (lifetime < config.DynamicLifetimeMinimum)
                    lifetime = config.DynamicLifetimeMinimum;
                if (lifetime > config.DynamicLifetimeMaximum)
                    lifetime = config.DynamicLifetimeMaximum;
            }

            if (config.Lifetime <= 0)
                return;

            var lifetimeTimer = new Timer(lifetime);
            lifetimeTimer.Elapsed += (s, e) =>
            {
                Remove(adorner, window);
                lifetimeTimer.Dispose();
            };

            if (config.RefreshLifetimeOnMouseOver)
            {
                adorner.MouseEnter += (s, e) => lifetimeTimer.Stop();
                adorner.MouseLeave += (s, e) => lifetimeTimer.Start();
            }

            lifetimeTimer.AutoReset = false;
            lifetimeTimer.Start();
        }

        delegate void AnimateDelegate();

        static void ConfigureFade(ToastAdorner adorner, ToastSettings config)
        {
            if (config.FadeTime <= 0 || config.Lifetime - config.FadeTime <= 0)
                return;

            var originalOpacity = adorner.Opacity;
            var fadeTimer = new Timer(config.Lifetime - config.FadeTime);

            fadeTimer.Elapsed += (s, e) => Fade(ref fadeTimer, originalOpacity, config, adorner);

            fadeTimer.AutoReset = false;
            fadeTimer.Start();

            if (!config.RefreshLifetimeOnMouseOver)
                return;

            adorner.MouseEnter += (s, e) =>
            {
                fadeTimer?.Stop();

                void Animate()
                {
                    adorner.BeginAnimation(UIElement.OpacityProperty, null);
                }

                Application.Current?.Dispatcher?.Invoke((AnimateDelegate) Animate);
                adorner.Opacity = originalOpacity;
            };

            adorner.MouseLeave += (s, e) =>
            {
                fadeTimer = new Timer(config.Lifetime - config.FadeTime);
                fadeTimer.Elapsed += (sender, args) => Fade(ref fadeTimer, originalOpacity, config, adorner);
                fadeTimer.Start();
            };
        }

        static void Fade(ref Timer fadeTimer, double originalOpacity, ToastSettings config, ToastAdorner adorner)
        {
            fadeTimer.Dispose();
            fadeTimer = null;

            void Animate()
            {
                var animation = new DoubleAnimation(
                    originalOpacity,
                    0,
                    new Duration(
                        new TimeSpan(0, 0, 0, 0, config.FadeTime)),
                    FillBehavior.Stop);

                adorner.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
        }

        static void Remove(ToastAdorner adorner, Window window)
        {
            Managers[window].Remove(adorner);
            if (!Managers[window].HasAdorners)
                Managers.Remove(window);
        }

        static ElementToaster GetManager(Window window)
        {
            var layer = GetWindowAdornerLayer(window, out var uiElement);
            if (layer == null)
                throw new InvalidOperationException("Window must have an adorner layer.");

            var manager = new ElementToaster(uiElement, layer);
            manager.MoveDuration = new Duration(new TimeSpan(0, 0, 0, 0, MoveDuration));
            manager.VerticalPadding = VerticalPadding;
            manager.VerticalAdjustment = VerticalAdjustment;
            manager.HorizontalPadding = HorizontalPadding;
            manager.HorizontalAdjustment = HorizontalAdjustment;
            manager.ToastLocation = ToastLocation;
            manager.FromDirection = FromDirection;
            return manager;
        }

        static AdornerLayer GetWindowAdornerLayer(Window window, out UIElement uiElement)
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
        
        static bool ConfigurationIsValid()
        {
            switch (ToastLocation)
            {
                case Location.Bottom:
                    return FromDirection == Direction.Bottom;
                case Location.Left:
                    return FromDirection == Direction.Left;
                case Location.Right:
                    return FromDirection == Direction.Right;
                case Location.Top:
                    return FromDirection == Direction.Top;
                case Location.BottomLeft:
                    return FromDirection == Direction.Bottom || FromDirection == Direction.Left;
                case Location.BottomRight:
                    return FromDirection == Direction.Bottom || FromDirection == Direction.Right;
                case Location.TopLeft:
                    return FromDirection == Direction.Top || FromDirection == Direction.Left;
                case Location.TopRight:
                    return FromDirection == Direction.Top || FromDirection == Direction.Right;
            }

            return false;
        }
    }
}