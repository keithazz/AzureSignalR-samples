// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class AzureTableUserStorage : IUserHandler
    {
        private readonly CloudStorageAccount _storageAccount;

        private readonly CloudTableClient _cloudTableClient;

        private readonly CloudTable _userTable;

        private readonly IConfiguration _configuration;

        public AzureTableUserStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureStorage"));

            _cloudTableClient = _storageAccount.CreateCloudTableClient();

            _userTable = _cloudTableClient.GetTableReference("UserTable");
            _userTable.CreateIfNotExistsAsync();
        }

        public Task<List<User>> GetUsers(string tenantId){
            
        }

        public Task SetUserStatus(string tenantId, string userId, string userStatus, string connectionId){
            
        }

        public Task<string> GetUserConnectionId(string tenantId, string userId){
            
        }
    }
}
