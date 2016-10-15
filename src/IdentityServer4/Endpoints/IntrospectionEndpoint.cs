﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling.Interfaces;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServer4.Validation.Interfaces;
using IdentityServer4.Validation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Endpoints
{
    public class IntrospectionEndpoint : IEndpoint
    {
        private readonly IEventService _events;
        private readonly IIntrospectionResponseGenerator _generator;
        private readonly ILogger<IntrospectionEndpoint> _logger;
        private readonly IIntrospectionRequestValidator _requestValidator;
        private readonly ScopeSecretValidator _scopeSecretValidator;

        public IntrospectionEndpoint(ScopeSecretValidator scopeSecretValidator, IIntrospectionRequestValidator requestValidator, IIntrospectionResponseGenerator generator, IEventService events, ILogger<IntrospectionEndpoint> logger)
        {
            _scopeSecretValidator = scopeSecretValidator;
            _requestValidator = requestValidator;
            _generator = generator;
            _events = events;
            _logger = logger;
        }

        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogTrace("Processing introspection request.");

            // validate HTTP
            if (context.Request.Method != "POST")
            {
                _logger.LogWarning("Introspection endpoint only supports POST requests");
                return new StatusCodeResult(405);
            }

            return await ProcessIntrospectionRequestAsync(context);
        }

        private async Task<IEndpointResult> ProcessIntrospectionRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Starting introspection request.");

            var scopeResult = await _scopeSecretValidator.ValidateAsync(context);
            if (scopeResult.Scope == null)
            {
                _logger.LogError("Scope unauthorized to call introspection endpoint. aborting.");
                return new StatusCodeResult(401);
            }

            var parameters = context.Request.Form.AsNameValueCollection();

            var validationResult = await _requestValidator.ValidateAsync(parameters, scopeResult.Scope);
            var response = await _generator.ProcessAsync(validationResult, scopeResult.Scope);

            if (validationResult.IsActive)
            {
                await RaiseSuccessEventAsync(validationResult.Token, "active", scopeResult.Scope.Name);
                return new IntrospectionResult(response);
            }

            if (validationResult.IsError)
            {
                if (validationResult.FailureReason == IntrospectionRequestValidationFailureReason.MissingToken)
                {
                    await RaiseFailureEventAsync(validationResult.ErrorDescription, validationResult.Token, scopeResult.Scope.Name);
                    //todo return BadRequest("missing_token");
                    return new StatusCodeResult(400);
                }

                if (validationResult.FailureReason == IntrospectionRequestValidationFailureReason.InvalidToken)
                {
                    await RaiseSuccessEventAsync(validationResult.Token, "inactive", scopeResult.Scope.Name);
                    return new IntrospectionResult(response);
                }

                if (validationResult.FailureReason == IntrospectionRequestValidationFailureReason.InvalidScope)
                {
                    await RaiseFailureEventAsync("Scope not authorized to introspect token", validationResult.Token, scopeResult.Scope.Name);
                    return new IntrospectionResult(response);
                }
            }

            _logger.LogError("Invalid token introspection outcome");
            throw new InvalidOperationException("Invalid token introspection outcome");
        }

        private async Task RaiseSuccessEventAsync(string token, string tokenStatus, string scopeName)
        {
            _logger.LogInformation("Success token introspection. Token status: {tokenStatus}, for scope name: {scopeName}", tokenStatus, scopeName);

            await _events.RaiseSuccessfulIntrospectionEndpointEventAsync(
                token,
                tokenStatus,
                scopeName);
        }

        private async Task RaiseFailureEventAsync(string error, string token, string scopeName)
        {
            _logger.LogError("Failed token introspection: {error}, for scope name: {scopeName}", error, scopeName);

            await _events.RaiseFailureIntrospectionEndpointEventAsync(
                error, token, scopeName);
        }
    }
}