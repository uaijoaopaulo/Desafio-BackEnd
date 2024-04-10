using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors.Interfaces
{
    public interface IMotoProcessor
    {
        Task<MotoModel> GetByLicensePlate(string licensePlate);
        Task<IEnumerable<MotoModel>> GetAll();
        Task<IEnumerable<MotoModel>> GetAll(int offset, int limit);
        Task CadastrarNovaMoto(MotoModel moto);
        Task UpdateMoto(MotoModel moto);
        Task RemoveMoto(string licensePlate);
    }
}
