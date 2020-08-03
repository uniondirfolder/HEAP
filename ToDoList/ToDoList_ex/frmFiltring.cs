using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoList_ex
{
    public partial class frmFiltring : Form
    {
        jobDS JOB;
        public frmFiltring(jobDS JOB)
        {
            InitializeComponent();
            this.JOB = JOB;
        }

        private void btnMake_Click(object sender, EventArgs e)
        {

            var query = from o in JOB.Job
                        where o.Name== txbNameProj.Text || o.Tag.Contains(txbTag.Text) || o.Worker.Contains(txtBoxWorkerJob.Text)
                        || cmbTime.Items[cmbTime.SelectedIndex].ToString()==o.Priority || checkBox1.Checked==o.State || textBox4.Text==o.Info || txbNameProj.Text==o.ProjName
                        select o;
            dataGridView1.DataSource = query.ToList();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(bm, new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(bm, 10, 10);
        }
    }
}
