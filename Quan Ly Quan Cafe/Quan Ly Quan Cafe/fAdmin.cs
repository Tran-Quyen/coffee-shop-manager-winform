using Quan_Ly_Quan_Cafe.DAO;
using Quan_Ly_Quan_Cafe.DTO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quan_Ly_Quan_Cafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource tableList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource accountList = new BindingSource();

        public Account loginAccount;
        public fAdmin()
        {
            InitializeComponent();
            LoadAll();
        }
        #region methods

        List<FoodDTO> SearchFoodByName(string name)
        {
            List<FoodDTO> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void LoadAll()
        {
            LoadListDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);

            dtgvFood.DataSource = foodList;
            LoadListFood();
            AddFoodBinding();
            LoadCategoryIntoComboBox(cbFoodCategory);

            dtgvTable.DataSource = tableList;
            LoadListTable();
            AddTableBinding();

            dtgvCategory.DataSource = categoryList;
            LoadListCategory();
            AddCategoryBinding();

            dtgvAccount.DataSource = accountList;
            LoadAccount();
            AddAccountBinding();
        }

        void LoadListDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillByDate(checkIn, checkOut);
        }
        void LoadListBillBillBeforeDeleteTableByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillBeforeDeleteTableByDate(checkIn, checkOut);
        }
        // Hiển thị, Binding Food(fAdmin)
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }
        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }
        void LoadCategoryIntoComboBox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        //Hiển thị, Binding Category(fAdmin)
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }
        void AddCategoryBinding()
        {
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
        }

        // Hiển thị, Binding Table(fAdmin)
        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.GetListTable();
        }
        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
        }

        // Hiển thị, Binding Account(fAdmin)
        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }
        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.CheckAccountByUserName(userName))
            {
                MessageBox.Show("Tài khoản đã tồn tại!!!");
            }
            else if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại!!!");
            }

            LoadAccount();
        }
        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại!!!");
            }

            LoadAccount();
        }
        void DeleteAccount(string userName)
        {
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn xóa {0} ?", userName), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (loginAccount.UserName.Equals(userName))
                {
                    MessageBox.Show("Vui lòng đừng xóa chính bạn chứ!!!");
                    return;
                }
                if (AccountDAO.Instance.DeleteAccount(userName))
                {
                    MessageBox.Show("Xóa tài khoản thành công!");
                }
                else
                {
                    MessageBox.Show("Xóa tài khoản thất bại!!!");
                }
            }
            LoadAccount();
        }
        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công!");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại!!!");
            }
        }
        #endregion

        #region events
        //Button Food
        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            if (dtgvFood.SelectedCells.Count > 0)
            {
                int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                CategoryDTO category = CategoryDAO.Instance.GetCategoryByID(id);

                cbFoodCategory.SelectedItem = category;

                int index = -1;
                int i = 0;
                foreach (CategoryDTO item in cbFoodCategory.Items)
                {
                    if (item.ID == category.ID)
                    {
                        index = i;
                        break;
                    }
                    i++;
                }

                cbFoodCategory.SelectedIndex = index;
            }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as CategoryDTO).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công!");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm thức ăn!!!");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as CategoryDTO).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công!");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn!!!");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as CategoryDTO).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn xóa {0} ?", name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (FoodDAO.Instance.DeleteFood(id))
                {
                    MessageBox.Show("Xóa món thành công!");
                    LoadListFood();
                    if (deleteFood != null)
                        deleteFood(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa thức ăn!!!");
                }
            }
        }

        private void btnViewBill_Click(object sender, System.EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }
        private void btnListDeleteTable_Click(object sender, EventArgs e)
        {
            LoadListBillBillBeforeDeleteTableByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        //Button Category
        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm Danh Mục " + name + " thành công!");
                LoadListCategory();
                LoadCategoryIntoComboBox(cbFoodCategory);
                if (insertCategory != null)
                    insertCategory(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm Danh Mục " + name + "!!!");
            }
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbCategoryName.Text;
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn xóa danh mục: {0} ?\n Thức ăn trong danh mục sẽ được xóa hết!!!", name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (id != -1)
                {
                    CategoryDAO.Instance.DeleteCategory(id);
                    MessageBox.Show("Xóa Danh Mục " + name + " thành công!");
                    LoadListCategory();
                    LoadCategoryIntoComboBox(cbFoodCategory);
                    LoadListFood();
                    if (deleteCategory != null)
                        deleteCategory(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa Danh Mục " + name + "!!!");
                }
            }
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbCategoryName.Text;
            if (CategoryDAO.Instance.UpdateCategory(id, name))
            {
                MessageBox.Show("Sửa Tên Danh Mục thành công!");
                LoadListCategory();
                LoadCategoryIntoComboBox(cbFoodCategory);
                if (updateCategory != null)
                    updateCategory(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa Danh Mục!!!");
            }
        }
        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }
        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }
        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }
        }

        //Button Table
        private void btnShowTable_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Thêm " + name + " thành công!");
                LoadListTable();
                if (insertTable != null)
                    insertTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm " + name + "!!!");
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            int id = Convert.ToInt32(txbTableID.Text);
            if (TableDAO.Instance.UpdateTable(id, name))
            {
                MessageBox.Show("Sửa Bàn thành công!");
                LoadListTable();
                if (updateTable != null)
                    updateTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi Sửa Bàn!!!");
            }
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            int id = Convert.ToInt32(txbTableID.Text);
            string status = txbTableStatus.Text;
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn xóa {0} ?", name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (status == "Trống")
                {
                    TableDAO.Instance.DeleteTable(id);
                    MessageBox.Show("Xóa " + name + " thành công!");
                    LoadListTable();
                    if (deleteTable != null)
                        deleteTable(this, new EventArgs());
                }
                else if (status == "Có người")
                {
                    MessageBox.Show("Bàn " + name + " hiện đang Có Người!! - Hãy Chuyển bàn cho họ");
                }
                else
                {
                    MessageBox.Show("Có lỗi khi Xóa " + name + "!!!");
                }
            }
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }
        //Button Account
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmType.Value;

            AddAccount(userName, displayName, type);
        }
        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            DeleteAccount(userName);
        }
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmType.Value;

            EditAccount(userName, displayName, type);
        }
        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            ResetPass(userName);
        }
        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        #endregion

        private void btnFirstBillPage_Click(object sender, EventArgs e)
        {
            txbPageBill.Text = "1";
        }

        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            int lastPage = sumRecord / 10;

            if (sumRecord % 10 != 0)
                lastPage++;

            txbPageBill.Text = lastPage.ToString();
        }

        private void txbPageBill_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
        }

        private void btnNextBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            if (page < sumRecord)
                page++;

            txbPageBill.Text = page.ToString();
        }

        private void btnPreviosBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);

            if (page > 1)
                page--;

            txbPageBill.Text = page.ToString();
        }

        private void btnTotalPrice_Click(object sender, EventArgs e)
        {
            double t1,t2;
            DateTime d1 = dtpkFromDate.Value;
            DateTime d2 = dtpkToDate.Value;
            t1=BillDAO.Instance.GetTotalPriceByDate(dtpkFromDate.Value, dtpkToDate.Value);
            t2=BillDAO.Instance.GetTotalPriceDeleteTableByDate(dtpkFromDate.Value, dtpkToDate.Value);
            MessageBox.Show("Từ ngày " + d1.Day + "/" + d1.Month + "/" + d1.Year + " đến hết ngày " + d2.Day + "/" + d2.Month + "/" + d2.Year + "\nTổng số tiền kiếm được : " + (t1 + t2)*1000 + " đồng chẵn"); 
        }

        private void btnhdsdTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hướng dẫn chi tiết.....(Hoàn Thành Test)");
        }

        private void btnhdsdBill_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I- Doanh Thu\n* Ban đầu vào trang, trang sẽ hiển thị tháng hiện tại với lịch bên phải là ngày đầu tháng và lịch bên trái là ngày cuối tháng và danh sách toàn bộ hóa đơn trong tháng (trừ TH bạn chưa có hóa đơn).\n1- Nút Thống Kê: Dùng để hiển thị toàn bộ hóa đơn từ ngày trong lịch bên trái và lịch bên phải.\n* Viết tắt: k <=> khoảng thời gian bạn chọn\n2- First: Hiển thị 10 hóa đơn đầu tiên trong k\n3- Last: Hiển thị 10 hóa đơn cuối cùng trong k\n4- Previos: Hiển thị 10 hóa đơn trang trước trang hiện tại trong k\n5- Next: Hiện thị 10 hóa đơn trang sau trang hiện tại trong k\n6- DS Xóa Bàn: hiển thị các hóa đơn trong k của các bàn đã xóa\n7- Tổng Tiền: Hiển thị thu nhập thu về trong k\n* Lưu ý: có thể thay đổi từ ngày nào (trong lịch bên trái) đến ngày nào (trong lịch bên phải).");
        }

        private void btnhdsdFood_Click(object sender, EventArgs e)
        {
            MessageBox.Show("II- Thức Ăn\n* Ban đầu khi vào trang, trang sẽ hiển thị một danh sách thức ăn đi kèm với giá và id danh mục (trừ TH bạn chưa thêm món ăn nào).\n1- Tìm: Tìm tên món ăn chứa chuỗi hoặc ký tự bạn nhập vào bất kể hoa thường trước đó hãy nhập chuỗi hoặc ký tự bạn muốn tìm ở ô bên trái nút Tìm.\n2- Thêm: Thêm món ăn vào danh sách.\n+ Đầu tiên bạn click bất kỳ vào danh sách.\n+ Bạn sửa thông tin Tên Món, chọn Danh Mục bạn muốn và Giá của món ăn.\n+ Sau khi hoàn thành bước 2 bạn ấn vào nút Thêm.\n3- Xóa: Xóa món ăn khỏi danh sách.\n+ Cách 1: Đầu tiên bạn click vào món ăn bạn muốn xóa trong danh sách rồi ấn xóa.\n+ Cách 2: Tìm kiếm nó bằng nút Tìm và chọn nó trong danh sách rồi xóa.\n4- Sửa: Sửa thông tin món ăn bạn muốn hoặc thay thế nó bằng món khác.\n+ Chọn món ăn bạn muốn sửa theo 2 cách ấn trực tiếp hoặc bằng nút Tìm.\n+ Sửa thông tin trong 3 ô Tên Món, Chọn lại Danh Mục ,Giá.\n* Chú ý:bạn có thể sửa một trong 3 thông tin trên hoặc 2,3 thông tin.\n+ Ấn nút sửa.\n5- Xem: Xem toàn bộ danh sách thức ăn.\n");
        }

        private void btnhdsdCategory_Click(object sender, EventArgs e)
        {
            MessageBox.Show("III- Danh Mục\n* Khi vào trang, trang sẽ hiển thị danh sách các danh mục đã có (trừ TH bạn chưa thêm danh mục nào).\n1- Thêm: Thêm danh mục mới.\n+ Click vào một mục bất kỳ.\n + Sửa tên danh mục thành danh mục bạn muốn thêm.\n+ Ấn nút Thêm.\n2- Xóa: Xóa một danh mục.\n+ Click vào tên danh mục bạn muốn xóa.\n+ Ấn nút Xóa.\n3- Sửa: Sửa tên danh mục.\n+ Click tên danh mục bạn muốn sửa tên.\n+ Sửa tên danh mục đó.\n+ Ấn nút Sửa.\n4- Xem: Xem toàn bộ danh mục.\n");
        }

        private void btnhdsdTable_Click(object sender, EventArgs e)
        {
            MessageBox.Show("IV- Bàn Ăn\n* Khi vào trang, trang sẽ hiển thị một danh sách bàn ăn (trừ TH bạn chưa thêm bàn ăn nào).\n1- Thêm: Thêm một bàn mới vào ds.\n+ Click bất kỳ vào danh sách bàn ăn.\n+ Sửa tên bàn cho bàn mình muốn thêm ở ô Tên Bàn bên trái.\n+ Ấn nút Thêm.\n2- Xóa: Xóa một bàn khỏi ds.\n+ Click vào bàn bạn muốn xóa.\n+ Ấn nút Xóa.\n3- Sửa : Sửa tên một bàn trong ds.\n+ Click vào bàn bạn muốn sửa trong danh sách.\n+ Sửa tên bàn trong ô Tên Bàn ở bên trái.\n+ Ấn nút Sửa.\n4- Xem: Xem toàn bộ bàn ăn trong ds.\n");
        }

        private void btnhdsdAccount_Click(object sender, EventArgs e)
        {
            MessageBox.Show("V- Tài Khoản\n* Khi vào trang, trang sẽ hiển thị danh sách gồm tài khoản, tên hiển thị và loại tài khoản.\n* Loại tài khoản: 1- Admin(Chủ Quán)\n                             0- staff(Nhân Viên)\n* Lưu ý: Trang Admin chỉ có loại tài khoản 1 mới được phép sử dụng, còn loại 0 sẽ bị đóng băng tự động.\n1- Thêm: Thêm một tài khoản mới.\n+ Click bất kỳ vào danh sách tài khoản.\n+ Sửa tên tài khoản(tên đăng nhập) và tên hiển thị bạn muốn.\n+ Ấn nút Thêm.\n* Lưu ý: Mật khẩu mặc định ban đầu là số 0, bạn có thể đổi mật khẩu ở trang Thông Tin Tài Khoản ở Giao Diện: Phần Mềm Quản Lý Quán Cafe.\n2- Xóa: Xóa một tài khoản.\n+ Click vào tài khoản bạn muốn xóa.\n+ Ấn nút Xóa.\n* Lưu ý: Bạn không thể xóa tài khoản bạn đang sử dụng hiện tại trên phần mềm!!!\n3- Sửa: Sửa thông tin tài khoản.\n+ Click vào tài khoản bạn muốn sửa trong ds.\n+ Sửa thông tin trong 2 ô Tài Khoản, Tên Hiển Thị và Loại Tài Khoản ở bên trái.\n+ Sau khi sửa những thông tin bạn muốn, Ấn nút sửa.\n4- Xem: Xem toàn bộ danh sách tài khoản.\n5- Đặt Lại Mật Khẩu: Đặt mật khẩu của tài khoản về mặc định là số 0.\n");

        }
    }
}