﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer4.IntegrationTests.Endpoints.Introspection.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IdentityServer4.IntegrationTests.Endpoints.Introspection
{
    public class IntrospectionTests
    {
        const string Category = "Introspection endpoint";
        const string IntrospectionEndpoint = "https://server/connect/introspect";
        const string TokenEndpoint = "https://server/connect/token";

        private readonly HttpClient _client;
        private readonly HttpMessageHandler _handler;

        public IntrospectionTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _handler = server.CreateHandler();
            _client = server.CreateClient();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Request()
        {
            var form = new Dictionary<string, string>();

            var response = await _client.PostAsync(IntrospectionEndpoint, new FormUrlEncodedContent(form));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_Scope()
        {
            var form = new Dictionary<string, string>();

            _client.SetBasicAuthentication("unknown", "invalid");
            var response = await _client.PostAsync(IntrospectionEndpoint, new FormUrlEncodedContent(form));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ScopeSecret()
        {
            var form = new Dictionary<string, string>();

            _client.SetBasicAuthentication("api1", "invalid");
            var response = await _client.PostAsync(IntrospectionEndpoint, new FormUrlEncodedContent(form));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Token()
        {
            var form = new Dictionary<string, string>();

            _client.SetBasicAuthentication("api1", "secret");
            var response = await _client.PostAsync(IntrospectionEndpoint, new FormUrlEncodedContent(form));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Token()
        {
            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "api1",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = "invalid"
            });

            response.IsActive.Should().Be(false);
            response.IsError.Should().Be(false);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_Valid_Scope()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "api1",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            response.IsActive.Should().Be(true);
            response.IsError.Should().Be(false);

            var scopes = from c in response.Claims
                         where c.Type == "scope"
                         select c;

            scopes.Count().Should().Be(1);
            scopes.First().Value.Should().Be("api1");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Response_data_should_be_valid_using_single_scope()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "api1",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            var values = response.Json.ToObject<Dictionary<string, object>>();

            values["aud"].GetType().Name.Should().Be("String");
            var iss = values["iss"].GetType().Name.Should().Be("String"); ;
            var nbf = values["nbf"].GetType().Name.Should().Be("Int64"); ;
            var exp = values["exp"].GetType().Name.Should().Be("Int64"); ;
            var clientId = values["client_id"].GetType().Name.Should().Be("String"); ;
            var active = values["active"].GetType().Name.Should().Be("Boolean"); ;
            var scopes = values["scope"] as JArray;

            scopes.Count.Should().Be(1);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Response_data_should_be_valid_using_multiple_scopes()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1 api2 unrestricted.api");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "unrestricted.api",
                "secret",
                _handler);


            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            var values = response.Json.ToObject<Dictionary<string, object>>();

            values["aud"].GetType().Name.Should().Be("String");
            var iss = values["iss"].GetType().Name.Should().Be("String"); ;
            var nbf = values["nbf"].GetType().Name.Should().Be("Int64"); ;
            var exp = values["exp"].GetType().Name.Should().Be("Int64"); ;
            var clientId = values["client_id"].GetType().Name.Should().Be("String"); ;
            var active = values["active"].GetType().Name.Should().Be("Boolean"); ;
            var scopes = values["scope"] as JArray;

            scopes.Count.Should().Be(3);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_Valid_Unrestricted_Scope()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1 api2 unrestricted.api");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "unrestricted.api",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            response.IsActive.Should().Be(true);
            response.IsError.Should().Be(false);

            var scopes = from c in response.Claims
                         where c.Type == "scope"
                         select c;

            scopes.Count().Should().Be(3);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_Valid_Scope_Multiple()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1 api2");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "api1",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            response.IsActive.Should().Be(true);
            response.IsError.Should().Be(false);

            var scopes = from c in response.Claims
                         where c.Type == "scope"
                         select c;

            scopes.Count().Should().Be(1);
            scopes.First().Value.Should().Be("api1");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_Invalid_Scope()
        {
            var tokenClient = new TokenClient(
                TokenEndpoint,
                "client1",
                "secret",
                _handler);

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            var introspectionClient = new IntrospectionClient(
                IntrospectionEndpoint,
                "api2",
                "secret",
                _handler);

            var response = await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = tokenResponse.AccessToken
            });

            response.IsActive.Should().Be(false);
            response.IsError.Should().Be(false);
        }
    }
}
