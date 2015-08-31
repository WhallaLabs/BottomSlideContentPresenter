using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BottomSlideContentPresenter.Interfaces;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BottomSlideContentPresenterSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Starting geoposition
            MapControl.Center = new Geopoint(new BasicGeoposition() { Latitude = 47.673988, Longitude = -122.121512 });

            //Event Listener to enable manipulations. More information in MainPageEventListener class.
            BottomSlideContentPresenter.EventListener = new MainPageEventListener();
            (BottomSlideContentPresenter.EventListener as IManipulatorEventListener).RegisterControlEvents(InputGrid);
        }

        private async void OnTextboxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                return;
            }

            var placesForName = await GetCoordinateForNameAsync(InputTextBox.Text);
            if (placesForName != null)
            {
                ResultsListView.ItemsSource = placesForName;
            }
        }

        private void OnSuggestionTapped(object sender, TappedRoutedEventArgs e)
        {
            var geoPoint = ((sender as Grid).DataContext as MapLocation).Point;
            if (geoPoint != null)
            {
                MapControl.Center = new Geopoint(geoPoint.Position);
            }
        }

        private async Task<IReadOnlyList<MapLocation>> GetCoordinateForNameAsync(string searchText)
        {
            IAsyncOperation<MapLocationFinderResult> finderTask = null;
            try
            {
                Debug.WriteLine("Started gettin place coordinate");

                finderTask = MapLocationFinder.FindLocationsAsync(searchText, MapControl.Center);

                MapLocationFinderResult result = await finderTask;


                if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
                {
                    return result.Locations;
                }

                return null;
            }
            finally
            {
                if (finderTask != null)
                {
                    if (finderTask.Status == AsyncStatus.Started)
                    {
                        Debug.WriteLine("Attempting to cancel place coordinate task");
                        finderTask.Cancel();
                    }

                    finderTask.Close();
                }
            }
        }
    }
}
