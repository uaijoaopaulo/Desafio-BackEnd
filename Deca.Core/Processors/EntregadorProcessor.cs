using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Desa.Core.Exceptions;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.Extensions.Configuration;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Processors
{
    public class EntregadorProcessor : IEntregadorProcessor
    {
        protected readonly IEntregadorRepository _entregadorRepository;
        protected readonly IUserRepository _userRepository;

        public EntregadorProcessor(IEntregadorRepository entregadorRepository, IUserRepository userRepository)
        {
            _entregadorRepository = entregadorRepository;
            _userRepository = userRepository;
        }

        public async Task CadastrarNovoEntregador(EntregadorModel request)
        {
            try
            {
                request.CNHImage = await EnviarFotoS3Amazon(request.CNHImage, request.CNH);
                await _entregadorRepository.SQLInsert(request);

                var entregador = await _entregadorRepository.GetByCNH(request.CNH);
                await _userRepository.SQLInsert(new UserModel() { IdEntregador = entregador.Id, PublicKey = entregador.CNPJ, PublicToken = entregador.CNH, Admin = false });
            }
            catch (Exception e)
            {
                if(e.InnerException.Message.Contains("duplicate key"))
                    throw new DesaException(409, "One or more unique keys are conflicting");
                throw;
            }
        }

        public async Task UpdateCNHImageCadastrada(UserModel user, string caminhoImagem)
        {
            try
            {
                var entregador = await _entregadorRepository.SQLGetOneById(user.IdEntregador);
                entregador.CNHImage = await EnviarFotoS3Amazon(caminhoImagem, entregador.CNH);
                await _entregadorRepository.SQLUpdate(entregador);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<string> EnviarFotoS3Amazon(string caminhoImagem, string cnhNumber)
        {
            try
            {
                var configurationManager = (new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)).Build();

                string bucketName = configurationManager.GetSection("AmazonS3")["BucketName"];
                string accessKeyId = configurationManager.GetSection("AmazonS3")["AccessKeyId"];
                string secretAccessKey = configurationManager.GetSection("AmazonS3")["SecretAccessKey"];
                string region = configurationManager.GetSection("AmazonS3")["Region"];

                string fileName = string.Format("{0}-{1}",cnhNumber, Path.GetFileName(caminhoImagem));

                var s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region));
                var transferUtility = new TransferUtility(s3Client);
                await transferUtility.UploadAsync(caminhoImagem, bucketName, fileName);

                return $"https://{bucketName}.s3.{region}.amazonaws.com/{fileName}";
            }
            catch (Exception)
            {
                return caminhoImagem;
            }

        }
    }
}
