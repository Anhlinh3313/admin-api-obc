using System;
using System.Security.Cryptography;
using System.Text;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
	public class EncryptionService : BaseService, IEncryptionService
	{
        private readonly Encryption _encryption;

        public EncryptionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {
            _encryption = new Encryption();
        }

        public string CreateSalt()
		{
            return _encryption.CreateSalt();
		}

        public string CreateSalt(string value)
        {
            return _encryption.CreateSalt(new object[] { value, _appSettings.Salt });
        }

		public string EncryptPassword(string password, string securityStamp)
		{
            return _encryption.EncryptPassword(password, securityStamp);
		}

        public string HashSHA256(string value)
        {
            return _encryption.HashSHA256(value);
        }
	}
}
