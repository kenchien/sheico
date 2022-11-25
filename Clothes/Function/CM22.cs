using Clothes.Service;
using System;
using System.Windows.Forms;

namespace Clothes.Function {
   public partial class CM22 : Form {
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      public CM22Service _service;

      public CM22(CM22Service service) {
         InitializeComponent();
         _service = service;
         log.Info("CM22 init...");
      }

      private void CM22_Load(object sender, EventArgs e) {
         try {

            //T15 test data (p key all)
            //FFE00279  	BKAQ	001 0010	MBD00013	001
            //FFE00279  	BKAQ	002 0010	MBD00013	001
            //FFE00280  	BKAQ	001 0010	MBD00013	001
            //FFE00281  	BKAQ	001 0010	MBD00013	001   
            var dt15 = _service.GetT15("FFE00279  ", "BKAQ", "001", "0010", "MBD00013", "001");
            var t15 = _service.GetT15test("FFE00279  ", "BKAQ", "001", "0010", "MBD00013", "001");



            //T14 test data (p key 1,3,5)
            //MBD00013	MBD00013	002	MB	MBD
            //MBD00013	MBD00013	001	MB	MBD
            //MBD00016	MBD00013	016	MB	MBD
            //MBD00017	MBD00013	017	MB	MBD
            var dt14 = _service.GetT14("MBD00013", "002", "MBD");



            //兩種data source都可以
            //dataGridView1.DataSource = dt15;
            dataGridView1.DataSource = t15;

            dataGridView1.Refresh();
            log.Info("CM22_Load success");

         } catch (Exception ex) {
            log.Error(ex);
         }
      }





   }
}
