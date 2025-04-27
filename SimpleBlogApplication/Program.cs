using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SimpleBlogApplication.BLL.Services;
using SimpleBlogApplication.DAL.Data;
using SimpleBlogApplication.DAL.Models;
using SimpleBlogApplication.DAL.Repositories;

namespace SimpleBlogApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddScoped<PostRepository>();
            builder.Services.AddScoped<PostService>();

            builder.Services.AddScoped<ReactionRepository>();
            builder.Services.AddScoped<ReactionService>();
            
            builder.Services.AddScoped<CommentRepository>();
            builder.Services.AddScoped<CommentService>();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)/*MinimumLevel.Information().WriteTo.Console()*/.CreateLogger();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
