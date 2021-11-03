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

namespace yazlabpro1
{
    public partial class Form1 : Form
    {
        IFirebaseConfig ifare = new FirebaseConfig()
        {

            AuthSecret = "Enter AuthSecret",
            BasePath = "Enter BasePath"

        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                client = new FireSharp.FirebaseClient(ifare);
            }

            catch
            {
                MessageBox.Show("İnternete bağlanın");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region Condition
            if (string.IsNullOrWhiteSpace(textBox1.Text) && string.IsNullOrWhiteSpace(textBox2.Text))
            {

                MessageBox.Show("gerekli yerleri doldurun");
                return;
            }
            #endregion
            

            FirebaseResponse res = client.Get("Kullanıcılar/" + textBox1.Text);
            Kullanicilar ResUser = res.ResultAs<Kullanicilar>();
            Kullanicilar CurUser = new Kullanicilar()
            {
                KullaniciAdi = textBox1.Text,
                Sifre = textBox2.Text
            };

            if (Kullanicilar.IsEqual(ResUser, CurUser))
            {

                HomePage real = new HomePage();
                real.ShowDialog();
                KullaniciAdi.Kullanici = textBox1.Text;
            }

            else
            {

                Kullanicilar.ShowError();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KayitOl register = new KayitOl();
            register.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Kullanicilar kullanici = new Kullanicilar()
            {
                KullaniciAdi = textBox1.Text,
                Sifre = textBox2.Text
            };
            var setter = client.Set("Kullanıcılar/" + textBox1.Text, kullanici);
            MessageBox.Show("Başardın adamım");
        }
    }
}
