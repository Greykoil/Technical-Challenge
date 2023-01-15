namespace RefactoringChallenge.OrderClient
{
    public enum DeleteOrderResult
    {
        Success,
        OrderNotFound,
        UnknownError
    }

    public class DeleteOrderResponse
    {
        public DeleteOrderResult Result { get; set; }
        public string Error { get; set; }
    }
}