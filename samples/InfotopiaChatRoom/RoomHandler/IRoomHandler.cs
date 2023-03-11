// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public interface IRoomHandler
    {
        /// <summary>
        /// Fetches a user's list of rooms
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>A list of Rooms for this user</returns>
        Task<List<Room>> GetUserRooms(string userId);

        /// <summary>
        /// Creates a new room and subscribes multiple members to it.
        /// Each member's membership should be described using a separate Room object
        /// </summary>
        /// <param name="roomMemberships"></param>
        /// <returns>the ID of the newly created room</returns>
        Task<string> CreateRoom(List<Room> roomMemberships);

        /// <summary>
        /// Subscribes a user to an existing room
        /// The member's membership should be described using a Room object
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="room"></param>
        Task AddUserToGroup(string userId, Room room);

        /// <summary>
        /// Creates a new room and subscribes multiple members to it.
        /// Each member's membership should be described using a separate Room object
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        Task RemoveUserFromGroup(string userId, string roomId);

        /// <summary>
        /// Creates a new room and subscribes multiple members to it.
        /// Each member's membership should be described using a separate Room object
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        /// <param name="sequenceId"></param>
        Task SetLastReadMessage(string userId, string roomId, string sequenceId);
    }
}
