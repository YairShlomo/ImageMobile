using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ImageService.Controller;
using Newtonsoft.Json;
using ImageService.Server;
using ImageService.Modal;
using ImageService.Infrastructure.Enums;
using System.Configuration;
using ImageService.Logging;
using System.Threading;
using ImageService.Infrastructure.Modal.Event;
using ImageService.Infrastructure.Modal;
using System.Drawing;

namespace ImageService.Communication
{
    class ClientHandler : IClientHandler
    {
        IImageController imageController { get; set; }
        ILoggingService Logging { get; set; }
        object mutexWrite = new object();
        BinaryReader reader;
        BinaryWriter writer;
        private Debug_program debug = new Debug_program();

        /// <summary>
        /// ClientHandler constructor.
        /// </summary>
        /// <param name="imageController">IImageController obj</param>
        /// <param name="logging">ILoggingService obj</param>
        public ClientHandler(IImageController m_imageController, ILoggingService logging)//, ImageServer imageServer)
        {
            this.imageController = m_imageController;
            this.Logging = logging;
            this.Logging.MessageRecieved += send;
            Console.WriteLine("ClientHandlerconstructor");

        }
        private bool isRunning= false;
        public bool IsRunning  // read-write instance property
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
            }
        }

        public static Mutex Mutex { get; set; }
        /// <summary>
        /// HandleClient function.
        /// handles the client-server communication.
        /// </summary>
        /// <param name="client">specified client</param>
        /// <param name="clients">list of all current clients</param>
        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            try
            {
                new Task(() =>
                {

                    try
                    {
                        isRunning = true;
                        NetworkStream stream = client.GetStream();
                        reader = new BinaryReader(stream);
                        writer = new BinaryWriter(stream);

                        while (isRunning)
                        {
                            int lenName = reader.ReadInt32();
                            string name = Encoding.Default.GetString(reader.ReadBytes(lenName));
                            debug.write("got "+name+"\n");

                            int lenImage =reader.ReadInt32(); ;

                            byte[] imageBytes = reader.ReadBytes(lenImage);
                            debug.write("got " + "bytes"+ "\n");

                            Image img = byteArrayToImage(imageBytes);
                            string[] dirPaths = ConfigurationManager.AppSettings["Handler"].Split(';');

                            img.Save(name);

                            // String[] Args = { name }

                            //System.IO.File.WriteAllBytes(@"C:\image.bmp", imageBytes);

                            /*
                             * 
                            char[] chars = new char[imageBytes.Length / sizeof(char)];
                            System.Buffer.BlockCopy(imageBytes, 0, chars, 0, imageBytes.Length);
                            string str = new string(chars);
                            */
                            // PhotoPackage photo = new PhotoPackage(name, image);
                            // savePhoto?.invoke(this, photo);
                            // photo.photo?.Dispose();
                            /*
                              Console.WriteLine("chbefore reading");
                             string commandLine = reader.ReadString();
                             Logging.Log("ClientHandler got command: " + commandLine, MessageTypeEnum.INFO);
                             CommandRecievedEventArgs commandRecievedEventArgs = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(commandLine);
                             if (commandRecievedEventArgs.CommandID == (int)CommandEnum.CloseClient)
                             {
                                 clients.Remove(client);
                                 client.Close();
                                 isRunning = false;
                                 break;
                             }
                            // Console.WriteLine("Got command: {0}", commandLine);
                             bool r;
                             string result = imageController.ExecuteCommand((int)commandRecievedEventArgs.CommandID,
                             commandRecievedEventArgs.Args, out r);
                             //Console.WriteLine("chExecutedCommand"+ (int)commandRecievedEventArgs.CommandID);
                             // string result = handleCommand(commandRecievedEventArgs);
                             // Mutex.WaitOne();
                             lock (mutexWrite)
                             {
                                 writer.Write(result);
                             }
                             // Console.WriteLine("chsend");
                             //Mutex.ReleaseMutex();
                             */
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"excption thrown senderch" + e.Message);
                        clients.Remove(client);
                        Logging.Log(e.ToString(), MessageTypeEnum.ERROR);
                        client.Close();
                        isRunning = false;
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), MessageTypeEnum.ERROR);
                isRunning = false;

            }
        }
        public void send(object o, MessageRecievedEventArgs dirArgs)
        {
            try
            {
                if (isRunning)
                {
                    MessageTypeEnum s = dirArgs.Status;
                    CommandRecievedEventArgs cre;
                    string[] Args = { Convert.ToString((int)dirArgs.Status), dirArgs.Message };
                    if(dirArgs.Message.Contains("Add File"))
                        cre = new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand, Args, null);
                    else
                        cre = new CommandRecievedEventArgs((int)CommandEnum.AddLog, Args, null);
                    string jsonCommand = JsonConvert.SerializeObject(cre);
                    writer.Write(jsonCommand);
                }
            }
            catch (Exception e)
            {

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
