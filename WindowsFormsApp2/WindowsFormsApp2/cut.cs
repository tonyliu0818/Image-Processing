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
    public partial class cut : Form
    {
        public cut()
        {
            InitializeComponent();
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
                else
                {
                    toolStripStatusLabel14.Text = "_";
                    toolStripStatusLabel16.Text = "_";
                    toolStripStatusLabel18.Text = "_";
                    toolStripStatusLabel20.Text = "_";
                    toolStripStatusLabel22.Text = "_";
                }
            }
        }
        public void snr_(Bitmap bitmap1, Bitmap bitmap2)
        {
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
                toolStripStatusLabel24.Text = "與原圖相同";
            }
            else
            {
                double snr = (double)10 * Math.Log10(outcount / difference);
                snr = Math.Round(snr, 2);
                toolStripStatusLabel24.Text = snr.ToString() + "db";
            }*/
        }
        public Bitmap src_bitmap,outbitmap,copy_bitmap;
        string draw;
        Point start,size,min_;
        List<Point> irre_point = new List<Point>();
        bool linecatch = false;
        List<Point> poly_point = new List<Point>();
        Point now_point;
        List<Point> MagicPoint = new List<Point>();
        List<Point> CopyPoint = new List<Point>();
        Point currentpoint, refrence;
        int magicstate = 0;
        public void rect(Bitmap buff)
        {
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            draw = "rect";
            for(int y = 0; y < buff.Height; y++)
            {
                for(int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            label1.Text = "rectangle";
            pictureBox1.Image = src_bitmap;
        }
        public void poly(Bitmap buff)
        {
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            draw = "poly";
            for (int y = 0; y < buff.Height; y++)
            {
                for (int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            pictureBox3.Visible = true;
            pictureBox1.Image = src_bitmap;
            label1.Text = "polygon";
        }
        public void circle(Bitmap buff)
        {
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            draw = "circle";
            for (int y = 0; y < buff.Height; y++)
            {
                for (int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label1.Text = "circle";
        }
        public void irre(Bitmap buff)
        {
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            draw = "irre";
            for (int y = 0; y < buff.Height; y++)
            {
                for (int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label1.Text = "irregular";
        }
        public void magic(Bitmap buff)
        {
            src_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            copy_bitmap = new Bitmap(buff.Width, buff.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            draw = "magic";
            pictureBox3.Visible = true;
            for (int y = 0; y < buff.Height; y++)
            {
                for (int x = 0; x < buff.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                    copy_bitmap.SetPixel(x, y, buff.GetPixel(x, y));
                }
            }
            pictureBox1.Image = src_bitmap;
            label1.Text = "magicwand";
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (draw == "rect")
            {
                float[] dashValues = { 5, 5, 5, 5 };
                Pen pen = new Pen(Brushes.DarkMagenta);
                pen.DashPattern = dashValues;
                pen.Width = 2.0F;
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;//哪一種形式的轉彎
                e.Graphics.DrawRectangle(pen, new Rectangle(Math.Min(start.X, min_.X), Math.Min(start.Y, min_.Y), size.X, size.Y));
                pen.Dispose();
                pictureBox1.Refresh();
            }
            else if (draw == "circle")
            {
                float[] dashValues = { 5, 5, 5, 5 };
                Pen pen = new Pen(Brushes.DarkMagenta);
                pen.DashPattern = dashValues;
                pen.Width = 2.0F;
                e.Graphics.DrawEllipse(pen, Math.Min(start.X, min_.X), Math.Min(start.Y, min_.Y), Math.Max(size.X, size.Y), Math.Max(size.X, size.Y));
                pictureBox1.Refresh();
            }
            else if (draw == "irre")
            {
                float[] dashValues = { 5, 5, 5, 5 };
                Pen pen = new Pen(Color.FromArgb(255, 0, 255, 0));
                pen.DashPattern = dashValues;
                pen.Width = 2.0F;
                for (int i = 0; i <= irre_point.Count - 2; i++) // points.Count - 2 因為兩兩連線少1，還有index減1
                {
                    e.Graphics.DrawLine(pen, irre_point[i], irre_point[i + 1]);
                }
                pictureBox1.Refresh();
            }
            else if (draw == "poly")
            {
                if (!linecatch)
                {
                    float[] dashValues = { 5, 5, 5, 5 };
                    Pen pen = new Pen(Brushes.DarkMagenta);
                    pen.DashPattern = dashValues;
                    pen.Width = 2.0F;
                    if (poly_point.Count > 0)//可以動的虛線
                    {
                        e.Graphics.DrawLine(pen, poly_point[poly_point.Count - 1], now_point);
                    }
                    if (poly_point.Count > 1)//所有list的縣
                    {
                        for (int i = 0; i < poly_point.Count - 1; i++)
                        {
                            e.Graphics.DrawLine(pen, poly_point[i], poly_point[i + 1]);
                        }
                    }

                }
                if (linecatch)
                {
                    float[] dashValues = { 5, 5, 5, 5 };

                    Pen pen1 = new Pen(Brushes.Gold);
                    pen1.DashPattern = dashValues;
                    pen1.Width = 2.0F;
                    if (poly_point.Count > 1)//補回來之前check掉的縣
                    {
                        for (int i = 0; i < poly_point.Count - 1; i++)
                        {
                            e.Graphics.DrawLine(pen1, poly_point[i], poly_point[i + 1]);
                        }
                    }
                    e.Graphics.DrawLine(pen1, poly_point[0], poly_point[poly_point.Count - 1]);
                    paintPolygon(poly_point);
                    //linecatch = false;
                }
                pictureBox1.Refresh();
            }
            else if (draw == "magic")
            {
                /*if (magicstate == 2)
                {
                    Bitmap buffbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    buffbitmap = src_bitmap;
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            buffbitmap.SetPixel(refrence.X + x, refrence.Y + y, Color.FromArgb(255, 0, 0));
                            buffbitmap.SetPixel(currentpoint.X + x, currentpoint.Y + y, Color.FromArgb(0, 255, 0));
                        }
                    }
                    pictureBox1.Image = buffbitmap;
                    float[] dashValues = { 5, 5, 5, 5 };
                    Pen pen = new Pen(Brushes.DarkMagenta);
                    pen.DashPattern = dashValues;
                    pen.Width = 2.0F;
                    Pen pen2 = new Pen(Brushes.Bisque);
                    pen2.DashPattern = dashValues;
                    pen2.Width = 2.0F;
                    for (int i = 0; i <= MagicPoint.Count - 2; i++) // points.Count - 2 因為兩兩連線少1，還有index減1
                    {
                        e.Graphics.DrawLine(pen, MagicPoint[i], MagicPoint[i + 1]);
                        e.Graphics.DrawLine(pen2, CopyPoint[i], CopyPoint[i + 1]);
                    }
                    //
                    pictureBox1.Refresh();
                    
                }*/
                
            }
        }
        private void bitmap1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw == "rect")
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.Button == MouseButtons.Left && e.X >= 0 && e.Y >= 0)
                {
                    Point buff = e.Location;
                    min_ = new Point(Math.Min(start.X, buff.X), Math.Min(start.Y, buff.Y));
                    size = new Point(Math.Abs(start.X - buff.X), Math.Abs(start.Y - buff.Y));

                }
            }
            else if (draw == "circle")
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.Button == MouseButtons.Left && e.X >= 0 && e.Y >= 0)
                {
                    Point buff = e.Location;
                    min_ = new Point(Math.Min(start.X, buff.X), Math.Min(start.Y, buff.Y));

                    size = new Point(Math.Abs(start.X - buff.X), Math.Abs(start.Y - buff.Y));
                }
            }
            else if (draw == "irre")
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.X >= 0 && e.Y >= 0 && e.Button == MouseButtons.Left)
                {
                    irre_point.Add(new Point(e.X, e.Y));
                }
            }
            else if (draw == "poly")
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.X >= 0 && e.Y >= 0)
                    now_point = e.Location;
            }
            else if (draw == "magic")
            {
                if ((e.X < src_bitmap.Width) && (e.Y < src_bitmap.Height) && e.X >= 0 && e.Y >= 0 && e.Button == MouseButtons.Left&&magicstate==2)
                {
                    //button2.Visible = true;
                    int outx = e.X - refrence.X + currentpoint.X;
                    int outy = e.Y - refrence.Y + currentpoint.Y;
                    /*MagicPoint.Add(new Point(e.X, e.Y));
                    CopyPoint.Add(new Point(e.X - refrence.X + currentpoint.X, e.Y - refrence.Y + currentpoint.Y));*/
                    for(int y = -3; y <= 3; y++)
                    {
                        for(int x = -3; x <= 3; x++)
                        {
                            if (outx+x > 0 && outx+x < src_bitmap.Width - 1 && outy+y > 0 && outy+y < src_bitmap.Height - 1&& e.X + x > 0 && e.X + x < src_bitmap.Width - 1 && e.Y + y > 0 && e.Y + y < src_bitmap.Height - 1)
                            {
                                outbitmap.SetPixel(outx+x, outy+y, copy_bitmap.GetPixel(e.X+x, e.Y+y));
                            }
                        }
                    }
                    
                    pictureBox2.Image = outbitmap;
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (draw == "poly")
            {
                linecatch = false;
                poly_point.Clear();
            }
            else if (draw == "magic")
            {
                magicstate = 0;
                button2.Visible = false;
                MagicPoint.Clear();
                CopyPoint.Clear();
                refrence.X=copy_bitmap.Width+1000;
                refrence.Y = copy_bitmap.Height+1000;
                currentpoint.X = copy_bitmap.Width+1000;
                currentpoint.Y = copy_bitmap.Height+1000;
                for(int y = 0; y < src_bitmap.Height; y++)
                {
                    for(int x = 0; x < src_bitmap.Width; x++)
                    {
                        src_bitmap.SetPixel(x, y, copy_bitmap.GetPixel(x,y));
                    }
                }
                pictureBox1.Image = copy_bitmap;
                pictureBox2.Image = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            magicwandfunction();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            f.Text = "New Form";
            f.Set_pic_from_another_form(outbitmap);
            this.Hide();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            f.show_chart1_combine(outbitmap);
        }

        private void mouseup(object sensor, MouseEventArgs e)
        {
            if (draw == "rect")
            {
                if (size.X == 0 || size.Y == 0) return;
                if (src_bitmap == null) return;
                outbitmap = new Bitmap(size.X, size.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                for (int y = 0; y < size.Y; y++)
                {
                    for (int x = 0; x < size.X; x++)
                    {
                        outbitmap.SetPixel(x, y, src_bitmap.GetPixel(min_.X + x, min_.Y + y));
                    }
                }
                pictureBox2.Image = outbitmap;
                snr_(src_bitmap, outbitmap);
            }
            else if (draw == "circle")
            {
                if (size.X == 0 || size.Y == 0) return;
                int width = Math.Max(size.X, size.Y);
                outbitmap = new Bitmap(width, width, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                int radius = width / 2;
                int X = min_.X + radius;
                int Y = min_.Y + radius;
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        outbitmap.SetPixel(x, y, Color.FromArgb(240, 240, 240));
                    }
                }
                for (int y = min_.Y; y < min_.Y + width; y++)
                {
                    for (int x = min_.X; x < min_.X + width; x++)
                    {
                        if ((x - X) * (x - X) + (y - Y) * (y - Y) <= radius * radius)
                        {
                            if (x > 0 && y > 0 && x < src_bitmap.Width && y < src_bitmap.Height)
                            {
                                outbitmap.SetPixel(x - min_.X, y - min_.Y, src_bitmap.GetPixel(x, y));
                            }
                        }
                    }
                }

                pictureBox2.Image = outbitmap;
                snr_(src_bitmap, outbitmap);
            }
            else if (draw == "irre")
            {
                if (irre_point.Count == 0) return;
                paintPolygon(irre_point);
                
            }
           
        }
        private void mousedown(object sensor, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (draw == "irre")
                {
                    irre_point.Clear();
                }
                start = e.Location;
                if (draw == "irre")
                {
                    irre_point.Add(start);
                }
                Invalidate();//xy不動
            }
        }
        private void mousedoubleclick(object sensor, MouseEventArgs e)
        {
            if (draw == "poly")
            {
                if (!linecatch && e.Button == MouseButtons.Left)
                {
                    poly_point.Add(e.Location);
                }
                if (e.Button == MouseButtons.Right && poly_point.Count > 2)
                {
                    linecatch = true;
                }
            }
            else if (draw == "magic")
            {
                outbitmap = src_bitmap;
                if (e.Button == MouseButtons.Left )
                {
                    if (magicstate == 0)
                    {
                        Bitmap buffbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        refrence = e.Location;
                        
                        buffbitmap = src_bitmap;
                        for (int y = -1; y <= 1; y++)
                        {
                            for (int x = -1; x <= 1; x++)
                            {
                                buffbitmap.SetPixel(refrence.X + x, refrence.Y + y, Color.FromArgb(255, 0, 0));
                            }
                        }
                        pictureBox1.Image = buffbitmap;
                        magicstate++;
                    }
                    else if (magicstate == 1)
                    {
                        Bitmap buffbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        currentpoint = e.Location;
                        buffbitmap = src_bitmap;
                        for (int y = -1; y <= 1; y++)
                        {
                            for (int x = -1; x <= 1; x++)
                            {
                                buffbitmap.SetPixel(refrence.X + x, refrence.Y + y, Color.FromArgb(255, 0, 0));
                                buffbitmap.SetPixel(currentpoint.X + x, currentpoint.Y + y, Color.FromArgb(0, 255, 0));
                            }
                        }
                        pictureBox1.Image = buffbitmap;
                        magicstate++;
                        //currentpoint = e.Location;
                    }
                }
            }
        }
        public void paintPolygon(List<Point> ppp)
        {
            if (src_bitmap == null) return;
            //抓邊界
            int maxx = 0, maxy = 0, minx = 10000, miny = 100000;
            for (int i = 0; i < ppp.Count; i++)
            {
                maxx = Math.Max(ppp[i].X, maxx);
                maxy = Math.Max(ppp[i].Y, maxy);
                minx = Math.Min(ppp[i].X, minx);
                miny = Math.Min(ppp[i].Y, miny);
            }
            outbitmap = new Bitmap(maxx - minx, maxy - miny, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //填滿
            for (int y = miny; y < maxy; y++)
            {
                for (int x = minx; x < maxx; x++)
                {
                    outbitmap.SetPixel(x - minx, y - miny, Color.FromArgb(240, 240, 240));
                }
            }
            for (int y = miny; y < maxy; y++)
            {
                for (int x = minx; x < maxx; x++)
                {
                    int count = 0;
                    int adjacent = 0;
                    for (int current_point = 0; current_point < ppp.Count; current_point++)
                    {
                        adjacent = current_point == ppp.Count - 1 ? 0 : current_point + 1;//是不是在最後一個點?是就要比較第0個點和最後一個點,不然就point相鄰的點比對
                        if (ppp[current_point].Y != ppp[adjacent].Y)//避免射線與判斷的線條重疊,平行的話不知道怎麼計數
                        {
                            if ((y >= ppp[adjacent].Y && y < ppp[current_point].Y) || (y >= ppp[current_point].Y && y < ppp[adjacent].Y))//判別射線是否打到線裡面
                            {
                                if (((y - ppp[current_point].Y) * (ppp[adjacent].X - ppp[current_point].X) / (ppp[adjacent].Y - ppp[current_point].Y)) + ppp[current_point].X > x)
                                {//用相似三角形去思考,打過去的焦點會在右邊穿過去
                                    count++;
                                }
                            }
                        }
                    }
                    if (count % 2 != 0)
                    {
                        outbitmap.SetPixel(x - minx, y - miny, src_bitmap.GetPixel(x, y));
                    }
                }
            }
            pictureBox2.Image = outbitmap;
            snr_(src_bitmap, outbitmap);
        }
        public void magicwandfunction()
        {

            //抓邊界
            int maxx = 0, maxy = 0, minx = 10000, miny = 100000;
            for (int i = 0; i < MagicPoint.Count; i++)
            {
                maxx = Math.Max(MagicPoint[i].X, maxx);
                maxy = Math.Max(MagicPoint[i].Y, maxy);
                minx = Math.Min(MagicPoint[i].X, minx);
                miny = Math.Min(MagicPoint[i].Y, miny);
            }

            outbitmap = new Bitmap(src_bitmap.Width, src_bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    outbitmap.SetPixel(x, y, copy_bitmap.GetPixel(x, y));
                }
            }
            bool exist = false;
            pictureBox1.Image = src_bitmap;
            //填滿
            int dx = currentpoint.X - refrence.X;
            int dy = currentpoint.Y - refrence.Y;
            for (int y = miny; y < maxy; y++)
            {
                for (int x = minx; x < maxx; x++)
                {
                    int count = 0;
                    int adjacent = 0;
                    for (int current_point = 0; current_point < MagicPoint.Count; current_point++)
                    {
                        adjacent = current_point == MagicPoint.Count - 1 ? 0 : current_point + 1;//是不是在最後一個點?是就要比較第0個點和最後一個點,不然就point相鄰的點比對
                        if (MagicPoint[current_point].Y != MagicPoint[adjacent].Y)//避免射線與判斷的線條重疊,平行的話不知道怎麼計數
                        {
                            if ((y >= MagicPoint[adjacent].Y && y < MagicPoint[current_point].Y) || (y >= MagicPoint[current_point].Y && y < MagicPoint[adjacent].Y))//判別射線是否打到線裡面
                            {
                                if (((y - MagicPoint[current_point].Y) * (MagicPoint[adjacent].X - MagicPoint[current_point].X) / (MagicPoint[adjacent].Y - MagicPoint[current_point].Y)) + MagicPoint[current_point].X > x)
                                {//用相似三角形去思考,打過去的焦點會在右邊穿過去
                                    count++;
                                }
                            }
                        }
                    }
                    if (count % 2 != 0)
                    {
                        if (x == refrence.X && y == refrence.Y) exist = true;
                        if ((dx + x) > 0 && (dx + x) < src_bitmap.Width && (dy + y) > 0 && (dy + y) < src_bitmap.Height)
                        {
                            outbitmap.SetPixel(dx + x, dy + y, copy_bitmap.GetPixel(x, y));
                        }
                    }
                }
            }

            for (int y = 0; y < src_bitmap.Height; y++)
            {
                for (int x = 0; x < src_bitmap.Width; x++)
                {
                    src_bitmap.SetPixel(x, y, copy_bitmap.GetPixel(x, y));
                }
            }
            if (exist)
            {
                pictureBox2.Image = outbitmap;
            }
            else
            {
                pictureBox2.Image = src_bitmap;
            }
            snr_(src_bitmap, outbitmap);
        }
    }
}
