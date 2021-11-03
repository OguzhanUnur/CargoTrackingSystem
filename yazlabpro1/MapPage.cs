using FireSharp.Response;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace yazlabpro1
{
    public partial class MapPage : Form
    {
        public MapPage()
        {
            InitializeComponent();
        }


        private void zamanlayaci(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            rotaHesapla();
        }


        private void MapPage_Load(object sender, EventArgs e)
        {
            
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMap.CacheLocation = @"cache";
            gMap.SetPositionByKeywords("Türkiye");
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.ShowCenter = false;
            gMap.MinZoom = 5;
            gMap.MaxZoom = 100;
            gMap.Zoom = 7;           
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (5 * 1000); 
            timer.Tick += new EventHandler(zamanlayaci);
            timer.Start();
            
        }


        void rotaHesapla()
        {
            GMapProviders.GoogleMap.ApiKey = ApiKey.Key;
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.MaxZoom = 100;
            gMap.MinZoom = 1;
            gMap.Zoom = 10;

            for (int i = 0; i < GlobalArray.globalNoktalar.Count; i++)
            {
                if (i == 0)
                {
                    double lat = Convert.ToDouble(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i])].Lat);
                    double lng = Convert.ToDouble(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i])].Lng);
                    gMap.Position = new PointLatLng(lat, lng);
                    PointLatLng point = new PointLatLng(lat, lng);
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue);
                    GMapOverlay markers = new GMapOverlay("markers");
                    markers.Markers.Add(marker);
                    gMap.Overlays.Add(markers);
                }
                else
                {
                    double lat = Convert.ToDouble(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i])].Lat);
                    double lng = Convert.ToDouble(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i])].Lng);
                    gMap.Position = new PointLatLng(lat, lng);
                    PointLatLng point = new PointLatLng(lat, lng);
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
                    GMapOverlay markers = new GMapOverlay("markers");
                    markers.Markers.Add(marker);
                    gMap.Overlays.Add(markers);
                }
                if (i + 1 < GlobalArray.globalNoktalar.Count)
                {
                    Console.WriteLine(Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i]).ToString() + Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i + 1]).ToString());
                    
                    var route = GoogleMapProvider.Instance.GetRoute(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i])], GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, i + 1])], false, false, 14);
                    var r = new GMapRoute(route.Points, "My Route")
                    {
                        Stroke = new Pen(Color.Red, 5)
                    };

                    var routes = new GMapOverlay("routes");
                    routes.Routes.Add(r);
                    gMap.Overlays.Add(routes);

                }

            }
            
        }

        private void gMap_Load(object sender, EventArgs e)
        {

        }
    }
}
