using Infastructure.Entities.Identity;
using Google.Apis.Auth;

namespace Core.Interfaces
{
    public interface IJwtTokenService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string tokenId);
        Task<string> CreateToken(UserEntity user);
    }
}
