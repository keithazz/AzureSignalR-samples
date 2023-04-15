// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Add a new message to the storage.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="senderId"></param>
        /// <param name="sendTime"></param>
        /// <param name="messageContent"></param>
        /// <param name="messageType"></param>
        /// <returns>A new message object with the sequence ID included</returns>
        Task<Message> AddNewMessage(string roomId, string senderId, DateTime sendTime, string messageContent, string messageType);

        //TODO paginate
        /// <summary>
        /// Fetches the messages for a given room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="startSequenceId"></param>
        /// <param name="endSequenceId"></param>
        /// <returns>A list of messages sorted by the sequenceId</returns>
        Task<List<Message>> GetRoomMessages(string roomId);

        /// <summary>
        /// Fetches the last message for each specified room
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns>A dictionary of <room ID, message> pairs</returns>
        Task<Dictionary<string,Message>> GetLastMessageOfEachRoom(List<string> roomIds);
    }
}
