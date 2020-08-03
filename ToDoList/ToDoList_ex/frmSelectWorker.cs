using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoList_ex
{
    public partial class frmSelectWorker : Form
    {
        TextBox txtBoxWorkerJob;
        public frmSelectWorker(TextBox txtBoxWorkerJob, MainDS db)
        {
            InitializeComponent();
            this.txtBoxWorkerJob = txtBoxWorkerJob;
            bindingSource1.DataSource = db.Persone;
        }


        private void dgvSelectWorker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtBoxWorkerJob.Text = "";
                var o = dgvSelectWorker.SelectedCells;
                for (int i = 0; i < o.Count; i++)
                {
                    txtBoxWorkerJob.Text += o[i].Value.ToString() + " ";
                }
                Close();
            }
        }
    }
}
