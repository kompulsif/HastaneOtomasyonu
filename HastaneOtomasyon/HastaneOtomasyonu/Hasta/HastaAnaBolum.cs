using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class HastaAnaBolum : Form
    {
        public HastaAnaBolum()
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
        private string id = "";
        SQLBaglantisi bgl = new SQLBaglantisi();

        private void HastaAnaBolum_load(object sender, EventArgs e)
        {
            try
            {
                lblTCKimlikNo.Text = tcNo;

                SqlCommand komut = new SqlCommand("Select HastaAd,HastaSoyad from tbl_Hastalar where HastaTCKimlikNo=@tcno", bgl.baglanti());
                komut.Parameters.AddWithValue("@tcno", lblTCKimlikNo.Text);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    lblAdSoyad.Text = dr[0] + " " + dr[1];
                }
                bgl.kapat();

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select RandevuId, RandevuTarih as 'TARİH', RandevuSaat as 'SAAT', RandevuBrans as 'ALAN', RandevuDoktor as 'DOKTOR', RandevuDurum as 'DURUM', HastaTCKimlikNo as 'KİMLİK', HastaSikayet as 'ŞİKAYET' from tbl_Randevular where HastaTCKimlikNo=" + tcNo, bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                bgl.kapat();

                SqlCommand komut2 = new SqlCommand("Select BransAd from tbl_Branslar", bgl.baglanti());
                SqlDataReader dr2 = komut2.ExecuteReader();
                while (dr2.Read())
                {
                    cmbBrans.Items.Add(dr2[0]);
                }
                bgl.kapat();
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                satirlar(dataGridView1, 35);
            }
            catch
            {
                MessageBox.Show("Veritabanından bilgi alınırken bir hata oluştu\nKapanıyor...");
                Thread.Sleep(2);
                this.Close();
            }
        }

        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut3 = new SqlCommand("Select DoktorAd,DoktorSoyad from tbl_Doktorlar where DoktorBrans=@brans", bgl.baglanti());
                komut3.Parameters.AddWithValue("@brans", cmbBrans.Text);
                SqlDataReader dr3 = komut3.ExecuteReader();
                cmbDoktor.Items.Clear();
                cmbDoktor.SelectedItem = null;
                while (dr3.Read())
                {
                    cmbDoktor.Items.Add(dr3[0] + " " + dr3[1]);
                }
                if (cmbDoktor.Items.Count > 0) { cmbDoktor.SelectedIndex = 0; }
            }
            catch
            {
                MessageBox.Show("Veritabanından bilgi alınırken bir hata oluştu!\nKapanıyor...");
                Thread.Sleep(2);
                this.Close();
            }

            bgl.kapat();
            
        }
        private void satirlar(DataGridView d, int y)
        {
            for (int i = 0; i < d.Rows.Count; i++)
            {
                d.Rows[i].Height = y + 5;
            }
        }
        private void cmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                //Randevusu bos olanlarin gözükmesi
                SqlDataAdapter da = new SqlDataAdapter("select RandevuId, RandevuTarih as 'TARİH', RandevuSaat as 'SAAT', RandevuBrans as 'ALAN', RandevuDoktor as 'DOKTOR', RandevuDurum as 'DURUM', HastaTCKimlikNo as 'KİMLİK', HastaSikayet as 'ŞİKAYET' from tbl_Randevular where RandevuBrans='" + cmbBrans.Text + "'" + "and RandevuDoktor='" + cmbDoktor.Text + "'and RandevuDurum=0", bgl.baglanti());
                da.Fill(dt);
                dataGridView2.DataSource = dt;
                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[5].Visible = false;
                dataGridView2.Columns[6].Visible = false;
                dataGridView2.Columns[7].Visible = false;
                satirlar(dataGridView2, 35);
                id = "";
            }
            catch
            {
                MessageBox.Show("Randevu bilgileri alınırken bir hata oluştu!\nKapanıyor...");
                Thread.Sleep(2);
                this.Close();
            }
            bgl.kapat();
        }

        private void btnRandevuAl_Click(object sender, EventArgs e)
        {
            if (rchSikayet.Text.Length >= 5 && cmbBrans.SelectedItem != null && cmbDoktor.SelectedItem != null)
            {
                if (id != "")
                {
                    try
                    {
                        SqlCommand komut = new SqlCommand("update tbl_Randevular set RandevuDurum=1,HastaTCKimlikNo=@tc,HastaSikayet=@sikayet where RandevuId=@id", bgl.baglanti());
                        komut.Parameters.AddWithValue("@tc", lblTCKimlikNo.Text);
                        komut.Parameters.AddWithValue("@sikayet", rchSikayet.Text);
                        komut.Parameters.AddWithValue("@id", id);
                        komut.ExecuteNonQuery();
                        bgl.baglanti().Close();
                        MessageBox.Show("Randevu Alındı", "Bilgi", MessageBoxButtons.OK);
                        cmbDoktor_SelectedIndexChanged(null, null);
                        HastaAnaBolum_load(null, null);
                        Thread.Sleep(1);
                    }
                    catch { MessageBox.Show("Randevu alınamadı!"); }
                } else { MessageBox.Show("Seçili bir randevu bulunamadı!"); }
                
            } else { MessageBox.Show("Alan ve Doktor bilgisi seçilmeli ve şikayet ise en az 5 karakterden oluşmalıdır"); }

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 1)
            {
                int secilen = dataGridView2.SelectedCells[0].RowIndex;
                id = dataGridView2.Rows[secilen].Cells[0].Value.ToString();
                MessageBox.Show("Randevu Seçildi!");
            }
        }

        private void lnkBilgiDuzenle_LinkClicked(object sender, EventArgs e)
        {
            HastaBilgiDuzenle frm = new HastaBilgiDuzenle();
            frm.TCNo = lblTCKimlikNo.Text;
            frm.ShowDialog();
        }

        private void cmbBrans_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cmbDoktor_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void FrmHastaDetay_FormClosing(object sender, FormClosingEventArgs e)
        {
            bgl.kapat();
            HastaGirisi frm = new HastaGirisi();
            frm.Show();
        }

        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
