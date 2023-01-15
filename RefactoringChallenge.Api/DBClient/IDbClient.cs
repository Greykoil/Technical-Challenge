using RefactoringChallenge.Entities;
using System.Collections.Generic;

namespace RefactoringChallenge.DbClient
{
    public interface IDbClient
    {
        bool OrderExists(int orderId);
        bool ProductExists(int productId);
        bool CustomerExists(string customerId);
        void AddProductsToOrder(int orderId, IEnumerable<OrderDetail> orderDetails);
        Order? GetOrderById(int orderId);
        int CreateOrder(Order order);
        void DeleteOrder(int id);
        IEnumerable<Order> GetOrders(int? skip, int? take);
    }
}
