using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using ImageService.Logging;
using System.Collections.Generic;
using ImageService.Infrastructure.Modal.Event;
using ImageService.Infrastructure.Modal;

using System.IO;
using Newtonsoft.Json;
using ImageService.Modal;
using System.Threading;

namespace ImageService.Communication
{
    class ISServer
    {
        ILoggingService Logging { get; set; }

        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        int Port { get; set; }
        TcpListener Listener { get; set; }
        IClientHandler Ch { get; set; }
        private List<TcpClient> clients = new List<TcpClient>();
        private static Mutex m_mutex = new Mutex();
        private Debug_program dp;

        public ISServer(int port, ILoggingService logging, IClientHandler ch)
        {
            this.port = port;
            this.Logging = logging;
            this.ch = ch;
            ClientHandler.Mutex = m_mutex;
            dp = new Debug_program();

        }
        public void Start()
        {
            try
            {


                IPEndPoint ep = new
                IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                listener = new TcpListener(ep);

                listener.Start();
                //Console.WriteLine("Waiting for connections...");
                Logging.Log("Waiting for client connections...", MessageTypeEnum.INFO);

                Task task = new Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            dp.write("listening");
                            TcpClient client = listener.AcceptTcpClient();
                            //client.Close;
                            Console.WriteLine("Got new connection");
                            dp.write("Got new connection\n");
                            clients.Add(client);
                            ch.HandleClient(client, clients);
                        }
                        catch (SocketException)
                        {
                            break;
                        }
                    }
                    Logging.Log("Server stopped", MessageTypeEnum.INFO);
                });
                task.Start();


            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), MessageTypeEnum.ERROR);
            }
        }

        public void NotifyClients(CommandRecievedEventArgs commandRecievedEventArgs)
        {
            try
            {
                foreach (TcpClient client in clients)
                {
                    new Task(() =>
                    {
                        using (NetworkStream stream = client.GetStream())
                        using (BinaryReader reader = new BinaryReader(stream))
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            string jsonCommand = JsonConvert.SerializeObject(commandRecievedEventArgs);
                            writer.Write(jsonCommand);
                        }
                        client.Close();
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), MessageTypeEnum.ERROR);

            }
        }
        public void NotifyAllClientsAboutUpdate(CommandRecievedEventArgs commandRecievedEventArgs)
        {
            try
            {
                List<TcpClient> copyClients = new List<TcpClient>(clients);
                foreach (TcpClient client in copyClients)
                {
                    new Task(() =>
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            BinaryWriter writer = new BinaryWriter(stream);
                            string jsonCommand = JsonConvert.SerializeObject(commandRecievedEventArgs);
                            m_mutex.WaitOne();
                            writer.Write(jsonCommand);
                            m_mutex.ReleaseMutex();
                        }
                        catch (Exception ex)
                        {
                            this.clients.Remove(client);
                        }

                    }).Start();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), MessageTypeEnum.ERROR);
            }
        }


        public void Stop()
        {
            listener.Stop();
            ch.IsRunning = false;
        }
    }
}