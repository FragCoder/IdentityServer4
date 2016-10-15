// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer4.Stores
{
    public interface ISigningCredentialStore
    {
        Task<SigningCredentials> GetSigningCredentialsAsync();
    }
}