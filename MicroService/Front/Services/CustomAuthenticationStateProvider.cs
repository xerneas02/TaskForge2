﻿using Front.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace Front.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private ProtectedLocalStorage _sessionStorage;

        public CustomAuthenticationStateProvider(ProtectedLocalStorage protectedSessionStorage)
        {
            _sessionStorage = protectedSessionStorage;
        }

        public async Task<ClaimsPrincipal> MarkUserAsAuthenticated(UserDTO user, string JWT)
        {
            await _sessionStorage.SetAsync("User", user);
            await _sessionStorage.SetAsync("JWT", JWT);


            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return _currentUser;
        }

        public async Task<string> GetJWT()
        {
            var token = await _sessionStorage.GetAsync<string>("JWT");
            if (token.Success && token.Value != null)
            {
                return token.Value;
            }
            return "";
        }

        public async Task<ClaimsPrincipal> Logout()
        {
            await _sessionStorage.DeleteAsync("User");
            await _sessionStorage.DeleteAsync("JWT");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return _currentUser;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            if(userSession.Success && userSession.Value != null)
            {
                var user = userSession.Value;
                var claims = new[] {
                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "User")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                _currentUser = new ClaimsPrincipal(identity);
            } else {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            return await Task.FromResult(new AuthenticationState(_currentUser));
        }


    }
}
