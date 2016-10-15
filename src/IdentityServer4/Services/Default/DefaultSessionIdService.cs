﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace IdentityServer4.Services.Default
{
    public class DefaultSessionIdService : ISessionIdService
    {
        private readonly HttpContext _context;

        public DefaultSessionIdService(IHttpContextAccessor context)
        {
            _context = context.HttpContext;
        }

        public void CreateSessionId(SignInContext context)
        {
            var sid = CryptoRandom.CreateUniqueId();
            context.Properties[OidcConstants.EndSessionRequest.Sid] = sid;
            IssueSessionIdCookie(sid);
        }

        public async Task<string> GetCurrentSessionIdAsync()
        {
            var info = await _context.GetIdentityServerUserInfoAsync();
            if (info.Properties.Items.ContainsKey(OidcConstants.EndSessionRequest.Sid))
            {
                var sid = info.Properties.Items[OidcConstants.EndSessionRequest.Sid];
                return sid;
            }
            return null;
        }

        public async Task EnsureSessionCookieAsync()
        {
            var sid = await GetCurrentSessionIdAsync();
            if (sid != null)
            {
                IssueSessionIdCookie(sid);
            }
        }

        public string GetCookieName()
        {
            // TODO: fix from config?
            return "idsvr.session";
        }

        public string GetCookieValue()
        {
            return _context.Request.Cookies[GetCookieName()];
        }

        public void RemoveCookie()
        {
            var name = GetCookieName();
            if (_context.Request.Cookies.ContainsKey(name))
            {
                // only remove it if we have it in the request
                var options = CreateCookieOptions();
                options.Expires = DateTimeHelper.UtcNow.AddYears(-1);

                _context.Response.Cookies.Append(name, ".", options);
            }
        }

        void IssueSessionIdCookie(string sid)
        {
            if (GetCookieValue() != sid)
            {
                _context.Response.Cookies.Append(
                    GetCookieName(),
                    sid,
                    CreateCookieOptions());
            }
        }

        CookieOptions CreateCookieOptions()
        {
            var secure = _context.Request.IsHttps;
            var path = _context.GetBasePath().CleanUrlPath();

            var options = new CookieOptions
            {
                HttpOnly = false,
                Secure = secure,
                Path = path
            };

            return options;
        }
    }
}