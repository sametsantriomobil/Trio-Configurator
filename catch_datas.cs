using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrioCom
{
    public partial class catch_datas : Form
    {
        public String catch_text="";
        public String foundtext="";


        public catch_datas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            catch_text = textBox1.Text;
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            catch_text = "";

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true){
                if (foundtext != "")
                {
                    textBox2.AppendText(foundtext+"\n");
                    foundtext = "";
                }


            }
        }
    }
}
