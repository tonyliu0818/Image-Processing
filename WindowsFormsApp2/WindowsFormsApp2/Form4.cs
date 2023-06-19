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
    public partial class Form4 : Form
    {
        public Form4()
        {
            
            InitializeComponent();
        }

        int count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            label1.Text = count.ToString()+"%";
            count++;
            if (count > 101)
            {
                timer1.Enabled = false;
                Form1 form = new Form1();
                form.Show();
                this.Hide();
            }
        }
    }
}
