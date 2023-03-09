using System.Collections.Generic;
using InterviewTest.Orders;
using InterviewTest.Returns;

namespace InterviewTest.Customers
{
    public interface ICustomer
    {
        string GetName();
        void CreateOrder(IOrder order);
        void CreateReturn(IReturn rga, string customerName);
        List<orderInfo> GetOrders(string customerName);
        List<returnInfo> GetReturns(string customerName);
        float GetTotalSales(string customerName);
        float GetTotalReturns(string customerName);
        float GetTotalProfit(string customerName);
    }
}
