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
    public partial class video : Form
    {
        long framecount = 0;
        long pos_now = 0;
        public video()
        {
            InitializeComponent();
        }
        Bitmap[] bitmap;
        Bitmap[] outbitmap;
        double orginal;
        double[] psnr;
        string playstate,algo;
        public int otsu(BitmapData bitmapdata, int height, int width, int size)
        {
            IntPtr intPtr = bitmapdata.Scan0;
            byte[] oriBytes = new byte[size];
            Marshal.Copy(intPtr, oriBytes, 0, size);
            int k = 3;
            int[] histogram = new int[256];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte b = oriBytes[y * bitmapdata.Stride + x * k];
                    byte g = oriBytes[y * bitmapdata.Stride + x * k + 1];
                    byte r = oriBytes[y * bitmapdata.Stride + x * k + 2];
                    int avg = (r + g + b) / 3;
                    histogram[avg]++;
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
                q1 = lefthistogramnum / (width * height);
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
                q2 = righthistogramnum / (width * height);
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
            return refrence;
        }
        public void putsnr(double [] snr,long framecount,string name)
        {
            chart1.Visible = true;
            double[] index = new double[framecount];
            for(int i = 0; i<framecount; i++)
            {
                index[i] = i;
            }
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "framecount";
            chart1.ChartAreas[0].AxisY.Title = "psnr";
            chart1.Series.Add(name);
            chart1.Series[name].ChartType = SeriesChartType.Line;
            chart1.Series[name].Color = Color.Red;
            chart1.Series[name].BorderWidth = 1;
            chart1.Series[name].XValueType = ChartValueType.Int32;
            chart1.Series[name].YValueType = ChartValueType.Double;
            chart1.Series[name].Points.DataBindXY(index, snr);
        }
        public double snr_compute(Bitmap p1, Bitmap p2)
        {
            /*
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
            double snr = (double)10 * Math.Log10(outcount / difference);
            snr = Math.Round(snr, 2);
            if (difference == 0)
            {
                toolStripStatusLabel33.Text = "與原圖相同";
            }
            else
            {
                toolStripStatusLabel33.Text = snr.ToString() + "db";
            }
             */
            long difference = 0;
            long outcount = 0;
            for (int y = 0; y < p1.Height; y++)
            {
                for (int x = 0; x < p1.Width; x++)
                {
                    Color c = p1.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    Color c_ = p2.GetPixel(x, y);
                    int avg_ = (c_.R + c_.G + c_.B) / 3;
                    outcount += (long)Math.Pow(255, 2);
                    difference += (long)Math.Pow(avg - avg_, 2);
                }
            }
            int size = p1.Width * p1.Height;
            /*double snr = 10 * Math.Log10(Math.Pow(size * 255, 2) / difference);
            return (difference==0) ? 100000 : snr;*/
            if (difference == 0)
            {
                return 10000;
            }
            else
            {
                return 10 * Math.Log10(outcount / difference);
            }
        }
        private void timer1_Tick(object sender, EventArgs e) {
            if (playstate == "normal")
            {
                pictureBox3.Image = null;
                pictureBox13.Image = null;
                chart1.Visible = false;
                if (downplay)
                {
                    if (pos_now < 0)
                    {
                        pos_now = framecount - 1;
                        pictureBox1.Image = bitmap[pos_now];
                        toolStripStatusLabel6.Text = pos_now.ToString();

                        toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                        toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                    }
                    else
                    {
                        if (next)
                        {
                            if (pos_now >= framecount - 1)
                            {
                                pos_now = 0;
                            }
                            else
                            {
                                pos_now++;
                            }
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();


                            toolStripStatusLabel6.Text = pos_now.ToString();
                            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();

                            timer1.Enabled = false;
                        }
                        else if (previous)
                        {
                            if (pos_now <= 0)
                            {
                                pos_now = framecount - 1;
                            }
                            else
                            {
                                pos_now--;
                            }
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();

                            toolStripStatusLabel6.Text = pos_now.ToString();

                            timer1.Enabled = false;
                        }
                        else
                        {
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();

                            toolStripStatusLabel6.Text = pos_now.ToString();

                            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                            pos_now--;

                        }

                    }
                }
                else
                {
                    if (pos_now > framecount - 1)
                    {
                        pos_now = 0;
                        pictureBox1.Image = bitmap[pos_now];

                        toolStripStatusLabel6.Text = pos_now.ToString();

                        toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                        toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                    }
                    else
                    {
                        if (next)
                        {
                            if (pos_now >= framecount - 1)
                            {
                                pos_now = 0;
                            }
                            else
                            {
                                pos_now++;
                            }
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();


                            toolStripStatusLabel6.Text = pos_now.ToString();
                            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();

                            timer1.Enabled = false;
                        }
                        else if (previous)
                        {
                            if (pos_now <= 0)
                            {
                                pos_now = framecount - 1;
                            }
                            else
                            {
                                pos_now--;
                            }
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();

                            toolStripStatusLabel6.Text = pos_now.ToString();

                            timer1.Enabled = false;
                        }
                        else
                        {
                            pictureBox1.Image = bitmap[pos_now];
                            pictureBox1.Refresh();

                            toolStripStatusLabel6.Text = pos_now.ToString();
                            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();

                            pos_now++;

                        }

                    }
                }
            }
            else if (playstate == "decode")
            {
                if (pos_now >= framecount - 1)
                {

                    pictureBox1.Image = bitmap[pos_now];
                    pictureBox1.Refresh();
                    pictureBox3.Image = newbitmap[pos_now];
                    pictureBox3.Refresh();
                    chart1.Visible = true;
                    for (int i = 1; i < framecount; i++)
                    {
                        psnr[i] = snr_compute(bitmap[i], newbitmap[i]);
                    }
                    putsnr(psnr, framecount, algo);

                    toolStripStatusLabel6.Text = pos_now.ToString();

                    toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                    toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                    pos_now = 0;
                    timer1.Enabled = false;
                }
                else
                {
                    chart1.Visible = false;
                    pictureBox1.Image = bitmap[pos_now];
                    pictureBox1.Refresh();
                    pictureBox3.Image = newbitmap[pos_now];
                    pictureBox3.Refresh();
                    string title, name, end,path="";
                    if (algo == "full_search")
                    {                        
                        if (videoname == "6.2")
                        {
                            title = "D:\\2022-Image\\6.2safe\\";
                        }
                        else if (videoname == "6.1")
                        {
                            title = "D:\\2022-Image\\6.1safe\\FULL";
                        }
                        else if (videoname == "6.3")
                        {
                            title = "D:\\2022-Image\\6.3safe\\";
                        }
                        else
                        {
                            title = "";
                        }
                        name = (pos_now).ToString();
                        end = ".txt";
                        path = title + name + end;
                    }
                    else if (algo == "tdl")
                    {
                        if (videoname == "6.2")
                        {
                            title = "D:\\2022-Image\\6.2safetdl\\";
                        }
                        else if (videoname == "6.1")
                        {
                            title = "D:\\2022-Image\\6.1safetdl\\";
                        }
                        else if (videoname == "6.3")
                        {
                            title = "D:\\2022-Image\\6.3safetdl\\";
                        }
                        else
                        {
                            title = "";
                        }
                        name = (pos_now).ToString() + (pos_now+1).ToString();
                        end = ".txt";
                        path = title + name + end;
                    }
                    else if (algo == "three")
                    {
                        if (videoname == "6.2")
                        {
                            title = "D:\\2022-Image\\6.2safethreestep\\";
                        }
                        else if (videoname == "6.1")
                        {
                            title = "D:\\2022-Image\\6.1safethreestep\\";
                        }
                        else if (videoname == "6.3")
                        {
                            title = "D:\\2022-Image\\6.3safethreestep\\";
                        }
                        else
                        {
                            title = "";
                        }
                        name = (pos_now).ToString() + (pos_now + 1).ToString();
                        end = ".txt";
                        path = title + name + end;
                    }
                    else if (algo == "osa")
                    {
                        if (videoname == "6.2")
                        {
                            title = "D:\\2022-Image\\6.2safeosa\\";
                        }
                        else if (videoname == "6.1")
                        {
                            title = "D:\\2022-Image\\6.1safeosa\\";
                        }
                        else if (videoname == "6.3")
                        {
                            title = "D:\\2022-Image\\6.3safeosa\\";
                        }
                        else
                        {
                            title = "";
                        }
                        name = (pos_now).ToString() + (pos_now + 1).ToString();
                        end = ".txt";
                        path = title + name + end;
                    }
                    int nowx = 0, nowy = 0;
                    int width = newbitmap[pos_now].Width;
                    int height = newbitmap[pos_now].Height;
                    pointcount = 0;
                    foreach (string line in System.IO.File.ReadLines(path))
                    {
                        int motionx=0, motiony=0;
                        if (algo == "full_search")
                        {
                            string[] words = line.Split('(', ',', ')');
                            motionx = Convert.ToInt16(words[1]);
                            motiony = Convert.ToInt16(words[2]);
                        }
                        else
                        {
                            char[] TrimChars = { ' ' };
                            string[] words = line.Split();
                            int split_count = 0;
                            foreach (string word in words)
                            {
                                if (split_count == 0)
                                {
                                    motionx = int.Parse(word.TrimEnd(TrimChars));
                                }
                                else
                                {
                                    motiony = int.Parse(word.TrimEnd(TrimChars));
                                }
                                split_count++;

                            }
                        }
                        int outx = nowx + motionx;
                        int outy = nowy + motiony;
                        setmotion2(nowx, nowy, outx, outy);
                        if (nowx + 8 > width - 1)
                        {
                            if (nowy + 8 > height - 1)
                            {
                                nowx = 0; nowy = 0;
                                //break;
                            }
                            else
                            {
                                nowx = 0; nowy += 8;
                            }
                        }
                        else
                        {
                            nowx += 8;
                        }
                    }
                    drawmotion = true;
                    pictureBox13.Refresh();
                    toolStripStatusLabel6.Text = pos_now.ToString();
                    toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                    toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                    pos_now++;

                }
            }

        }
        public Bitmap process(Bitmap bitmap, string state)
        {
            Bitmap bbb = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData outbitmapdata = bbb.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtrN = outbitmapdata.Scan0;
            IntPtr intPtr = bitmapdata.Scan0;
            int size = bitmapdata.Stride * bitmap.Height;
            byte[] oriBytes = new byte[size];
            byte[] newBytes = new byte[size];
            Marshal.Copy(intPtr, oriBytes, 0, size);
            Marshal.Copy(intPtr, newBytes, 0, size);
            int k = 3;
            if (state == "r")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)0;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)0;
                    }
                }
            }
            else if (state == "g")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)0;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)0;
                    }
                }
            }
            else if (state == "b")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)0;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)0;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)out_;
                    }
                }
            }
            else if (state == "gray")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)out_;
                    }
                }
            }
            else if (state == "negative")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)(255 - out_);
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)(255 - out_);
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)(255 - out_);
                    }
                }
            }
            else if (state == "twolevel")
            {
                int threshold = otsu(bitmapdata, bitmap.Height, bitmap.Width, size);
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)(out_ > threshold ? 255 : 0);
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)(out_ > threshold ? 255 : 0);
                        newBytes[(y) * bitmapdata.Stride + x * k + 0] = (byte)(out_ > threshold ? 255 : 0);
                    }
                }
            }
            else if (processstate == "x")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {

                        byte p1 = oriBytes[y * bitmapdata.Stride + x * k];
                        byte p2 = oriBytes[y * bitmapdata.Stride + (bitmap.Width - 1 - x) * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)p2;
                        newBytes[y * bitmapdata.Stride + (bitmap.Width - 1 - x) * k + 2] = (byte)p1;
                        newBytes[y * bitmapdata.Stride + (bitmap.Width - 1 - x) * k + 1] = (byte)p1;
                        newBytes[y * bitmapdata.Stride + (bitmap.Width - 1 - x) * k] = (byte)p1;
                    }
                }
            }
            else if (processstate == "y")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {

                        byte p1 = oriBytes[y * bitmapdata.Stride + x * k];
                        byte p2 = oriBytes[(bitmap.Height - 1 - y) * bitmapdata.Stride + (x) * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)p2;
                        newBytes[(bitmap.Height - 1 - y) * bitmapdata.Stride + (x) * k + 2] = (byte)p1;
                        newBytes[(bitmap.Height - 1 - y) * bitmapdata.Stride + (x) * k + 1] = (byte)p1;
                        newBytes[(bitmap.Height - 1 - y) * bitmapdata.Stride + (x) * k] = (byte)p1;
                    }
                }
            }
            else if (processstate == "diagonal")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = y; x < bitmap.Width; x++)
                    {
                        byte p1 = oriBytes[y * bitmapdata.Stride + x * k];
                        byte p2 = oriBytes[(x) * bitmapdata.Stride + (y) * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)p2;
                        newBytes[(x) * bitmapdata.Stride + (y) * k + 2] = (byte)p1;
                        newBytes[(x) * bitmapdata.Stride + (y) * k + 1] = (byte)p1;
                        newBytes[(x) * bitmapdata.Stride + (y) * k] = (byte)p1;
                    }
                }
            }
            else if (processstate == "opposite")
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width - y; x++)
                    {
                        byte p1 = oriBytes[y * bitmapdata.Stride + x * k];
                        byte p2 = oriBytes[(bitmap.Width - 1 - x) * bitmapdata.Stride + (bitmap.Height - 1 - y) * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)p2;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)p2;
                        newBytes[(bitmap.Width - 1 - x) * bitmapdata.Stride + (bitmap.Height - 1 - y) * k + 2] = (byte)p1;
                        newBytes[(bitmap.Width - 1 - x) * bitmapdata.Stride + (bitmap.Height - 1 - y) * k + 1] = (byte)p1;
                        newBytes[(bitmap.Width - 1 - x) * bitmapdata.Stride + (bitmap.Height - 1 - y) * k] = (byte)p1;
                    }
                }
            }
            else if (processstate == "noise")
            {
                var rand = new Random();
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int num = rand.Next(101);
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        if (num > 0 && num < 5)
                        {
                            out_ = 0;
                        }
                        else if (num >= 5 && num < 10)
                        {
                            out_ = 255;
                        }
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)out_;
                    }
                }
            }
            else
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        byte out_ = oriBytes[y * bitmapdata.Stride + x * k];
                        newBytes[(y) * bitmapdata.Stride + x * k + 2] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k + 1] = (byte)out_;
                        newBytes[(y) * bitmapdata.Stride + x * k] = (byte)out_;
                    }
                }
            }
            Marshal.Copy(newBytes, 0, intPtrN, size);
            bitmap.UnlockBits(bitmapdata);
            bbb.UnlockBits(outbitmapdata);
            return bbb;
        }

        bool next = false;

        string processstate = string.Empty;

        
        bool previous = false;
        string videoname = string.Empty;
        private void p6_1(object sender, EventArgs e)
        {
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            playstate = "normal";
            chart1.Visible = false;
            functionToolStripMenuItem.Enabled = true;
            framecount = 16;
            bitmap = new Bitmap[16];
            pos_now = 0;
            outbitmap = new Bitmap[16];
            psnr = new double[framecount];
            videoname = "6.1";
            bitmap[0] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.01.tiff");
            bitmap[1] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.02.tiff");
            bitmap[2] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.03.tiff");
            bitmap[3] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.04.tiff");
            bitmap[4] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.05.tiff");
            bitmap[5] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.06.tiff");
            bitmap[6] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.07.tiff");
            bitmap[7] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.08.tiff");
            bitmap[8] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.09.tiff");
            bitmap[9] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.10.tiff");
            bitmap[10] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.11.tiff");
            bitmap[11] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.12.tiff");
            bitmap[12] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.13.tiff");
            bitmap[13] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.14.tiff");
            bitmap[14] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.15.tiff");
            bitmap[15] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.1.16.tiff");
            pictureBox1.Image = bitmap[0];
            timer1.Enabled = false;
            timer2.Enabled = false;
            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
            toolStripStatusLabel8.Text = (framecount - 1).ToString();
            pictureBox3.Image = null;
            pictureBox13.Image = null;
            timer1.Interval = (100);
            toolStripStatusLabel6.Text = "0";
            orginal = timer1.Interval;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
        }
        private void p6_2(object sender, EventArgs e)
        {
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            playstate = "normal";
            chart1.Visible = false;
            functionToolStripMenuItem.Enabled = true;
            framecount = 32;
            bitmap = new Bitmap[32];
            pos_now = 0;
            outbitmap = new Bitmap[32];
            psnr = new double[framecount];
            videoname = "6.2";
            bitmap[0] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.01.tiff");
            bitmap[1] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.02.tiff");
            bitmap[2] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.03.tiff");
            bitmap[3] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.04.tiff");
            bitmap[4] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.05.tiff");
            bitmap[5] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.06.tiff");
            bitmap[6] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.07.tiff");
            bitmap[7] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.08.tiff");
            bitmap[8] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.09.tiff");
            bitmap[9] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.10.tiff");
            bitmap[10] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.11.tiff");
            bitmap[11] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.12.tiff");
            bitmap[12] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.13.tiff");
            bitmap[13] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.14.tiff");
            bitmap[14] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.15.tiff");
            bitmap[15] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.16.tiff");
            bitmap[16] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.17.tiff");
            bitmap[17] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.18.tiff");
            bitmap[18] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.19.tiff");
            bitmap[19] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.20.tiff");
            bitmap[20] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.21.tiff");
            bitmap[21] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.22.tiff");
            bitmap[22] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.23.tiff");
            bitmap[23] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.24.tiff");
            bitmap[24] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.25.tiff");
            bitmap[25] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.26.tiff");
            bitmap[26] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.27.tiff");
            bitmap[27] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.28.tiff");
            bitmap[28] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.29.tiff");
            bitmap[29] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.30.tiff");
            bitmap[30] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.31.tiff");
            bitmap[31] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.2.32.tiff");
            pictureBox1.Image = bitmap[0];
            timer2.Enabled = false;
            timer1.Enabled = false;
            pictureBox3.Image = null;
            pictureBox13.Image = null;
            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
            toolStripStatusLabel8.Text = (framecount - 1).ToString();
            timer1.Interval = (100);
            toolStripStatusLabel6.Text = "0";
            orginal = timer1.Interval;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
        }
        private void p6_3(object sender, EventArgs e)
        {
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            playstate = "normal";
            chart1.Visible = false;
            functionToolStripMenuItem.Enabled = true;
            framecount = 11;
            pos_now = 0;
            bitmap = new Bitmap[11];
            outbitmap = new Bitmap[11];
            psnr = new double[framecount];
            videoname = "6.3";
            pictureBox3.Image = null;
            pictureBox13.Image = null;
            bitmap[0] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.01.tiff");
            bitmap[1] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.02.tiff");
            bitmap[2] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.03.tiff");
            bitmap[3] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.04.tiff");
            bitmap[4] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.05.tiff");
            bitmap[5] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.06.tiff");
            bitmap[6] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.07.tiff");
            bitmap[7] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.08.tiff");
            bitmap[8] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.09.tiff");
            bitmap[9] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.10.tiff");
            bitmap[10] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\6.3.11.tiff");
            pictureBox1.Image = bitmap[0];
            timer1.Enabled = false;
            timer2.Enabled = false;
            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
            toolStripStatusLabel8.Text = (framecount - 1).ToString();
            timer1.Interval = (100);
            toolStripStatusLabel6.Text = "0";
            orginal = timer1.Interval;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
        }
        private void motion(object sender, EventArgs e)
        {
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            playstate = "normal";
            chart1.Visible = false;
            functionToolStripMenuItem.Enabled = true;
            framecount = 10;
            pos_now = 0;
            bitmap = new Bitmap[10];
            outbitmap = new Bitmap[10];
            psnr = new double[framecount];
            videoname = "motion";
            pictureBox3.Image = null;
            pictureBox13.Image = null;
            bitmap[0] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion01.512.tiff");
            bitmap[1] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion02.512.tiff");
            bitmap[2] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion03.512.tiff");
            bitmap[3] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion04.512.tiff");
            bitmap[4] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion05.512.tiff");
            bitmap[5] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion06.512.tiff");
            bitmap[6] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion07.512.tiff");
            bitmap[7] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion08.512.tiff");
            bitmap[8] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion09.512.tiff");
            bitmap[9] = new Bitmap("D:\\2022-Image\\Src_img\\sequences\\motion10.512.tiff");
            pictureBox1.Image = bitmap[0];
            timer1.Enabled = false;
            timer2.Enabled = false;
            toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
            toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
            toolStripStatusLabel8.Text = (framecount - 1).ToString();
            toolStripStatusLabel6.Text = "0";
            timer1.Interval = (100);
            orginal = timer1.Interval;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
        }  
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            next = false;
            previous = false;
            downplay = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            next = true;
            if (previous) previous = false;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (timer1.Interval / 2 > 0)
            {
                timer1.Interval /= 2;
                double sppedup = orginal / timer1.Interval;
                sppedup = Math.Round(sppedup, 3);
                toolStripStatusLabel10.Text = sppedup.ToString() + "倍";
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            previous = true;
            if (next) next = false;
            timer1.Enabled = true;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            timer1.Interval *= 2;
            double sppedup = orginal / timer1.Interval;
            sppedup = Math.Round(sppedup, 3);
            toolStripStatusLabel10.Text = sppedup.ToString() + "倍";
        }

        private void openbyfile(object sender, EventArgs e)
        {
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            playstate = "normal";
            chart1.Visible = false;
            bitmap = new Bitmap[100];
            framecount = 0;
            pos_now = 0;
            outbitmap = new Bitmap[100];
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "所有檔案(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    bitmap[framecount] = new Bitmap(file);
                    framecount++;
                }
                functionToolStripMenuItem.Enabled = true;
                psnr = new double[framecount];
                pictureBox1.Image = bitmap[0];
                timer1.Enabled = false;
                timer2.Enabled = false;
                toolStripStatusLabel2.Text = bitmap[pos_now].Height.ToString();
                toolStripStatusLabel4.Text = bitmap[pos_now].Width.ToString();
                toolStripStatusLabel8.Text = (framecount - 1).ToString();
                toolStripStatusLabel6.Text = "0";
                timer1.Interval = (100);
                orginal = timer1.Interval;
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = true;
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                videoname = "multiselect";
            }

        }
        bool downplay = false;
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            downplay = true;
            next = false;
            previous = false;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            this.Hide();
            timer2.Enabled = false;
            timer1.Enabled = false;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            //timer1.Enabled = true;
            chart1.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            pictureBox11.Image = null;
            pictureBox12.Image = null;
            pictureBox13.Image = null;
            pictureBox3.Image = null;
            curx = cury = refx = refy = 0;
            drawcur = false;
            drawref = false;
            pos_now = 0;
            pictureBox1.Image = bitmap[pos_now];
        }
        bool drawref = false;
        bool drawcur = false;
        int curx = 0, cury = 0, refx = 0, refy = 0;
        int drawx1 = 0,drawy1=0,drawx2=0,drawy2=0;
        public void enabledraw(int x1,int y1,int x2,int y2)
        {
            drawx1 = x1;
            drawy1 = y1;
            drawx2 = x2;
            drawy2 = y2;
            drawref = true;
            drawcur = true;
            pictureBox1.Refresh();
            pictureBox3.Refresh();

        }
        class drawpoint
        {
            public int startx { get; set; }
            public int starty { get; set; }
            public int endx { get; set; }
            public int endy { get; set; }
            public drawpoint(int x1, int y1, int x2, int y2)
            {
                startx = x1;
                starty = y1;
                endx = x2;
                endy = y2;
            }
        }


        bool drawmotion = false;
        int pointcount = 0;
        public void setmotion(int cx, int cy, int rx, int ry)
        {
            d[pointcount] = new drawpoint(cx, cy, rx, ry);
            string title = "D:\\2022-Image\\";
            string name =(pos_now - 1).ToString() +  pos_now.ToString() + ".txt";
            string filepath = title + name;
            if (!File.Exists(filepath))
            {
                using (sw = File.CreateText(filepath))
                {
                    sw.Write((d[pointcount].endx - d[pointcount].startx).ToString());
                    sw.Write(" ");
                    sw.Write((d[pointcount].endy - d[pointcount].starty).ToString());
                    sw.Write("\n");
                }
            }
            else
            {
                using (sw = File.AppendText(filepath))
                {
                    sw.Write((d[pointcount].endx - d[pointcount].startx).ToString());
                    sw.Write(" ");
                    sw.Write((d[pointcount].endy - d[pointcount].starty).ToString());
                    sw.Write("\n");
                }

            }
            pointcount++;
            drawmotion = true;
            pictureBox13.Refresh();
        }
        public void setmotion2(int cx, int cy, int rx, int ry)
        {
            d[pointcount] = new drawpoint(cx, cy, rx, ry);
            
            pointcount++;
            /*drawmotion = true;
            pictureBox13.Refresh();*/
        }
        private void button9_Click(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            drawcur = true;
            drawref = true;
            timer2.Enabled = false;

        }

        double mintotal = double.MaxValue;
        int minx, miny;
        private void timer2_Tick(object sender, EventArgs e)
        {

            if (pos_now > 0 && pos_now < framecount - 1)
            {
                Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
                Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
                int[,] refmatrix = new int[8, 8];
                int[,] curmatrix = new int[8, 8];
                BitmapData bitmapdata = bitmap[pos_now].LockBits(new Rectangle(0, 0, bitmap[pos_now].Width, bitmap[pos_now].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData outbitmapdata = small_cur.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                IntPtr intPtrN = outbitmapdata.Scan0;
                IntPtr intPtr = bitmapdata.Scan0;
                int size = bitmapdata.Stride * bitmap[pos_now].Height;
                int size2 = outbitmapdata.Stride * small_cur.Height;
                byte[] oriBytes = new byte[size];
                byte[] newBytes = new byte[size2];
                Marshal.Copy(intPtr, oriBytes, 0, size);
                Marshal.Copy(intPtrN, newBytes, 0, size2);
                int k = 3;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        byte p1 = oriBytes[(y + cury) * bitmapdata.Stride + (x + curx) * k];
                        newBytes[(y) * outbitmapdata.Stride + x * k + 2] = (byte)p1;
                        newBytes[(y) * outbitmapdata.Stride + x * k + 1] = (byte)p1;
                        newBytes[(y) * outbitmapdata.Stride + x * k] = (byte)p1;
                        curmatrix[y, x] = p1;

                    }
                }
                Marshal.Copy(newBytes, 0, intPtrN, size2);
                bitmap[pos_now].UnlockBits(bitmapdata);
                small_cur.UnlockBits(outbitmapdata);
                bitmapdata = bitmap[pos_now - 1].LockBits(new Rectangle(0, 0, bitmap[pos_now - 1].Width, bitmap[pos_now - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                outbitmapdata = small_ref.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                intPtrN = outbitmapdata.Scan0;
                intPtr = bitmapdata.Scan0;
                size = bitmapdata.Stride * bitmap[pos_now - 1].Height;
                size2 = outbitmapdata.Stride * small_ref.Height;
                oriBytes = new byte[size];
                newBytes = new byte[size2];
                Marshal.Copy(intPtr, oriBytes, 0, size);
                Marshal.Copy(intPtrN, newBytes, 0, size2);
                k = 3;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        byte p1 = oriBytes[(y + refy) * bitmapdata.Stride + (x + refx) * k];
                        newBytes[(y) * outbitmapdata.Stride + x * k + 2] = (byte)p1;
                        newBytes[(y) * outbitmapdata.Stride + x * k + 1] = (byte)p1;
                        newBytes[(y) * outbitmapdata.Stride + x * k] = (byte)p1;
                        refmatrix[y, x] = p1;
                    }
                }
                Marshal.Copy(newBytes, 0, intPtrN, size2);
                bitmap[pos_now - 1].UnlockBits(bitmapdata);
                small_ref.UnlockBits(outbitmapdata);
                double total = 0;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        total += Math.Abs(refmatrix[y, x] - curmatrix[y, x]);
                    }
                }
                if ((total) < mintotal)
                {
                    mintotal = (double)(total);
                    minx = refx;
                    miny = refy;
                }
                pictureBox1.Image = bitmap[pos_now];
                pictureBox12.Refresh();
                pictureBox12.Image = small_cur;
                pictureBox11.Refresh();
                pictureBox11.Image = small_ref;
                pictureBox3.Image = bitmap[pos_now - 1];
                if (refx + 8 > bitmap[pos_now - 1].Width - 1)
                {

                    if (refy + 8 > bitmap[pos_now - 1].Height - 1)
                    {
                        setmotion(curx, cury, minx, miny);
                        mintotal = double.MaxValue;
                        refy = 0;
                        refx = 0;
                        if (curx + 8 > bitmap[pos_now].Width - 1)
                        {
                            if (cury + 8 > bitmap[pos_now].Height - 1)
                            {
                                cury = 0;
                                curx = 0;
                                if (pos_now + 1 < framecount)
                                {
                                    for (int y = 0; y < bitmap[pos_now].Height; y++)
                                    {
                                        for (int x = 0; x < bitmap[pos_now].Width; x++)
                                        {
                                            motionbitmap.SetPixel(x, y, Color.Black);
                                        }
                                    }
                                }
                                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                                {
                                    d[i] = new drawpoint(-1, -1, -1, -1);
                                }
                                pointcount = 0;
                                pos_now++;

                            }
                            else
                            {
                                cury += 8;
                                curx = 0;
                            }
                        }
                        else
                        {
                            enabledraw(curx,cury,refx,refy);
                            curx += 8;
                        }
                    }
                    else
                    {
                        enabledraw(curx, cury, refx, refy);
                        refy += 1;
                        refx = 0;
                    }
                }
                else
                {
                    enabledraw(curx, cury, refx, refy);
                    refx += 1;
                }
                /*mintotal = int.MaxValue;
                for (cury=0;cury< bitmap[pos_now].Height; cury+=8)
                {
                    for(curx = 0; curx < bitmap[pos_now].Width; curx+=8)
                    {
                        for (refy = 0; refy < bitmap[pos_now].Height; refy+=1)
                        {
                            for (refx = 0; refx < bitmap[pos_now].Width; refx+=1)
                            {
                                Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
                                Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
                                int[,] refmatrix = new int[8, 8];
                                int[,] curmatrix = new int[8, 8];
                                BitmapData bitmapdata = bitmap[pos_now].LockBits(new Rectangle(0, 0, bitmap[pos_now].Width, bitmap[pos_now].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                BitmapData outbitmapdata = small_cur.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                IntPtr intPtrN = outbitmapdata.Scan0;
                                IntPtr intPtr = bitmapdata.Scan0;
                                int size = bitmapdata.Stride * bitmap[pos_now].Height;
                                int size2 = outbitmapdata.Stride * small_cur.Height;
                                byte[] oriBytes = new byte[size];
                                byte[] newBytes = new byte[size2];
                                Marshal.Copy(intPtr, oriBytes, 0, size);
                                Marshal.Copy(intPtrN, newBytes, 0, size2);
                                int k = 3;
                                for (int y = 0; y < 8; y++)
                                {
                                    for (int x = 0; x < 8; x++)
                                    {
                                        byte p1 = oriBytes[(y + cury) * bitmapdata.Stride + (x + curx) * k];
                                        newBytes[(y) * outbitmapdata.Stride + x * k + 2] = (byte)p1;
                                        newBytes[(y) * outbitmapdata.Stride + x * k + 1] = (byte)p1;
                                        newBytes[(y) * outbitmapdata.Stride + x * k] = (byte)p1;
                                        curmatrix[y, x] = p1;

                                    }
                                }
                                Marshal.Copy(newBytes, 0, intPtrN, size2);
                                bitmap[pos_now].UnlockBits(bitmapdata);
                                small_cur.UnlockBits(outbitmapdata);
                                bitmapdata = bitmap[pos_now - 1].LockBits(new Rectangle(0, 0, bitmap[pos_now - 1].Width, bitmap[pos_now - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                outbitmapdata = small_ref.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                intPtrN = outbitmapdata.Scan0;
                                intPtr = bitmapdata.Scan0;
                                size = bitmapdata.Stride * bitmap[pos_now - 1].Height;
                                size2 = outbitmapdata.Stride * small_ref.Height;
                                oriBytes = new byte[size];
                                newBytes = new byte[size2];
                                Marshal.Copy(intPtr, oriBytes, 0, size);
                                Marshal.Copy(intPtrN, newBytes, 0, size2);
                                k = 3;
                                for (int y = 0; y < 8; y++)
                                {
                                    for (int x = 0; x < 8; x++)
                                    {
                                        byte p1 = oriBytes[(y + refy) * bitmapdata.Stride + (x + refx) * k];
                                        newBytes[(y) * outbitmapdata.Stride + x * k + 2] = (byte)p1;
                                        newBytes[(y) * outbitmapdata.Stride + x * k + 1] = (byte)p1;
                                        newBytes[(y) * outbitmapdata.Stride + x * k] = (byte)p1;
                                        refmatrix[y, x] = p1;
                                    }
                                }
                                Marshal.Copy(newBytes, 0, intPtrN, size2);
                                bitmap[pos_now - 1].UnlockBits(bitmapdata);
                                small_ref.UnlockBits(outbitmapdata);
                                int total = 0;
                                for (int y = 0; y < 8; y++)
                                {
                                    for (int x = 0; x < 8; x++)
                                    {
                                        total += Math.Abs(refmatrix[y, x] - curmatrix[y, x]);
                                    }
                                }
                                if (total < mintotal)
                                {
                                    mintotal = total;
                                    minx = refx;
                                    miny = refy;
                                }
                                enabledraw();
                                Console.WriteLine($"{mintotal}, {minx}, {miny}");
                                pictureBox1.Image = bitmap[pos_now];
                                pictureBox12.Refresh();
                                pictureBox12.Image = small_cur;
                                pictureBox11.Refresh();
                                pictureBox11.Image = small_ref;
                                pictureBox3.Image = bitmap[pos_now - 1];
                                
                            }
                        }
                        setmotion(curx, cury, minx, miny);
                        mintotal = int.MaxValue;
                    }
                }
                cury = 0;
                curx = 0;
                if (pos_now + 1 < framecount)
                {
                    for (int y = 0; y < bitmap[pos_now].Height; y++)
                    {
                        for (int x = 0; x < bitmap[pos_now].Width; x++)
                        {
                            motionbitmap.SetPixel(x, y, Color.Black);
                        }
                    }
                }
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                pointcount = 0;
                pos_now++;*/
            }
            else
            {
                if (pos_now >= framecount - 1)
                {
                    pos_now = 0;
                    pictureBox11.Refresh();
                    pictureBox11.Image = bitmap[0];
                    pictureBox3.Refresh();
                    pictureBox3.Image = null;
                    timer2.Enabled = false;
                }
                else
                {

                    pos_now++;
                    pictureBox11.Refresh();
                    pictureBox11.Image = bitmap[pos_now - 1];
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                }

            }
        }
        Bitmap motionbitmap;
        drawpoint[] d;
        private StreamWriter sw;

        private void motionToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            timer1.Enabled = false;
            //timer2.Enabled = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            //full_search();
            /*Thread t = new Thread(new ThreadStart(full_search));
            t.Start();*/
        }
        Bitmap[] newbitmap;
        private void decodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            playstate = "decode";
            algo = "full_search";
            chart1.Visible = false;
            //psnr[0] = 100;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Image = motionbitmap;
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            newbitmap = new Bitmap[framecount];
            for(int i = 0; i < framecount; i++)
            {
                newbitmap[i] = new Bitmap(bitmap[0].Width, bitmap[0].Height, PixelFormat.Format24bppRgb);
            }
            newbitmap[0] = bitmap[0];
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap[0];
            pictureBox3.Refresh();
            pictureBox3.Image = newbitmap[0];
            for (int pos = 1; pos < framecount; pos++)
            {
                string title;
                if (videoname == "6.2")
                {
                    title = "D:\\2022-Image\\6.2safe\\";
                }
                else if (videoname == "6.1")
                {
                    title = "D:\\2022-Image\\6.1safe\\FULL";
                }
                else if (videoname == "6.3")
                {
                    title = "D:\\2022-Image\\6.3safe\\";
                }
                else
                {
                    title = "";
                }
                string name = (pos - 1).ToString();
                string end =".txt";
                string path = title + name + end;
                int nowx = 0, nowy = 0;
                int width = newbitmap[pos - 1].Width;
                int height = newbitmap[pos - 1].Height;
                
                foreach (string line in System.IO.File.ReadLines(path))
                {
                    string[] words = line.Split('(', ',', ')');
                    int motionx = Convert.ToInt16(words[1]), motiony = Convert.ToInt16(words[2]);                   
                    int outx = nowx + motionx;
                    int outy = nowy + motiony;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            newbitmap[pos].SetPixel(x + nowx, y + nowy, newbitmap[pos-1].GetPixel(x + outx, y + outy));
                        }
                    }
                    /*pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos];
                    pictureBox3.Refresh();
                    pictureBox3.Image = decodebitmap[pos];
                    setmotion2(nowx, nowy, outx, outy);*/
                    if (nowx + 8 > width - 1)
                    {
                        if (nowy + 8 > height - 1)
                        {
                            nowx = 0; nowy = 0;
                            //break;
                        }
                        else
                        {
                            nowx = 0; nowy += 8;
                        }
                    }
                    else
                    {
                        nowx += 8;
                    }
                }
                
                /*drawmotion = true;
                pictureBox13.Refresh();
                pictureBox1.Refresh();
                pictureBox1.Image = bitmap[pos];
                pictureBox3.Refresh();
                pictureBox3.Image = newbitmap[pos];
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                pointcount = 0;*/
            }
            /*chart1.Visible = true;
            for (int i = 1; i < framecount; i++)
            {
                psnr[i] = snr_compute(bitmap[i], newbitmap[i]);
            }
            putsnr(psnr, framecount,"full_search");*/
        }
        public Bitmap decode_next_bitmap(Bitmap past_bitmap,long pos)
        {
            string title = "D:\\2022-Image\\";
            string name = (pos - 1).ToString()+pos.ToString();
            string end = ".txt";
            string path = title + name + end;
            int nowx = 0, nowy = 0;
            int width = past_bitmap.Width;
            int height = past_bitmap.Height;
            Bitmap outbitmap = new Bitmap(past_bitmap.Width, past_bitmap.Height, PixelFormat.Format24bppRgb);
            foreach (string line in System.IO.File.ReadLines(path))
            {
                char[] TrimChars = {' '};
                string[] words = line.Split();
                int split_count = 0;
                int motionx = 0, motiony = 0;
                foreach (string word in words)
                {
                    if (split_count == 0)
                    {
                        motionx = int.Parse(word.TrimEnd(TrimChars));
                    }
                    else
                    {
                        motiony = int.Parse(word.TrimEnd(TrimChars));
                    }
                    split_count++;

                }
                int outx = nowx + motionx;
                int outy = nowy + motiony;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        outbitmap.SetPixel(x + nowx, y + nowy, past_bitmap.GetPixel(x + outx, y + outy));
                    }
                }
                /*pictureBox2.Refresh();
                pictureBox2.Image = outbitmap;

                setmotion2(nowx, nowy, outx, outy);*/
                if (nowx + 8 > width - 1)
                {
                    if (nowy + 8 > height - 1)
                    {
                        nowx = 0; nowy = 0;
                        break;
                    }
                    else
                    {
                        nowx = 0; nowy += 8;
                    }
                }
                else
                {
                    nowx += 8;
                }
            }
            //outbitmap.Save(title + name + ".png");
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            pointcount = 0;
            return outbitmap;
        }

        private void full_timer_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            chart1.Visible = false;
            //timer2.Enabled = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            button11.Visible = true;
            button10.Visible = true;
            button9.Visible = true;
        }        
        Bitmap decodebitmap;
        private void threeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            timer1.Enabled = false;
            chart1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            for (pos_now = 1; pos_now < framecount; pos_now++)
            {
                if (pos_now == 1)
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos_now-1];
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    threestep(bitmap[pos_now-1], bitmap[pos_now]);
                }
                else if (pos_now > framecount - 1)
                {
                    pos_now = 0;
                    break;
                }
                else
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = decodebitmap;
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    threestep(decodebitmap, bitmap[pos_now]);
                }
            }
        }
        public void threestep(Bitmap past, Bitmap current)
        {
            int width = past.Width;
            int height = past.Height;
            pictureBox1.Refresh();
            pictureBox1.Image = past;
            pictureBox3.Refresh();
            pictureBox3.Image = current;
            mintotal = int.MaxValue;
            refx = refy = 0;
            Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            for (cury = 0; cury < height; cury += 8)
            {
                for (curx = 0; curx < width; curx += 8)
                {
                    int s = 3;
                    int centerx = curx;
                    int centery = cury;
                    while (s>0)
                    {
                        mintotal = int.MaxValue;
                        
                        for (int disy = -s; disy <= s; disy++)
                        {
                            for (int disx = -s; disx <= s; disx++)
                            {
                                int total = 0;
                                if (centerx + disx > 0 && centery + disy > 0 && centerx + disx < past.Width - 8 && centery + disy < past.Height - 8)
                                {
                                    for (int y = 0; y < 8; y++)
                                    {
                                        for (int x = 0; x < 8; x++)
                                        {
                                            Color past1 = past.GetPixel(centerx + x + disx, centery + y + disy);
                                            Color cur1 = current.GetPixel(curx + x, cury + y);
                                            total += Math.Abs(past1.R - cur1.R);
                                            small_ref.SetPixel(x, y, past1);
                                            small_cur.SetPixel(x, y, cur1);
                                        }
                                    }
                                    if (mintotal > total)
                                    {
                                        mintotal = total;
                                        minx = centerx + disx;
                                        miny = centery + disy;
                                    }
                                    enabledraw(curx, cury, centerx+disx, centery+disy);
                                    pictureBox12.Refresh();
                                    pictureBox12.Image = small_cur;
                                    pictureBox11.Refresh();
                                    pictureBox11.Image = small_ref;
                                }
                            }
                        }
                        centerx = minx;
                        centery = miny;
                        s--;
                        
                    }
                    setmotion(curx, cury, minx, miny);
                }
            }
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            decodebitmap = decode_next_bitmap(past, pos_now);
            cury = 0;
            curx = 0;
            refx = 0;
            refy = 0;
        }

        private void osaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            timer1.Enabled = false;
            chart1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            for (pos_now = 1; pos_now < framecount; pos_now++)
            {
                if (pos_now == 1)
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos_now-1];
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    osa(bitmap[pos_now-1], bitmap[pos_now]);
                }
                else if (pos_now > framecount - 1)
                {
                    pos_now = 0;
                    break;
                }
                else
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = decodebitmap;
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    osa(decodebitmap, bitmap[pos_now]);
                }
            }
        }
        public void osa(Bitmap past,Bitmap current)
        {
            int width = past.Width;
            int height = past.Height;
            pictureBox1.Refresh();
            pictureBox1.Image = past;
            pictureBox3.Refresh();
            pictureBox3.Image = current;
            mintotal = int.MaxValue;
            refx = refy = 0;
            Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            for (cury = 0; cury < height; cury += 8)
            {
                for (curx = 0; curx < width; curx += 8)
                {
                    int s = 4;
                    int centerx = curx;
                    int centery = cury;
                    while (s>=1)
                    {
                        mintotal = int.MaxValue;
                        int total1 = 0;
                        int total2 = 0;
                        int total3 = 0;
                        int total4 = 0;
                        int total5 = 0;

                        //中
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                Color past1 = past.GetPixel(centerx + x, centery + y);
                                Color cur1 = current.GetPixel(curx + x, cury + y);
                                total1 += Math.Abs(past1.R - cur1.R);
                                small_ref.SetPixel(x, y, past1);
                                small_cur.SetPixel(x, y, cur1);
                            }
                        }
                        if (mintotal > total1)
                        {
                            mintotal = total1;
                            minx = centerx;
                            miny = centery;
                        }
                        enabledraw(curx, cury, centerx, centery);
                        pictureBox12.Refresh();
                        pictureBox12.Image = small_cur;
                        pictureBox11.Refresh();
                        pictureBox11.Image = small_ref;
                        //右
                        if (centerx + s >= 0 && centery >= 0 && centerx + s < past.Width - 8 && centery < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x + s, centery + y);
                                    Color cur1 = current.GetPixel(curx + x, cury + y);
                                    total4 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total4)
                            {
                                mintotal = total4;
                                minx = centerx + s;
                                miny = centery;
                            }
                            enabledraw(curx, cury, centerx + s, centery);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        //左
                        if (centerx - s >= 0 && centery >= 0 && centerx - s < past.Width - 8 && centery < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x - s, centery + y);
                                    Color cur1 = current.GetPixel(curx + x, cury + y);
                                    total5 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total5)
                            {
                                mintotal = total5;
                                minx = centerx - s;
                                miny = centery;
                            }
                            enabledraw(curx, cury, centerx - s, centery);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }
                        centerx = minx;
                        centery = miny;
                        //中
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                Color past1 = past.GetPixel(centerx + x, centery + y);
                                Color cur1 = current.GetPixel(curx + x, cury + y);
                                total1 += Math.Abs(past1.R - cur1.R);
                                small_ref.SetPixel(x, y, past1);
                                small_cur.SetPixel(x, y, cur1);
                            }
                        }
                        if (mintotal > total1)
                        {
                            mintotal = total1;
                            minx = centerx;
                            miny = centery;
                        }
                        enabledraw(curx, cury, centerx, centery);
                        pictureBox12.Refresh();
                        pictureBox12.Image = small_cur;
                        pictureBox11.Refresh();
                        pictureBox11.Image = small_ref;
                        //上
                        if (centerx >= 0 && centery - s >= 0 && centerx < past.Width - 8 && centery - s < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x, centery + y - s);
                                    Color cur1 = current.GetPixel(curx + x, cury + y);
                                    total2 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total2)
                            {
                                mintotal = total2;
                                minx = centerx;
                                miny = centery - s;
                            }
                            enabledraw(curx, cury, centerx, centery - s);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        //下
                        if (centerx >= 0 && centery + s >= 0 && centerx < past.Width - 8 && centery + s < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x, centery + y + s);
                                    Color cur1 = current.GetPixel(curx + x, cury + y);
                                    total3 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total3)
                            {
                                mintotal = total3;
                                minx = centerx;
                                miny = centery + s;
                            }
                            enabledraw(curx, cury, centerx, centery + s);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        
                        s /= 2;
                        centerx = minx;
                        centery = miny;
                    }
                    setmotion(curx, cury, minx, miny);
                }
            }
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            decodebitmap = decode_next_bitmap(past, pos_now);
            cury = 0;
            curx = 0;
            refx = 0;
            refy = 0;
        }

        private void decodeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            pos_now = 0;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            playstate = "decode";
            algo = "tdl";
            chart1.Visible = false;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Image = motionbitmap;
            newbitmap = new Bitmap[framecount];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            for (int i = 0; i < framecount; i++)
            {
                newbitmap[i] = new Bitmap(bitmap[0].Width, bitmap[0].Height, PixelFormat.Format24bppRgb);
            }
            psnr[0] = 0;
            newbitmap[0] = bitmap[0];
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap[0];
            pictureBox3.Refresh();
            pictureBox3.Image = newbitmap[0];
            for (int pos = 1; pos < framecount; pos++)
            {
                /*pictureBox1.Refresh();
                pictureBox1.Image = decodebitmap[pos - 1];*/
                string title;
                if (videoname == "6.2")
                {
                    title = "D:\\2022-Image\\6.2safetdl\\";
                }
                else if (videoname == "6.1")
                {
                    title = "D:\\2022-Image\\6.1safetdl\\";
                }
                else if (videoname == "6.3")
                {
                    title = "D:\\2022-Image\\6.3safetdl\\";
                }
                else
                {
                    title = "";
                }
                string name = (pos - 1).ToString() + pos.ToString();
                string end = ".txt";
                string path = title + name + end;
                int nowx = 0, nowy = 0;
                int width = newbitmap[pos - 1].Width;
                int height = newbitmap[pos - 1].Height;

                foreach (string line in System.IO.File.ReadLines(path))
                {
                    char[] TrimChars = { ' ' };
                    string[] words = line.Split();
                    int split_count = 0;
                    int motionx = 0, motiony = 0;
                    foreach (string word in words)
                    {
                        if (split_count == 0)
                        {
                            motionx = int.Parse(word.TrimEnd(TrimChars));
                        }
                        else
                        {
                            motiony = int.Parse(word.TrimEnd(TrimChars));
                        }
                        split_count++;

                    }
                    int outx = nowx + motionx;
                    int outy = nowy + motiony;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            newbitmap[pos].SetPixel(x + nowx, y + nowy, newbitmap[pos - 1].GetPixel(x + outx, y + outy));
                        }
                    }
                    //setmotion2(nowx, nowy, outx, outy);
                    
                    if (nowx + 8 > width - 1)
                    {
                        if (nowy + 8 > height - 1)
                        {
                            nowx = 0; nowy = 0;
                            break;
                        }
                        else
                        {
                            nowx = 0; nowy += 8;
                        }
                    }
                    else
                    {
                        nowx += 8;
                    }
                }
                /*drawmotion = true;
                pictureBox13.Refresh();
                pictureBox1.Refresh();
                pictureBox1.Image = bitmap[pos];
                pictureBox3.Refresh();
                pictureBox3.Image = decodebitmap[pos];
                
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                
                pointcount = 0;*/
            }
            /*chart1.Visible = true;
            for (int i = 1; i < framecount; i++)
            {
                psnr[i] = snr_compute(bitmap[i], decodebitmap[i]);
            }
            putsnr(psnr, framecount,"tdl");*/
        }

        private void decodeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            playstate = "decode";
            algo = "osa";
            chart1.Visible = false;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Image = motionbitmap;
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            psnr[0] = 0;
            newbitmap = new Bitmap[framecount];
            for (int i = 0; i < framecount; i++)
            {
                newbitmap[i] = new Bitmap(bitmap[0].Width, bitmap[0].Height, PixelFormat.Format24bppRgb);
            }
            newbitmap[0] = bitmap[0];
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap[0];
            pictureBox3.Refresh();
            pictureBox3.Image = newbitmap[0];
            for (int pos = 1; pos < framecount; pos++)
            {
                string title;
                if (videoname == "6.2")
                {
                    title = "D:\\2022-Image\\6.2safeosa\\";
                }
                else if (videoname == "6.1")
                {
                    title = "D:\\2022-Image\\6.1safeosa\\";
                }
                else if (videoname == "6.3")
                {
                    title = "D:\\2022-Image\\6.3safeosa\\";
                }
                else
                {
                    title = "";
                }
                string name = (pos - 1).ToString() + pos.ToString();
                string end = ".txt";
                string path = title + name + end;
                int nowx = 0, nowy = 0;
                int width = newbitmap[pos - 1].Width;
                int height = newbitmap[pos - 1].Height;

                foreach (string line in System.IO.File.ReadLines(path))
                {
                    char[] TrimChars = { ' ' };
                    string[] words = line.Split();
                    int split_count = 0;
                    int motionx = 0, motiony = 0;
                    foreach (string word in words)
                    {
                        if (split_count == 0)
                        {
                            motionx = int.Parse(word.TrimEnd(TrimChars));
                        }
                        else
                        {
                            motiony = int.Parse(word.TrimEnd(TrimChars));
                        }
                        split_count++;

                    }
                    int outx = nowx + motionx;
                    int outy = nowy + motiony;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            newbitmap[pos].SetPixel(x + nowx, y + nowy, newbitmap[pos - 1].GetPixel(x + outx, y + outy));
                        }
                    }
                    /*pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos];
                    pictureBox3.Refresh();
                    pictureBox3.Image = decodebitmap[pos];*/
                    //setmotion2(nowx, nowy, outx, outy);
                    if (nowx + 8 > width - 1)
                    {
                        if (nowy + 8 > height - 1)
                        {
                            nowx = 0; nowy = 0;
                            break;
                        }
                        else
                        {
                            nowx = 0; nowy += 8;
                        }
                    }
                    else
                    {
                        nowx += 8;
                    }
                }

                /*drawmotion = true;
                pictureBox13.Refresh();
                pictureBox1.Refresh();
                pictureBox1.Image = bitmap[pos];
                pictureBox3.Refresh();
                pictureBox3.Image = decodebitmap[pos];
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                pointcount = 0;*/
            }
            /*chart1.Visible = true;
            for (int i = 1; i < framecount; i++)
            {
                psnr[i] = snr_compute(bitmap[i], decodebitmap[i]);
            }
            putsnr(psnr, framecount,"osa");*/
        }

        private void decodeToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            playstate = "decode";
            algo = "three";
            chart1.Visible = false;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Image = motionbitmap;
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            newbitmap = new Bitmap[framecount];
            for (int i = 0; i < framecount; i++)
            {
                newbitmap[i] = new Bitmap(bitmap[0].Width, bitmap[0].Height, PixelFormat.Format24bppRgb);
            }
            psnr[0] = 0;
            newbitmap[0] = bitmap[0];
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap[0];
            pictureBox3.Refresh();
            pictureBox3.Image = newbitmap[0];
            for (int pos = 1; pos < framecount; pos++)
            {
                string title;
                if (videoname == "6.2")
                {
                    title = "D:\\2022-Image\\6.2safethreestep\\";
                }
                else if (videoname == "6.1")
                {
                    title = "D:\\2022-Image\\6.1safethreestep\\";
                }
                else if (videoname == "6.3")
                {
                    title = "D:\\2022-Image\\6.3safethreestep\\";
                }
                else
                {
                    title = "";
                }
                string name = (pos - 1).ToString() + pos.ToString();
                string end = ".txt";
                string path = title + name + end;
                int nowx = 0, nowy = 0;
                int width = newbitmap[pos - 1].Width;
                int height = newbitmap[pos - 1].Height;

                foreach (string line in System.IO.File.ReadLines(path))
                {
                    char[] TrimChars = { ' ' };
                    string[] words = line.Split();
                    int split_count = 0;
                    int motionx = 0, motiony = 0;
                    foreach (string word in words)
                    {
                        if (split_count == 0)
                        {
                            motionx = int.Parse(word.TrimEnd(TrimChars));
                        }
                        else
                        {
                            motiony = int.Parse(word.TrimEnd(TrimChars));
                        }
                        split_count++;

                    }
                    int outx = nowx + motionx;
                    int outy = nowy + motiony;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            newbitmap[pos].SetPixel(x + nowx, y + nowy, newbitmap[pos - 1].GetPixel(x + outx, y + outy));
                        }
                    }
                    //setmotion2(nowx, nowy, outx, outy);
                    if (nowx + 8 > width - 1)
                    {
                        if (nowy + 8 > height - 1)
                        {
                            nowx = 0; nowy = 0;
                            break;
                        }
                        else
                        {
                            nowx = 0; nowy += 8;
                        }
                    }
                    else
                    {
                        nowx += 8;
                    }
                }

                /*drawmotion = true;
                pictureBox13.Refresh();
                pictureBox1.Refresh();
                pictureBox1.Image = bitmap[pos];
                pictureBox3.Refresh();
                pictureBox3.Image = decodebitmap[pos];
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                pointcount = 0;*/
            }
            /*chart1.Visible = true;
            for (int i = 1; i < framecount; i++)
            {
                psnr[i] = snr_compute(bitmap[i], decodebitmap[i]);
            }
            putsnr(psnr, framecount,"threestep");*/
        }

        /*private void decode8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            Bitmap[] decodebitmap = new Bitmap[framecount];
            for (int i = 0; i < framecount; i++)
            {
                decodebitmap[i] = new Bitmap(bitmap[0].Width, bitmap[0].Height, PixelFormat.Format24bppRgb);
            }
            psnr[0] = 0;
            decodebitmap[0] = bitmap[0];
            pictureBox1.Refresh();
            pictureBox1.Image = bitmap[0];
            pictureBox3.Refresh();
            pictureBox3.Image = decodebitmap[0];
            for (int pos = 1; pos < framecount; pos++)
            {
                string title = "D:\\2022-Image\\6.1safe\\";
                string name = (pos - 1).ToString() + pos.ToString();
                string end = ".txt";
                string path = title + name + end;
                int nowx = 0, nowy = 0;
                int width = decodebitmap[pos - 1].Width;
                int height = decodebitmap[pos - 1].Height;

                foreach (string line in System.IO.File.ReadLines(path))
                {
                    char[] TrimChars = { ' ' };
                    string[] words = line.Split();
                    int split_count = 0;
                    int motionx = 0, motiony = 0;
                    foreach (string word in words)
                    {
                        if (split_count == 0)
                        {
                            motionx = int.Parse(word.TrimEnd(TrimChars));
                        }
                        else
                        {
                            motiony = int.Parse(word.TrimEnd(TrimChars));
                        }
                        split_count++;

                    }
                    int outx = nowx + motionx;
                    int outy = nowy + motiony;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            decodebitmap[pos].SetPixel(x + nowx, y + nowy, decodebitmap[pos - 1].GetPixel(x + outx, y + outy));
                        }
                    }
                    setmotion2(nowx, nowy, outx, outy);
                    if (nowx + 8 > width - 1)
                    {
                        if (nowy + 8 > height - 1)
                        {
                            nowx = 0; nowy = 0;
                            break;
                        }
                        else
                        {
                            nowx = 0; nowy += 8;
                        }
                    }
                    else
                    {
                        nowx += 8;
                    }
                }

                drawmotion = true;
                pictureBox13.Refresh();
                pictureBox1.Refresh();
                pictureBox1.Image = bitmap[pos];
                pictureBox3.Refresh();
                pictureBox3.Image = decodebitmap[pos];
                for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
                {
                    d[i] = new drawpoint(-1, -1, -1, -1);
                }
                pointcount = 0;
            }
            chart1.Visible = true;
            for (int i = 1; i < framecount; i++)
            {
                psnr[i] = snr_compute(bitmap[i], decodebitmap[i]);
            }
            putsnr(psnr, framecount);
        }*/


        private void runfulldataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //timer2.Enabled = false;
            chart1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            for (pos_now = 1; pos_now < framecount; pos_now++)
            {
                if (pos_now == 1)
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos_now-1];
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    full_search(bitmap[pos_now-1], bitmap[pos_now]);
                }
                else if (pos_now > framecount - 1)
                {
                    pos_now = 0;
                    break;
                }
                else
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = decodebitmap;
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    full_search(decodebitmap, bitmap[pos_now]);
                }
            }
        }
        public void tdl(Bitmap past,Bitmap current)
        {
            int width = past.Width;
            int height = past.Height;
            pictureBox1.Refresh();
            pictureBox1.Image = past;
            pictureBox3.Refresh();
            pictureBox3.Image = current;
            mintotal = int.MaxValue;
            refx = refy = 0;
            Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
            for (cury = 0; cury < height; cury += 8)
            {
                for (curx = 0; curx < width; curx += 8)
                {
                    int s = 4;
                    int centerx = curx;
                    int centery = cury;
                    while (true)
                    {
                        mintotal = int.MaxValue;
                        int total1 = 0;
                        int total2 = 0;
                        int total3 = 0;
                        int total4 = 0;
                        int total5 = 0;

                        //中
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                Color past1 = past.GetPixel(centerx + x, centery + y);
                                Color cur1 = current.GetPixel(curx+x, cury+y);
                                total1 += Math.Abs(past1.R - cur1.R);
                                small_ref.SetPixel(x, y, past1);
                                small_cur.SetPixel(x, y, cur1);
                            }
                        }
                        if (mintotal > total1)
                        {
                            mintotal = total1;
                            minx = centerx;
                            miny = centery;
                        }
                        enabledraw(curx, cury, centerx, centery);
                        pictureBox12.Refresh();
                        pictureBox12.Image = small_cur;
                        pictureBox11.Refresh();
                        pictureBox11.Image = small_ref;
                        //上
                        if (centerx >= 0 && centery - s >= 0 && centerx < past.Width - 8 && centery - s < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x, centery + y - s);
                                    Color cur1 = current.GetPixel(curx+x, cury+y);
                                    total2 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total2)
                            {
                                mintotal = total2;
                                minx = centerx;
                                miny = centery - s;
                            }
                            enabledraw(curx, cury, centerx, centery-s);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        //下
                        if (centerx >= 0 && centery + s >= 0 && centerx < past.Width - 8 && centery + s < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x, centery + y + s);
                                    Color cur1 = current.GetPixel(curx+x, cury+y);
                                    total3 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total3)
                            {
                                mintotal = total3;
                                minx = centerx;
                                miny = centery + s;
                            }
                            enabledraw(curx, cury, centerx, centery+s);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        //右
                        if (centerx + s >= 0 && centery >= 0 && centerx + s < past.Width - 8 && centery < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x + s, centery + y);
                                    Color cur1 = current.GetPixel(curx+x, cury+y);
                                    total4 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total4)
                            {
                                mintotal = total4;
                                minx = centerx + s;
                                miny = centery;
                            }
                            enabledraw(curx, cury, centerx+s, centery);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }

                        //左
                        if (centerx - s >= 0 && centery >= 0 && centerx - s < past.Width - 8 && centery < past.Height - 8)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past1 = past.GetPixel(centerx + x - s, centery + y);
                                    Color cur1 = current.GetPixel(curx+x, cury+y);
                                    total5 += Math.Abs(past1.R - cur1.R);
                                    small_ref.SetPixel(x, y, past1);
                                    small_cur.SetPixel(x, y, cur1);
                                }
                            }
                            if (mintotal > total5)
                            {
                                mintotal = total5;
                                minx = centerx - s;
                                miny = centery;
                            }
                            enabledraw(curx, cury, centerx-s, centery);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }
                        Console.WriteLine($"{minx},{miny},{s}");
                        if (miny == centery && minx == centerx)
                        {
                            s /= 2;

                        }
                        else
                        {
                            centerx = minx; centery = miny;
                        }
                        if (s <= 1)
                        {
                            break;
                        }
                    }
                    if (s == 1)
                    {
                        mintotal = int.MaxValue;
                        for (int disy = -1; disy <= 1; disy++)
                        {
                            for (int disx = -1; disx <= 1; disx++)
                            {
                                int total = 0;
                                if (centerx + disx > 0 && centery + disy > 0 && centerx + disx < past.Width - 8 && centery + disy < past.Height - 8)
                                {
                                    for (int y = 0; y < 8; y++)
                                    {
                                        for (int x = 0; x < 8; x++)
                                        {
                                            Color past1 = past.GetPixel(centerx + x + disx, centery + y + disy);
                                            Color cur1 = current.GetPixel(curx+x, cury+y);
                                            total += Math.Abs(past1.R - cur1.R);
                                            small_ref.SetPixel(x, y, past1);
                                            small_cur.SetPixel(x, y, cur1);
                                        }
                                    }
                                    if (mintotal > total)
                                    {
                                        mintotal = total;
                                        minx = centerx + disx;
                                        miny = centery + disy;
                                    }
                                    enabledraw(curx, cury, centerx+disx, centery+disy);
                                    pictureBox12.Refresh();
                                    pictureBox12.Image = small_cur;
                                    pictureBox11.Refresh();
                                    pictureBox11.Image = small_ref;
                                }
                            }
                        }
                        setmotion(curx, cury, minx, miny);
                    }
                }
            }
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            decodebitmap = decode_next_bitmap(past, pos_now);
            cury = 0;
            curx = 0;
            refx = 0;
            refy = 0;
        }
        private void tDLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos_now = 0;
            timer1.Enabled = false;
            chart1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            fullsearchToolStripMenuItem.Enabled = true;
            motionbitmap = new Bitmap(bitmap[pos_now].Width, bitmap[pos_now].Height, PixelFormat.Format24bppRgb);
            pictureBox13.Refresh();
            pictureBox13.Image = motionbitmap;
            d = new drawpoint[bitmap[pos_now].Width * bitmap[pos_now].Height];
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            for (pos_now = 1; pos_now < framecount; pos_now++)
            {
                if (pos_now == 1)
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = bitmap[pos_now-1];
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now ];
                    tdl(bitmap[pos_now-1], bitmap[pos_now]);
                }
                else if (pos_now > framecount - 1)
                {
                    //pos_now = 0;
                    break;
                }
                else
                {
                    pictureBox1.Refresh();
                    pictureBox1.Image = decodebitmap;
                    pictureBox3.Refresh();
                    pictureBox3.Image = bitmap[pos_now];
                    tdl(decodebitmap, bitmap[pos_now]);
                }
            }
        }

        public void full_search(Bitmap past,Bitmap current)
        {
            int width = past.Width;
            int height = past.Height;
            pictureBox1.Refresh();
            pictureBox1.Image = past;
            pictureBox3.Refresh();
            pictureBox3.Image = current;
            mintotal = int.MaxValue;

            for (cury = 0; cury < height; cury += 8)
            {
                for (curx = 0; curx < width; curx += 8)
                {
                    for (refy = 0; refy <= height - 8; refy += 1)
                    {
                        for (refx = 0; refx <= width - 8; refx += 1)
                        {
                            Bitmap small_ref = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
                            Bitmap small_cur = new Bitmap(8, 8, PixelFormat.Format24bppRgb);
        
                            int total = 0;
                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    Color past_ = past.GetPixel(refx + x, refy + y);
                                    Color cur_ = current.GetPixel(curx + x, cury + y);
                                    total += Math.Abs(past_.R - cur_.R);
                                    small_ref.SetPixel(x, y, past_);
                                    small_cur.SetPixel(x, y, cur_);
                                }
                            }
                            if (total < mintotal)
                            {
                                mintotal = total;
                                minx = refx;
                                miny = refy;
                            }
                            enabledraw(curx,cury,refx,refy);
                            pictureBox12.Refresh();
                            pictureBox12.Image = small_cur;
                            pictureBox11.Refresh();
                            pictureBox11.Image = small_ref;
                        }
                    }
                    setmotion(curx, cury, minx, miny);
                    mintotal = int.MaxValue;
                    refx = 0; refy = 0;

                }
            }
            for (int i = 0; i < bitmap[pos_now].Width * bitmap[pos_now].Height; i++)
            {
                d[i] = new drawpoint(-1, -1, -1, -1);
            }
            decodebitmap = decode_next_bitmap(past, pos_now);
            cury = 0;
            curx = 0;
            refx = 0;
            refy = 0;
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (drawcur)
            {
                
                Pen pen = new Pen(Color.Gold,2);
                e.Graphics.DrawRectangle(pen, new Rectangle(drawx1, drawy1, 8, 8));
                pen.Dispose();
                drawcur =false;
            }
        }
        
        private void pictureBox13_Paint(object sender, PaintEventArgs e)
        {
            if (drawref)
            {
                
                Pen pen = new Pen(Color.LightGoldenrodYellow,2);
                e.Graphics.DrawRectangle(pen, new Rectangle(drawx2, drawy2, 8, 8));
                pen.Dispose();
                drawref = false;
            }
        }
        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            if (drawmotion)
            {
                
                for (int i = 0; i < pointcount; i++)
                {
                    Pen pen;
                    SolidBrush Brush = new SolidBrush(Color.LightGray); ;
                    if (i % 11 == 0)
                    {
                        pen = new Pen(Color.Red, 1);
                    }
                    else if (i % 11 == 1)
                    {
                        pen = new Pen(Color.Orange, 1);
                    }
                    else if (i % 11 == 2)
                    {
                        pen = new Pen(Color.Yellow, 1);
                    }
                    else if (i % 11 == 3)
                    {
                        pen = new Pen(Color.Green, 1);
                    }
                    else if (i % 11 == 4)
                    {
                        pen = new Pen(Color.Blue, 1);
                    }
                    else if (i % 11 == 5)
                    {
                        pen = new Pen(Color.Cyan, 1);
                    }
                    else if (i % 11 == 6)
                    {
                        pen = new Pen(Color.Purple, 1);
                    }
                    else if (i % 11 == 7)
                    {
                        pen = new Pen(Color.Chocolate, 1);
                    }
                    else if (i % 11 == 8)
                    {
                        pen = new Pen(Color.LightSeaGreen, 1);
                    }
                    else if (i % 11 == 9)
                    {
                        pen = new Pen(Color.Gold, 1);
                    }
                    else
                    {
                        pen = new Pen(Color.Salmon, 1);
                    }
                    pen.StartCap = LineCap.NoAnchor;
                    pen.EndCap = LineCap.ArrowAnchor;
                    if (d[i].startx == d[i].endx && d[i].starty == d[i].endy)
                    {
                        e.Graphics.FillRectangle(Brush, new Rectangle(d[i].startx+3, d[i].starty+3, 2, 2));
                        
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, d[i].startx+3, d[i].starty+3, d[i].endx+3, d[i].endy+3);
                    }
                    pen.Dispose();
                }
                drawmotion = false;
            }
        }
    }
}
