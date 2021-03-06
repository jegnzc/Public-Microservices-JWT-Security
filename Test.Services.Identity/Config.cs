// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;

namespace Test.Services.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("test1", "Test 1 API")
                {
                    Scopes = { "test1.fullaccess" }
                },
                new ApiResource("test2", "Test 2 API")
                {
                    Scopes = { "test2.fullaccess" }
                }
                ,
                new ApiResource("testcore", "Test Core API")
                {
                    Scopes = { "testcore.fullaccess" }
                }
                ,
                new ApiResource("testgateway", "Test API Gateway")
                {
                    Scopes = { "testgateway.fullaccess" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("test1.fullaccess"),
                new ApiScope("test2.fullaccess"),
                new ApiScope("test1.read"),
                new ApiScope("test1.write"),
                new ApiScope("testcore.fullaccess"),
                new ApiScope("testgateway.fullaccess"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "M2M Client",
                    ClientId = "test.m2m.clientId",
                    ClientSecrets = { new Secret("test.secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "test1.fullaccess" }
                },
                new Client
                {
                    ClientName = "Test MVC Client",
                    ClientId = "test.clientIdMvc",
                    ClientSecrets = { new Secret("test.secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "test2.fullaccess" }
                },
                new Client
                {
                    ClientName = "Test Client",
                    ClientId = "test",
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60,
                    ClientSecrets = { new Secret("test.secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile",
                        "testgateway.fullaccess",
                        "test2.fullaccess"}
                },
                new Client
                {
                    ClientId = "test2todownstreamtokenexchangeclient",
                    ClientName = "Test 2 Core Token Exchange Client",
                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:token-exchange" },
                    ClientSecrets = { new Secret("test.secret".Sha256())},
                    AllowedScopes = {
                        "openid", "profile", "testcore.fullaccess" }
                }
                ,
                new Client
                {
                    ClientId = "gatewaytodownstreamtokenexchangeclient",
                    ClientName = "ApiGateway Token Exchange",
                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:token-exchange" },
                    ClientSecrets = { new Secret("test.secret".Sha256())},
                    AllowedScopes = {
                        "openid", "profile", "test1.fullaccess" }
                },
            };
    }
}