using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conquer_Online_Server.NotForPublicNotAtAll
{
    public partial class NoCrash : Form
    {
        public NoCrash()
        {
            Visible = false;
            InitializeComponent();
        }

        private void NoCrash_Load(object sender, EventArgs e)
        {
            Hide();
            Program.StartEngine();
        }
    }
}
