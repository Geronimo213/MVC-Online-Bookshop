namespace Bookshop.Utility
{
    public static class SD
    {
        public const string RoleCustomer = "Customer";
        public const string RoleAdmin = "Admin";
        public const string RoleEmployee = "Employee";

        public const string SessionCart = "SessionShoppingCart";

        public const int PagesAroundIndex = 4;
        public const int PageSizeProduct = 5;
        public const int PageSizeOrder = 5;

        public const string CartIncrement = "Increment";
        public const string CartDecrement = "Decrement";

        public static readonly string[] StopWords = { "the", "of", "a" };

        public const int NumberCategorySuggestions = 6;
        public const int MaxBooksPerSlider = 12;

        public const string OrderPlaced = "Placed";
        public const string PaymentPending = "Payment Pending";
        public const string PaymentProcessed = "Paid/Awaiting Shipment";
        public const string OrderShipped = "Shipped";

        public const int CoverHeight = 500;
        public const int CoverWidth = 314;

        //Email address from
        public const string NoReplyEmail = "noreply@geronimo.design";
        public const string EmailName = "Ye Olde Bookshop";

        //Email template ids
        public const string ConfirmEmailTemplate = "d-68f001d3399a4bfab1fa4194a9148871";
        public const string ConfirmOrderTemplate = "d-a95ce7a96a934e03bf5803b8da1ab996";
        public const string ResetPasswordTemplate = "d-e83ab55a12aa4c789f2c3138df654503";
        public const string PasswordChangeNotificationTemplate = "d-c8941906e86c4c3e8290b5fbdce56d37";

    }
}
