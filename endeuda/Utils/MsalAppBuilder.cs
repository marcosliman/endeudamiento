using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Configuration;
namespace tesoreria.Utils
{
    public static class MsalAppBuilder
    {
        
        public static string LocalContext { get; } = ConfigurationManager.ConnectionStrings["ErpContext"].ConnectionString;
        public static string GetAccountId(this ClaimsPrincipal claimsPrincipal)
        {
            string oid = claimsPrincipal.GetObjectId();
            string tid = claimsPrincipal.GetTenantId();
            return $"{oid}.{tid}";
        }

        private static IConfidentialClientApplication clientapp;

        public static IConfidentialClientApplication BuildConfidentialClientApplication()
        {
            CacheOptions options= new CacheOptions();
            options.UseSharedCache=true;
            if (clientapp == null)
            {
                clientapp = ConfidentialClientApplicationBuilder.Create(AuthenticationConfig.ClientId)
                      .WithClientSecret(AuthenticationConfig.ClientSecret)
                      .WithRedirectUri(AuthenticationConfig.RedirectUri)
                      .WithAuthority(new Uri(AuthenticationConfig.Authority))
                      //.WithCacheSynchronization(true)
                      //.WithCacheOptions(options)
                      .Build();

                // After the ConfidentialClientApplication is created, we overwrite its default UserTokenCache serialization with our implementation
                //clientapp.AddInMemoryTokenCache();
                //clientapp.AddDistributedTokenCache(services =>
                //{
                //    services.AddDistributedMemoryCache();
                //    services.Configure<MsalDistributedTokenCacheAdapterOptions>(o =>
                //    {
                //        o.DisableL1Cache = false;
                //        o.Encrypt = false;

                //    });
                    
                //});

                // Could also use other forms of cache, like Redis
                // See https://aka.ms/ms-id-web/token-cache-serialization
                //clientapp.AddDistributedTokenCache(services =>
                //{
                //    services.AddStackExchangeRedisCache(options =>
                //    {
                //        options.Configuration = "localhost";
                //        options.InstanceName = "SampleInstance";
                //    });
                //});
                clientapp.AddDistributedTokenCache(services =>
                {
                    //services.AddDistributedSqlServerCache(config =>
                    //{
                    //    config.ConnectionString = LocalContext;
                    //    config.SchemaName = "dbo";
                    //    config.TableName = "TestCache";
                    //});
                });

            }

            return clientapp;
            //return clientapp;
        }

       
        public static async Task RemoveAccount()
        {
            BuildConfidentialClientApplication();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad != null)
            {
                var userAccount = await clientapp.GetAccountAsync(seguridad.AccountId);
                if (userAccount != null)
                {
                    await clientapp.RemoveAsync(userAccount);
                }

            }
            
        }
    }
}