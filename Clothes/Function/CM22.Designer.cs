
namespace Clothes.Function {
   partial class CM22 {
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
         System.Windows.Forms.Label t1402Label;
         this.dataGridView1 = new System.Windows.Forms.DataGridView();
         this.t1402TextBox = new System.Windows.Forms.TextBox();
         t1402Label = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         this.SuspendLayout();
         // 
         // t1402Label
         // 
         t1402Label.AutoSize = true;
         t1402Label.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         t1402Label.Location = new System.Drawing.Point(44, 95);
         t1402Label.Name = "t1402Label";
         t1402Label.Size = new System.Drawing.Size(58, 20);
         t1402Label.TabIndex = 3;
         t1402Label.Text = "T1402:";
         // 
         // dataGridView1
         // 
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridView1.Location = new System.Drawing.Point(45, 320);
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.RowTemplate.Height = 24;
         this.dataGridView1.Size = new System.Drawing.Size(803, 397);
         this.dataGridView1.TabIndex = 0;
         // 
         // t1402TextBox
         // 
         this.t1402TextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.t1402TextBox.Location = new System.Drawing.Point(109, 94);
         this.t1402TextBox.Name = "t1402TextBox";
         this.t1402TextBox.Size = new System.Drawing.Size(217, 29);
         this.t1402TextBox.TabIndex = 4;
         // 
         // CM22
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1350, 729);
         this.Controls.Add(t1402Label);
         this.Controls.Add(this.t1402TextBox);
         this.Controls.Add(this.dataGridView1);
         this.Name = "CM22";
         this.Text = "CM22";
         this.Load += new System.EventHandler(this.CM22_Load);
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.DataGridView dataGridView1;
      private System.Windows.Forms.TextBox t1402TextBox;
   }
}