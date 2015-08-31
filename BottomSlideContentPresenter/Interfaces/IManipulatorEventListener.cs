using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace BottomSlideContentPresenter.Interfaces
{
    /// <summary>
    /// Class that provide a proxy for drag manipulations.
    /// </summary>
    public interface IManipulatorEventListener
    {
        event ManipulationCompletedEventHandler ListenerManipulationCompleted;
        event ManipulationDeltaEventHandler ListenerManipulationDelta;

        /// <summary>
        /// Register control from which manipulation will be captured to slide in and out presenter.
        /// </summary>
        /// <param name="element">Element that users will be draging.</param>
        void RegisterControlEvents(FrameworkElement element);
    }
}
