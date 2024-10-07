using System.Security.Cryptography;
using System.Text;

namespace DataCloudDemo.Common
{
    public static class Utilities
    {
        public static string DATABASE_SCHEMA = Environment.GetEnvironmentVariable("DATABASE_SCHEMA");
        public static string DC_DMO_NAME = Environment.GetEnvironmentVariable("DC_DMO_NAME");
        public static string DC_CONNECTOR_NAME = Environment.GetEnvironmentVariable("DC_CONNECTOR_NAME");
        public static string WEBHOOK_SIGNED_KEY = Environment.GetEnvironmentVariable("WEBHOOK_SIGNED_KEY");

        public static string GetPGConnectionString(string pDatabaseUrl)
        {
            if (!string.IsNullOrEmpty(pDatabaseUrl))
            {
                string conStrParts = pDatabaseUrl.Replace("//", "");
                string[] strConn = conStrParts.Split(new char[] { '/', ':', '@', '?' });
                strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return string.Format("Host={0};Port={1};Database={2};User ID={3};Password={4};sslmode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=1;MaxPoolSize=20;", strConn[3], strConn[4], strConn[5], strConn[1], strConn[2]);
            }

            return string.Empty;
        }

        public static bool IsVerifySignature(string receivedSignature, string payload)
        {
            try
            {
                var hmac = new HMACSHA256();
                hmac.Key = Encoding.UTF8.GetBytes(WEBHOOK_SIGNED_KEY);
                var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
                if (signature.Equals(receivedSignature))
                {
                    Console.WriteLine("signature matched");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!", e.Message);
            }
            Console.WriteLine("signature not matched");
            return false;
        }
    }
}
