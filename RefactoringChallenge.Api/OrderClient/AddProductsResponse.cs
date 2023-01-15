using RefactoringChallenge.Entities;

namespace RefactoringChallenge.OrderClient
{
    public enum AddProductResult
    {
        Success,
        OrderNotFound,
        ProductNotFound,
        UnknownError
    }

    public class AddProductsResponse
    {
        public AddProductResult Result { get; set; }
        public string Error { get; set; }
        public Order? UpdatedOrder { get; set; }

    }
}