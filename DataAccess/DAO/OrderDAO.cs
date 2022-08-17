using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAO
{
    internal class OrderDAO
    {
        private static OrderDAO instance = null;
        private static readonly object instanceLock = new object();
        private OrderDAO()
        {

        }
        internal static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        internal IEnumerable<Order> GetOrders()
        {
            List<Order> orders = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                orders = database.Orders
                    .Include(order => order.Member)
                    .Include(order => order.OrderDetails)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        internal IEnumerable<Order> GetOrders(int memberId)
        {
            List<Order> orders = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                orders = database.Orders.Where(order => order.MemberId == memberId)
                    .Include(order => order.Member)
                    .Include(order => order.OrderDetails)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        internal IEnumerable<Order> SearchOrder(DateTime startDate, DateTime endDate)
        {
            List<Order> orders = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                orders = database.Orders
                    .Where(order => DateTime.Compare(order.OrderDate, startDate) >= 0 &&
                            DateTime.Compare(order.OrderDate, endDate) <= 0)
                    .Include(order => order.Member)
                    .Include(order => order.OrderDetails)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        internal IEnumerable<Order> SearchOrder(DateTime date, FilterMode mode)
        {
            List<Order> orders = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                if (mode == FilterMode.Min)
                {
                    orders = database.Orders
                        .Where(order => DateTime.Compare(order.OrderDate, date) >= 0)
                        .Include(order => order.Member)
                        .Include(order => order.OrderDetails)
                        .ToList();
                }
                else if (mode == FilterMode.Max)
                {
                    orders = database.Orders
                        .Where(order => DateTime.Compare(order.OrderDate, date) <= 0)
                        .Include(order => order.Member)
                        .Include(order => order.OrderDetails)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        internal Order GetOrder(int orderId)
        {
            Order order = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                order = database.Orders
                    .Include(order => order.Member)
                    .Include(order => order.OrderDetails)
                    .SingleOrDefault(or => or.OrderId == orderId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }
        internal Order Add(Order newOrder)
        {
            Order order = null;
            try
            {
                if (GetOrder(newOrder.OrderId) != null)
                {
                    throw new Exception($"The order with the ID {newOrder.OrderId} " +
                        $"is existed!!");
                }
                var database = new FStoreDBAssignmentContext();
                database.Orders.Add(newOrder);
                database.SaveChanges();
                order = newOrder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }

        internal Order Update(Order updatedOrder)
        {
            Order order = null;
            try
            {
                if (GetOrder(updatedOrder.OrderId) == null)
                {
                    throw new Exception("Order is not existed! Please check again...");
                }
                var database = new FStoreDBAssignmentContext();
                database.Orders.Update(updatedOrder);
                database.SaveChanges();
                order = updatedOrder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }

        internal void Delete(int orderId)
        {
            try
            {
                if (GetOrder(orderId) == null)
                {
                    throw new Exception("Order is not existed! Please check again...");
                }
                var database = new FStoreDBAssignmentContext();
                database.Orders.Remove(GetOrder(orderId));
                database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal int GetNextOderId()
        {
            int nextId = -1;
            try
            {
                var database = new FStoreDBAssignmentContext();
                Order currentMax = database.Orders
                    .OrderByDescending(order => order.OrderId)
                    .First();
                nextId = currentMax.OrderId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return nextId;
        }
    }
}
