using Core.Models.Account;

namespace Core.Interfaces
{
    public interface IAccountService
    {
        Task<string> UpdateAsync(string email, EditUserViewModel model);
        Task<bool> SendConfirmEmailAsync(string email);
        Task<string> ConfirmEmailAsync(ConfirmEmailViewModel model);
        Task<bool> SendForgotPasswordEmailAsync(ForgotPasswordViewModel model);
        Task<bool> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<IEnumerable<BasketItemViewModel>> Basket(string email);
        Task AddToBasket(string email, BasketViewModel model);
        Task DeleteFromBasket(string email, int productId);
    }
}
