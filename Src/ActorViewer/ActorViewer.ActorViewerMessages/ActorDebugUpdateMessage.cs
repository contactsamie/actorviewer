using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActorViewer.ActorViewerMessages
{
    public class ActorDebugUpdateMessage
    {
        public ActorDebugUpdateMessage(string actorSystemInformation, string sourceActor, string sourceDescription, string destinationActor, string destinationDescription, string messageType, object message, string messageFullType, object extraInformation, MessageNature messageNature)
        {
            SourceActor = sourceActor;
            SourceDescription = sourceDescription;
            DestinationActor = destinationActor;
            DestinationDescription = destinationDescription;
            MessageType = messageType;
            Message = message;
            MessageFullType = messageFullType;
            ExtraInformation = extraInformation;
            MessageNature = messageNature;
            ActorSystemInformation = actorSystemInformation;
            ReceivedOn = DateTime.UtcNow;
        }

        public string ActorSystemInformation { get; }
        public string SourceActor { get; }
        public string SourceDescription { get; }
        public string DestinationActor { get; }
        public string DestinationDescription { get; }
        public string MessageType { get; }
        public object Message { get; }
        public string MessageFullType { get; }
        public DateTime ReceivedOn { get; }
        public object ExtraInformation { get; }
        public MessageNature MessageNature { get; }

        public List<string> GetDestinationPath()
        {
            return GetPath(DestinationActor);
        }

        public List<string> GetSourcePath()
        {
            return GetPath(SourceActor);
        }

        private List<string> GetPath(string path)
        {
            return path.Split('/').ToList();
        }

        public override string ToString()
        {
            var propertyInfos = GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var info in propertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value);
            }
            return sb.ToString();
        }
    }
}