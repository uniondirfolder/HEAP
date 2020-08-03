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

    public partial class MainF : Form
    {
        MainDS db;
        jobDS JOB;

        public MainF(MainDS App, jobDS JOB)
        {
            db = App;
            this.JOB = JOB;
            InitializeComponent();
        }
        private void MainF_Load(object sender, EventArgs e)
        {
            if (File.Exists(string.Format("{0}//Person.dat", Application.StartupPath)))
            { db.Persone.ReadXml(string.Format("{0}//Person.dat", Application.StartupPath));}
            personeBindingSource.DataSource = db.Persone;

            if (File.Exists(string.Format("{0}//Job.dat", Application.StartupPath)))
            { JOB.ReadXml(string.Format("{0}//Job.dat", Application.StartupPath)); }
            jobBindingSource.DataSource = JOB.Job;
            if (!Directory.Exists(Application.StartupPath + "\\Jobs")) { Directory.CreateDirectory(Application.StartupPath + "\\Jobs"); }

            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnCancelJob.Enabled = false;
            btnSaveJob.Enabled = false;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try { 
            personeBindingSource.EndEdit();
            db.Persone.AcceptChanges();
            db.Persone.WriteXml(string.Format("{0}//Person.dat", Application.StartupPath));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                db.Persone.RejectChanges();
            }
            
            gbPersona.Enabled = false;
            gbPlace.Enabled = false;
            btnSave.BackColor = btnNew.BackColor;
            btnCancel.BackColor = btnNew.BackColor;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnEdit.Enabled = true;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnSave.BackColor = btnNew.BackColor;
            btnCancel.BackColor = btnNew.BackColor;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnEdit.Enabled = true;

            db.Persone.RejectChanges();
            personeBindingSource.ResetBindings(false);

            txbName.Focus();
            gbPersona.Enabled = false;
            gbPlace.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            btnSave.BackColor = Color.GreenYellow;
            btnCancel.BackColor = Color.Red;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnNew.Enabled = false;
            btnEdit.Enabled = false;

            try
            {
                gbPersona.Enabled = true;
                gbPlace.Enabled = true;
                db.Persone.AddPersoneRow(db.Persone.NewPersoneRow());
                personeBindingSource.MoveLast();
                lblDateCreated.Text = DateTime.Now.ToString();
                txbName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                db.Persone.RejectChanges();
            }
           
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnSave.BackColor = Color.GreenYellow;
            btnCancel.BackColor = Color.Red;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnNew.Enabled = false;
            btnEdit.Enabled = false;

            gbPersona.Enabled = true;
            gbPlace.Enabled = true;
            txbName.Focus();
        }

        private void dgvPersone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Ви впевнені, що хочете видалити запис?", "Повідомлення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    personeBindingSource.RemoveCurrent();
            }
        }

        private void txtBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!string.IsNullOrEmpty(txbSearch.Text))
            {
                var query = from o in db.Persone
                            where o.Name.Contains(txbSearch.Text) || o.LName.Contains(txbSearch.Text) || o.FName.Contains(txbSearch.Text) ||
                            o.PhMobile == txbSearch.Text || o.PhPrivate == txbSearch.Text || o.PhWork == txbSearch.Text || o.EMail == txbSearch.Text
                            select o;
                dgvPersone.DataSource = query.ToList();
            }
            else
                dgvPersone.DataSource = personeBindingSource;
        }




        //-----------------------------------------------------------------------
        private void txbNameJob_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txbNameJob.Text)) 
            {
                txbFileNameJob.Text = "\\Jobs\\"+ (JOB.Job.Rows.Count + 1).ToString() +"\\"+ txbNameJob.Text+ ".rtf";
            }
        }

        private void btnGetWorkerSelect_Click(object sender, EventArgs e)
        {
            frmSelectWorker frm = new frmSelectWorker(this.txtBoxWorkerJob, this.db);
            frm.ShowDialog();
        }
        
        private void btnNewJob_Click(object sender, EventArgs e)
        {
            btnSaveJob.BackColor = Color.GreenYellow;
            btnCancelJob.BackColor = Color.Red;
            btnCancelJob.Enabled = true;
            btnSaveJob.Enabled = true;
            btnNewJob.Enabled = false;
            btnEditJob.Enabled = false;

            try
            {
                gbJobs.Enabled = true;
                JOB.Job.AddJobRow(JOB.Job.NewJobRow());
                jobBindingSource.MoveLast();
                txbNameJob.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JOB.Job.RejectChanges();
            }
        }

        private void btnEditJob_Click(object sender, EventArgs e)
        {
            btnSaveJob.BackColor = Color.GreenYellow;
            btnCancelJob.BackColor = Color.Red;
            btnCancelJob.Enabled = true;
            btnSaveJob.Enabled = true;
            btnNewJob.Enabled = false;
            btnEditJob.Enabled = false;

            gbJobs.Enabled = true;
            txbNameJob.Focus();
            try
            {
                rtbJob.LoadFile(Application.StartupPath + txbFileNameJob.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnCancelJob_Click(object sender, EventArgs e)
        {
            btnSaveJob.BackColor = btnNewJob.BackColor;
            btnCancelJob.BackColor = btnNewJob.BackColor;
            btnCancelJob.Enabled = false;
            btnSaveJob.Enabled = false;
            btnNewJob.Enabled = true;
            btnEditJob.Enabled = true;

            JOB.Job.RejectChanges();
            jobBindingSource.ResetBindings(false);
            
            txbNameJob.Focus();
            gbJobs.Enabled = false;
        }

        private void btnSaveJob_Click(object sender, EventArgs e)
        {
            string t = Application.StartupPath + txbFileNameJob.Text;
            if (!Directory.Exists(Path.GetDirectoryName(t))) { Directory.CreateDirectory(Path.GetDirectoryName(t)); }
            try
            {
                jobBindingSource.EndEdit();
                JOB.Job.AcceptChanges();
                JOB.Job.WriteXml(string.Format("{0}//Job.dat", Application.StartupPath));
                rtbJob.SaveFile(Application.StartupPath + txbFileNameJob.Text);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JOB.Job.RejectChanges();
            }

            gbJobs.Enabled = false;

            btnSaveJob.BackColor = btnNewJob.BackColor;
            btnCancelJob.BackColor = btnNewJob.BackColor;
            btnCancelJob.Enabled = false;
            btnSaveJob.Enabled = false;
            btnNewJob.Enabled = true;
            btnEditJob.Enabled = true;
        }
        private void dgvJobs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Ви впевнені, що хочете видалити запис?", "Повідомлення", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    jobBindingSource.RemoveCurrent();
            }
        }

        private void btnTimeToDoJurnalSet_Click(object sender, EventArgs e)
        {
            if (dtpEndJurnalToDo.Value < dtpStartJurnalToDo.Value) 
            {
                dtpStartJurnalToDo.Value = dtpEndJurnalToDo.Value;
            }

                var query = from o in JOB.Job
                            where o.DateEnd>= dtpStartJurnalToDo.Value && o.DateEnd <= dtpEndJurnalToDo.Value
                            select o;
                dgvPersone.DataSource = query.ToList();
                //dgvPersone.DataSource = personeBindingSource;

        }

        private void btnSearchJurnalToDo_Click(object sender, EventArgs e)
        {
            frmFiltring frm = new frmFiltring(this.JOB);
            frm.ShowDialog();
        }

        private void btnFontToDo_Click(object sender, EventArgs e)
        {
            if (fontD.ShowDialog() == DialogResult.OK) 
            {
                rtbJob.Font = fontD.Font;
            }
        }

        private void btnBackGroColorToDo_Click(object sender, EventArgs e)
        {
            if (colorD.ShowDialog() == DialogResult.OK) 
            {
                rtbJob.BackColor = colorD.Color;
            }
        }

        private void btnColorToDo_Click(object sender, EventArgs e)
        {
            if (colorD.ShowDialog() == DialogResult.OK)
            {
                rtbJob.ForeColor = colorD.Color;
            }
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
