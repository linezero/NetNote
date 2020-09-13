using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace NetNote.Middleware
{
    public class BasicMiddleware
    {
        private readonly RequestDelegate _next;
        public const string AuthorizationHeader = "Authorization";
        public const string WWWAuthenticateHeader = "WWW-Authenticate";
        private BasicUser _user;
        public BasicMiddleware(RequestDelegate next, BasicUser user)
        {
            _next = next;
            _user = user;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var Request = httpContext.Request;
            string auth = Request.Headers[AuthorizationHeader];
            if (auth == null)
            {
                return BasicResult(httpContext);
            }
            //取得Base64并解码成字符串
            string[] authParts = auth.Split(' ');
            if (authParts.Length != 2)
                return BasicResult(httpContext);
            string base64 = authParts[1];
            string authValue;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                authValue = Encoding.ASCII.GetString(bytes);
            }
            catch
            {
                authValue = null;
            }
            if (string.IsNullOrEmpty(authValue))

                return BasicResult(httpContext);

            // 解析用户名和密码
            string userName;
            string password;
            int sepIndex = authValue.IndexOf(':');
            if (sepIndex == -1)
            {
                userName = authValue;
                password = string.Empty;
            }
            else
            {
                userName = authValue.Substring(0, sepIndex);
                password = authValue.Substring(sepIndex + 1);
            }
            //判断用户名和密码
            if (_user.UserName.Equals(userName) && _user.Password.Equals(password))
                return _next(httpContext);
            else
                return BasicResult(httpContext);
        }

        /// <summary>
        /// 返回需Basic认证输出
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static Task BasicResult(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Add(WWWAuthenticateHeader, "Basic realm=\"localhost\"");
            return Task.FromResult(httpContext);
        }
    }

    public static class BasicMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicMiddleware(this IApplicationBuilder builder, BasicUser user)
        {
            if (user == null)
                throw new ArgumentException("需设置Basic用户");
            return builder.UseMiddleware<BasicMiddleware>(user);
        }
    }

    public class BasicUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
