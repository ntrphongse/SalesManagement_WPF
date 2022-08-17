using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAO
{
    internal class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;
        private readonly static object instanceLock = new object();
        private OrderDetailDAO()
        {

        }
        internal static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }

        internal IEnumerable<OrderDetail> GetOrderDetails(int orderId)
        {
            List<OrderDetail> orderDetails = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                orderDetails = database.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .Include(od => od.Product)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetails;
        }

        internal OrderDetail Add(OrderDetail newOrderDetail)
        {
            OrderDetail orderDetail = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                database.OrderDetails.Add(newOrderDetail);
                database.SaveChanges();
                orderDetail = newOrderDetail;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetail;
        }

        internal OrderDetail GetOrderDetail(int orderId, int productId)
        {
            OrderDetail orderDetail = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                orderDetail = database.OrderDetails
                    .Include(od => od.Product)
                    .SingleOrDefault(od => od.OrderId == orderId
                                    && od.ProductId == productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetail;
        }
        internal void Delete(int orderId, int productId)
        {
            try
            {
                if (GetOrderDetail(orderId, productId) == null)
                {
                    throw new Exception("Order Detail does not exist!!");
                }

                var database = new FStoreDBAssignmentContext();
                database.OrderDetails.Remove(
                    GetOrderDetail(orderId, productId));
                database.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal OrderDetail Update(OrderDetail updatedOrderDetail)
        {
            OrderDetail orderDetail = null;
            try
            {
                if (GetOrderDetail(updatedOrderDetail.OrderId, updatedOrderDetail.ProductId) == null)
                {
                    throw new Exception("Order Detail does not exist!!");
                }
                var database = new FStoreDBAssignmentContext();
                database.OrderDetails.Update(updatedOrderDetail);
                database.SaveChanges();
                orderDetail = updatedOrderDetail;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetail;
        }
    }
}
