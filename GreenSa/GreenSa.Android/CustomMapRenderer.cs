using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using static Android.Gms.Maps.GoogleMap;
using Xamarin.Forms;
using GreenSa.Models.Tools.GPS_Maps;
using GreenSa.Models.Tools;
using Greensa.Droid;
using System.Collections.ObjectModel;
using GreenSa.Models.GolfModel;
using GreenSa.ViewController.Play.Game;

using Geodesy;
using System.Threading;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace Greensa.Droid
{
    public class CustomMapRenderer : MapRenderer
    {

        GoogleMap map;
        private Android.Gms.Maps.Model.Polyline targetLine;  // The line from the user pin to the target
        private List<Android.Gms.Maps.Model.Polyline> coneLines;  // The lines from the shot cone
        private Android.Gms.Maps.Model.Circle circle;  // The range circle
        private Android.Gms.Maps.Model.Polygon triangle;  // The shot triangle

        public CustomMapRenderer(Context context) : base(context){
            coneLines = new List<Android.Gms.Maps.Model.Polyline>();
            MessagingCenter.Subscribe<CustomMap>(this, "updateTheMap", (sender) => {
                try
                {
                    UpdatePolyLinePos(false);
                    // UpdateShotTriangle();
                    // UpdateShotCone(Math.PI / 4);
                }
                catch(Exception) { }
            });
            MessagingCenter.Subscribe<Partie>(this, "updateTheCircle", (sender) => {
                try
                {
                    updateCircle();
                }
                catch (Exception) { }
            });

            MessagingCenter.Subscribe<MainGamePage, bool>(this, "updateTheCircleVisbility", (sender, visible) => {
                try
                {
                    setCircleVisible(visible);

                }
                catch (Exception) { }
            });
        }
        

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null) { }

            if (e.NewElement != null)
            {
                ((MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            base.OnElementPropertyChanged(sender, e);
            if (this.Element == null || this.Control == null)
                return;
        }


        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            this.map = map;
            map.SetOnMarkerDragListener(new markerListenerDrag(this));
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = false;
        }


        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = base.CreateMarker(pin);

            if (!( pin is CustomPin ))
            {
                return marker;
            }
            if (((CustomPin)(pin)).type == CustomPin.MOVABLE)
            {
                marker.Draggable(true);
                marker.SetRotation(30.5f);
                BitmapDescriptor ic = BitmapDescriptorFactory.FromResource(GreenSa.Droid.Resource.Drawable.Target);

                marker.SetIcon(ic);
                marker.Anchor(0.5f, 0.5f);
            }
            else if (((CustomPin)(pin)).type == CustomPin.HOLE)
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(GreenSa.Droid.Resource.Drawable.flag));
            }
            else if (((CustomPin)(pin)).type == CustomPin.USER)
            {

            }
            else if (((CustomPin)(pin)).type == CustomPin.LOCKED)
            {
                marker.SetRotation(30.5f);
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
            }


            return marker;
        }

        // Updates the position of the shot cone
        public void UpdateShotCone(double angle)
        {
            // If cone lines are currently displayed, remove them
            if (coneLines.Count != 0)
            {
                foreach (Android.Gms.Maps.Model.Polyline line in coneLines)
                {
                    line.Remove();
                }
            }

            CustomMap customMap = (CustomMap)this.Element;
            LatLng userPos = new LatLng(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude);

            if (customMap != null)
            {
                // DrawTriangle();
                addConePolyline(-angle, customMap, userPos);  // Draws one part of the cone
                addConePolyline(angle, customMap, userPos);  // Draws the other part of the cone
            }
        }

        // Add a cone's side to the variable coneLines
        private void addConePolyline(double angle, CustomMap customMap, LatLng userPos)
        {
            // The coordinates of the end of the side to be drawn
            LatLng conePoint = MovePoint(angle, customMap.UserPin.Position, customMap.TargetPin.Position);

            var polylineOptions = new PolylineOptions();

            polylineOptions.InvokeWidth(10f);
            polylineOptions.InvokeColor(Android.Graphics.Color.Argb(240, 255, 20, 147)); // Pink

            polylineOptions.Add(userPos);
            polylineOptions.Add(conePoint);

            // Add the line to coneLines
            coneLines.Add(map.AddPolyline(polylineOptions));

        }

        // !!! TODO !!! Fix this
        // Moves a point by the given angle on a circle of center rotationCenter with respect to p
        // Resources for the fix...
        // https://stackoverflow.com/questions/238260/how-to-calculate-the-bounding-box-for-a-given-lat-lng-location
        // https://stackoverflow.com/questions/1253499/simple-calculations-for-working-with-lat-lon-and-km-distance?noredirect=1&lq=1
        // https://stackoverflow.com/questions/41425939/android-maps-polygonoptions-lat-and-long-value-in-km?noredirect=1&lq=1
        private LatLng MovePoint(double angle, Position rotationCenter, Position initialPoint)
        {
            // Compute the components of the translation vector between rotationCenter and initialPoint
            double dx = initialPoint.Latitude - rotationCenter.Latitude;
            double dy = initialPoint.Longitude - rotationCenter.Longitude;

            // Compute the moved point's position
            double x = rotationCenter.Latitude + Math.Cos(angle) * dx - Math.Sin(angle) * dy;
            double y = rotationCenter.Longitude + Math.Sin(angle) * dx + Math.Cos(angle) * dy;

            LatLng res = new LatLng(x, y);
            
            return res;
        }

        public void UpdatePolyLinePos(bool init,LatLng pos=null)
        {
            if (targetLine != null)
            {
                targetLine.Remove();
                targetLine.Dispose();           
            }
            var polylineOptions = new PolylineOptions();
            polylineOptions.Clickable(true);
            polylineOptions.InvokeJointType(JointType.Round);  // Does not change anything
            polylineOptions.InvokeWidth(10f);
            polylineOptions.InvokeColor(0x664444FF);

            int i = 0;
            CustomMap customMap = (CustomMap)this.Element;
            if (customMap != null)
            {
                foreach (var position in customMap.RouteCoordinates)
                {
                    if (i == 1 && !init && pos != null)
                        polylineOptions.Add(pos);
                    else
                        polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));

                    i++;
                }
                targetLine = map.AddPolyline(polylineOptions);
            }
        }

        // Updates the position of the circle
        public void updateCircle()
        {
            CustomMap customMap = (CustomMap)this.Element;

            if (customMap != null)
            {
                if (customMap.UserPin != null)
                {
                    if (circle != null)
                    {
                        circle.Remove();
                        circle.Dispose();
                    }
                    //moy
                    CircleOptions circleOptions = new CircleOptions();
                    circleOptions.InvokeCenter(new LatLng(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude));
                    circleOptions.InvokeRadius(customMap.getDistanceUserTarget());
                    circleOptions.InvokeFillColor(Android.Graphics.Color.Argb(0, 0, 0, 0));
                    circleOptions.InvokeStrokeColor(Android.Graphics.Color.Argb(240, 250, 250, 250));
                    circleOptions.InvokeStrokeWidth(5f);

                    circle = map.AddCircle(circleOptions);
                }
            }
        }

        // Makes the circle visible
        public void setCircleVisible(bool visible)
        {
            circle.Visible = visible;
        }

        // TEST ZONE //
        // Draws a triangle in front of the player
        private void DrawTriangle()
        {
            CustomMap customMap = (CustomMap)this.Element;

            LatLng userPos = new LatLng(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude);
            LatLng pointA = MovePoint(Math.PI / 4, customMap.UserPin.Position, customMap.TargetPin.Position);
            LatLng pointB = MovePoint(-Math.PI / 4, customMap.UserPin.Position, customMap.TargetPin.Position);
            this.triangle = map.AddPolygon(new PolygonOptions()
                .Add(userPos, pointA, pointB)
                .InvokeFillColor(Android.Graphics.Color.Argb(150, 255, 0, 0)));
        }

        // Updates the position of the shot triangle
        public void UpdateShotTriangle()
        {
            // If triangle is currently displayed, remove it
            if (triangle != null)
            {
                triangle.Remove();
            }

            CustomMap customMap = (CustomMap)this.Element;

            // Re-draw the triangle
            if (customMap != null)
            {
                DrawTriangle();
            }
        }

    }
}
