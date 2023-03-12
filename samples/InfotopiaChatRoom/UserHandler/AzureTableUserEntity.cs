
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{

    
    public class UserEntity : TableEntity
    {
        //TODO check if this should be here, or read from the RowKey
        //public string UserId { get; }

        // "online" or "offline"
        public string UserStatus { get; set; }

        public string ConnectionId { get; set; }

        public UserEntity() {}

        //Partition Key: Tenant ID
        //Row Key: User ID
        public UserEntity(string pkey, string rkey, User user){
            PartitionKey = pkey;
            RowKey = rkey;
            UserStatus = user.UserStatus;
            ConnectionId = user.ConnectionId;
        }

        public User ToUser() {
            return new User(RowKey, UserStatus, ConnectionId);
        }
    }
}
