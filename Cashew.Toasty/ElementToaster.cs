using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cashew.Toasty
{
    class ElementToaster
    {
        readonly List<ToastAdorner> _adorners = new List<ToastAdorner>();
        readonly AdornerLayer _adornerLayer;


        public ElementToaster(Window window)
        {
            _adornerLayer = GetWindowAdornerLayer(window, out var elementToAdorn);
            ElementToAdorn = elementToAdorn;
        }

        public ElementToaster(UIElement elementToAdorn, AdornerLayer adornerLayer)
        {
            _adornerLayer = adornerLayer;
            ElementToAdorn = elementToAdorn;
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

        public UIElement ElementToAdorn { get; set; }

        public Duration MoveDuration { get; set; }

        public Location ToastLocation { get; set; }
        public Direction FromDirection { get; set; }

        public double VerticalPadding { get; set; }
        public double HorizontalPadding { get; set; }

        public double VerticalAdjustment { get; set; }
        public double HorizontalAdjustment { get; set; }

        public bool HasAdorners => _adorners.Any();

        HorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (ToastLocation == Location.Left ||
                    ToastLocation == Location.TopLeft ||
                    ToastLocation == Location.BottomLeft)
                    return HorizontalAlignment.Left;

                if (ToastLocation == Location.Right ||
                    ToastLocation == Location.TopRight ||
                    ToastLocation == Location.BottomRight)
                    return HorizontalAlignment.Right;

                return HorizontalAlignment.Center;

            }
        }

        VerticalAlignment VerticalAlignment
        {
            get
            {
                if (ToastLocation == Location.Top ||
                    ToastLocation == Location.TopLeft ||
                    ToastLocation == Location.TopRight)
                    return VerticalAlignment.Top;

                if (ToastLocation == Location.Bottom ||
                    ToastLocation == Location.BottomRight ||
                    ToastLocation == Location.BottomLeft)
                    return VerticalAlignment.Bottom;

                return VerticalAlignment.Center;
            }
        }


        public void Add(ToastAdorner toast)
        {
            toast.Loaded += (s, e) =>
            {
                toast.Top = GetTop(toast);
                toast.Left = GetLeft(toast);
                MoveForward();
            };
            
            _adorners.Add(toast);
            _adornerLayer.Add(toast);
        }

        public void Remove(ToastAdorner toast)
        {
            var index = _adorners.IndexOf(toast) - 1;

            if (!_adorners.Remove(toast))
                return;

            RemoveDelegate del = _adornerLayer.Remove;
            Application.Current?.Dispatcher?.Invoke(del, toast);

            if (index < 0)
                return;

            MoveBackwards(index);
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

            var targetLeft = GetLeft(_adorners.Last());

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                if (moveRight)
                    targetLeft += _adorners[i].ActualWidth;
                else
                    targetLeft -= _adorners[i].ActualWidth;

                var targetLeftCopy = targetLeft;

                if (moveRight)
                    targetLeft += HorizontalPadding;
                else
                    targetLeft -= HorizontalPadding;

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

            var targetTop = GetTop(_adorners.Last());

            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                if (moveDown)
                    targetTop += _adorners[i].ActualHeight;
                else
                    targetTop -= _adorners[i].ActualHeight;

                var targetTopCopy = targetTop;

                if (moveDown)
                    targetTop += VerticalPadding;
                else
                    targetTop -= VerticalPadding;

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


        double GetLeft(ToastAdorner adorner)
        {
            if (ToastLocation == Location.Top && FromDirection == Direction.Top ||
                ToastLocation == Location.Bottom && FromDirection == Direction.Bottom)
                return GetWidth() / 2 - adorner.ActualWidth / 2 + HorizontalAdjustment;
            
            if (ToastLocation == Location.TopLeft)
            {
                if (FromDirection == Direction.Left)
                    return -adorner.ActualWidth + HorizontalAdjustment;
                if (FromDirection == Direction.Top)
                    return HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.BottomLeft)
            {
                if (FromDirection == Direction.Left)
                    return -adorner.ActualWidth + HorizontalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.TopRight)
            {
                if (FromDirection == Direction.Right)
                    return GetWidth() + HorizontalAdjustment;
                if (FromDirection == Direction.Top)
                    return GetWidth() - adorner.ActualWidth + HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.BottomRight)
            {
                if (FromDirection == Direction.Right)
                    return GetWidth() + HorizontalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return GetWidth() - adorner.ActualWidth + HorizontalAdjustment;
                throw new InvalidOperationException();
            }

            throw new InvalidOperationException();
        }

        double GetTop(ToastAdorner adorner)
        {
            if (ToastLocation == Location.Left && FromDirection == Direction.Left ||
                ToastLocation == Location.Right && FromDirection == Direction.Right)
                return GetHeight() / 2 - adorner.ActualHeight / 2 + VerticalAdjustment;

            if (ToastLocation == Location.TopLeft)
            {
                if (FromDirection == Direction.Left)
                    return VerticalAdjustment;
                if (FromDirection == Direction.Top)
                    return -adorner.ActualHeight + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.TopRight)
            {
                if (FromDirection == Direction.Right)
                    return VerticalAdjustment;
                if (FromDirection == Direction.Top)
                    return -adorner.ActualHeight + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.BottomLeft)
            {
                if (FromDirection == Direction.Left)
                    return GetHeight() - adorner.ActualHeight + VerticalAdjustment;
                if (FromDirection == Direction.Bottom)
                    return GetHeight() + VerticalAdjustment;
                throw new InvalidOperationException();
            }

            if (ToastLocation == Location.BottomRight)
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