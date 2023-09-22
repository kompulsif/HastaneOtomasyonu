using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class DoktorAnaBolum : Form
    {
        public DoktorAnaBolum()
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

        private void DoktorAnaBolum_load(object sender, EventArgs e)
        {

            try
            {
                lblTCKimlikNo.Text = tcNo;
                SqlCommand komut = new SqlCommand("select DoktorAd,DoktorSoyad from tbl_Doktorlar where DoktorTCKimlikNo=@tc", bgl.baglanti());
                komut.Parameters.AddWithValue("@tc", lblTCKimlikNo.Text);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    lblAdSoyad.Text = dr[0] + " " + dr[1];
                }
                bgl.kapat();

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("Select (RandevuId) as 'ID',(RandevuTarih) as 'Tarih',(RandevuSaat) as 'Saat',(RandevuBrans) as 'Brans',(RandevuDoktor) as 'Doktor',(RandevuDurum) as 'Durum', (HastaTCKimlikNO) as 'TC No', (HastaSikayet) as 'Sikayet' from tbl_Randevular where RandevuDoktor='" + lblAdSoyad.Text + "'", bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Visible = false;
            } catch {
                MessageBox.Show("Veritabanından bilgiler alınırken bir sorun oluştu!");
                this.Close();
            }

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            DoktorBilgiDuzenle frm = new DoktorBilgiDuzenle();
            frm.DrTcNo = lblTCKimlikNo.Text;
            frm.ShowDialog();
        }

        private void btnDuyurular_Click(object sender, EventArgs e)
        {
            Duyurular frd = new Duyurular("doktor");
            frd.ShowDialog();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int secilen = dataGridView1.SelectedCells[0].RowIndex;
                rchSikayet.Text = dataGridView1.Rows[secilen].Cells[7].Value.ToString();
            } else { rchSikayet.Text = ""; }
        }

        private void FrmDoktorDetay_FormClosing(object sender, FormClosingEventArgs e)
        {
            GirisBolumleri frm = new GirisBolumleri();
            frm.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            bgl.kapat();
            this.Close();
        }

        private void label12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
