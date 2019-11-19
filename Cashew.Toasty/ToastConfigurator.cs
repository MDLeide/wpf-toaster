using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using Cashew.Toasty.Config;

namespace Cashew.Toasty
{
    class ToastConfigurator
    {
        public static void Configure(
            ToastAdorner adorner, 
            Window window, 
            ToastSettings config, 
            string title, 
            string message, 
            Action<ToastAdorner> remove,
            Action clickAction)
        {
            ConfigureClose(adorner, config, remove);
            if (clickAction != null)
                ConfigureClickAction(adorner, config, remove, clickAction);
            ConfigureLifetime(adorner, config, title, message, remove);
            ConfigureFade(adorner, config);
        }

        static void ConfigureClose(ToastAdorner adorner, ToastSettings config, Action<ToastAdorner> remove)
        {
            if (!config.CanUserClose)
                return;

            adorner.CloseRequested += (s, e) => remove(adorner);
            if (config.CloseOnRightClick)
                adorner.ToastView.MouseRightButtonUp += (s, e) => remove(adorner);
        }

        static void ConfigureClickAction(ToastAdorner adorner, ToastSettings config, Action<ToastAdorner> remove, Action clickAction)
        {
            adorner.ToastView.MouseLeftButtonUp += (s, e) =>
            {
                clickAction();
                if (config.CloseAfterClickAction)
                    remove(adorner);
            };
        }

        static void ConfigureLifetime(
            ToastAdorner adorner,
            ToastSettings config,
            string title,
            string message,
            Action<ToastAdorner> remove)
        {
            var lifetime = config.DynamicLifetime ? GetDynamicLifetime(config, title, message) : config.Lifetime;
            if (config.Lifetime <= 0)
                return;

            var lifetimeTimer = new Timer(lifetime);
            lifetimeTimer.Elapsed += (s, e) =>
            {
                remove(adorner);
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

        static int GetDynamicLifetime(ToastSettings config, string title, string message)
        {
            var lifetime =
                (
                    (string.IsNullOrEmpty(title) ? 0 : title.Length) +
                    (string.IsNullOrEmpty(message) ? 0 : message.Length)
                ) * config.DynamicLifetimeMillisecondsPerCharacter +
                config.DynamicLifetimeBase;

            if (lifetime < config.DynamicLifetimeMinimum)
                lifetime = config.DynamicLifetimeMinimum;
            if (lifetime > config.DynamicLifetimeMaximum)
                lifetime = config.DynamicLifetimeMaximum;
            return lifetime;
        }

        delegate void AnimateDelegate();

        static void ConfigureFade(ToastAdorner adorner, ToastSettings config)
        {
            if (config.LeaveTime <= 0 || config.Lifetime - config.LeaveTime <= 0)
                return;

            var originalOpacity = adorner.Opacity;
            var fadeTimer = new Timer(config.Lifetime - config.LeaveTime);

            fadeTimer.Elapsed += (s, e) => Fade(ref fadeTimer, originalOpacity, config, adorner);

            fadeTimer.AutoReset = false;
            fadeTimer.Start();

            if (!config.RefreshLifetimeOnMouseOver)
                return;

            adorner.MouseEnter += (s, e) =>
            {
                fadeTimer?.Stop();
                void Animate() { adorner.BeginAnimation(UIElement.OpacityProperty, null); }
                Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
                adorner.Opacity = originalOpacity;
            };

            adorner.MouseLeave += (s, e) =>
            {
                fadeTimer = new Timer(config.Lifetime - config.LeaveTime);
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
                        new TimeSpan(0, 0, 0, 0, config.LeaveTime)),
                    FillBehavior.Stop);

                adorner.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            Application.Current?.Dispatcher?.Invoke((AnimateDelegate)Animate);
        }
    }
}
