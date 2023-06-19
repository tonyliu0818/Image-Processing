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
    public partial class HighBoost : Form
    {
        public HighBoost()
        {
            InitializeComponent();
        }
        public Bitmap src_bitmap,outbitmap;
        public void func(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for(int y = 0; y < buffbitmap.Height; y++)
            {
                for(int x = 0; x < buffbitmap.Height; x++)
                {
                    int avg = (buffbitmap.GetPixel(x, y).R+ buffbitmap.GetPixel(x, y).G+ buffbitmap.GetPixel(x, y).B) / 3;
                    src_bitmap.SetPixel(x, y,Color.FromArgb(avg,avg,avg));
                }
            }
            pictureBox1.Image = src_bitmap;
            label2.Text = "High_boost";
            label1.Text = trackBar1.Minimum.ToString();
            //label3.Text = trackBar2.Minimum.ToString();
        }
        private void trackBar1_change(object sender, EventArgs e)
        {
            int mask= trackBar1.Value;
            double A = (double)trackBar2.Value / 10;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (mask % 2 == 0) return;
            int blank = mask / 2;
            int size = mask * mask;
            double ratio = (double)(mask * mask * A - 1);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c_ = src_bitmap.GetPixel(x, y);
                    int count = 0;
                    double sumr = 0;
                    double sumg = 0;
                    double sumb = 0;

                    for (int j = -blank; j <= blank; j++)
                    {
                        for (int i = -blank; i <= blank; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumr += 0;
                                sumg += 0;
                                sumb += 0;
                            }
                            else
                            {
                                Color c = src_bitmap.GetPixel(x + i, y + j);
                                if (i == 0 && j == 0)
                                {
                                    sumr += (c.R * ratio);
                                    sumg += (c.G * ratio);
                                    sumb += (c.B * ratio);
                                }
                                else
                                {
                                    sumr += (c.R * -1);
                                    sumg += (c.G * -1);
                                    sumb += (c.B * -1);
                                }
                            }
                            count++;
                        }
                    }
                    double buff1 = sumr / size;
                    double buff2 = sumg / size;
                    double buff3 = sumb / size;
                    if (buff3 > 255)
                    {
                        buff3 = 255;
                    }
                    else if (buff3 < 0)
                    {
                        buff3 = 0;
                    }
                    if (buff2 > 255)
                    {
                        buff2 = 255;
                    }
                    else if (buff2 < 0)
                    {
                        buff2 = 0;
                    }
                    if (buff1 > 255)
                    {
                        buff1 = 255;
                    }
                    else if (buff1 < 0)
                    {
                        buff1 = 0;
                    }
                    int pixelr = (int)(Math.Ceiling(buff1));
                    int pixelg = (int)(Math.Ceiling(buff2));
                    int pixelb = (int)(Math.Ceiling(buff3));
                    outbitmap.SetPixel(x, y, Color.FromArgb(pixelr, pixelg, pixelb));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
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
                toolStripStatusLabel24.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                toolStripStatusLabel24.Text = snr.ToString() + "db";
            }*/
            long outcount = 0;
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
            
            if (difference == 0)
            {
                toolStripStatusLabel24.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)10 * Math.Log10(outcount / difference);
                snr = Math.Round(snr, 2);
                toolStripStatusLabel24.Text = snr.ToString() + "db";
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
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar1.Value;
            label1.Text = t1.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar2.Value / 10;
            label3.Text = t1.ToString();
        }
    }
    

}
