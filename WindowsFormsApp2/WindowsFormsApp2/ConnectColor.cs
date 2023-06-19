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
    public partial class ConnectColor : Form
    {
        public ConnectColor()
        {
            InitializeComponent();
        }
        Bitmap outbitmap;
        public void put()
        {
            int size = 50;
            outbitmap = new Bitmap(size * 11, size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int index = 0;
            for (int y = 0; y <size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Red);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Orange);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Yellow);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Green);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Blue);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Cyan);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Purple);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Chocolate);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.LightSeaGreen);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Gold);
                }
            }
            index++;
            for (int y = 0; y < size; y++)
            {
                for (int x = size * index; x < size * (index + 1); x++)
                {
                    outbitmap.SetPixel(x, y, Color.Salmon);
                }
            }
            pictureBox1.Image = outbitmap;
        }
        private void p1_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel3.Text = e.X.ToString();
                    toolStripStatusLabel5.Text = e.Y.ToString();
                    toolStripStatusLabel7.Text = outbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel9.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel11.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                } 
            }
        }
    }
}
