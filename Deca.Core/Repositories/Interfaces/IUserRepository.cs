using Desa.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface IUserRepository : IRepositoryModel<UserModel>
    {
        Task<UserModel> ObtemUserPorAppTokenEAppKeyFromSQL(string AppToken, string AppKey);
    }
}
