using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace ImageService.Commands
{
    /// <summary>
    /// close command
    /// </summary>
    /// <seealso cref="ImageService.Commands.ICommand" />
    public class AddLogCommand : ICommand
    {
        private IImageServiceModal m_modal;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseCommand"/> class.
        /// </summary>
        /// <param name="modal">The modal.</param>
        public AddLogCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <returns></returns>
        public string Execute(string[] args, out bool result, TcpClient client = null)
        {
            // The String Will Return the New Path if result = true, and will return the error message
            result = true;
            return "AddLog";
        }
    }
}