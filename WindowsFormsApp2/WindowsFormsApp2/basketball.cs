using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class basketball : Form
    {
        public basketball()
        {
            InitializeComponent();
        }
        int Xstep = 5;
        int Ystep = 5;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((pictureBox1.Location.X + pictureBox1.Width) > this.ClientSize.Width || pictureBox1.Location.X < 0) Xstep = 0 - Xstep;
            if ((pictureBox1.Location.Y + pictureBox1.Height) > this.ClientSize.Height || pictureBox1.Location.Y < 0) Ystep = 0 - Ystep;
            pictureBox1.Location = new Point(pictureBox1.Location.X + Xstep, pictureBox1.Location.Y + Ystep);
        }
    }
}
