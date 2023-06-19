
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
using  System.Drawing.Design;
using  System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Drawing2D;
using System.Threading;
using OpenCvSharp;
namespace WindowsFormsApp2
{
        
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
        }
        public void SplashStart()
        {
            Application.Run(new Form4());
        }
        //全域bitmap
        public Bitmap src_bitmap,src2_bitmap,copy_bitmap,outbitmap;
        class HeaderInformation
        {
            byte[] header = new byte[128];
            public HeaderInformation(string filepath)
            {
                byte[] readtext = File.ReadAllBytes(filepath);
                for(int i = 0; i < 128; i++)
                {
                    header[i] = readtext[i];
                }
            }
            public HeaderInformation(byte[] readtext)
            {
                for (int i = 0; i < 128; i++)
                {
                    header[i] = readtext[i];
                }
            }
            public byte manufacturer
            {
                get 
                { 
                    return header[0]; 
                }
            }
            public byte version
            {
                get 
                { 
                    return header[1]; 
                }
            }
            public byte encoding
            {
                get 
                { 
                    return header[2]; 
                }
            }
            public byte BitsPerPixel
            {
                get 
                { 
                    return header[3];
                }
            }
            public ushort Xmin
            {
                get
                {
                    return BitConverter.ToUInt16(header,4);
                }
            }
            public ushort Ymin
            {
                get
                {
                    return BitConverter.ToUInt16(header, 6);
                }
            }
            public ushort Xmax
            {
                get
                {
                    return BitConverter.ToUInt16(header, 8);
                }
            }
            public ushort Ymax
            {
                get
                {
                    return BitConverter.ToUInt16(header, 10);
                }
            }
            public ushort WidthDpi
            {
                get 
                {
                    return BitConverter.ToUInt16(header, 12);
                }
            }
            public ushort HeightDpi
            {
                get
                {
                    return BitConverter.ToUInt16(header, 14);
                }
            }
            public Color[] colormap
            {
                get
                {
                    Color[] palette = new Color[16];
                    int count = 0;
                    for(int i = 16; i < 64; i += 3)
                    {
                        palette[count]=Color.FromArgb(header[i], header[i + 1], header[i + 2]);
                        count++;
                    }
                    return palette;
                }
            }
            public byte reserve
            {
                get
                {
                    return header[64];
                }
            }
            public byte NPlanes
            {
                get
                {
                    return header[65];
                }
            }
            public ushort BytePerLine
            {
                get
                {
                    return BitConverter.ToUInt16(header, 66);
                }
            }
            public ushort palettetype
            {
                get
                {
                    return BitConverter.ToUInt16(header, 68);
                }
            }
            public ushort screenX
            {
                get
                {
                    return BitConverter.ToUInt16(header, 70);
                }
            }
            public ushort screenY
            {
                get
                {
                    return BitConverter.ToUInt16(header, 72);
                }
            }
            public byte[] filled
            {
                get
                {
                    byte[] palette = new byte[54];
                    for (int i = 74; i < 128; i++)
                    {
                        palette[i-74] = header[i];
                    }
                    return palette;
                }
            }
            public int Width
            {
                get
                {
                    return (Xmax - Xmin + 1);
                }
            }
            public int Height
            {
                get
                {
                    return (Ymax - Ymin + 1);
                }
            }
        }
        HeaderInformation Header1,Header2;
        //header information
        public void snr_(Bitmap bitmap1,Bitmap bitmap2)
        {
            /*long inputcount = 0;
            long outcount = 0;
            for(int y = 0; y < bitmap1.Height; y++)
            {
                for(int x = 0; x < bitmap1.Width; x++)
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
        public void PutLabelText()
        {
            DataGridView dataGridview = new DataGridView();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Header", typeof(string));
            dataTable.Columns.Add("information", typeof(string));
            if (Header1.manufacturer == 10)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "manufacturer";
                row[1] = $"PCX,為:{Header1.manufacturer}";
                dataTable.Rows.Add(row);
            }
            else
            {
                DataRow row = dataTable.NewRow();
                row[0] = "manufacturer";
                row[1] = $"文件輸入錯誤";
                dataTable.Rows.Add(row);
            }
            DataRow row1 = dataTable.NewRow();
            row1[0] = "version";
            row1[1] = $"{Header1.version}";
            dataTable.Rows.Add(row1);
            dataGridView1.DataSource = dataTable;
            if (Header1.encoding == 1)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "encoding";
                row[1] = $"RLE,為:{Header1.encoding}";
                dataTable.Rows.Add(row);
            }
            else
            {
                DataRow row = dataTable.NewRow();
                row[0] = "encoding";
                row[1] = $"error encoding";
                dataTable.Rows.Add(row);
            }
            DataRow row2 = dataTable.NewRow();
            row2[0] = "每個pixel有幾個bits";
            row2[1] = $"{Header1.BitsPerPixel}";
            dataTable.Rows.Add(row2);
            dataGridView1.DataSource = dataTable;
            DataRow row3 = dataTable.NewRow();
            row3[0] = "Xmin";
            row3[1] = $"{Header1.Xmin}";
            dataTable.Rows.Add(row3);
            dataGridView1.DataSource = dataTable;
            DataRow row4 = dataTable.NewRow();
            row4[0] = "Ymin";
            row4[1] = $"{Header1.Ymin}";
            dataTable.Rows.Add(row4);
            dataGridView1.DataSource = dataTable;
            
            DataRow row6 = dataTable.NewRow();
            row6[0] = "Xmax";
            row6[1] = $"{Header1.Xmax}";
            dataTable.Rows.Add(row6);
            dataGridView1.DataSource = dataTable;
            DataRow row7 = dataTable.NewRow();
            row7[0] = "Ymax";
            row7[1] = $"{Header1.Ymax}";
            dataTable.Rows.Add(row7);
            dataGridView1.DataSource = dataTable;
            DataRow row8 = dataTable.NewRow();
            row8[0] = "X方向的dpi";
            row8[1] = $"{Header1.WidthDpi}";
            dataTable.Rows.Add(row8);
            dataGridView1.DataSource = dataTable;
            DataRow row9 = dataTable.NewRow();
            row9[0] = "X方向的dpi";
            row9[1] = $"{Header1.HeightDpi}";
            dataTable.Rows.Add(row9);
            dataGridView1.DataSource = dataTable;
            if (Header1.NPlanes == 1)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "ColorPlane";
                row[1] = $"Color plane為:{Header1.NPlanes}";
                dataTable.Rows.Add(row);
            }
            else if (Header1.NPlanes == 3)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "ColorPlane";
                row[1] = $"Color plane為:{Header1.NPlanes}";
                dataTable.Rows.Add(row);
            }
            DataRow row10 = dataTable.NewRow();
            row10[0] = "每一行的Byte";
            row10[1] = $"{Header1.BytePerLine}";
            dataTable.Rows.Add(row10);
            DataRow row11 = dataTable.NewRow();
            row11[0] = "調色盤種類";
            row11[1] = $"{Header1.palettetype}";
            dataTable.Rows.Add(row11);
            DataRow row13 = dataTable.NewRow();
            row13[0] = "screenX";
            row13[1] = $"{Header1.screenX}";
            dataTable.Rows.Add(row13);
            DataRow row12 = dataTable.NewRow();
            row12[0] = "screenY";
            row12[1] = $"{Header1.screenY}";
            dataTable.Rows.Add(row12);
            DataRow row14 = dataTable.NewRow();
            row14[0] = "影像的寬";
            row14[1] = $"{Header1.Width}";
            dataTable.Rows.Add(row14);
            DataRow row15 = dataTable.NewRow();
            row15[0] = "影像的長";
            row15[1] = $"{Header1.Height}";
            dataTable.Rows.Add(row15);
            dataGridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void PutLabel2Text()
        {
            DataGridView dataGridview = new DataGridView();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Header", typeof(string));
            dataTable.Columns.Add("information", typeof(string));
            if (Header2.manufacturer == 10)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "manufacturer";
                row[1] = $"PCX,為:{Header2.manufacturer}";
                dataTable.Rows.Add(row);
            }
            else
            {
                DataRow row = dataTable.NewRow();
                row[0] = "manufacturer";
                row[1] = $"文件輸入錯誤";
                dataTable.Rows.Add(row);
            }
            DataRow row1 = dataTable.NewRow();
            row1[0] = "version";
            row1[1] = $"{Header2.version}";
            dataTable.Rows.Add(row1);
            dataGridView2.DataSource = dataTable;
            if (Header2.encoding == 1)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "encoding";
                row[1] = $"RLE,為:{Header2.encoding}";
                dataTable.Rows.Add(row);
            }
            else
            {
                DataRow row = dataTable.NewRow();
                row[0] = "encoding";
                row[1] = $"error encoding";
                dataTable.Rows.Add(row);
            }
            DataRow row2 = dataTable.NewRow();
            row2[0] = "每個pixel有幾個bits";
            row2[1] = $"{Header2.BitsPerPixel}";
            dataTable.Rows.Add(row2);
            dataGridView2.DataSource = dataTable;
            DataRow row3 = dataTable.NewRow();
            row3[0] = "Xmin";
            row3[1] = $"{Header2.Xmin}";
            dataTable.Rows.Add(row3);
            dataGridView2.DataSource = dataTable;
            DataRow row4 = dataTable.NewRow();
            row4[0] = "Ymin";
            row4[1] = $"{Header2.Ymin}";
            dataTable.Rows.Add(row4);
            dataGridView2.DataSource = dataTable;

            DataRow row6 = dataTable.NewRow();
            row6[0] = "Xmax";
            row6[1] = $"{Header2.Xmax}";
            dataTable.Rows.Add(row6);
            dataGridView2.DataSource = dataTable;
            DataRow row7 = dataTable.NewRow();
            row7[0] = "Ymax";
            row7[1] = $"{Header2.Ymax}";
            dataTable.Rows.Add(row7);
            dataGridView2.DataSource = dataTable;
            DataRow row8 = dataTable.NewRow();
            row8[0] = "X方向的dpi";
            row8[1] = $"{Header2.WidthDpi}";
            dataTable.Rows.Add(row8);
            dataGridView2.DataSource = dataTable;
            DataRow row9 = dataTable.NewRow();
            row9[0] = "X方向的dpi";
            row9[1] = $"{Header2.HeightDpi}";
            dataTable.Rows.Add(row9);
            dataGridView2.DataSource = dataTable;
            if (Header2.NPlanes == 1)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "ColorPlane";
                row[1] = $"Color plane為:{Header2.NPlanes}";
                dataTable.Rows.Add(row);
            }
            else if (Header2.NPlanes == 3)
            {
                DataRow row = dataTable.NewRow();
                row[0] = "ColorPlane";
                row[1] = $"Color plane為:{Header2.NPlanes}";
                dataTable.Rows.Add(row);
            }
            DataRow row10 = dataTable.NewRow();
            row10[0] = "每一行的Byte";
            row10[1] = $"{Header2.BytePerLine}";
            dataTable.Rows.Add(row10);
            DataRow row11 = dataTable.NewRow();
            row11[0] = "調色盤種類";
            row11[1] = $"{Header2.palettetype}";
            dataTable.Rows.Add(row11);
            DataRow row13 = dataTable.NewRow();
            row13[0] = "screenX";
            row13[1] = $"{Header2.screenX}";
            dataTable.Rows.Add(row13);
            DataRow row12 = dataTable.NewRow();
            row12[0] = "screenY";
            row12[1] = $"{Header2.screenY}";
            dataTable.Rows.Add(row12);
            DataRow row14 = dataTable.NewRow();
            row14[0] = "影像的寬";
            row14[1] = $"{Header2.Width}";
            dataTable.Rows.Add(row14);
            DataRow row15 = dataTable.NewRow();
            row15[0] = "影像的長";
            row15[1] = $"{Header2.Height}";
            dataTable.Rows.Add(row15);
            dataGridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void Put16color(Color[] PaletteData,bool state)
        {
            int ratio = 20;
            Bitmap outbitmap = new Bitmap(4*ratio,4* ratio, PixelFormat.Format24bppRgb);
            int count = 0;
            for (int y = 0; y < 4 * ratio; y += ratio)
            {
                for (int x = 0; x < 4 * ratio; x += ratio)
                {
                    for (int j = 0; j < ratio; j++)
                    {
                        for (int i = 0; i < ratio; i++)
                        {
                            outbitmap.SetPixel(x + i, y + j, Color.FromArgb(PaletteData[count].R, PaletteData[count].G, PaletteData[count].B));
                        }
                    }
                    count++;
                }
            }
            if(state)
                pictureBox4.Image = outbitmap;
            else
            {
                pictureBox7.Image = outbitmap;
            }
        }
        public void PutPalette(Color[] PaletteData,bool state)
        {
            int ratio = 7;
            Bitmap outbitmap = new Bitmap(16 * ratio,16* ratio, PixelFormat.Format24bppRgb);
            int count = 0;
            
            for(int y = 0; y < 16 * ratio; y += ratio)
            {
                for(int x = 0; x < 16 * ratio; x += ratio)
                {
                    for(int j = 0; j < ratio; j++)
                    {
                        for(int i=0;i<ratio; i++)
                        {
                            outbitmap.SetPixel(x + i, y+j, Color.FromArgb(PaletteData[count].R, PaletteData[count].G, PaletteData[count].B));
                        }
                    }
                    count++;
                }
                
            }
            if (state)
                pictureBox5.Image = outbitmap;
            else
                pictureBox6.Image = outbitmap;
        }
        public void Set_pic_from_another_form(Bitmap bitmap1)
        {
            label13.Visible = false;
            label15.Visible = false; 
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            pictureBox7.Visible = false;
            pictureBox6.Visible = false;
            pictureBox3.Visible = false;
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            functionToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem1.Enabled = true;
            addpToolStripMenuItem.Enabled = false;
            watermarketingToolStripMenuItem.Enabled = false;
            histogramToolStripMenuItem.Enabled = true;
            scrollbarToolStripMenuItem.Enabled = true;
            statusStrip2.Visible = true;
            statusStrip1.Visible = true;
            src_bitmap = new Bitmap(bitmap1.Width, bitmap1.Height, PixelFormat.Format24bppRgb);
            for(int y = 0; y < bitmap1.Height; y++)
            {
                for(int x = 0; x < bitmap1.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, bitmap1.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
        }
        //openpicture
        private void p2(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "所有檔案(*.*)|*.*";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK && ofd.FileName != "")
            {
                statusStrip3.Visible = true;
                label1.Text = $"Filename:{ofd.FileName}";
                Header2 = new HeaderInformation(ofd.FileName);
                PutLabel2Text();
                label2.Visible = true;
                label3.Visible = true;
                pictureBox7.Visible = true;
                pictureBox6.Visible = true;
                dataGridView2.Visible = true;
                pictureBox3.Visible = true;
                addpToolStripMenuItem.Enabled = true;
                watermarketingToolStripMenuItem.Enabled = true;
                byte[] content = File.ReadAllBytes(ofd.FileName);
                src2_bitmap = new Bitmap(Header2.Width, Header2.Height, PixelFormat.Format24bppRgb);
                Color[] PaletteData = new Color[256];
                byte[] tailpalette = new byte[768];
                int count = 0;
                Put16color(Header2.colormap, false);
                for (int i = content.Length - 768; i < content.Length; i++)
                {
                    tailpalette[count] = content[i];
                    count++;
                }
                count = 0;
                for (int i = 0; i < 768; i += 3)
                {
                    Color buff = Color.FromArgb(tailpalette[i], tailpalette[i + 1], tailpalette[i + 2]);
                    PaletteData.SetValue(buff, i / 3);
                }

                int endindex = content.Length - 769;
                if (Header2.manufacturer != 10) return;
                if (Header2.NPlanes == 1 && Header2.BitsPerPixel == 8)
                {
                    int startindex = 128;
                    byte[] indexdata = new byte[Header2.Width * Header2.Height];
                    int pixelcount = 0;
                    while (startindex < endindex)
                    {
                        byte refrence = content[startindex];
                        if (refrence > 192)
                        {
                            int reply = refrence - 192;
                            startindex++;
                            int replynumber = content[startindex];
                            for (int i = 0; i < reply; i++)
                            {
                                indexdata[i + pixelcount] = content[startindex];
                            }
                            pixelcount += reply;
                            startindex++;
                        }
                        else
                        {
                            indexdata[pixelcount] = refrence;
                            pixelcount++;
                            startindex++;
                        }
                    }
                    int c = 0;
                    for (int y = 0; y < Header2.Height; y++)
                    {
                        for (int x = 0; x < Header2.Width; x++)
                        {
                            src2_bitmap.SetPixel(x, y, PaletteData[indexdata[c]]);
                            c++;
                        }
                    }
                    PutPalette(PaletteData, false);
                    pictureBox3.Image = src2_bitmap;
                }
                else if (Header2.NPlanes == 3 && Header2.BitsPerPixel == 8)
                {
                    int startindex = 128;
                    BitmapData bd = src2_bitmap.LockBits(new Rectangle(0, 0, Header2.Width, Header2.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    byte[] RGBdata = new byte[bd.Height * bd.Stride];
                    for (int y = 0; y < bd.Height; y++)
                    {
                        byte[] RowRGBdata = new byte[Header2.BytePerLine * 3];
                        int pixelcount = 0;
                        int rgbindex = 2;
                        while (true)
                        {
                            byte refrence = content[startindex];
                            if (refrence > 192)
                            {
                                int reply = refrence - 192;
                                startindex++;
                                for (int i = 0; i < reply; i++)
                                {
                                    if (pixelcount + i >= Header2.BytePerLine)
                                    {
                                        i = 0;
                                        pixelcount = 0;
                                        rgbindex--;
                                        count = count - i;
                                        if (rgbindex == -1) break;
                                    }
                                    RowRGBdata[(pixelcount + i) * 3 + rgbindex] = content[startindex];
                                }
                                pixelcount += reply;
                                startindex++;
                            }
                            else
                            {
                                RowRGBdata[pixelcount * 3 + rgbindex] = refrence;
                                pixelcount++;
                                startindex++;
                            }

                            if (pixelcount >= Header2.BytePerLine)
                            {
                                pixelcount = 0;
                                rgbindex--;

                            }
                            if (rgbindex == -1) break;
                        }
                        Array.Copy(RowRGBdata, 0, RGBdata, y * bd.Stride, RowRGBdata.Length);
                    }
                    Marshal.Copy(RGBdata, 0, bd.Scan0, RGBdata.Length);
                    src2_bitmap.UnlockBits(bd);
                    pictureBox3.Image = src2_bitmap;
                    label2.Visible = false;
                    //PutPalette(PaletteData, false);
                }
            }
        }

        private void p1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "所有檔案(*.*)|*.*";
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK && ofd.FileName != "")
            {
                statusStrip2.Visible = true;
                statusStrip1.Visible = true;
                label1.Visible = true;
                label13.Visible = true;
                label15.Visible = true;
                dataGridView1.Visible = true;
                label1.Text = $"Filename:{ofd.FileName}";
                functionToolStripMenuItem.Enabled = true;
                cutToolStripMenuItem1.Enabled = true;
                addpToolStripMenuItem.Enabled = false;
                watermarketingToolStripMenuItem.Enabled = false;
                histogramToolStripMenuItem.Enabled = true;
                scrollbarToolStripMenuItem.Enabled = true;
                Header1 = new HeaderInformation(ofd.FileName);
                PutLabelText();
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                Put16color(Header1.colormap,true);
                byte[] content = File.ReadAllBytes(ofd.FileName);
                src_bitmap = new Bitmap(Header1.Width, Header1.Height, PixelFormat.Format24bppRgb);
                Color[] PaletteData = new Color[256];
                byte[] tailpalette = new byte[768];
                int count = 0;
                for (int i = content.Length - 768; i < content.Length; i++)
                {
                    tailpalette[count] = content[i];
                    count++;
                }
                count = 0;
                for (int i = 0; i < 768; i += 3)
                {
                    Color buff = Color.FromArgb(tailpalette[i], tailpalette[i + 1], tailpalette[i + 2]);
                    PaletteData.SetValue(buff, i / 3);
                }
                
                int endindex = content.Length - 769;
                if (Header1.manufacturer != 10) return;
                if (Header1.NPlanes == 1 && Header1.BitsPerPixel == 8)
                {
                    int startindex = 128;
                    byte[] indexdata = new byte[Header1.Width * Header1.Height];
                    int pixelcount = 0;
                    while (startindex < endindex)
                    {
                        byte refrence = content[startindex];
                        if (refrence > 192)
                        {
                            int reply = refrence - 192;
                            startindex++;
                            int replynumber = content[startindex];
                            for (int i = 0; i < reply; i++)
                            {
                                indexdata[i + pixelcount] = content[startindex];
                            }
                            pixelcount += reply;
                            startindex++;
                        }
                        else
                        {
                            indexdata[pixelcount] = refrence;
                            pixelcount++;
                            startindex++;
                        }
                    }
                    int c = 0;
                    for (int y = 0; y < Header1.Height; y++)
                    {
                        for (int x = 0; x < Header1.Width; x++)
                        {
                            src_bitmap.SetPixel(x, y, PaletteData[indexdata[c]]);
                            c++;
                        }
                    }
                    PutPalette(PaletteData,true);
                    pictureBox1.Image = src_bitmap;
                }
                else if (Header1.NPlanes == 3 && Header1.BitsPerPixel == 8)
                {
                    int startindex = 128;
                    BitmapData bd = src_bitmap.LockBits(new Rectangle(0, 0, Header1.Width, Header1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    byte[] RGBdata = new byte[bd.Height * bd.Stride];
                    for (int y = 0; y < bd.Height; y++)
                    {
                        byte[] RowRGBdata = new byte[Header1.BytePerLine * 3];
                        int pixelcount = 0;
                        int rgbindex = 2;
                        while (true)
                        {
                            byte refrence = content[startindex];
                            if (refrence > 192)
                            {
                                int reply = refrence - 192;
                                startindex++;
                                for (int i = 0; i < reply; i++)
                                {
                                    if (pixelcount + i >= Header1.BytePerLine)
                                    {
                                        i = 0;
                                        pixelcount = 0;
                                        rgbindex--;
                                        count = count - i;
                                        if (rgbindex == -1) break;
                                    }
                                    RowRGBdata[(pixelcount + i) * 3 + rgbindex] = content[startindex];
                                }
                                pixelcount += reply;
                                startindex++;
                            }
                            else
                            {
                                RowRGBdata[pixelcount * 3 + rgbindex] = refrence;
                                pixelcount++;
                                startindex++;
                            }

                            if (pixelcount >= Header1.BytePerLine)
                            {
                                pixelcount = 0;
                                rgbindex--;

                            }
                            if (rgbindex == -1) break;
                        }
                        Array.Copy(RowRGBdata, 0, RGBdata, y * bd.Stride, RowRGBdata.Length);
                    }
                    Marshal.Copy(RGBdata, 0, bd.Scan0, RGBdata.Length);
                    src_bitmap.UnlockBits(bd);
                    pictureBox1.Image = src_bitmap;
                    label15.Visible = false;
                    //PutPalette(PaletteData, true);
                }
                copy_bitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
                for(int y = 0; y < src_bitmap.Height; y++)
                {
                    for(int x = 0; x < src_bitmap.Width; x++)
                    {
                        copy_bitmap.SetPixel(x, y, src_bitmap.GetPixel(x, y));
                    }
                }
            }
        }
        //灰階RGB負片
        private void grayscale(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    double avg = (c.R+c.G+c.B)/3;
                    outbitmap.SetPixel(x, y, Color.FromArgb((int)avg, (int)avg, (int)avg));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void rfunction(object sender, EventArgs e)
        {
            if (src_bitmap == null) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    outbitmap.SetPixel(x, y, Color.FromArgb(c.R, 0, 0));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void gfunction(object sender, EventArgs e)
        {
            if (src_bitmap == null) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    outbitmap.SetPixel(x, y, Color.FromArgb(0, c.G, 0));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void bfunction(object sender, EventArgs e)
        {
            if (src_bitmap == null) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    outbitmap.SetPixel(x, y, Color.FromArgb(0, 0, c.B));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }
        private void negativefunction(object sender, EventArgs e)
        {
            if (src_bitmap == null) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    double avg = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
                    outbitmap.SetPixel(x, y, Color.FromArgb((Byte)(255 - avg), (Byte)(255 - avg), (Byte)(255 - avg)));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        //直方圖
        private void combinecolumn(object sender, EventArgs e)
        {
            Form2 fr2 = new Form2();
            fr2.Show();
            if (src_bitmap == null) return;
            fr2.show_chart1_combine(src_bitmap);
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            
        }
        private void p2_MouseMove(object sender, MouseEventArgs e)
        {
            if (src2_bitmap != null)
            {
                if ((e.X < src2_bitmap.Width) && (e.Y < src2_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel24.Text = e.X.ToString();
                    toolStripStatusLabel26.Text = e.Y.ToString();
                    toolStripStatusLabel28.Text = src2_bitmap.GetPixel(e.X, e.Y).R.ToString();
                    toolStripStatusLabel30.Text = src2_bitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel32.Text = src2_bitmap.GetPixel(e.X, e.Y).B.ToString();
                }
                
            }
        }
        bool mousestop = false;
        private void out_MouseMove(object sender, MouseEventArgs e)
        {
            if (outbitmap != null)
            {
                if ((e.X < outbitmap.Width) && (e.Y < outbitmap.Height) && e.X >= 0 && e.Y >= 0)
                {
                    toolStripStatusLabel12.Text = e.X.ToString();
                    toolStripStatusLabel14.Text = e.Y.ToString();
                    toolStripStatusLabel16.Text = outbitmap.GetPixel(e.X,e.Y).R.ToString();
                    toolStripStatusLabel18.Text = outbitmap.GetPixel(e.X, e.Y).G.ToString();
                    toolStripStatusLabel20.Text = outbitmap.GetPixel(e.X, e.Y).B.ToString();
                }
            }
        }
        private void bitmap1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mousestop&&src_bitmap!=null)
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                {

                    toolStripStatusLabel1.Text = $"X:";
                    toolStripStatusLabel2.Text = $"{e.X}";
                    toolStripStatusLabel3.Text = $"Y:";
                    toolStripStatusLabel4.Text = $"{e.Y}";
                    Color c = src_bitmap.GetPixel(e.X, e.Y);
                    toolStripStatusLabel5.Text = $"R:";
                    toolStripStatusLabel6.Text = $"{c.R}";
                    toolStripStatusLabel7.Text = $"G:";
                    toolStripStatusLabel8.Text = $"{c.G}";
                    toolStripStatusLabel9.Text = $"B:";
                    toolStripStatusLabel10.Text = $"{c.B}";
                }
            }
            
        }
        private void mousedown(object sensor, MouseEventArgs e)
        {
            
        }
        private void mouseup(object sensor, MouseEventArgs e)
        {
           
            
        }

        private void mousedoubleclick(object sensor, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left&&src_bitmap!=null)
            {
                mousestop = true;
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {
                        outbitmap.SetPixel(x, y, src_bitmap.GetPixel(x, y));
                    }
                }
                for (int y = -1; y < 1; y++)
                {
                    for (int x = -1; x < 1; x++)
                    {
                        outbitmap.SetPixel(x + e.X, y + e.Y, Color.FromArgb(255, 0, 0));
                    }
                }
                pictureBox1.Image = outbitmap;
            }
        }
        //球
        Form3 f;
        private void ballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f = new Form3();
            f.Show();
        }
        //replace
        private void ReplaceFunction(object sender, EventArgs e)
        {
            if (mousestop&& toolStripTextBox1.Text!=""&& toolStripTextBox2.Text!=""&& toolStripTextBox3.Text!="")
            {
                int findx = int.Parse(toolStripStatusLabel2.Text);
                int findy = int.Parse(toolStripStatusLabel4.Text);
                int findr = int.Parse(toolStripTextBox1.Text);
                int findg = int.Parse(toolStripTextBox2.Text);
                int findb = int.Parse(toolStripTextBox3.Text);
                outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
                for (int y = 0; y < src_bitmap.Height; y++)
                {
                    for (int x = 0; x < src_bitmap.Width; x++)
                    {

                        Color c = src_bitmap.GetPixel(x, y);
                        outbitmap.SetPixel(x, y, Color.FromArgb(c.R, c.G, c.B));
                    }
                }
                for (int y = -1; y < 1; y++)
                {
                    for (int x = -1; x < 1; x++)
                    {
                        outbitmap.SetPixel(x + findx, y + findy, Color.FromArgb(findr, findg, findb));
                    }
                }
                pictureBox2.Image = outbitmap;
                pictureBox1.Image = src_bitmap;
                mousestop = false;
                snr_(src_bitmap, outbitmap);
            }
            
        }
        //正旋
        private void RotationFunction(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.rotation1(src_bitmap);
        }

        private void large1(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.dump(src_bitmap);
        }

        private void linear(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.linear(src_bitmap);
        }

        private void firstelement(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.first(src_bitmap);
        }

        private void average(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.average(src_bitmap);
        }

        

        private void ADDP(object sender, EventArgs e)
        {
            if (src2_bitmap == null) return;
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.addp(src_bitmap,src2_bitmap);
        }
        private void offsetp_(object sender, EventArgs e)
        {
            
            offsetp o = new offsetp();
            o.Show();
            o.offset(src_bitmap);
        }
        private void depth(object sender, EventArgs e)
        {
            
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.depth(src_bitmap);
        }

       
        private void watermarketing(object sender, EventArgs e)
        {
            if (src2_bitmap == null) return;
            Form5 f = new Form5();
            f.Show();
            f.split(src_bitmap, src2_bitmap,0);
        }

        private void manualthreshold(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.threshold(src_bitmap);
        }

        private void otsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.otsu(src_bitmap);
        }

        private void ditheringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rand = new Random();
            int mask = 3;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);  
            for(int y = 0; y < src_bitmap.Height; y ++)
            {
                for(int x = 0; x < src_bitmap.Width; x ++)
                {
                    Color c_ = src_bitmap.GetPixel(x, y);
                    if (c_.R == 0)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        int white = 0;
                        for(int j = 0; j < mask; j++)
                        {
                            for(int i = 0; i < mask; i++)
                            {
                                /*int z = rand.Next(2);
                                if (z == 1)
                                {
                                    white++;
                                }*/
                                white = (rand.Next(101) > 2) ? white + 1 : white;
                            }
                        }
                        double ratio = white / (mask * mask);
                        
                        int avg = (c_.R + c_.G + c_.B) / 3;
                        outbitmap.SetPixel(x, y, Color.FromArgb((Byte)(avg*ratio), (Byte)(avg * ratio), (Byte)(avg * ratio)));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }
        private void equlationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = c.R;
                    histogram[avg]++;
                }
            }
            double[] mapping = new double[256];
            int[] step = new int[256];
            int[] step2 = new int[256];
            int step1buff=0;
            for (int i = 0; i < 256; i++)
            {
                double value = histogram[i];
                double rate = value / (src_bitmap.Width * src_bitmap.Height);
                mapping[i] = rate;
                step1buff += histogram[i];
                step[i] += step1buff;
            }
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    double count = 0;
                    Color c = src_bitmap.GetPixel(x, y);
                    for(int i = 0; i < c.R; i++)
                    {
                        count += mapping[i];
                    }
                    byte s = (byte)Math.Round(255 * count);
                    step2[s]++;
                    outbitmap.SetPixel(x, y, Color.FromArgb(s,s,s));
                }
            }
            int[] HistogramResult = new int[256];
            int step2buff = 0;
            for (int i = 0; i < 256; i++)
            {
                step2buff += step2[i];
                HistogramResult[i] += step2buff;
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
            Form2 form = new Form2();
            form.Show();
            form.show_step(step, HistogramResult);
        }

        private void contrast(object sender, EventArgs e)
        {
            Contrast c = new Contrast();
            c.Show();
            c.c(src_bitmap);
        }

        private void graycode(object sender, EventArgs e)
        {
            Form5 f = new Form5();
            f.Show();
            f.gray(src_bitmap);
        }

        private void resultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void copyResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outbitmap != null)
            {
                Form1 f = new Form1();
                f.Show();
                f.Set_pic_from_another_form(outbitmap);
            }
        }

        

        private void gamma(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.gamma(src_bitmap);
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for(int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c1 = src_bitmap.GetPixel(x, y);
                    Color c2 = src_bitmap.GetPixel(src_bitmap.Width - 1 - x, y);
                    outbitmap.SetPixel(x, y, c2);
                    outbitmap.SetPixel(src_bitmap.Width - 1 - x, y, c1);
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c1 = src_bitmap.GetPixel(x, y);
                    Color c2 = src_bitmap.GetPixel(x,src_bitmap.Height - 1 -  y);
                    outbitmap.SetPixel(x, y, c2);
                    outbitmap.SetPixel(x,src_bitmap.Height - 1 -  y, c1);
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void diagonalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (src_bitmap.Width != src_bitmap.Height) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = y; x < src_bitmap.Width; x++)
                {
                    Color c1 = src_bitmap.GetPixel(x, y);
                    Color c2 = src_bitmap.GetPixel(y, x);
                    outbitmap.SetPixel(x, y, c2);
                    outbitmap.SetPixel(y, x, c1);
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void oppositeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (src_bitmap.Width != src_bitmap.Height) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width-y; x++)
                {
                    Color c1 = src_bitmap.GetPixel(x, y);
                    Color c2 = src_bitmap.GetPixel(src_bitmap.Height - 1 - y, src_bitmap.Width - 1 - x);
                    outbitmap.SetPixel(x, y, c2);
                    outbitmap.SetPixel(src_bitmap.Height - 1 - y, src_bitmap.Width - 1 - x, c1);
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void outlierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lowpass l = new lowpass();
            l.Show();
            l.outlier(src_bitmap);

        }

       
        private void shear(object sender, EventArgs e)
        {
            Shear s = new Shear();
            s.Show();
            s.she(src_bitmap);
        }

        private void averageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lowpass l = new lowpass();
            l.Show();
            l.average(src_bitmap);
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            lowpass l = new lowpass();
            l.Show();
            l.cross(src_bitmap);
        }

        private void mediannnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lowpass l = new lowpass();
            l.Show();
            l.square(src_bitmap);
        }
        public double fourier(int v,int u)
        {
            double cossum = 0;
            double sinsum = 0;
            BitmapData bitmapdata = src_bitmap.LockBits(new Rectangle(0, 0, src_bitmap.Width, src_bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = bitmapdata.Scan0;
            int size2 = bitmapdata.Stride * src_bitmap.Height;
            byte[] oriBytes = new byte[size2];
            Marshal.Copy(intPtr, oriBytes, 0, size2);
            int k = 3;
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    /*Color c = src_bitmap.GetPixel(x, y);
                    byte avg = (byte)((c.R + c.G + c.B) / 3);*/
                    byte r = oriBytes[(y) * bitmapdata.Stride + (x) * k];
                    byte g = oriBytes[(y) * bitmapdata.Stride + (x) * k+1];
                    byte b = oriBytes[(y) * bitmapdata.Stride + (x) * k+2]; 
                    int avg = (r+g+b)/3;
                    cossum += avg * Math.Cos(2 * -1 * Math.PI * (y * v + x * u)/ 256 );
                    sinsum += avg * Math.Sin(2 * -1 * Math.PI * (y * v + x * u)/ 256 );
                }
            }
            src_bitmap.UnlockBits(bitmapdata);
            double total= (Math.Pow(Math.Pow(cossum , 2) + Math.Pow(sinsum, 2), 0.5));
            return total / 256;
        }
        private void frequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*double[,] F = new double[256, 256];
            for(int v = -128; v < 128; v++)
            {
                for(int u = -128; u < 128; u++)
                {
                    
                    F[v+128, u+128] = Math.Round(fourier(v, u));
                    Console.WriteLine($"{u},{v},{F[v+128,u+128]}");
                    //Console.WriteLine(F[y, x]);
                }
            }

            outbitmap = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    
                    if (F[y, x] <256)
                    {
                        Byte pixel = (Byte)(F[x, y]);
                        //outbitmap.SetPixel(x , y , Color.FromArgb(pixel, pixel, pixel));
                        outbitmap.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }
            pictureBox2.Image = outbitmap;*/
            /*Bitmap bbb = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bitmapdata = src_bitmap.LockBits(new Rectangle(0, 0, src_bitmap.Width, src_bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData outbitmapdata = bbb.LockBits(new Rectangle(0, 0, src_bitmap.Width, src_bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtrN = outbitmapdata.Scan0;
            IntPtr intPtr = bitmapdata.Scan0;
            int size = bitmapdata.Stride * src_bitmap.Height;
            byte[] oriBytes = new byte[size];
            byte[] newBytes = new byte[size];
            Marshal.Copy(intPtr, oriBytes, 0, size);
            Marshal.Copy(intPtrN, newBytes, 0, size);
            int k = 3;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y1 = 0; y1 < src_bitmap.Height; y1++)
            {
                for (int x1 = 0; x1 < src_bitmap.Width; x1++)
                {
                    if (x1 % 8 == 0 && y1 % 8 == 0 && x1 < src_bitmap.Width - (src_bitmap.Width % 8) && y1 < src_bitmap.Height - (src_bitmap.Height % 8))
                    {
                        int[,] data = new int[8,8];
                        for (int v = 0; v < 8; v++)
                        {
                            for (int u = 0; u < 8; u++)
                            {
                                double sum = 0;
                                for (int x = 0; x < 8; x++)
                                {
                                    for (int y = 0; y < 8; y++)
                                    {
                                        byte r = oriBytes[(y1 + y) * bitmapdata.Stride + (x1 + x) * k];
                                        byte g = oriBytes[(y1 + y) * bitmapdata.Stride + (x1 + x) * k + 1];
                                        byte b = oriBytes[(y1 + y) * bitmapdata.Stride + (x1 + x) * k + 2];
                                        int avg = (r + g + b) / 3;
                                        sum += (double)((avg - 128.0) * Math.Cos((2.0 * x + 1.0) * Math.PI * u / 16.0) * Math.Cos((2.0 * y + 1.0) * Math.PI * v / 16.0));
                                    }
                                }
                                if (v == 0)
                                {
                                    sum *= 0.354;
                                }
                                else
                                {
                                    sum *= 0.5;
                                }
                                if (u == 0)
                                {
                                    sum *= 0.354;
                                }
                                else
                                {
                                    sum *= 0.5;
                                }
                                if (u < 4 && v < 4)
                                {
                                    data[v,u] = (int)(sum);
                                }
                                else data[v,u] = 0;
                            }
                        }
                        for(int y = 0; y < 8; y++)
                        {
                            for(int x = 0; x < 8; x++)
                            {
                                if (y + y1 > 0 && x + x1 > 0 && y + y1 < src_bitmap.Height && x + x1 < src_bitmap.Width)
                                {
                                    if (data[y, x] > 0)
                                    {
                                        outbitmap.SetPixel(y + y1, x + x1, Color.White);
                                    }
                                    else
                                    {
                                        outbitmap.SetPixel(y + y1, x + x1, Color.Black);
                                    }
                                }
                            }
                        }
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                double sum = 0;
                                for (int v = 0; v < 8; v++)
                                {
                                    for (int u = 0; u < 8; u++)
                                    {
                                        if (u == 0 && v == 0)
                                        {
                                            sum += 0.125 * (data[v,u]) * Math.Cos((2.0 * x + 1.0) * Math.PI * u / 16.0) * Math.Cos((2.0 * y + 1.0) * Math.PI * v / 16.0);
                                        }
                                        else if ((u == 0 && v != 0) || (u != 0 && v == 0))
                                        {
                                            sum += 0.5 * 0.354 * (data[v,u]) * Math.Cos((2.0 * x + 1.0) * Math.PI * u / 16.0) * Math.Cos((2.0 * y + 1.0) * Math.PI * v / 16.0);
                                        }
                                        else
                                        {
                                            sum += 0.354 * 0.354 * (data[v,u]) * Math.Cos((2.0 * x + 1.0) * Math.PI * u / 16.0) * Math.Cos((2.0 * y + 1.0) * Math.PI * v / 16.0);
                                        }
                                    }
                                }
                                sum = sum + 128;
                                newBytes[(y1+y) * bitmapdata.Stride + (x1+x) * k + 2] = (byte)sum;
                                newBytes[(y1+y) * bitmapdata.Stride + (x1+x) * k + 1] = (byte)sum;
                                newBytes[(y1+y) * bitmapdata.Stride + (x+x1) * k] = (byte)sum;
                            }
                        }
                    }
                }

            }
            Marshal.Copy(newBytes, 0, intPtrN, size);
            src_bitmap.UnlockBits(bitmapdata);
            bbb.UnlockBits(outbitmapdata);
            
            pictureBox2.Image = outbitmap;*/
            frequency f = new frequency();
            f.Show();
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for(int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    outbitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            f.decode(outbitmap);
        }

        private void highpassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualThreshold m = new ManualThreshold();
            m.Show();
            m.highpass(src_bitmap);
        }

        private void highboostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighBoost h = new HighBoost();
            h.Show();
            h.func(src_bitmap);
        }

        private void crispeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualThreshold m = new ManualThreshold();
            m.Show();
            m.crispening(src_bitmap);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.sobel(src_bitmap);
        }

        private void prewittToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.prewitt(src_bitmap);
        }

        private void robertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.robert(src_bitmap);
        }

        private void laplacianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.laplacian(src_bitmap);
        }

        private void simplelaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.simple(src_bitmap);
        }

        private void noiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rand = new Random();
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for(int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    int num = rand.Next(100);
                    if (num >= 0 && num < 5)
                    {
                        outbitmap.SetPixel(x, y, Color.White);
                    }
                    else if(num >= 95 && num < 99)
                    {
                        outbitmap.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y, src_bitmap.GetPixel(x,y));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }
        public int otsu()
        {
            int[] histogram = new int[256];
            int count = 0;
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    histogram[avg]++;
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
            return refrence;
        }
        private void connentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            int[,] block = new int[src_bitmap.Height,src_bitmap.Width];
            int index =1;
            int the = otsu();
            Bitmap buffbitmap= new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    if (avg > the)
                    {
                        buffbitmap.SetPixel(x, y, Color.FromArgb(255,255,255));
                    }
                    else
                    {
                        buffbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    byte refrence = buffbitmap.GetPixel(x, y).R; 
                    if (refrence != 0)
                    {
                        if (x == 0 && y == 0)
                        {
                            block[y, x] = index;
                            index++;
                        }
                        else if (x != 0 && y == 0)
                        {
                            if (block[y, x - 1] != 0)
                            {
                                block[y, x] = block[y, x - 1];
                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                        else if (x == 0 && y != 0)
                        {
                            if (block[y - 1, x] != 0)
                            {
                                block[y, x] = block[y - 1, x];
                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                        else
                        {
                            if (block[y, x - 1] != 0 && block[y - 1, x] == 0)
                            {
                                block[y, x] = block[y, x - 1];
                            }
                            else if(block[y - 1, x] != 0 && block[y , x - 1] == 0)
                            {
                                block[y, x] = block[y - 1, x];
                            }
                            else if(block[y - 1, x] != 0 && block[y, x - 1] != 0)
                            {
                                block[y, x] = block[y - 1, x];
                                int top = block[y - 1, x];
                                int left = block[y, x - 1];
                                if (top != left)
                                {
                                    for(int j = 0; j < src_bitmap.Height; j++)
                                    {
                                        for(int i = 0; i < src_bitmap.Width; i++)
                                        {
                                            if (block[j, i] == left)
                                            {
                                                block[j, i] = top;
                                            }
                                        }
                                    }
                                }
                                
                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                    }
                    else
                    {
                        block[y, x] = 0;
                    }
                }
            }
            for(int y = 0; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    if (block[y, x] != 0)
                    {
                        //outbitmap.SetPixel(x, y, Color.FromArgb(255,0,0));
                        if(block[y, x] % 11==0)
                        {
                            outbitmap.SetPixel(x, y,Color.Red);
                        }
                        else if (block[y, x] %11== 1)
                        {
                            outbitmap.SetPixel(x, y,Color.Orange);
                        }
                        else if (block[y, x] % 11 == 2)
                        {
                            outbitmap.SetPixel(x, y, Color.Yellow);
                        }
                        else if (block[y, x] % 11 == 3)
                        {
                            outbitmap.SetPixel(x, y, Color.Green);
                        }
                        else if (block[y, x] % 11 == 4)
                        {
                            outbitmap.SetPixel(x, y, Color.Blue);
                        }
                        else if (block[y, x] % 11 == 5)
                        {
                            outbitmap.SetPixel(x, y, Color.Cyan);
                        }
                        else if (block[y, x] % 11 == 6)
                        {
                            outbitmap.SetPixel(x, y, Color.Purple);
                        }
                        else if (block[y, x] % 11 == 7)
                        {
                            outbitmap.SetPixel(x, y, Color.Chocolate);
                        }
                        else if (block[y, x] % 11 == 8)
                        {
                            outbitmap.SetPixel(x, y, Color.LightSeaGreen);
                        }
                        else if (block[y, x] % 11 == 9)
                        {
                            outbitmap.SetPixel(x, y, Color.Gold);
                        }
                        else if (block[y, x] % 11 == 10)
                        {
                            outbitmap.SetPixel(x, y, Color.Salmon);
                        }
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y,buffbitmap.GetPixel(x,y));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void InverseFunction(object sender, EventArgs e)
        {
            ManualThreshold m1 = new ManualThreshold();
            m1.Show();
            m1.InverseRotation(src_bitmap);
        }
        private void rectanglefunction(object sender, EventArgs e)
        {
            cut c = new cut();
            c.Show();
            c.rect(src_bitmap);
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            video v = new video();
            v.Show();
        }

        private void wayToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            int[,] block = new int[src_bitmap.Height, src_bitmap.Width];
            int index = 1;
            int the = otsu();
            Bitmap buffbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    if (avg > the)
                    {
                        buffbitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        buffbitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    byte refrence = buffbitmap.GetPixel(x, y).R;
                    if (refrence != 0)
                    {
                        if (x == 0 && y == 0)
                        {
                            block[y, x] = index;
                            index++;
                        }
                        else if (x != 0 && y == 0)
                        {
                            if (block[y, x - 1] != 0)
                            {
                                block[y, x] = block[y, x - 1];
                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                        else if (x == 0 && y != 0)
                        {
                            if (block[y - 1, x] != 0)
                            {
                                block[y, x] = block[y - 1, x];
                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                        else
                        {
                            if (block[y, x - 1] != 0 && block[y - 1, x] == 0)
                            {
                                block[y, x] = block[y, x - 1];
                            }
                            else if (block[y - 1, x] != 0 && block[y, x - 1] == 0)
                            {
                                block[y, x] = block[y - 1, x];
                            }
                            else if (block[y - 1, x] != 0 && block[y, x - 1] != 0)
                            {
                                block[y, x] = block[y - 1, x];
                                int top = block[y - 1, x];
                                int left = block[y, x - 1];
                                if (top != left)
                                {
                                    for (int j = 0; j < src_bitmap.Height; j++)
                                    {
                                        for (int i = 0; i < src_bitmap.Width; i++)
                                        {
                                            if (block[j, i] == left)
                                            {
                                                block[j, i] = top;
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                block[y, x] = index;
                                index++;
                            }
                        }
                    }
                    else
                    {
                        block[y, x] = 0;
                    }
                }
            }
            for(int y = 1; y < src_bitmap.Height; y++)
            {
                for(int x = 0; x < src_bitmap.Width; x++)
                {
                    if (block[y, x] != 0)
                    {
                        int center = block[y, x];
                        if (x == 0)
                        {
                            int topright = block[y - 1, x + 1];
                            if (topright != 0 && topright != center)
                            {
                                for (int j = 0; j < src_bitmap.Height; j++)
                                {
                                    for (int i = 0; i < src_bitmap.Width; i++)
                                    {
                                        if (block[j, i] == center)
                                        {
                                            block[j, i] = topright;
                                        }
                                    }
                                }
                            }
                        }
                        else if (x == src_bitmap.Width - 1)
                        {
                            int topleft = block[y - 1, x - 1];
                            if (topleft != 0 && topleft != center)
                            {
                                for (int j = 0; j < src_bitmap.Height; j++)
                                {
                                    for (int i = 0; i < src_bitmap.Width; i++)
                                    {
                                        if (block[j, i] == center)
                                        {
                                            block[j, i] = topleft;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            int topright = block[y - 1, x + 1];
                            int topleft = block[y - 1, x - 1];
                            if (topleft != 0 && topright == 0 && topleft != center)
                            {
                                for (int j = 0; j < src_bitmap.Height; j++)
                                {
                                    for (int i = 0; i < src_bitmap.Width; i++)
                                    {
                                        if (block[j, i] == center)
                                        {
                                            block[j, i] = topleft;
                                        }
                                    }
                                }
                            }
                            else if (topleft == 0 && topright != 0 && topright != center)
                            {
                                for (int j = 0; j < src_bitmap.Height; j++)
                                {
                                    for (int i = 0; i < src_bitmap.Width; i++)
                                    {
                                        if (block[j, i] == center)
                                        {
                                            block[j, i] = topright;
                                        }
                                    }
                                }
                            }
                            else if(topleft != 0 && topright != 0 && topleft != topright)
                            {
                                for (int j = 0; j < src_bitmap.Height; j++)
                                {
                                    for (int i = 0; i < src_bitmap.Width; i++)
                                    {
                                        if (block[j, i] == center)
                                        {
                                            block[j, i] = topleft;
                                        }
                                        if (block[j, i] == topright)
                                        {
                                            block[j, i] = topleft;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    if (block[y, x] != 0)
                    {
                        //outbitmap.SetPixel(x, y, Color.FromArgb(255,0,0));
                        if (block[y, x] % 11 == 0)
                        {
                            outbitmap.SetPixel(x, y, Color.Red);
                        }
                        else if (block[y, x] % 11 == 1)
                        {
                            outbitmap.SetPixel(x, y, Color.Orange);
                        }
                        else if (block[y, x] % 11 == 2)
                        {
                            outbitmap.SetPixel(x, y, Color.Yellow);
                        }
                        else if (block[y, x] % 11 == 3)
                        {
                            outbitmap.SetPixel(x, y, Color.Green);
                        }
                        else if (block[y, x] % 11 == 4)
                        {
                            outbitmap.SetPixel(x, y, Color.Blue);
                        }
                        else if (block[y, x] % 11 == 5)
                        {
                            outbitmap.SetPixel(x, y, Color.Cyan);
                        }
                        else if (block[y, x] % 11 == 6)
                        {
                            outbitmap.SetPixel(x, y, Color.Purple);
                        }
                        else if (block[y, x] % 11 == 7)
                        {
                            outbitmap.SetPixel(x, y, Color.Chocolate);
                        }
                        else if (block[y, x] % 11 == 8)
                        {
                            outbitmap.SetPixel(x, y, Color.LightSeaGreen);
                        }
                        else if (block[y, x] % 11 == 9)
                        {
                            outbitmap.SetPixel(x, y, Color.Gold);
                        }
                        else if (block[y, x] % 11 == 10)
                        {
                            outbitmap.SetPixel(x, y, Color.Salmon);
                        }
                    }
                    else
                    {
                        outbitmap.SetPixel(x, y, buffbitmap.GetPixel(x, y));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            Console.WriteLine(index);
            snr_(src_bitmap, outbitmap);
        }

        private void pseudoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lowpass l1 = new lowpass();
            l1.Show();
            l1.pseido(src_bitmap);
        }

        private void colorNegativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (src_bitmap == null) return;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    Color c = src_bitmap.GetPixel(x, y);
                    
                    outbitmap.SetPixel(x, y, Color.FromArgb((Byte)(255 - c.R), (Byte)(255 - c.G), (Byte)(255 - c.B)));
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void huffmanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            huffman h = new huffman();
            h.Show();
            h.decode(src_bitmap);
        }

        private void contrast5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int buff_ = int.Parse(toolStripTextBox4.Text);
            Console.WriteLine(buff_);
            double precent = (double)buff_ / 100;
            if (precent < 0 || precent > 0.5) return;
            int s1 = 0;
            int r1 =(int)( 255 * precent);
            int r2 = (int)(255 * (1-precent));
            int s2 = 255;
            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    double buff = 0;
                    byte value = src_bitmap.GetPixel(x, y).R;
                    if (value <= r1)
                    {
                        buff = 0;
                    }
                    else if (value >= r2)
                    {
                        buff = 255;
                    }
                    else if (value < r2 && value > r1)
                    {
                        buff = ((value - r1)* (s2 - s1) / (r2 - r1))  + s1;
                    }
                    outbitmap.SetPixel(x, y, Color.FromArgb((int)buff, (int)buff, (int)buff));
                }
            }
            pictureBox2.Image = outbitmap;
            chart1.Visible = true;
            int[] index = new int[256];
            for(int i = 0; i < 256; i++)
            {
                index[i] = i;
            }
            int[] out_ = new int[256];
            for(int i = 0; i < 256; i++)
            {
                if (i <= r1)
                {
                    out_[i] = 0;
                }
                else if (i >= r2)
                {
                    out_[i] = 255;
                }
                else
                {
                    out_[i] = ((i - r1)* (s2 - s1) / (r2 - r1))  + s1;
                }
            }
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "pixel";
            chart1.ChartAreas[0].AxisY.Title = "outpixel";
            chart1.Series.Add("pixel");
            chart1.Series["pixel"].ChartType = SeriesChartType.Line;
            chart1.Series["pixel"].Color = Color.Red;
            chart1.Series["pixel"].BorderWidth = 1;
            chart1.Series["pixel"].XValueType = ChartValueType.Int32;
            chart1.Series["pixel"].YValueType = ChartValueType.Int32;
            chart1.Series["pixel"].Points.DataBindXY(index, out_);
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
        }

        private void usecolorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectColor c = new ConnectColor();
            c.Show();
            c.put();
        }

        private void newBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outbitmap = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
            for(int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    outbitmap.SetPixel(x,y, Color.White);
                }
            }
            for (int y = 16; y < 64; y++)
            {
                for (int x = 16; x < 64; x++)
                {
                    outbitmap.SetPixel(x,y, Color.White);
                }
            }
            for (int i = 64; i < 128; i++)
            {
                outbitmap.SetPixel(i,i, Color.White);
            }
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    outbitmap.SetPixel(255-x, 255 - y, Color.White);
                }
            }
            for (int y = 16; y < 64; y++)
            {
                for (int x = 16; x < 64; x++)
                {
                    outbitmap.SetPixel(255-x, 255 - y, Color.White);
                }
            }
            for (int i = 64; i < 128; i++)
            {
                outbitmap.SetPixel(255-i, 255 - i, Color.White);
            }
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    outbitmap.SetPixel(255-x, y, Color.White);
                }
            }
            for (int y = 16; y < 64; y++)
            {
                for (int x = 16; x < 64; x++)
                {
                    outbitmap.SetPixel(255-x, y, Color.White);
                }
            }
            for (int i = 64; i < 128; i++)
            {
                outbitmap.SetPixel(255-i, i, Color.White);
            }
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    outbitmap.SetPixel(x,255- y, Color.White);
                }
            }
            for (int y = 16; y < 64; y++)
            {
                for (int x = 16; x < 64; x++)
                {
                    outbitmap.SetPixel(x,255- y, Color.White);
                }
            }
            for (int i = 64; i < 128; i++)
            {
                outbitmap.SetPixel(i,255- i, Color.White);
            }
            Form1 f = new Form1();
            f.Show();
            f.Set_pic_from_another_form(outbitmap);
        }

        private void crispening2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gradient g = new gradient();
            g.Show();
            g.crispening(src_bitmap);
        }

        private void inverseclick_Click(object sender, EventArgs e)
        {
            int xita = int.Parse(rotationtext.Text);
            int w0 = src_bitmap.Width;
            int h0 = src_bitmap.Height;
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
                        outbitmap.SetPixel(x, y, src_bitmap.GetPixel(x1, y1));

                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void postiveclick_Click(object sender, EventArgs e)
        {
            int xita = int.Parse(rotationtext.Text);
            int w0 = src_bitmap.Width;
            int h0 = src_bitmap.Height;
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
                    outbitmap.SetPixel(x1, y1, src_bitmap.GetPixel(x, y));
                }
            }

            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void basToolStripMenuItem_Click(object sender, EventArgs e)
        {
            basketball b=new basketball();
            b.Show();
        }

        private void first_Click(object sender, EventArgs e)
        {
            int ratio = int.Parse(smalltext.Text);
            if (src_bitmap.Width % ratio != 0 && src_bitmap.Height % ratio != 0) return;
            int w = (int)(src_bitmap.Width / ratio);
            int h = (int)(src_bitmap.Height / ratio);
            outbitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y += ratio)
            {
                for (int x = 0; x < src_bitmap.Width; x += ratio)
                {
                    if (x + ratio > 0 && y + ratio > 0 && x + ratio < src_bitmap.Width && y + ratio < src_bitmap.Height)
                    {
                        Color c = src_bitmap.GetPixel(x, y);
                        outbitmap.SetPixel(x / ratio, y / ratio, Color.FromArgb(c.R, c.G, c.B));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void average_small_Click(object sender, EventArgs e)
        {
            int ratio = int.Parse(smalltext.Text);
            if (src_bitmap.Width % ratio != 0 && src_bitmap.Height % ratio != 0) return;
            outbitmap = new Bitmap(src_bitmap.Width / ratio, src_bitmap.Height / ratio, PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y += ratio)
            {
                for (int x = 0; x < src_bitmap.Width; x += ratio)
                {
                    if (x + ratio > 0 && y + ratio > 0 && x + ratio < src_bitmap.Width && y + ratio < src_bitmap.Height)
                    {
                        int r = 0, g = 0, b = 0;
                        for (int j = 0; j < ratio; j++)
                        {
                            for (int i = 0; i < ratio; i++)
                            {
                                if (x + i > 0 && y + i > 0 && x + i < src_bitmap.Width && y + i < src_bitmap.Height)
                                {
                                    Color c = src_bitmap.GetPixel(x + i, y + j);
                                    r += c.R; g += c.G; b += c.B;
                                }
                            }
                        }
                        int rout = r / (ratio * ratio);
                        int gout = g / (ratio * ratio);
                        int bout = b / (ratio * ratio);
                        outbitmap.SetPixel(x / ratio, y / ratio, Color.FromArgb(rout, gout, bout));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }

        private void circlefunction(object sender, EventArgs e)
        {
            cut c = new cut();
            c.Show();
            c.circle(src_bitmap);
        }
        private void Polygon(object sender, EventArgs e)
        {
            cut c = new cut();
            c.Show();
            c.poly(src_bitmap);
        }
        private void irregular(object sender, EventArgs e)
        {
            cut c = new cut();
            c.Show();
            c.irre(src_bitmap);
        }
        private void magicwandfunction(object sender, EventArgs e)
        {
            cut c = new cut();
            c.Show();
            c.magic(src_bitmap);
        }
        
    }
}
