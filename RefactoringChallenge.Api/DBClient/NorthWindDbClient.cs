﻿using MapsterMapper;
using RefactoringChallenge.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RefactoringChallenge.DbClient
{
    public class NorthWindDbClient : IDbClient
    {
        private readonly NorthwindDbContext _northwindDbContext;

        public NorthWindDbClient(NorthwindDbContext northwindDbContext)
        {
            _northwindDbContext = northwindDbContext;
        }

        public void AddProductsToOrder(int orderId, IEnumerable<OrderDetail> orderDetails)
        {

            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return;
            }

            var newOrderDetails = new List<OrderDetail>();
            foreach (var orderDetail in orderDetails)
            {
                newOrderDetails.Add(new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = orderDetail.ProductId,
                    Discount = orderDetail.Discount,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                });
            }

            _northwindDbContext.OrderDetails.AddRange(newOrderDetails);
            _northwindDbContext.SaveChanges();
        }

        public int CreateOrder(Order order)
        {
            var newOrder = _northwindDbContext.Orders.Add(order);
            _northwindDbContext.SaveChanges();
            return newOrder.Entity.OrderId;
        }

        public void DeleteOrder(int orderId)
        {
            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return;

            var orderDetails = _northwindDbContext.OrderDetails.Where(od => od.OrderId == orderId);

            _northwindDbContext.OrderDetails.RemoveRange(orderDetails);
            _northwindDbContext.Orders.Remove(order);
            _northwindDbContext.SaveChanges();
        }

        public Order? GetOrderById(int orderId)
        {
            return _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        public IEnumerable<Order> GetOrders(int? skip, int? take)
        {
            var query = _northwindDbContext.Orders;
            IQueryable<Order> result = query;
            if (skip != null)
            {
                result = query.Skip(skip.Value);
            }
            if (take != null)
            {
                result = result.Take(take.Value);
            }
            return result;
        }

        public bool OrderExists(int orderId)
        {
            return _northwindDbContext.Orders.Any(x => x.OrderId == orderId);
        }

        public bool ProductExists(int productId)
        {
            return _northwindDbContext.Products.Any(x => x.ProductId == productId);
        }
    }
}
