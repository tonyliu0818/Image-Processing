using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Drawing2D;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class ManualThreshold : Form
    {
        public ManualThreshold()
        {
            InitializeComponent();
        }
       
        Bitmap buff,buff2,outbitmap;
        string state;
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
                toolStripStatusLabel35.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                toolStripStatusLabel35.Text = snr.ToString() + "db";
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
           
            if (difference == 0)
            {
                toolStripStatusLabel35.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)10 * Math.Log10(outcount / difference);
                snr = Math.Round(snr, 2);
                toolStripStatusLabel35.Text = snr.ToString() + "db";
            }*/
        }
        public void set(Bitmap src_bitmap,string state_,int max,int min)
        {
            pictureBox3.Image = null;
            trackBar1.Enabled = true;
            trackBar1.Maximum = max;
            trackBar1.Minimum = min;
            pictureBox1.Image = src_bitmap;
            buff = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    buff.SetPixel(x, y, src_bitmap.GetPixel(x, y));
                }
            }
            state = state_;
            label2.Text = state_;
            if (state_ == "PostiveRotation" || state_ == "InverseRotation")
            {
                label1.Text = 0.ToString();
            }
            else
            {
                label1.Text = trackBar1.Minimum.ToString();
            }
        }
        public void dump(Bitmap src_bitmap)
        {
            set(src_bitmap, "dump", 5, 1);
        }
        public void outlier(Bitmap src_bitmap)
        {
            set(src_bitmap, "outlier", 11, 1);
        }
        public void average_(Bitmap src_bitmap)
        {
            set(src_bitmap, "Average_fifter", 11, 1);
        }
        public void median(Bitmap src_bitmap)
        {
            set(src_bitmap, "median", 11, 1);
        }
        public void highpass(Bitmap src_bitmap)
        {
            set(src_bitmap, "highpass", 11, 1);
            buff = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    buff.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            pictureBox1.Image = buff;
        }
        public void pseudo(Bitmap src_bitmap)
        {
            int mask = 3;
            int blank = mask / 2;
            int masksize = mask + mask - 1;
            //int masksize = mask * mask;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c_ = src_bitmap.GetPixel(x, y);
                    int count = 0;
                    int[] rcontent = new int[masksize];
                    int[] gcontent = new int[masksize];
                    int[] bcontent = new int[masksize];
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
                                }
                                else
                                {
                                    Color c = src_bitmap.GetPixel(x + i, y + j);
                                    rcontent[count] = c.R;
                                    gcontent[count] = c.G;
                                    bcontent[count] = c.B;

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
                }
            }
            pictureBox1.Image = src_bitmap;
            pictureBox2.Image = outbitmap;
            label1.Text = "3";
            label2.Text = "pseudo median";
            trackBar1.Enabled = false;
            snr_(src_bitmap, outbitmap);
        }
        public void median_(Bitmap src_bitmap)
        {
            set(src_bitmap, "median_nn", 11, 1);
        }
        public void threshold(Bitmap src_bitmap)
        {
            set(src_bitmap, "manual",255,1);
        }
        public void gamma(Bitmap src_bitmap)
        {
            set(src_bitmap, "gamma", 1000, 1);
        }
        
        public void linear(Bitmap src_bitmap)
        {
            set(src_bitmap, "linear",5,1);
        }
        public void crispening(Bitmap src_bitmap)
        {
            set(src_bitmap, "crispening", 11, 1);
            buff = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    buff.SetPixel(x, y, Color.FromArgb(avg,avg,avg));
                }
            }
            pictureBox1.Image = buff;
        }
        public void average(Bitmap src_bitmap)
        {
            set(src_bitmap, "average",256,1);
        }
        public void first(Bitmap src_bitmap)
        {
            set(src_bitmap, "first",256,1);
        }
        public void otsu(Bitmap src_bitmap)
        {
            label2.Text = "otsu";
            trackBar1.Enabled = false;
            trackBar1.Maximum = 255;
            trackBar1.Minimum = 0;
            pictureBox1.Image = src_bitmap;
            buff = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            int count = 0;
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    histogram[avg]++;
                    buff.SetPixel(x, y, c);
                    count++;
                }
            }
            double min = double.MaxValue;
            int refrence = 0;
            double[] www = new double[256];
            for (int threshold = 0; threshold < 256; threshold++)
            {
                //left side
                double lefttotal = 0;
                double lefthistogramnum = 0;
                double q1;
                for (int i = 0; i < threshold; i++)
                {
                    lefttotal += histogram[i] * i;
                    lefthistogramnum += histogram[i];
                }
                double leftavg = (lefthistogramnum == 0) ? 0 : lefttotal / lefthistogramnum;
                q1 = lefthistogramnum / (src_bitmap.Width * src_bitmap.Height);
                //right side
                int righttotal = 0;
                double righthistogramnum = 0;
                double q2;
                for (int i = threshold; i < 256; i++)
                {
                    righttotal += histogram[i] * i;
                    righthistogramnum += histogram[i];
                }
                double rightavg = (lefthistogramnum == 0) ? 0 : righttotal / lefthistogramnum;
                q2 = righthistogramnum / (src_bitmap.Width * src_bitmap.Height);
                double buff1 = 0;
                for (int i = 0; i < threshold; i++)
                {
                    buff1 += Math.Pow(i - leftavg, 2) * (histogram[i]);
                }
                double leftsd = (lefthistogramnum == 0) ? 0 : buff1 / lefthistogramnum;
                double buff2 = 0;
                for (int i = threshold; i < 256; i++)
                {
                    buff2 += Math.Pow(i - rightavg, 2) * (histogram[i]);
                }
                double rightsd = (righthistogramnum == 0) ? 0 : buff2 / righthistogramnum;
                double wsd = (q1 * leftsd + q2 * rightsd);
                if (min > wsd)
                {

                    min = wsd;
                    refrence = threshold;
                }
            }
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    if (avg < refrence)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }
            label1.Text = refrence.ToString();
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }
        public void depth(Bitmap src_bitmap)
        {
            set(src_bitmap, "depth",20,2);
        }
        public void rotation1(Bitmap src_bitmap)
        {
            set(src_bitmap, "PostiveRotation", 360, -360);
        }
        public void InverseRotation(Bitmap src_bitmap)
        {
            set(src_bitmap, "InverseRotation", 360, -360);
        }
        public void addp(Bitmap b1,Bitmap b2)
        {
            if (b1.Width != b2.Width)
            {
                this.Hide();
                return;
            }
            statusStrip3.Visible = true;
            pictureBox1.Image = b1;
            pictureBox3.Image = b2;
            trackBar1.Enabled = true;
            trackBar1.Maximum = 100;
            trackBar1.Minimum = 0;
            buff = new Bitmap(b1.Width, b1.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < b1.Height; y++)
            {
                for (int x = 0; x < b1.Width; x++)
                {
                    buff.SetPixel(x, y, b1.GetPixel(x, y));
                }
            }
            buff2 = new Bitmap(b1.Width, b1.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < b1.Height; y++)
            {
                for (int x = 0; x < b1.Width; x++)
                {
                    buff2.SetPixel(x, y, b2.GetPixel(x, y));
                }
            }
            state = "addp";
            label2.Text = state;
        }
        public void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (state == "manual")
            {
                int threshold2 = trackBar1.Value;
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c = buff.GetPixel(x, y);
                        int avg = (c.R + c.G + c.B) / 3;
                        if (avg < threshold2)
                        {
                            outbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        }
                        else
                        {
                            outbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "depth")
            {
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                int depth = trackBar1.Value;
                if (depth == 0 || depth == 1) return;
                int cut = (int)(256 / (depth - 1));
                int[] ReplaceValue = new int[depth];
                ReplaceValue[0] = 0;
                for (int i = 1; i < depth; i++)
                {
                    if (ReplaceValue[i - 1] + cut > 255)
                    {
                        ReplaceValue[i] = 255;
                    }
                    else
                    {
                        ReplaceValue[i] = ReplaceValue[i - 1] + cut;
                    }
                }
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color buff_ = buff.GetPixel(x, y);
                        int avg = (buff_.R + buff_.G + buff_.B) / 3;
                        bool exist_ = false;
                        for (int i = 0; i < depth - 1; i++)
                        {
                            int judgevalue = (ReplaceValue[i] + ReplaceValue[i + 1]) / 2;
                            if (avg >= ReplaceValue[i] && avg < judgevalue)
                            {
                                outbitmap.SetPixel(x, y, Color.FromArgb(ReplaceValue[i], ReplaceValue[i], ReplaceValue[i]));
                                exist_ = true;
                                break;
                            }
                            else if (avg >= judgevalue && avg < ReplaceValue[i + 1])
                            {
                                exist_ = true;
                                outbitmap.SetPixel(x, y, Color.FromArgb(ReplaceValue[i + 1], ReplaceValue[i + 1], ReplaceValue[i + 1]));
                                break;
                            }
                        }
                        if (!exist_)
                        {
                            outbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "average")
            {
                int ratio = trackBar1.Value;
                if (buff.Width % ratio != 0 && buff.Height % ratio != 0) return;
                outbitmap = new Bitmap(buff.Width / ratio, buff.Height / ratio, PixelFormat.Format24bppRgb);
                int a = (buff.Width * buff.Width) / (ratio * ratio);
                Color[] Bitmappixel = new Color[a];
                int count = 0;
                for (int y = 0; y < buff.Height; y += ratio)
                {
                    for (int x = 0; x < buff.Width; x += ratio)
                    {
                        int r = 0, g = 0, b = 0;
                        for (int j = 0; j < ratio; j++)
                        {
                            for (int i = 0; i < ratio; i++)
                            {
                                Color c = buff.GetPixel(x + i, y + j);
                                r += c.R; g += c.G; b += c.B;
                            }
                        }
                        Bitmappixel[count] = Color.FromArgb(r / (ratio * ratio), g / (ratio * ratio), b / (ratio * ratio));
                        count++;
                    }
                }
                count = 0;
                for (int y = 0; y < buff.Height / ratio; y++)
                {
                    for (int x = 0; x < buff.Width / ratio; x++)
                    {
                        outbitmap.SetPixel(x, y, Bitmappixel[count]);
                        count++;
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "first")
            {
                int ratio = trackBar1.Value;
                if (buff.Width % ratio != 0 && buff.Height % ratio != 0) return;
                int w = (int)(Math.Ceiling((double)(buff.Width / ratio)));
                int h = (int)(Math.Ceiling((double)(buff.Height / ratio)));
                outbitmap = new Bitmap(w,h, PixelFormat.Format24bppRgb);
                Color[] BitmapPixel = new Color[(buff.Width * buff.Height)];
                int count = 0;
                for (int y = 0; y < buff.Height; y += ratio)
                {
                    for (int x = 0; x < buff.Width; x += ratio)
                    {
                        Color c = buff.GetPixel(x, y);
                        BitmapPixel.SetValue(c, count);
                        count++;
                    }
                }
                count = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(BitmapPixel[count].R, BitmapPixel[count].G, BitmapPixel[count].B));
                        count++;
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "linear")
            {
                double ratio = trackBar1.Value;

                int h = (int)(buff.Height * ratio);
                int w = (int)(buff.Width * ratio);
                outbitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        double y = j / ratio;
                        double x = i / ratio;
                        if (x <= (buff.Width - 1) & y <= (buff.Height - 1))
                        {
                            Color RGB = new Color();
                            int x00 = (int)x;
                            int y00 = (int)y;
                            int x01 = (int)Math.Ceiling(x);
                            int y01 = (int)y;
                            int y10 = (int)Math.Ceiling(y);
                            int x10 = (int)x;
                            int x11 = (int)Math.Ceiling(x);
                            int y11 = (int)Math.Ceiling(y);
                            double dx1 = x - x00;
                            double dx2 = x01 - x;
                            double dy1 = y - y00;
                            double dy2 = y10 - y;
                            if (dx1 != 0 & dx2 != 0 & dy1 != 0 & dy2 != 0)
                            {
                                byte r1 = buff.GetPixel(x00, y00).R;
                                byte g1 = buff.GetPixel(x00, y00).G;
                                byte b1 = buff.GetPixel(x00, y00).B;
                                byte r2 = buff.GetPixel(x01, y01).R;
                                byte g2 = buff.GetPixel(x01, y01).G;
                                byte b2 = buff.GetPixel(x01, y01).B;
                                byte r3 = buff.GetPixel(x10, y10).R;
                                byte b3 = buff.GetPixel(x10, y10).B;
                                byte g3 = buff.GetPixel(x10, y10).G;
                                byte r4 = buff.GetPixel(x11, y11).R;
                                byte b4 = buff.GetPixel(x11, y11).B;
                                byte g4 = buff.GetPixel(x11, y11).G;
                                byte r = (byte)((r1 * dx2 + r2 * dx1) * dy2 + (r3 * dx2 + r4 * dx1) * dy1);
                                byte b = (byte)((b1 * dx2 + b2 * dx1) * dy2 + (b3 * dx2 + b4 * dx1) * dy1);
                                byte g = (byte)((g1 * dx2 + g2 * dx1) * dy2 + (g3 * dx2 + g4 * dx1) * dy1);
                                RGB = Color.FromArgb(r, g, b);

                            }
                            else
                            {
                                RGB = buff.GetPixel((int)Math.Round(x), (int)Math.Round(y));
                            }
                            outbitmap.SetPixel(i, j, RGB);
                        }
                        else
                        {
                            int b1 = (int)Math.Round(x);
                            int b2 = (int)Math.Round(y);
                            if (b1 > (buff.Width - 1)) b1 = buff.Width - 1;
                            if (b2 > (buff.Height - 1)) b2 = buff.Height - 1;
                            outbitmap.SetPixel(i, j, buff.GetPixel(b1, b2));
                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "dump")
            {
                int ratio = trackBar1.Value;
                outbitmap = new Bitmap(buff.Width * ratio, buff.Height * ratio, PixelFormat.Format24bppRgb);
                for (int y = 0; y < buff.Height * ratio; y += ratio)
                {
                    for (int x = 0; x < buff.Width * ratio; x += ratio)
                    {
                        if (y % ratio == 0 && x % ratio == 0)
                        {
                            Color c = buff.GetPixel(x / ratio, y / ratio);
                            for (int j = 0; j < ratio; j++)
                            {
                                for (int i = 0; i < ratio; i++)
                                {
                                    outbitmap.SetPixel(i + x, j + y, Color.FromArgb(c.R, c.G, c.B));
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "PostiveRotation")
            {
                int xita = trackBar1.Value;
                int w0 = buff.Width;
                int h0 = buff.Height;
                double outw = Math.Ceiling(Math.Abs(w0 * Math.Cos(Math.PI * xita / 180)) + Math.Abs(h0 * Math.Sin(Math.PI * xita / 180)));
                double outh = Math.Ceiling(Math.Abs(w0 * Math.Sin(Math.PI * xita / 180)) + Math.Abs(h0 * Math.Cos(Math.PI * xita / 180)));
                outbitmap = new Bitmap((int)outw, (int)outh, PixelFormat.Format24bppRgb);
                for (int y = 0; y < (int)outh; y++)
                {
                    for (int x = 0; x < (int)outw; x++)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(240, 240, 240));
                    }
                }
                for (int y = 0; y < h0; y++)
                {
                    for (int x = 0; x < w0; x++)
                    {
                        double co = Math.Cos(Math.PI * xita / 180);
                        double si = Math.Sin(Math.PI * xita / 180);
                        int x1 = (int)(Math.Round(co * x - si * y - (0.5 * (w0 - 1) * co) + (0.5 * (h0 - 1) * si) + 0.5 * (outw - 1)));
                        int y1 = (int)(Math.Round(si * x + co * y - (0.5 * (w0 - 1) * si) - (0.5 * (h0 - 1) * co) + 0.5 * (outh - 1)));
                        outbitmap.SetPixel(x1, y1, buff.GetPixel(x, y));
                    }
                }

                pictureBox2.Image = outbitmap;
            }
            else if(state== "InverseRotation")
            {
                int xita = trackBar1.Value;
                int w0 = buff.Width;
                int h0 = buff.Height;
                double outw = Math.Ceiling(Math.Abs(w0 * Math.Cos(Math.PI * xita / 180)) + Math.Abs(h0 * Math.Sin(Math.PI * xita / 180)));
                double outh = Math.Ceiling(Math.Abs(w0 * Math.Sin(Math.PI * xita / 180)) + Math.Abs(h0 * Math.Cos(Math.PI * xita / 180)));
                outbitmap = new Bitmap((int)outw, (int)outh, PixelFormat.Format24bppRgb);
                for (int y = 0; y < (int)outh; y++)
                {
                    for (int x = 0; x < (int)outw; x++)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(240, 240, 240));
                    }
                }
                for (int y = 0; y < outh; y++)
                {
                    for (int x = 0; x < outw; x++)
                    {
                        double co = Math.Cos(Math.PI * xita / 180);
                        double si = Math.Sin(Math.PI * xita / 180);
                        int x1 = (int)((co * x + si * y - (0.5 * (outw - 1) * co) - (0.5 * (outh - 1) * si) + 0.5 * (w0 - 1)));
                        int y1 = (int)((-si * x + co * y + (0.5 * (outw - 1) * si) - (0.5 * (outh - 1) * co) + 0.5 * (h0 - 1)));
                        if (y1 < h0 && x1 < w0 && x1 >= 0 && y1 >= 0)
                        {
                            outbitmap.SetPixel(x, y, buff.GetPixel(x1, y1));

                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "addp")
            {
                double alpha = (double)trackBar1.Value/100;
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                if (buff.Width != buff2.Width || buff.Height != buff2.Height)
                {
                    return;
                }
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c1 = buff.GetPixel(x, y);
                        Color c2 = buff2.GetPixel(x, y);
                        int r = (int)(alpha * c1.R + (1 - alpha) * c2.R);
                        int g = (int)(alpha * c1.G + (1 - alpha) * c2.G);
                        int b = (int)(alpha * c1.B + (1 - alpha) * c2.B);
                        outbitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "gamma")
            {
                pictureBox6.Visible = true;
                double alpha = (double)trackBar1.Value/100;
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                for(int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c1 = buff.GetPixel(x, y);
                        
                        Byte rout = (Byte)((Math.Pow((c1.R+0.5) / 256, alpha) * 256)-0.5);
                        Byte gout = (Byte)((Math.Pow((c1.G+ 0.5) / 256 , alpha) * 256) - 0.5);
                        Byte bout = (Byte)((Math.Pow((c1.B+ 0.5) / 256 , alpha) * 256) - 0.5);
                        //Console.WriteLine(rout);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    }
                }
                pictureBox2.Image = outbitmap; 
            }
            else if (state == "outlier")
            {
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c_ = buff.GetPixel(x, y);
                        int count = 0;
                        int sumr = 0;
                        int sumg = 0;
                        int sumb = 0;
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (x + i < 0 || y + j < 0 || x + i > buff.Width - 1 || y + j > buff.Height - 1)
                                {
                                    sumr += 0;
                                    sumg += 0;
                                    sumb += 0;
                                    count++;
                                }
                                else
                                {
                                    if (i != 0 && j != 0)
                                    {
                                        Color c = buff.GetPixel(x + i, y + j);
                                        sumr += c.R;
                                        sumg += c.G;
                                        sumb += c.B;
                                        count++;
                                    }

                                }
                            }
                        }
                        Byte rout = (c_.R - (sumr / count) > 0) ? (Byte)(sumr / count) : (Byte)(c_.R);
                        Byte gout = (c_.G - (sumg / count) > 0) ? (Byte)(sumg / count) : (Byte)(c_.G);
                        Byte bout = (c_.R - (sumb / count) > 0) ? (Byte)(sumb / count) : (Byte)(c_.B);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    }
                }
                pictureBox2.Image = outbitmap;

            }
            else if (state == "contrast")
            {
                double ratio = (double)trackBar1.Value / 100;
                int r1 = (int)(ratio * 256);
                int r2 = (int)((1 - ratio) * 256);
                int s1 = 0;
                int s2 = 255;
                outbitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        double buff_ = 0;
                        byte value = buff.GetPixel(x, y).R;
                        if (value <= r1)
                        {
                            buff_ = (value / r1) * s1;
                        }
                        else if (value >= r2)
                        {
                            buff_ = ((value - r2) / (255 - r2)) * (255 - s2) + s2;
                        }
                        else if (value < r2 && value > r1)
                        {
                            buff_ = ((value - r1) / (r2 - r1)) * (s2 - s1) + s1;
                        }
                        outbitmap.SetPixel(x, y, Color.FromArgb((int)buff_, (int)buff_, (int)buff_));
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "Average_fifter")
            {
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c_ = buff.GetPixel(x, y);
                        int count = 0;
                        int sumr = 0;
                        int sumg = 0;
                        int sumb = 0;
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (x + i < 0 || y + j < 0||x+i>buff.Width-1||y+j>buff.Height-1)
                                {
                                    sumr += 0;
                                    sumg += 0;
                                    sumb += 0;
                                }
                                else
                                {
                                    Color c = buff.GetPixel(x + i, y + j);
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
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "median")
            {
                
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                int masksize = mask + mask -1;
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c_ = buff.GetPixel(x, y);
                        int count = 0;
                        int[] rcontent = new int[masksize];
                        int[] gcontent = new int[masksize];
                        int[] bcontent = new int[masksize];
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (i == 0 || j == 0)
                                {
                                    if (x + i < 0 || y + j < 0 || x + i > buff.Width - 1 || y + j > buff.Height - 1)
                                    {
                                        rcontent[count] = 0;
                                        gcontent[count] = 0;
                                        bcontent[count] = 0;
                                    }
                                    else
                                    {
                                        Color c = buff.GetPixel(x + i, y + j);
                                        rcontent[count] = c.R;
                                        gcontent[count] = c.G;
                                        bcontent[count] = c.B;

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
                        Byte rout = (Byte)(rcontent[masksize / 2]);
                        Byte gout = (Byte)(gcontent[masksize / 2]);
                        Byte bout = (Byte)(bcontent[masksize / 2]);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "median_nn")
            {
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                //int masksize = mask + mask -1;
                int masksize = mask * mask;
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        //Color c_ = buff.GetPixel(x, y);
                        int count = 0;
                        int[] rcontent = new int[masksize];
                        int[] gcontent = new int[masksize];
                        int[] bcontent = new int[masksize];
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (x + i < 0 || y + j < 0 || x + i > buff.Width - 1 || y + j > buff.Height - 1)
                                {
                                    rcontent[count] = 0;
                                    gcontent[count] = 0;
                                    bcontent[count] = 0;
                                }
                                else
                                {
                                    Color c = buff.GetPixel(x + i, y + j);
                                    rcontent[count] = c.R;
                                    gcontent[count] = c.G;
                                    bcontent[count] = c.B;

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
                        Byte rout = (Byte)(rcontent[masksize / 2]);
                        Byte gout = (Byte)(gcontent[masksize / 2]);
                        Byte bout = (Byte)(bcontent[masksize / 2]);
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                        
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "crispening")
            {
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                int masksize = mask + mask - 1;
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        Color c_ = buff.GetPixel(x, y);
                        int count = 0;
                        int[] rcontent = new int[masksize];
                        int[] gcontent = new int[masksize];
                        int[] bcontent = new int[masksize];
                        for (int j = -blank; j <= blank; j++)
                        {
                            for (int i = -blank; i <= blank; i++)
                            {
                                if (i == 0 || j == 0)
                                {
                                    if (x + i < 0 || y + j < 0 || x + i > buff.Width - 1 || y + j > buff.Height - 1)
                                    {
                                        rcontent[count] = 0;
                                        gcontent[count] = 0;
                                        bcontent[count] = 0;
                                    }
                                    else
                                    {
                                        Color c = buff.GetPixel(x + i, y + j);
                                        rcontent[count] = c.R;
                                        gcontent[count] = c.G;
                                        bcontent[count] = c.B;

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
                        int gray = (c_.R + c_.G + c_.B) / 3;
                        Byte rout = (Byte)(gray - rcontent[masksize / 2]);
                        Byte gout = (Byte)(gray - gcontent[masksize / 2]);
                        Byte bout = (Byte)(gray - bcontent[masksize / 2]);
                        if (rout > 255) rout = 255;
                        else if (rout < 0) rout = 0;
                        if (gout > 255) gout = 255;
                        else if (gout < 0) gout = 0;
                        if (bout > 255) bout = 255;
                        else if (bout < 0) bout = 0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(rout, gout, bout));
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            else if (state == "highpass")
            {
                outbitmap = new Bitmap(buff.Width, buff.Height, PixelFormat.Format24bppRgb);
                int mask = trackBar1.Value;
                if (mask % 2 == 0) return;
                int blank = mask / 2;
                for (int y = 0; y < buff.Height; y++)
                {
                    for (int x = 0; x < buff.Width; x++)
                    {
                        if (x > blank && y > blank && x < buff.Width - blank - 1 && y < buff.Height - blank - 1)
                        {
                            Color c_ = buff.GetPixel(x, y);
                            int count = 0;
                            int sumr = 0;
                            int sumg = 0;
                            int sumb = 0;
                            for (int j = -blank; j <= blank; j++)
                            {
                                for (int i = -blank; i <= blank; i++)
                                {
                                    if (x + i < 0 || y + j < 0 || x + i > buff.Width - 1 || y + j > buff.Height - 1)
                                    {
                                        sumr += 0;
                                        sumg += 0;
                                        sumb += 0;
                                    }
                                    else
                                    {
                                        Color c = buff.GetPixel(x + i, y + j);
                                        if (i == 0 && j == 0)
                                        {
                                            sumr += (c.R * (mask * mask - 1));
                                            sumg += (c.G * (mask * mask - 1));
                                            sumb += (c.B * (mask * mask - 1));
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
                            int buff1 = sumr / 9;
                            int buff2 = sumg / 9;
                            int buff3 = sumb / 9;
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
                            outbitmap.SetPixel(x, y, Color.FromArgb(buff1, buff2, buff3));
                        }
                        else
                        {
                            outbitmap.SetPixel(x, y, Color.FromArgb(240, 240, 240));
                        }
                    }
                }
                pictureBox2.Image = outbitmap;
            }
            if (outbitmap != null)
            {
                snr_(buff, outbitmap);
            }
            else
            {
                toolStripStatusLabel35.Text = "_";
            }
        }
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
        private void p1_MouseMove(object sender, MouseEventArgs e)
        {
            if (buff != null)
            {
                if ((e.X < buff.Width) && (e.Y < buff.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel3.Text = e.X.ToString();
                    toolStripStatusLabel5.Text = e.Y.ToString();
                    toolStripStatusLabel7.Text = buff.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel9.Text = buff.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel11.Text = buff.GetPixel(e.X, e.Y).B.ToString();
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
                
            }
        }
        private void p2_MouseMove(object sender, MouseEventArgs e)
        {
            if (buff2 != null)
            {
                if ((e.X < buff2.Width) && (e.Y < buff2.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel25.Text = e.X.ToString();
                    toolStripStatusLabel27.Text = e.Y.ToString();
                    toolStripStatusLabel29.Text = buff2.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel31.Text = buff2.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel33.Text = buff2.GetPixel(e.X, e.Y).B.ToString();
                }
                
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            double value = (double)trackBar1.Value / 100;
            f.showtransform(value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (state == "gamma"||state=="addp"||state=="contrast")
            {
                double t = (double)trackBar1.Value / 100;
                //Console.WriteLine(t);
                label1.Text = t.ToString();
            }
            else
            {
                label1.Text = trackBar1.Value.ToString();
            }
        }
    }
}
