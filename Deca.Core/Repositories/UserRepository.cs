using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        const string CollectionName = "users";
        public override string GetCollectionName<T>()
        {
            return CollectionName;
        }

        public async Task<UserModel> ObtemUserPorAppTokenEAppKeyFromSQL(string AppToken, string AppKey)
        {
            return await SqlDatabase.Users.FirstOrDefaultAsync(_ => _.PublicToken.Equals(AppToken) && _.PublicKey.Equals(AppKey));
        }
    }
}
