using RefactoringChallenge.Entities;

namespace RefactoringChallenge.OrderClient
{
    public enum CreateOrderResult
    {
        Success,
        ProductNotFound,
        CustomerNotFound,
        UnknownError
    }

    public class CreateOrderResponse
    {
        public CreateOrderResult Result { get; set; }
        public Order? CreatedOrder { get; set; }
        public string Error { get; set; }
    }
}