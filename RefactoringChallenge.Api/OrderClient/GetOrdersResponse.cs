using RefactoringChallenge.Entities;
using System.Collections.Generic;

namespace RefactoringChallenge.OrderClient
{
    public enum GetOrdersResult
    {
        Success,
        OrderNotFound,
        UnknownError
    }

    public class GetOrdersResponse
    {
        public GetOrderResult Result { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public string Error { get; set; }
    }
}
