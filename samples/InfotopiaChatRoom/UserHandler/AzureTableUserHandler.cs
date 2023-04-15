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

            _userTable = _cloudTableClient.GetTableReference("TestUserTable");
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
            Console.WriteLine("----> SetUserStatus called");
            var retry = 0;
            const int MAX_RETRY = 10;

            while (retry < MAX_RETRY)
            {
                Console.WriteLine("----> SetUserStatus retry {0}",retry);
                try
                {
                    var retrieveOperation = TableOperation.Retrieve<UserEntity>(tenantId, userId);
                    var retrievedResult = await _userTable.ExecuteAsync(retrieveOperation);
                    var updateEntity = retrievedResult.Result as UserEntity;

                    if (updateEntity != null)//user already listed, we need to update
                    {
                        Console.WriteLine("----> SetUserStatus updating entry");
                        updateEntity.UpdateStatus(userStatus, connectionId);

                        var updateOperation = TableOperation.Replace(updateEntity);
                        await _userTable.ExecuteAsync(updateOperation);
                    }
                    else { //add a new user to the list
                        Console.WriteLine("----> SetUserStatus creating new entry");
                        var user = new User(userId, userStatus, connectionId);
                        var insertEntity = new UserEntity(tenantId, userId, user);

                        var insertOperation = TableOperation.Insert(insertEntity);
                        await _userTable.ExecuteAsync(insertOperation);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    if (++retry == MAX_RETRY) 
                    {
                        throw ex;
                    }

                    await Task.Delay(new Random().Next(10, 100));
                }
            }
            Console.WriteLine("----> SetUserStatus done");
        }

        public async Task<string> GetUserConnectionId(string tenantId, string userId){
            //old code but leaving here as a reference for combining queries
            //https://stackoverflow.com/a/18549818
            //string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, tenantId);
            //string rkFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId);
            //string filter = TableQuery.CombineFilters(pkFilter, TableOperators.And, rkFilter);
            //var query = new TableQuery<UserEntity>().Where(filter);
            //var result = await _userTable.ExecuteQuerySegmentedAsync<UserEntity>(query, null);
            
            //there can be at most 1 result (PK and RK combo are guaranteed to be unique by design)
            //if the user is not registered, we get null and return an empty string
            //If the used is offline, the database should contain an empty string too.
            var retrieveOperation = TableOperation.Retrieve<UserEntity>(tenantId, userId);
            var retrievedResult = await _userTable.ExecuteAsync(retrieveOperation);
            var entity = retrievedResult.Result as UserEntity;
            
            if (entity!=null) {
                return entity.ConnectionId;
            }
            else {
                return "";
            }
            
        }
    }
}
