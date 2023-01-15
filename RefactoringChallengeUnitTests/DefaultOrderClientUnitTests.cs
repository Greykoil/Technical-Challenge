using Moq;
using RefactoringChallenge.DbClient;
using RefactoringChallenge.Entities;
using RefactoringChallenge.OrderClient;
using System.Collections.Generic;
using Xunit;

namespace RefactoringChallengeUnitTests
{
    public class DefaultOrderClientUnitTests
    {
        [Fact]
        public void Given_OrderExists_ThenSuccessReturned_AndOrderHasValues()
        {
            var dbOrder = new Order()
            {
                OrderId = 1,
                ShipName = "ShipName"
            };

            var dbClient = new Mock<IDbClient>();
            dbClient.Setup(x => x.OrderExists(It.IsAny<int>())).Returns(true);
            dbClient.Setup(x => x.GetOrderById(It.Is<int>(x => x == 0))).Returns(dbOrder);

            DefaultOrderClient client = new DefaultOrderClient(dbClient.Object);

            var result = client.GetOrderById(0);

            Assert.Equal(GetOrderResult.Success, result.Result);
            Assert.NotNull(result.Order);
            Assert.Equal(dbOrder.OrderId, result.Order.OrderId);
            Assert.Equal(dbOrder.ShipName, result.Order.ShipName);
        }

        [Fact]
        public void Given_OrderNotFoundInDb_ThenFailureCodeReturned()
        {
            var dbOrder = new Order()
            {
                OrderId = 1,
                ShipName = "ShipName"
            };

            var dbClient = new Mock<IDbClient>();
            dbClient.Setup(x => x.OrderExists(It.IsAny<int>())).Returns(false);

            DefaultOrderClient client = new DefaultOrderClient(dbClient.Object);

            var result = client.GetOrderById(0);

            Assert.Equal(GetOrderResult.OrderNotFound, result.Result);
            Assert.Null(result.Order);
        }

        [Fact]
        public void GivenAddProductsToOrder_WhenOrderAlreadyHasMatchingProducts_ThenFailureCodeReturned()
        {

            var dbOrder = new Order()
            {
                OrderId = 1,
                ShipName = "ShipName",
                OrderDetails = new List<OrderDetail>() {
                    new OrderDetail() { Product = new Product() { ProductId = 1}, Quantity = 1, OrderId = 1 },
                    new OrderDetail() { Product = new Product() { ProductId = 2}, Quantity = 1, OrderId = 1 },
                    new OrderDetail() { Product = new Product() { ProductId = 3}, Quantity = 1, OrderId = 1 } }
            };

            var dbClient = new Mock<IDbClient>();
            dbClient.Setup(x => x.OrderExists(It.IsAny<int>())).Returns(true);
            dbClient.Setup(x => x.GetOrderById(It.IsAny<int>())).Returns(dbOrder);

            DefaultOrderClient client = new DefaultOrderClient(dbClient.Object);


            var response = client.AddProductsToOrder(1, new List<OrderDetail>()
                {
                    new OrderDetail()  {Product = new Product() {ProductId = 4 }, Quantity = 1, OrderId=1},
                    new OrderDetail()  {Product = new Product() {ProductId = 1 }, Quantity = 5, OrderId=1},
                });

            Assert.Equal(AddProductResult.ProductAlreadyAdded, response.Result);
            Assert.Null(response.UpdatedOrder);
        }
    }
}