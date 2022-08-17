using BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public interface IOrderRepository
    {
        public IEnumerable<Order> GetOrders();
        public IEnumerable<Order> GetOrders(int memberId);
        public IEnumerable<Order> SearchOrder(DateTime startDate, DateTime endDate);
        public IEnumerable<Order> SearchOrder(DateTime date, FilterMode mode);
        public Order GetOrder(int orderId);
        public Order Add(Order newOrder);
        public Order Update(Order updatedOrder);
        public void Delete(int orderId);
        public int GetNextOderId();
    }
}
