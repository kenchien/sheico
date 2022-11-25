using Clothes.Dal.DBComponment;
using Clothes.Dal.Models;
using Clothes.Dal.Repository;
using Clothes.Function;
using Clothes.Service;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Clothes {
   static class Program {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main() {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);


         string connectionString = ConfigurationManager.ConnectionStrings["Clothes.Properties.Settings.sheicoConnectionString"].ToString();
         IDapper dapper = new DapperBase(connectionString);
         IRepository<T14> t14Repository = new Repository<T14>(dapper);


         CM22Service service = new CM22Service(t14Repository);
         Application.Run(new CM22(service));
      }
   }
}
