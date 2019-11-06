using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Editor
{
    public partial class Snowman : Form
    {
        public Snowman()
        {
            InitializeComponent();
        }

        private void Snowman_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
