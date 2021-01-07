using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace JxCode.Net
{
    public class ContentType
    {
        public const string xhtml_xml = "application/xhtml+xml";
        public const string xml = "application/xml";
        public const string json = "application/json";
        public const string pdf = "application/pdf";
        public const string msword = "application/msword";
        public const string octet_stream = "application/octet-stream";
        public const string form = "application/x-www-form-urlencoded";

        public const string html = "text/html";
        public const string text_plain = "text/plain";
        public const string text_xml = "text/xml";

        public const string img_gif = "image/gif";
        public const string img_jpeg = "image/jpeg";
        public const string img_png = "image/png";
    }
    public class RequestResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        public RequestResult(bool isSuccess, string message)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }
    }
    public class Http
    {

        /// <summary>
        /// 获取远程文件长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static long GetHttpFileLenght(string url)
        {
            long length = 0;
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;// 打开网络连接
            HttpWebResponse rsp = req.GetResponse() as HttpWebResponse;

            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                length = rsp.ContentLength;// 从文件头得到远程文件的长度
            }

            rsp.Close();
            return length;
        }

        public static Task<RequestResult> Post(string target, string content, string contentType, int timeOut = 5)
        {
            HttpWebRequest req = HttpWebRequest.Create(target) as HttpWebRequest;
            req.Method = "POST";
            req.Timeout = timeOut * 1000;
            req.ContentType = contentType;
            Task<RequestResult> task = Task.Factory.StartNew<RequestResult>(() =>
            {
                string rtnStr = string.Empty;
                WebResponse webres = null;
                //写入流
                try
                {
                    byte[] buf = Encoding.UTF8.GetBytes(content);
                    req.ContentLength = buf.Length;
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(buf, 0, buf.Length);
                        stream.Flush();
                    }
                }
                catch (Exception e)
                {
                    //写入失败
                    return new RequestResult(false, e.Message);
                }

                //获取返回流
                try
                {
                    webres = req.GetResponse();
                }
                catch (WebException ex)
                {
                    //遇到404什么的都会报错
                    //可以先不管，稍后还可以获取具体的错误返回信息
                    webres = ex.Response;
                }
                catch (Exception ex)
                {
                    //其他错误
                    return new RequestResult(false, ex.Message);
                }

                //具体的返回信息
                try
                {
                    if(webres == null)
                    {
                        return new RequestResult(false, "无法连接服务器");
                    }
                    using (Stream stream = webres.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            rtnStr = sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new RequestResult(false, ex.Message);
                }
                //一切正常的返回
                return new RequestResult(true, rtnStr);
            });
            return task;
        }

        public static Task<RequestResult> PostJson(string target, string json, int timeOut = 5)
        {
            return Post(target, json, ContentType.json, timeOut);
        }
        public static Task<RequestResult> PostForm(string target, string form, int timeOut = 5)
        {
            return Post(target, form, ContentType.form, timeOut);
        }
		
		public static Dictionary<string, string> Parse(string url)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] elements = url.Split('&');
            foreach (string element in elements)
            {
                string[] keyValue = element.Split('=');
                if (keyValue.Length != 2) throw new ArgumentException("表单内容错误");
                dict.Add(keyValue[0], keyValue[1]);
            }
            return dict;
        }
    }
}
