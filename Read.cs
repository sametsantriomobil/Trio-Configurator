using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrioCom
{
    public class Read
    {
        public String imei="", 
                      imsi="", 
                      ip="", 
                      port="", 
                      gsm_signal="", 
                      gps_counter="",
                      battery_level="",
                      set_command="";
        private String Data;

        public void get(String yazi)
        {
           this.Data = yazi;
           this.imei=this.read("IMEI:", 16,this.imei);
           this.gsm_signal = this.read("+CSQ:",4, this.gsm_signal);
           this.gsm_signal = this.read("SignalPower:",2,this.gsm_signal);
           this.battery_level = this.read("BatteryVoltage:", 4, this.battery_level);
           this.set_command = this.read("Param1:", 2, this.set_command);
        }

        public String read(String Value, Int32 uzu,String otd)
        {

            string de =otd;
            int n = Data.IndexOf(Value);
            if (n > -1 && (n + Value.Length+uzu)< Data.Length)
            {
                
                de = Data.Substring(n + Value.Length, uzu);
                de = de.Replace("<CR>", "");
                de = de.Replace("<LF>", "");
                de = de.Trim();

              
            }
           
            return de;
        }

    }
}
