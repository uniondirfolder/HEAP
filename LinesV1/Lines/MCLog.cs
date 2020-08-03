using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lines
{
    public class MCLog
    {
        string path;
        public void WriteToLog(string msg) 
        {

            try
            {
                File.AppendAllText(path, msg);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        public void ClearLog() 
        {
            try
            {
                File.WriteAllText(path,"");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        public MCLog() 
        {
            path = Directory.GetCurrentDirectory() + "\\log.txt";
            if (!File.Exists(path)) { var t = File.CreateText(path); t.Close(); }
        }
    }
}
