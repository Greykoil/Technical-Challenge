using RefactoringChallenge.Entities;
using System.Collections.Generic;

namespace RefactoringChallenge.OrderClient
{
    public interface IOrderClient
    {

        IEnumerable<Order> GetOrders(int? skip = null, int? take = null);

        GetOrderResponse GetOrderById(int id);

        CreateOrderResponse CreateOrder(Order order);

        AddProductsResponse AddProductsToOrder(int orderId, IEnumerable<OrderDetail> orderDetails);

        DeleteOrderResponse DeleteOrder(int id);
    }
}
