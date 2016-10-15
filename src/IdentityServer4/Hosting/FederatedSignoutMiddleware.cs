﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration.DependencyInjection.Options;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Hosting
{
    public class FederatedSignOutMiddleware
    {
        const string DocumentHtml = "<!DOCTYPE html><html><body>{0}</body></html>";
        const string IframeHtml = "<iframe style='display:none' width='0' height='0' src='{0}'></iframe>";

        private readonly RequestDelegate _next;
        private readonly IdentityServerOptions _options;
        private readonly ILogger<FederatedSignOutMiddleware> _logger;

        public FederatedSignOutMiddleware(RequestDelegate next, IdentityServerOptions options, ILogger<FederatedSignOutMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 200 && 
                _options.AuthenticationOptions.FederatedSignOutPaths.Any(x=>x == context.Request.Path))
            {
                await ProcessResponseAsync(context);
            }
        }

        private async Task ProcessResponseAsync(HttpContext context)
        {
            _logger.LogDebug("Federated signout path requested");

            var user = await context.GetIdentityServerUserAsync();
            if (user != null)
            {
                var sid = user.FindFirst(OidcConstants.EndSessionRequest.Sid)?.Value;
                if (sid != null)
                {
                    var sidParam = await GetSidRequestParamAsync(context.Request);
                    if (TimeConstantComparer.IsEqual(sid, sidParam))
                    {
                        _logger.LogDebug("sid parameter matches current sid");

                        var iframeUrl = await context.GetIdentityServerSignoutFrameCallbackUrlAsync();
                        if (iframeUrl != null)
                        {
                            _logger.LogDebug("Rendering signout callback iframe");
                            await RenderResponseAsync(context, iframeUrl, sid);
                        }
                        else
                        {
                            _logger.LogDebug("No signout callback iframe to render");
                        }
                    }
                }
                else
                {
                    _logger.LogDebug("no sid param passed");
                }
            }
            else
            {
                _logger.LogDebug("no authenticated user");
            }
        }

        private async Task<string> GetSidRequestParamAsync(HttpRequest request)
        {
            if (String.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return request.Query[OidcConstants.EndSessionRequest.Sid].FirstOrDefault();
            }
            if (String.Equals(request.Method, "POST", StringComparison.OrdinalIgnoreCase) && 
                !String.IsNullOrEmpty(request.ContentType) &&
                request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) && 
                request.Body.CanRead)
            {
                var form = await request.ReadFormAsync();
                return form[OidcConstants.EndSessionRequest.Sid].FirstOrDefault();
            }

            return null;
        }

        private async Task RenderResponseAsync(HttpContext context, string iframeUrl, string sid)
        {
            await context.Authentication.SignOutAsync(_options.AuthenticationOptions.EffectiveAuthenticationScheme);

            if (context.Response.Body.CanWrite)
            {
                var iframe = String.Format(IframeHtml, iframeUrl);
                var doc = String.Format(DocumentHtml, iframe);
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(doc);
            }
        }
    }
}