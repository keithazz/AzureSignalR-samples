// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public interface IUserHandler
    {
        /// <summary>
        /// Fetches a list of users for the user's tenant,
        /// along with their statuses
        /// </summary>
        /// <param name="tenantId"></param>
        Task<List<User>> GetUsers(string tenantId);

        /// <summary>
        /// Sets the user's status
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        /// <param name="connectionId"></param>
        Task SetUserStatus(string tenantId, string userId, string userStatus, string connectionId);

        /// <summary>
        /// Get the connection ID of a currently connected user, or empty string if offline
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        Task<string> GetUserConnectionId(string tenantId, string userId);
    }
}
