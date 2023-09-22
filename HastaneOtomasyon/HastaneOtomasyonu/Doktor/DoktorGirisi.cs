using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class DoktorGirisi : Form
    {
        public DoktorGirisi()
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

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            if (mskTCKimlikNo.MaskCompleted && txtSifre.Text.Length >= 4)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("select * from tbl_Doktorlar where DoktorTCKimlikNo=@tc and DoktorSifre=@sifre", bgl.baglanti());
                    komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                    komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                    SqlDataReader dr = komut.ExecuteReader();
                    if (dr.Read())
                    {
                        DoktorAnaBolum frm = new DoktorAnaBolum();
                        frm.tcNo = mskTCKimlikNo.Text;
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("TC Kimlik No & Sifre Yanlis", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    bgl.kapat();
                } catch { MessageBox.Show("Bir hata oluştu! Doğrulama yapılamadı!"); }
            } else { MessageBox.Show("Eksik bilgiler var!"); }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            GirisBolumleri frm = new GirisBolumleri();
            frm.Show();
            this.Close();
        }

        private void FrmDoktorGiris_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
