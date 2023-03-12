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

        public async Task<List<User>> GetUsers(string tenantId){
            var query = new TableQuery<UserEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, tenantId)
            );
            var result = await _userTable.ExecuteQuerySegmentedAsync<UserEntity>(query, null);

            List<User> users = new List<User>();
            foreach (var entity in result)
            {
                users.Add(entity.ToUser());
            }

            return users;
        }

        public async Task SetUserStatus(string tenantId, string userId, string userStatus, string connectionId){

        }

        public async Task<string> GetUserConnectionId(string tenantId, string userId){
            //https://stackoverflow.com/a/18549818
            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, tenantId);
            string rkFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, tenantId);
            string filter = TableQuery.CombineFilters(pkFilter, TableOperators.And, rkFilter);
            var query = new TableQuery<UserEntity>().Where(filter);
            var result = await _userTable.ExecuteQuerySegmentedAsync<UserEntity>(query, null);
            
            //there can be at most 1 result (PK and RK combo are guaranteed to be unique by design)
            //if the user is not registered, this will throw an error, so we catch and return
            //an empty string. If the used is offline, the database should contain an empty string too.
            try
            {
                return result.ToList()[0].ConnectionId;
            }
            //TODO catch correct exception type
            catch (Exception)
            {
                return "";
            }
            
        }
    }
}
