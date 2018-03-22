using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Management;

namespace CarputerWPF
{
    public class Temperature
    {
        public static string GetTemp()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = false;
            computer.Accept(updateVisitor);
            string result = string.Empty;
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature && result == string.Empty)
                            result = computer.Hardware[i].Sensors[j].Value.ToString();
                    }
                }
            }
            if (result == string.Empty)
            {
                const byte TEMPERATURE_ATTRIBUTE = 194;
                List<object> retval = new List<object>();
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData");
                    //loop through all the hard disks
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        byte[] arrVendorSpecific = (byte[])queryObj.GetPropertyValue("VendorSpecific");
                        //Find the temperature attribute
                        int tempIndex = Array.IndexOf(arrVendorSpecific, TEMPERATURE_ATTRIBUTE);
                        retval.Add(arrVendorSpecific[tempIndex + 5]);
                    }
                }
                catch (ManagementException err)
                {
                    result = err.Message;
                    result = "N/A";
                }
                if (retval.Count > 0)
                {
                    foreach (var ret in retval)
                    {
                        if (int.Parse(ret?.ToString()) > 0)
                        {
                            result = ret.ToString();
                        }
                    }
                }
                if (result == string.Empty)
                    result = "N/A";

            }
            computer.Close();
            return result;
        }
    }
}