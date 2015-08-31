using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using BottomSlideContentPresenter.Interfaces;

namespace BottomSlideContentPresenterSample
{
    /// <summary>
    /// Because control is built with two contentPresenters, it doesn't know from which control take manipulations to slide in and out.
    /// You must create a class implementing IManipulatorEventListener to point that and set it to EventListener property of a BottomSlideContentPresenter.
    /// </summary>
    public class MainPageEventListener: IManipulatorEventListener
    {
        private ICollection<FrameworkElement> _notifyingElements; 


        public event ManipulationCompletedEventHandler ListenerManipulationCompleted;
        public event ManipulationDeltaEventHandler ListenerManipulationDelta;
        
        public void RegisterControlEvents(FrameworkElement element)
        {
            if (_notifyingElements.Contains(element))
            {
                return;
            }

            _notifyingElements.Add(element);
            element.ManipulationMode = ManipulationModes.All;
            element.ManipulationDelta += ListenerManipulationDelta;
            element.ManipulationCompleted += ListenerManipulationCompleted;
        }

        public MainPageEventListener()
        {
            _notifyingElements = new List<FrameworkElement>();
        }
    }
}
