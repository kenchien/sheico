
namespace MaintainReport {
    partial class MaintainReport {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.txtEndDate = new System.Windows.Forms.TextBox();
            this.labDate = new System.Windows.Forms.Label();
            this.btnCreateExcel = new System.Windows.Forms.Button();
            this.txtStartDate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConn = new System.Windows.Forms.TextBox();
            this.labConn = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.labFilename = new System.Windows.Forms.Label();
            this.labMsg = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.labOutput = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtEndDate
            // 
            this.txtEndDate.Font = new System.Drawing.Font("PMingLiU", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtEndDate.Location = new System.Drawing.Point(630, 194);
            this.txtEndDate.Name = "txtEndDate";
            this.txtEndDate.Size = new System.Drawing.Size(358, 35);
            this.txtEndDate.TabIndex = 1;
            // 
            // labDate
            // 
            this.labDate.AutoSize = true;
            this.labDate.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labDate.Location = new System.Drawing.Point(37, 197);
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(150, 42);
            this.labDate.TabIndex = 2;
            this.labDate.Text = "日期區間";
            // 
            // btnCreateExcel
            // 
            this.btnCreateExcel.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCreateExcel.Location = new System.Drawing.Point(44, 345);
            this.btnCreateExcel.Name = "btnCreateExcel";
            this.btnCreateExcel.Size = new System.Drawing.Size(230, 104);
            this.btnCreateExcel.TabIndex = 3;
            this.btnCreateExcel.Text = "產生Excel";
            this.btnCreateExcel.UseVisualStyleBackColor = true;
            this.btnCreateExcel.Click += new System.EventHandler(this.btnCreateExcel_Click);
            // 
            // txtStartDate
            // 
            this.txtStartDate.Font = new System.Drawing.Font("PMingLiU", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStartDate.Location = new System.Drawing.Point(198, 194);
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.Size = new System.Drawing.Size(378, 35);
            this.txtStartDate.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(582, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 42);
            this.label2.TabIndex = 5;
            this.label2.Text = "~";
            // 
            // txtConn
            // 
            this.txtConn.Font = new System.Drawing.Font("PMingLiU", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtConn.Location = new System.Drawing.Point(198, 51);
            this.txtConn.Name = "txtConn";
            this.txtConn.Size = new System.Drawing.Size(1059, 35);
            this.txtConn.TabIndex = 7;
            // 
            // labConn
            // 
            this.labConn.AutoSize = true;
            this.labConn.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labConn.Location = new System.Drawing.Point(37, 48);
            this.labConn.Name = "labConn";
            this.labConn.Size = new System.Drawing.Size(150, 42);
            this.labConn.TabIndex = 6;
            this.labConn.Text = "連線字串";
            // 
            // txtFilename
            // 
            this.txtFilename.Font = new System.Drawing.Font("PMingLiU", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtFilename.Location = new System.Drawing.Point(198, 119);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(1059, 35);
            this.txtFilename.TabIndex = 9;
            // 
            // labFilename
            // 
            this.labFilename.AutoSize = true;
            this.labFilename.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labFilename.Location = new System.Drawing.Point(37, 116);
            this.labFilename.Name = "labFilename";
            this.labFilename.Size = new System.Drawing.Size(117, 42);
            this.labFilename.TabIndex = 8;
            this.labFilename.Text = "範本檔";
            // 
            // labMsg
            // 
            this.labMsg.AutoSize = true;
            this.labMsg.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labMsg.Location = new System.Drawing.Point(321, 345);
            this.labMsg.Name = "labMsg";
            this.labMsg.Size = new System.Drawing.Size(52, 25);
            this.labMsg.TabIndex = 10;
            this.labMsg.Text = "訊息";
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("PMingLiU", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOutput.Location = new System.Drawing.Point(198, 272);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(1059, 35);
            this.txtOutput.TabIndex = 12;
            // 
            // labOutput
            // 
            this.labOutput.AutoSize = true;
            this.labOutput.Font = new System.Drawing.Font("Microsoft JhengHei", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labOutput.Location = new System.Drawing.Point(37, 269);
            this.labOutput.Name = "labOutput";
            this.labOutput.Size = new System.Drawing.Size(150, 42);
            this.labOutput.TabIndex = 11;
            this.labOutput.Text = "產出路徑";
            // 
            // MaintainReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 508);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.labOutput);
            this.Controls.Add(this.labMsg);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.labFilename);
            this.Controls.Add(this.txtConn);
            this.Controls.Add(this.labConn);
            this.Controls.Add(this.txtStartDate);
            this.Controls.Add(this.btnCreateExcel);
            this.Controls.Add(this.labDate);
            this.Controls.Add(this.txtEndDate);
            this.Controls.Add(this.label2);
            this.Name = "MaintainReport";
            this.Text = "疾管署_產生系統指標";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEndDate;
        private System.Windows.Forms.Label labDate;
        private System.Windows.Forms.Button btnCreateExcel;
        private System.Windows.Forms.TextBox txtStartDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtConn;
        private System.Windows.Forms.Label labConn;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label labFilename;
        private System.Windows.Forms.Label labMsg;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label labOutput;
    }
}

