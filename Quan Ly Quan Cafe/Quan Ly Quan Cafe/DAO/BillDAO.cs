using Quan_Ly_Quan_Cafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quan_Ly_Quan_Cafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return BillDAO.instance; }
            private set { BillDAO.instance = value; }
        }
        //Thành Công: Bill ID
        //Thất Bại:-1
        private BillDAO() { }

        public int GetUnCheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable =" + id + " AND status = 0");

            if(data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }

            return -1;
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = " UPDATE dbo.Bill SET dateCheckOut = GETDATE(), status = 1 , discount = " + discount + ", totalPrice =" + totalPrice + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery(" exec USP_InsertBill @idTable",new object[] { id });
        }

        public DataTable GetListBillByDate(DateTime checkIn,DateTime checkOut)
        {
           return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut",new object[] { checkIn,checkOut});
        }
        public DataTable GetListBillBeforeDeleteTableByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery(" USP_GetListBillBeforeDeleteTableByDate @checkIn , @checkOut ", new object[] { checkIn, checkOut });
        }
        public DataTable GetListBillByDateAndPage(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDateAndPage @checkIn , @checkOut , @page", new object[] { checkIn, checkOut, pageNum });
        }

        public int GetNumBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("exec USP_GetNumBillListByDate @checkIn , @checkOut ", new object[] { checkIn,checkOut });
        }
        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar(" Select Max(id) from dbo.Bill ");
            }
            catch 
            {
                return 1; 
            }

        }

        public double GetTotalPriceByDate(DateTime checkIn,DateTime checkOut)
        {
            return (double)DataProvider.Instance.ExecuteScalar("exec USP_GetTotalPriceOnBillByDate @checkIn , @checkOut ", new object[] { checkIn, checkOut });
        }
        public double GetTotalPriceDeleteTableByDate(DateTime checkIn, DateTime checkOut)
        {
            //double totalPrice = (double)DataProvider.Instance.ExecuteScalar("exec USP_GetTotalPriceOnBillDeleteByDate @checkIn , @checkOut ", new object[] { checkIn, checkOut });
            //if (totalPrice > 0)
            //{
            //    return totalPrice;
            //}
            return 0; 
        }
    }
}
