using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Http;
using WebApplication1.ExtendMethods;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   
            // Đăng ký dịch vụ email
            services.AddOptions();
            var mailsetting = _configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddControllersWithViews();

            services.AddRazorPages();

            // Cấu hình các view tại thư mục khác
            services.Configure<RazorViewEngineOptions>(options => {
                // /View/Controller/Action.cshtml
                // /MyView/Controller/Action.cshtml
                
                // {0} -> ten Action
                // {1} -> ten Controller
                // {2} -> ten Area
                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

                options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/{1}/{0}.cshtml");

            });

            // Đăng ký dịch vụ kết nối Sql Server
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectString = _configuration.GetConnectionString("AppContext");
                options.UseSqlServer(connectString);
            }); 

            // Dang ky Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất


                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;

            });

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            }); 

            services.AddAuthentication()
                    .AddGoogle(options => {
                        var gconfig = _configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        // https://localhost:5001/signin-google
                        options.CallbackPath = "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options => {
                        var fconfig = _configuration.GetSection("Authentication:Facebook");
                        options.AppId = fconfig["AppId"];
                        options.AppSecret = fconfig["AppSecret"];
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    });
                    // .AddTwitter()
                    // .AddMicrosoftAccount()

                    // Dịch vụ thông báo lỗi Identity
                    services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

                    // Đăng ký dịch vụ mới
                    services.AddAuthorization(option => {
                        option.AddPolicy("ViewMenu", builder =>
                        {
                            builder.RequireAuthenticatedUser();
                            builder.RequireRole(RoleName.Administrator);
                        });
                    });
                }
            //services.Configure<RazorViewEngineOption>(option => { });

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //Xác định danh tính
            app.UseAuthorization(); // Xác định quyền truy cập

            app.AddStatusCode(); //Tùy biến lỗi 400 - 599

            app.UseEndpoints(endpoints =>
            {// /sayhi
                endpoints.MapGet("/sayhi", async (context) => {
                    await context.Response.WriteAsync($"Hello ASP.NET MVC {DateTime.Now}");
                });endpoints.MapControllers();
 
                endpoints.MapControllerRoute(
                    name: "first",
                    pattern: "{url:regex(^((xemsanpham)|(viewproduct))$)}/{id:range(2,4)}", 
                    defaults: new {
                        controller = "First",
                        action = "ViewProduct"
                    }

                );

                endpoints.MapAreaControllerRoute(
                    name: "account",
                    pattern: "/Account/{controller}/{action=Index}/{id?}",
                    areaName: "Identity"
                );
                endpoints.MapAreaControllerRoute(
                    name: "role",
                    pattern: "/Role/{controller}/{action=Index}/{id?}",
                    areaName: "Identity"
                );
                endpoints.MapAreaControllerRoute(
                    name: "manager",
                    pattern: "/Member/{controller}/{action=Index}/{id?}",
                    areaName: "Identity"
                );
                endpoints.MapAreaControllerRoute(
                    name: "user",
                    pattern: "/ManageUser/{controller}/{action=Index}/{id?}",
                    areaName: "Identity"
                );
                
                // Controller khong co Area
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapRazorPages();
            });
        }
    }
}
