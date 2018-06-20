using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;
using System.Collections.ObjectModel;
using ImageService.Infrastructure.Modal;
using ImageService.Infrastructure.Modal.Event;

namespace ImageService.Logging
{
    public delegate void UpdateLog(CommandRecievedEventArgs updateObj);
    public interface ILoggingService
    {
         ObservableCollection<Log> ListLog { get; set; }

        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        void Log(string message, MessageTypeEnum type);           // Logging the Message
        event UpdateLog UpdateLogs;  //Invoked everytime a new event log entry is written to the log
        void InvokeUpdateEvent(string message, MessageTypeEnum type);
    }

}
