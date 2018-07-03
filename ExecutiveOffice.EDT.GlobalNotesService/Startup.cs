using ExecutiveOffice.EDT.GlobalNotesService.Controllers;
using ExecutiveOffice.EDT.GlobalNotesService.Infrsatructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace ExecutiveOffice.EDT.GlobalNotesService
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
            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new CsvInputFormatter(new CsvFormatterOptions()));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/csv"));
            }).AddXmlSerializerFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Generate Random Data API", Version = "v1" });
            });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IGlobalNotesProcessor>(new GlobalNotesAsHtmlProcessor());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Generate Random Data API V1");
            });

            var redirectRootToSwagger = new RewriteOptions()
                .AddRedirect("^$", "swagger");
            app.UseRewriter(redirectRootToSwagger);
        }
    }
}
