// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public class User
    {
        public string UserId { get; }

        // "online" or "offline"
        public string UserStatus { get; set; }

        public User(string userId, string userStatus)
        {
            UserId = userId;
            UserStatus = userStatus;
        }

    }
}
