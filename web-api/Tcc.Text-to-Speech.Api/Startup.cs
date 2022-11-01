using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Tcc.Text_to_Speech.Application;
using Tcc.Text_to_Speech.Application.Commands;
using Tcc.Text_to_Speech.Application.Interfaces;
using Tcc.Text_to_Speech.Domain;

namespace Tcc.Text_to_Speech.Api
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
			services.AddSingleton<Cache>();
			services.AddScoped<ITextToSpeechHandler, TextToSpeechCommandHandler>();

			services.AddTransient<ILiteDatabase>
			(
				(config) => 
				{ 
					var db = new LiteDatabase(@"Filename=audio.db;Connection=Shared");
					return db;
				}
			);

			services.AddScoped<AudioRepository>();

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tcc.Text_to_Speech.Api", Version = "v1" });
			});

			services.AddCors(options =>
			{
				options.AddPolicy("All", builder =>
				{
					builder.SetIsOriginAllowed(_ => true)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tcc.Text_to_Speech.Api v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseCors("All");

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
