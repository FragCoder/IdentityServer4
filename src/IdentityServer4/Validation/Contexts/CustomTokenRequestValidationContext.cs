// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Validation.Models;

namespace IdentityServer4.Validation.Contexts
{
    /// <summary>
    /// Context class for custom token request validation
    /// </summary>
    public class CustomTokenRequestValidationContext
    {
        public TokenRequestValidationResult Result { get; set; }
    }
}