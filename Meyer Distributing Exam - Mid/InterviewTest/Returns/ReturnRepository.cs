using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace InterviewTest.Returns
{
    public struct returnInfo
    {
        public string orderNumber { get; set; }
        public string returnNumber { get; set; }
        public string[] products { get; set; }
    }
    public class ReturnRepository
    {
        
        private List<IReturn> returns;
        SqlConnection sqlCn;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sarim\OneDrive\Desktop\Meyer Distributing Exam - Mid\InterviewTest\InterviewTestDB.mdf;Integrated Security=True";
        public ReturnRepository()
        {
            returns = new List<IReturn>();
        }

        public void Add(IReturn newReturn, string customerName)
         {
            string productNames = "";
            foreach (var item in newReturn.ReturnedProducts)
            {
                productNames = productNames + item.OrderProduct.Product.GetProductNumber() + ",";
            }
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_AddReturn";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            sqlCmd.Parameters.AddWithValue("@returnNumber", newReturn.ReturnNumber);
            sqlCmd.Parameters.AddWithValue("@originalOrderNumber", newReturn.OriginalOrder.OrderNumber);
            sqlCmd.Parameters.AddWithValue("@returnedProducts", productNames);
            sqlCmd.Parameters.AddWithValue("@customerName", customerName);
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Connection.Close();
            sqlCmd.Dispose();
        }

        public void Remove(IReturn removedReturn)
        {
            returns = returns.Where(o => !string.Equals(removedReturn.ReturnNumber, o.ReturnNumber)).ToList();
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_RemoveReturn";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            foreach (var item in returns)
            {
                sqlCmd.Parameters.AddWithValue("@returnNumber", item.ReturnNumber);
                sqlCmd.ExecuteNonQuery();
            }
            sqlCmd.Connection.Close();
            sqlCmd.Dispose();
        }

        public List<returnInfo> Get(string customerName)
        {
            List<returnInfo> lstReturns = new List<returnInfo>();
            DataTable dt = new DataTable();
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_GetAllReturns";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            sqlCmd.Parameters.AddWithValue("@customerName", customerName);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);
            sqlCmd.Connection.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                lstReturns.Add(new returnInfo
                {
                    orderNumber = dt.Rows[i]["OriginalOrderNumber"].ToString(),
                    returnNumber = dt.Rows[i]["ReturnNumber"].ToString(),
                    products = dt.Rows[i]["ReturnedProducts"].ToString().Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray()
                });
            }

            return lstReturns;
        }
    }
}
