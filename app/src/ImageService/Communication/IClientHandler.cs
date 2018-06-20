using System;
using System.Net.Sockets;
using System.Collections.Generic;
namespace ImageService.Communication

{

    public interface IClientHandler
    {
        Boolean IsRunning
        {
            get;
            set;
        }

        void HandleClient(TcpClient client, List<TcpClient> clients);
        //void send(object o, MessageRecievedEventArgs dirArgs);


    }
}
