using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HastaneOtomasyonu
{
    public partial class GirisBolumleri : Form
    {
        public GirisBolumleri()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private void btnHastaGirisi_Click(object sender, EventArgs e)
        {
            HastaGirisi frm = new HastaGirisi();
            frm.Show();
            this.Hide();
        }

        private void btnDoktorGirisi_Click(object sender, EventArgs e)
        {
            DoktorGirisi frm = new DoktorGirisi();
            frm.Show();
            this.Hide();
        }

        private void btnSekreterGirisi_Click(object sender, EventArgs e)
        {
            SekreterGirisi frm = new SekreterGirisi();
            frm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = Process.GetCurrentProcess();
            try
            {
                process.Kill();
            }
            catch { Application.Exit(); }
        }

        private void FrmGirisler_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
