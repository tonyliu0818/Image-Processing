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
    public partial class Shear : Form
    {
        public Shear()
        {
            InitializeComponent();
        }
        public Bitmap src_bitmap, outbitmap;
        string state;
        public void set(Bitmap buff, string state_, int max, int min)
        {
            trackBar1.Enabled = true;
            trackBar1.Maximum = max;
            trackBar1.Minimum = min;
            pictureBox1.Image = buff;
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buff.Height; y++)
            {
                for (int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            state = state_;
            //label2.Text = state_;
        }
        public void she(Bitmap bitmap1)
        {
            set(bitmap1, "shear", 10, 0);
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar1.Value / 10;
            label1.Text =t1.ToString();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar2.Value / 10;
            label2.Text = t1.ToString();
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (state == "shear")
            {
                double m = (double)trackBar1.Value/10;
                double n = (double)trackBar2.Value/10;
                int width = (int)Math.Round(src_bitmap.Width + (m) * src_bitmap.Height);
                int height = (int)Math.Round(src_bitmap.Height + (n) * src_bitmap.Width);
                outbitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //Bitmap outbitmap = new Bitmap(1000, 1000, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(240, 240, 240));
                    }
                }

                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        int x1 = (int)(x + m * y);
                        int y1 = (int)(n * x + y);
                        if (m > 0 && n == 0)
                        {
                            outbitmap.SetPixel(x1, src_bitmap.Height - y1 - 1, src_bitmap.GetPixel(x, src_bitmap.Height - 1 - y));
                        }
                        else if (m == 0 && n > 0)
                        {
                            outbitmap.SetPixel(src_bitmap.Width - 1 - x1, y1, src_bitmap.GetPixel(src_bitmap.Width - 1 - x, y));
                        }
                        else
                        {
                            outbitmap.SetPixel(x1, height-y1-1, src_bitmap.GetPixel(x,src_bitmap.Height- y-1));
                        }
                    }
                }

                pictureBox2.Image = outbitmap;
                snr_(src_bitmap, outbitmap);
            }
        }
        
        private void p1_MouseMove(object sender, MouseEventArgs e)
        {
            if (src_bitmap != null)
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel3.Text = e.X.ToString();
                    toolStripStatusLabel5.Text = e.Y.ToString();
                    toolStripStatusLabel7.Text = src_bitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel9.Text = src_bitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel11.Text = src_bitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                else
                {
                    
                    toolStripStatusLabel3.Text = "_";
                    toolStripStatusLabel5.Text = "_";
                    toolStripStatusLabel7.Text = "_";
                    toolStripStatusLabel9.Text = "_";
                    toolStripStatusLabel11.Text = "_";
                }
            }
        }
        private void out_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel14.Text = e.X.ToString();
                    toolStripStatusLabel16.Text = e.Y.ToString();
                    toolStripStatusLabel18.Text = outbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel20.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel22.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                else
                {
                    toolStripStatusLabel14.Text = "_";
                    toolStripStatusLabel16.Text = "_";
                    toolStripStatusLabel18.Text = "_";
                    toolStripStatusLabel20.Text = "_";
                    toolStripStatusLabel22.Text = "_";
                }
            }
        }
        public void snr_(Bitmap bitmap1, Bitmap bitmap2)
        {
            /*long inputcount = 0;
            long outcount = 0;
            for (int y = 0; y < bitmap1.Height; y++)
            {
                for (int x = 0; x < bitmap1.Width; x++)
                {
                    Color c = bitmap1.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    inputcount += (avg);
                }
            }
            for (int y = 0; y < bitmap2.Height; y++)
            {
                for (int x = 0; x < bitmap2.Width; x++)
                {
                    Color c = bitmap2.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    outcount += (avg);
                }
            }
            if (outcount == inputcount)
            {
                toolStripStatusLabel2.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                toolStripStatusLabel24.Text = snr.ToString() + "db";
            }*/
            /*long outcount = 0;
            long difference = 0;
            for (int y = 0; y < bitmap1.Height; y++)
            {
                for (int x = 0; x < bitmap1.Width; x++)
                {
                    Color c = bitmap1.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    Color c2 = bitmap2.GetPixel(x, y);
                    int avg2 = (c2.R + c2.G + c2.B) / 3;
                    outcount += (long)Math.Pow(avg2, 2);
                    difference += (long)Math.Pow(avg2 - avg, 2);
                }
            }
            double snr = (double)10 * Math.Log10(outcount / difference);
            snr = Math.Round(snr, 2);
            if (difference == 0)
            {
                toolStripStatusLabel24.Text = "與原圖相同";
            }
            else
            {
                toolStripStatusLabel24.Text = snr.ToString() + "db";
            }*/
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }
    }
}
