using System;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Core.Infrastructure.Enum;
using Core.Infrastructure.Helper;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Core.Infrastructure.Extensions;
using Core.Entity.Entities;
using Core.Infrastructure.Http;

namespace Core.Api.Controllers
{
    public class BaseController : Controller
    {
        protected ILogger<dynamic> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly AppSettings _appSettings;
        protected readonly JwtIssuerOptions _jwtOptions;

        public BaseController(
            ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _appSettings = optionsAccessor.Value;
			_jwtOptions = jwtOptions.Value;
            _unitOfWork = unitOfWork;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public BaseController(ILogger<dynamic> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        protected int GetCurrentUserId()
        {
            ClaimsPrincipal currentUser = this.User;
            var nameIdentifier = currentUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return nameIdentifier.Value.ToSafeInt();
        }

        protected User GetCurrentUser()
        {
            return _unitOfWork.RepositoryR<User>().GetSingle(GetCurrentUserId());
        }

        protected JwtSecurityToken JwtDecode(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        }

        protected void JwtCheckSecurityStamp(string jwtToken)
        {
            var jwtDecode = JwtDecode(jwtToken);

			foreach (Claim claim in jwtDecode.Claims)
			{
                Console.WriteLine("Type: " + claim.Type);
				Console.WriteLine("Value: " + claim.Value);
				Console.WriteLine("ValueType: " + claim.ValueType);
			}
        }

        protected void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
