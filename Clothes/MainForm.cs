using Clothes.Dal.DBComponment;
using Clothes.Dal.Models;
using Clothes.Dal.Repository;
using Clothes.Function;
using Clothes.Service;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Clothes {
   public partial class MainForm : Form {
      private int childFormNumber = 0;

      public MainForm() {
         InitializeComponent();

         //LoginForm loginForm = new LoginForm();
         //loginForm.MdiParent = this;
         //loginForm.Show();

         string connectionString = ConfigurationManager.ConnectionStrings["Clothes.Properties.Settings.sheicoConnectionString"].ToString();
         IDapper dapper = new DapperBase(connectionString);
         IRepository<T14> t14Repository = new Repository<T14>(dapper);
         CM22Service service = new CM22Service(t14Repository);
         CM22 cm22 = new CM22(service);
         cm22.MdiParent = this;
         cm22.Show();
      }

      private void ShowNewForm(object sender, EventArgs e) {
         Form childForm = new Form();
         childForm.MdiParent = this;
         childForm.Text = "Window " + childFormNumber++;
         childForm.Show();
      }

      private void OpenFile(object sender, EventArgs e) {
         OpenFileDialog openFileDialog = new OpenFileDialog();
         openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
         if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
            string FileName = openFileDialog.FileName;
         }
      }

      private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
         SaveFileDialog saveFileDialog = new SaveFileDialog();
         saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
         if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
            string FileName = saveFileDialog.FileName;
         }
      }

      private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) {
         this.Close();
      }

      private void CutToolStripMenuItem_Click(object sender, EventArgs e) {
      }

      private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
      }

      private void PasteToolStripMenuItem_Click(object sender, EventArgs e) {
      }

      private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
         toolStrip.Visible = toolBarToolStripMenuItem.Checked;
      }

      private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
         statusStrip.Visible = statusBarToolStripMenuItem.Checked;
      }

      private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) {
         LayoutMdi(MdiLayout.Cascade);
      }

      private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e) {
         LayoutMdi(MdiLayout.TileVertical);
      }

      private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e) {
         LayoutMdi(MdiLayout.TileHorizontal);
      }

      private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
         LayoutMdi(MdiLayout.ArrangeIcons);
      }

      private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
         foreach (Form childForm in MdiChildren) {
            childForm.Close();
         }
      }
   }
}
