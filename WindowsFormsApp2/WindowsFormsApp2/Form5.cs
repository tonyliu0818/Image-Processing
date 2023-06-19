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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        public Bitmap bbb,orginal, out0, out1, out2, out3, out4, out5, out6, out7, out8, out9,water,out_,outbitmap;

        int enter = 0;
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out0);
                label15.Text = "SNR:bit0-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit0";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg2 > 128 ? 1 : 0;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out0;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 0;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out1);
                label15.Text = "SNR:bit1-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit1";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg2 > 128 ? 2 : 0;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out0;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 1;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out2);
                label15.Text = "SNR:bit2-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit2";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg2 > 128 ? 4 : 0;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out0;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 2;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out3);
                label15.Text = "SNR:bit3-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit3";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg2 > 128 ? 8 : 0;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out0;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 3;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out4);
                label15.Text = "SNR:bit4-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit4";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg2 > 128 ? 16 : 0;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out0;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 4;
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out5);
                label15.Text = "SNR:bit5-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit5";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg2 > 128 ? 32 : 0;
                        int l6 = avg1 & 64;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out0;
                pictureBox7.Image = out6;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 5;
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out6);
                label15.Text = "SNR:bit6-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit6";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg2 > 128 ? 64 : 0;
                        int l7 = avg1 & 128;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out0;
                pictureBox8.Image = out7;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 6;
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            /*if (label11.Text == "graycode")
            {
                snr_(out_, out7);
                label15.Text = "SNR:bit7-orginal";
            }*/
            if (label11.Text == "watermarketing")
            {
                label9.Visible = true;
                label15.Text = "浮水印放的位置為bit7";
                outbitmap = new Bitmap(orginal.Width, orginal.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < orginal.Height; y++)
                {
                    for (int x = 0; x < orginal.Width; x++)
                    {
                        Color c1 = orginal.GetPixel(x, y);
                        Color c2 = water.GetPixel(x, y);
                        int avg1 = (c1.R + c1.G + c1.B) / 3;
                        int avg2 = (c2.R + c2.G + c2.B) / 3;
                        int l0 = avg1 & 1;
                        int l1 = avg1 & 2;
                        int l2 = avg1 & 4;
                        int l3 = avg1 & 8;
                        int l4 = avg1 & 16;
                        int l5 = avg1 & 32;
                        int l6 = avg1 & 64;
                        int l7 = avg2 > 128 ? 128 : 0;
                        int total = l7 + l6 + l5 + l4 + l3 + l2 + l1 + l0;
                        outbitmap.SetPixel(x, y, Color.FromArgb(total, total, total));
                    }
                }
                pictureBox12.Image = out9;
                pictureBox2.Image = out1;
                pictureBox3.Image = out2;
                pictureBox4.Image = out3;
                pictureBox5.Image = out4;
                pictureBox6.Image = out5;
                pictureBox7.Image = out6;
                pictureBox8.Image = out0;
                pictureBox9.Image = outbitmap;
                snr_(orginal, outbitmap);
                enter = 7;
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
                label14.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                label14.Text = snr.ToString() + "db";
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
                label14.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)10 * Math.Log10(outcount / difference);
                snr = Math.Round(snr, 2);
                label14.Text = snr.ToString() + "db";
            }
        }
        public void split(Bitmap bitmap1,Bitmap bitmap2,int state)
        {
            if (bitmap1.Width != bitmap2.Width)
            {
                
                this.Hide();return;
            }
            label1.Visible = true;
            label9.Visible = true;
            label15.Visible = true;
            label11.Text = "watermarketing";
            out0 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out1 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out2 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out3 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out4 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out5 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out6 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out7 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bbb = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out8 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out9 = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            long outcount = 0;
            long originalcount = 0;
            Bitmap black = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for(int y = 0; y < bitmap2.Height; y++)
            {
                for(int x = 0; x < bitmap2.Width; x++)
                {
                    Color c = bitmap2.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    if (avg > 128)
                    {
                        black.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        black.SetPixel(x, y, Color.Black);
                    }
                }
            }
            for (int y = 0; y < bitmap1.Height; y++)
            {
                for(int x = 0; x < bitmap1.Width; x++)
                {
                    Color buff = bitmap1.GetPixel(x, y);
                    Color buff2 = black.GetPixel(x, y);
                    //原圖灰階
                    int avg = (buff.R + buff.G + buff.B) / 3;
                    //浮水印黑白
                    int avg2 = (buff2.R + buff2.G + buff2.B) / 3;
                    //原圖p0
                    int p1 = avg & 1;
                    int l1 = p1 != 0 ? 255 : 0;
                    //原圖p1
                    int p2 = avg & 2;
                    int l2 = p2 != 0 ? 255 : 0;
                    //原圖p2
                    int p3 = avg & 4;
                    int l3 = p3 != 0 ? 255 : 0;
                    //原圖p3
                    int p4 = avg & 8;
                    int l4 = p4 != 0 ? 255 : 0;
                    //原圖p4
                    int p5 = avg & 16;
                    int l5 = p5 != 0 ? 255 : 0;
                    //原圖p5
                    int p6 = avg & 32;
                    int l6 = p6 != 0 ? 255 : 0;
                    //原圖p6
                    int p7 = avg & 64;
                    int l7 = p7 != 0 ? 255 : 0;
                    //原圖p7
                    int p8 = avg & 128;
                    int l8 = p8 != 0 ? 255 : 0;
                    ////浮水印p0
                    //int lnew = avg2 != 0 ? 1 : 0;
                    //浮水印取代原圖p0
                    //int total = lnew + p8 + p7 + p6 + p5 + p4 + p3 + p2;
                    out0.SetPixel(x, y, Color.FromArgb(avg2,avg2,avg2));
                    out1.SetPixel(x, y, Color.FromArgb(l2, l2, l2));
                    out2.SetPixel(x, y, Color.FromArgb(l3, l3, l3));
                    out3.SetPixel(x, y, Color.FromArgb(l4, l4, l4));
                    out4.SetPixel(x, y, Color.FromArgb(l5, l5, l5));
                    out5.SetPixel(x, y, Color.FromArgb(l6, l6, l6));
                    out6.SetPixel(x, y, Color.FromArgb(l7, l7, l7));
                    out7.SetPixel(x, y, Color.FromArgb(l8, l8, l8));
                    //bbb.SetPixel(x, y, Color.FromArgb(total, total, total));
                    out8.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                    out9.SetPixel(x, y, Color.FromArgb(l1, l1, l1));
                    //originalcount += avg;
                    //outcount += total;
                }
            }
            pictureBox1.Image = out0;
            pictureBox2.Image = out1;
            pictureBox3.Image = out2;
            pictureBox4.Image = out3;
            pictureBox5.Image = out4;
            pictureBox6.Image = out5;
            pictureBox7.Image = out6;
            pictureBox8.Image = out7;
            //pictureBox9.Image = bbb;
            pictureBox10.Image = out8;
            pictureBox12.Image = out9;
            label9.Visible = false;
            
            //snr
            /*double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - originalcount , 2)));
            snr = Math.Round(snr, 2);
            label14.Text = snr.ToString()+"db";*/
            orginal = new Bitmap(bitmap1.Width, bitmap1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap1.Height; y++)
            {
                for (int x = 0; x < bitmap1.Width; x++)
                {
                    orginal.SetPixel(x, y, bitmap1.GetPixel(x, y));
                }
            }
            water = new Bitmap(bitmap2.Width, bitmap2.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bitmap2.Height; y++)
            {
                for (int x = 0; x < bitmap2.Width; x++)
                {
                    water.SetPixel(x, y, bitmap2.GetPixel(x, y));
                }
            }
        }
        public void gray(Bitmap src_bitmap)
        {
            label1.Visible = false;
            label9.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = true;
            label11.Text = "graycode";
            out0 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out1 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out2 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out3 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out4 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out5 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out6 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out7 = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            out_ = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color buff = src_bitmap.GetPixel(x, y);
                    //原圖灰階
                    int avg = (buff.R + buff.G + buff.B) / 3;
                    //原圖p0
                    int p0 = avg & 1;
                    int l0 = p0 != 0 ? 255 : 0;
                    //原圖p1
                    int p1 = avg & 2;
                    int l1 = p1 != 0 ? 255 : 0;
                    //原圖p2
                    int p2 = avg & 4;
                    int l2 = p2 != 0 ? 255 : 0;
                    //原圖p3
                    int p3 = avg & 8;
                    int l3 = p3 != 0 ? 255 : 0;
                    //原圖p4
                    int p4 = avg & 16;
                    int l4 = p4 != 0 ? 255 : 0;
                    //原圖p5
                    int p5 = avg & 32;
                    int l5 = p5 != 0 ? 255 : 0;
                    //原圖p6
                    int p6 = avg & 64;
                    int l6 = p6 != 0 ? 255 : 0;
                    //原圖p7
                    int p7 = avg & 128;
                    int l7 = p7 != 0 ? 255 : 0;
                    int g7_ = p7;
                    int g7 = g7_ != 0 ? 255 : 0;
                    int g6_ = l7 ^ l6;
                    int g6 = g6_ != 0 ? 255 : 0;
                    int g5_ = l6 ^ l5;
                    int g5 = g5_ != 0 ? 255 : 0;
                    int g4_ = l5 ^ l4;
                    int g4 = g4_ != 0 ? 255 : 0;
                    int g3_ = l4 ^ l3;
                    int g3 = g3_ != 0 ? 255 : 0;
                    int g2_ = l3 ^ l2;
                    int g2 = g2_ != 0 ? 255 : 0;
                    int g1_ = l2 ^ l1;
                    int g1 = g1_ != 0 ? 255 : 0;
                    int g0_ = l1 ^ l0;
                    int g0 = g0_ != 0 ? 255 : 0;
                    out0.SetPixel(x, y, Color.FromArgb(g0, g0, g0));
                    out1.SetPixel(x, y, Color.FromArgb(g1, g1, g1));
                    out2.SetPixel(x, y, Color.FromArgb(g2, g2, g2));
                    out3.SetPixel(x, y, Color.FromArgb(g3, g3, g3));
                    out4.SetPixel(x, y, Color.FromArgb(g4, g4, g4));
                    out5.SetPixel(x, y, Color.FromArgb(g5, g5, g5));
                    out6.SetPixel(x, y, Color.FromArgb(g6, g6, g6));
                    out7.SetPixel(x, y, Color.FromArgb(g7, g7, g7));
                    out_.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            pictureBox12.Image = out0;
            pictureBox2.Image = out1;
            pictureBox3.Image = out2;
            pictureBox4.Image = out3;
            pictureBox5.Image = out4;
            pictureBox6.Image = out5;
            pictureBox7.Image = out6;
            pictureBox8.Image = out7;
            pictureBox10.Image = out_;
        }
       

        
    }
}
