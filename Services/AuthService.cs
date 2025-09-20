using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
namespace MusicHFE2.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient; 
        private readonly IJSRuntime _jsRuntime; 
        private readonly NavigationManager _navigation; 
        private readonly StateService _stateService; 
        private const string TokenKey = "authToken";
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigation, StateService stateService)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _navigation = navigation;
            _stateService = stateService;
        }

        public async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        }

        public async Task ClearTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _stateService.SetUser(null);
        }

        public async Task<bool> IsTokenValidAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var exp = jwtToken.Payload.Expiration;
                if (exp == null)
                {
                    return false;
                }

                var expiration = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
                return DateTime.UtcNow < expiration;
            }
            catch
            {
                return false;
            }
        }

        public async Task EnsureAuthenticatedAsync()
        {
            if (!await IsTokenValidAsync())
            {
                await ClearTokenAsync();
                _navigation.NavigateTo("/login", true);
            }
        }
        public async Task<User> GetUserfromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var user = new User
            {
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Type = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            };
            return user;
        }
        
    }
}
