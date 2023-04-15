
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class MessageEntity : TableEntity
    {
        public string SenderId { get; set; }

        public string SendTime { get; set; }

        //TODO check if we should have this
        //public string SequenceId { get; set; }

        public string MessageContent { get; set; }

        //for now, "TextMessage" or "Information"
        public string MessageType { get; set; }

        public MessageEntity() {}

        //Partition Key: (Room ID)
        //Row Key: Sequence ID (Unix Timestamp)
        public MessageEntity(string pkey, string rkey, Message message){
            PartitionKey = pkey;
            RowKey = rkey;
            SenderId = message.SenderId;
            SendTime = message.SendTime.ToString("yyyy/MM/dd-HH:mm:ss");
            MessageContent = message.MessageContent;
            MessageType = message.MessageType;
        }

        public Message ToMessage() {
            return new Message(SenderId, RowKey, DateTime.ParseExact(SendTime,"yyyy/MM/dd-HH:mm:ss", null), MessageContent, MessageType);
        }
    }

}
