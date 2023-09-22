using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class HastaKayit : Form
    {
        public HastaKayit()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        SQLBaglantisi bgl = new SQLBaglantisi();

        private void btnKayitYap_Click(object sender, EventArgs e)
        {
            if (mskTCKimlikNo.MaskCompleted && txtAd.Text.Trim().Length >= 3 && txtSoyad.Text.Trim().Length >= 2 && mskTelefon.MaskCompleted && txtSifre.Text.Length >= 4)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("insert into tbl_Hastalar (HastaAd,HastaSoyad,HastaTCKimlikNo,HastaTelefon,HastaSifre,HastaCinsiyet) values (@ad,@soyad,@tcno,@telefon,@sifre,@cinsiyet)", bgl.baglanti());
                    komut.Parameters.AddWithValue("@ad", txtAd.Text);
                    komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                    komut.Parameters.AddWithValue("@tcno", mskTCKimlikNo.Text);
                    komut.Parameters.AddWithValue("@telefon", mskTelefon.Text);
                    komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                    komut.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text);
                    komut.ExecuteNonQuery();
                    bgl.kapat();
                    MessageBox.Show("Kaydiniz Gerceklesmistir Sifreniz : " + txtSifre.Text, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                } catch { MessageBox.Show("Bir hata oluştu! Kaydınız yapılamadı!"); }
            } else { MessageBox.Show("Eksik bilgileri doldurun!"); }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void cmbCinsiyet_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void FrmHastaKayit_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}