using System;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty
{
    class ToastQueueEntry
    {
        public ToastAdorner Adorner { get; set; }
        public ToastAdornerSettings Settings { get; set; }
        public Action ClickAction { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }
}