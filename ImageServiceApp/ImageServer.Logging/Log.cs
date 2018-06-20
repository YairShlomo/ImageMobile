using System;
using ImageService.Infrastructure.Modal;

namespace ImageService.Logging
{
    public class Log
    {
        private MessageTypeEnum type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get { return Enum.GetName(typeof(MessageTypeEnum), type); }
            set { type = (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), value); }
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

    }
}