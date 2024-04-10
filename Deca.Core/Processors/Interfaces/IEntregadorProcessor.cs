using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors.Interfaces
{
    public interface IEntregadorProcessor
    {
        Task CadastrarNovoEntregador(EntregadorModel request);
        Task UpdateCNHImageCadastrada(UserModel user, string caminhoImagem);
    }
}
