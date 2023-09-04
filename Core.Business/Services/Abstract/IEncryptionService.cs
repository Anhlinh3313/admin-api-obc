using System;
namespace Core.Business.Services.Abstract
{
    public interface IEncryptionService
    {
		/// <summary>
		/// Creates a random salt
		/// </summary>
		/// <returns></returns>
		string CreateSalt();
		/// <summary>
		/// Creates a random salt
		/// </summary>
		/// <returns></returns>
		string CreateSalt(string code);
		/// <summary>
        /// Encrypts the password.
        /// </summary>
        /// <returns>The password.</returns>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
		string EncryptPassword(string userName, string passWord);
        /// <summary>
        /// Hashs the SHA 256.
        /// </summary>
        /// <returns>The SHA 256.</returns>
        /// <param name="value">Value.</param>
		string HashSHA256(string value);
    }
}
