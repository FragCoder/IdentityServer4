﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    public interface IClientSessionService
    {
        Task AddClientIdAsync(string clientId);
        Task<IEnumerable<string>> GetClientListAsync();

        Task EnsureClientListCookieAsync();
        IEnumerable<string> GetClientListFromCookie();
        void RemoveCookie();
    }
}
