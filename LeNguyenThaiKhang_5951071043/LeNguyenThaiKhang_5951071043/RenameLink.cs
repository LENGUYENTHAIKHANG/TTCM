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
    public partial class RenameLink : Form
    {
        String oldName;
        public RenameLink(string oldName)
        {
            this.oldName = oldName;
            InitializeComponent();
        }

        private void RenameLink_Load(object sender, EventArgs e)
        {
            newName.Text = oldName;
        }
    }
}
