using RefactoringChallenge.Entities;

namespace RefactoringChallenge.OrderClient
{
    public enum GetOrderResult
    {
        Success,
        OrderNotFound,
        UnknownError
    }

    public class GetOrderResponse
    {
        public GetOrderResult Result { get; set; }
        public Order? Order { get; set; }
        public string Error { get; set; }
    }
}
