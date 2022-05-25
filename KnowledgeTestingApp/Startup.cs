using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using KnowledgeTestingApp.Models;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;

namespace KnowledgeTestingApp {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }


    public void ConfigureServices(IServiceCollection services) {
      //services.Configure<FormOptions>(options => {
      //  options.ValueCountLimit = 10;
      //  options.ValueLengthLimit = int.MaxValue;
      //  options.MultipartBodyLengthLimit = long.MaxValue;
      //  options.MemoryBufferThreshold = Int32.MaxValue;
      //});
      //services.AddMvc(options=> { options.MaxModelBindingCollectionSize = int.MaxValue; });
      services.AddControllersWithViews();
      services.AddDbContext<Context>
        (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      services.AddSession();
      services.AddDistributedMemoryCache();
      services.Configure<MvcViewOptions>(options => {
        options.HtmlHelperOptions.CheckBoxHiddenInputRenderMode = Microsoft.AspNetCore.Mvc.Rendering.
        CheckBoxHiddenInputRenderMode.None;
      });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      app.UseDeveloperExceptionPage();
      //if (env.IsDevelopment()) {
      //} else {
      //  app.UseExceptionHandler("/Home/Error");
      //  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      //  app.UseHsts();
      //}
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSession();
      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=User}/{action=Login}/{id?}");
      });
    }
  }
}
