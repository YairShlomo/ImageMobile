using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;
using ImageService.Logging;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;
        private ImageServer imageServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageController"/> class.
        /// </summary>
        /// <param name="modal">The modal.</param>
        public ImageController(IImageServiceModal modal, ILoggingService loggingService)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            CommandEnum y = CommandEnum.NewFileCommand;
            CommandEnum g = CommandEnum.GetConfigCommand;
            CommandEnum l = CommandEnum.LogCommand;
            //CommandEnum a = CommandEnum.AddLog;
            CommandEnum c = CommandEnum.CloseCommand;
            CommandEnum d = CommandEnum.DeleteFileCommand;
          //  CommandEnum cc = CommandEnum.CloseClient;
            commands = new Dictionary<int, ICommand>()
            {
                // For Now will contain NEW_FILE_COMMAND
                {(int)y, new NewFileCommand(m_modal) },
                 {(int)g, new GetConfigCommand()},
                 { (int)l, new LogCommand(loggingService)},
                 //{ (int)a, new AddLog(m_modal)},
                  {(int)c, new CloseCommand(m_modal)},
                  {(int)d, new DeleteFileCommand(m_modal)},

             //  { (int)ch, new CloseHandlerCommand(imageServer)}
                // { (int)cc, new CloseClient()}
             
        };
        }
        public ImageServer ImageServer
        {
            get
            {
                return imageServer;
            }
            set
            {
                this.imageServer = value;
                this.commands[((int)CommandEnum.CloseHandlerCommand)] = new CloseHandlerCommand(imageServer);

            }
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="commandID">The command identifier.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="resultSuccesful">if set to <c>true</c> [result succesful].</param>
        /// <returns></returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            ICommand commandObj = commands[commandID];
            return commandObj.Execute(args, out resultSuccesful);
        }
    }
}