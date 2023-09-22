using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class SekreterGirisi : Form
    {
        public SekreterGirisi()
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
            if (mskTCKimlikNo.MaskCompleted && txtSifre.Text.Length >= 4) {
                try
                {
                    SqlCommand komut = new SqlCommand("select * from tbl_Sekreterler where SekreterTCKimlikNo=@tc and SekreterSifre=@sifre", bgl.baglanti());
                    komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                    komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                    SqlDataReader dr = komut.ExecuteReader();
                    if (dr.Read())
                    {
                        SekreterAnaBolum frm = new SekreterAnaBolum();
                        frm.tcNo = mskTCKimlikNo.Text;
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("TC Kimlik No & Sifre Yanlis", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    bgl.kapat();

                } catch { MessageBox.Show("Veritabanına bağlanılamadı!\nLütfen daha sonra tekrar deneyiniz."); }
            }
            else { MessageBox.Show("Kimlik 11 | Parola en az 4 haneli olmalıdır!"); }
        }

        private void FrmSekreterGiris_FormClosing(object sender, FormClosingEventArgs e)
        {
            bgl.kapat();
            GirisBolumleri frm = new GirisBolumleri();
            frm.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSekreterGiris_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
