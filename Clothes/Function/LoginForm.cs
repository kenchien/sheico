using Clothes.Dal.DBComponment;
using Clothes.Dal.Models;
using Clothes.Dal.Repository;
using Clothes.Service;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Clothes.Function {
   public partial class LoginForm : Form {
      public LoginForm() {
         InitializeComponent();
         this.WindowState = FormWindowState.Maximized;
      }

      private void buttonBack_Click(object sender, EventArgs e) {
         string connectionString = ConfigurationManager.ConnectionStrings["Clothes.Properties.Settings.sheicoConnectionString"].ToString();
         IDapper dapper = new DapperBase(connectionString);
         IRepository<T14> t14Repository = new Repository<T14>(dapper);
         CM22Service service = new CM22Service(t14Repository);
         CM22 cm22 = new CM22(service);
         cm22.MdiParent = this.MdiParent;
         cm22.Show();
         //this.Close();
      }
   }
}
