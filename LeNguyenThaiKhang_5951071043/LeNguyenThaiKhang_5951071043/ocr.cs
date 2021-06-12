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
using Tesseract;

namespace LeNguyenThaiKhang_5951071043
{
    public partial class ocr : Form
    {
        OpenFileDialog op = new OpenFileDialog();
        public ocr()
        {
            InitializeComponent();
        }

        private string OCR(Bitmap b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "viet", EngineMode.Default))
            {
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
            }
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (op.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.LoadAsync(op.FileName);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string result = "";
            Task.Factory.StartNew(() => {
                picloading.BeginInvoke(new Action(() =>
                {
                    picloading.Visible = true;
                }));

                result = OCR((Bitmap)pictureBox1.Image);
                richTextBox1.BeginInvoke(new Action(() => {

                    richTextBox1.Text = result;

                }));
                picloading.BeginInvoke(new Action(() =>
                {
                    picloading.Visible = false;
                }));

            });
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Clipboard.GetImage();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String source = ("source.txt");
            StreamWriter writer = File.CreateText(source);
            writer.Write(richTextBox1.Text);
            writer.Close();
            Process.Start("notepad.exe", source);
        }
    }
}
