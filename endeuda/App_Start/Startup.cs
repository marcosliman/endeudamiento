using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using gestor.Helpers;
using gestor.TokenStorage;
using System.Security.Claims;

using Microsoft.Owin.Host.SystemWeb;

using tesoreria.Utils;

namespace tesoreria
{
    public partial class Startup
    {
        const string scopesToRequest = "user.read";

        private static string appId = ConfigurationManager.AppSettings["ClientId"];
        private static string appSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["AppScopes"];
        private static string UrlAuthority = ConfigurationManager.AppSettings["Authority"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = appId,
                    Authority = UrlAuthority,
                    Scope = $"openid email profile offline_access {graphScopes}",
                    RedirectUri = redirectUri,
                    PostLogoutRedirectUri = redirectUri,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        // For demo purposes only, see below
                        ValidateIssuer = false

                        // In a real multi-tenant app, you would add logic to determine whether the
                        // issuer was from an authorized tenant
                        //ValidateIssuer = true,
                        //IssuerValidator = (issuer, token, tvp) =>
                        //{
                        //  if (MyCustomTenantValidation(issuer))
                        //  {
                        //    return issuer;
                        //  }
                        //  else
                        //  {
                        //    throw new SecurityTokenInvalidIssuerException("Invalid issuer");
                        //  }
                        //}
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailedAsync,
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync
                    },
                    // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                    CookieManager = new SameSiteCookieManager(new SystemWebCookieManager())

                }
            );
        }

        private static Task OnAuthenticationFailedAsync(AuthenticationFailedNotification<OpenIdConnectMessage,
            OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            var owinContext = notification.OwinContext;
            string redirect = $"/Home/Error?message={notification.Exception.Message}";
            if (notification.ProtocolMessage != null && !string.IsNullOrEmpty(notification.ProtocolMessage.ErrorDescription))
            {
                redirect += $"&debug={notification.ProtocolMessage.ErrorDescription}";
            }
            notification.Response.Redirect(redirect);
            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                .WithRedirectUri(redirectUri)
                .WithClientSecret(appSecret)
                //.WithCacheSynchronization(true)                
                .Build();

            var signedInUser = new ClaimsPrincipal(notification.AuthenticationTicket.Identity);
            var tokenStore = new SessionTokenStore(idClient.UserTokenCache, HttpContext.Current, signedInUser);
            //System.Web.HttpContext.Current.Session["notification"] = notification.OwinContext;            
            //notification.OwinContext.Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            try
            {
                string[] scopes = graphScopes.Split(' ');

                var result = await idClient.AcquireTokenByAuthorizationCode(
                    scopes, notification.Code).ExecuteAsync();

                var userDetails = await GraphHelper.GetUserDetailsAsync(result.AccessToken);

                tokenStore.SaveUserDetails(userDetails);
                notification.HandleCodeRedemption(null, result.IdToken);
                System.Web.HttpContext.Current.Session["AccountId"] = result.IdToken;

            }
            catch (MsalException ex)
            {
                string message = "AcquireTokenByAuthorizationCodeAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                string message = "GetUserDetailsAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
        }

    }
}