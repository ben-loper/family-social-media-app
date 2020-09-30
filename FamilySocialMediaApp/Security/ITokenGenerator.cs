using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilySocialMediaApp.Security
{
    public interface ITokenGenerator
    {

        /// <summary>
        /// Generates a new authentication token.
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="role">The user's role</param>
        /// <returns></returns>
        string GenerateToken(string email, string firstName, string role);
    }
}
