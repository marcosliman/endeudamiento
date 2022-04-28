using System.Configuration;
using System.Globalization;

namespace tesoreria.Utils
{
    public static class AuthenticationConfig
    {

        /*public static string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
        public static string appPassword = System.Configuration.ConfigurationManager.AppSettings["AppPass"];
        public static string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
        public static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];
        public static string[] scopes = System.Configuration.ConfigurationManager.AppSettings["AppScopes"]
          .Replace(' ', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        // public static string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);
        public static string authority = System.Configuration.ConfigurationManager.AppSettings["Authority"];
        */
        public const string IssuerClaim = "iss";
        public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string MicrosoftGraphGroupsApi = "https://graph.microsoft.com/v1.0/groups";
        public const string MicrosoftGraphUsersApi = "https://graph.microsoft.com/v1.0/users";
        public const string AdminConsentFormat = "https://login.microsoftonline.com/{0}/adminconsent?client_id={1}&state={2}&redirect_uri={3}";
        public const string BasicSignInScopes = "openid profile offline_access email";
        public const string NameClaimType = "name";

        /// <summary>
        /// The Client ID is used by the application to uniquely identify itself to Azure AD.
        /// </summary>
        public static string ClientId { get; } = ConfigurationManager.AppSettings["ClientId"];

        /// <summary>
        /// The ClientSecret is a credential used to authenticate the application to Azure AD.  Azure AD supports password and certificate credentials.
        /// </summary>
        public static string ClientSecret { get; } = ConfigurationManager.AppSettings["ClientSecret"];

        /// <summary>
        /// The Redirect Uri is the URL where the user will be redirected after they sign in.
        /// </summary>
        public static string RedirectUri { get; } = ConfigurationManager.AppSettings["RedirectUri"];

        public static string AADInstance { get; } = ConfigurationManager.AppSettings["AADInstance"];

        /// <summary>
        /// The authority
        /// </summary>
        public static string Authority = string.Format(CultureInfo.InvariantCulture, AADInstance, "common", "/v2.0");
    }
}