using Microsoft.Extensions.Configuration;
using System.IO;

namespace SmartVault.Core
{
    public record ConnectionStrings(string DefaultConnection);
    public record DataBaseConfig(string DatabaseFileName, ConnectionStrings ConnectionStrings)
    {
        public string FormatedConnectionString()
        {
            return string.Format(this?.ConnectionStrings?.DefaultConnection ?? "", this?.DatabaseFileName);
        }
    }

    public class SmartVaultConfigurationManager
    {
        public static DataBaseConfig GetDBConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            DataBaseConfig c = new DataBaseConfig("", null);
            config.Bind(c);
            return c;
        }
    }


}