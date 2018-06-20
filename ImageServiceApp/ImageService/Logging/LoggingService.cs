
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Modal;
using ImageService.Infrastructure.Modal.Event;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public ObservableCollection<Log> ListLog {get; set;}
        public event UpdateLog UpdateLogs;
        public LoggingService()
            {
            ListLog = new ObservableCollection<Log>();
            }
        /// <summary>
        /// Occurs when [message recieved].
        /// </summary>
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public void Log(string message, MessageTypeEnum type)
        {
            string s = Convert.ToString((int)type);
            ListLog.Add(new Log{ Type = (s), Message = message });
            MessageRecievedEventArgs eventArgs = new MessageRecievedEventArgs();
            eventArgs.Message = message;
            eventArgs.Status = type;

            MessageRecieved?.Invoke(this, eventArgs);
            InvokeUpdateEvent(message, type);

        }
        public void InvokeUpdateEvent(string message, MessageTypeEnum type)
        {
            Log log = new Log { Type = Enum.GetName(typeof(MessageTypeEnum), type), Message = message };
            string[] args = new string[2];

            // args[0] = EntryType, args[1] = Message
            args[0] = log.Type;
            args[1] = log.Message;
            CommandRecievedEventArgs crea = new CommandRecievedEventArgs((int)CommandEnum.AddLog, args, null);
            if (UpdateLogs != null)
            {
                UpdateLogs?.Invoke(crea);

            }
        }
    }

}
