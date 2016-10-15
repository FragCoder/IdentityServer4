// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation.Contexts;
using IdentityServer4.Validation.Interfaces;
using IdentityServer4.Validation.Models;

namespace IdentityServer4.IntegrationTests.Clients.Setup
{
    public class ExtensionGrantValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var credential = context.Request.Raw.Get("custom_credential");

            if (credential != null)
            {
                // valid credential
                context.Result = new GrantValidationResult("818727", "custom");
            }
            else
            {
                // custom error message
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_custom_credential");
            }

            return Task.FromResult(0);
        }

        public string GrantType
        {
            get { return "custom"; }
        }
    }
}