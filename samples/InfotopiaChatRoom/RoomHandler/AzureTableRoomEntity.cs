
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class RoomEntity : TableEntity
    {

        //TODO check if this should be here, or read from the RowKey
        //public string RoomId { get; }

        // the sequence ID of the last message that THIS LOGGED IN USER has read
        public string LastReadSequenceId { get; set; }

        // "Private" or "Group"
        public string RoomType { get; set; }

        // "Admin" or "Member"
        public string UserRole { get; set; }

        // the name which will be shown to the user in the chat list
        public string DisplayName { get; set; }
        
        //Partition Key: User ID
        //Row Key: Room ID
        public RoomEntity(string pkey, string rkey, Room room){
            PartitionKey = pkey;
            RowKey = rkey;
            LastReadSequenceId = room.LastReadSequenceId;
            RoomType = room.RoomType;
            UserRole = room.UserRole;
            DisplayName = room.DisplayName;
        }

        public Room ToRoom() {
            return new Room(RowKey,LastReadSequenceId,RoomType,UserRole,DisplayName);
        }
    }
}
