using System;
using System.Collections.Generic;
using InterviewTest.Orders;
using InterviewTest.Returns;
using System.Data;
using System.Data.SqlClient;

namespace InterviewTest.Customers
{
    public abstract class CustomerBase : ICustomer
    {
        private readonly OrderRepository _orderRepository;
        private readonly ReturnRepository _returnRepository;
        SqlConnection sqlCn;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sarim\OneDrive\Desktop\Meyer Distributing Exam - Mid\InterviewTest\InterviewTestDB.mdf;Integrated Security=True";

        protected CustomerBase(OrderRepository orderRepo, ReturnRepository returnRepo)
        {
            _orderRepository = orderRepo;
            _returnRepository = returnRepo;
        }

        public abstract string GetName();
        
        public void CreateOrder(IOrder order)
        {
            _orderRepository.Add(order);
        }

        public List<orderInfo> GetOrders(string customerName)
        {
            return _orderRepository.Get(customerName);
        }

        public void CreateReturn(IReturn rga, string customerName)
        {
            _returnRepository.Add(rga, customerName);
        }

        public List<returnInfo> GetReturns(string customerName)
        {
            return _returnRepository.Get(customerName);
        }

        public float GetTotalSales(string customerName)
        {
            List<orderInfo> allOrders = GetOrders(customerName);
            float num = 0;

            DataTable dt = new DataTable();
            foreach (var item in allOrders)
            {
                
                for(int i = 0; i < item.products.Length; i++)
                {
                    sqlCn = new SqlConnection(connectionString);
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlCn;
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCmd.CommandText = "usp_GetProduct";
                    sqlCmd.Parameters.Clear();
                    sqlCmd.Connection.Open();
                    sqlCmd.Parameters.AddWithValue("@ProductNumber", item.products[i]);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                    sqlDA.Fill(dt);
                    sqlCmd.Connection.Close();
                    string tNum = dt.Rows[0]["ProductPrice"].ToString();
                    num += Convert.ToInt32(tNum);
                    dt = new DataTable();
                }
                
            }
           
            return num;
        }

        public float GetTotalReturns(string customerName)
        {
            List<returnInfo> allReturns = GetReturns(customerName);
            float num = 0;

            DataTable dt = new DataTable();
            foreach (var item in allReturns)
            {

                for (int i = 0; i < item.products.Length; i++)
                {
                    sqlCn = new SqlConnection(connectionString);
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlCn;
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlCmd.CommandText = "usp_GetProduct";
                    sqlCmd.Parameters.Clear();
                    sqlCmd.Connection.Open();
                    sqlCmd.Parameters.AddWithValue("@ProductNumber", item.products[i]);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                    sqlDA.Fill(dt);
                    sqlCmd.Connection.Close();
                    string tNum = dt.Rows[0]["ProductPrice"].ToString();
                    num += Convert.ToInt32(tNum);
                    dt = new DataTable();
                }

            }

            return num;
        }

        public float GetTotalProfit(string customerName)
        {
            float totalProfit = GetTotalSales(customerName) - GetTotalReturns(customerName);
            return totalProfit;
        }
    }
}
