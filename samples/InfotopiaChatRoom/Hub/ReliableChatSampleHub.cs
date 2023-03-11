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

        public async Task<string> createPrivateChat()
        {

        }

        
        public async Task<string> createGroup()
        {

        }

        public async Task leaveGroup()
        {
            
        }

        public async Task addUserToGroup()
        {

        }

        public async Task removeUserFromGroup()
        {

        }

        public async Task<string> sendMessage()
        {

        } 





    }
}
