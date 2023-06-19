using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace WindowsFormsApp2
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
           
        }
        
        int pointx = 128, pointy = 128;
        int dx, dy;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            pointx += dx;
            pointy += dy;
            if (pointy + 10 > 256)
            {
                dy = -dy;
            }
            if (pointy - 10 < 0)
            {
                dy = -dy;
            }
            if (pointx + 10 > 256)
            {
                dx = -dx;
            }
            if (pointx - 10 < 0)
            {
                dx = -dx;
            }
            pictureBox1.Refresh();
        }

        
        private void mousedoubleclick(object sensor, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left&&e.X>0&&e.Y>0&&e.X<256&&e.Y<256)
            {
                Console.WriteLine(e.Location);
                dx = e.X - pointx;
                dy = e.Y - pointy;
                timer1.Enabled = true;
            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            SolidBrush redBrush = new SolidBrush(Color.Red);
            e.Graphics.FillEllipse(redBrush, pointx, pointy, 20, 20);
        }
    }
}
