using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using System.Threading;

namespace Location
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource locationCancellationTokenSource;
        private Pin currentLocationPin;

        public MainPage()
        {
            InitializeComponent();
            StartLocationTracking();
        }
        private void StartLocationTracking()
        {
            locationCancellationTokenSource = new CancellationTokenSource();
            Task.Run(TrackLocation, locationCancellationTokenSource.Token);
        }

        private async Task TrackLocation()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(5));
            var location = await Geolocation.GetLocationAsync(request, locationCancellationTokenSource.Token);

            if (location != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (currentLocationPin == null)
                    {
                        currentLocationPin = new Pin
                        {
                            Type = PinType.Place,
                            Label = "My Location",
                        };
                        map.Pins.Add(currentLocationPin);
                    }

                    currentLocationPin.Position = new Position(location.Latitude, location.Longitude);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(currentLocationPin.Position, Distance.FromKilometers(1)));
                });
            }

            // Continuar el seguimiento de ubicación mientras el CancellationToken no esté cancelado
            if (!locationCancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(1000);
                await TrackLocation();
            }
        }


    }
}
