﻿using RefactoringChallenge.DbClient;
using RefactoringChallenge.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RefactoringChallenge.OrderClient
{
    public class DefaultOrderClient : IOrderClient
    {
        public IDbClient DbClient { get; }

        public DefaultOrderClient(IDbClient dbClient)
        {
            DbClient = dbClient;
        }

        public AddProductsResponse AddProductsToOrder(int orderId, IEnumerable<OrderDetail> orderDetails)
        {
            if (!DbClient.OrderExists(orderId))
            {
                return new AddProductsResponse()
                {
                    Result = AddProductResult.OrderNotFound,
                    UpdatedOrder = null,
                    Error = $"Order not found for Id {orderId}"
                };
            }

            var current = DbClient.GetOrderById(orderId);


            // This logic should probably be in the db context rather than here,  
            // It is not clear what should happen in this case. 
            // Currently the whole update is rejected, but it would be possible to ignore the duplicate, 
            // or to update the duplicate details.
            foreach (var item in orderDetails)
            {
                if (current.OrderDetails.Any(x => x.ProductId == item.ProductId))
                {
                    return new AddProductsResponse()
                    {
                        Result = AddProductResult.ProductAlreadyAdded,
                        UpdatedOrder = null,
                        Error = $"Product {item.ProductId} already exists for order {orderId}"
                    };
                }
            }

            foreach (var productId in orderDetails.Select(x => x.ProductId))
            {
                if (!DbClient.ProductExists(productId))
                {
                    return new AddProductsResponse()
                    {
                        Result = AddProductResult.ProductNotFound,
                        UpdatedOrder = null,
                        Error = $"Product not found for id {productId}"
                    };
                }
            }

            DbClient.AddProductsToOrder(orderId, orderDetails);
            Order updatedOrder = DbClient.GetOrderById(orderId);

            if (updatedOrder == null)
            {
                return new AddProductsResponse()
                {
                    Result = AddProductResult.UnknownError,
                    UpdatedOrder = null,
                    Error = $"Unable to find order {orderId} after updating"
                };
            }

            return new AddProductsResponse()
            {
                Result = AddProductResult.Success,
                UpdatedOrder = updatedOrder,
                Error = ""
            };
        }

        public CreateOrderResponse CreateOrder(Order order)
        {
            if (!DbClient.CustomerExists(order.CustomerId))
            {
                return new CreateOrderResponse()
                {
                    Result = CreateOrderResult.CustomerNotFound,
                    CreatedOrder = null,
                    Error = $"Customer not found for id {order.CustomerId}"
                };
            }

            foreach (var productId in order.OrderDetails.Select(x => x.ProductId))
            {
                if (!DbClient.ProductExists(productId))
                {
                    return new CreateOrderResponse()
                    {
                        Result = CreateOrderResult.ProductNotFound,
                        CreatedOrder = null,
                        Error = $"Product not found for id {productId}"
                    };
                }
            }
            int newOrderId = DbClient.CreateOrder(order);
            var createdOrder = DbClient.GetOrderById(newOrderId);
            if (createdOrder == null)
            {
                return new CreateOrderResponse()
                {
                    Result = CreateOrderResult.UnknownError,
                    CreatedOrder = null,
                    Error = $"Unknown error encountered when getting created order {newOrderId}"
                };
            }
            return new CreateOrderResponse()
            {
                Result = CreateOrderResult.Success,
                CreatedOrder = createdOrder,
                Error = ""
            };
        }

        public DeleteOrderResponse DeleteOrder(int id)
        {
            if (!DbClient.OrderExists(id))
            {
                return new DeleteOrderResponse()
                {
                    Result = DeleteOrderResult.OrderNotFound,
                    Error = $"Unable to find order {id}"
                };
            }
            DbClient.DeleteOrder(id);
            return new DeleteOrderResponse()
            {
                Result = DeleteOrderResult.Success,
                Error = ""
            };
        }

        public GetOrderResponse GetOrderById(int id)
        {
            var order = DbClient.GetOrderById(id);
            if (order == null)
            {
                return new GetOrderResponse()
                {
                    Result = GetOrderResult.OrderNotFound,
                    Order = null,
                    Error = $"Order not found for id {id}"
                };
            }

            return new GetOrderResponse()
            {
                Result = GetOrderResult.Success,
                Order = order,
                Error = ""
            };
        }

        public IEnumerable<Order> GetOrders(int? skip = null, int? take = null)
        {
            IEnumerable<Order> orders = DbClient.GetOrders(skip, take);
            return orders;
        }
    }
}
