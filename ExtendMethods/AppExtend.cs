using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.ExtendMethods
{
    public static class AppExtend
    {
        public static void AddStatusCode(this IApplicationBuilder app){
app.UseStatusCodePages(err => {
                err.Run(async context => {
                    var respon = context.Response;
                    var code = respon.StatusCode;

                    var content = $@"<html>
<head>
<meta charset = 'UTF-8' />
<title> Lỗi {code}</title>
<body>
<p style =' color: red; font-size: 30px; '> Có lỗi xảy ra: {code} - {(HttpStatusCode)code}
</p>
</body>
</head>";
                    await respon.WriteAsync(content);
                }); 
            });//Code 400 - 599

        }
    }
}