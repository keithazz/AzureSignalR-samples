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

        private readonly ISessionHandler _sessionHandler;

        private readonly IUserHandler _userHandler;

        public InfotopiaChatSampleHub(IMessageHandler messageHandler, ISessionHandler sessionHandler, IUserHandler userHandler)
        {
            _messageHandler = messageHandler;
            _sessionHandler = sessionHandler;
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



    }
}
