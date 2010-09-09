using System;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using GithubSharp.Core.Services;

namespace GithubSharp.Core.Base
{
    internal class Url
    {
        internal Url(ICacheProvider cacheProvider, ILogProvider logProvider)
        {
            _CacheProvider = cacheProvider;
            _LogProvider = logProvider;
        }

        internal ICacheProvider _CacheProvider;
        internal ILogProvider _LogProvider;

        internal string GithubBaseURL { get { return "http://github.com/api/v2/json/"; } }

        internal string GithubAuthenticationQueryString(GithubSharp.Core.Models.GithubUser User)
        {
            return User != null ? string.Format("?login={0}&token={1}", User.Name, User.APIToken) : string.Empty;
        }

        internal byte[] GetBinaryFromURL(string URL)
        {
            _LogProvider.LogMessage(string.Format("Url.GetBinaryFromURL - '{0}'", URL));
            string cacheKey = string.Format("GetBinaryFromURL_{0}", URL);

            var cached = _CacheProvider.Get<byte[]>(cacheKey);
            if (cached != null)
            {
                _LogProvider.LogMessage("Url.GetBinaryFromURL  :  Returning cached result");
                return cached;
            }
            _LogProvider.LogMessage("Url.GetBinaryFromURL  :  Cached result unavailable, fetching url content");

            var webClient = new System.Net.WebClient();
            byte[] result;
            try
            {
                result = webClient.DownloadData(URL);
            }
            catch (Exception error)
            {
                if (_LogProvider.HandleAndReturnIfToThrowError(error))
					throw;
                return null;
            }

            _CacheProvider.Set(result, cacheKey);
            return result;
        }

        internal byte[] UploadValuesAndGetBinary(string URL, NameValueCollection FormValues)
        {
            return UploadValuesAndGetBinary(URL, FormValues, "POST");
        }
        internal byte[] UploadValuesAndGetBinary(string URL, NameValueCollection FormValues, string Method)
        {
            _LogProvider.LogMessage(
              string.Format("Url.UploadValuesAndGetBinary ({0}) {1}",
              URL,
              string.Join(":", FormValues.AllKeys.ToList().Select(Key => string.Format("\nKey : {0} , Value : {1}", Key, FormValues[Key])).ToArray())));

            string cacheKey = string.Format("UploadValuesAndGetBinary{0}_{1}_{2}_{3}",
                URL,
                Method,
                string.Join("-", FormValues.AllKeys),
                string.Join(":", FormValues.AllKeys.ToList().Select(Key => FormValues[Key]).ToArray())
                );

            var cached = _CacheProvider.Get<byte[]>(cacheKey);
            if (cached != null)
            {
                _LogProvider.LogMessage("Url.UploadValuesAndGetBinary  :  Returning cached result");
                return cached;
            }

            _LogProvider.LogMessage("Url.UploadValuesAndGetBinary  :  Cached result unavailable, fetching url content");


            var webClient = new System.Net.WebClient();
            byte[] result;
            try
            {
                result = webClient.UploadValues(URL, Method, FormValues);
            }
            catch (Exception error)
            {
                if (_LogProvider.HandleAndReturnIfToThrowError(error))
					throw;
                return null;
            }

            _CacheProvider.Set(result, cacheKey);
            return result;
        }

        internal string UploadValuesAndGetString(string URL, NameValueCollection FormValues)
        {
            return UploadValuesAndGetString(URL, FormValues, "POST");
        }

        internal string UploadValuesAndGetString(string URL, NameValueCollection FormValues, string Method)
        {
            _LogProvider.LogMessage("Url.UploadValuesAndGetString - Calling UploadValuesAndGetBinary...");

            var result = UploadValuesAndGetBinary(URL, FormValues, Method);

            return result == null ? null : Encoding.UTF8.GetString(result);
        }

        internal string GetStringFromURL(string URL)
        {
            string cacheKey = string.Format("GetStringFromURL_{0}", URL);
            var cached = _CacheProvider.Get<string>(cacheKey);
            if (cached != null)
            {
                _LogProvider.LogMessage("Url.GetStringFromURL  :  Returning cached result");
                return cached;
            }

            _LogProvider.LogMessage("Url.GetStringFromURL  :  Cached result unavailable, fetching url content");

            var webClient = new System.Net.WebClient();
            string result;
            try
            {
                result = webClient.DownloadString(URL);
            }
            catch (Exception error)
            {
                if (_LogProvider.HandleAndReturnIfToThrowError(error))
					throw;
                return null;
            }

            _CacheProvider.Set(result, cacheKey);
            return result;
        }

    }
}
