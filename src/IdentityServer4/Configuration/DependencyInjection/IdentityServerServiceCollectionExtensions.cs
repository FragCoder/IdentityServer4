﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Configuration.DependencyInjection.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Configuration.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerBuilder(this IServiceCollection services)
        {
            return new IdentityServerBuilder(services);
        }

        public static IIdentityServerBuilder AddIdentityServer(this IServiceCollection services)
        {
            var builder = services.AddIdentityServerBuilder();

            builder.AddRequiredPlatformServices();
            
            builder.AddCoreServices();
            builder.AddDefaultEndpoints();
            builder.AddPluggableServices();
            builder.AddValidators();
            builder.AddResponseGenerators();
            
            builder.AddDefaultSecretParsers();
            builder.AddDefaultSecretValidators();

            return new IdentityServerBuilder(services);
        }

        public static IIdentityServerBuilder AddIdentityServer(this IServiceCollection services, Action<IdentityServerOptions> setupAction)
        {
            services.Configure(setupAction);
            return services.AddIdentityServer();
        }

        public static IIdentityServerBuilder AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityServerOptions>(configuration);
            return services.AddIdentityServer();
        }

        public static IIdentityServerBuilder AddDeveloperIdentityServer(this IServiceCollection services)
        {
            var builder = services.AddIdentityServer();
            builder.AddInMemoryStores();
            builder.SetTemporarySigningCredential();

            return builder;
        }

        public static IIdentityServerBuilder AddDeveloperIdentityServer(this IServiceCollection services, Action<IdentityServerOptions> setupAction)
        {
            services.Configure(setupAction);
            return services.AddDeveloperIdentityServer();
        }

        public static IIdentityServerBuilder AddDeveloperIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityServerOptions>(configuration);
            return services.AddDeveloperIdentityServer();
        }
    }
}