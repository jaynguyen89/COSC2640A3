using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COSC2640A3.Bindings;
using COSC2640A3.Models;

namespace COSC2640A3.Services.Interfaces {

    public interface IAuthenticationService {

        /// <summary>
        /// Returns null if error, otherwise returns the ID of the inserted account.
        /// </summary>
        Task<string> InsertToUserPool(RegistrationVM registration);

        /// <summary>
        /// Key == null if error, Key == false if request to AWS failed then Value holds the reason, Key == true then Value holds the authToken.
        /// </summary>
        Task<KeyValuePair<bool?, string>> Authenticate(string username, string password);
        
        /// <summary>
        /// Key == null if error then Value holds the message, Key == false if confirmation failed, otherwise Key == true.
        /// </summary>
        Task<KeyValuePair<bool?, string>> ConfirmUserInPool(string username, string code);
    }
}