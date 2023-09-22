using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class DoktorBilgiDuzenle : Form
    {
        public DoktorBilgiDuzenle()
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
        public string DrTcNo;

        private void DoktorBilgiDuzenle_load(object sender, EventArgs e)
        {
            try
            {
                mskTCKimlikNo.Text = DrTcNo;
                SqlCommand komut = new SqlCommand("Select * from tbl_Doktorlar where DoktorTCKimlikNo = @tc", bgl.baglanti());
                komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                SqlDataReader dr = komut.ExecuteReader();
                string bransi = "";
                while (dr.Read())
                {
                    bransi = dr[3].ToString();
                    txtSifre.Text = dr[5].ToString();
                }
                    
                bgl.kapat();
                SqlCommand komut2 = new SqlCommand("Select BransAd from tbl_Branslar", bgl.baglanti());
                SqlDataReader dr2 = komut2.ExecuteReader();
                while (dr2.Read())
                {
                    cmbBrans.Items.Add(dr2[0]);
                }
                bgl.kapat();
                if (cmbBrans.Items.Count > 0) { cmbBrans.SelectedIndex = 0; }
                for (int i = 0; i < cmbBrans.Items.Count; i++)
                {
                    if (cmbBrans.Items[i].ToString() == bransi)
                    {
                        cmbBrans.SelectedIndex = i;
                    }
                }
            } catch {
                MessageBox.Show("Yükleme sırasında bir hata oluştu!\nLütfen daha sonra tekrar deneyiniz.");
                this.Close();
            }
        }

        private void btnBilgiGuncelle_Click(object sender, EventArgs e)
        {
            if (txtSifre.Text.Length >= 4 && mskTCKimlikNo.MaskCompleted)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("update tbl_Doktorlar set DoktorBrans=@brans, DoktorSifre=@sifre where DoktorTCKimlikNo=@tc", bgl.baglanti());
                    komut.Parameters.AddWithValue("@brans", cmbBrans.Text);
                    komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                    komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                    komut.ExecuteNonQuery();
                    bgl.kapat();
                    MessageBox.Show("Bilgiler Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { MessageBox.Show("Bilgiler Güncellenemedi!"); }
            } else { MessageBox.Show("Eksik bilgiler var!"); }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbBrans_KeyPress(object sender, KeyPressEventArgs e)
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

        private void mskTCKimlikNo_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Kimlik Değiştirilemez!");
        }
    }
}
