using Carter;
using Microsoft.Extensions.Options;

namespace FacebookAPIExample.Backend.Integrations.Facebook;

public class ConnectRequest
{
    public string Code { get; set; }
}

public class Connect : ICarterModule
{
    private readonly FacebookOptions _facebookConfig;

    public Connect(IOptions<FacebookOptions> facebookConfig)
    {
        _facebookConfig = facebookConfig.Value;
    }
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("integrations/facebook/authorize", async (ConnectRequest request) =>
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://graph.facebook.com/v15.0");

            var appId = _facebookConfig.AppId;
            var appSecret = _facebookConfig.ClientSecret;
            
            // must be the same as the one we redirect to during the auth flow on the client side
            var redirectUrl = "https://localhost:7229/Authorization";
            
            var result = await client.GetAsync($@"oauth/access_token?client_id={appId}&redirect_uri={redirectUrl}&client_secret={appSecret}&code={request.Code}");

            var accessTokenResponse = await result.Content.ReadFromJsonAsync<FacebookAccessTokenResponse>();
            Console.WriteLine(accessTokenResponse.AccessToken);
    
            // store the access token in the DB, against the user
            // probably worth storing the expiry date too, so you can warn a user if their connection needs to be re-authenticated
            var expiresInSeconds = accessTokenResponse.ExpiresIn;
            Console.WriteLine("Expires in (seconds) " + expiresInSeconds);

            // you can now use the access token for future requests (on behalf of that user) to interact with the Facebook API

        });
    }
}