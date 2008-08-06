///
/// Author: Aron J
/// Date: 08/05/08

using System;
using System.IO;
using System.Net;
using System.Text;

namespace FreericeBot
{
    class StatusNotOK : Exception
        {
        private readonly HttpStatusCode m_statusCode;
        
        public StatusNotOK(HttpStatusCode statusCode)
            {
            m_statusCode = statusCode;
            }

        public HttpStatusCode GetStatusCode()
            {
            return m_statusCode;
            }
        }


    class WebRequestWrapper
        {
        public string GetStringRepresentationOfSite(string site)
            {
            WebRequest request = WebRequest.Create(site);
            return GetStringRepresentationOfSite(request);
            }

        public string GetStringRepresentationOfSite(string site, string method, string data)
            {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] byteData = encoding.GetBytes(data);

            WebRequest request = WebRequest.Create(site);
            request.Method = method;
            request.ContentLength = byteData.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteData, 0, byteData.Length);
            dataStream.Close();

            return GetStringRepresentationOfSite(request);
            }

        private static string GetStringRepresentationOfSite(WebRequest request)
            {
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            if(response.StatusCode != HttpStatusCode.OK)
                {
                throw new StatusNotOK(response.StatusCode);
                }

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
            }
       }
}
