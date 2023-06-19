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
    public partial class lowpass : Form
    {
        public lowpass()
        {
            InitializeComponent();
        }
        Bitmap src_bitmap, noise_bitmap, outbitmap, noiseout_bitmap;
        public int MaxMin(int[] array)
        {
            int min1 = Math.Min(Math.Min(array[0], array[1]), array[2]);
            int min2 = Math.Min(Math.Min(array[0], array[1]), array[3]);
            int min3 = Math.Min(Math.Min(array[0], array[1]), array[4]);
            int min4 = Math.Min(Math.Min(array[0], array[2]), array[3]);
            int min5 = Math.Min(Math.Min(array[0], array[2]), array[4]);
            int min6 = Math.Min(Math.Min(array[0], array[3]), array[4]);
            int min7 = Math.Min(Math.Min(array[1], array[2]), array[3]);
            int min8 = Math.Min(Math.Min(array[1], array[2]), array[4]);
            int min9 = Math.Min(Math.Min(array[1], array[3]), array[4]);
            int min10 = Math.Min(Math.Min(array[2], array[3]), array[4]);
            int max1 = Math.Max(min1, min2);
            int max2 = Math.Max(min3, min4);
            int max3 = Math.Max(min5, min6);
            int max4 = Math.Max(min7, min8);
            int max5 = Math.Max(min9, min10);
            int max6 = Math.Max(max1, max2);
            int max7 = Math.Max(Math.Max(max3, max4), max5);
            int max = Math.Max(max6, max7);
            return max;
        }
        public int MinMax(int[] array)
        {
            int max1 = Math.Min(Math.Min(array[0], array[1]), array[2]);
            int max2 = Math.Min(Math.Min(array[0], array[1]), array[3]);
            int max3 = Math.Min(Math.Min(array[0], array[1]), array[4]);
            int max4 = Math.Min(Math.Min(array[0], array[2]), array[3]);
            int max5 = Math.Min(Math.Min(array[0], array[2]), array[4]);
            int max6 = Math.Min(Math.Min(array[0], array[3]), array[4]);
            int max7 = Math.Min(Math.Min(array[1], array[2]), array[3]);
            int max8 = Math.Min(Math.Min(array[1], array[2]), array[4]);
            int max9 = Math.Min(Math.Min(array[1], array[3]), array[4]);
            int max10 = Math.Min(Math.Min(array[2], array[3]), array[4]);
            int min1 = Math.Max(max1, max2);
            int min2 = Math.Max(max3, max4);
            int min3 = Math.Max(max5, max6);
            int min4 = Math.Max(max7, max8);
            int min5 = Math.Max(max9, max10);
            int min6 = Math.Max(min1, min2);
            int min7 = Math.Max(Math.Max(min3, min4), min5);
            int min = Math.Max(min6, min7);
            return min;
        }
        public void average(Bitmap bitmap)
        {
            var rand = new Random();
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noise_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    src_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    int num = rand.Next(101);
                    int out2;
                    if (num >= 0 && num < 5)
                    {
                        out2 = 0;
                    }
                    else if (num >= 5 && num < 10)
                    {
                        out2 = 255;
                    }
                    else
                    {
                        out2 = avg;
                    }
                    noise_bitmap.SetPixel(x, y, Color.FromArgb(out2, out2, out2));
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox3.Image = noise_bitmap;
            label5.Text = "average";
            label6.Text = "Mask:" + trackBar1.Minimum.ToString();
            snr_(src_bitmap, noise_bitmap,2);
        }
        public void square(Bitmap bitmap)
        {
            var rand = new Random();
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noise_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    src_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    int num = rand.Next(101);
                    int out2;
                    if (num >= 0 && num < 5)
                    {
                        out2 = 0;
                    }
                    else if (num >= 5 && num < 10)
                    {
                        out2 = 255;
                    }
                    else
                    {
                        out2 = avg;
                    }
                    noise_bitmap.SetPixel(x, y, Color.FromArgb(out2, out2, out2));
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox3.Image = noise_bitmap;
            label5.Text = "median_square";
            label6.Text = trackBar1.Minimum.ToString();
            snr_(src_bitmap, noise_bitmap, 2);
        }
        public void outlier(Bitmap bitmap)
        {
            var rand = new Random();
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noise_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    src_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    int num = rand.Next(101);
                    int out2;
                    if (num >= 0 && num < 5)
                    {
                        out2 = 0;
                    }
                    else if (num >= 5 && num < 10)
                    {
                        out2 = 255;
                    }
                    else
                    {
                        out2 = avg;
                    }
                    noise_bitmap.SetPixel(x, y, Color.FromArgb(out2, out2, out2));
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox3.Image = noise_bitmap;
            label5.Text = "outlier";
            label6.Text = "Mask:" + trackBar1.Minimum.ToString();
            trackBar2.Visible = true;
            label7.Text = "threshold:" + trackBar2.Minimum.ToString();
            label7.Visible = true;
            snr_(src_bitmap, noise_bitmap, 2);
        }
        public void pseido(Bitmap bitmap)
        {
            var rand = new Random();
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noise_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {

                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    src_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    int num = rand.Next(101);
                    int out2;
                    if (num >= 0 && num < 5)
                    {
                        out2 = 0;
                    }
                    else if (num >= 5 && num < 10)
                    {
                        out2 = 255;
                    }
                    else
                    {
                        out2 = avg;
                    }
                    noise_bitmap.SetPixel(x, y, Color.FromArgb(out2, out2, out2));
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox3.Image = noise_bitmap;
            snr_(src_bitmap, noise_bitmap, 2);
            label5.Text = "pseudo";
            label6.Text = "Mask:"+3.ToString();
            trackBar1.Enabled = false;
            int mask = 3;
            int blank = mask / 2;
            int masksize = mask + mask - 1;
            //int masksize = mask * mask;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noiseout_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    int count = 0;
                    int[] rcontent = new int[masksize];
                    int[] gcontent = new int[masksize];
                    int[] bcontent = new int[masksize];
                    int[] r1content = new int[masksize];
                    int[] g1content = new int[masksize];
                    int[] b1content = new int[masksize];
                    for (int j = -blank; j <= blank; j++)
                    {
                        for (int i = -blank; i <= blank; i++)
                        {
                            if (i == 0 || j == 0)
                            {
                                if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                                {
                                    rcontent[count] = 0;
                                    gcontent[count] = 0;
                                    bcontent[count] = 0;
                                    r1content[count] = 0;
                                    g1content[count] = 0;
                                    b1content[count] = 0;
                                }
                                else
                                {
                                    Color c = src_bitmap.GetPixel(x + i, y + j);
                                    rcontent[count] = c.R;
                                    gcontent[count] = c.G;
                                    bcontent[count] = c.B;
                                    Color c1 = noise_bitmap.GetPixel(x + i, y + j);
                                    r1content[count] = c1.R;
                                    g1content[count] = c1.G;
                                    b1content[count] = c1.B;


                                }
                                count++;
                            }
                        }
                    }
                    int maxr = MaxMin(rcontent);
                    int minr = MinMax(rcontent);
                    int maxg = MaxMin(gcontent);
                    int ming = MinMax(gcontent);
                    int maxb = MaxMin(bcontent);
                    int minb = MinMax(bcontent);
                    Byte rout = (Byte)((maxr + minr) / 2);
                    Byte gout = (Byte)((maxg + ming) / 2);
                    Byte bout = (Byte)((maxb + minb) / 2);
                    outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    maxr = MaxMin(r1content);
                    minr = MinMax(r1content);
                    maxg = MaxMin(g1content);
                    ming = MinMax(g1content);
                    maxb = MaxMin(b1content);
                    minb = MinMax(b1content);
                    rout = (Byte)((maxr + minr) / 2);
                    gout = (Byte)((maxg + ming) / 2);
                    bout = (Byte)((maxb + minb) / 2);
                    noiseout_bitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                }
            }
            pictureBox2.Image = outbitmap;
            pictureBox4.Image = noiseout_bitmap;
            trackBar1.Enabled = false;
            snr_(src_bitmap, outbitmap, 0);
            snr_(noise_bitmap, noiseout_bitmap, 1);
        }
        public void cross(Bitmap bitmap)
        {
            var rand = new Random();
            src_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            noise_bitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for(int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    src_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    int num = rand.Next(101);
                    int out2;
                    if (num >= 0 && num < 5)
                    {
                        out2 = 0;
                    }
                    else if (num >= 5 && num < 10)
                    {
                        out2 = 255;
                    }
                    else
                    {
                        out2 = avg;
                    }
                    noise_bitmap.SetPixel(x, y, Color.FromArgb(out2, out2, out2));
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox3.Image = noise_bitmap;
            label5.Text = "median_cross";
            label6.Text = trackBar1.Minimum.ToString();
            snr_(src_bitmap, noise_bitmap, 2);
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

            }

        }
        private void P2_MouseMove(object sender, MouseEventArgs e)
        {
            if (noise_bitmap != null)
            {
                if ((e.X < noise_bitmap.Width) && (e.Y < noise_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel14.Text = e.X.ToString();
                    toolStripStatusLabel16.Text = e.Y.ToString();
                    toolStripStatusLabel18.Text = noise_bitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel20.Text = noise_bitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel22.Text = noise_bitmap.GetPixel(e.X, e.Y).B.ToString();
                }

            }
        }
        private void p3_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel27.Text = e.X.ToString();
                    toolStripStatusLabel29.Text = e.Y.ToString();
                    toolStripStatusLabel31.Text = outbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel33.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel35.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
            }

        }
        private void out_MouseMove(object sender, MouseEventArgs e)
        {
            if (noiseout_bitmap != null)
            {
                if ((e.X < noiseout_bitmap.Width) && (e.Y < noiseout_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel40.Text = e.X.ToString();
                    toolStripStatusLabel42.Text = e.Y.ToString();
                    toolStripStatusLabel44.Text = noiseout_bitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel46.Text = noiseout_bitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel48.Text = noiseout_bitmap.GetPixel(e.X, e.Y).B.ToString();
                }

            }
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if(label5.Text== "median_cross")
            {
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                int masksize = mask + mask - 1;
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                noiseout_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        int count = 0;
                        int[] rcontent = new int[masksize];
                        int[] gcontent = new int[masksize];
                        int[] bcontent = new int[masksize];
                        int[] r1content = new int[masksize];
                        int[] g1content = new int[masksize];
                        int[] b1content = new int[masksize];
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (i == 0 || j == 0)
                                {
                                    if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                                    {
                                        rcontent[count] = 0;
                                        gcontent[count] = 0;
                                        bcontent[count] = 0;
                                        r1content[count] = 0;
                                        g1content[count] = 0;
                                        b1content[count] = 0;
                                    }
                                    else
                                    {
                                        Color c = src_bitmap.GetPixel(x + i, y + j);
                                        rcontent[count] = c.R;
                                        gcontent[count] = c.G;
                                        bcontent[count] = c.B;
                                        Color c1 = noise_bitmap.GetPixel(x + i, y + j);
                                        r1content[count] = c1.R;
                                        g1content[count] = c1.G;
                                        b1content[count] = c1.B;


                                    }
                                    count++;
                                }
                            }
                        }
                        int temp_r, temp_g, temp_b;
                        for (int i = masksize; i > 1; i--)
                        {
                            for (int j = 0; j < i - 1; j++)
                            {
                                if (rcontent[j] > rcontent[j + 1])
                                {
                                    temp_r = rcontent[j];
                                    rcontent[j] = rcontent[j + 1];
                                    rcontent[j + 1] = temp_r;
                                }
                                if (gcontent[j] > gcontent[j + 1])
                                {
                                    temp_g = gcontent[j];
                                    gcontent[j] = gcontent[j + 1];
                                    gcontent[j + 1] = temp_g;
                                }
                                if (bcontent[j] > bcontent[j + 1])
                                {
                                    temp_b = bcontent[j];
                                    bcontent[j] = bcontent[j + 1];
                                    bcontent[j + 1] = temp_b;
                                }
                            }
                        }
                        int temp_r1, temp_g1, temp_b1;
                        for (int i = masksize; i > 1; i--)
                        {
                            for (int j = 0; j < i - 1; j++)
                            {
                                if (r1content[j] > r1content[j + 1])
                                {
                                    temp_r1 = r1content[j];
                                    r1content[j] = r1content[j + 1];
                                    r1content[j + 1] = temp_r1;
                                }
                                if (g1content[j] > g1content[j + 1])
                                {
                                    temp_g1 = g1content[j];
                                    g1content[j] = g1content[j + 1];
                                    g1content[j + 1] = temp_g1;
                                }
                                if (b1content[j] > b1content[j + 1])
                                {
                                    temp_b1 = b1content[j];
                                    b1content[j] = b1content[j + 1];
                                    b1content[j + 1] = temp_b1;
                                }
                            }
                        }
                        Byte rout = (Byte)(rcontent[masksize / 2]);
                        Byte gout = (Byte)(gcontent[masksize / 2]);
                        Byte bout = (Byte)(bcontent[masksize / 2]);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                        Byte r1out = (Byte)(r1content[masksize / 2]);
                        Byte g1out = (Byte)(g1content[masksize / 2]);
                        Byte b1out = (Byte)(b1content[masksize / 2]);
                        noiseout_bitmap.SetPixel(x, y, Color.FromArgb(r1out, g1out, b1out));
                    }
                }
                pictureBox2.Image = outbitmap;
                pictureBox4.Image = noiseout_bitmap;
                snr_(src_bitmap, outbitmap, 0);
                snr_(noise_bitmap, noiseout_bitmap, 1);
            }
            else if(label5.Text== "median_square")
            {
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                int masksize = mask * mask;
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                noiseout_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        int count = 0;
                        int[] rcontent = new int[masksize];
                        int[] gcontent = new int[masksize];
                        int[] bcontent = new int[masksize];
                        int[] r1content = new int[masksize];
                        int[] g1content = new int[masksize];
                        int[] b1content = new int[masksize];
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                                {
                                    rcontent[count] = 0;
                                    gcontent[count] = 0;
                                    bcontent[count] = 0;
                                    r1content[count] = 0;
                                    g1content[count] = 0;
                                    b1content[count] = 0;
                                }
                                else
                                {
                                    Color c = src_bitmap.GetPixel(x + i, y + j);
                                    rcontent[count] = c.R;
                                    gcontent[count] = c.G;
                                    bcontent[count] = c.B;
                                    Color c1 = noise_bitmap.GetPixel(x + i, y + j);
                                    r1content[count] = c1.R;
                                    g1content[count] = c1.G;
                                    b1content[count] = c1.B;


                                }
                                count++;
                            }
                        }
                        int temp_r, temp_g, temp_b;
                        for (int i = masksize; i > 1; i--)
                        {
                            for (int j = 0; j < i - 1; j++)
                            {
                                if (rcontent[j] > rcontent[j + 1])
                                {
                                    temp_r = rcontent[j];
                                    rcontent[j] = rcontent[j + 1];
                                    rcontent[j + 1] = temp_r;
                                }
                                if (gcontent[j] > gcontent[j + 1])
                                {
                                    temp_g = gcontent[j];
                                    gcontent[j] = gcontent[j + 1];
                                    gcontent[j + 1] = temp_g;
                                }
                                if (bcontent[j] > bcontent[j + 1])
                                {
                                    temp_b = bcontent[j];
                                    bcontent[j] = bcontent[j + 1];
                                    bcontent[j + 1] = temp_b;
                                }
                            }
                        }
                        int temp_r1, temp_g1, temp_b1;
                        for (int i = masksize; i > 1; i--)
                        {
                            for (int j = 0; j < i - 1; j++)
                            {
                                if (r1content[j] > r1content[j + 1])
                                {
                                    temp_r1 = r1content[j];
                                    r1content[j] = r1content[j + 1];
                                    r1content[j + 1] = temp_r1;
                                }
                                if (g1content[j] > g1content[j + 1])
                                {
                                    temp_g1 = g1content[j];
                                    g1content[j] = g1content[j + 1];
                                    g1content[j + 1] = temp_g1;
                                }
                                if (b1content[j] > b1content[j + 1])
                                {
                                    temp_b1 = b1content[j];
                                    b1content[j] = b1content[j + 1];
                                    b1content[j + 1] = temp_b1;
                                }
                            }
                        }
                        Byte rout = (Byte)(rcontent[masksize / 2]);
                        Byte gout = (Byte)(gcontent[masksize / 2]);
                        Byte bout = (Byte)(bcontent[masksize / 2]);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                        Byte r1out = (Byte)(r1content[masksize / 2]);
                        Byte g1out = (Byte)(g1content[masksize / 2]);
                        Byte b1out = (Byte)(b1content[masksize / 2]);
                        noiseout_bitmap.SetPixel(x, y, Color.FromArgb(r1out, g1out, b1out));
                    }
                }
                pictureBox2.Image = outbitmap;
                pictureBox4.Image = noiseout_bitmap;
                snr_(src_bitmap, outbitmap, 0);
                snr_(noise_bitmap, noiseout_bitmap, 1);
            }
            else if(label5.Text== "average")
            {
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                noiseout_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        int count = 0;
                        int sumr = 0;
                        int sumg = 0;
                        int sumb = 0;
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
                                    sumr += c.R;
                                    sumg += c.G;
                                    sumb += c.B;
                                }
                                count++;
                            }
                        }
                        Byte rout = (Byte)(sumr / count);
                        Byte gout = (Byte)(sumg / count);
                        Byte bout = (Byte)(sumb / count);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                        count = 0;
                        sumr = 0;
                        sumg = 0;
                        sumb = 0;
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (x + i < 0 || y + j < 0 || x + i > noise_bitmap.Width - 1 || y + j > noise_bitmap.Height - 1)
                                {
                                    sumr += 0;
                                    sumg += 0;
                                    sumb += 0;
                                }
                                else
                                {
                                    Color c = noise_bitmap.GetPixel(x + i, y + j);
                                    sumr += c.R;
                                    sumg += c.G;
                                    sumb += c.B;
                                }
                                count++;
                            }
                        }
                        rout = (Byte)(sumr / count);
                        gout = (Byte)(sumg / count);
                        bout = (Byte)(sumb / count);
                        noiseout_bitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    }
                }
                pictureBox2.Image = outbitmap;
                pictureBox4.Image = noiseout_bitmap;
                snr_(src_bitmap, outbitmap, 0);
                snr_(noise_bitmap, noiseout_bitmap, 1);
            }
            else if (label5.Text == "outlier")
            {
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                noiseout_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                int threshold = trackBar2.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                int[] mask_ = new int[8];
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        int count = 0;
                        int sumr = 0;
                        int sumg = 0;
                        int sumb = 0;
                        int centerr=0, centerg=0, centerb=0;
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (i == 0 && j == 0)
                                {
                                    centerr = src_bitmap.GetPixel(x, y).R;
                                    centerg = src_bitmap.GetPixel(x, y).G;
                                    centerb = src_bitmap.GetPixel(x, y).B;

                                }
                                else
                                {
                                    if (x + i > 0 && y + j > 0 && x + i < src_bitmap.Width && y + j < src_bitmap.Height)
                                    {
                                        Color c = src_bitmap.GetPixel(x + i, y + j);
                                        sumr += c.R;
                                        sumg += c.G;
                                        sumb += c.B;
                                        mask_[count] = (c.R + c.G + c.B) / 3;
                                    }
                                    else
                                    {
                                        sumr += 0;
                                        sumg += 0;
                                        sumb += 0;
                                    }
                                    count++;
                                }

                            }
                        }
                        //Console.WriteLine(count);
                        if (count == 0) return;
                        Byte rout = (Byte)(sumr / count);
                        Byte gout = (Byte)(sumg / count);
                        Byte bout = (Byte)(sumb / count);
                        Byte out1;
                        Byte out2;
                        Byte out3;
                        if (Math.Abs(rout - centerr) > threshold)
                        {
                            out1 = rout;
                        }
                        else
                        {
                            out1 = (Byte)centerr;
                        }
                        if (Math.Abs(gout - centerg) > threshold)
                        {
                            out2 = gout;
                        }
                        else
                        {
                            out2 = (Byte)centerg;
                        }
                        if (Math.Abs(bout - centerb) > threshold)
                        {
                            out3 = bout;
                        }
                        else
                        {
                            out3 = (Byte)centerb;
                        }
                        outbitmap.SetPixel(x, y, Color.FromArgb(out1, out2, out3));
                        count = 0;
                        sumr = 0;
                        sumg = 0;
                        sumb = 0;
                        centerr = 0; centerg = 0; centerb = 0;
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (i == 0 && j == 0)
                                {
                                    centerr = noise_bitmap.GetPixel(x, y).R;
                                    centerg = noise_bitmap.GetPixel(x, y).G;
                                    centerb = noise_bitmap.GetPixel(x, y).B;

                                }
                                else
                                {
                                    if (x + i > 0 && y + j > 0 && x + i < noise_bitmap.Width && y + j < noise_bitmap.Height)
                                    {
                                        Color c = noise_bitmap.GetPixel(x + i, y + j);
                                        sumr += c.R;
                                        sumg += c.G;
                                        sumb += c.B;
                                        mask_[count] = (c.R + c.G + c.B) / 3;
                                    }
                                    else
                                    {
                                        sumr += 0;
                                        sumg += 0;
                                        sumb += 0;
                                    }
                                    count++;
                                }
                            }
                        }
                        rout = (Byte)(sumr / count);
                        gout = (Byte)(sumg / count);
                        bout = (Byte)(sumb / count);
                        if (Math.Abs(rout - centerr) > threshold)
                        {
                            out1 = rout;
                        }
                        else
                        {
                            out1 = (Byte)centerr;
                        }
                        if (Math.Abs(gout - centerg) > threshold)
                        {
                            out2 = gout;
                        }
                        else
                        {
                            out2 = (Byte)centerg;
                        }
                        if (Math.Abs(bout - centerb) > threshold)
                        {
                            out3 = bout;
                        }
                        else
                        {
                            out3 = (Byte)centerb;
                        }
                        noiseout_bitmap.SetPixel(x, y, Color.FromArgb(out1, out2, out3));
                    }
                }
                pictureBox2.Image = outbitmap;
                pictureBox4.Image = noiseout_bitmap;
                snr_(src_bitmap, outbitmap, 0);
                snr_(noise_bitmap, noiseout_bitmap, 1);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
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

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(noiseout_bitmap);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar1.Value;
            label6.Text = "Mask:" + t1.ToString();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar2.Value;
            label7.Text = "threshold:" + t1.ToString();
        }
        public void snr_(Bitmap bitmap1, Bitmap bitmap2,int state)
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
            /*for (int y = 0; y < bitmap2.Height; y++)
            {
                for (int x = 0; x < bitmap2.Width; x++)
                {
                    Color c = bitmap2.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    outcount += (avg);
                }
            }*/
            
            if (state == 0)
            {
                if (difference == 0)
                {
                    toolStripStatusLabel49.Text = "與原圖相同";
                }
                else
                {
                    double snr = (double)10 * Math.Log10(outcount / difference);
                    snr = Math.Round(snr, 2);
                    toolStripStatusLabel49.Text = snr.ToString() + "db";
                }
            }
            else if (state ==2)
            {
                if (difference == 0)
                {
                    toolStripStatusLabel37.Text = "與原圖相同";
                }
                else
                {
                    double snr = (double)10 * Math.Log10(outcount / difference);
                    snr = Math.Round(snr, 2);
                    toolStripStatusLabel37.Text = snr.ToString() + "db";
                }
            }
            else
            {
                if (difference == 0)
                {
                    toolStripStatusLabel50.Text = "與原圖相同";
                }
                else
                {
                    double snr = (double)10 * Math.Log10(outcount / difference);
                    snr = Math.Round(snr, 2);
                    toolStripStatusLabel50.Text = snr.ToString() + "db";
                }
            }
        }
    }
}
