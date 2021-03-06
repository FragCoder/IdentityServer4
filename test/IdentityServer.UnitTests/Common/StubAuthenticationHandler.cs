﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace IdentityServer4.UnitTests.Common
{
    public class StubAuthenticationHandler : IAuthenticationHandler
    {
        public ClaimsPrincipal User { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public string Scheme { get; set; }

        public StubAuthenticationHandler(ClaimsPrincipal user, string scheme)
        {
            User = user;
            Scheme = scheme;
        }

        public Task AuthenticateAsync(AuthenticateContext context)
        {
            if (User == null)
            {
                context.NotAuthenticated();
            }
            else if (Scheme == null || Scheme == context.AuthenticationScheme)
            {
                context.Authenticated(User, Properties, new Dictionary<string, object>());
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(ChallengeContext context)
        {
            if (Scheme == null || context.AuthenticationScheme == Scheme)
            {
                context.Accept();
            }

            return Task.FromResult(0);
        }

        public void GetDescriptions(DescribeSchemesContext context)
        {
        }

        public Task SignInAsync(SignInContext context)
        {
            if (Scheme == null || context.AuthenticationScheme == Scheme)
            {
                context.Accept();
            }

            return Task.FromResult(0);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            if (Scheme == null || context.AuthenticationScheme == Scheme)
            {
                context.Accept();
            }

            return Task.FromResult(0);
        }
    }
}
