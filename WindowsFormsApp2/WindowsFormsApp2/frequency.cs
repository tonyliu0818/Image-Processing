using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
namespace WindowsFormsApp2
{
    public partial class frequency : Form
    {
        public frequency()
        {
            InitializeComponent();
        }
        public double fourier(int v, int u,Bitmap src_bitmap)
        {
            double cossum = 0;
            double sinsum = 0;
            BitmapData bitmapdata = src_bitmap.LockBits(new Rectangle(0, 0, src_bitmap.Width, src_bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = bitmapdata.Scan0;
            int size2 = bitmapdata.Stride * src_bitmap.Height;
            byte[] oriBytes = new byte[size2];
            System.Runtime.InteropServices.Marshal.Copy(intPtr, oriBytes, 0, size2);
            int k = 3;
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    /*Color c = src_bitmap.GetPixel(x, y);
                    byte avg = (byte)((c.R + c.G + c.B) / 3);*/
                    byte r = oriBytes[(y) * bitmapdata.Stride + (x) * k];
                    byte g = oriBytes[(y) * bitmapdata.Stride + (x) * k + 1];
                    byte b = oriBytes[(y) * bitmapdata.Stride + (x) * k + 2];
                    int avg = (r + g + b) / 3;
                    cossum += avg * Math.Cos(2 * -1 * Math.PI * (y * v + x * u) / 256);
                    sinsum += avg * Math.Sin(2 * -1 * Math.PI * (y * v + x * u) / 256);
                }
            }
            src_bitmap.UnlockBits(bitmapdata);
            double size = 256 * 256;
            double total = (Math.Pow(Math.Pow(cossum, 2) + Math.Pow(sinsum, 2), 0.5));
            return total>size?255:total / 256;
        }
        public void decode(Bitmap src_bitmap)
        {
            BitmapData bitmapdata = src_bitmap.LockBits(new Rectangle(0, 0, src_bitmap.Width, src_bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = bitmapdata.Scan0;
            int size2 = bitmapdata.Stride * src_bitmap.Height;
            byte[] oriBytes = new byte[size2];
            System.Runtime.InteropServices.Marshal.Copy(intPtr, oriBytes, 0, size2);
            int k = 3;
            int[,] data = new int[256, 256];
            for(int y = 0; y < 256; y++)
            {
                for(int x = 0; x < 256; x++)
                {
                    byte r = oriBytes[(y) * bitmapdata.Stride + (x) * k];
                    byte g = oriBytes[(y) * bitmapdata.Stride + (x) * k + 1];
                    byte b = oriBytes[(y) * bitmapdata.Stride + (x) * k + 2];
                    int avg = (r + g + b) / 3;
                    data[y, x] = avg;
                }
            }
            src_bitmap.UnlockBits(bitmapdata);
            pictureBox1.Image = src_bitmap;
            double[,] F = new double[256, 256];
            double[,] F2 = new double[256, 256];
            double[,] F3 = new double[256, 256];
            for (int v = -128; v < 128; v++)
            {
                for (int u = -128; u < 128; u++)
                {
                    double cossum = 0;
                    double sinsum = 0;
                    
                    for (int y = 0; y < 256; y++)
                    {
                        for (int x = 0; x < 256; x++)
                        {
                            /*Color c = src_bitmap.GetPixel(x, y);
                            byte avg = (byte)((c.R + c.G + c.B) / 3);*/
                            //real
                            cossum += data[y,x] * Math.Cos(2 * -1 * Math.PI * (x * u + y * v) / 256);
                            //complex
                            sinsum += data[y,x] * Math.Sin(2 * -1 * Math.PI * (x * u + y * v) / 256);
                        }
                    }
                    
                    double size = 256 * 256;
                    double total = (Math.Abs(cossum));
                    F[v+128, u+128] = total>size?255:Math.Round(total/256);
                    Console.WriteLine($"{u},{v},{F[v+128, u+128]}");
                    double b = Math.Pow(Math.Pow(cossum, 2) + Math.Pow(sinsum, 2), 0.5);
                    F2[v+128, u+128] = cossum;
                    F3[v+128, u+128] = sinsum;
                }
            }
            
            double[,] highpassR = new double[256, 256];
            double[,] lowpassR = new double[256, 256];
            double[,] highpassI = new double[256, 256];
            double[,] lowpassI = new double[256, 256];
            double[,] bandpassR = new double[256, 256];
            double[,] bandpassI = new double[256, 256];
            Bitmap outbitmap = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap outbitmap2 = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap outbitmap3 = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap outbitmap4 = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {

                    if (F[y, x] < 256)
                    {
                        Byte pixel = (Byte)(F[y, x]);
                        outbitmap.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                    if ((Math.Pow(x-128, 2) + Math.Pow(y-128, 2) > Math.Pow(20, 2) && x > 0 && y > 0))
                    {
                        Byte pixel = (Byte)(F[y, x]);
                        outbitmap2.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                        outbitmap3.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        highpassR[y, x] = 0;
                        lowpassR[y, x] = F2[y, x];
                        highpassI[y, x] = 0;
                        lowpassI[y, x] = F3[y, x];
                    }
                    else
                    {
                        Byte pixel = (Byte)(F[y, x]);
                        outbitmap2.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        outbitmap3.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                        highpassR[y, x] = F2[y, x];
                        lowpassR[y, x] = 0;
                        highpassI[y, x] = F3[y, x];
                        lowpassI[y, x] = 0;
                    }
                    if ((Math.Pow(x - 128, 2) + Math.Pow(y - 128, 2) < Math.Pow(30, 2))&& (Math.Pow(x - 128, 2) + Math.Pow(y - 128, 2) > Math.Pow(10, 2)))
                    {
                        Byte pixel = (Byte)(F[y, x]);
                        outbitmap4.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                        bandpassR[y, x] = F2[y, x];
                        bandpassI[y, x] = F3[y, x];
                    }
                    else
                    {
                        Byte pixel = (Byte)(F[y, x]);
                        outbitmap4.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        bandpassR[y, x] = 0;
                        bandpassI[y, x] = 0;
                    }
                    
                   

                }
            }
            pictureBox2.Image = outbitmap;
            pictureBox4.Image = outbitmap2;
            pictureBox6.Image = outbitmap3;
            pictureBox8.Image = outbitmap3;
            Bitmap low = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap high = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap band = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < 256; y++)
            {
                for(int x = 0; x < 256; x++)
                {
                    double cossum1 = 0;
                    double sinsum1 = 0;
                    double cossum2 = 0;
                    double cossum3 = 0;
                    double sinsum2 = 0;
                    for (int v = -128; v < 128; v++)
                    {
                        for(int u = -128; u < 128; u++)
                        {
                            cossum1 += (highpassR[v+128, u+128] * Math.Cos(2 * Math.PI * (x * (u) + y * (v)) / 256))-(highpassI[v+128, u+128] * Math.Sin(2 * Math.PI * (x * (u) + y * (v)) / 256));
                            //sinsum1 += highpassR[v, u] * Math.Sin(2 * Math.PI * (x * (u) + y * (v)) / 256);
                            cossum2 += (lowpassR[v+128, u+128] * Math.Cos(2 * Math.PI * (x * (u) + y * (v)) / 256))-(lowpassI[v+128, u+128] * Math.Sin(2 * Math.PI * (x * (u) + y * (v)) / 256));
                            cossum3 += (bandpassR[v + 128, u + 128] * Math.Cos(2 * Math.PI * (x * (u) + y * (v)) / 256)) - (bandpassI[v + 128, u + 128] * Math.Sin(2 * Math.PI * (x * (u) + y * (v)) / 256));
                            //sinsum2 += lowpassR[v, u] * Math.Sin(2 * Math.PI * (x * (v) + y * (u)) / 256);
                        }
                        
                    }
                    double size = 256 * 256;
                    double total1 = Math.Abs(cossum1) / size;
                    if (total1 > 255)
                    {
                        total1 = 255;
                    }
                    int pixel = (int)Math.Round(total1);
                    high.SetPixel(x, y, Color.FromArgb(pixel,pixel,pixel));
                    double total2 = Math.Abs(cossum2) / size;
                    if (total2 > 255)
                    {
                        total2 = 255;
                    }
                    int pixel2 = (int)Math.Round(total2);
                    low.SetPixel(x, y, Color.FromArgb(pixel2, pixel2, pixel2));
                    double total3 = Math.Abs(cossum3) / size;
                    if (total3 > 255)
                    {
                        total3 = 255;
                    }
                    int pixel3 = (int)Math.Round(total3);
                    band.SetPixel(x, y, Color.FromArgb(pixel3, pixel3, pixel3));
                    /*double total2 = (Math.Pow(Math.Pow(cossum2, 2), 0.5)) / size;
                    if (total2 > 255)
                    {
                        total2 = 255;
                    }
                    low.SetPixel(x, y, Color.FromArgb((Byte)Math.Round(total2), (Byte)Math.Round(total2), (Byte)Math.Round(total2)));
                    Console.WriteLine($"{y},{x},{(Byte)Math.Round(total1)},{(Byte)Math.Round(total2)}");*/
                }
            }
            pictureBox3.Image = low;
            pictureBox5.Image = high;
            pictureBox7.Image = band;
        }
    }
}
