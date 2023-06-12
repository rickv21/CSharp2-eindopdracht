using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GameProject.Utils
{

    public class NetworkingUtils
    {
        private static readonly String ipRegex = @"^(?:\d{1,3}\.){3}\d{1,3}$";

        /**
         * Retrieves the local IPv4 address of the machine.
         * Uses the Dns.GetHostEntry() method to obtain host information for the local machine.
         * Iterates through the list of IP addresses associated with the host.
         * Checks each IP address's AddressFamily property to find an IPv4 address.
         * Returns the IPv4 address as a string representation if found.
         * Throws an exception if no IPv4 address is found.
         */
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static Boolean CheckIfInputIsValidIp(string input)
        {
            return Regex.IsMatch(input, ipRegex);
        }
    }


}
