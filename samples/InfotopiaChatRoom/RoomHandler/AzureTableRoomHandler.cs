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
    public class AzureTableRoomStorage : IRoomHandler
    {
        private readonly CloudStorageAccount _storageAccount;

        private readonly CloudTableClient _cloudTableClient;

        private readonly CloudTable _roomTable;

        private readonly IConfiguration _configuration;

        public AzureTableRoomStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureStorage"));

            _cloudTableClient = _storageAccount.CreateCloudTableClient();

            _roomTable = _cloudTableClient.GetTableReference("RoomTable");
            _roomTable.CreateIfNotExistsAsync();
        }

        public Task<List<Room>> GetUserRooms(string userId){

        }

        public Task<string> CreateRoom(List<string> userIds,List<Room> roomMemberships){

        }

        public Task AddUserToGroup(string userId, Room room){

        }

        public Task RemoveUserFromGroup(string userId, string roomId){

        }

        public Task SetLastReadMessage(string userId, string roomId, string sequenceId){

        }
    }
}
