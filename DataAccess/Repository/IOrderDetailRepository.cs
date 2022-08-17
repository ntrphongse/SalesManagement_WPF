using BusinessObject;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public interface IOrderDetailRepository
    {
        public IEnumerable<OrderDetail> GetOrderDetails(int orderId);
        public OrderDetail Add(OrderDetail newOrderDetail);
        public OrderDetail GetOrderDetail(int orderId, int productId);
        public void Delete(int orderId, int productId);
        public OrderDetail Update(OrderDetail updatedOrderDetail);
    }
}
