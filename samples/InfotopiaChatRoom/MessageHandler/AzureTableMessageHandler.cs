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

            _messageTable = _cloudTableClient.GetTableReference("MessageTable");
            _messageTable.CreateIfNotExistsAsync();
        }

        public Task<string> AddNewMessageAsync(string roomId, Message message){
            
        }

        //TODO paginate
        public Task<List<Message>> GetRoomMessages(string roomId){
            
        }

        public Task<Dictionary<string,Message>> GetLastMessageOfEachRoom(List<string> roomIds){
            
        }
    }
}
