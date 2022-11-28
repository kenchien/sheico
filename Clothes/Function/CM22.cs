using Clothes.Extension;
using Clothes.Service;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clothes.Function {
   public partial class CM22 : Form {
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      public CM22Service _service;
      public DataTable dt15;
      public DataTable dt14;
      public DataTable dt151;

      public CM22(CM22Service service) {
         InitializeComponent();
         this.WindowState = FormWindowState.Maximized;
         _service = service;
         log.Info("CM22 init...");

         //hide tab header
         tabControl1.Appearance = TabAppearance.FlatButtons;
         tabControl1.ItemSize = new Size(0, 1);
         tabControl1.SizeMode = TabSizeMode.Fixed;
         tabControl1.TabIndex = 0;

         dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;
         dataGridView2.AutoGenerateColumns = false;
         dataGridView2.AllowUserToOrderColumns = false;

      }

      private void CM22_Load(object sender, EventArgs e) {
         try {

            //setup dropdownlist value
            try {
               var lookupT1502 = _service.GetColorList<ComboBoxItem>("T1502");
               comboBoxT1502.DataSource = lookupT1502;
               comboBoxT1502.Refresh();


               var lookupBig = _service.GetAllBigList<ComboBoxItem>();
               //DataGridViewComboBoxColumn dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn();
               //dataGridViewComboBoxColumn.DataSource = lookupBig;
               Column2.DataSource = lookupBig;
               Column2.DisplayMember = "Text";
               Column2.ValueMember = "Value";
            } catch (Exception ex) {
               MessageBox.Show("資料庫連線發生異常");
               return;
            }






            //setup default value
            textBoxT1501.Text = "FFE00279  ";
            ComboUtil.SetItemValue(comboBoxT1502, "BKAQ");

            textBoxT1512.Text = "";
            textBoxT1513.Text = "";
            textBoxT1514.Text = "";

            //page2
            textBoxT1408.Text = "作法重要變更(T1408)";
            textBoxT1508.Text = "成品作法備註(T1508)";
            textBoxT1530.Text = "部件作法備註(T1530)";
            textBoxT1531.Text = "部件作法提示(T1531)";


            //page3
            //            <輔料清單>  
            //部件作法編碼： MBD0003   
            //            textBoxT1508.Text = "成品作法備註(T1508)";
            //            textBoxT1530.Text = "部件作法備註(T1530)";
            //            textBoxT1531.Text = "部件作法提示(T1531)";

            log.Info("CM22_Load ok");

            buttonSearch_Click(this, null);
         } catch (Exception ex) {
            log.Error(ex);
         }
      }

      private void buttonSearch_Click(object sender, EventArgs e) {
         try {

            if (string.IsNullOrEmpty(textBoxT1501.Text)) {
               MessageBox.Show("T1501必填");
               return;
            }
            if (string.IsNullOrEmpty(comboBoxT1502.Text)) {
               MessageBox.Show("T1502必填");
               return;
            }



            #region //T15 test data (p key all)
            //FFE00279  	BKAQ	001 0010	MBD00013	001
            //FFE00279  	BKAQ	002 0010	MBD00013	001
            //FFE00280  	BKAQ	001 0010	MBD00013	001
            //FFE00281  	BKAQ	001 0010	MBD00013	001
            //var dt15 = _service.GetT15("FFE00279  ", "BKAQ");
            #endregion


            //讀取資料源,dataTable或是dataSet或是class, dataGridView都可以吃
            dt15 = _service.GetT15(textBoxT1501.Text,
                                       ComboUtil.GetItem(comboBoxT1502).Value,
                                       textBoxT1512.Text,
                                       textBoxT1513.Text,
                                       textBoxT1514.Text);
            //var dt15 = _service.GetT15test("FFE00279  ", "BKAQ", "001", "0010", "MBD00013", "001");


            for (int pos = 0; pos < dt15.Columns.Count; pos++) {
               dataGridView2.Columns[pos + 1].DataPropertyName = dt15.Columns[pos].ColumnName;
            }



            //如果要在第一列顯示新增列,測試幾種方法,
            //1.直接新增row,但是會發生 "當控制項已繫結資料時，無法以程式設計的方式將資料列加入 DataGridView 的資料列集合。"
            //System.Data.DataRow dataRow = new System.Data.DataRow();
            //dt15.Rows.InsertAt(dataRow, 0);






            //3.對dt操作
            dt15.Constraints.Clear();
            foreach (DataColumn dcA in dt15.Columns)
               dcA.AllowDBNull = true;
            dt15.Rows.InsertAt(dt15.NewRow(), 0);

            //2.可以用一層bindingsource作DataGridView和DataTable的中間層 
            //BindingSource bds = new BindingSource();
            //bds.DataSource = dt15;
            ////將AllowNew設為true, 允許可新增資料 
            //bds.AllowNew = true;
            ////將AllowEdit設為true, 允許可編輯資料 
            ////bds.AllowEdit = true;
            //dataGridView2.DataSource = bds;//dt15
            dataGridView2.DataSource = dt15;


            //setup grid button
            //DataGridViewButtonColumn btnColumnDel = new DataGridViewButtonColumn();
            //btnColumnDel.Name = "btnAdd";
            //btnColumnDel.UseColumnTextForButtonValue = true;
            //btnColumnDel.HeaderText = "狀態";
            //btnColumnDel.Text = "刪除";
            //dataGridView2.Columns.Insert(0, btnColumnDel);

            //DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
            //btnColumn.Name = "btnAdd";
            //btnColumn.UseColumnTextForButtonValue = true;
            //btnColumn.HeaderText = "狀態";
            //btnColumn.Text = "修改";//新增
            ////dataGridView2.Columns.Add(btnColumn);
            //dataGridView2.Columns.Insert(0, btnColumn);

            //this.dataGridView2.CurrentCell = this.dataGridView2[0, 0];
            //this.dataGridView2.CurrentCell.Value = "ken";
            //this.dataGridView2.CurrentCell = this.dataGridView2[0, 1];
            //this.dataGridView2.CurrentCell.Value = "";

            dataGridView2.AutoResizeColumns();
            dataGridView2.Refresh();
            dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;

            //dataGridView2.Rows[0].Cells[0].Value = "新增";
            //dataGridView2.Rows[0].Cells[11
            tabControl1.SelectedIndex = 1;
            log.Info("CM22_Load success");



         } catch (Exception ex) {
            log.Error(ex);
         }
      }





      private void buttonUploadFile_Click(object sender, EventArgs e) {
         try {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "*.pdf|*.pdf|*.jpg|*.jpg|*.gif|*.gif|*.png|*.png";
            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;

            textBoxUploadFile.Text = openFileDialog.FileName;
            var fileExt = textBoxUploadFile.Text.Substring(textBoxUploadFile.Text.IndexOf(".") + 1);

            //系統自動以"部件作法編碼"+"部件作法版次"為檔名另存於 IIS Server 本機root 之下\Wetsuit\03_Draft"資料夾
            var fileName = $@"部件作法編碼_部件作法版次." + fileExt;
            //將檔案複製到網路芳鄰的Y槽(注意權限)
            System.IO.File.Copy(textBoxUploadFile.Text, $@"C:\ken\" + fileName, true);

            log.Info("CM22 buttonUploadFile success,filename=" + textBoxUploadFile.Text);
         } catch (Exception ex) {
            log.Error(ex);
            MessageBox.Show("上傳檔案失敗,message=" + ex.Message);
         }
      }

      private void buttonSave_Click(object sender, EventArgs e) {
         try {
            //_service.SaveT15();


            MessageBox.Show("存檔完成");
            log.Info("CM22-2 Save OK");
         } catch (Exception ex) {
            log.Error(ex);
            MessageBox.Show("存檔發生錯誤,message=" + ex.Message);
         }
      }
      private void buttonBack_Click(object sender, EventArgs e) {
         tabControl1.SelectedIndex = 0;
      }



      public void kentest() {

         //T14 test data (p key 1,3,5)
         //MBD00013	MBD00013	002	MB	MBD
         //MBD00013	MBD00013	001	MB	MBD
         //MBD00016	MBD00013	016	MB	MBD
         //MBD00017	MBD00013	017	MB	MBD
         //var dt14 = _service.GetT14("MBD00013", "002", "MBD");

      }

      private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) {
         try {
            Console.WriteLine($@"row={e.RowIndex},col={e.ColumnIndex}");

            if (e.RowIndex == 0 && e.ColumnIndex == 0) {
               //新增按鈕,檢查有值就新增
               var grd = (sender as DataGridView);
               //檢查,全部沒有值或p key沒完整 就不要新增
               var haveValue = false;
               for (int pos = 0; pos < grd.Rows[0].Cells.Count; pos++) {
                  if (grd.Rows[0].Cells[pos].Value.ToString() != string.Empty) {
                     haveValue = true;
                     break;
                  }
               }
               if(!haveValue){
                  MessageBox.Show("p key不完整,取消新增動作");
                  return;
               }


               DataRow dataRow = dt15.NewRow();
               for (int pos = 0; pos < dataRow.ItemArray.Length; pos++)
                  dataRow[pos] = grd.Rows[0].Cells[pos + 1].Value;




               dt15.Rows.Add(dataRow);
               dataGridView2.Refresh();
               return;
            }
            if (e.RowIndex > 0 && e.ColumnIndex == 0) {
               //刪除
               if (MessageBox.Show("警告", "請確認是否要刪除?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                  return;
               dt15.Rows.RemoveAt(e.RowIndex);
               dataGridView2.Refresh();
               return;
            }

            switch (e.ColumnIndex) {
            case 1:
               break;
            case 3:
               break;
            case 5:
               break;
            case 8:
               break;
            case 10://show pic
               System.Diagnostics.Process.Start(@"C:\ken\部件作法編碼_部件作法版次.pdf");
               break;
            case 11://jump tab3
                    //準備好grid3



               tabControl1.SelectedIndex = 2;
               break;
            }

         } catch (Exception ex) {
            log.Error(ex);
            MessageBox.Show("dataGridView2_CellContentClick發生錯誤,message=" + ex.Message);
         }

      }



      private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
         if (e.RowIndex == 0 && e.ColumnIndex == 0) {
            e.Value = "新增";
         } else if (e.RowIndex == 0) {
            //把一些欄位便灰色
            switch (e.ColumnIndex) {

            case 3:
            case 5:
            case 8:
            case 10://show pic
            case 11://jump tab3
               e.CellStyle.BackColor = Color.Gray;
               break;
            }

         }
      }

      private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
         //if (e.RowIndex == 0 && e.ColumnIndex == 1) {
         //   e.PaintContent(new Rectangle(0, 0, 40, 40));
         //}
      }

      private void dataGridView2_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
         if (dataGridView2.Rows[e.RowIndex].Cells[10].Value.ToString() == "Y") {
            dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
         }
      }
   }//public partial class CM22 : Form {
}
