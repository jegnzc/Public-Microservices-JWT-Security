using System.Runtime.CompilerServices;
using IdentityModel.Client;
using Ocelot.Requester;

namespace Test.Services.Gateway.DelegatingHandlers
{
    public class TokenExchangeDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenExchangeDelegatingHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // extract token
            var incomingToken = request.Headers.Authorization.Parameter;

            // exchange token

            var newToken = await ExchangeToken(incomingToken);

            // replace old token

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> ExchangeToken(string? incomingToken)
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

            return tokenResponse.AccessToken;
        }
    }
}