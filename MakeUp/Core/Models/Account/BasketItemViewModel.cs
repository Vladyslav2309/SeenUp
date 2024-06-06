using Core.Models.Products;

namespace Core.Models.Account
{
    public class BasketItemViewModel
    {
        public int Count { get; set; }
        public ProductItemViewModel Product { get; set; }
    }
}
