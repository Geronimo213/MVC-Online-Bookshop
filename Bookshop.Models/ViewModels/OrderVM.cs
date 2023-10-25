namespace Bookshop.Models.ViewModels
{
    public class OrderVM
    {
        public Order Header { get; set; }
        public IEnumerable<OrderLines> Lines { get; set; }

        public OrderVM()
        {
            this.Header = new Order();
            this.Lines = new List<OrderLines>();
        }
    }
}
