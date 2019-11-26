using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cashew.Toasty.Defaults;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty
{
    // todo: synch thread access
    // todo: test element to adorn margins

    public class ToasterManager
    {
        readonly ToasterSettings _settings;
        readonly Func<string, string, UIElement> _getDefaultView;
        readonly ToastAdornerSettings _defaultToastSettings;
        readonly Dictionary<Window, Toaster> _toasters = new Dictionary<Window, Toaster>();


        public ToasterManager(ToasterSettings settings = null, Func<string, string, UIElement> getDefaultView = null, ToastAdornerSettings defaultToastSettings = null)
        {
            _settings = settings;
            _getDefaultView = getDefaultView;
            _defaultToastSettings = defaultToastSettings;
        }


        public void Show(
            string title, 
            string message,
            Window window, 
            ToastAdornerSettings toastSettings = null,
            UIElement toastView = null,
            Action clickAction = null)
        {
            if (!_toasters.ContainsKey(window))
            {
                var toaster = new Toaster(window, _settings, _getDefaultView, _defaultToastSettings);
                toaster.IsEmpty += (s, e) => _toasters.Remove(window);
                _toasters.Add(window, toaster);
            }
            _toasters[window].Show(title, message, toastSettings, toastView, clickAction);
        }

        public void RegisterWindowToaster(Toaster toaster, bool removeWhenEmpty = false)
        {
            if (_toasters.ContainsKey(toaster.Window))
                throw new InvalidOperationException("There is already a toaster for this window.");

            _toasters.Add(toaster.Window, toaster);
            if (removeWhenEmpty)
                toaster.IsEmpty += (s, e) => _toasters.Remove(toaster.Window);
        }
    }
}