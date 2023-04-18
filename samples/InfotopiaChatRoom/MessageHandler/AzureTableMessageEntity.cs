
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class MessageEntity : TableEntity
    {
        public string SenderId { get; set; }

        public string SendTime { get; set; }

        public string SequenceId { get; set; }

        public string MessageContent { get; set; }

        //for now, "TextMessage" or "Information"
        public string MessageType { get; set; }

        //TODO make use of this
        public bool IsDeleted { get; set; }

        public MessageEntity() {}

        //Partition Key: (Room ID)
        //Row Key: Sequence ID as DateTime
        public MessageEntity(string pkey, DateTime rkey, Message message){
            PartitionKey = pkey;
            //Azure Storage Table automatically orders each partition by row key,
            //and we want it to always return the latest messages first
            //https://stackoverflow.com/a/40594821
            //https://stackoverflow.com/a/36055173
            //TODO check that the time zone isn't messing us up
            RowKey = (DateTime.MaxValue.Ticks - rkey.Ticks).ToString("d19");
            SenderId = message.SenderId;
            SendTime = message.SendTime.ToString("yyyy/MM/dd-HH:mm:ss");
            SequenceId = message.SequenceId;
            MessageContent = message.MessageContent;
            MessageType = message.MessageType;
            IsDeleted = false;
        }

        public Message ToMessage() {
            return new Message(SenderId, SequenceId, DateTime.ParseExact(SendTime,"yyyy/MM/dd-HH:mm:ss", null), MessageContent, MessageType);
        }
    }

}
