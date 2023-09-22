using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class HastaBilgiDuzenle : Form
    {
        public HastaBilgiDuzenle()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        public string TCNo;
        SQLBaglantisi bgl = new SQLBaglantisi();

        private void HastaBilgiDuzenle_load(object sender, EventArgs e)
        {
            mskTCKimlikNo.Text = TCNo;

            //Güncelleme Islemi
            cmbCinsiyet.Items.Add("Erkek");
            cmbCinsiyet.Items.Add("Kadin");
            SqlCommand komut = new SqlCommand("select * from tbl_Hastalar where HastaTCKimlikNo=@tcno", bgl.baglanti());
            komut.Parameters.AddWithValue("tcno", mskTCKimlikNo.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                txtAd.Text = dr[1].ToString();
                txtSoyad.Text = dr[2].ToString();                
                mskTelefon.Text = dr[4].ToString();
                txtSifre.Text = dr[5].ToString();
                cmbCinsiyet.SelectedIndex = (dr[6].ToString().ToLower() == "erkek") ? 0 : 1;
            }
            bgl.kapat();
        }

        private void btnBilgiGuncelle_Click(object sender, EventArgs e)
        {
            if (txtAd.Text.Trim().Length >= 3 && txtSifre.Text.Length >= 4 && txtSoyad.Text.Trim().Length >= 2 && mskTelefon.MaskCompleted)
            {
                try
                {
                    SqlCommand komut2 = new SqlCommand("update tbl_hastalar set HastaAd=@ad,HastaSoyad=@soyad,HastaTelefon=@telefon,HastaSifre=@sifre,HastaCinsiyet=@cinsiyet where HastaTCKimlikNo =@tc", bgl.baglanti());
                    komut2.Parameters.AddWithValue("@ad", txtAd.Text);
                    komut2.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                    komut2.Parameters.AddWithValue("@telefon", mskTelefon.Text);
                    komut2.Parameters.AddWithValue("@sifre", txtSifre.Text);
                    komut2.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text);
                    komut2.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                    komut2.ExecuteNonQuery();
                    bgl.kapat();
                    MessageBox.Show("Hasta Bilgisi Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
                catch { MessageBox.Show("Bilgiler Güncellenemedi!"); }
            } else { MessageBox.Show("Lütfen eksik bilgileri doldurunuz"); }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void cmbCinsiyet_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
