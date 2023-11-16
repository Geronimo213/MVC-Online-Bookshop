namespace Bookshop.Models.ViewModels
{
    public class FrontPageVM
    {

        public List<Carousel> Carousels { get; set; } = new List<Carousel>();

        public string Title { get; set; } = "Welcome to Ye Olde Bookshop!";

    }
}
