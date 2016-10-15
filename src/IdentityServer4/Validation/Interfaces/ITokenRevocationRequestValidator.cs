// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation.Models;

namespace IdentityServer4.Validation.Interfaces
{
    public interface ITokenRevocationRequestValidator
    {
        Task<TokenRevocationRequestValidationResult> ValidateRequestAsync(NameValueCollection parameters, Client client);
    }
}