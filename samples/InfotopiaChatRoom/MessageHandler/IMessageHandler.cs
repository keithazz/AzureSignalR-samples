// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Add a new message to the storage.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="message"></param>
        /// <returns>The sequenceId of the new message.</returns>
        Task<string> AddNewMessage(string roomId, Message message);

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
