using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quan_Ly_Quan_Cafe.DTO
{
    public class CategoryDTO
    {
        public CategoryDTO(int id,string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public CategoryDTO(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
        }

        private int iD;

        public int ID 
        {
            get { return iD;  }
            set { iD = value;  } 
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
