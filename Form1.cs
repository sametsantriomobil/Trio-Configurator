using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;


namespace TrioCom
{
    public partial class Form1 : Form
    {
        Int32[] baudrates = { 9600, 115200 };
        String[] ports;
        String PortName;

        Int32 Baudrate;
        String yazi;
        Read data;
        SerialPort port;
        bool durum=false;
        SET set;
        String TrioPath;
        String PasswordFile;
        catch_datas ctch;


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
   
            TrioFile();
            port_scan();
            baudrate_scan();
            data = new Read();
            set = new SET();

            Password_Read();
      
        }

        private void Password_Read()
        {
            PasswordFile = TrioPath + "password.tpf";
            try
            {
                if (File.Exists(PasswordFile))
                {
                    StreamReader f = File.OpenText(PasswordFile);
                    set.Password = f.ReadLine();
                    f.Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.ToString());
            }
        }

        private void TrioFile()
        {
            TrioPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TrioMobil\\";

            bool TPE = System.IO.Directory.Exists(TrioPath);
            if (!TPE)
                System.IO.Directory.CreateDirectory(TrioPath);
   
            log_file.InitialDirectory = TrioPath;
            save_file.InitialDirectory = TrioPath;
            open_file.InitialDirectory = TrioPath;

        }


        private void port_write(String data)
        {

            if (durum == false)
            {
                ShowMessageBox("Please open port.");
            }
            else
            {
                port.Write(data);
            }

        }

        private void port_scan()
        {
            CBPorts.Items.Clear();
            ports = SerialPort.GetPortNames();
   
            CBPorts.Items.AddRange(ports);
            
        }

        private void baudrate_scan()
        {
            int i = 0;
            while (i < baudrates.Length)
            {

                CBBaudrates.Items.Add(baudrates[i].ToString());
                i++;
            }

            CBBaudrates.Text = baudrates[1].ToString();
        }

    

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F5))
            {
                Connect();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void Connect()
        {
           
            try
            {

                if (durum==false)
                {
                    if (CBPorts.SelectedIndex > -1)
                        PortName = CBPorts.SelectedItem.ToString();
                    else
                    {
                        ShowMessageBox("Please select port!");
                        return;
                    }

                    if (CBBaudrates.SelectedIndex > -1)
                        Baudrate = Convert.ToInt32(CBBaudrates.SelectedItem.ToString());
                    else
                    {
                        ShowMessageBox("Please select baudrate!");
                        return;
                    }


                    port = new SerialPort(PortName, Baudrate);
                    //port.DataReceived += new SerialDataReceivedEventHandler(verileri_al);

                 

                    if (port.IsOpen==false)      
                        port.Open();

                    backgroundWorker2.RunWorkerAsync();
                    BConnect.Text = "Disconnect";
                    SSDurum.Text = "Connected.";
                    durum = true;
                 
                }
                else
                {
                    backgroundWorker2.WorkerSupportsCancellation = true;
                    backgroundWorker2.CancelAsync();
                    port.Close();
                    BConnect.Text = "Connect";
                    SSDurum.Text = "Connection is closed.";
                    durum = false;
                }


            }
            catch (Exception ex)
            {
                ShowMessageBox("ERROR :: " + ex.Message, "Error!");
            }

          

         
        }

      /*  private void verileri_al(object sender,
                        SerialDataReceivedEventArgs e)
        {
       
            SerialPort sp = (SerialPort)sender;
            if (port.BytesToRead > 0)
            read(sp);
          
        }
        */

        private void read()
        {

      
                if (Log.InvokeRequired)
                {
                    SSDurum.Text = "Reading...";
                    yazi = port.ReadLine();

           
                    Log.SelectionColor = Color.Red;
                    String date = " \n[" + DateTime.Now.ToString() + "] :";
                    Log.AppendText(date);
                    Log.SelectionColor = Color.Black;
                    
                    Log.AppendText(yazi);

                    // Logging
                    save_log(date + yazi);

                    String y = yazi.Replace(" ", "");
                    data.get(y);

                    // catch_data(yazi);
                    get_imei();
                    get_gsm_signal();
                    get_battery_level();
                    detect_set_command();

                
            }

           
        }

        private void get_imei()
        {
            TSimei.Text = data.imei;
        }

        private void get_gsm_signal()
        {
           
           
            if (data.gsm_signal != "")
            {
                //toolStripLabel2.Text = "GSM Signal Power : " + data.gsm_signal;
                toolStripProgressBar1.ToolTipText = "GSM Signal Power : " + data.gsm_signal; 
                String[] d = data.gsm_signal.Split(',');
                int value = Convert.ToInt32(d[0]) * 10;

                if (value.GetType() == typeof(int))
                {

                    toolStripProgressBar1.Value = value;
                }
                data.gsm_signal = "";
            }

        }

        private void get_battery_level()
        {
         
            if (data.battery_level != "")
            {
                Regex regex = new Regex(@"\d+");
                Match match = regex.Match(data.battery_level);

               // toolStripLabel3.Text = "Battery : " + match.Value+ "mV";
                toolStripLabel3.ToolTipText = "Battery : " + match.Value + "mV";

                int value = Convert.ToInt32(match.Value);
                if (value.GetType() == typeof(int))
                {
                    toolStripProgressBar2.Value = value;
                }
                data.battery_level = "";
            }
        }

        private void detect_set_command()
        {
          
            if (data.set_command != "")
            {
                String Message="";
                if (set.get_command(data.set_command) != "YOK")
                {
                    Message = "SET Command : " + set.get_command(data.set_command) + " is sent successfuly.";
                    //ShowMessageBox("SET Command : " + set.get_command(data.set_command) + " is sent successfuly.");
                    //SSDurum.Text="SET Command : " + set.get_command(data.set_command) + " is successfuly.";

                }
                else
                {
                    Message = "SET Command : " + data.set_command + " is sent successfuly.";
                    //ShowMessageBox("SET Command : " + data.set_command + " is sent successfuly.");
                }

                data.set_command = "";
                set.sent_set = "";
                try
                {
                    if (Message != "")
                    {
                      // Uyari alrt = new Uyari(Message);
                      // alrt.Show();
                       ShowMessageBox(Message);
                    }
                }
                catch (Exception e)
                {
                    ShowMessageBox(e.ToString());
                }
            }

       


        }
    public void ShowMessageBox(String mesaj,String title="Result")
    {
         var thread = new Thread(
          () =>
             {
              MessageBox.Show(mesaj,title);
             });
          thread.Start();
    }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            set.device_reset();
            SSDurum.Text = "Sent SET Command.Waiting Request...";

        }


        private void button8_Click(object sender, EventArgs e)
        {
            port_write(set.apn(apn_apn.Text,apn_username.Text,apn_password.Text));
            SSDurum.Text = "Sent SET Command.Waiting Request...";
        }

        private void button9_Click(object sender, EventArgs e)
        {

            port_write(set.server(server_ip.Text,server_port.Text));
            SSDurum.Text = "Sent SET Command.Waiting Request...";
        }

  
        private void button2_Click(object sender, EventArgs e)
        {
          port_write(  set.period_domestic(dsp_d_p1.Text,dsp_d_p2.Text));
          SSDurum.Text = "Sent SET Command.Waiting Request...";
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
           port_write( set.period_roaming(dsp_r_p1.Text,dsp_r_p2.Text));
           SSDurum.Text = "Sent SET Command.Waiting Request...";
        }


  
        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            Connect();

        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            port_write(set.wake_up(wake_up_min.Text));
            SSDurum.Text = "Sent SET Command.Waiting Request...";
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DialogResult file_result = save_file.ShowDialog();
            if (file_result== DialogResult.OK)
            {
                FileStream file = File.Create(save_file.FileName);
                String data = tc_1.Text + Environment.NewLine + tc_2.Text + Environment.NewLine + tc_3.Text + Environment.NewLine + tc_4.Text + Environment.NewLine + tc_5.Text + Environment.NewLine + tc_6.Text + Environment.NewLine;
                Byte[] info = new UTF8Encoding(true).GetBytes(data);
                file.Write(info,0,info.Length);
                file.Close();

            }

        }

        private void button20_Click(object sender, EventArgs e)
        {
            DialogResult file_result = open_file.ShowDialog();
            if (file_result == DialogResult.OK)
            {
                StreamReader file = File.OpenText(open_file.FileName);

                tc_1.Text = file.ReadLine();
                tc_2.Text = file.ReadLine();
                tc_3.Text = file.ReadLine();
                tc_4.Text = file.ReadLine();
                tc_5.Text = file.ReadLine();
                tc_6.Text = file.ReadLine();
                file.Close();

            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            port_write(tc_1.Text);
        
        }

        private void button14_Click(object sender, EventArgs e)
        {
            port_write(tc_2.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            port_write(tc_3.Text);

        }

        private void button16_Click(object sender, EventArgs e)
        {
            port_write(tc_4.Text);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            port_write(tc_5.Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            port_write(tc_6.Text);
        }


        private void save_log(String data)
        {
            if (save_log_check.Checked)
            {

                if (!File.Exists(log_file_adress.Text))
                {
                    save_log_check.CheckState = CheckState.Unchecked;
                    ShowMessageBox("Log file is not exist or please select file!");

                }
                else
                {
                    StreamWriter file_log;
                    file_log = File.AppendText(log_file_adress.Text);
                    log_file_adress.Enabled = false;
                    button3.Enabled = false;
                    file_log.Write(data, 0, data.Length);
                    file_log.Close();
                }
            }
            else
            {
                button3.Enabled = true;
                log_file_adress.Enabled = true;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult file_result = log_file.ShowDialog();
            if (file_result == DialogResult.OK)
            {
                StreamWriter f = File.CreateText(log_file.FileName);
                log_file_adress.Text = log_file.FileName;
                f.Close();
            }
        }

        private void BReset_Click(object sender, EventArgs e)
        {
           port_write( set.device_reset());
        }

 

        private void CBPorts_Click(object sender, EventArgs e)
        {
            port_scan();
        }

        

        private void button10_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
          
                if (activation_lbs.Checked )
                    port_write(set.lbs_activation());
                else
                    port_write(set.lbs_deactivation());

                Thread.Sleep(5000);

                if (activation_deep_sleep.Checked)
                    port_write(set.deep_sleep());
                else
                    port_write(set.soft_sleep());


        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (port.IsOpen)
            {
                    read();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Log.Clear();
        }

        private void aboutTrioMobilConfiguratorToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About frm = new About();
            frm.Show();
            
        }

        private void enterDeviceFirmwarePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Form3 frm = new Form3(PasswordFile);
         
            if (frm.ShowDialog() == DialogResult.OK)
            {
                set.Password = frm.Password;
                if (set.Password=="")
                {
              
                    if (File.Exists(PasswordFile))
                        File.Delete(PasswordFile);
                }
                else
                {    
                    StreamWriter f = File.CreateText(PasswordFile);
                    f.WriteLine(set.Password);
                    f.Close();
                }

            }
        }

        private void catchDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctch = new catch_datas();
            
            ctch.Show();

        }

        private void catch_data(String yazi)
        {
            if(ctch.catch_text.Length >0 )
            if (yazi.IndexOf(ctch.catch_text) > -1)
            {
                ctch.foundtext = yazi;
            }
            
        }

     
      
    }
}
