using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yazlabpro1
{
    class Kullanicilar
    {
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }

        private static string error = "Hata";

        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(error);
        }

        public static bool IsEqual(Kullanicilar kullanici1, Kullanicilar kullanici2)
        {
            if (kullanici1 == null || kullanici2 == null) { return false; }

            if (kullanici1.KullaniciAdi != kullanici2.KullaniciAdi)
            {
                error = "Username does not exist!";
                return false;
            }

            else if (kullanici1.Sifre != kullanici2.Sifre)
            {
                error = "Hata";
                return false;
            }

            return true;
        }
    }
}
