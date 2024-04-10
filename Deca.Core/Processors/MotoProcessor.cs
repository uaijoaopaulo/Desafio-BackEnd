using Desa.Core.Exceptions;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors
{
    public class MotoProcessor : IMotoProcessor
    {
        protected readonly IMotoRepository _motoRepository;
        protected readonly ILocacaoRepository _locacaoRepository;
        public MotoProcessor(IMotoRepository motoRepository, IUserRepository userRepository, ILocacaoRepository locacaoRepository)
        {
            _motoRepository = motoRepository;
            _locacaoRepository = locacaoRepository;
        }

        public async Task<MotoModel> GetByLicensePlate(string licensePlate)
            => await _motoRepository.GetByLicensePlate(licensePlate);
        public async Task<IEnumerable<MotoModel>> GetAll()
            => await _motoRepository.SQLGetAll();
        public async Task<IEnumerable<MotoModel>> GetAll(int offset, int limit)
            => await _motoRepository.SQLGetAll(offset, limit);

        public async Task CadastrarNovaMoto(MotoModel moto)
        {
            try
            {
                await _motoRepository.SQLInsert(moto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateMoto(MotoModel moto)
        {
            try
            {
                await _motoRepository.SQLUpdate(moto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveMoto(string licensePlate)
        {
            try
            {
                var moto = await _motoRepository.GetByLicensePlate(licensePlate);
                var locacaoAtiva = _locacaoRepository.GetLocacaoAtivaByMoto(moto.Id);
                if (locacaoAtiva != null)
                    throw new DesaException(409, "It is not possible to delete a motorcycle while there is an active rental."); 
                await _motoRepository.SQLDelete(moto);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
