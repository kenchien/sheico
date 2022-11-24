using log4net;
using MaintainReport.Models;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace Common.Helper {
    public class NetworkHelper {

        /// <summary>
        /// get local ip address with network interface
        /// </summary>
        /// <param name="networkInterfaceType">無線用Wireless80211,乙太用Ethernet</param>
        /// <returns></returns>
        public string GetLocalIp(NetworkInterfaceType networkInterfaceType) {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
                if (item.NetworkInterfaceType == networkInterfaceType && item.OperationalStatus == OperationalStatus.Up) {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses) {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                            var output = ip.Address.ToString();
                            return output;
                        }
                    }
                }
            }
            return "";
        }


    }

}
