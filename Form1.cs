using CsharpAes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncroFile
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox3.AllowDrop = true;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            pictureBox3.BackgroundImage = Properties.Resources.release;
            pictureBox3.BackColor = Color.FromArgb(49, 49, 49);
        }

        private void pictureBox3_DragLeave(object sender, EventArgs e)
        {
            pictureBox3.BackgroundImage = Properties.Resources.drop_here;
            pictureBox3.BackColor = Color.FromArgb(38, 38, 38);
        }

        private void label2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void pictureBox3_DragDrop(object sender, DragEventArgs e)
        {
            pictureBox3.BackgroundImage = Properties.Resources.drop_here;
            pictureBox3.BackColor = Color.FromArgb(38, 38, 38);
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            string f_name = Path.GetFileNameWithoutExtension(files[0]); // niiko
            string f_path = Path.GetFullPath(files[0]);                 // C:\Users\awest\Desktop\nou\niiko.txt
            string f_ext  = Path.GetExtension(files[0]);                // .txt

            string f_folder = Path.GetFullPath(files[0]).Replace(Path.GetFileName(files[0]), ""); // C:\Users\awest\Desktop\nou\

            FileAttributes attr = File.GetAttributes(files[0]);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                MessageBox.Show("Target can only be type of: file");
                return;
            }

            string password = Program.default_key;
            if (textBox1.Text.Length < 1)
            {
                //MessageBox.Show("No key provided, encryption using default: " + Program.default_key);
            } else
            {
                password = textBox1.Text;
            }

            if (File.ReadAllText(files[0]).StartsWith("ENCRYPTED"))
            {
                pictureBox3.BackgroundImage = Properties.Resources.decrypting;

                string plaintext = File.ReadAllText(files[0]).Replace("ENCRYPTED", "");
                string cipher;
                using (var aes = new PasswordAes(password))
                {
                    File.WriteAllText(f_folder + f_name + f_ext, aes.Decrypt(plaintext));
                    pictureBox3.BackgroundImage = Properties.Resources.done;
                }
            } else
            {
                pictureBox3.BackgroundImage = Properties.Resources.encrypting;
                string plaintext = File.ReadAllText(files[0]);
                string cipher;
                using (var aes = new PasswordAes(password))
                    cipher = aes.Encrypt(plaintext);

                File.WriteAllText(f_folder + f_name + f_ext, "ENCRYPTED" + cipher);
                pictureBox3.BackgroundImage = Properties.Resources.done;
            }
        }
    }
}
