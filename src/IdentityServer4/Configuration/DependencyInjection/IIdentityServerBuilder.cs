// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Configuration.DependencyInjection
{
    public interface IIdentityServerBuilder
    {
        IServiceCollection Services { get; }
    }
}