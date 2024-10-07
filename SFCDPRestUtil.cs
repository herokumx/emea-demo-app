using DataCloudDemo.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;

namespace DataCloudDemo.Services
{
    public class SFCDPRestUtil : IDisposable
    {
        public string cdpAuthToken;

        private string clientId = string.Empty;

        private string clientSecret = string.Empty;

        private string userId = string.Empty;

        private string password = string.Empty;

        private string sfLoginUrl = "https://login.salesforce.com";

        private string cdpRestEndpointUrl = string.Empty;

        private string sfEndpointUrl = string.Empty;

        private DateTime cdpAuthTokenExpiration = DateTime.Now;

        private RestClient _client = null;

        public SFCDPRestUtil(CDPApiConfig sfCDPConfig)
        {
            if (sfCDPConfig != null)
            {
                if (!string.IsNullOrEmpty(sfCDPConfig.sfEndpointUrl))
                {
                    sfEndpointUrl = sfCDPConfig.sfEndpointUrl;
                }
                if (!string.IsNullOrEmpty(sfCDPConfig.clientId))
                {
                    clientId = sfCDPConfig.clientId;
                }

                if (!string.IsNullOrEmpty(sfCDPConfig.clientSecret))
                {
                    clientSecret = sfCDPConfig.clientSecret;
                }

                if (!string.IsNullOrEmpty(sfCDPConfig.userId))
                {
                    userId = sfCDPConfig.userId;
                }

                if (!string.IsNullOrEmpty(sfCDPConfig.password))
                {
                    password = sfCDPConfig.password;
                }
            }

            //if (sfEndpointUrl.Equals(string.Empty))
            //{
            //    throw new Exception("Salesforce CDP Endpoint is null: Must be passed when instantiating SFCDPRestUtil");
            //}
            if (clientId.Equals(string.Empty) || clientSecret.Equals(string.Empty))
            {
                throw new Exception("clientId or clientSecret is null: Must be passed when instantiating SFCDPRestUtil");
            }
            if (userId.Equals(string.Empty) || password.Equals(string.Empty))
            {
                throw new Exception("Salesforce user login Id or password is null: Must be passed when instantiating SFCDPRestUtil");
            }
            //Create RequestFactory instance
            //restFactory = new RequestFactory();

            //Make salesforce auth call to get auth token
            //getSalesforceAuthToken().Wait();
        }

        private async Task getSalesforceAuthToken(bool force = false)
        {
            // RefreshToken
            if (!string.IsNullOrEmpty(cdpAuthToken) && DateTime.Now.AddSeconds(300) <= cdpAuthTokenExpiration && !force)
                return;
            try
            {
                RestResponse oAuthhttpResponse = await getoauthToken();
                if (oAuthhttpResponse != null)
                {
                    if (oAuthhttpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Parse the response
                        var parsedResponse = JObject.Parse(oAuthhttpResponse.Content);
                        var lAuthToken = parsedResponse["access_token"].Value<string>().Trim();
                        var lAuthTokenExpiration = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parsedResponse["issued_at"].Value<string>().Trim())).AddMinutes(15).LocalDateTime;
                        //sfLoginUrl = parsedResponse["id"].Value<string>().Trim().TrimEnd('/');
                        sfEndpointUrl = parsedResponse["instance_url"].Value<string>().Trim();

                        ;
                        //request.Timeout = Constants.RESTCLIENT_TIMEOUT;
                        //request.ReadWriteTimeout = Constants.RESTCLIENT_READWRITETIMEOUT;
                        RestRequest request = null;
                        request = new RestRequest($"{sfEndpointUrl.Trim()}/services/a360/token", Method.Post);

                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddParameter("grant_type", "urn:salesforce:grant-type:external:cdp");
                        request.AddParameter("subject_token_type", "urn:ietf:params:oauth:token-type:access_token");
                        request.AddParameter("subject_token", lAuthToken);

                        createRestClientInstance(120);

                        ServicePointManager.Expect100Continue = false;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        RestResponse httpResponseAccessToken = await _client.ExecuteAsync(request);
                        if (httpResponseAccessToken != null)
                        {
                            if (httpResponseAccessToken.ContentType != "application/json")
                            {
                                //retry1
                                Console.WriteLine("Get CDP Access Token Content Type is not application/json so trying after 30000 ms");
                                await Task.Delay(30000);
                                oAuthhttpResponse = await getoauthToken();
                                if (oAuthhttpResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    // Parse the response
                                    parsedResponse = JObject.Parse(oAuthhttpResponse.Content);
                                    lAuthToken = parsedResponse["access_token"].Value<string>().Trim();
                                    lAuthTokenExpiration = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parsedResponse["issued_at"].Value<string>().Trim())).AddMinutes(15).LocalDateTime;
                                    //sfLoginUrl = parsedResponse["id"].Value<string>().Trim().TrimEnd('/');
                                    sfEndpointUrl = parsedResponse["instance_url"].Value<string>().Trim();

                                    request = new RestRequest($"{sfEndpointUrl.Trim()}/services/a360/token", Method.Post);

                                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                    request.AddParameter("grant_type", "urn:salesforce:grant-type:external:cdp");
                                    request.AddParameter("subject_token_type", "urn:ietf:params:oauth:token-type:access_token");
                                    request.AddParameter("subject_token", lAuthToken);


                                    //client = new RestClient(clientOptions);
                                    httpResponseAccessToken = await _client.ExecuteAsync(request);
                                    if (httpResponseAccessToken.ContentType != "application/json")
                                    {
                                        //retry2
                                        Console.WriteLine("Get CDP Access Token Content Type is not application/json so trying after 60000 ms");
                                        await Task.Delay(60000);
                                        oAuthhttpResponse = await getoauthToken();
                                        if (oAuthhttpResponse.StatusCode == HttpStatusCode.OK)
                                        {
                                            // Parse the response
                                            parsedResponse = JObject.Parse(oAuthhttpResponse.Content);
                                            lAuthToken = parsedResponse["access_token"].Value<string>().Trim();
                                            lAuthTokenExpiration = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parsedResponse["issued_at"].Value<string>().Trim())).AddMinutes(15).LocalDateTime;
                                            //sfLoginUrl = parsedResponse["id"].Value<string>().Trim().TrimEnd('/');
                                            sfEndpointUrl = parsedResponse["instance_url"].Value<string>().Trim();

                                            request = new RestRequest($"{sfEndpointUrl.Trim()}/services/a360/token", Method.Post);

                                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                            request.AddParameter("grant_type", "urn:salesforce:grant-type:external:cdp");
                                            request.AddParameter("subject_token_type", "urn:ietf:params:oauth:token-type:access_token");
                                            request.AddParameter("subject_token", lAuthToken);

                                            //client = new RestClient(clientOptions);
                                            httpResponseAccessToken = await _client.ExecuteAsync(request);
                                            if (httpResponseAccessToken.ContentType != "application/json")
                                            {
                                                //retry3
                                                Console.WriteLine("Get CDP Access Token Content Type is not application/json so trying after 90000 ms");
                                                await Task.Delay(90000);
                                                oAuthhttpResponse = await getoauthToken();
                                                if (oAuthhttpResponse.StatusCode == HttpStatusCode.OK)
                                                {
                                                    // Parse the response
                                                    parsedResponse = JObject.Parse(oAuthhttpResponse.Content);
                                                    lAuthToken = parsedResponse["access_token"].Value<string>().Trim();
                                                    lAuthTokenExpiration = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parsedResponse["issued_at"].Value<string>().Trim())).AddMinutes(15).LocalDateTime;
                                                    //sfLoginUrl = parsedResponse["id"].Value<string>().Trim().TrimEnd('/');
                                                    sfEndpointUrl = parsedResponse["instance_url"].Value<string>().Trim();

                                                    request = new RestRequest($"{sfEndpointUrl.Trim()}/services/a360/token", Method.Post);

                                                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                                    request.AddParameter("grant_type", "urn:salesforce:grant-type:external:cdp");
                                                    request.AddParameter("subject_token_type", "urn:ietf:params:oauth:token-type:access_token");
                                                    request.AddParameter("subject_token", lAuthToken);


                                                    //client = new RestClient(clientOptions);
                                                    httpResponseAccessToken = await _client.ExecuteAsync(request);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (httpResponseAccessToken.StatusCode == HttpStatusCode.OK)
                            {
                                // Parse the response
                                parsedResponse = JObject.Parse(httpResponseAccessToken.Content);
                                cdpAuthToken = parsedResponse["access_token"].Value<string>().Trim();
                                cdpRestEndpointUrl = parsedResponse["instance_url"].Value<string>().Trim().TrimEnd('/');
                                if (!string.IsNullOrEmpty(cdpRestEndpointUrl))
                                {
                                    cdpRestEndpointUrl = $"https://{cdpRestEndpointUrl}";
                                }
                                cdpAuthTokenExpiration = DateTime.Now.AddMilliseconds(int.Parse(parsedResponse["expires_in"].Value<string>().Trim()));
                            }
                            else if (httpResponseAccessToken.StatusCode == HttpStatusCode.BadRequest)
                            {
                                // Parse the response
                                parsedResponse = JObject.Parse(httpResponseAccessToken.Content);
                                var error = parsedResponse["error"].Value<string>().Trim();
                                var error_description = parsedResponse["error_description"].Value<string>().Trim();
                                if (!string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(error_description))
                                {
                                    throw new Exception(error + " " + error_description);
                                }
                                else if (!string.IsNullOrEmpty(error))
                                {
                                    throw new Exception(error);
                                }
                                else
                                {
                                    throw new Exception(error_description);
                                }
                            }
                        }

                    }
                    else if (oAuthhttpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        // Parse the response
                        var parsedResponse = JObject.Parse(oAuthhttpResponse.Content);
                        var error = parsedResponse["error"].Value<string>().Trim();
                        var error_description = parsedResponse["error_description"].Value<string>().Trim();
                        if (!string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(error_description))
                        {
                            throw new Exception(error + " " + error_description);
                        }
                        else if (!string.IsNullOrEmpty(error))
                        {
                            throw new Exception(error);
                        }
                        else
                        {
                            throw new Exception(error_description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CDP Token Error: {0}", ex);
                throw;
            }
        }

        private async Task<RestResponse> getoauthToken()
        {
            Console.WriteLine("Get CDP OAuth Token Start");
            // RefreshToken
            if (!string.IsNullOrEmpty(cdpAuthToken) && DateTime.Now.AddSeconds(300) <= cdpAuthTokenExpiration)
                return null;

            // workaround to support TLS 1.2 in .NET 4.0
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            RestRequest request = null;

            //request.Timeout = Constants.RESTCLIENT_TIMEOUT;
            //request.ReadWriteTimeout = Constants.RESTCLIENT_READWRITETIMEOUT;

            request = new RestRequest($"{sfLoginUrl.Trim()}/services/oauth2/token", Method.Post);

            //request.Resource = "/services/oauth2/token";
            //request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", userId);
            request.AddParameter("password", password);
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            try
            {
                createRestClientInstance(120);
                RestResponse httpResponse = await _client.ExecuteAsync(request).ConfigureAwait(false);
                if (httpResponse != null && httpResponse.ContentType != "application/json")
                {
                    Console.WriteLine("Get CDP OAuth Token Content Type is not application/json so trying after 30000 ms");
                    await Task.Delay(30000);
                    //client = new RestClient(clientOptions);
                    httpResponse = await _client.ExecuteAsync(request);
                    if (httpResponse != null && httpResponse.ContentType != "application/json")
                    {
                        Console.WriteLine("Get CDP OAuth Token Content Type is not application/json so trying after 60000 ms");
                        await Task.Delay(60000);
                        //client = new RestClient(clientOptions);
                        httpResponse = await _client.ExecuteAsync(request);
                        if (httpResponse != null && httpResponse.ContentType != "application/json")
                        {
                            Console.WriteLine("Get CDP OAuth Token Content Type is not application/json so trying after 90000 ms");
                            await Task.Delay(90000);
                            //client = new RestClient(clientOptions);
                            httpResponse = await _client.ExecuteAsync(request);
                        }
                    }

                }
                return httpResponse;

            }
            catch (Exception ex)
            {
                Console.WriteLine("CDP OAuth Token Error: {0}", ex);
                return null;
            }
        }

        public async Task<SFCDPDataModelQueryResult> getSFCDPDataModelRowsByQuery()
        {
            try
            {
                //getSFCDPDataModelRowsByVIPStatus1();
                string emailId = "customer@geniedemos.com";
                string firstName = "Genie Platform";
                string lastName = "Customer Account";

                //string firstName = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("First_Name")) ? Environment.GetEnvironmentVariable("First_Name") : "";
                //string lastName = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Last_Name")) ? Environment.GetEnvironmentVariable("Last_Name") : "";
                //string emailId = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Email_ID")) ? Environment.GetEnvironmentVariable("Email_ID") : "";

                await getSalesforceAuthToken();

                if (string.IsNullOrEmpty(cdpAuthToken))
                {
                    throw new Exception("Salesforce CDP AuthToken is null");
                }

                if (string.IsNullOrEmpty(cdpRestEndpointUrl))
                {
                    throw new Exception("Salesforce CDP Instance Url is null");
                }

                if (!string.IsNullOrEmpty(cdpAuthToken))
                {
                    RestRequest request = new RestRequest($"{cdpRestEndpointUrl.Trim()}/api/v1/query", Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", $"Bearer {cdpAuthToken}");

                    request.AddJsonBody(
                        new
                        {
                            //sql = $"SELECT UnifiedssotContactPointEmail1001__dlm.ssot__EmailAddress__c," +
                            //$" UnifiedssotIndividual1001__dlm.ssot__BirthDate__c, " +
                            //$"UnifiedssotIndividual1001__dlm.ssot__CurrentEmployerName__c, " +
                            //$"UnifiedssotIndividual1001__dlm.ssot__FirstName__c, " +
                            //$"UnifiedssotIndividual1001__dlm.ssot__InternalOrganizationId__c," +
                            //$" UnifiedssotIndividual1001__dlm.ssot__LastName__c, " +
                            //$"UnifiedssotIndividual1001__dlm.ssot__MaritalStatusId__c, " +
                            //$"UnifiedssotIndividual1001__dlm.ssot__Id__c, " +
                            //$"UnifiedssotIndividual1001__dlm.VIP_Status__c" +
                            //$" FROM UnifiedssotIndividual1001__dlm LEFT JOIN UnifiedssotContactPointEmail1001__dlm  ON UnifiedssotContactPointEmail1001__dlm.ssot__PartyId__c = UnifiedssotIndividual1001__dlm.ssot__Id__c" +
                            //$" WHERE UnifiedssotContactPointEmail1001__dlm.ssot__EmailAddress__c='{emailId}' " +
                            //$"and UnifiedssotIndividual1001__dlm.ssot__FirstName__c='{firstName}' and UnifiedssotIndividual1001__dlm.ssot__LastName__c='{lastName}'  "

                            sql = $"SELECT ssot__ContactPointEmail__dlm .ssot__EmailAddress__c," +
                            $"ssot__ContactPointEmail__dlm.ssot__PartyId__c," +
                            $"ssot__Individual__dlm .ssot__BirthDate__c, " +
                            $"ssot__Individual__dlm .ssot__CurrentEmployerName__c, " +
                            $"ssot__Individual__dlm .ssot__FirstName__c, " +
                            $"ssot__Individual__dlm .ssot__InternalOrganizationId__c," +
                            $"ssot__Individual__dlm .ssot__LastName__c, " +
                            $"ssot__Individual__dlm .ssot__MaritalStatusId__c, " +
                            $"ssot__Individual__dlm .ssot__Id__c, " +
                            $"ssot__Individual__dlm.VIP_Status__c" +
                            $" FROM ssot__Individual__dlm LEFT JOIN ssot__ContactPointEmail__dlm  ON ssot__ContactPointEmail__dlm.ssot__PartyId__c = ssot__Individual__dlm.ssot__Id__c" +
                            $" WHERE ssot__ContactPointEmail__dlm.ssot__EmailAddress__c='{emailId}' " +
                            $"and ssot__Individual__dlm.ssot__FirstName__c='{firstName}' and ssot__Individual__dlm.ssot__LastName__c='{lastName}'"

                            //sql = $"SELECT {string.Join(", ", fields)} FROM {modelName} WHERE ssot__FirstName__c='{firstName}' and ssot__LastName__c='{lastName}' ORDER BY {string.Join(", ", primaryKeys.Select(s => $"{s} ASC").ToList())}"
                            //sql = $"SELECT {string.Join(", ", fields)} FROM {modelName} WHERE ssot__FirstName__c='{firstName}' and ssot__LastName__c='{lastName}' ORDER BY {string.Join(", ", primaryKeys.Select(s => $"{s} ASC").ToList())} "
                        }
                    );


                    createRestClientInstance(120);

                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    RestResponse httpResponse = await _client.ExecuteAsync(request);
                    if (httpResponse != null)
                    {
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<SFCDPDataModelQueryResult>(httpResponse.Content);
                            return await Task.FromResult(result);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("CDP Token Error: {0}", ex);
                throw ex;
            }

            return await Task.FromResult(default(SFCDPDataModelQueryResult));
        }

        public async Task<SFCDPDataModelQueryResult> getSFContactById(string contactId)
        {
            try
            {
                await getSalesforceAuthToken();

                if (string.IsNullOrEmpty(cdpAuthToken))
                {
                    throw new Exception("Salesforce CDP AuthToken is null");
                }

                if (string.IsNullOrEmpty(cdpRestEndpointUrl))
                {
                    throw new Exception("Salesforce CDP Instance Url is null");
                }

                if (!string.IsNullOrEmpty(cdpAuthToken))
                {
                    RestRequest request = new RestRequest($"{cdpRestEndpointUrl.Trim()}/services/data/v42.0/query", Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", $"Bearer {cdpAuthToken}");
                    //https://ap5.salesforce.com/services/data/v42.0/query?q=SELECT Id,FirstName,LastName FROM Contact
                    request.AddJsonBody(
                        new
                        {
                            sql = $"SELECT FirstName,LastName,Email,VIP_Status_c,LastModifiedDate FROM Contact WHERE Id='{contactId}'"
                        }
                    );


                    createRestClientInstance(120);

                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    RestResponse httpResponse = await _client.ExecuteAsync(request);
                    if (httpResponse != null)
                    {
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            var result = JsonConvert.DeserializeObject<SFCDPDataModelQueryResult>(httpResponse.Content);
                            return await Task.FromResult(result);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("CDP Token Error: {0}", ex);
                throw;
            }

            return await Task.FromResult(default(SFCDPDataModelQueryResult));
        }

        public void createRestClientInstance(int timeoutInSeconds)
        {
            if (_client == null)
            {
                _client = new RestClient(new RestClientOptions()
                {
                    FollowRedirects = false,
                    MaxTimeout = (timeoutInSeconds * 1000),
                    ThrowOnAnyError = false,
                    CachePolicy = new CacheControlHeaderValue()
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true,
                        MaxAge = TimeSpan.Zero
                    },
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                });
                //_client.UseNewtonsoftJson();
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
