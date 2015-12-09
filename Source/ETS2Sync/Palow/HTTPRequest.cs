using log4net;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Palow.Library
{
    public class HTTPRequest
    {
        #region Properties
        private String contentType;

        public String ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }


        private String origin;

        public String Origin
        {
            get { return origin; }
            set { origin = value; }
        }


        private Boolean sendCookies;

        public Boolean SendCookies
        {
            get { return sendCookies; }
            set { sendCookies = value; }
        }

        public Int32 TimeOut { get; set; }

        private String accept;

        public String Accept
        {
            get { return accept; }
            set { accept = value; }
        }


        private CookieContainer cookies;

        public CookieContainer Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }
        private Boolean isAjax;

        public Boolean IsAjax
        {
            get { return isAjax; }
            set { isAjax = value; }
        }

        private String referer;

        public String Referer
        {
            get { return referer; }
            set { referer = value; }
        }

        private Boolean logSendHTTPHeader;

        public Boolean LogSendHTTPHeader
        {
            get { return logSendHTTPHeader; }
            set { logSendHTTPHeader = value; }
        }

        private Boolean logResponseHTTPHeader;

        public Boolean LogResponseHTTPHeader
        {
            get { return logResponseHTTPHeader; }
            set { logResponseHTTPHeader = value; }
        }

        private Boolean logUrlsAndPost;

        public Boolean LogUrlsAndPost
        {
            get { return logUrlsAndPost; }
            set { logUrlsAndPost = value; }
        }


        private String url;

        public String Url
        {
            get { return url; }
            set { url = value; }
        }

        private String post;

        public String Post
        {
            get { return post; }
            set { post = value; }
        }

        private String response;

        public String Response
        {
            get { return response; }
            set { response = value; }
        }

        private String baseUrl;

        public String BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        private String cookieSavePath;

        public String CookieSavePath
        {
            get { return cookieSavePath; }
            set { cookieSavePath = value; }
        }

        private Boolean allowAutoRedirect;

        public Boolean AllowAutoRedirect
        {
            get { return allowAutoRedirect; }
            set { allowAutoRedirect = value; }
        }



        #endregion


        public HTTPRequest()
        {
            this.post = "";
            this.cookies = new CookieContainer();
            this.logResponseHTTPHeader = false;
            this.logSendHTTPHeader = false;
            this.logUrlsAndPost = false;
            this.response = "";
            this.cookieSavePath = "cookies.txt";
            this.origin = "";
            this.sendCookies = true;
            this.contentType = "application/x-www-form-urlencoded";
            this.accept = "";
            this.allowAutoRedirect = true;
            ServicePointManager.DefaultConnectionLimit = Int32.MaxValue;
        }


        #region methods
        /// <summary>
        /// Creates FullUrl, prefix baseurl
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public String CreateFullUrl(String path)
        {
            this.url = this.baseUrl + path;
            return this.url;
        }

        public void SaveCookiesToDisk()
        {
            Functions.WriteCookiesToDisk(this.cookieSavePath, this.cookies);
        }

        public void LoadCookiesFromDisk()
        {
            this.cookies = Functions.ReadCookiesFromDisk(this.cookieSavePath);
        }

        #region Request Methods
        public virtual String Request()
        {
            String tmpPost = this.post;
            this.post = "";
            Boolean tmpIsAjax = this.isAjax;
            this.isAjax = false;
            String tmpReferer = this.referer;
            this.referer = "";

            if (this.url == null || this.url.Length == 0)
            {
                this.url = this.baseUrl;
            }

            return (String)this.Request(this.url, tmpPost, tmpIsAjax, tmpReferer, false, false, true);
        }


        public virtual WebResponse Request(Boolean returnWebResponse, Boolean autoCloseWebresponse)
        {
            String tmpPost = this.post;
            this.post = "";
            Boolean tmpIsAjax = this.isAjax;
            this.isAjax = false;
            String tmpReferer = this.referer;
            this.referer = "";

            if (this.url == null || this.url.Length == 0)
            {
                this.url = this.baseUrl;
            }

            return (WebResponse)this.Request(this.url, tmpPost, tmpIsAjax, tmpReferer, false, true, autoCloseWebresponse);
        }

        public virtual WebResponse Request(Boolean autoCloseWebResponse, Int32 rangeStart, Int32 rangeEnd)
        {
            String tmpPost = this.post;
            this.post = "";
            Boolean tmpIsAjax = this.isAjax;
            this.isAjax = false;
            String tmpReferer = this.referer;
            this.referer = "";

            if (this.url == null || this.url.Length == 0)
            {
                this.url = this.baseUrl;
            }

            return (WebResponse)this.Request(this.url, tmpPost, tmpIsAjax, tmpReferer, false, true, autoCloseWebResponse, rangeStart, rangeEnd);
        }



        public virtual Object Request(String url, String post, Boolean isAjax, String referer, Boolean getImage, Boolean getWebResponse, Boolean autoCloseWebresponse)
        {
            Object resp = this.Request(this.url, post, isAjax, referer, getImage, getWebResponse, autoCloseWebresponse, Int64.MinValue, Int64.MinValue);

            return resp;
        }


        public virtual Object Request(String url, String post, Boolean isAjax, String referer, Boolean getImage, Boolean getWebResponse, Boolean autoCloseWebresponse, Int64 rangeStart, Int64 rangeEnd)
        {
            return this.Request(this.url, post, isAjax, referer, getImage, getWebResponse, autoCloseWebresponse, rangeStart, rangeEnd, Int32.MinValue);
        }

        public virtual Object Request(String url, String post, Boolean isAjax, String referer, Boolean getImage, Boolean getWebResponse, Boolean autoCloseWebresponse, Int64 rangeStart, Int64 rangeEnd, Int32 timeout)
        {
            return this.Request(this.url, post, isAjax, referer, getImage, getWebResponse, autoCloseWebresponse, rangeStart, rangeEnd, timeout, false);
        }

     
        public virtual Object Request(String url, String post, Boolean isAjax, String referer, Boolean getImage, Boolean getWebResponse, Boolean autoCloseWebresponse, Int64 rangeStart, Int64 rangeEnd, Int32 timeout, Boolean requestHead)
        {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
            WebRequest webRequest = WebRequest.Create(url);
   

            ((HttpWebRequest)webRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
            ((HttpWebRequest)webRequest).AllowAutoRedirect = this.allowAutoRedirect;

            if (timeout != Int32.MinValue || this.TimeOut != 0)
            {
                if (this.TimeOut != 0)
                {
                    timeout = this.TimeOut;
                }
                ((HttpWebRequest)webRequest).Timeout = timeout * 1000;
            }
            else
            {
                ((HttpWebRequest)webRequest).Timeout = 5000;
            }


            ((HttpWebRequest)webRequest).ReadWriteTimeout = webRequest.Timeout;

            if (rangeStart != Int64.MinValue && rangeEnd != Int64.MinValue)
            {
                ((HttpWebRequest)webRequest).AddRange(rangeStart, rangeEnd);
            }

            if (sendCookies)
            {
                ((HttpWebRequest)webRequest).CookieContainer = this.cookies;
            }

            //         (HttpWebRequest)webRequest).CookieContainer = this.cookies;    

            if (isAjax)
            {
                ((HttpWebRequest)webRequest).Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
            if (referer != "")
            {
                ((HttpWebRequest)webRequest).Referer = referer;
            }
            if (getImage)
            {
                ((HttpWebRequest)webRequest).Accept = "image/png,image/*;q=0.8,*/*;q=0.5";
            }
            if (this.accept.Length > 0)
            {
                ((HttpWebRequest)webRequest).Accept = this.accept;
            }

            if (post.Length > 0)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = this.contentType;

                byte[] bytes = Encoding.ASCII.GetBytes(post);

                Stream os = null;
                try
                { // send the Post
                    webRequest.ContentLength = bytes.Length;   //Count bytes to send
                    os = webRequest.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);         //Send it

                }
                catch (WebException ex)
                {
                    lg.Error("Error Send HTTP Request: " + ex.Message);
                }
                finally
                {
                    if (os != null)
                    {
                        os.Flush();
                        os.Close();
                    }
                }
            }
            else if (requestHead)
            {
                webRequest.Method = "HEAD";
            }
            else
            {
                webRequest.Method = "GET";
            }

            if (this.logUrlsAndPost)
            {
                lg.Debug("Load: " + url + " Post:" + post);
            }

            WebResponse webResponse = null;
            try
            { // get the response
                if (this.logSendHTTPHeader)
                {
                    lg.Debug("SentHeaders:" + webRequest.Headers.ToString() + "\r\n");
                }
                webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse == null)
                {
                    if (getImage)
                    {
                        return null;
                    }
                    else
                    {
                        return "";
                    }
                }
                try
                {
                    this.cookies.Add(((HttpWebResponse)webResponse).Cookies);
                }
                catch (Exception e)
                {
                    lg.Error("Can't add Cookie: " + e.Message);
                }
                if (this.logSendHTTPHeader)
                {
                    lg.Debug("ResponseHeaders:" + webResponse.Headers.ToString());
                }

                // convert webstream to image
                if (getImage)
                {
                  
                    return null;
                }
                else if (getWebResponse)
                {
                    if (autoCloseWebresponse)
                    {
                        webResponse.Close();
                    }
                    return webResponse;
                }
                else
                {
                    StreamReader sr = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    this.response = sr.ReadToEnd();
                    webResponse.Close();
                    return this.response;
                }
            }
            catch (Exception ex)
            {
                lg.Error("HttpPost: Response error" + Functions.GetExceptionText(ex));
                if (webResponse != null)
                {
                    webResponse.Close();
                }
                return webResponse;
            }
        }
        #endregion

        #region Upload Methods
        /// <summary>
        /// Upload File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="paramName"></param>
        /// <param name="contentType"></param>
        /// <param name="post"></param>
        public String UploadFile(string file, string paramName, string contentType, NameValueCollection post)
        {
            return this.UploadFile(this.url, file, paramName, contentType, post);
        }

        public String UploadFile(string file, string paramName, string contentType)
        {
            return this.UploadFile(file, paramName, contentType, new NameValueCollection());
        }

        public String UploadFile(string url, string file, string paramName, string contentType, NameValueCollection post)
        {
            return this.UploadFile(this.url, file, paramName, contentType, post, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:16.0) Gecko/20100101 Firefox/16.0");
        }

        /// <summary>
        /// Upload File (ignores settings)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="paramName"></param>
        /// <param name="contentType"></param>
        /// <param name="post"></param>
        public String UploadFile(string url, string file, string paramName, string contentType, NameValueCollection post, String userAgent)
        {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.response = "";
            lg.Debug(string.Format("Uploading {0} to {1}", file, url));
            //string boundary = "---------------------------" + Guid.NewGuid().ToString();
            string boundary = "---------------------------2921238217425";
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("--2921238217421" + "\n");
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.UserAgent = userAgent;
            wr.Accept = this.accept;
            wr.Expect = "";
            if (this.origin.Length > 0)
            {
                wr.Headers.Add("Origin", this.origin);
            }
            wr.KeepAlive = true;
            wr.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            wr.Referer = this.referer;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            if (sendCookies)
            {
                ((HttpWebRequest)wr).CookieContainer = this.cookies;
            }

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            foreach (string key in post.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, post[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, Path.GetFileName(file), contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                this.response = reader2.ReadToEnd();
                lg.Debug(string.Format("File uploaded, server response is: {0}", this.response));
            }
            catch (Exception ex)
            {
                lg.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            try
            {
                this.cookies.Add(((HttpWebResponse)wresp).Cookies);
            }
            catch (Exception e)
            {
                lg.Error("Can't add Cookie: " + e.Message);
            }

            return this.response;
        }
        #endregion




      


        public String DownloadFile(String filePath)
        {
            WebResponse response = (WebResponse)this.Request(this.url, this.post, this.isAjax, this.referer, false, true, false, Int64.MinValue, Int64.MinValue);

            String filename;
            
            filename = Path.GetFileName(filePath);
            

            if (String.IsNullOrEmpty(filename))
            {
                filename = Path.GetRandomFileName();
            }


            String path = "";
            if (Path.IsPathRooted(filePath))
            {
                path = Path.GetDirectoryName(filePath) + @"\";
            }
            path += filename;

            Functions.StreamToFile(response.GetResponseStream(), path, FileMode.Create);

            response.Close();


            return filename;
        }

        #endregion
    }
}
