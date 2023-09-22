using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class HastaGirisi : Form
    {
        public HastaGirisi()
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

        private void label5_Click(object sender, EventArgs e)
        {
            HastaKayit frm = new HastaKayit();
            frm.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            GirisBolumleri frm = new GirisBolumleri();
            frm.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (mskTCKimlikNo.MaskCompleted && txtSifre.Text.Length >= 4)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("select * from tbl_Hastalar where HastaTCKimlikNo=@tcno and HastaSifre=@sifre", bgl.baglanti());
                    komut.Parameters.AddWithValue("tcno", mskTCKimlikNo.Text);
                    komut.Parameters.AddWithValue("sifre", txtSifre.Text);
                    SqlDataReader dr = komut.ExecuteReader();
                    if (dr.Read())
                    {
                        bgl.kapat();
                        HastaAnaBolum frm = new HastaAnaBolum();
                        frm.tcNo = mskTCKimlikNo.Text;
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Hatalı TC Kimlik No & Sifre", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    bgl.kapat();
                }
                catch { MessageBox.Show("Bir hata oluştu! Doğrulama yapılamadı!"); }
            }
            else { MessageBox.Show("TC Kimlik numarası 11 haneli ve şifre de en az 4 haneli olmalıdır!"); }
        }

        private void FrmHastaGiris_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
