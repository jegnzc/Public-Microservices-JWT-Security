using System.Runtime.CompilerServices;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Ocelot.Requester;

namespace Test.Services.Gateway.DelegatingHandlers
{
    public class TokenExchangeDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;

        public TokenExchangeDelegatingHandler(IHttpClientFactory httpClientFactory,
            IClientAccessTokenCache clientAccessTokenCache)
        {
            _httpClientFactory = httpClientFactory;
            _clientAccessTokenCache = clientAccessTokenCache;
        }

        public async Task<string> GetAccessToken(string incomingToken)
        {
            var test = _clientAccessTokenCache;
            var item = await _clientAccessTokenCache
                .GetAsync("gatewaytodownstreamtokenexchangeclient_test1", new ClientAccessTokenParameters(), CancellationToken.None);
            if (item != null)
            {
                return item.AccessToken;
            }

            var (accessToken, expiredIn) = await ExchangeToken(incomingToken);

            await _clientAccessTokenCache.SetAsync(
                "gatewaytodownstreamtokenexchangeclient_test1",
                accessToken,
                expiredIn,
                new ClientAccessTokenParameters(),
                CancellationToken.None
            );
            return accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // extract token
            var incomingToken = request.Headers.Authorization.Parameter;

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    await GetAccessToken(incomingToken));

            //// exchange token

            //var newToken = await ExchangeToken(incomingToken);

            //// replace old token

            //request.Headers.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<(string, int)> ExchangeToken(string? incomingToken)
        {
            var client = _httpClientFactory.CreateClient();

            var discoveryDocumentResponse = await client
                .GetDiscoveryDocumentAsync("https://localhost:5010/");
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var customParams = new Dictionary<string, string>
            {
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token"},
                { "subject_token", incomingToken},
                { "scope", "openid profile test1.fullaccess" }
            };

            var customP = new Parameters(customParams);
            var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                Parameters = customP,
                ClientId = "gatewaytodownstreamtokenexchangeclient",
                ClientSecret = "test.secret"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return (tokenResponse.AccessToken, tokenResponse.ExpiresIn);
        }
    }
}