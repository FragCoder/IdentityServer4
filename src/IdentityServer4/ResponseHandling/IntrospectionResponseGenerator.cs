﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling.Interfaces;
using IdentityServer4.Validation.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.ResponseHandling
{
    public class IntrospectionResponseGenerator : IIntrospectionResponseGenerator
    {
        private readonly ILogger _logger;

        public IntrospectionResponseGenerator(ILogger<IntrospectionResponseGenerator> logger)
        {
            _logger = logger;
        }

        public Task<Dictionary<string, object>> ProcessAsync(IntrospectionRequestValidationResult validationResult, Scope scope)
        {
            _logger.LogTrace("Creating introspection response");

            var response = new Dictionary<string, object>();
            
            if (validationResult.IsActive == false)
            {
                _logger.LogDebug("Creating introspection response for inactive token.");

                response.Add("active", false);
                return Task.FromResult(response);
            }

            if (scope.AllowUnrestrictedIntrospection)
            {
                _logger.LogDebug("Creating unrestricted introspection response for active token.");

                response = validationResult.Claims.ToClaimsDictionary();
                response.Add("active", true);
            }
            else
            {
                _logger.LogDebug("Creating restricted introspection response for active token.");

                response = validationResult.Claims.Where(c => c.Type != JwtClaimTypes.Scope).ToClaimsDictionary();
                response.Add("active", true);
                response.Add("scope", new[] { scope.Name });
            }

            return Task.FromResult(response);
        }
    }
}