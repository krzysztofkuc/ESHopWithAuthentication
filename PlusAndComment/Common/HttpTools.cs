using HtmlAgilityPack;
using System.IO;
using System.Net;

namespace PlusAndComment.Common
{
    public static class HttpTools
    {
        public static HtmlDocument GetHtmlDocument(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader responseStream = new StreamReader(response.GetResponseStream());

            string result = responseStream.ReadToEnd();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);

            return document;
        }
    }
}