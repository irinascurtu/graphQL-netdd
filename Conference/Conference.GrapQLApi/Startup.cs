using Conference.Api.Infrastructure;
using Conference.Data.Data.Repositories;
using Conference.Service;
using GraphQL.Api.Data.Infrastructure;
using GraphQL.Api.Data.Repositories;
using GraphQL.Api.GraphQL;
using GraphQL.Api.GraphQL.Mutations;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.Common;
using GraphQL.Server.Ui.Altair;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace Conference.GrapQLApi
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

            services.AddControllers();
            services.AddDbContext<ConferenceDbContext>(options =>
            {
                //    options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration["ConnectionStrings:ConferenceDb"]);
            });


            services.AddScoped<SpeakersRepository>();
            services.AddScoped<TalksRepository>();
            services.AddScoped<FeedbackService>();
            services.AddScoped<ConferenceQuery>();
            services.AddScoped<ConferenceMutation>();
            services.AddScoped<ConferenceSchema>();


            services.AddHttpClient("Feedbacks", client =>
            {
                client.BaseAddress = new Uri(Configuration["Feedbacks"]);
            });

            services.AddGraphQL(o =>
            {
                o.EnableMetrics = true;
            })
               .AddSystemTextJson(deserializerSettings => { }, serializerSettings => { }) // For .NET Core 3+
                .AddDataLoader()
                .AddWebSockets()
                .AddGraphTypes(typeof(ConferenceSchema), ServiceLifetime.Scoped);
            //.AddUserContextBuilder(context => context.User)

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conference.GrapQLApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference.GrapQLApi v1"));
            }


            app.UseGraphQL<ConferenceSchema>();


            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()
            {
            });//ui/playground
            app.UseGraphiQLServer(new GraphiQLOptions());// graphiql
            
            app.UseGraphQLAltair(new GraphQLAltairOptions
            {
                Path = "/ui/altair",
                GraphQLEndPoint = "/graphql",                
                Headers = new Dictionary<string, string>
                {
                    ["X-api-token"] = "130fh9823bd023hd892d0j238dh",
                }
            });//ui/altaier

            app.UseGraphQLVoyager();//ui/voyager

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
