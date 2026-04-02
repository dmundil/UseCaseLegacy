using AnyTeller.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AnyTeller.Forms
{
    public partial class DashboardForm : Form
    {
        private DataGridView dgvUsers;
        private Button btnStartSession;
        private Label lblTitle;

        public DashboardForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.btnStartSession = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsers
            // 
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Location = new System.Drawing.Point(12, 50);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.Size = new System.Drawing.Size(960, 400);
            this.dgvUsers.TabIndex = 0;
            // 
            // btnStartSession
            // 
            this.btnStartSession.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartSession.Location = new System.Drawing.Point(12, 460);
            this.btnStartSession.Name = "btnStartSession";
            this.btnStartSession.Size = new System.Drawing.Size(150, 40);
            this.btnStartSession.TabIndex = 1;
            this.btnStartSession.Text = "Start Session";
            this.btnStartSession.UseVisualStyleBackColor = true;
            this.btnStartSession.Click += new System.EventHandler(this.btnStartSession_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 30);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "AnyTeller - Regional User Management v4.2";
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 511);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnStartSession);
            this.Controls.Add(this.dgvUsers);
            this.Name = "DashboardForm";
            this.Text = "AnyTeller - Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData()
        {
            var data = UserRecord.GetMockData();
            dgvUsers.DataSource = data;
        }

        private void btnStartSession_Click(object sender, EventArgs e)
        {
            TransactionForm transactionForm = new TransactionForm(UserRecord.GetMockData());
            this.Hide();
            transactionForm.ShowDialog();
            this.Show();
        }
    }
}
