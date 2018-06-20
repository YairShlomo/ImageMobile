using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ImageService.Infrastructure.Modal.Event;
using Newtonsoft.Json;

namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {
        private ImageServer m_imageServer;
        public CloseHandlerCommand(ImageServer imageServer)
        {
            this.m_imageServer = imageServer;

        }

        /// <summary>
        /// That function will execute the task of the command.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <param name="result"> tells if the command succeded or not.</param>
        /// <returns>command return a string describes the operartion of the command.</returns>
        public string Execute(string[] args, out bool result, TcpClient tcpClient)
        {
            //try
            // {
            result = true;
            if (args == null || args.Length == 0)
            {
                throw new Exception("Invalid args for deleting handler");
            }
            string HandlertoDelete = args[0];
            string[] directories = (ConfigurationManager.AppSettings.Get("Handler").Split(';'));
            StringBuilder newHandlers = new StringBuilder();
            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i] != HandlertoDelete)
                {
                    newHandlers.Append(directories[i] + ";");
                }
            }
            string updatededHandlers = (newHandlers.ToString()).TrimEnd(';');
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("Handler");
            config.AppSettings.Settings.Add("Handler", updatededHandlers);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            DirectoryCloseEventArgs directoryCloseEventArgs = new DirectoryCloseEventArgs(HandlertoDelete, null);
            m_imageServer.CloseHandler(this, directoryCloseEventArgs);
            string[] Args = { HandlertoDelete };
            //array[0] = HandlertoDelete;
            CommandRecievedEventArgs crea = new CommandRecievedEventArgs((int)CommandEnum.CloseHandlerCommand, Args, HandlertoDelete);
            ImageServer.NotifyAll(crea);
            return JsonConvert.SerializeObject(crea);
        }
        /*
        catch (Exception ex)
        {
            debug.write($"excption thrown closehandler" + ex.Message);
            string[] Args = {null,ex.Message };
            result = false;
            return null;
        }
        */
    }
}
