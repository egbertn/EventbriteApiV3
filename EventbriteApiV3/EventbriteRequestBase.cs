﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace EventbriteApiV3
{
    public abstract class EventbriteRequestBase
    {
        protected EventbriteContext Context;

        protected readonly NameValueCollection Query;
        private readonly string _path;

        public Uri Url
        {
            get
            {
                var uri = new UriBuilder();
                uri.Scheme = Uri.UriSchemeHttps;
                uri.Host = Context.Uri;
                uri.Path = _path;

                var querystring = HttpUtility.ParseQueryString("");
                querystring.Add(Query);
                uri.Query = querystring.ToString();
                return uri.Uri;
            }
        }

        protected EventbriteRequestBase(string path, EventbriteContext context, NameValueCollection query = null)
        {
            Context = context;
            Query = query ?? new NameValueCollection();
            _path = path;
        }

        protected string GetJsonResponse()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {Context.AppKey}");
            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }

        protected async Task<string> GetJsonResponseAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {Context.AppKey}");
            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            var response = await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                return await sr.ReadToEndAsync();
            }
        }

        protected  async Task<TextReader> GetStreamResponseAsync()
        {
            var request = WebRequest.Create(Url);
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {Context.AppKey}");
            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            return new StreamReader((await request.GetResponseAsync()).GetResponseStream());
        }
    }
}
