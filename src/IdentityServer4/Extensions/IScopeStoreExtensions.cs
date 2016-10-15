// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Extensions
{
    public static class IScopeStoreExtensions
    {
        public static async Task<IEnumerable<Scope>> FindEnabledScopesAsync(this IScopeStore store, IEnumerable<string> scopeNames)
        {
            var scopes = await store.FindScopesAsync(scopeNames);
            return scopes.Where(s => s.Enabled).ToArray();
        }

       
        public static async Task<IEnumerable<Scope>> GetEnabledScopesAsync(this IScopeStore store, bool publicOnly = true)
        {
            var scopes = await store.GetScopesAsync(publicOnly);
            return scopes.Where(s => s.Enabled).ToArray();
        }
    }
}