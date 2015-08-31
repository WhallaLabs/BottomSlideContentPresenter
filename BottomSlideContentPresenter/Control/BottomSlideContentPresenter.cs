using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using BottomSlideContentPresenter.Interfaces;

namespace BottomSlideContentPresenter.Control
{
    [TemplatePart(Name = "PART_TopArea", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_BottomArea", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_MainCanvas", Type = typeof(ContentPresenter))]
    public class BottomSlideContentPresenter: Windows.UI.Xaml.Controls.Control
    {
        #region Fields

        private ContentPresenter _topAreaContent;
        private ContentPresenter _bottomAreaContent;

        private Canvas _mainCanvas;

        private Storyboard _closeBottomContentStoryboard;
        private Storyboard _openBottomContentStoryboard;
        private DoubleAnimation _openAnimation;
        private DoubleAnimation _closeAnimation;

        private Size _lastAvailableSize;

        #endregion

        #region Event declarations

        public event EventHandler PresenterClosed;
        public event EventHandler PresenterOpened;

        #endregion

        #region Dependency properties

        /// <summary>
        /// Top part of the control that is default visible.
        /// </summary>
        public object TopAreaContent
        {
            get { return (object)GetValue(TopAreaContentProperty); }
            set { SetValue(TopAreaContentProperty, value); }
        }

        public static readonly DependencyProperty TopAreaContentProperty =
            DependencyProperty.Register("TopAreaContent", typeof(object), typeof(BottomSlideContentPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Bottom part of the control that slide in and out.
        /// </summary>
        public object BottomAreaContent
        {
            get { return (object)GetValue(BottomAreaContentProperty); }
            set { SetValue(BottomAreaContentProperty, value); }
        }

        public static readonly DependencyProperty BottomAreaContentProperty =
            DependencyProperty.Register("BottomAreaContent", typeof(object), typeof(BottomSlideContentPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Set or get if control is open or not.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BottomSlideContentPresenter), new PropertyMetadata(false, IsOpenPropertyChanged));

        /// <summary>
        /// Property of type IManipulatorEventListener providing a proxy for drag manipulations.
        /// </summary>
        public object EventListener
        {
            get { return (IManipulatorEventListener)GetValue(EventListenerProperty); }
            set { SetValue(EventListenerProperty, value); }
        }

        public static readonly DependencyProperty EventListenerProperty =
            DependencyProperty.Register("EventListener", typeof(object), typeof(BottomSlideContentPresenter), new PropertyMetadata(null, EventListenerPropertyChangedCallback));

        /// <summary>
        /// Amount of bottom part pixels that should always stay on the screen. 
        /// </summary>
        public double BottomContentOffset
        {
            get { return (double)GetValue(BottomContentOffsetProperty); }
            set { SetValue(BottomContentOffsetProperty, value); }
        }

        public static readonly DependencyProperty BottomContentOffsetProperty =
            DependencyProperty.Register("BottomContentOffset", typeof(double), typeof(BottomSlideContentPresenter), new PropertyMetadata(0.0));

        /// <summary>
        /// Value indicating how far the bottom content should slide out.
        /// </summary>
        public double PercentsOfScreenToReveal
        {
            get { return (double)GetValue(PercentsOfScreenToRevealProperty); }
            set { SetValue(PercentsOfScreenToRevealProperty, value); }
        }

        public static readonly DependencyProperty PercentsOfScreenToRevealProperty =
            DependencyProperty.Register("PercentsOfScreenToReveal", typeof(double), typeof(BottomSlideContentPresenter), new PropertyMetadata(75.0,OnPercentsOfScreenToRevealPropertyChangedCallback));

        #endregion

        #region Constructor

        public BottomSlideContentPresenter()
        {
            this.DefaultStyleKey = typeof(BottomSlideContentPresenter);
        }

        #endregion


        protected override void OnApplyTemplate()
        {
            _topAreaContent = base.GetTemplateChild("PART_TopArea") as ContentPresenter;
            _bottomAreaContent = base.GetTemplateChild("PART_BottomArea") as ContentPresenter;
            _mainCanvas = base.GetTemplateChild("PART_MainCanvas") as Canvas;

            _closeBottomContentStoryboard =
                _mainCanvas.Resources["CloseBottomContentStoryboard"]
                    as Storyboard;

            _openBottomContentStoryboard = _mainCanvas.Resources["OpenBottomContentStoryboard"] as Storyboard;

            _openAnimation = _openBottomContentStoryboard.Children[0] as DoubleAnimation;
            _closeAnimation = _closeBottomContentStoryboard.Children[0] as DoubleAnimation;

            Loaded -= OnBottomSlideContentPresenterLoaded;
            Loaded += OnBottomSlideContentPresenterLoaded;

            SubscribeForEventListenerEvents();

            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _lastAvailableSize = availableSize;
            SetInternalSizesAndTranslations(availableSize);

            return base.MeasureOverride(availableSize);
        }

        private void SetInternalSizesAndTranslations(Size availableSize)
        {
            _topAreaContent.Height = availableSize.Height - BottomContentOffset;

            _bottomAreaContent.Height = availableSize.Height * PercentsOfScreenToReveal / (double)100;

            _openAnimation.To = ActualHeight - _bottomAreaContent.ActualHeight;
            _closeAnimation.To = _topAreaContent.Height;

            (_bottomAreaContent.RenderTransform as TranslateTransform).Y = _topAreaContent.Height;
        }

        private static void OnPercentsOfScreenToRevealPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var castedSender = (dependencyObject as BottomSlideContentPresenter);
            if (castedSender != null)
            {
                castedSender.SetInternalSizesAndTranslations(castedSender._lastAvailableSize);
            }
        }


        private void OnBottomSlideContentPresenterLoaded(object sender, RoutedEventArgs e)
        {
            Width =
                _topAreaContent.Width = _bottomAreaContent.Width =
                    _mainCanvas.Width = ActualWidth;

            SubscribeForChildrenTextBoxEvents();
            SubscribeForEventListenerEvents();
        }

        private void BottomContentPresenterChildrensManipulationCompletedEventHandler(object sender,
            ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.Y > 0)
            {
                IsOpen = false;
                OnPresenterClosed();
            }
            else
            {
                var translateTransform = _bottomAreaContent.RenderTransform as TranslateTransform;
                if (translateTransform != null && translateTransform.Y != 0)
                {
                    IsOpen = true;
                    OnPresenterOpened();
                }
            }
        }

        private void BottomContentPresenterChildrensManipulationDeltaEventHandler(object sender,
            ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                e.Complete();
            }

            var translateTransform = _bottomAreaContent.RenderTransform as TranslateTransform;

            if (translateTransform != null)
            {
                var currentOffset = translateTransform.Y;

                if (IsMovementInsideBorders(e.Delta.Translation, currentOffset))
                {
                    translateTransform.Y += e.Delta.Translation.Y;
                }
            }
        }

        /// <summary>
        /// Subscribing for textbox controls events that are inside control to catch focus state and delay frame preventing scrolling whole control up.
        /// </summary>
        private void SubscribeForChildrenTextBoxEvents()
        {
            if (_bottomAreaContent == null)
            {
                return;
            }

            var _bottomContentChildrens = _bottomAreaContent.GetLogicalChildrenBreadthFirst();

            var textBoxChildrens = _bottomContentChildrens.OfType<TextBox>();

            if (textBoxChildrens == null)
            {
                return;
            }

            foreach (TextBox textBoxChildren in textBoxChildrens)
            {
                textBoxChildren.PointerPressed -= TextBoxChildrenPointerPressedEventHandler;
                textBoxChildren.PointerPressed += TextBoxChildrenPointerPressedEventHandler;
            }
        }

        private async void TextBoxChildrenPointerPressedEventHandler(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            //Ugly hack for delaying system keyboard and prevent frame from scrolling up whole control
            IsOpen = true;
            (sender as Windows.UI.Xaml.Controls.Control).IsEnabled = false;
            await Task.Delay(100);
            (sender as Windows.UI.Xaml.Controls.Control).IsEnabled = true;
            (sender as Windows.UI.Xaml.Controls.Control).Focus(FocusState.Programmatic);
        }

        private bool IsMovementInsideBorders(Point translationPoint, double bottomContentOffset)
        {
            var desiredOffset = bottomContentOffset + translationPoint.Y;

            return (desiredOffset >= ActualHeight - _bottomAreaContent.ActualHeight && translationPoint.Y < 0) ||
                   (desiredOffset <= _topAreaContent.ActualHeight && translationPoint.Y >= 0);
        }

        private void SubscribeForEventListenerEvents()
        {
            if (!(EventListener is IManipulatorEventListener))
            {
                return;
            }

            ((IManipulatorEventListener) EventListener).ListenerManipulationDelta -= BottomContentPresenterChildrensManipulationDeltaEventHandler;
            ((IManipulatorEventListener) EventListener).ListenerManipulationDelta += BottomContentPresenterChildrensManipulationDeltaEventHandler;

            ((IManipulatorEventListener) EventListener).ListenerManipulationCompleted -= BottomContentPresenterChildrensManipulationCompletedEventHandler;
            ((IManipulatorEventListener) EventListener).ListenerManipulationCompleted += BottomContentPresenterChildrensManipulationCompletedEventHandler;
        }

        private static void IsOpenPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var bottomPresenter = (dependencyObject as BottomSlideContentPresenter);

            if (bottomPresenter == null)
            {
                return;
            }

            if ((bool)dependencyPropertyChangedEventArgs.NewValue && bottomPresenter._openBottomContentStoryboard != null)
            {
                bottomPresenter._openBottomContentStoryboard.Begin();
            }
            else if (bottomPresenter._closeBottomContentStoryboard != null)
            {
                bottomPresenter._closeBottomContentStoryboard.Begin();
            }
        }

        private static void EventListenerPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            BottomSlideContentPresenter bottomSlideContentPresenter = dependencyObject as BottomSlideContentPresenter;
            if (bottomSlideContentPresenter != null)
            {
                bottomSlideContentPresenter.SubscribeForEventListenerEvents();
            }
        }

        protected virtual void OnPresenterOpened()
        {
            if (PresenterOpened != null)
            {
                PresenterOpened(this, null);
            }
        }

        protected virtual void OnPresenterClosed()
        {
            if (PresenterClosed != null)
            {
                PresenterClosed(this, null);
            }
        }

    }
}
