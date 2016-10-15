// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation.Models;

namespace IdentityServer4.Validation.Interfaces
{
    internal interface IEndSessionRequestValidator
    {
        Task<EndSessionValidationResult> ValidateAsync(NameValueCollection parameters, ClaimsPrincipal subject);
        Task<EndSessionCallbackValidationResult> ValidateCallbackAsync(NameValueCollection parameters);
    }
}
