using Quan_Ly_Quan_Cafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quan_Ly_Quan_Cafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return AccountDAO.instance; }
            private set { AccountDAO.instance = value; }
        }
        private AccountDAO() { }
        
        public bool Login(string userName,string passWord)
        {
            //byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            //byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            //string hasPass = "";

            //foreach (byte item in hasData)
            //{
            //    hasPass += item;
            //}
            ////var list = hasData.ToString();
            ////list.Reverse();

            //string query = "USP_Login @userName , @passWord";

            //DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, hasPass /*List*/ });

            //return result.Rows.Count > 0;

            string query = "SELECT * FROM dbo.Account WHERE UserName = N'" + userName + "' AND PassWord = N'" + passWord + "' ";

            DataTable result = DataProvider.Instance.ExecuteQuery(query);

            return result.Rows.Count > 0;
        }
        public bool CheckAccountByUserName(string userName)
        {
            DataTable result = DataProvider.Instance.ExecuteQuery("Select * From Account Where UserName = '"+userName+"'");
            return result.Rows.Count > 0;
        }
        public bool UpdateAccount(string userName, string displayName, string password, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newPassword ",new object[] {userName,displayName,password,newPass});

            return result > 0;
        }    
        public Account Login(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from account where userName = '" + userName +"'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT UserName, DisplayName, Type FROM dbo.Account");
        }

        public bool InsertAccount(string name, string displayName, int type)
        {
            string query = string.Format("INSERT dbo.Account ( UserName, DisplayName, Type, Password)VALUES  ( N'{0}', N'{1}', {2},N'{3}')", name, displayName, type,"0");
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateAccount(string name, string displayName, int type)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{0}', Type = {1} WHERE UserName = N'{2}'", displayName, type, name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        
        public bool DeleteAccount(string name)
        {
            string query = string.Format("Delete Account WHERE UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ResetPassword(string name)
        {
            string query = string.Format("update account set password = N'20720532132149213101239102231223249249135100218' where UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}


