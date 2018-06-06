using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace TrioCom
{
    class SET
    {
       private string set="#SET;";
       public string sent_set = "";
       public string Password = "";
     

       Dictionary<string, string> c = new Dictionary<string, string>(){

            {"01","Server Setttings"},
            {"04","Timing (Live Mode) Setttings"},
            {"33","Timings (Interval Mode / Power Save Mode) Setttings"},
            {"12","Device Reset"},
            {"25","APN Settings"},
            {"42","LBS"},
            {"32","Deep Sleep"}
        };

       public string get_command(string d)
       {
           if (c.ContainsKey(d))
               return c[d];
           else
               return "YOK";
       }

       private String command_combine(String command,String parameters)
       {
        String code=this.set;
        this.sent_set = command;
        code+=command;

        if (this.Password!="")
            code += ":"+this.Password ;

        code += ";"+parameters;

        return code + ";0!";

       }



       public string server(string ip, string port) { return command_combine("01",ip+";"+port);  }
       public string period_domestic(string pr1, string pr2) { return command_combine("04", "0;"+pr1 + ";" + pr2); }
       public string period_roaming(string pr1, string pr2) { return command_combine("04", "1;" + pr1 + ";" + pr2); }
       public string device_reset() { return command_combine("12", "0"); }
       public string apn(string apn, string username, string password) { return command_combine("25", apn + ";" + username + ";" + password); }
       public string lbs_activation() { return command_combine("42", "1"); }
       public string lbs_deactivation() { return command_combine("42", "0"); }
       public string wake_up(string pr1) { return command_combine("33", pr1+";0"); }
       public string deep_sleep() { return command_combine("32", "0;0"); }
       public string soft_sleep() { return command_combine("34", "1;1"); }




    }
}
