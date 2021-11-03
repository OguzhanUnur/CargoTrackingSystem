using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace yazlabpro1
{
    class GlobalArray
    {
        public static int sayac { get; set; } = 0;
        
        public static int noktaSayisi { get; set; } = 0;

        public static int index { get; set; } = 0;

        public static string[,] toplamKenar { get; set; } = new string[100, 100];
       
        public static List<PointLatLng> globalNoktalar { get; set; } = new List<PointLatLng>();
    }
}
