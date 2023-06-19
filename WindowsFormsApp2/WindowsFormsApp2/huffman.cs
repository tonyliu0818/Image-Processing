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
    public partial class huffman : Form
    {
        public huffman()
        {
            InitializeComponent();
        }
        public string tobinary(int num,int codemax)
        {
            int rem = 0;
            string buff=string.Empty;
            while (num > 0)
            {
                rem = num % 2;
                num = num / 2;
                buff = rem.ToString() + buff;
            }
            string result=string.Empty;
            if (buff.Length < 8)
            {
                for(int i = 0; i < 8 - buff.Length; i++)
                {
                    result = 0.ToString() + result;
                }
                result = result + buff;
            }
            else
            {
                result = buff;
            }
            return result;
        }
        class tree
        {
            public double name { get; set; }
            public double left { get; set; }
            public double right { get;set; }
            public double up { get;set; }
            public double weight { get; set; }
            public tree(double Name,double Left,double Right,double Up,double Weight)
            {
                up = Up;
                name = Name;
                right = Right;
                left = Left;
                weight = Weight;
            }
        }
        class huffman_code
        {
            public double pixel { get; set; }
            public double p{ get;set; }
            public string code { get; set; }
            public huffman_code(double pixel_,double probility_,string c)
            {
                pixel = pixel_;
                p = probility_;
                code = c;
            }
        }
        double codesize = 0;
        public void putlabel(double []p,int codemax)
        {
            DataGridView dataGridview = new DataGridView();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Pixel", typeof(string));
            dataTable.Columns.Add("機率", typeof(double));
            dataTable.Columns.Add("encode", typeof(string));
            double sum = 0;
            int count = 0;
            for(int i = 0; i < 256; i++)
            {
                if (p[i] != 0)
                {
                    DataRow row1 = dataTable.NewRow();
                    row1[0] = i.ToString();
                    row1[1] = Math.Round(p[i],7);
                    row1[2] = tobinary(i, codemax);
                    dataTable.Rows.Add(row1);
                    dataGridView1.DataSource = dataTable;
                    sum += p[i];
                    codesize += p[i] * 8;
                    count++;
                }
            }
            DataRow row2 = dataTable.NewRow();
            row2[0] = "total";
            row2[1] = Math.Round(sum);
            dataTable.Rows.Add(row2);
            dataGridView1.DataSource = dataTable;
            DataRow row3 = dataTable.NewRow();
            row3[0] = "codesize";
            row3[1] = Math.Round(codesize);
            dataTable.Rows.Add(row3);
            dataGridView1.DataSource = dataTable;
            dataGridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void decode(Bitmap bitmap)
        {
            int[] histogram = new int[256];
            for(int y = 0; y < bitmap.Height; y++)
            {
                for(int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int avg = (c.R + c.G + c.B) / 3;
                    histogram[avg]++;
                }
            }
            double size = bitmap.Width * bitmap.Height;
            double[] p = new double[256];
            double[] p1 = new double[256];
            double[] sequence = new double[256];
            double sum = 0;
            int count=0;
            int cc = -1;
            for (int i = 0; i < 256; i++)
            {
                p[i] = (histogram[i] / size);
                sum += p[i];               
                if (p[i] != 0)
                {
                    p1[count] = p[i];
                    sequence[count] = i;
                    count++;
                    cc++;
                }
            }
            int len = p1.Length;
            for (int i = 1; i <= len - 1; i++)
            {
                for (int j = 1; j <= len - i; j++)
                {
                    if (p1[j] < p1[j - 1])
                    {
                        double temp = p1[j];
                        double temp1 = sequence[j];
                        p1[j] = p1[j - 1];
                        sequence[j] = sequence[j - 1];
                        p1[j - 1] = temp;
                        sequence[j - 1] = temp1;
                    }
                }
            }
            int codemax=findcodemax(cc);
            putlabel(p,codemax);
            huffman_tree(p1,sequence,256-count);
            pictureBox1.Image = bitmap;
        }
        public void huffman_tree(double[] list, double[] sequence,int startindex)
        {
            List<double> p = new List<double> { };
            List<double> pixel = new List<double> { };
            List<double> processp = new List<double> { };
            List<double> processpixel = new List<double> { };
            for (int i = startindex; i < 256; i++)
            {
                p.Add(list[i]);
                processp.Add(list[i]);
                pixel.Add(sequence[i]);
                processpixel.Add(sequence[i]);
            }
            bool rightexist = false;
            bool leftexist = false;
            int treenode = 0;
            tree[] trees = new tree[10000];
            for(int i = 0; i < 10000; i++)
            {
                trees[i] = new tree(-1, -1, -1, -1, -1);
            }
            while (processp.Count>1)
            {
                double p1 = processp[0];
                double pixel1 = processpixel[0];
                double p2 = processp[1];
                double pixel2 = processpixel[1];
                double add = p1 + p2;
                //把pixel和機率放到list尾端並移除第1個和第二個element
                processp.Add(add);
                processpixel.Add(add);
                processp.RemoveAt(0);
                processp.RemoveAt(0);
                processpixel.RemoveAt(0);
                processpixel.RemoveAt(0);
                int len = processp.Count;
                //sort
                for (int i = 1; i <= len - 1; i++)
                {
                    for (int j = 1; j <= len - i; j++)
                    {
                        if (processp[j] < processp[j - 1])
                        {
                            double temp = processp[j];
                            double temp1 = processpixel[j];
                            processp[j] = processp[j - 1];
                            processpixel[j] = processpixel[j - 1];
                            processp[j - 1] = temp;
                            processpixel[j - 1] = temp1;
                        }
                    }
                }
                //      add
                //     /    \
                //    p1    p2
                trees[treenode].name = add;
                trees[treenode].weight = add;
                trees[treenode].left = pixel1;
                trees[treenode].right = pixel2;
                rightexist = false;
                leftexist = false;
                //找尋有無重複計數的node
                for (int i = 0; i < treenode; i++)
                {
                    if (trees[i].name == pixel1)
                    {
                        trees[i].up = trees[treenode].name;
                        leftexist = true;
                        //break;
                    }
                    else if (trees[i].name == pixel2)
                    {
                        trees[i].up = trees[treenode].name;
                        rightexist = true;
                        //break;
                    }
                }
                treenode++;
                if (!leftexist && rightexist)
                {
                    trees[treenode].name = pixel1;
                    trees[treenode].weight = p1;
                    trees[treenode].up = add;
                    treenode++;
                }
                else if (leftexist && !rightexist)
                {
                    trees[treenode].name = pixel2;
                    trees[treenode].weight = p2;
                    trees[treenode].up = add;
                    treenode++;

                }
                else if (!leftexist && !rightexist)
                {
                    trees[treenode].name = pixel1;
                    trees[treenode].weight = p1;
                    trees[treenode].up = add;
                    treenode++;
                    trees[treenode].name = pixel2;
                    trees[treenode].weight = p2;
                    trees[treenode].up = add;
                    treenode++;
                }
            }
            huffman_code[] code = new huffman_code[pixel.Count];
            for(int i = 0; i < pixel.Count; i++)
            {
                code[i] = new huffman_code(0, 0, "");
            }
            //從tree裡找result
            for(int i = 0; i < pixel.Count; i++)
            {
                double currentlabel=0;
                double currentweight=0;
                double currentleft=0;
                double currentright=0;
                double currentup=0;
                for(int t = 0; t < treenode; t++)
                {
                    //找目前要找的pixel
                    if (trees[t].name == pixel[i])
                    {
                        currentlabel = trees[t].name;
                        currentleft = trees[t].left;
                        currentright = trees[t].right;
                        currentup = trees[t].up;
                        currentweight = trees[t].weight;
                        break;
                    }
                }
                string result = string.Empty;
                while (true)
                {
                    //走到尾端了
                    if (currentweight == 1)
                    {

                        code[i] = new huffman_code(pixel[i], p[i], result);
                        break;
                    }
                    else
                    {
                        //用來尋找現在跑的pixel上面是誰,並分辨是在左邊還右邊
                        for (int t = 0; t < treenode; t++)
                        {
                            if (trees[t].left == currentlabel)
                            {
                                result =  "0" +result;
                                currentlabel = trees[t].name;
                                currentleft = trees[t].left;
                                currentright = trees[t].right;
                                currentup = trees[t].up;
                                currentweight = trees[t].weight;
                                break;
                            }
                            else if(trees[t].right == currentlabel)
                            {
                                result = "1"+result ;
                                currentlabel = trees[t].name;
                                currentleft = trees[t].left;
                                currentright = trees[t].right;
                                currentup = trees[t].up;
                                currentweight = trees[t].weight;
                                break;
                            }
                        }
                    }
                }
            }
            //put result to datagridview
            DataGridView dataGridview = new DataGridView();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Pixel", typeof(string));
            dataTable.Columns.Add("機率", typeof(double));
            dataTable.Columns.Add("encode", typeof(string));
            double sum = 0;
            double codesize2 = 0;
            for (int i = 0; i < p.Count; i++)
            {
                DataRow row1 = dataTable.NewRow();
                row1[0] = code[i].pixel.ToString();
                row1[1] = Math.Round(code[i].p,7);
                row1[2] = code[i].code;
                dataTable.Rows.Add(row1);
                dataGridView2.DataSource = dataTable;
                sum += p[i];
                codesize2 += p[i] * code[i].code.Length;
            }
            DataRow row2 = dataTable.NewRow();
            row2[0] = "total";
            row2[1] = sum;
            dataTable.Rows.Add(row2);
            dataGridView2.DataSource = dataTable;
            DataRow row3 = dataTable.NewRow();
            row3[0] = "codesize";
            row3[1] = Math.Round(codesize2,7);
            dataTable.Rows.Add(row3);
            dataGridView2.DataSource = dataTable;
            DataRow row4 = dataTable.NewRow();
            row4[0] = "Compress Ratio";
            row4[1] = Math.Round(codesize/codesize2,7);
            dataTable.Rows.Add(row4);
            dataGridView2.DataSource = dataTable;
            dataGridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public int findcodemax(int count)
        {
            if (count < 0) return 0;
            int codemax = 0;
            int p1 = count & 128;
            int p2 = count & 64;
            int p3 = count & 32;
            int p4 = count & 16;
            int p5 = count & 8;
            int p6 = count & 4;
            int p7 = count & 2;
            int p8 = count & 1;
            if (p1 > 0)
            {
                codemax = 8;
            }
            else if (p2 > 0)
            {
                codemax = 7;
            }
            else if (p3 > 0)
            {
                codemax = 6;
            }
            else if (p4 > 0)
            {
                codemax = 5;
            }
            else if (p5 > 0)
            {
                codemax = 4;
            }
            else if (p6 > 0)
            {
                codemax = 3;
            }
            else if (p7 > 0)
            {
                codemax = 2;
            }
            else if (p8 > 0)
            {
                codemax = 1;
            }
            return codemax;
        } 
    }
}
