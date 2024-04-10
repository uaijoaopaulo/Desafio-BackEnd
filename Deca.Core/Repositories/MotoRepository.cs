using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories
{
    public class MotoRepository : BaseRepository<MotoModel>, IMotoRepository
    {
        const string CollectionName = "motos";
        public override string GetCollectionName<MotoModel>()
        {
            return CollectionName;
        }

        public async Task<MotoModel> GetByLicensePlate(string LicensePlate)
        {
            return await SqlDatabase.Motos.FirstOrDefaultAsync(_ => _.LicensePlate.Equals(LicensePlate));
        }
    }
}
