using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Google.Cloud.Firestore;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace yazlabpro1
{
    public partial class HomePage : Form
    {

        int deleteForMarkers = 0;
        private string[,] globalDizi = new string[100, 100]; 
        private string[,] toplamKenar = new string[100, 100];
        private double[,] toplamKm = new double[100, 100];       


        IFirebaseConfig ifare = new FirebaseConfig()
        {
            AuthSecret = "5EG9VCcyxhG9Fx7bOOFi43CYDgsV9pedv31N2I1s",
            BasePath = "https://ozjkargo-9f971-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;
        public HomePage()
        {

            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            try
            {
                client = new FireSharp.FirebaseClient(ifare);
            }

            catch
            {
                MessageBox.Show("İnternete bağlan");
            }
            //SetListener();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            //this.Controls.Clear();
            //this.InitializeComponent();
            KoordinatlarList.list.Clear();
            GlobalArray.sayac = 0;
            GlobalArray.globalNoktalar.Clear();
            GlobalArray.index = 0;
            GlobalArray.noktaSayisi = 0;
            Array.Clear(GlobalArray.toplamKenar, 0, GlobalArray.toplamKenar.Length);
            Array.Clear(toplamKenar, 0, toplamKenar.Length);
            Array.Clear(toplamKm, 0, toplamKm.Length);
            Array.Clear(globalDizi, 0, globalDizi.Length);
            listBox1.Items.Clear();
            KoordinatlariGetir();
        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMapControl1.CacheLocation = @"cache";
            gMapControl1.SetPositionByKeywords("Türkiye, izmit");
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.ShowCenter = false;
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 11.5;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (8 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void PostFirebase(string lat, string lng)
        {
            string adres = "https://ozjkargo-9f971-default-rtdb.firebaseio.com/.json";
            WebRequest request = HttpWebRequest.Create(adres);
            WebResponse webRes;
            webRes = request.GetResponse();

            StreamReader streamReader = new StreamReader(webRes.GetResponseStream());
            string bilgi = streamReader.ReadToEnd();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(bilgi);
           

            Dictionary<string, object> koordinatlar = new Dictionary<string, object>()
            {
                {"lat",lat},
                {"lng",lng }
            };

            Index.index = myDeserializedClass.İndex;
            SetResponse set = client.Set("Koordinatlar/" + Index.index, koordinatlar);
            Index.index++;
            SetResponse setİndex = client.Set("İndex", Index.index);
            MessageBox.Show("Ok");
            

        }
        public class Koordinatlar
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }


        public class Root
        {
            public List<Koordinatlar> Koordinatlar { get; set; }
            public int İndex { get; set; }
        }


        // private EventStreamResponse listener;

        async void Sil(PointLatLng point)
        {
            string adres = "https://ozjkargo-9f971-default-rtdb.firebaseio.com/.json";
            WebRequest request = HttpWebRequest.Create(adres);
            WebResponse webRes;
            webRes = request.GetResponse();

            StreamReader streamReader = new StreamReader(webRes.GetResponseStream());
            string bilgi = streamReader.ReadToEnd();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(bilgi);
            List<Koordinatlar> x = new List<Koordinatlar>();
            int tutucu = 0;
            PointLatLng latLng = point;
            for (int i = 0; i < myDeserializedClass.Koordinatlar.Count; i++)
            {
                if (myDeserializedClass.Koordinatlar[i].lat.ToString() == latLng.Lat.ToString() && myDeserializedClass.Koordinatlar[i].lng.ToString() == latLng.Lng.ToString())
                {
                    tutucu = i;
                }
            }
            for (int i = 0; i < myDeserializedClass.Koordinatlar.Count; i++)
            {
                x.Add(myDeserializedClass.Koordinatlar[i]);
            }
            x.RemoveAt(tutucu);

            for (int i = 0; i < myDeserializedClass.Koordinatlar.Count - 1; i++)
            {
                Dictionary<string, object> koordinatlar = new Dictionary<string, object>()
                {
                    {"lat",x[i].lat},
                    {"lng",x[i].lng}
                };
                SetResponse setIndex = client.Set("Koordinatlar/" + i, koordinatlar);
            }
            FirebaseResponse removeCoordinate = await client.DeleteAsync("Koordinatlar/" + (myDeserializedClass.İndex - 1).ToString());
            SetResponse setIndex2 = client.Set("İndex", (myDeserializedClass.İndex - 1));
        }


        /* async void SetListener()
         {

             listener = await client.OnAsync("Coordinates",

             added: (sender, args, context) => { GetCoordinates(); },
             changed: (sender, args, context) => { GetCoordinates(); },
             removed: (sender, args, context) => { GetCoordinates(); });
         }*/

        void KoordinatlariGetir()
        {
            
            string adres = "https://ozjkargo-9f971-default-rtdb.firebaseio.com/.json";
            WebRequest request = HttpWebRequest.Create(adres);
            WebResponse webRes;
            webRes = request.GetResponse();

            StreamReader streamReader = new StreamReader(webRes.GetResponseStream());
            string bilgi = streamReader.ReadToEnd();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(bilgi);

            if( myDeserializedClass.Koordinatlar != null)
            {



                for (int i = 0; i < myDeserializedClass.Koordinatlar.Count; i++)
                {
                    if (myDeserializedClass.Koordinatlar[i].lat != null && myDeserializedClass.Koordinatlar[i].lng != null)
                    {
                        KoordinatlarList.list.Add(myDeserializedClass.Koordinatlar[i].lat);
                        KoordinatlarList.list.Add(myDeserializedClass.Koordinatlar[i].lng);
                    }
                }


                for (int i = 0; i < KoordinatlarList.list.Count; i = i + 2)
                {
                    if (KoordinatlarList.list[i] != null)
                    {
                        GlobalArray.globalNoktalar.Add(new PointLatLng(Convert.ToDouble(KoordinatlarList.list[i]), Convert.ToDouble(KoordinatlarList.list[i + 1])));
                    }
                }


                for (int i = 0; i < GlobalArray.globalNoktalar.Count; i++)
                {

                    listBox1.Items.Add(GlobalArray.globalNoktalar[i]);

                }


                GlobalArray.noktaSayisi = GlobalArray.globalNoktalar.Count - 1;
                int[] arr = new int[GlobalArray.noktaSayisi];
                for (int i = 0; i < GlobalArray.noktaSayisi; i++)
                {
                    arr[i] = i + 1;
                }


                Dijikstra(arr, 200);
                for (int i = 0; i < GlobalArray.noktaSayisi; i++)
                {
                    int[] geciciDizi = new int[arr.Length];
                    for (int j = 0; j < GlobalArray.noktaSayisi; j++)
                    {
                        geciciDizi[j] = Convert.ToInt32(globalDizi[i, j]);

                    }
                    Dijikstra(geciciDizi, i);
                }


                for (int i = GlobalArray.noktaSayisi; i < (GlobalArray.noktaSayisi * GlobalArray.noktaSayisi); i++)
                {
                    int[] geciciDizi = new int[GlobalArray.noktaSayisi];
                    for (int j = 0; j < GlobalArray.noktaSayisi; j++)
                    {
                        geciciDizi[j] = Convert.ToInt32(globalDizi[i, j]);
                    }
                    Dijikstra(geciciDizi, i);
                }


                for (int i = 0; i < GlobalArray.sayac; i++)
                {
                    int[] geciciDizi = new int[GlobalArray.noktaSayisi];
                    for (int j = 0; j < GlobalArray.noktaSayisi; j++)
                    {
                        geciciDizi[j] = Convert.ToInt32(globalDizi[i, j]);
                    }
                    geciciDizi = InsertFunction(geciciDizi, 0);
                    for (int a = 0; a < geciciDizi.Length; a++)
                    {
                        GlobalArray.toplamKenar[i, a] = geciciDizi[a].ToString();
                    }
                }
                resulst();
            }
        }


        void DeplacementFunction(int[] dizi, int[] diziDegeri)
        {
            
            for (int j = 0; j < dizi.Length; j++)
            {
                int temp = dizi[dizi.Length - 1];

                for (int i = dizi.Length - 1; i > 0; i--)
                {
                    dizi[i] = dizi[i - 1];

                }

                dizi[0] = temp;

                for (int i = diziDegeri.Length - 1; i >= 0; i--)
                {
                    dizi = InsertFunction(dizi, diziDegeri[i]);
                }
                for (int i = 0; i < dizi.Length; i++)
                {
                    globalDizi[GlobalArray.sayac, i] = dizi[i].ToString();
                }

                GlobalArray.sayac += 1;

                for (int i = 0; i < diziDegeri.Length; i++)
                {
                    dizi = RemoveElement(dizi);
                }
            }
        }


        void DeplacementFunctionForZero(int[] dizi)
        {
            
            for (int j = 0; j < dizi.Length; j++)
            {
                int temp = dizi[dizi.Length - 1];

                for (int i = dizi.Length - 1; i > 0; i--)
                {
                    dizi[i] = dizi[i - 1];

                }
                dizi[0] = temp;

                for (int i = 0; i < dizi.Length; i++)
                {
                    globalDizi[j, i] = dizi[i].ToString();

                }
                GlobalArray.sayac += 1;

            }
        }


        int[] InsertFunction(int[] dizi, int deger)
        {
            int[] geciciDizi = new int[dizi.Length + 1];
            int pos = 1;

            for (int i = 0; i < dizi.Length + 1; i++)
            {
                if (i < pos - 1)
                    geciciDizi[i] = dizi[i];
                else if (i == pos - 1)
                    geciciDizi[i] = deger;
                else
                    geciciDizi[i] = dizi[i - 1];
            }

            return geciciDizi;

        }


        int[] RemoveElement(int[] geciciDizi)
        {
            geciciDizi = geciciDizi.Where((source, index) => index != 0).ToArray();
            return geciciDizi;
        }

        void Dijikstra(int[] dizi, int index)
        {
            if (index != 200)
            {
                if (index < dizi.Length)
                {
                    int[] diziDegeri = new int[1];

                    for (int i = 0; i < diziDegeri.Length; i++)
                    {
                        diziDegeri[i] = dizi[i];
                    }
                    for (int i = 0; i < diziDegeri.Length; i++)
                    {
                        dizi = RemoveElement(dizi);
                    }
                    DeplacementFunction(dizi, diziDegeri);

                }

                else if (dizi.Length <= index && index < (dizi.Length * dizi.Length))
                {
                    int[] diziDegeri = new int[2];
                    for (int i = 0; i < diziDegeri.Length; i++)
                    {
                        diziDegeri[i] = dizi[i];
                    }
                    for (int i = 0; i < diziDegeri.Length; i++)
                    {
                        dizi = RemoveElement(dizi);
                    }
                    DeplacementFunction(dizi, diziDegeri);

                }

            }

            else if (index == 200)
            {
                DeplacementFunctionForZero(dizi);
            }


        }


        void resulst()
        {
            toplamKenar = GlobalArray.toplamKenar;

            for (int i = 0; i < GlobalArray.sayac; i++)
            {
                double km = 0;

                for (int j = 0; j < GlobalArray.noktaSayisi + 1; j++)
                {
                    km = km + calculateDistance(GlobalArray.globalNoktalar[Convert.ToInt32(toplamKenar[i, j])], GlobalArray.globalNoktalar[Convert.ToInt32(toplamKenar[i, j + 1])]);
                }
                toplamKm[i, 0] = km;
                toplamKm[i, 1] = i;
            }
            double shortest = toplamKm[0, 0];

            for (int i = 1; i < GlobalArray.sayac; i++)
            {
                if (toplamKm[i, 0] < shortest)
                {
                    shortest = toplamKm[i, 0];
                    GlobalArray.index = Convert.ToInt32(toplamKm[i, 1]);
                }
            }
            Console.WriteLine("En kısa yol haritası;\n");

            for (int i = 0; i < GlobalArray.noktaSayisi + 1; i++)
            {
                Console.WriteLine(toplamKenar[GlobalArray.index, i]);
            }
            Console.WriteLine("Toplam yol =>" + " " + shortest + "km");


        }


        int calculateDistance(PointLatLng point1, PointLatLng point2)
        {

            var route = GoogleMapProvider.Instance.GetRoute(point1, point2, false, false, 14);
            var r = new GMapRoute(route.Points, "My Route");
            var routes = new GMapOverlay("routes");
            routes.Routes.Add(r);
            gMapControl1.Overlays.Add(routes);

            return Convert.ToInt32(route.Distance);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            GMapProviders.GoogleMap.ApiKey = ApiKey.Key;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            double lat = Convert.ToDouble(textBox1.Text);
            double lng = Convert.ToDouble(textBox2.Text);
            gMapControl1.Position = new PointLatLng(lat, lng);


            PointLatLng point = new PointLatLng(lat, lng);
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
            GMapOverlay markers = new GMapOverlay("markers");
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);
            gMapControl1.MaxZoom = 100;
            gMapControl1.MinZoom = 1;
            gMapControl1.Zoom = 10;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            PostFirebase(textBox1.Text, textBox2.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            KoordinatlarList.list.Clear();
            GlobalArray.sayac = 0;
            GlobalArray.globalNoktalar.Clear();
            GlobalArray.index = 0;
            GlobalArray.noktaSayisi = 0;
            Array.Clear(GlobalArray.toplamKenar, 0, GlobalArray.toplamKenar.Length);
            Array.Clear(toplamKenar, 0, toplamKenar.Length);
            Array.Clear(toplamKm, 0, toplamKm.Length);
            Array.Clear(globalDizi, 0, globalDizi.Length);
            listBox1.Items.Clear();
            KoordinatlariGetir();
            
        
        }


        private void button3_Click(object sender, EventArgs e)
        {
            var route = GoogleMapProvider.Instance.GetRoute(GlobalArray.globalNoktalar[0], GlobalArray.globalNoktalar[1], false, false, 14);
            var r = new GMapRoute(route.Points, "My Route");
            var routes = new GMapOverlay("routes");
            routes.Routes.Add(r);
            gMapControl1.Overlays.Add(routes);

            textBox3.Text = route.Distance + "Km";
        }


        private void button5_Click(object sender, EventArgs e)
        {
            MapPage mapPage = new MapPage();
            mapPage.Show();
        }


        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                double lat = point.Lat;
                double lng = point.Lng;
                textBox1.Text = lat + "";
                textBox2.Text = lng + "";

                LoadMap(point);


                point = new PointLatLng(lat, lng);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
                GMapOverlay markers = new GMapOverlay("markers");
                markers.Markers.Add(marker);
                gMapControl1.Overlays.Add(markers);

                var adresler = AdresGetir(point);

                if (adresler != null)
                    richTextBox1.Text = "Adres:  \n--------------\n" + String.Join(",", adresler.ToArray());
                else
                    richTextBox1.Text = "Adres bulunmadı";
            }
        }


        private List<String> AdresGetir(PointLatLng point)
        {
            List<Placemark> placemarks = null;
            var statusCode = GMapProviders.GoogleMap.GetPlacemarks(point, out placemarks);

            if (statusCode == GeoCoderStatusCode.OK && placemarks != null)
            {
                List<String> adresler = new List<string>();

                foreach (var placemark in placemarks)
                {
                    adresler.Add(placemark.Address);
                }
                return adresler;
            }
            return null;
        }


        private void LoadMap(PointLatLng point)
        {
            gMapControl1.Position = point;
        }


        private void button6_Click(object sender, EventArgs e)
        {
            /*int indexCount = GlobalArray.globalPoints.Count();
            indexCount -= 1;
            FirebaseResponse res = await client.DeleteAsync("Coordinates/" + totalArray[GlobalArray.index, deleteForMarkers]);
            SetResponse setİndex = client.Set("İndex", indexCount);
            deleteForMarkers++;
            MessageBox.Show("Ok");*/

            Sil(GlobalArray.globalNoktalar[Convert.ToInt32(GlobalArray.toplamKenar[GlobalArray.index, 0])]);
            KoordinatlarList.list.Clear();
            GlobalArray.sayac = 0;
            GlobalArray.globalNoktalar.Clear();
            GlobalArray.index = 0;
            GlobalArray.noktaSayisi = 0;
            Array.Clear(GlobalArray.toplamKenar, 0, GlobalArray.toplamKenar.Length);
            Array.Clear(toplamKenar, 0, toplamKenar.Length);
            Array.Clear(toplamKm, 0, toplamKm.Length);
            Array.Clear(globalDizi, 0, globalDizi.Length);
            listBox1.Items.Clear();
            KoordinatlariGetir();

        }
    }
}


