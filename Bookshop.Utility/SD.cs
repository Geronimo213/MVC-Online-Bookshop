﻿namespace Bookshop.Utility
{
    public static class SD
    {
        public const string RoleCustomer = "Customer";
        public const string RoleAdmin = "Admin";
        public const string RoleEmployee = "Employee";

        public const string SessionCart = "SessionShoppingCart";

        public const int PagesAroundIndex = 4;
        public const int PageSizeProduct = 5;

        public const string CartIncrement = "Increment";
        public const string CartDecrement = "Decrement";

        public static readonly string[] stopWords = { "the", "of", "a" };

        public const int NumberCategorySuggestions = 6;
        public const int MaxBooksPerSlider = 12;

        public const string OrderPlaced = "Placed";
        public const string PaymentPending = "Payment Pending";
        public const string PaymentProcessed = "Paid/Awaiting Shipment";

        public const int CoverWidth = 500;
        public const int CoverHeight = 314;

        //Email address from
        public const string NoReplyEmail = "noreply@geronimo.design";
        public const string EmailName = "Ye Olde Bookshop";

        //Email template ids
        public const string ConfirmEmailTemplate = "d-68f001d3399a4bfab1fa4194a9148871";
        public const string ConfirmOrderTemplate = "";

    }
}
