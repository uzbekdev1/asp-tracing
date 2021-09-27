using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspMvcTrackingLib
{
    public static class TrackingManager
    {
         
        private static string GetAuditLog(HttpContext context)
        {

            var sb = new StringBuilder();

            sb.AppendLine("-------------" + DateTime.Now + "------------------");

            var ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            sb.AppendLine("Remote IP:" + ip);
            sb.AppendLine("Request Method:" + context.Request.HttpMethod);
            sb.AppendLine("Request URL:" + context.Request.Url);
            sb.AppendLine("User agent:" + context.Request.UserAgent);
            sb.AppendLine("Headers:");
            var heades = context.Request.Headers;
            for (var i = 0; i < heades.Count; i++)
            {
                sb.AppendLine("     " + heades.GetKey(i) + ":" + heades.Get(i));
            }

            sb.AppendLine("Post data:");
            var stream = context.Request.InputStream;
            if (stream != null && stream.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                sb.AppendLine(reader.ReadToEnd());
            }

            sb.AppendLine("---------------------------------------------------");

            return sb.ToString();
        }

        public static void WriteAccessLog(this HttpContext context, string user)
        {

            var date = DateTime.Now;
            var root = context.Server.MapPath("~/logs");
            var dir = Path.Combine(root, date.Year.ToString(), date.Month.ToString(), date.Day.ToString()); 
            var path = Path.Combine(dir, string.Format("access-{0}.log", user));
            var logs = GetAuditLog(context);

            Task.Factory.StartNew(() =>
            { 
                Directory.CreateDirectory(dir); 
                File.AppendAllText(path, logs, Encoding.UTF8);
            });
        }

    }
}
