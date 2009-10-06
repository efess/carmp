using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;

namespace CarMp
{
    public class SystemInformation
    {
        public static List<StorageDrive> GetDeviceId()
        {
            List<StorageDrive> driveList = new List<StorageDrive>();
            ManagementClass mc = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                StorageDrive drive = new StorageDrive();

                drive.DriveType = (DriveType)(uint)mo["DriveType"];
                drive.DriveName = (string)mo["DeviceID"];
                drive.SerialNumber = (string)mo["VolumeSerialNumber"];
                drive.DriveName = (string)mo["VolumeName"];

                drive.DriveAvailability = mo["Availability"] == null || (byte)mo["Availability"] == 0x03; 

                driveList.Add(drive);
            }
            return driveList;
        }


        public class StorageDrive
        {
            public string DriveLetter;
            public string SerialNumber;
            public DriveType DriveType;
            public string DriveName;
            public bool DriveAvailability;
        }
    }
}
