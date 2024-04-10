using Desa.Core.Authentication.Basic;
using Desa.Core.Processors;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories;
using Desa.Core.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace Desa.Rest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(BasicAuthenticationDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = BasicAuthenticationDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                        Description = "Basic Authentication Header"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = BasicAuthenticationDefaults.AuthenticationScheme
                             }
                        },
                        new string[] { BasicAuthenticationDefaults.AuthenticationScheme }
                    }
                });
            });

            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                    BasicAuthenticationDefaults.AuthenticationScheme, null
                );

            builder.Services.AddScoped<IMotoRepository, MotoRepository>();
            builder.Services.AddScoped<IEntregadorRepository, EntregadorRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<INotificacaoPedidoRepository, NotificacaoPedidoRepository>();
            builder.Services.AddScoped<ISqsRepository, SqsRepository>();

            builder.Services.AddScoped<IMotoProcessor, MotoProcessor>();
            builder.Services.AddScoped<IEntregadorProcessor, EntregadorProcessor>();
            builder.Services.AddScoped<ILocacaoProcessor, LocacaoProcessor>();
            builder.Services.AddScoped<IPedidoProcessor, PedidoProcessor>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
