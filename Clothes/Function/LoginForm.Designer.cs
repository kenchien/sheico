
namespace Clothes.Function {
   partial class LoginForm {
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
         this.label1 = new System.Windows.Forms.Label();
         this.buttonBack = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Microsoft JhengHei", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.label1.Location = new System.Drawing.Point(262, 161);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(227, 81);
         this.label1.TabIndex = 0;
         this.label1.Text = "登入頁";
         // 
         // buttonBack
         // 
         this.buttonBack.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
         this.buttonBack.Location = new System.Drawing.Point(276, 306);
         this.buttonBack.Name = "buttonBack";
         this.buttonBack.Size = new System.Drawing.Size(109, 41);
         this.buttonBack.TabIndex = 16;
         this.buttonBack.Text = "CM22";
         this.buttonBack.UseVisualStyleBackColor = true;
         this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
         // 
         // Login
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.buttonBack);
         this.Controls.Add(this.label1);
         this.Name = "Login";
         this.Text = "Login";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button buttonBack;
   }
}