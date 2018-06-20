using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ImageService.Logging;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ImageService.Modal;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Modal;
using ImageService.Infrastructure.Modal.Event;
using System.Drawing;
using System.IO;
using System.Configuration;

namespace ImageService.Commands
{
    class SaveImageCommand : ICommand
    {
        private byte[] image;

        public SaveImageCommand()
        {
        }

        public string Execute(string[] args, out bool result, TcpClient client = null)
        {
            try
            {
                result = true;
                
                // Image image = byteArrayToImage(args[1]);
                return "false";


            }
            catch (Exception e)
            {
                result = false;
                return "false";
            }
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(mStream);
            }
        }
    }
}
