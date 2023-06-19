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
    public partial class offsetp : Form
    {
        public offsetp()
        {
            InitializeComponent();
        }
        public Bitmap src_bitmap,outbitmap;
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
        public void offset(Bitmap bitmap)
        {
            set(bitmap, "offset", 255, 0);
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (state == "offset")
            {
                int Roffset = trackBar1.Value;
                int Goffset = trackBar2.Value;
                int Boffset = trackBar3.Value;
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        Color buffr, buffg, buffb;
                        if (x < Roffset)
                        {
                            buffr = src_bitmap.GetPixel(src_bitmap.Width - x-1, y);
                        }
                        else
                        {
                            buffr = src_bitmap.GetPixel(x - Roffset, y);
                        }
                        if (x < Goffset)
                        {
                            buffg = src_bitmap.GetPixel(src_bitmap.Width - x-1, y);
                        }
                        else
                        {
                            buffg = src_bitmap.GetPixel(x - Goffset, y);
                        }
                        if (x < Boffset)
                        {
                            buffb = src_bitmap.GetPixel(src_bitmap.Width - x-1, y);
                        }
                        else
                        {
                            buffb = src_bitmap.GetPixel(x - Boffset, y);
                        }
                        outbitmap.SetPixel(x, y, Color.FromArgb(buffr.R, buffg.G, buffb.B));
                    }
                }
                pictureBox2.Image = outbitmap;
                snr_(src_bitmap, outbitmap);
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar1.Value;
            label1.Text = t1.ToString();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar2.Value ;
            label2.Text = t1.ToString();
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            double t1 = (double)trackBar3.Value;
            label3.Text = t1.ToString();
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

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }
    }
}
