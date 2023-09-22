using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class SekreterAnaBolum : Form
    {
        public SekreterAnaBolum()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public string tcNo;

        SQLBaglantisi bgl = new SQLBaglantisi();

        private void SekreterAnaBolum_load(object sender, EventArgs e)
        {
            lblTCKimlikNo.Text = tcNo;

            try
            {
                SqlCommand komut = new SqlCommand("select SekreterAdSoyad from tbl_Sekreterler where SekreterTCKimlikNo=@tc", bgl.baglanti());
                komut.Parameters.AddWithValue("@tc", lblTCKimlikNo.Text);
                SqlDataReader dr1 = komut.ExecuteReader();
                while (dr1.Read())
                {
                    lblAdSoyad.Text = dr1[0].ToString();
                }
                bgl.kapat();

                DataTable dt1 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select BransId, (BransAd) as 'Brans Adi' from tbl_Branslar", bgl.baglanti());
                da.Fill(dt1);
                dgwBranslar.DataSource = dt1;
                dgwBranslar.Columns[0].Visible = false;

                DataTable dt2 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter("select (DoktorAd + ' ' + DoktorSoyad) as 'Doktorlar',(DoktorBrans) as 'Brans' from tbl_Doktorlar", bgl.baglanti());
                da2.Fill(dt2);
                dgwDoktorlar.DataSource = dt2;

                SqlCommand komut2 = new SqlCommand("Select BransAd from tbl_Branslar", bgl.baglanti());
                SqlDataReader dr2 = komut2.ExecuteReader();
                while (dr2.Read())
                {
                    cmbBrans.Items.Add(dr2[0]);
                }
                bgl.kapat();
            }
            catch { MessageBox.Show("Yükleme sırasında bir sorun oluştu, lütfen daha sonra tekrar deneyiniz..");
                bgl.kapat();
                this.Close();
            }            
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (mskTarih.MaskCompleted && mskSaat.MaskCompleted && cmbBrans.SelectedIndex != -1 && cmbDoktor.SelectedIndex != -1)
            {
                try
                {
                    SqlCommand komutKaydet = new SqlCommand("insert into tbl_Randevular (RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor) values (@tarih,@saat,@brans,@doktor)", bgl.baglanti());
                    komutKaydet.Parameters.AddWithValue("@tarih", mskTarih.Text);
                    komutKaydet.Parameters.AddWithValue("@saat", mskSaat.Text);
                    komutKaydet.Parameters.AddWithValue("@brans", cmbBrans.Text);
                    komutKaydet.Parameters.AddWithValue("@doktor", cmbDoktor.Text);
                    komutKaydet.ExecuteNonQuery();
                    bgl.kapat();
                    MessageBox.Show("Randevu Oluşturuldu", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { MessageBox.Show("Randevu Oluşturulamadı!"); }
            } else { MessageBox.Show("Eksik bilgiler var!"); }
        }

        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut = new SqlCommand("Select DoktorAd,DoktorSoyad from tbl_Doktorlar where DoktorBrans=@brans", bgl.baglanti());
                komut.Parameters.AddWithValue("@brans", cmbBrans.Text);
                SqlDataReader dr = komut.ExecuteReader();
                cmbDoktor.Items.Clear();
                cmbDoktor.Text = "";
                while (dr.Read())
                {
                    cmbDoktor.Items.Add(dr[0] + " " + dr[1]);
                }
                if (cmbDoktor.Items.Count > 0) { cmbDoktor.SelectedIndex = 0; }
                bgl.kapat();
                dgwBranslar.Columns[0].Visible = false;
            } catch { MessageBox.Show("Doktor bilgisi alınırken bir sorun oluştu!\nLütfen daha sonra tekrar deneyiniz"); }
        }

        private void btnDuyuruOlustur_Click(object sender, EventArgs e)
        {
            if (rchDuyuru.Text.Trim().Length >= 10)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("insert into tbl_Duyurular (duyuru) values (@d1)", bgl.baglanti());
                    komut.Parameters.AddWithValue("@d1", rchDuyuru.Text);
                    komut.ExecuteNonQuery();
                    bgl.kapat();
                    MessageBox.Show("Duyuru Oluşturuldu", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { MessageBox.Show("Duyuru oluşturulamadı!"); }
            } else { MessageBox.Show("Duyuru en az 10 karakter içermelidir!"); }
        }

        private void btnDoktorPaneli_Click(object sender, EventArgs e)
        {
            DoktorBilgi drp = new DoktorBilgi();
            drp.ShowDialog();
        }

        private void btnBransPaneli_Click(object sender, EventArgs e)
        {
            BransBilgi frb = new BransBilgi();
            frb.ShowDialog();
        }

        private void btnRandevuListe_Click(object sender, EventArgs e)
        {
            Randevular frl = new Randevular();
            frl.ShowDialog();
        }

        private void btnDuyurular_Click(object sender, EventArgs e)
        {
            Duyurular frd = new Duyurular();
            frd.ShowDialog();
        }

        private void FrmSekreterDetay_FormClosing(object sender, FormClosingEventArgs e)
        {
            bgl.kapat();
            GirisBolumleri frm = new GirisBolumleri();
            frm.Show();
        }

        private void label12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void cmbBrans_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cmbDoktor_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void bilgileriGuncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SekreterAnaBolum_load(null, null);
        }
    }
}
