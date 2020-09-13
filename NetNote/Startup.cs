using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using NetNote.Models;
using NetNote.Repository;
using NetNote.Middleware;
using Microsoft.AspNetCore.Identity;

namespace NetNote
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddDbContext<NoteContext>(options =>options.UseSqlite(Configuration.GetConnectionString("NetNote")));
            services.AddIdentity<NoteUser, IdentityRole>()
                .AddEntityFrameworkStores<NoteContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<INoteTypeRepository,NoteTypeRepository>();
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
            });
            services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.LoginPath = "/Login";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitData(app.ApplicationServices);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //app.UseBasicMiddleware(new BasicUser { UserName = "admin", Password = "123456" });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }

        private void InitData(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //获取注入的NoteContext
                var context = serviceScope.ServiceProvider.GetRequiredService<NoteContext>();
                context.Database.EnsureCreated();//如果数据库不存在则创建，存在则不做操作
                if (!context.NoteTypes.Any())//不存在类型则添加
                {
                    var notetypes = new List<NoteType>{
                        new NoteType{ Name="日常记录"},
                        new NoteType{ Name="代码收藏"},
                        new NoteType{ Name="消费记录"},
                        new NoteType{ Name="网站收藏"}
                    };
                    context.NoteTypes.AddRange(notetypes);
                    context.SaveChanges();
                }
                if (!context.Users.Any()) //不存在用户，默认添加admin用户
                {
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<NoteUser>>();
                    var noteUser = new NoteUser { UserName = "admin", Email = "admin@dot.net" };
                    userManager.CreateAsync(noteUser, "admin123").Wait();
                }
            }
        }
    }
}
