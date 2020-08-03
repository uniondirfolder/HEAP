using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoList_ex
{
    static class Program
    {
        static MainDS db;
        
        public static MainDS App 
        {
            get 
            {
                if (db == null)
                    db = new MainDS();
                return db;
            }
        }
        static jobDS dbJob;
        public static jobDS App1
        {
            get
            {
                if ( dbJob== null)
                    dbJob = new jobDS();
                return dbJob;
            }
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainF(App, App1));
        }
    }
}
