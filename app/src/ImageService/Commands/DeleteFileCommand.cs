using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace ImageService.Commands
{
    public class DeleteFileCommand : ICommand
    {
        private IImageServiceModal m_modal;


        public DeleteFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }


        public string Execute(string[] args, out bool result, TcpClient client = null)
        {
            return m_modal.DeleteFile(args[0], out result);
          
        }
    }
}
