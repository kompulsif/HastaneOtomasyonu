using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HastaneOtomasyonu
{
    public partial class Duyurular : Form
    {
        private string t = "";
        public Duyurular(string t = "sekreter")
        {
            InitializeComponent();
            this.t = t;
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        SQLBaglantisi bgl = new SQLBaglantisi();

        private void Duyurular_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Duyurular", bgl.baglanti());
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Visible = false;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Height = 35;
                    dataGridView1.RowsDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                }
                bgl.kapat();
            } catch
            {
                MessageBox.Show("Duyurular yüklenemedi!\nVeritabanını kontrol ediniz.");
                this.Close();
            }
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

        private void duyuruyuSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (t == "sekreter")
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    try
                    {
                        using (SqlCommand k = new SqlCommand("delete from tbl_Duyurular where DuyuruId = @p1", bgl.baglanti()))
                        {
                            k.Parameters.AddWithValue("@p1", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                            k.ExecuteNonQuery();
                            MessageBox.Show("Duyuru Silindi!");
                            Duyurular_Load(null, null);
                        }
                        bgl.kapat();
                    }
                    catch
                    {
                        MessageBox.Show("Bir hata oluştu!\nDuyuru silinemedi!");
                    }
                }
                else { MessageBox.Show("Bir duyuru seçmediniz..."); }
            } else { MessageBox.Show("Duyuruları sadece sekreterler silebilir..."); }
        }
    }
}
