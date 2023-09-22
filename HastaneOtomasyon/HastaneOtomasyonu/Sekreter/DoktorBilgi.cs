using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class DoktorBilgi : Form
    {
        public DoktorBilgi()
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

        public void Temizle()
        {
            txtAd.Clear();
            txtSoyad.Clear();
            mskTCKimlikNo.Clear();
            txtSifre.Clear();
            dgwDoktorPanel.DataSource = null;
            cmbBrans.SelectedIndex = -1;
            DoktorBilgi_load(null, null);
        }

        private void DoktorBilgi_load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter("select(DoktorID) as 'ID', (DoktorAd) as 'AD', (DoktorSoyad) as 'SOYAD', (DoktorBrans) as 'BRANS', (DoktorTCKimlikNo) as 'TC KIMLIK', (DoktorSifre) as 'SIFRE' from tbl_Doktorlar", bgl.baglanti());
                da1.Fill(dt1);
                dgwDoktorPanel.DataSource = dt1;

                SqlCommand komut2 = new SqlCommand("Select BransAd from tbl_Branslar", bgl.baglanti());
                SqlDataReader dr2 = komut2.ExecuteReader();
                while (dr2.Read())
                {
                    cmbBrans.Items.Add(dr2[0]);
                }
                bgl.kapat();
            } catch { MessageBox.Show("Doktorlar görüntülenirken bir sorun oluştu!\nVeritabanını kontrol ediniz."); }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (txtAd.Text.Length >= 3 && txtSoyad.Text.Length >= 2 && cmbBrans.SelectedIndex != -1 && mskTCKimlikNo.MaskCompleted && txtSifre.Text.Length >= 4)
            {
                bool x = true;
                foreach (DataGridViewRow satir in dgwDoktorPanel.Rows)
                {
                    if (satir.Cells[4].Value.ToString().Trim() == mskTCKimlikNo.Text)
                    {
                        MessageBox.Show("Bu TC kimlik kullanımdadır!");
                        x = false;
                        break;
                    }
                }
                if (x)
                {
                    try
                    {
                        SqlCommand komut = new SqlCommand("insert into tbl_Doktorlar (DoktorAd,DoktorSoyad,DoktorBrans,DoktorTCKimlikNo,DoktorSifre) values (@ad,@soyad,@brans,@tc,@sifre)", bgl.baglanti());
                        komut.Parameters.AddWithValue("@ad", txtAd.Text);
                        komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                        komut.Parameters.AddWithValue("@brans", cmbBrans.Text);
                        komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                        komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                        komut.ExecuteNonQuery();
                        bgl.kapat();
                        MessageBox.Show("Doktor Eklendi");
                        Temizle();
                    }
                    catch { MessageBox.Show("Doktor eklenemedi!"); }
                }
            } else { MessageBox.Show("Eksik bilgileri doldurunuz"); }
        }

        private void dgwDoktorPanel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dgwDoktorPanel.SelectedCells[0].RowIndex;
            string bransi = dgwDoktorPanel.Rows[secilen].Cells[3].Value.ToString();
            bool d = false;
            for (int i = 0; i < cmbBrans.Items.Count; i++)
            {
                string b = (string)cmbBrans.Items[i];
                if (bransi.Trim() == b.Trim())
                {
                    d = true;
                    cmbBrans.SelectedIndex = i;
                }
            }
            txtAd.Text = dgwDoktorPanel.Rows[secilen].Cells[1].Value.ToString();
            txtSoyad.Text = dgwDoktorPanel.Rows[secilen].Cells[2].Value.ToString();
            txtSifre.Text = dgwDoktorPanel.Rows[secilen].Cells[5].Value.ToString();
            mskTCKimlikNo.Text = dgwDoktorPanel.Rows[secilen].Cells[4].Value.ToString();
            if (!d) {
                MessageBox.Show("Seçili doktorun branşı listede bulunamadı!\nLütfen hata oluşmaması için veritabanında birebir branş eklemesi veya doktor güncellemesini gerçekleştirin!");
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgwDoktorPanel.SelectedRows.Count > 0)
            {
                if (txtAd.Text.Length >= 3 && txtSoyad.Text.Length >= 2 && cmbBrans.SelectedIndex != -1 && mskTCKimlikNo.Text.Length == 11 && txtSifre.Text.Length >= 4)
                {
                    try
                    {
                        SqlCommand komut = new SqlCommand("Update tbl_Doktorlar set doktorAd=@ad,doktorSoyad=@soyad,doktorBrans=@brans,doktorSifre=@sifre where doktorTCKimlikNo=@tc", bgl.baglanti());
                        komut.Parameters.AddWithValue("@ad", txtAd.Text);
                        komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                        komut.Parameters.AddWithValue("@brans", cmbBrans.Text);
                        komut.Parameters.AddWithValue("@tc", mskTCKimlikNo.Text);
                        komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                        komut.ExecuteNonQuery();
                        bgl.kapat();
                        MessageBox.Show("Doktor Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Temizle();
                    }
                    catch { MessageBox.Show("Doktor Bilgisi Güncellenemedi!"); }
                }
                else { MessageBox.Show("Doktor Bilgisi Güncellenemedi!"); }
            } else { MessageBox.Show("Henüz bir doktor seçmediniz"); }
        }

        private void btnListeGuncelle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void cmbBrans_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void doktoruSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgwDoktorPanel.SelectedRows.Count > 0)
            {
                string tcKimlik = dgwDoktorPanel.SelectedRows[0].Cells[4].Value.ToString();
                if (tcKimlik.Length == 11)
                {
                    try
                    {
                        SqlCommand komut = new SqlCommand("delete from tbl_Doktorlar where DoktorTCKimlikNo=@p1", bgl.baglanti());
                        komut.Parameters.AddWithValue("p1", tcKimlik);
                        komut.ExecuteNonQuery();
                        bgl.kapat();
                        MessageBox.Show("Doktor Silindi!");
                        Temizle();
                    }
                    catch { MessageBox.Show("Kayıt Silinemedi!"); }
                }
                else { MessageBox.Show("TC Kimlik doğru uzunlukta değil!\nLütfen güncelleme yapınız."); }
            } else { MessageBox.Show("Bir seçim yapmadınız"); }
        }
    }
}
