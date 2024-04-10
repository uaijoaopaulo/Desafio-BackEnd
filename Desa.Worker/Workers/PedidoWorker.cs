using Desa.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Worker.Workers
{
    public class PedidoWorker : BackgroundService
    {

        private int Delay = 1000;
        private IPedidoService _service;

        public PedidoWorker(IPedidoService service)
        {
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    Console.WriteLine("PedidoWorker - running");
                    await _service.DoAsync();
                    await Task.Delay(Delay);
                }
                catch (Exception)
                {
                    Console.WriteLine("PedidoWorker - stopping");
                }
            } while (!(stoppingToken.IsCancellationRequested));
        }
    }
}
