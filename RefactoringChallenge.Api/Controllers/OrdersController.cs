using System;
using System.Collections.Generic;
using System.Linq;
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
            var orderResponse = _orderClient.GetOrderById(orderId);
            switch (orderResponse.Result)
            {
                case GetOrderResult.Success:
                    var result = orderResponse.Order.Adapt<OrderResponse>();
                    return Json(result);
                case GetOrderResult.OrderNotFound:
                    return NotFound(orderResponse.Error);
                case GetOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, orderResponse.Error);
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
            
            var createResponse = _orderClient.CreateOrder(newOrder);
            switch (createResponse.Result)
            {
                case CreateOrderResult.Success:
                    return Json(createResponse.CreatedOrder.Adapt<OrderResponse>());
                case CreateOrderResult.ProductNotFound:
                case CreateOrderResult.CustomerNotFound:
                    return NotFound(createResponse.Error);
                case CreateOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, createResponse.Error);
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

            var addResponse = _orderClient.AddProductsToOrder(orderId, newOrderDetails);

            switch (addResponse.Result)
            {
                case AddProductResult.Success:
                    return Json(newOrderDetails.Select(od => od.Adapt<OrderDetailResponse>()));
                case AddProductResult.OrderNotFound:
                case AddProductResult.ProductNotFound:
                    return NotFound(addResponse.Error);
                case AddProductResult.ProductAlreadyAdded:
                    return StatusCode(StatusCodes.Status409Conflict, addResponse.Error);
                case AddProductResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, addResponse.Error);
            }
        }

        [HttpPost("{orderId}/[action]")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            var deleteResponse = _orderClient.DeleteOrder(orderId);

            switch (deleteResponse.Result)
            {
                case DeleteOrderResult.Success:
                    return Ok();
                case DeleteOrderResult.OrderNotFound:
                    return NotFound(deleteResponse.Error);
                case DeleteOrderResult.UnknownError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, deleteResponse.Error);
            }
        }
    }
}
