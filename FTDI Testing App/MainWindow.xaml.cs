using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FTD2XX_NET;
using FTDI_Testing_App.Models;

namespace FTDI_Testing_App
{

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObjectDataProvider commandList = new ObjectDataProvider();
        public DeviceConfig currentDeviceConfig = new DeviceConfig();

        private FTDI.FT_DEVICE expectedFTDIType = FTDI.FT_DEVICE.FT_DEVICE_232R;
        FTDI ftdi = new FTDI();


        //Binding CommandList = new Binding().Path = typeof (FTDI.FT_CBUS_OPTIONS);

        public MainWindow()
        {
            commandList.ObjectType = typeof(Enum);
            commandList.MethodName = "GetValues";
            //commandList.MethodParameters.Add(typeof (FTDI.FT232R_EEPROM_STRUCTURE));
            InitializeComponent();
        }


        

        public void GetDeviceInfo()
        {
            LogBox.AppendText(String.Format("Starting Testing.... \r"));
            LogBox.AppendText("\r");
            LogBox.AppendText("---------------------------\r");

            var getStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            uint ftdiDeviceCount = 0;
            getStatus = ftdi.GetNumberOfDevices(ref ftdiDeviceCount);
            var ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            
            if ((getStatus = ftdi.GetDeviceList(ftdiDeviceList)) == FTDI.FT_STATUS.FT_OK)
            {

                foreach (var device in ftdiDeviceList)
                {
                    LogBox.AppendText(String.Format("Status of Get Device List: {0}\r\rFTDI Device List:\rFlags: {1:x}\rType: {2}\rID: {3}\rLocation ID: {4}\rSerial Number: {5}\r", getStatus, device.Flags.ToString(), device.Type.ToString(), device.ID, device.LocId, device.SerialNumber));
                    currentDeviceConfig.Type = ftdiDeviceList[0].Type;
                    currentDeviceConfig.SerialNumber = ftdiDeviceList[0].SerialNumber;
                    currentDeviceConfig.LocationID = ftdiDeviceList[0].LocId;
                    SerialNumTestBox.Text = ftdiDeviceList[0].SerialNumber;
                }
                LogBox.AppendText("\r");
                LogBox.AppendText("---------------------------\r");
                
                
                
                LogBox.AppendText(String.Format("Number of FTDI Devices: {0} \rStatus of Get NumDevices: {1}\r", ftdiDeviceCount, getStatus));
                LogBox.AppendText("\r");
                SerialNumTestBox.Visibility = Visibility.Visible;
                ConnectButton.Visibility = Visibility.Visible;


            }
            else
            {
                LogBox.AppendText((String.Format("Error Pulling FTDI Device List\rError: {0}", getStatus)));
            }


            LogBox.AppendText("\r");
            LogBox.AppendText("---------------------------\r");
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogBox.Clear();
            GetDeviceInfo();
            PIDTextBox.Visibility = Visibility.Hidden;
            EnablePIDWriteCheckbox.Visibility = Visibility.Hidden;
            WritePIDButton.Visibility = Visibility.Hidden;
        }


        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LogBox.Clear();
            LogBox.Document.Blocks.Clear();

            var serial = SerialNumTestBox.Text;
            LogBox.AppendText("---------------------------\r");
            LogBox.AppendText(String.Format("Attempting to Connect to device: {0} \r", serial));
            var getStatus = ftdi.OpenBySerialNumber(serial);
            LogBox.AppendText(String.Format("Connection return status from device: {0} \r", getStatus));

            if (getStatus == FTDI.FT_STATUS.FT_OK)
            {
                LogBox.AppendText(String.Format("YAY Connected to Device: {0} \r\r", serial));
                LogBox.AppendText("---------------------------\r");
                currentDeviceConfig.Connected = true;
                DisconnectButton.Visibility = Visibility.Visible;
            }
            else
            {
                currentDeviceConfig.Connected = false;
                LogBox.AppendText(String.Format("Failed to connect to Device: {0} \r:( :( :( :( :(\r", serial));
                LogBox.AppendText("---------------------------\r");
                DisconnectButton.IsEnabled = false;
            }
            var comPort = "";
            getStatus = ftdi.GetCOMPort(out comPort);

            switch (getStatus)
            {
                    case FTDI.FT_STATUS.FT_OK:
                    LogBox.AppendText(String.Format("Device {0} COM Port: {1} \r", serial,comPort));
                    LogBox.AppendText("---------------------------\r");
                    LogBox.AppendText("---------------------------\r");
                    break;
                default:
                    LogBox.AppendText(String.Format("Error getting device {0} COM Prot\r", serial));
                    LogBox.AppendText("---------------------------\r");
                    LogBox.AppendText("---------------------------\r");
                    break;
            }
            eepromDumpCommand();

            LogBox.AppendText("---------------------------\r");

            PIDTextBox.IsEnabled = false;
            PIDTextBox.Text = currentDeviceConfig.ProductID.ToString("X");
            WritePIDButton.IsEnabled = false;

            if (currentDeviceConfig.Connected)
            {
                EnablePIDWriteCheckbox.Visibility = Visibility.Visible;
            }
        }

        //Need to set PID back to 0x6001 (24577 in Dec)


        private FTDI.FT232R_EEPROM_STRUCTURE eepromDumpCommand()
        {
            var myEEPROM = new FTDI.FT232R_EEPROM_STRUCTURE();
            
            if (ftdi.ReadFT232REEPROM(myEEPROM) == FTDI.FT_STATUS.FT_OK)
            {
                LogBox.AppendText("EEPROM Buffer Data:\r");
                
                LogBox.AppendText(String.Format("Vendor ID: {0:x}\r", myEEPROM.VendorID));
                LogBox.AppendText(String.Format("Product ID: {0:x}\r", myEEPROM.ProductID));
                LogBox.AppendText(String.Format("Manufacturer: {0}\r" ,myEEPROM.Manufacturer));
                LogBox.AppendText(String.Format("Manufacturer ID: {0}\r" ,myEEPROM.ManufacturerID));
                LogBox.AppendText(String.Format("Description: {0}\r" ,myEEPROM.Description));
                LogBox.AppendText(String.Format("Serial Number: {0}\r" ,myEEPROM.SerialNumber));
                LogBox.AppendText(String.Format("Max Power: {0}mA\r",myEEPROM.MaxPower));
                LogBox.AppendText(String.Format("Self Powered: {0}\r", myEEPROM.SelfPowered));
                LogBox.AppendText(String.Format("High Drive IO's: {0:x}\r", myEEPROM.HighDriveIOs));
                LogBox.AppendText(String.Format("Invert CTS: {0:x}\r", myEEPROM.InvertCTS));
                LogBox.AppendText(String.Format("Invert DCD: {0}\r", myEEPROM.InvertDCD));
                LogBox.AppendText(String.Format("Invert DSR: {0}\r", myEEPROM.InvertDSR));
                LogBox.AppendText(String.Format("Invert DTR: {0}\r", myEEPROM.InvertDTR));
                LogBox.AppendText(String.Format("Invert RI: {0}\r", myEEPROM.InvertRI));
                LogBox.AppendText(String.Format("Invert RTS: {0}\r", myEEPROM.InvertRTS));
                LogBox.AppendText(String.Format("Invert RXD: {0}\r", myEEPROM.InvertRXD));
                LogBox.AppendText(String.Format("Invert TXD: {0}\r", myEEPROM.InvertTXD));
                LogBox.AppendText(String.Format("Pull Down Enabled: {0}\r", myEEPROM.PullDownEnable));
                LogBox.AppendText(String.Format("RIsD@XX: {0}\r", myEEPROM.RIsD2XX));
                LogBox.AppendText(String.Format("Serial Number Enabled: {0}\r", myEEPROM.SerNumEnable));
                LogBox.AppendText(String.Format("Use External Oscillator: {0}\r", myEEPROM.UseExtOsc));
                LogBox.AppendText(String.Format("CBUS0 Value: {0}\r", myEEPROM.Cbus0));
                LogBox.AppendText(String.Format("CBUS1 Value: {0}\r", myEEPROM.Cbus1));
                LogBox.AppendText(String.Format("CBUS2 Value: {0}\r", myEEPROM.Cbus2));
                LogBox.AppendText(String.Format("CBUS3 Value: {0}\r", myEEPROM.Cbus3));
                LogBox.AppendText(String.Format("CBUS4 Value: {0}\r", myEEPROM.Cbus4));
                LogBox.AppendText(String.Format("EndPoint Size: {0}\r", myEEPROM.EndpointSize));
                
                currentDeviceConfig.BuildFromEEPROMStruct(myEEPROM);
                

                
            }
            return myEEPROM;
        }

        private void WritePIDButton_Click(object sender, RoutedEventArgs e)
        {
            LogBox.Clear();
            //Oi Scary...
            LogBox.AppendText("Starting EEPROM flash...\r--------------------------\r\r");
            var oldRawEEPROM = currentDeviceConfig.RawEEPROMStructure;
            
            Thread.Sleep(200);

            LogBox.AppendText("------------EEPROM PRIOR TO FLASH-----------\r");
            //Test Connection first
            eepromDumpCommand();

            if (oldRawEEPROM == currentDeviceConfig.RawEEPROMStructure)
            {
                currentDeviceConfig.RawEEPROMStructure.ProductID = Convert.ToUInt16(Convert.ToInt32(PIDTextBox.Text, 16));
            }


            
            //Validate
            if (currentDeviceConfig.RawEEPROMStructure.ProductID == Convert.ToUInt16(PIDTextBox.Text))
            {
                WriteNewEEPROM(currentDeviceConfig.RawEEPROMStructure);
            }


            
        }

        private void EnablePIDWriteCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (currentDeviceConfig.Connected)
            {
                PIDTextBox.Visibility = Visibility.Visible;
                WritePIDButton.Visibility = Visibility.Visible;
                PIDTextBox.IsEnabled = true;
            }
            
        }

        private void WriteNewEEPROM(FTDI.FT232R_EEPROM_STRUCTURE eepromStruct)
        {
            var getStatus = FTDI.FT_STATUS.FT_FAILED_TO_WRITE_DEVICE;
            try
            {
                getStatus = ftdi.WriteFT232REEPROM(eepromStruct);
            }
            catch (FTDI.FT_EXCEPTION)
            {
                LogBox.AppendText("Exception thrown when attempting write new EEPROM data\r");
            }

            if (getStatus == FTDI.FT_STATUS.FT_OK)
            {

                if (ftdi.CyclePort() == FTDI.FT_STATUS.FT_OK)
                {
                    if (ftdi.OpenBySerialNumber(currentDeviceConfig.SerialNumber) == FTDI.FT_STATUS.FT_OK)
                    {
                        LogBox.AppendText("PORT cycle and re-Connect completed successfully\r");
                    }
                    else
                    {
                        //Reload
                    }
                }
                


                LogBox.AppendText(String.Format("Success Writting to EEPROM Data to Device: {0}\r\r",eepromStruct.SerialNumber));
                LogBox.AppendText("------------EEPROM AFTER FLASH-----------\r");
                LogBox.AppendText("-----------------------------------------\r");
                LogBox.AppendText("If data looks unchanged please verify that your computer can now see the device.\r");
                LogBox.AppendText("!!!!!!!!!!!! - You may need to disconnect the device from the computer and \rreconnect to install the new instance of the driver - !!!!!!!!!!!!\r");
                eepromDumpCommand();
            }
        }

        private void PIDTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WritePIDButton.IsEnabled = true;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            ftdi.CyclePort();
            ftdi.Close();
            DisconnectButton.Visibility = Visibility.Collapsed;
            SerialNumTestBox.Visibility = Visibility.Hidden;
            ConnectButton.Visibility = Visibility.Hidden;
            PIDTextBox.Visibility = Visibility.Hidden;
            WritePIDButton.Visibility = Visibility.Hidden;
            EnablePIDWriteCheckbox.Visibility = Visibility.Hidden;
        }
    }
}
