// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class AzureTableMessageStorage : IMessageHandler
    {
        private readonly CloudStorageAccount _storageAccount;

        private readonly CloudTableClient _cloudTableClient;

        private readonly CloudTable _messageTable;

        private readonly IConfiguration _configuration;

        public AzureTableMessageStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureStorage"));

            _cloudTableClient = _storageAccount.CreateCloudTableClient();

            _messageTable = _cloudTableClient.GetTableReference("TestMessageTable");
            _messageTable.CreateIfNotExistsAsync();
        }

        public async Task<Message> AddNewMessage(string roomId, string senderId, DateTime sendTime, string messageContent, string messageType){
            string sequenceId = DateTime.Now.Ticks.ToString();
            Message message = new Message(
                senderId,
                sequenceId,
                sendTime,
                messageContent,
                messageType
            );
            MessageEntity entity = new MessageEntity(roomId, sequenceId, message);
            TableOperation insertOperation = TableOperation.Insert(entity);
            var task = await _messageTable.ExecuteAsync(insertOperation);
            return message;
        }

        //TODO paginate
        public async Task<List<Message>> GetRoomMessages(string roomId){
            var query = new TableQuery<MessageEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, roomId)
            );
            var result = await _messageTable.ExecuteQuerySegmentedAsync<MessageEntity>(query, null);

            List<Message> messages = new List<Message>();
            foreach (var entity in result)
            {
                messages.Add(entity.ToMessage());
            }

            return messages;
        }

        public async Task<Dictionary<string,Message>> GetLastMessageOfEachRoom(List<string> roomIds){

            //TODO check if this is the most efficient way of doing things
            //For now, we run 1 query per room with .Take(1) in an attempt to minimize document reads
            //We could also build a query to match the Partition Key only (using a combo of queries to mimic an IN statement)
            //and then filter for the last message for each partition manually.

            Dictionary<string,Message> messageMap = new Dictionary<string, Message>();
            foreach (var roomId in roomIds)
            {
                var query = new TableQuery<MessageEntity>()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, roomId)
                )
                //Azure Storage Table automatically orders each partition by row key, so this gives us the last message
                //TODO if this starts returning the first message, we'll need to store in reverse order
                //https://stackoverflow.com/a/40594821
                .Take(1);

                var result = await _messageTable.ExecuteQuerySegmentedAsync<MessageEntity>(query, null);
                
                //TODO is this the correct way to convert to a list and read the first entity?
                messageMap[roomId] = result.Results[0].ToMessage();
            }

            return messageMap;   
        }
    }
}
