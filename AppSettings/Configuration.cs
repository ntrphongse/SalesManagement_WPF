using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

namespace AppSettings
{
    public static class Configuration
    {
        #region Private Members to get Configuration
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            return builder.Build();
        }
        #endregion

        public static string ConnectionString => GetConfiguration().GetSection("ConnectionStrings")["SalesManagement"];

        public static string DefaultAccount => JsonSerializer.Serialize(new
        {
            Email = GetConfiguration()["Account:DefaultAccount:Email"],
            Password = GetConfiguration()["Account:DefaultAccount:Password"]
        });
        public static string AppName => GetConfiguration().GetSection("AppName")["Default"];
    }
}
