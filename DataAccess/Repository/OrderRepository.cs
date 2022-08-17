using BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public Order Add(Order newOrder) => OrderDAO.Instance.Add(newOrder);

        public void Delete(int orderId) => OrderDAO.Instance.Delete(orderId);

        public int GetNextOderId() => OrderDAO.Instance.GetNextOderId();

        public Order GetOrder(int orderId) => OrderDAO.Instance.GetOrder(orderId);

        public IEnumerable<Order> GetOrders() => OrderDAO.Instance.GetOrders();

        public IEnumerable<Order> GetOrders(int memberId) => OrderDAO.Instance.GetOrders(memberId);

        public IEnumerable<Order> SearchOrder(DateTime startDate, DateTime endDate) => OrderDAO.Instance.SearchOrder(startDate, endDate);

        public IEnumerable<Order> SearchOrder(DateTime date, FilterMode mode) => OrderDAO.Instance.SearchOrder(date, mode);

        public Order Update(Order updatedOrder) => OrderDAO.Instance.Update(updatedOrder);
    }
}
