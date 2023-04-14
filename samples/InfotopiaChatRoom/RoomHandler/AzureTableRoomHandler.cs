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

            _roomTable = _cloudTableClient.GetTableReference("TestRoomTable");
            _roomTable.CreateIfNotExistsAsync();
        }

        public async Task<List<Room>> GetUserRooms(string userId){

            var query = new TableQuery<RoomEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId)
            );
            var result = await _roomTable.ExecuteQuerySegmentedAsync<RoomEntity>(query, null);

            List<Room> rooms = new List<Room>();
            foreach (var entity in result)
            {
                rooms.Add(entity.ToRoom());
            }

            return rooms;
        }

        public async Task<string> CreateRoom(List<string> userIds,List<Room> roomMemberships){
            //TODO this isn't really safe ... we can get clashes
            string newRoomId = Guid.NewGuid().ToString();

            for (int i = 0; i < userIds.Count; i++) 
            {
                RoomEntity entity = new RoomEntity(userIds[i], newRoomId, roomMemberships[i]);
                TableOperation insertOperation = TableOperation.Insert(entity);
                var task = await _roomTable.ExecuteAsync(insertOperation);//TODO batch instead of loop?
            }

            return newRoomId;
        }

        public async Task AddUserToGroup(string userId, Room room){
            RoomEntity entity = new RoomEntity(userId, room.RoomId, room);
            TableOperation insertOperation = TableOperation.Insert(entity);
            await _roomTable.ExecuteAsync(insertOperation);
        }

        public async Task RemoveUserFromGroup(string userId, string roomId){
            RoomEntity entity = new RoomEntity(userId, roomId);
            TableOperation deleteOperation = TableOperation.Delete(entity);
            await _roomTable.ExecuteAsync(deleteOperation);
        }

        public async Task SetLastReadMessage(string userId, string roomId, string sequenceId){
            var retry = 0;
            const int MAX_RETRY = 10;

            while (retry < MAX_RETRY)
            {
                try
                {
                    var retrieveOperation = TableOperation.Retrieve<RoomEntity>(userId, roomId);
                    var retrievedResult = await _roomTable.ExecuteAsync(retrieveOperation);
                    var updateEntity = retrievedResult.Result as RoomEntity;

                    if (updateEntity != null)
                    {
                        updateEntity.UpdateLastReadMessage(sequenceId);

                        var updateOperation = TableOperation.Replace(updateEntity);
                        await _roomTable.ExecuteAsync(updateOperation);
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
        }
    }
}
