using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Win8Apps.Extensions
{
    public static class UriBuilderExtensions
    {
        /// <summary>
        /// Sets the specified query parameter key-value pair of the URI.
        /// If the key already exists, the value is overwritten.
        /// </summary>
        public static UriBuilder SetQueryParam(this UriBuilder uri, string key, string value)
        {
            var queryToAppend = string.Format("{0}={1}", key, value);
            if (uri.Query.Length > 1)
                uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
            else
                uri.Query = queryToAppend;

            return uri;
        }

        public static string ConstructQueryString(this NameValueCollection parameters)
        {
            var items = new List<string>();

            foreach (String name in parameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

            return String.Join("&", items.ToArray());
        }
    }
}