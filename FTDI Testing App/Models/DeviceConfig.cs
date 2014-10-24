using System;
using System.Security.Cryptography;
using FTD2XX_NET;

namespace FTDI_Testing_App.Models
{
    [Serializable]
    public class DeviceConfig
    {

        //Need to Finish this object to include all eeprom data (To be used a ser/deser for eeprom Flashing)
        public string SerialNumber { get; set; }
        public ushort VendorID { get; set; }
        public ushort ProductID { get; set; }
        public string Description { get; set; }
        public FTDI.FT_DEVICE Type { get; set; }
        public uint LocationID { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerID { get; set; }
        public ushort MaxPowerMa { get; set; }
        public bool SelfPowered { get; set; }
        public bool HiDrIOs { get; set; }
        public bool InvertTX { get; set; }
        public bool InvertRx { get; set; }
        public bool PullDownEnabled { get; set; }
        public bool SerialEnabled { get; set; }
        public bool UseExtOsc { get; set; }

        public bool Connected { get; set; }

        public FTDI.FT232R_EEPROM_STRUCTURE RawEEPROMStructure { get; set; }

        public void BuildFromEEPROMStruct(FTDI.FT232R_EEPROM_STRUCTURE eepromStruct)
        {
            RawEEPROMStructure = eepromStruct;

            SerialNumber = eepromStruct.SerialNumber;
            VendorID = eepromStruct.VendorID;
            ProductID = eepromStruct.ProductID;
            Description = eepromStruct.Description;
            Manufacturer = eepromStruct.Manufacturer;
            ManufacturerID = eepromStruct.ManufacturerID;
            MaxPowerMa = eepromStruct.MaxPower;
            SelfPowered = eepromStruct.SelfPowered;
            HiDrIOs = eepromStruct.HighDriveIOs;
            InvertTX = eepromStruct.InvertTXD;
            InvertRx = eepromStruct.InvertRXD;
            PullDownEnabled = eepromStruct.PullDownEnable;
            SerialEnabled = eepromStruct.SerNumEnable;
            UseExtOsc = eepromStruct.UseExtOsc;
        }

        public void WriteToFile(string fileName, string filePath = null)
        {
            
        }

    }
}
