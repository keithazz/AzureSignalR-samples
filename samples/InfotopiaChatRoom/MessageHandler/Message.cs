// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Azure.SignalR.Samples.InfotopiaChatRoom
{
    public class Message : IComparable<Message>
    {
        public string SenderId { get; }

        public DateTime SendTime { get; }

        public string SequenceId { get; set; }

        public string MessageContent { get; set; }

        //for now, "TextMessage" or "Information"
        public string MessageType { get; set; }

        public Message(string senderId, string sequenceId, DateTime sendTime, string messageContent, string messageType)
        {
            SenderId = senderId;
            SequenceId = sequenceId;
            SendTime = sendTime;
            MessageContent = messageContent;
            MessageType = messageType;
        }

        public int CompareTo(Message message)
        {
            return SequenceId.CompareTo(message.SequenceId);
        }
    }
}
