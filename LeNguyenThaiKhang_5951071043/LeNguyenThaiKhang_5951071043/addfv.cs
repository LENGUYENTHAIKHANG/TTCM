using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeNguyenThaiKhang_5951071043
{
   
    public partial class addfv : Form
    {
        String url;
        public String favName, favFile;

        private void addfv_Load(object sender, EventArgs e)
        {
            textBox3.Text = url;
            comboBox1.Text = comboBox1.Items[0].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            favName = textBox3.Text;
            favFile = comboBox1.SelectedItem.ToString();
        }

        public addfv(String url)
        {
            this.url = url;
            InitializeComponent();
        }
    }
}
