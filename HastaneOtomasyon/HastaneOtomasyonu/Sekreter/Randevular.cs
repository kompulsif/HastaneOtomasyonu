using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class Randevular : Form
    {
        public Randevular()
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

        private void RandevuListesi_load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select RandevuId,RandevuTarih as 'Tarih', RandevuSaat as 'Saat', RandevuBrans as 'Branş', RandevuDoktor as 'Doktor', RandevuDurum as 'Durum', HastaTCKimlikNo as 'Kimlik', HastaSikayet as 'Şikayet'  from tbl_Randevular", bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                bgl.kapat();
                dataGridView1.Columns[0].Visible = false;
            } catch { MessageBox.Show("Randevu bilgileri alınamıyor!\nVeritabanını kontrol ediniz."); this.Close(); }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void randevuSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    using (SqlCommand k = new SqlCommand("delete from tbl_Randevular where RandevuId = @p1", bgl.baglanti()))
                    {
                        k.Parameters.AddWithValue("@p1", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                        k.ExecuteNonQuery();
                        MessageBox.Show("Randevu Silindi!");
                        RandevuListesi_load(null, null);
                    }
                    bgl.kapat();
                }
                catch
                {
                    MessageBox.Show("Bir hata oluştu!\nRandevu silinemedi!");
                }
            }
            else { MessageBox.Show("Bir randevu seçmediniz..."); }
        }
    }
}
