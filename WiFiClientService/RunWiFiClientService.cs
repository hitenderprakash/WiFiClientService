using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace WiFiClientService
{
    public partial class RunWiFiClientService : ServiceBase
    {
        bool connected;
        WlanClient client;
        string location = "C:/WiFi-Profile-XMLs"; // Can be read from registry entry or config file
        Thread ClientThread;
        public string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }
        public bool HasConnection()
        {
            try
            {
                System.Net.IPHostEntry i = System.Net.Dns.GetHostEntry("www.google.com");
                //System.Console.Write("\nSystem has Internet Connection");
                return true;
            }
            catch
            {
                //System.Console.Write("\nSystem doed not have internet connection");
                return false;
            }
        }
        //*******************************Main activity is controlled by this code********************
        public void MainActivity()
        {
            Wlan.WlanAvailableNetwork[] sortedNetworks;
            WlanClient.WlanInterface[] wlanIface;
            while (true)
            {
                this.connected = false;
                this.client = new WlanClient();
                wlanIface = client.Interfaces;
                foreach (var iface in wlanIface)
                {
                    Wlan.WlanAvailableNetwork[] Networks = iface.GetAvailableNetworkList(0);
                    sortedNetworks = Networks.OrderByDescending(x => x.wlanSignalQuality).ToArray();
                    foreach (Wlan.WlanAvailableNetwork network in sortedNetworks)
                    {
                        //Console.Write("\nSearching for available profiles..");
                        Wlan.Dot11Ssid ssid = network.dot11Ssid;
                        string networkName = (this.GetStringForSSID(ssid));
                        //Console.Write("\nStrongest Signal from: " + networkName);
                        string filename = this.location + "/" + networkName + ".xml";
                        string profileXML;
                        if (System.IO.File.Exists(filename))
                        {
                            profileXML = System.IO.File.ReadAllText(@filename);
                            //System.Console.Write(profileXML);

                            try
                            {
                                iface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXML, true);
                                iface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, networkName);
                                //Console.Write("\nConnection Successful..................");
                                //Console.Write("\nGoing To Stop for 10 Seconds....");
                                this.connected = true;
                                System.Threading.Thread.Sleep(10000);
                                //Console.Write("\nWait Over...");

                            }
                            catch
                            {
                                this.connected = false;
                            }
                        }
                        if (this.connected) { break; }
                    }
                    if (this.connected) { break; }
                } if (this.connected) { break; }
            }
        }
        //*******************************************************************
        public RunWiFiClientService()
        {
            InitializeComponent();
        }
        static void parallelFunc(RunWiFiClientService that)
        {
            that = new RunWiFiClientService();
            while (true)
            {
                if (that.HasConnection() != true)
                {
                    that.MainActivity();
                }
            }
        }
        protected override void OnStart(string[] args)
        {
            ClientThread = new Thread(() => parallelFunc(this));         // Kick off a new thread
            ClientThread.Start();   
        }
        protected override void OnStop()
        {
            ClientThread.Abort();
        }
       
        

    }
}
