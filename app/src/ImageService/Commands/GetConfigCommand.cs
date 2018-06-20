using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;
using ImageService.Communication;
using ImageService.Infrastructure.Modal.Event;
using ImageService.Infrastructure.Enums;

namespace ImageService.Commands

{
    class GetConfigCommand : ICommand
    {
        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="client"></param>
        /// <returns></returns>
        public string Execute(string[] args,out bool result, TcpClient client = null)
        {
            try
            {
                result = true;
                TcpMessages tcpMessages = new TcpMessages();
                string[] dirPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
                int sizeDirPath = dirPaths.Length;
                
                tcpMessages.Args = new string[4+sizeDirPath];
                tcpMessages.Args[0] = ConfigurationManager.AppSettings.Get("OutputDir");
                tcpMessages.Args[1] = ConfigurationManager.AppSettings.Get("SourceName");
                tcpMessages.Args[2] = ConfigurationManager.AppSettings.Get("LogName");
                tcpMessages.Args[3] = ConfigurationManager.AppSettings.Get("ThumbnailSize");
                for (int i=0; i< sizeDirPath;i++)
                {
                    tcpMessages.Args[4 + i] = dirPaths[i];
                }
                string tcpMessagesJson = JsonConvert.SerializeObject(tcpMessages);
                string[] Args = { tcpMessagesJson };
                CommandRecievedEventArgs crea = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, Args, "");
                string commandSerialized = JsonConvert.SerializeObject(crea);
                return commandSerialized;
            }
            catch (Exception ex)
            {
                result = false;
                return ex.ToString();
            }
        }
    }
}

