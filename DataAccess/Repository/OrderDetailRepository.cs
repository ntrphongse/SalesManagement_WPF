using BusinessObject;
using DataAccess.DAO;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public OrderDetail Add(OrderDetail newOrderDetail) => OrderDetailDAO.Instance.Add(newOrderDetail);

        public void Delete(int orderId, int productId) => OrderDetailDAO.Instance.Delete(orderId, productId);

        public OrderDetail GetOrderDetail(int orderId, int productId) => OrderDetailDAO.Instance.GetOrderDetail(orderId, productId);

        public IEnumerable<OrderDetail> GetOrderDetails(int orderId) => OrderDetailDAO.Instance.GetOrderDetails(orderId);

        public OrderDetail Update(OrderDetail updatedOrderDetail) => OrderDetailDAO.Instance.Update(updatedOrderDetail);
    }
}
