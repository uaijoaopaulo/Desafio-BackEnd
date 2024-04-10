using Desa.Core.Processors.Interfaces;
using Desa.Core.Processors;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories;
using Desa.Worker.Workers;
using Desa.Worker.Services;
using Desa.Worker.Services.Interfaces;

namespace Desa.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<IPedidoService, PedidoService>();

            builder.Services.AddSingleton<IEntregadorRepository, EntregadorRepository>();
            builder.Services.AddSingleton<ILocacaoRepository, LocacaoRepository>();
            builder.Services.AddSingleton<INotificacaoPedidoRepository, NotificacaoPedidoRepository>();
            builder.Services.AddSingleton<IPedidoRepository, PedidoRepository>();
            builder.Services.AddSingleton<ISqsRepository, SqsRepository>();

            builder.Services.AddSingleton<IPedidoProcessor, PedidoProcessor>();

            builder.Services.AddHostedService<PedidoWorker>();

            var host = builder.Build();
            host.Run();
        }
    }
}