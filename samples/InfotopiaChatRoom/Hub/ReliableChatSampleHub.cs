// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class InfotopiaChatSampleHub : Hub
    {
        private readonly IMessageHandler _messageHandler;

        private readonly IRoomHandler _roomHandler;

        private readonly IUserHandler _userHandler;

        public InfotopiaChatSampleHub(IMessageHandler messageHandler, IRoomHandler roomHandler, IUserHandler userHandler)
        {
            _messageHandler = messageHandler;
            _roomHandler = roomHandler;
            _userHandler = userHandler;
        }

        public override async Task OnConnectedAsync()
        {
            
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<string> CreatePrivateChatAsync(string firstUserId, string secondUserId)
        {

        }

        
        public async Task<string> CreateGroupAsync(List<string> adminUserIds, List<string> memberUserIds)
        {

        }

        public async Task LeaveGroupAsync(string userId, roomId)
        {
            
        }

        public async Task AddUserToGroupAsync(string userId, string roomId, string roomName, string userRole)
        {

        }

        public async Task RemoveUserFromGroupAsync(string userId, string roomId)
        {

        }

        public async Task<string> SendTextMessageAsync(string roomId, string senderId, string messageContent)
        {

        } 

        public async Task MarkMessageAsReadAsync(string userId, string roomId, string sequenceId)
        {

        }

        //returns a list of rooms and their last message, ordered chronologically
        public async Task<List<KeyValuePair<Room,Message>>> GetChatPreviewsAsync(string userId)
        {

        }

        public async Task<List<Message>> GetChatMessagesAsync(string userId, string roomId)
        {

        }

        public async Task<List<User>> GetUsersWithStatusesAsync(string tenantId)
        {

        }



    }
}
