using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TrioCom
{
    public partial class Form3 : Form
    {
        public string Password
        {
            get
            {
                return (textBox1.Text);
            }
        }

        String PasswordFile;

        public Form3(String PasswordFile)
        {
            this.PasswordFile = PasswordFile;
            InitializeComponent();
       
            try
            {
                if (File.Exists(PasswordFile))
                {
                    StreamReader f = File.OpenText(PasswordFile);
                    textBox1.Text = f.ReadLine();
                    f.Close();
                }

            }catch(Exception ex){
                MessageBox.Show(ex.ToString());
            }

        }

     

        private void button1_Click(object sender, EventArgs e)
        {
          
            this.DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
