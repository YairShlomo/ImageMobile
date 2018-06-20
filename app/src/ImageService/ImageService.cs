using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Server;
using ImageService.Modal;
using ImageService.Controller;
using ImageService.Logging;
using ImageService.Communication;
using ImageService.Infrastructure.Modal;
//using System.Security.Permissions;[assembly: PermissionSetAttribute(SecurityAction.RequestMinimum, Name="Internet")]
//[assembly: PermissionSetAttribute(SecurityAction.RequestOptional, Unrestricted=true)]
namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };
    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer m_imageServer;
        private IImageServiceModal modal;
        private IImageController controller;
        private ILoggingService logging;
        private ISServer server;
      //  private Debug_program debug;

        public ImageService(string[] args)
        {
            //debug = new Debug_program();
            //debug.write("constructor ImageService");
            

            InitializeComponent();
            string eventSourceName = "MySource";
            string logName = "MyNewLog";
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        protected override void OnStart(string[] args)
        {
            //eventLog1.WriteEntry("In OnStart");

            //debug.write("onstart ImageService");
            modal = new ImageServiceModal();            
            logging = new LoggingService();
            /*
            if (logging != null)
            {
                logging.InvokeUpdateEvent("In OnStart", MessageTypeEnum.INFO);
            }
            */
            controller = new ImageController(modal,logging);
            m_imageServer = new ImageServer(logging, controller);
            controller.ImageServer = m_imageServer;
           
            StartCommunication();
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
            //server.Stop();
           // m_imageServer.CloseAll();
        }
        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
        protected void StartCommunication()
        {
            ClientHandler client = new ClientHandler(controller, logging);
            server = new ISServer(8000, logging, client);
            ImageServer.NotifyAllHandlerRemoved += server.NotifyAllClientsAboutUpdate;
            logging.UpdateLogs += server.NotifyAllClientsAboutUpdate;

            server.Start();
        }

    }
}
