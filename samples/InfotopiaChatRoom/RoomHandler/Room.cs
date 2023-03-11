// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.SignalR.Samples.ReliableChatRoom
{
    public class Room
    {
        public string RoomId { get; }

        // the sequence ID of the last message that THIS LOGGED IN USER has read
        public string LastReadSequenceId { get; set; }

        // "Private" or "Group"
        public string RoomType { get; set; }

        // the name which will be shown to the user in the chat list
        public string DisplayName { get; set; }

        public Room(string roomId, string lastReadSequenceId, string roomType, string displayName)
        {
            RoomId = roomId;
            RoomType = roomType;
            LastReadSequenceId = lastReadSequenceId;
            DisplayName = displayName;
        }

    }
}
