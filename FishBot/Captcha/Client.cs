using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FishBot.Captcha
{
    #region Two Captcha Class
    public class TwoCaptcha
    {
        /**
         * API KEY
         */
        public string ApiKey { get; set; }

        /**
         * ID of software developer. Developers who integrated their software
         * with our service get reward: 10% of spendings of their software users.
         */
        public int SoftId { get; set; }

        /**
         * URL to which the result will be sent
         */
        public string Callback { get; set; }

        /**
         * How long should wait for captcha result (in seconds)
         */
        public int DefaultTimeout { get; set; } = 120;

        /**
         * How long should wait for recaptcha result (in seconds)
         */
        public int RecaptchaTimeout { get; set; } = 600;

        /**
         * How often do requests to `/res.php` should be made
         * in order to check if a result is ready (in seconds)
         */
        public int PollingInterval { get; set; } = 10;

        /**
         * Helps to understand if there is need of waiting
         * for result or not (because callback was used)
         */
        private bool lastCaptchaHasCallback;

        /**
         * Network client
         */
        private ApiClient apiClient;

        /**
         * TwoCaptcha constructor
         */
        public TwoCaptcha()
        {
            apiClient = new ApiClient();
        }

        /**
         * TwoCaptcha constructor
         *
         * @param apiKey
         */
        public TwoCaptcha(string apiKey) : this()
        {
            ApiKey = apiKey;
        }

        /**
         * @param apiClient
         */
        public void SetApiClient(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        /**
         * Sends captcha to `/in.php` and waits for it's result.
         * This helper can be used instead of manual using of `send` and `getResult` functions.
         *
         * @param captcha
         * @throws Exception
         */
        public void Solve(Captcha captcha)
        {
            var waitOptions = new Dictionary<string, int>();

            /*            if (captcha.GetType() == typeof(ReCaptcha))
                        {
                            waitOptions["timeout"] = RecaptchaTimeout;
                        }*/

            Solve(captcha, waitOptions);
        }

        /**
         * Sends captcha to `/in.php` and waits for it's result.
         * This helper can be used instead of manual using of `send` and `getResult` functions.
         *
         * @param captcha
         * @param waitOptions
         * @throws Exception
         */
        public void Solve(Captcha captcha, Dictionary<string, int> waitOptions)
        {
            captcha.Id = Send(captcha);

            if (!lastCaptchaHasCallback)
            {
                WaitForResult(captcha, waitOptions);
            }
        }

        /**
         * This helper waits for captcha result, and when result is ready, returns it
         *
         * @param captcha
         * @param waitOptions
         * @throws Exception
         */
        public void WaitForResult(Captcha captcha, Dictionary<string, int> waitOptions)
        {
            long startedAt = CurrentTime();

            int timeout = waitOptions.TryGetValue("timeout", out timeout) ? timeout : DefaultTimeout;
            int pollingInterval = waitOptions.TryGetValue("pollingInterval", out pollingInterval)
                ? pollingInterval
                : PollingInterval;

            while (true)
            {
                long now = CurrentTime();

                if (now - startedAt < timeout)
                {
                    Thread.Sleep(pollingInterval * 1000);
                }
                else
                {
                    break;
                }

                try
                {
                    string result = GetResult(captcha.Id);
                    if (result != null)
                    {
                        captcha.Code = result;
                        return;
                    }
                }
                catch (NetworkException)
                {
                    // ignore network errors
                }
            }

            throw new TimeoutException("Timeout " + timeout + " seconds reached");
        }

        private long CurrentTime()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        /**
         * Sends captcha to '/in.php', and returns its `id`
         *
         * @param captcha
         * @return
         * @throws Exception
         */
        public string Send(Captcha captcha)
        {
            var parameters = captcha.GetParameters();
            var files = captcha.GetFiles();

            SendAttachDefaultParameters(parameters);

            ValidateFiles(files);

            string response = apiClient.In(parameters, files);

            if (!response.StartsWith("OK|"))
            {
                throw new ApiException("Cannot recognise api response (" + response + ")");
            }

            return response.Substring(3);
        }

        /**
         * Returns result of captcha if it was solved or `null`, if result is not ready
         *
         * @param id
         * @return
         * @throws Exception
         */
        public string GetResult(String id)
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = "get";
            parameters["id"] = id;

            string response = Res(parameters);

            if (response.Equals("CAPCHA_NOT_READY"))
            {
                return null;
            }

            if (!response.StartsWith("OK|"))
            {
                throw new ApiException("Cannot recognise api response (" + response + ")");
            }

            return response.Substring(3);
        }

        /**
         * Gets account's balance
         *
         * @return
         * @throws Exception
         */
        public string Balance()
        {
            return Res("getbalance");
        }

        /**
         * Reports if captcha was solved correctly (sends `reportbad` or `reportgood` to `/res.php`)
         *
         * @param id
         * @param correct
         * @throws Exception
         */
        public void Report(string id, bool correct)
        {
            var parameters = new Dictionary<string, string>();
            parameters["id"] = id;

            if (correct)
            {
                parameters["action"] = "reportgood";
            }
            else
            {
                parameters["action"] = "reportbad";
            }

            Res(parameters);
        }

        /**
         * Makes request to `/res.php`
         *
         * @param action
         * @return
         * @throws Exception
         */
        private string Res(string action)
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = action;
            return Res(parameters);
        }

        /**
         * Makes request to `/res.php`
         *
         * @param params
         * @return
         * @throws Exception
         */
        private string Res(Dictionary<string, string> parameters)
        {
            parameters["key"] = ApiKey;
            return apiClient.Res(parameters);
        }

        /**
         * Attaches default parameters to request
         *
         * @param params
         */
        private void SendAttachDefaultParameters(Dictionary<string, string> parameters)
        {
            parameters["key"] = ApiKey;

            if (Callback != null)
            {
                if (!parameters.ContainsKey("pingback"))
                {
                    parameters["pingback"] = Callback;
                }
                else if (parameters["pingback"].Length == 0)
                {
                    parameters.Remove("pingback");
                }
            }

            lastCaptchaHasCallback = parameters.ContainsKey("pingback");

            if (SoftId != 0 && !parameters.ContainsKey("soft_id"))
            {
                parameters["soft_id"] = Convert.ToString(SoftId);
            }
        }

        /**
         * Validates if files parameters are correct
         *
         * @param files
         * @throws ValidationException
         */
        private void ValidateFiles(Dictionary<string, FileInfo> files)
        {
            foreach (KeyValuePair<string, FileInfo> entry in files)
            {
                FileInfo file = entry.Value;

                if (!file.Exists)
                {
                    throw new ValidationException("File not found: " + file.FullName);
                }
            }
        }
    }
    #endregion

    #region ApiClient Class
    public class ApiClient
    {
        /**
         * API server
         */
        private string baseUrl = "https://rucaptcha.com/";

        /**
         * Network client
         */
        private readonly HttpClient client = new HttpClient();

        public ApiClient()
        {
            client.BaseAddress = new Uri(baseUrl);
        }

        public virtual string In(Dictionary<string, string> parameters, Dictionary<string, FileInfo> files)
        {
            var content =
                new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

            foreach (KeyValuePair<string, string> p in parameters)
            {
                content.Add(new StringContent(p.Value), p.Key);
            }

            foreach (KeyValuePair<string, FileInfo> f in files)
            {
                var fileStream = new StreamContent(new MemoryStream(File.ReadAllBytes(f.Value.FullName)));
                content.Add(fileStream, f.Key, f.Value.Name);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "in.php")
            {
                Content = content
            };

            return Execute(request);
        }

        public virtual string Res(Dictionary<string, string> parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "res.php?" + BuildQuery(parameters));

            return Execute(request);
        }

        private string BuildQuery(Dictionary<string, string> parameters)
        {
            string query = "";

            foreach (KeyValuePair<string, string> p in parameters)
            {
                if (query.Length > 0)
                {
                    query += "&";
                }

                query += p.Key + "=" + Uri.EscapeDataString(p.Value);
            }

            return query;
        }

        private string Execute(HttpRequestMessage request)
        {
            var response = client.SendAsync(request).GetAwaiter().GetResult();

            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Unexpected response: " + body);
            }

            if (body.StartsWith("ERROR_"))
            {
                throw new ApiException(body);
            }

            return body;
        }
    }
    #endregion

    #region Exception Classes
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {

        }
    }

    public class NetworkException : Exception
    {
        public NetworkException(string message) : base(message)
        {

        }
    }

    public class TimeoutException : Exception
    {
        public TimeoutException(string message) : base(message)
        {

        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
    #endregion

    #region Captcha Class
    public abstract class Captcha
    {
        public string Id { get; set; }
        public string Code { get; set; }

        protected Dictionary<string, string> parameters;
        protected Dictionary<string, FileInfo> files;

        public Captcha()
        {
            parameters = new Dictionary<string, string>();
            files = new Dictionary<string, FileInfo>();
        }

        public void SetProxy(string type, string uri)
        {
            parameters["proxy"] = uri;
            parameters["proxytype"] = type;
        }

        public void SetSoftId(int softId)
        {
            parameters["soft_id"] = Convert.ToString(softId);
        }

        public void SetCallback(String callback)
        {
            parameters["pingback"] = callback;
        }

        public Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>(this.parameters);

            if (!parameters.ContainsKey("method"))
            {
                if (parameters.ContainsKey("body"))
                {
                    parameters["method"] = "base64";
                }
                else
                {
                    parameters["method"] = "post";
                }
            }

            return parameters;
        }

        public Dictionary<string, FileInfo> GetFiles()
        {
            return new Dictionary<string, FileInfo>(files);
        }
    }

    public class Normal : Captcha
    {
        public Normal() : base()
        {
        }

        public Normal(String filePath) : this(new FileInfo(filePath))
        {
        }

        public Normal(FileInfo file) : this()
        {
            SetFile(file);
        }

        public void SetFile(string filePath)
        {
            SetFile(new FileInfo(filePath));
        }

        public void SetFile(FileInfo file)
        {
            files["file"] = file;
        }

        public void SetBase64(String base64)
        {
            parameters["body"] = base64;
        }

        public void SetPhrase(bool phrase)
        {
            parameters["phrase"] = phrase ? "1" : "0";
        }

        public void SetCaseSensitive(bool caseSensitive)
        {
            parameters["regsense"] = caseSensitive ? "1" : "0";
        }

        public void SetCalc(bool calc)
        {
            parameters["calc"] = calc ? "1" : "0";
        }

        public void SetNumeric(int numeric)
        {
            parameters["numeric"] = Convert.ToString(numeric);
        }

        public void SetMinLen(int length)
        {
            parameters["min_len"] = Convert.ToString(length);
        }

        public void SetMaxLen(int length)
        {
            parameters["max_len"] = Convert.ToString(length);
        }

        public void SetLang(String lang)
        {
            parameters["lang"] = lang;
        }

        public void SetHintText(String hintText)
        {
            parameters["textinstructions"] = hintText;
        }

        public void SetHintImg(String base64)
        {
            parameters["imginstructions"] = base64;
        }

        public void SetHintImg(FileInfo hintImg)
        {
            files["imginstructions"] = hintImg;
        }
    }
    #endregion
}