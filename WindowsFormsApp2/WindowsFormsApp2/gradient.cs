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
    public partial class gradient : Form
    {
        public gradient()
        {
            InitializeComponent();
        }
        public Bitmap src_bitmap, outbitmap;
        Bitmap xbitmap, ybitmap;
        public void sobel(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = buffbitmap;
            label2.Text = "sobel";
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            xbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ybitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    //x
                    int[] kernely = new int[9] { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
                    int[] kernelx = new int[9] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
                    int count = 0;
                    int sumy = 0, sumx = 0;
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumy += 0;
                                sumx += 0;
                            }
                            else
                            {
                                Color c = src_bitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                sumy += (avg*kernely[count]);
                                sumx += (avg*kernelx[count]);

                            }
                            count++;
                        }
                    }
                    sumx = (int)(Math.Abs(sumx));
                    sumy = (int)(Math.Abs(sumy));
                    int g = sumx + sumy;
                    if (sumy > 255)
                    {
                        sumy = 255;
                    }
                    else if (sumy < 0)
                    {
                        sumy = 0;
                    }
                    if (sumx > 255)
                    {
                        sumx = 255;
                    }
                    else if (sumx < 0)
                    {
                        sumx = 0;
                    }
                    if (g > 255)
                    {
                        g = 255;
                    }
                    ybitmap.SetPixel(x, y, Color.FromArgb(sumy, sumy, sumy));
                    xbitmap.SetPixel(x, y, Color.FromArgb(sumx, sumx, sumx));
                    outbitmap.SetPixel(x, y, Color.FromArgb(g, g, g));
                }
            }
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pictureBox16.Visible = true;
            pictureBox17.Visible = true;
            pictureBox2.Image = xbitmap;
            pictureBox3.Image = ybitmap;
            pictureBox4.Image = outbitmap;
            snr1_(src_bitmap, xbitmap);
            snr2_(src_bitmap, ybitmap);
            snr3_(src_bitmap, outbitmap);
        }
        public void prewitt(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = buffbitmap;
            label2.Text = "prewitt";
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            xbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ybitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    //x
                    int[] kernely = new int[9] { 1, 1, 1, 0, 0, 0, -1, -1, -1 };
                    int[] kernelx = new int[9] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                    int count = 0;
                    int sumy = 0, sumx = 0;
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumy += 0;
                                sumx += 0;
                            }
                            else
                            {
                                Color c = src_bitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                sumy += (avg * kernely[count]);
                                sumx += (avg * kernelx[count]);

                            }
                            count++;
                        }
                    }
                    sumx = (int)(Math.Abs(sumx));
                    sumy = (int)(Math.Abs(sumy));
                    int g = sumx + sumy;
                    if (sumy > 255)
                    {
                        sumy = 255;
                    }
                    else if (sumy < 0)
                    {
                        sumy = 0;
                    }
                    if (sumx > 255)
                    {
                        sumx = 255;
                    }
                    else if (sumx < 0)
                    {
                        sumx = 0;
                    }
                    if (g > 255)
                    {
                        g = 255;
                    }
                    ybitmap.SetPixel(x, y, Color.FromArgb(sumy, sumy, sumy));
                    xbitmap.SetPixel(x, y, Color.FromArgb(sumx, sumx, sumx));
                    outbitmap.SetPixel(x, y, Color.FromArgb(g, g, g));
                }
            }
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pictureBox18.Visible = true;
            pictureBox19.Visible = true;
            pictureBox2.Image = xbitmap;
            pictureBox3.Image = ybitmap;
            pictureBox4.Image = outbitmap;
            snr1_(src_bitmap, xbitmap);
            snr2_(src_bitmap, ybitmap);
            snr3_(src_bitmap, outbitmap);
        }
        public void robert(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label2.Text = "robert";
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            xbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ybitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    //x
                    int[] kernely = new int[4] { 0,1,-1,0 };
                    int[] kernelx = new int[4] { 1,0,0,-1 };
                    int count = 0;
                    int sumy = 0, sumx = 0;
                    for (int j = 0; j <2; j++)
                    {
                        for (int i = 0; i <2; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumy += 0;
                                sumx += 0;
                            }
                            else
                            {
                                Color c = buffbitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                sumy += (avg * kernely[count]);
                                sumx += (avg * kernelx[count]);

                            }
                            count++;
                        }
                    }
                    sumx = (int)(Math.Abs(sumx));
                    sumy = (int)(Math.Abs(sumy));
                    int g = sumx + sumy;
                    if (sumy > 255)
                    {
                        sumy = 255;
                    }
                    else if (sumy < 0)
                    {
                        sumy = 0;
                    }
                    if (sumx > 255)
                    {
                        sumx = 255;
                    }
                    else if (sumx < 0)
                    {
                        sumx = 0;
                    }
                    if (g > 255)
                    {
                        g = 255;
                    }
                    ybitmap.SetPixel(x, y, Color.FromArgb(sumy, sumy, sumy));
                    xbitmap.SetPixel(x, y, Color.FromArgb(sumx, sumx, sumx));
                    outbitmap.SetPixel(x, y, Color.FromArgb(g, g, g));
                }
            }
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            pictureBox14.Visible = true;
            pictureBox15.Visible = true;
            label3.Text = "GX";
            label4.Text = "GY";
            pictureBox2.Image = xbitmap;
            pictureBox3.Image = ybitmap;
            pictureBox4.Image = outbitmap;
            snr1_(src_bitmap, xbitmap);
            snr2_(src_bitmap, ybitmap);
            snr3_(src_bitmap, outbitmap);
        }
        public void crispening(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label2.Text = "crispening";
            label3.Text = "Mask1";
            label4.Text = "Mask2";
            label5.Text = "Mask3";
            toolStripStatusLabel12.Text = "Mask1";
            toolStripStatusLabel25.Text = "Mask2";
            toolStripStatusLabel38.Text = "Mask3";
            pictureBox11.Visible = true;
            pictureBox12.Visible = true;
            pictureBox13.Visible = true;
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            xbitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ybitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            outbitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for(int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    int total1 = 0;
                    int total2 = 0;
                    int total3 = 0;
                    int count = 0;
                    int[] mask1 = new int[9] { 0, -1, 0, -1, 5, -1, 0, -1, 0 };
                    int[] mask2 = new int[9] { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
                    int[] mask3 = new int[9] { 1, -2, 1, -2, 5, -2, 1, -2, 1 };
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                total1 += 0;
                                total2 += 0;
                                total3 += 0;
                            }
                            else
                            {
                                Color c = buffbitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                total1 += (avg * mask1[count]);
                                total2 += (avg * mask2[count]);
                                total3 += (avg * mask3[count]);

                            }
                            count++;
                        }
                    }
                    if (total1 < 0) total1 = 0;
                    else if (total1 > 255) total1 = 255;
                    if (total2 < 0) total2 = 0;
                    else if (total2 > 255) total2 = 255;
                    if (total3 < 0) total3 = 0;
                    else if (total3 > 255) total3 = 255;
                    xbitmap.SetPixel(x, y, Color.FromArgb(total1, total1, total1));
                    ybitmap.SetPixel(x, y, Color.FromArgb(total2, total2, total2));
                    outbitmap.SetPixel(x, y, Color.FromArgb(total3, total3, total3));
                }
            }
            pictureBox2.Image = xbitmap;
            pictureBox3.Image = ybitmap;
            pictureBox4.Image = outbitmap;
        }
        public void laplacian(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label2.Text = "variant_laplacian";
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            label3.Visible = false;
            label4.Visible = false;
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    //x
                    int[] kernely = new int[9] { 1,1,1,1,-8,1,1,1,1 };
                    int count = 0;
                    int sumy = 0;
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumy += 0;
                            }
                            else
                            {
                                Color c = src_bitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                sumy += (avg * kernely[count]);

                            }
                            count++;
                        }
                    }
                    if (sumy > 255)
                    {
                        sumy = 255;
                    }
                    else if (sumy < 0)
                    {
                        sumy = 0;
                    }

                    outbitmap.SetPixel(x, y, Color.FromArgb(sumy, sumy, sumy));
                    
                }
            }
            statusStrip2.Visible = false;
            statusStrip3.Visible = false;
            pictureBox4.Image = outbitmap;
            snr3_(src_bitmap, outbitmap);
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
            if (xbitmap != null)
            {
                if ((e.X < xbitmap.Width) && (e.Y < xbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel14.Text = e.X.ToString();
                    toolStripStatusLabel16.Text = e.Y.ToString();
                    toolStripStatusLabel18.Text = xbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel20.Text = xbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel22.Text = xbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                
            }
        }
        private void p3_MouseMove(object sender, MouseEventArgs e)
        {
            if (ybitmap != null)
            {
                if ((e.X < ybitmap.Width) && (e.Y < ybitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel27.Text = e.X.ToString();
                    toolStripStatusLabel29.Text = e.Y.ToString();
                    toolStripStatusLabel31.Text = ybitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel33.Text = ybitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel35.Text = ybitmap.GetPixel(e.X, e.Y).B.ToString();
                }
            }

        }
        private void out_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel40.Text = e.X.ToString();
                    toolStripStatusLabel42.Text = e.Y.ToString();
                    toolStripStatusLabel44.Text = outbitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel46.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel48.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                
            }
        }
        public void snr1_(Bitmap bitmap1, Bitmap bitmap2)
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
        public void snr2_(Bitmap bitmap1, Bitmap bitmap2)
        {
            long inputcount = 0;
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
                toolStripStatusLabel37.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                toolStripStatusLabel37.Text = snr.ToString() + "db";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(xbitmap);
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(ybitmap);
            this.Hide();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(xbitmap);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(ybitmap);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        public void snr3_(Bitmap bitmap1, Bitmap bitmap2)
        {
            long inputcount = 0;
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
                toolStripStatusLabel50.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)(10 * Math.Log10(Math.Pow(outcount, 2) / Math.Pow(outcount - inputcount, 2)));
                snr = Math.Round(snr, 2);
                toolStripStatusLabel50.Text = snr.ToString() + "db";
            }
        }
        public void simple(Bitmap buffbitmap)
        {
            src_bitmap = new Bitmap(buffbitmap.Width, buffbitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < buffbitmap.Height; y++)
            {
                for (int x = 0; x < buffbitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label2.Text = "simple_laplacian";
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            label3.Visible = false;
            label4.Visible = false;
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    //x
                    int[] kernely = new int[9] { 0, 1, 0, 1, -4, 1, 0, 1, 0 };
                    int count = 0;
                    int sumy = 0;
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i < 0 || y + j < 0 || x + i > src_bitmap.Width - 1 || y + j > src_bitmap.Height - 1)
                            {
                                sumy += 0;
                            }
                            else
                            {
                                Color c = src_bitmap.GetPixel(x + i, y + j);
                                int avg = (c.R + c.G + c.B) / 3;
                                sumy += (avg * kernely[count]);

                            }
                            count++;
                        }
                    }
                    if (sumy > 255)
                    {
                        sumy = 255;
                    }
                    else if (sumy < 0)
                    {
                        sumy = 0;
                    }

                    outbitmap.SetPixel(x, y, Color.FromArgb(sumy, sumy, sumy));

                }
            }
            statusStrip2.Visible = false;
            statusStrip3.Visible = false;
            pictureBox4.Image = outbitmap;
            snr3_(src_bitmap, outbitmap);
        }
    }
}
