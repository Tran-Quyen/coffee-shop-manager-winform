using Quan_Ly_Quan_Cafe.DAO;
using Quan_Ly_Quan_Cafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quan_Ly_Quan_Cafe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount 
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
        }
        #region Method
        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + loginAccount.DisplayName + ")";
        }
        void LoadCategory()
        {
            List<CategoryDTO> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<FoodDTO> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flTable.Controls.Clear();   

            List<Table> tableList = TableDAO.Instance.LoadTableListInTableManager();
            foreach (Table item in tableList)
            {
                Button btn = new Button() {Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };

                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;
                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.SaddleBrown;
                        break;
                }
                                
                flTable.Controls.Add(btn);
            }
        }
        
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<MenuDTO> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);

            float totalPrice = 0;

            foreach (MenuDTO item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");

            //Thread.CurrentThread.CurrentCulture = culture;

            txbTotalPrice.Text = totalPrice.ToString("c",culture);

        }
        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableListInTableManager();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region Event
        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this, new EventArgs());
        }

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }
        private void chuyểnBànToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSwitchTable_Click(this, new EventArgs());
        }
        private void btn_Click(object sender, EventArgs e)
        {
                int tableID = ((sender as Button).Tag as Table).ID;
                lsvBill.Tag = (sender as Button).Tag;
                ShowBill(tableID);
                        
        }

        private void AccountProfile_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        private void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            CategoryDTO selected = cb.SelectedItem as CategoryDTO;

            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as FoodDTO).ID;
            int count = (int)nmFoodCount.Value;
            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);

            LoadTable();
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;

            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);//{1}
            double finalTotalPrice = totalPrice - (totalPrice) * discount/100;//{3}
            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho {0}:\nTổng tiền - Tổng tiền x Giảm giá \n => {1} - {1} x {2}% = {3} nghìn đồng", table.Name, totalPrice * 1000 , discount, finalTotalPrice * 1000), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill,discount,(float)finalTotalPrice ); 
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        #endregion

        private void LogOut_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = loginAccount;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.InsertTable += f_InsertTable;
            f.DeleteTable += f_DeleteTable;
            f.UpdateTable += f_UpdateTable;
            f.InsertCategory += f_InsertCategory;
            f.UpdateCategory += f_UpdateCategory;
            f.DeleteCategory += f_DeleteCategory;
            f.ShowDialog();
        }

        private void f_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        private void f_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        private void f_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboboxTable(cbSwitchTable);
        }
        private void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboboxTable(cbSwitchTable);
        }
        private void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboboxTable(cbSwitchTable);
        }

        private void f_UpdateFood(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as CategoryDTO).ID);
            if(lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        private void f_DeleteFood(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as CategoryDTO).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }
        private void f_InsertFood(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as CategoryDTO).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;

            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }

        private void tsmGuide_Click(object sender, EventArgs e)
        {
            string hd1, hd2, hd3, hd4,hd5;
            hd1 = "Hướng Dẫn Sử Dụng Trang\n1- Cách thêm món :\n+ Chọn Bàn muốn thêm món.\n+ Chọn danh mục và chọn món ở 2 ô bên trái nút Thêm Món.\n+ Chọn số lượng món ở bên phải, rồi ấn nút Thêm Món.\n+ Nếu muốn giảm đi bao nhiêu món bạn thêm dấu (-) trước số lượng, rồi ấn nút Thêm Món.\n=>Bàn đã thêm món sẽ chuyển trạng thái từ 'Trống' sang 'Có người' và danh sách thức ăn bạn thêm sẽ hiển thị ở bảng thông tin bàn bên dưới nút Thêm Món.\n";
            hd2 = "2- Xem lại thông tin bàn :\n+ Chỉ việc ấn vào bàn đó để hiển thị danh sách thức ăn phục vụ bàn đó.\n";
            hd3 = "3- Giảm Giá và Thanh Toán :\n+ Khi thanh toán bạn chọn bàn bạn muốn thanh toán.\n+ Bạn muốn giảm giá bao nhiêu (theo phần trăm) chọn ở ô Giảm Giá.\n+ Rồi ấn thanh toán.\n";
            hd4 = "4- Chuyển bàn :\n+ Chọn bàn muốn chuyển (Click vào bàn đó)\n+ Chọn bàn muốn chuyển tới tại ô dưới nút Chuyển Bàn\n+ Ấn nút Chuyển bàn.\n";
            hd5 = "5- Tổng Tiền :\n+ Ấn vào mỗi bàn sẽ hiện tổng tiền của bàn đó.\n+ Khi thêm hoặc xóa món sẽ thay đổi tổng tiền.\n+ Riêng giảm giá sẽ hiển thị sau khi ấn nút Thanh Toán.\n";
            MessageBox.Show(hd1+hd2+hd3+hd4+hd5);
        }

        private void fTableManager_Load(object sender, EventArgs e)
        {

        }
    }
}
