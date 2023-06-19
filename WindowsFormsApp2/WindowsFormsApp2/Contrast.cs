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
    public partial class Contrast : Form
    {
        public Contrast()
        {
            InitializeComponent();
            
        }
        Bitmap src_bitmap,xy_bitmap,outbitmap;
        bool draw = false;
        int p1 = 50, p2 = 50, p3 = 200, p4 = 200;
        public void drawxy_init(int r1,int s1,int r2,int s2)
        {
            xy_bitmap = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    xy_bitmap.SetPixel(x, y, Color.Bisque);
                }
            }
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (x == r1 && y == 255 - s1)
                    {
                        for (int j = -3; j <= 3; j++)
                        {
                            for (int i = -3; i <= 3; i++)
                            {
                                xy_bitmap.SetPixel(x + i, y + j, Color.Black);
                            }
                        }

                    }
                    else if (x == r2 && y == 255 - s2)
                    {
                        for (int j = -3; j <= 3; j++)
                        {
                            for (int i = -3; i <= 3; i++)
                            {
                                xy_bitmap.SetPixel(x + i, y + j, Color.DarkCyan);
                            }
                        }
                    }
                    else if (x == 255 - y)
                    {
                        xy_bitmap.SetPixel(x, y, Color.Red);
                    }
                }
            }
            toolStripStatusLabel2.Text = r1.ToString();
            toolStripStatusLabel4.Text = s1.ToString();
            toolStripStatusLabel6.Text = r2.ToString();
            toolStripStatusLabel8.Text = s2.ToString();
            pictureBox3.Image = xy_bitmap;
        }
        public void drawxy(int r1, int s1, int r2, int s2)
        {
            int[] pixel = new int[256];
            int[] outpixel = new int[256];
            for (int i = 0; i < 256; i++)
            {
                pixel[i] = i;
            }
            for(int i = 0; i < 256; i++)
            {
                if (i <= r1)
                {
                    outpixel[i] = (i * s1 / r1) ;
                }
                else if (i >= r2)
                {
                    outpixel[i] = ((i - r2)* (255 - s2) / (255 - r2))  + s2;
                }
                else if (i < r2 && i > r1)
                {
                    outpixel[i] = ((i - r1) * (s2 - s1) / (r2 - r1)) + s1;
                }
            }
            xy_bitmap = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    xy_bitmap.SetPixel(x, y, Color.Bisque);
                }
            }
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (x == r1 && y == 255 - s1)
                    {
                        for (int j = -3; j <= 3; j++)
                        {
                            for (int i = -3; i <= 3; i++)
                            {
                                if (x + i > 0 && x + i < xy_bitmap.Width - 1 && y + j > 0 && y + j < xy_bitmap.Height - 1)
                                {
                                    xy_bitmap.SetPixel(x + i, y + j, Color.Black);
                                }
                            }
                        }

                    }
                    else if (x == r2 && y == 255 - s2)
                    {
                        for (int j = -3; j <= 3; j++)
                        {
                            for (int i = -3; i <= 3; i++)
                            {
                                if (x + i > 0 && x + i < xy_bitmap.Width - 1 && y + j > 0 && y + j < xy_bitmap.Height - 1)
                                {
                                    xy_bitmap.SetPixel(x + i, y + j, Color.DarkCyan);
                                }
                            }
                        }
                    }
                    else if (x == 255 - y)
                    {
                        xy_bitmap.SetPixel(pixel[x], (int)(255-outpixel[255-y]), Color.Red);
                    }
                }
            }
            pictureBox3.Image = xy_bitmap;
        }
        public void pictureout(int r1, int s1, int r2, int s2)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    double buff = 0;
                    byte value = src_bitmap.GetPixel(x, y).R;
                    if (value <= r1)
                    {
                        buff = (value* s1 / r1) ;
                    }
                    else if (value >= r2)
                    {
                        buff = ((value - r2)* (255 - s2) / (255 - r2))  + s2;
                    }
                    else if (value < r2 && value > r1)
                    {
                        buff = ((value - r1)* (s2 - s1) / (r2 - r1))  + s1;
                    }
                    outbitmap.SetPixel(x, y, Color.FromArgb((int)buff, (int)buff, (int)buff));
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
                    toolStripStatusLabel12.Text = e.X.ToString();
                    toolStripStatusLabel14.Text = e.Y.ToString();
                    toolStripStatusLabel16.Text = src_bitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel18.Text = src_bitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel20.Text = src_bitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                else
                {
                    toolStripStatusLabel12.Text = "_";
                    toolStripStatusLabel14.Text = "_";
                    toolStripStatusLabel16.Text = "_";
                    toolStripStatusLabel18.Text = "_";
                    toolStripStatusLabel20.Text = "_";
                }
            }
        }
        private void out_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel23.Text = e.X.ToString();
                    toolStripStatusLabel25.Text = e.Y.ToString();
                    toolStripStatusLabel27.Text = outbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel29.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel31.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                else
                {
                    toolStripStatusLabel23.Text = "_";
                    toolStripStatusLabel25.Text = "_";
                    toolStripStatusLabel27.Text = "_";
                    toolStripStatusLabel29.Text = "_";
                    toolStripStatusLabel31.Text = "_";
                }
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }



        public void snr_(Bitmap bitmap1, Bitmap bitmap2)
        {
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
                toolStripStatusLabel33.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)10 * Math.Log10(outcount / difference);
                snr = Math.Round(snr, 2);
                toolStripStatusLabel33.Text = snr.ToString() + "db";
            }
        }
        public void c(Bitmap bitmap)
        {
            //label2.Text = "contrast";
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, bitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            drawxy_init(p1, p2, p3, p4);
        }
        
        private void mousedown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draw = true;
                Invalidate();
            }
        }
        private void mouseup(object sender, MouseEventArgs e)
        {
            draw = false;
           
            int pointx = e.X;
            int pointy = 255 - e.Y;
            if (e.X < 125)
            {
                p1 = pointx;
                p2 = pointy;
                toolStripStatusLabel9.Text = "你現在移動的是r1和s1";
            }
            else
            {
                p3 = pointx;
                p4 = pointy;
                toolStripStatusLabel9.Text = "你現在移動的是r2和s2";
            }
            toolStripStatusLabel2.Text = (p1).ToString();
            toolStripStatusLabel4.Text = (p2).ToString();
            toolStripStatusLabel6.Text = (p3).ToString();
            toolStripStatusLabel8.Text = (p4).ToString();
            drawxy(p1, p2, p3, p4);
            pictureout(p1, p2, p3, p4);
        }
    }
}
