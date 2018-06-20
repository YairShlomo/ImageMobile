using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public interface IImageController
    {
        ImageServer ImageServer
        {
            get;
            set;
        }
        string ExecuteCommand(int commandID, string[] args, out bool result);          // Executing the Command Requet
    }
}
