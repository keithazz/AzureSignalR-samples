// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class InfotopiaChatSampleHub : Hub
    {
        private readonly IMessageHandler _messageHandler;

        private readonly IRoomHandler _roomHandler;

        private readonly IUserHandler _userHandler;

        //TODO this might not be the best way of doing things, but we combine the tenant and user ID
        //maybe look into context items instead
        //https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.hubconnectioncontext.items?view=aspnetcore-7.0
        private string getUserId(){
            return Context.UserIdentifier.Split('-')[0];
        }
        private string getTenantId(){
            return Context.UserIdentifier.Split('-')[1];
        }

        public InfotopiaChatSampleHub(IMessageHandler messageHandler, IRoomHandler roomHandler, IUserHandler userHandler)
        {
            _messageHandler = messageHandler;
            _roomHandler = roomHandler;
            _userHandler = userHandler;
        }

        //use to add a user to a room group if they're online, or skip if they're offline
        private async Task AddUserToHubGroupIfOnlineAsync(string userId, string hubGroupName)
        {
            string connection = await _userHandler.GetUserConnectionId(getTenantId(), userId);
            //an empty string indicates that the user is not online
            if (connection!="") {
                await Groups.AddToGroupAsync(connection, hubGroupName);
            }

        }

        private async Task RemoveUserFromHubGroupIfOnlineAsync(string userId, string hubGroupName)
        {
            string connection = await _userHandler.GetUserConnectionId(getTenantId(), userId);
            //an empty string indicates that the user is not online
            if (connection!="") {
                await Groups.RemoveFromGroupAsync(connection, hubGroupName);
            }
        }


        public override async Task OnConnectedAsync()
        {
            //IDictionary<object,object> it = Context.Items;
            //Console.WriteLine("Context.Items={0}",Context.Items.Keys);
            Console.WriteLine("----> UserIdentifier={0}",Context.UserIdentifier);
            Console.WriteLine("----> userId={0}, tenantId={1}",getUserId(), getTenantId());
            await _userHandler.SetUserStatus(getTenantId(), getUserId(), "Online", Context.ConnectionId);
            string hubGroupName = "tenant-"+getTenantId();
            await Groups.AddToGroupAsync(Context.ConnectionId,hubGroupName);
            await Clients.Group(hubGroupName).SendAsync("NewStatusUpdate", getUserId(), "Online");
            
            
            //fetch the user's rooms, and subscribe to them
            List<Room> rooms = await _roomHandler.GetUserRooms(getUserId());
            foreach(var room in rooms) {
                await Groups.AddToGroupAsync(Context.ConnectionId, "room-"+room.RoomId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _userHandler.SetUserStatus(getTenantId(), getUserId(), "Offline", "");
            string hubGroupName = "tenant-"+getTenantId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,hubGroupName);
            await Clients.Group(hubGroupName).SendAsync("NewStatusUpdate", getUserId(), "Offline");

            await base.OnDisconnectedAsync(exception);
        }


        public async Task<string> CreatePrivateChatAsync(string userId, string otherUserId)
        {
            List<string> ids = new List<string>(){ userId, otherUserId};
            List<Room> memberships = new List<Room>(){
                new Room("", "0", "Private", "Admin", otherUserId),
                new Room("", "0", "Private", "Admin", userId), 
            };

            string newRoomId = await _roomHandler.CreateRoom(ids, memberships);
            string hubGroupName = "room-"+newRoomId;
            await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupName);
            await AddUserToHubGroupIfOnlineAsync(otherUserId, hubGroupName);
            await Clients.Group(hubGroupName).SendAsync("NewRoom", userId, newRoomId, "Private");
            Message message = new Message(userId, DateTime.Now, userId + " started the chat", "Information");
            await _messageHandler.AddNewMessage(newRoomId, message);
            await Clients.Group(hubGroupName).SendAsync("newMessage", message);

            return newRoomId;
        }

        
        public async Task<string> CreateGroupChatAsync(List<string> adminUserIds, List<string> memberUserIds, string chatName)
        {
            List<string> ids = new List<string>();
            List<Room> memberships = new List<Room>();
            foreach (var userId in adminUserIds)
            {
                ids.Add(userId);
                memberships.Add(new Room("","0","Group", "Admin", chatName));
            }
            foreach (var userId in memberUserIds)
            {
                ids.Add(userId);
                memberships.Add(new Room("","0","Group", "Member", chatName));
            }
            string newRoomId = await _roomHandler.CreateRoom(ids, memberships);
            string hubGroupName = "room-"+newRoomId;
            await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupName);
            //TODO will result in duplicate addition of current user
            foreach (var userId in adminUserIds.Concat(memberUserIds))
            {
                await AddUserToHubGroupIfOnlineAsync(userId, hubGroupName);
            }
            await Clients.Group(hubGroupName).SendAsync("NewRoom", getUserId(), newRoomId, "Group");
            Message message = new Message(getUserId(), DateTime.Now, getUserId() + " created the group", "Information");
            await _messageHandler.AddNewMessage(newRoomId, message);
            await Clients.Group(hubGroupName).SendAsync("newMessage", message);

            return newRoomId;
        }

        public async Task LeaveGroupAsync(string roomId)
        {
            //TODO check if the user is actually a member
            await _roomHandler.RemoveUserFromGroup(getUserId(), roomId);
            string hubGroupName = "room-"+roomId;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupName);
            Message message = new Message(
                                    getUserId(),
                                    DateTime.Now,
                                    getUserId() + " has left the group chat",
                                    "Information");
            await _messageHandler.AddNewMessage(roomId, message);
            await Clients.Group(hubGroupName).SendAsync("NewMessage", message);
        }

        public async Task AddUserToGroupAsync(string userId, string roomId, string roomName, string userRole)
        {
            //TODO ensure that user is an admin first
            //TODO check that user being added is not already a member
            Room roomMembership = new Room(roomId, "0", "Group", userRole, roomName);
            await _roomHandler.AddUserToGroup(userId, roomMembership);
            string hubGroupName = "room-"+roomId;
            await AddUserToHubGroupIfOnlineAsync(userId, hubGroupName);
            Message message = new Message(
                                    getUserId(),
                                    DateTime.Now,
                                    userId + " was added to the group chat",
                                    "Information");
            await _messageHandler.AddNewMessage(roomId, message);
            await Clients.Group(hubGroupName).SendAsync("NewMessage", message);
        }

        public async Task RemoveUserFromGroupAsync(string userId, string roomId)
        {
            //TODO ensure that user is an admin first
            //TODO ensure that user being removed is in the chat in the first place
            await _roomHandler.RemoveUserFromGroup(userId, roomId);
            string hubGroupName = "room-"+roomId;
            await RemoveUserFromHubGroupIfOnlineAsync(userId, hubGroupName);
            Message message = new Message(
                                    getUserId(),
                                    DateTime.Now,
                                    userId + " was kicked from the group chat",
                                    "Information");
            await _messageHandler.AddNewMessage(roomId, message);
            await Clients.Group(hubGroupName).SendAsync("NewMessage", message);
        }

        public async Task SendTextMessageAsync(string roomId, string senderId, string messageContent)
        {
            Message message = new Message(getUserId(), DateTime.Now, messageContent, "Text");
            string hubGroupName = "room-"+roomId;
            await _messageHandler.AddNewMessage(roomId, message);
            await Clients.Group(hubGroupName).SendAsync("NewMessage",message);
        } 

        public async Task MarkMessageAsReadAsync(string roomId, string sequenceId)
        {
            //update the last read message in the DB
            await _roomHandler.SetLastReadMessage(getUserId(), roomId, sequenceId);
            //inform users in the same room that the message has been read
            string hubGroupName = "room-"+roomId;
            await Clients.Group(hubGroupName).SendAsync("NewReadReceipt",getUserId(), roomId, sequenceId);
        }

        //returns a list of rooms and their last message, ordered chronologically
        public async Task<List<KeyValuePair<Room,Message>>> GetChatPreviewsAsync(string userId)
        {
            //fetch the list of rooms that a user is part of
            List<Room> rooms = await _roomHandler.GetUserRooms(getUserId());

            //fetch the last message for each room
            List<string> roomIds = rooms.Select(x => x.RoomId).ToList();
            Dictionary<string, Message> messageMap = await _messageHandler.GetLastMessageOfEachRoom(roomIds);
            
            //order the messages chronologically
            List<KeyValuePair<string, Message>> orderedMessages = messageMap.ToList();
            orderedMessages.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

            //using the message ordering, pair up the messages and rooms
            List<KeyValuePair<Room,Message>> preview = new List<KeyValuePair<Room,Message>>();
            foreach(var message in orderedMessages) {
                Room room = rooms.First(r => r.RoomId == message.Key);
                KeyValuePair<Room,Message> pair = new KeyValuePair<Room,Message>(room, message.Value);
                preview.Add(pair);
            }
            return preview;
        }

        public async Task<List<Message>> GetChatMessagesAsync(string roomId)
        {
            List<Message> messages = await _messageHandler.GetRoomMessages(roomId);
            return messages;
        }

        public async Task<List<User>> GetUsersWithStatusesAsync()
        {
            List<User> users = await _userHandler.GetUsers(getTenantId());
            return users;
        }



    }
}
