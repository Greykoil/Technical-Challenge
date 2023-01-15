using System;
using System.Collections.Generic;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefactoringChallenge.Entities;
using RefactoringChallenge.OrderClient;

namespace RefactoringChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderClient _orderClient;
        private readonly IMapper _mapper;

        public OrdersController(IOrderClient orderClient, IMapper mapper)
        {
            _orderClient = orderClient;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? skip = null, int? take = null)
        {
            var orders = _orderClient.GetOrders(skip, take);

            var result = orders.Adapt<IEnumerable<OrderResponse>>();

            return Json(result);
        }

        [HttpGet("{orderId}")]
        public IActionResult GetById([FromRoute] int orderId)
        {
            var order = _orderClient.GetOrderById(orderId);
            switch (order.Result)
            {
                case GetOrderResult.Success:
                    var result = order.Adapt<OrderResponse>();
                    return Json(result);
                case GetOrderResult.OrderNotFound:
                    return NotFound(order.Error);
                case GetOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, order.Error);
            }
        }

        [HttpPost("[action]")]
        public IActionResult Create(
            string customerId,
            int? employeeId,
            DateTime? requiredDate,
            int? shipVia,
            decimal? freight,
            string shipName,
            string shipAddress,
            string shipCity,
            string shipRegion,
            string shipPostalCode,
            string shipCountry,
            IEnumerable<OrderDetailRequest> orderDetails
            )
        {
            var newOrderDetails = new List<OrderDetail>();
            foreach (var orderDetail in orderDetails)
            {
                newOrderDetails.Add(new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Discount = orderDetail.Discount,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                });
            }

            var newOrder = new Order
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = DateTime.Now,
                RequiredDate = requiredDate,
                ShipVia = shipVia,
                Freight = freight,
                ShipName = shipName,
                ShipAddress = shipAddress,
                ShipCity = shipCity,
                ShipRegion = shipRegion,
                ShipPostalCode = shipPostalCode,
                ShipCountry = shipCountry,
                OrderDetails = newOrderDetails,
            };
            
            var result = _orderClient.CreateOrder(newOrder);
            switch (result.Result)
            {
                case CreateOrderResult.Success:
                    return Json(result.CreatedOrder.Adapt<OrderResponse>());
                case CreateOrderResult.ProductNotFound:
                    return NotFound(result.Error);
                case CreateOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
        }

        [HttpPost("{orderId}/[action]")]
        public IActionResult AddProductsToOrder([FromRoute] int orderId, IEnumerable<OrderDetailRequest> orderDetails)
        {
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

            var clientResult = _orderClient.AddProductsToOrder(orderId, newOrderDetails);

            switch (clientResult.Result)
            {
                case AddProductResult.Success:
                    return Json(clientResult.UpdatedOrder.Adapt<OrderDetailResponse>());
                case AddProductResult.OrderNotFound:
                case AddProductResult.ProductNotFound:
                    return NotFound(clientResult.Error);
                    break;
                case AddProductResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, clientResult.Error);
            }
        }

        [HttpPost("{orderId}/[action]")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            var result = _orderClient.DeleteOrder(orderId);

            switch (result.Result)
            {
                case DeleteOrderResult.Success:
                    return Ok();
                case DeleteOrderResult.OrderNotFound:
                    return NotFound(result.Error);
                case DeleteOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
        }
    }
}
