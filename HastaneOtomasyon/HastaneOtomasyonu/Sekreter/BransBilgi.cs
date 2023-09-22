using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class BransBilgi : Form
    {
        public BransBilgi()
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
        string bransID = "";
        public void FormTemizle()
        {
            txtBransAd.Text = "";
            dataGridView1.DataSource = null;
            BransBilgi_load(null, null);
            dataGridView1.RowHeadersVisible = false;
        }

        private void BransBilgi_load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select BransId, bransAd as 'Branş Adı' from tbl_Branslar", bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Visible = false;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Height = 35;
                }
                bgl.kapat();
            } catch { MessageBox.Show("Bir hata meydana geldi!"); 
                bgl.kapat(); this.Close(); }

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (txtBransAd.Text.Length >= 3)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("insert into tbl_Branslar (BransAd) values (@bransad)", bgl.baglanti());
                    komut.Parameters.AddWithValue("@bransad", txtBransAd.Text);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Brans Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bgl.kapat();
                    FormTemizle();
                }
                catch { MessageBox.Show("Branş Eklenemedi!"); }
            } else MessageBox.Show("Ekleyeceğiniz branş adının uzunluğu en az 3 karakter olmalı!");

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int secilen = dataGridView1.SelectedCells[0].RowIndex;
                bransID = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
                txtBransAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
                MessageBox.Show("Branş Seçildi!");
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (bransID != "")
            {
                try
                {
                    using (SqlCommand komut = new SqlCommand("update tbl_Branslar set BransAd=@ad where BransId=@id", bgl.baglanti()))
                    {
                        komut.Parameters.AddWithValue("@ad", txtBransAd.Text);
                        komut.Parameters.AddWithValue("@id", bransID);
                        komut.ExecuteNonQuery();
                    }

                    using (SqlCommand komut = new SqlCommand("update tbl_Doktorlar set DoktorBrans=@yeniAd where DoktorBrans=@eskiAd", bgl.baglanti()))
                    {
                        komut.Parameters.AddWithValue("@yeniAd", txtBransAd.Text);
                        komut.Parameters.AddWithValue("@eskiAd", dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
                        komut.ExecuteNonQuery();
                    }

                    using (SqlCommand komut = new SqlCommand("update tbl_Randevular set RandevuBrans=@yeniAd where RandevuBrans=@eskiAd", bgl.baglanti()))
                    {
                        komut.Parameters.AddWithValue("@yeniAd", txtBransAd.Text);
                        komut.Parameters.AddWithValue("@eskiAd", dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
                        komut.ExecuteNonQuery();
                    }

                    MessageBox.Show("Brans, branşa ait tüm doktorların ve randevuların bilgileri güncellendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bransID = "";
                    bgl.kapat();
                    FormTemizle();
                }
                catch { MessageBox.Show("Branş Güncellenemedi!"); }
            } else MessageBox.Show("Seçili bir branş bulunamadı!");
        }

        private void btnListeGuncelle_Click(object sender, EventArgs e)
        {
            FormTemizle();
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

        private void bransSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    SqlCommand komut = new SqlCommand("delete from tbl_Branslar where BransId=@id", bgl.baglanti());
                    komut.Parameters.AddWithValue("@id", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Branş Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    bgl.kapat();
                    FormTemizle();
                }
                catch { MessageBox.Show("Branş Silinemedi!"); }
            }
            else MessageBox.Show("Seçili bir branş bulunamadı!");
        }
    }
}
