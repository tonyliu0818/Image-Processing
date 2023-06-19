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

namespace WindowsFormsApp2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public void show_step(int []orginal,int []result)
        {
            string[] index = new string[256];
            for (int i = 0; i < 256; i++)
            {
                index[i] = i.ToString();
            }
            
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "pixel";
            chart1.ChartAreas[0].AxisY.Title = "pixelcount";
            chart2.ChartAreas[0].AxisX.Title = "pixel";
            chart2.ChartAreas[0].AxisY.Title = "total";
            chart1.Series.Add("gray");
            chart2.Series.Add("R");
            chart1.Series["gray"].ChartType = SeriesChartType.Line;
            chart2.Series["R"].ChartType = SeriesChartType.Line;
            chart1.Series["gray"].Color = Color.Gray;
            chart2.Series["R"].Color = Color.Gray;
            chart1.Series["gray"].BorderWidth = 5;
            chart2.Series["R"].BorderWidth = 1;
            chart1.Series["gray"].XValueType = ChartValueType.String;
            chart1.Series["gray"].YValueType = ChartValueType.Int32;
            chart2.Series["R"].XValueType = ChartValueType.String;
            chart2.Series["R"].YValueType = ChartValueType.Int32;
            chart1.Series["gray"].Points.DataBindXY(index, orginal);
            chart2.Series["R"].Points.DataBindXY(index, result);
        }
        public void show_xy(double[] trans)
        {
            string[] index = new string[256];
            double[] index_ = new double[256];
            for (int i = 0; i < 256; i++)
            {
                index[i] = i.ToString();
                index_[i] = i;
            }

            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "orginal";
            chart1.ChartAreas[0].AxisY.Title = "trans"; 
            chart1.Series.Add("contrast");
            chart1.Series["contrast"].ChartType = SeriesChartType.Line;
            chart1.Series["contrast"].Color = Color.Red;
            chart1.Series["contrast"].BorderWidth = 1;
            chart1.Series["contrast"].XValueType = ChartValueType.String;
            chart1.Series["contrast"].YValueType = ChartValueType.Double;
            chart1.Series.Add("orginal");
            chart1.Series["orginal"].ChartType = SeriesChartType.Line;
            chart1.Series["orginal"].Color = Color.Green;
            chart1.Series["orginal"].BorderWidth = 1;
            chart1.Series["orginal"].XValueType = ChartValueType.String;
            chart1.Series["orginal"].YValueType = ChartValueType.Double;
            chart1.Series["contrast"].Points.DataBindXY(index, trans);
            chart1.Series["orginal"].Points.DataBindXY(index, index_);
        }
        public void show_chart1_combine(Bitmap bitmap)
        {
            if (bitmap == null) return;
            int[] PixelCountG = new int[256];
            int[] PixelCountB = new int[256];
            int[] PixelCountR = new int[256];
            int[] PixelCountGray = new int[256];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color buff = bitmap.GetPixel(x, y);
                    PixelCountG[buff.G]++;
                    PixelCountB[buff.B]++;
                    PixelCountR[buff.R]++;
                    int avg = (buff.R + buff.G + buff.B) / 3;
                    PixelCountGray[avg]++;
                }
            }
            string[] index = new string[256];
            for (int i = 0; i < 256; i++)
            {
                index[i] = i.ToString();
            }
            chart1.Series.Clear();
            chart2.Series.Clear();
            //chart1.Titles.Add("RGB直方圖");
            //chart2.Titles.Add("RGB曲線圖");
            chart1.ChartAreas[0].AxisX.Title = "pixel";
            chart1.ChartAreas[0].AxisY.Title = "pixelcount";
            chart2.ChartAreas[0].AxisX.Title = "pixel";
            chart2.ChartAreas[0].AxisY.Title = "pixelcount";
            chart1.Series.Add("R");
            chart1.Series.Add("G");
            chart1.Series.Add("B");
            chart2.Series.Add("Gray");
            chart1.Series["R"].ChartType = SeriesChartType.Column;
            chart1.Series["G"].ChartType = SeriesChartType.Column;
            chart1.Series["B"].ChartType = SeriesChartType.Column;
            chart2.Series["Gray"].ChartType = SeriesChartType.Column;
            chart1.Series["R"].Color = Color.Red;
            chart1.Series["G"].Color = Color.Green;
            chart1.Series["B"].Color = Color.Blue;
            chart2.Series["Gray"].Color = Color.Gray;
            chart1.Series["R"].BorderWidth = 5;
            chart1.Series["G"].BorderWidth = 5;
            chart1.Series["B"].BorderWidth = 5;
            chart2.Series["Gray"].BorderWidth = 5;
            chart1.Series["R"].XValueType = ChartValueType.Int32;
            chart1.Series["G"].XValueType = ChartValueType.Int32;
            chart1.Series["B"].XValueType = ChartValueType.Int32;
            chart1.Series["R"].YValueType = ChartValueType.Int32;
            chart1.Series["G"].YValueType = ChartValueType.Int32;
            chart1.Series["B"].YValueType = ChartValueType.Int32;
            chart2.Series["Gray"].XValueType = ChartValueType.Int32;
            chart2.Series["Gray"].YValueType = ChartValueType.Int32;
            chart1.Series["R"].Points.DataBindXY(index, PixelCountR);
            chart1.Series["G"].Points.DataBindXY(index, PixelCountG);
            chart1.Series["B"].Points.DataBindXY(index, PixelCountB);
            chart2.Series["Gray"].Points.DataBindXY(index, PixelCountGray);
        }
        public void showtransform(double ratio)
        {
            int[] index = new int[256];
            double[] outpixel = new double[256];
            
            for (int i = 0; i < 256; i++)
            {
                index[i] = i;
            }
            for(int i = 0; i < 256; i++)
            {
                double rout = (double)((Math.Pow((i + 0.5) / 256, ratio) * 256) - 0.5);
                outpixel[i] = rout;
            }
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "pixel";
            chart1.ChartAreas[0].AxisY.Title = "pixelcount";
            chart2.ChartAreas[0].AxisX.Title = "pixel";
            chart2.ChartAreas[0].AxisY.Title = "total";
            chart1.Series.Add("orginal");
            chart2.Series.Add("gamma");
            chart1.Series["orginal"].ChartType = SeriesChartType.Line;
            chart2.Series["gamma"].ChartType = SeriesChartType.Line;
            chart1.Series["orginal"].Color = Color.Gray;
            chart2.Series["gamma"].Color = Color.Gray;
            chart1.Series["orginal"].BorderWidth = 1;
            chart2.Series["gamma"].BorderWidth = 1;
            chart1.Series["orginal"].XValueType = ChartValueType.Int32;
            chart1.Series["orginal"].YValueType = ChartValueType.Int32;
            chart2.Series["gamma"].XValueType = ChartValueType.String;
            chart2.Series["gamma"].YValueType = ChartValueType.Double;
            chart1.Series["orginal"].Points.DataBindXY(index, index);
            chart2.Series["gamma"].Points.DataBindXY(index, outpixel);
        }

       
    }
}
