using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace InterviewTest.Orders
{
    public struct orderInfo
    {
        public string orderNumber { get; set; }
        public string customerName { get; set; }
        public string[] products { get; set; }
    }
    public class OrderRepository
    {
        
        public List<orderInfo> orderList;
        private List<IOrder> orders;
        SqlConnection sqlCn;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sarim\OneDrive\Desktop\Meyer Distributing Exam - Mid\InterviewTest\InterviewTestDB.mdf;Integrated Security=True";
        public OrderRepository()
        {
            orders = new List<IOrder>();
            orderList = new List<orderInfo>();
        }

        public void Add(IOrder newOrder)
        {
            string productNames = "";
            foreach(var item in newOrder.Products)
            {
                productNames = productNames + item.Product.GetProductNumber() + ",";
            }
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_AddOrder";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            sqlCmd.Parameters.AddWithValue("@orderNumber", newOrder.OrderNumber);
            sqlCmd.Parameters.AddWithValue("@customernName", newOrder.Customer.GetName());
            sqlCmd.Parameters.AddWithValue("@producs", productNames);
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Connection.Close();
            sqlCmd.Dispose();

            //Add to product purchase history
            string[] productNumbers = productNames.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < productNumbers.Length; i++)
            {
                sqlCn = new SqlConnection(connectionString);
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlCn;
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.CommandText = "usp_AddPurchaseHistory";
                sqlCmd.Parameters.Clear();
                sqlCmd.Connection.Open();
                sqlCmd.Parameters.AddWithValue("@productNumber", productNumbers[i]);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Connection.Close();
                sqlCmd.Dispose();
            }
            

            //orders.Add(newOrder);
        }

        public void Remove(IOrder removedOrder)
        {
            orders = orders.Where(o => !string.Equals(removedOrder.OrderNumber, o.OrderNumber)).ToList();
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_RemoveOrder";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            foreach (var item in orders)
            {
                sqlCmd.Parameters.AddWithValue("@orderNumber", item.OrderNumber);
                sqlCmd.ExecuteNonQuery();
            }
            sqlCmd.Connection.Close();
            sqlCmd.Dispose();
        }

        public List<orderInfo> Get(string customerName)
        {
            List<orderInfo> lstOrders = new List<orderInfo>();
            DataTable dt = new DataTable();
            sqlCn = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCn;
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            sqlCmd.CommandText = "usp_GetAllOrders";
            sqlCmd.Parameters.Clear();
            sqlCmd.Connection.Open();
            sqlCmd.Parameters.AddWithValue("@customerName", customerName);
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);
            sqlCmd.Connection.Close();

            for(int i = 0; i < dt.Rows.Count; i++)
            {

                lstOrders.Add(new orderInfo
                {
                    orderNumber = dt.Rows[i]["OrderNumber"].ToString(),
                    customerName = dt.Rows[i]["CustomerName"].ToString(),
                    products = dt.Rows[i]["Products"].ToString().Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray()
                });
            }

            return lstOrders;
        }
    }
}
