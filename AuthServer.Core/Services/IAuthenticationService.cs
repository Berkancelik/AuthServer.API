using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        // Revoke etkinliği kırmak anlamını taşımaktadır.
        //RevokeRefreshToken ele geçirildiği zaman ilgili kullanıcının datasını null a çevirir
        //yani refresh token da veri çalınmasını önlemek için revokerefreshtoken kullanılmaktadır
        
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);

        Response <ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);

    }
}
