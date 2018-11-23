using Microsoft.Win32;
using System;
using System.Net;
using System.Windows.Forms;


namespace VT_Editor
{
    internal class Program
    {
        public const string enterToCont = "Press Enter to continue. . .";
        private const string subKey = @"SOFTWARE\HighJump Software\Advantage Virtual Terminal";
        private const string userKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";

        private static void Main()
        {
            MessageBox.Show("This application's log file will not save if you close the window. For the time being, if you want to save the log file, use option 5 to exit the program.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //Console.WindowHeight = 36;
            //Console.WindowWidth = 96;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Log.CreateOrOpenLogFile();
            Log.WriteLine("");
            Log.Write("!");
            for (int i = 0 ; i < 41 ; i++) { Log.Write("*"); }
            Log.Write("!");
            Log.Write("\n");
            Log.WriteLine("\nVT Editor last run: " + "[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "]");
            Log.Write("Username: ");
            SetColor("green");
            Log.WriteLine("[" + Environment.UserName + "]");
            SetColor("white");
            Log.Write("Domain name: ");
            SetColor("green");
            Log.WriteLine("[" + Environment.UserDomainName + "]");
            SetColor("white");
            Console.WriteLine(GetLocalIPAddress(true));
            GetRegistryInfo(true);
            Menu();
        }

        private static void Menu()
        {
            Console.Clear();
            SetColor("cyan");
            CenterText("Advantage Virtual Terminal Registry Editor/Removal tool\n");
            SetColor("white");
            Console.WriteLine(GetLocalIPAddress(false));
            GetRegistryInfo(false);
            Console.WriteLine("\n1: Delete the VT Registry Keys (Removes the 'HighJump Software' key and all of it's Subkeys)");
            Console.WriteLine("2: Edit the default VT");
            Console.WriteLine("3: Turn VT menu options ON or OFF");
            Console.WriteLine("4: Remove unused VTs");
            Console.WriteLine("5: Exit the program");

            try
            {
                int userInput = Convert.ToInt32(Console.ReadLine());

                switch (userInput)
                {
                    case 1:
                        DeleteAllVTs();
                        break;

                    case 2:
                        EditVT();
                        break;

                    case 3:
                        SetMenuOptionsOnOff(true);
                        break;

                    case 4:
                        RemoveVT();
                        break;

                    case 5:
                        Log.Close();
                        Environment.Exit(0);
                        break;

                    default:
                        ErrorHandler("MenuError");
                        Menu();
                        break;
                }
            }
            catch (FormatException)
            {
                ErrorHandler("MenuError");
                Menu();
            }
            
        }

        private static void GetRegistryInfo(bool writeToLog)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey);

            if (key != null)
            {
                Console.Write("The default Virtual Terminal is: ");
                SetColor("green");
                Console.WriteLine(Convert.ToString(key.GetValue("Default Device Name")));
                SetColor("white");
                Console.WriteLine("Other installed VTs: ");
                SetColor("yellow");
                if (writeToLog)
                {
                    Log.Write("The default Virtual Terminal is: ");
                    Log.WriteLine("[" + Convert.ToString(key.GetValue("Default Device Name")) + "]");
                    Log.WriteLine("Other installed VTs: ");
                }
                foreach (var v in key.GetSubKeyNames())
                {
                    if (v != Convert.ToString(key.GetValue("Default Device Name")))
                    {
                        Console.WriteLine(Convert.ToString(v));

                            if (writeToLog)
                            {
                                Log.WriteLine(Convert.ToString($"[{v}]"));
                            }
                    }
                }
                if (writeToLog) { for (int i = 0 ; i < 41 ; i++) { Log.Write("-"); }
                Log.Write("\n");
                }
                key.Close();
                SetColor("white");
                writeToLog = false;

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                CenterText("There doesn't seem to be a Virtual Terminal installed on this computer.");
                Log.WriteLine("There was no VT installed on the computer");
                CenterText("Press Enter to exit the program");
                Console.ReadLine();
                Log.Close();
                Environment.Exit(0);
                key.Close();
            }

        }

        private static IPAddress GetLocalIPAddress(bool writeToLog)
        {
            bool hasNetworkIpAddress = false;

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var v in host.AddressList)
            {
                if (v.ToString().Substring(0, 3) == "10.")
                {
                    hasNetworkIpAddress = true;
                    SetColor("green");
                    if (writeToLog)
                    {
                        Log.Write("Ip Address: ");
                        Log.WriteLine("[" + Convert.ToString(v) + "]");
                        for (int i = 0 ; i < 41 ; i++) { Log.Write("-"); } Log.Write("\n");
                        writeToLog = false;
                    }
                    SetColor("white");


                }
            }
            if (hasNetworkIpAddress == false)
            {
                SetColor("red");
                foreach (var v in host.AddressList)
                {
                    CenterText(Convert.ToString(v));
                }
                SetColor("yellow");
                CenterText("The above Ip Addresse(s) (or lack of) do(es) not appear to be part of the local network.");
                CenterText("Please ensure this machine is on the correct network (10.*.*.*)");
                SetColor("white");
                if (writeToLog)
                {
                    Log.WriteLine("The machine was not on the local network when VT Editor was run");
                    writeToLog = false;
                }
            }
            return null;
        }

        private static void DeleteAllVTs()
        {
            SetColor("red");
            Console.WriteLine("\nYou are about to remove all Advantage Virtual Terminal Registry Keys!");
            SetColor("white");
            Console.Write("Are you sure? (Y/N)");
            string chooseYesNo = Console.ReadLine().ToUpper();
            string keyIsThere = Convert.ToString(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\HighJump Software"));

            if (chooseYesNo == "Y" && keyIsThere != null)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE", true);
                key.DeleteSubKeyTree("HighJump Software");
                Console.WriteLine("\nThe Advantage Virtual Terminal keys were successfully removed from the Registry");
                Console.WriteLine(enterToCont);
                Console.ReadLine();
                Log.WriteLine("Advantage Virtual Terminal keys were removed");
            }
            else if (chooseYesNo == "N")
            {
                Console.WriteLine("\nThe Advantage Virtual Terminal keys were not deleted");
                Console.WriteLine(enterToCont);
                Console.ReadLine();
                Console.Clear();
                GetRegistryInfo(false);
                Menu();
            }
            else
            {
                ErrorHandler("MenuError");
                DeleteAllVTs();
            }
            Menu();
        }

        private static void EditVT()
        {
            Console.Write("\nEnter the new VT number: VT_");
            string newVT = Console.ReadLine();
            Console.Write("Enter IP address (Enter to leave as default 10.92.237.27):");
            string vtIP = Console.ReadLine();
            Console.Write("Enter port number (Enter to leave as default 4500):");
            string vtPort = Console.ReadLine();

            if (vtIP == "")
            {
               vtIP = "10.92.237.27";
            }

            if (vtPort == "")
            {
                vtPort = "4500";
            }
            string moveOn; 
            try
            {
                var conversion = Convert.ToInt32(newVT);
                moveOn = Summerize(newVT, vtIP, vtPort, false);
            }

            catch (FormatException)
            {
                moveOn = Summerize(newVT, vtIP, vtPort, true);
            }

            if (moveOn == "Y")
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true);
                key.SetValue("Default Device Name", "VT_" + newVT);
                RegistryKey key1 = Registry.CurrentUser.OpenSubKey(subKey + "\\VT_" + newVT, true);

                try
                {
                    if (key1 == null)
                    {
                        key.CreateSubKey("VT_" + newVT);
                        RegistryKey newkey = Registry.CurrentUser.OpenSubKey(subKey + "\\VT_" + newVT, true);
                        newkey.CreateSubKey("IP Address");
                        newkey.CreateSubKey("Port");
                        newkey.SetValue("IP Address", vtIP);
                        newkey.SetValue("Port", vtPort);
                        newkey.Close();
                    }

                    else
                    {
                        key1.SetValue("IP Address", vtIP);
                        key1.SetValue("IP Address", vtIP);
                    }

                    Console.Write("VT changed to ");
                    SetColor("green");
                    Console.WriteLine($"VT_{newVT}\n");
                    SetColor("white");
                    Log.WriteLine($"The default VT was changed to: [VT_{newVT}]");
                    Log.WriteLine($"VT Ip Address was set to: [{vtIP}]");
                    Log.WriteLine($"VT Port was set to: [{vtPort}]");
                    for (int i = 0 ; i < 41 ; i++) { Log.Write("-"); }
                    Log.Write("\n"); 
                    Console.Write(enterToCont);
                    Console.ReadLine();
                    Menu();
                    }
                    finally
                    {
                        key.Close();
                        key1.Close();
                    }
            }
            else
            {
                Menu();
            }
        }

        private static string Summerize(string newVT, string vtIP, string vtPort, bool nonNumeric)
        {
            if (!nonNumeric)
            {
                SetColor("yellow");
                Console.WriteLine($"VT number: VT_{newVT}");
                Console.WriteLine($"IP Address: {vtIP}");
                Console.WriteLine($"Port: {vtPort}");
                SetColor("white");
                Console.Write("Commit changes? (Y/N)");
                return Console.ReadLine().ToUpper();
            }
            else
            {
                Console.Write("Are you sure you want to set the VT to a non-numeric value? (Y/N)");
                string yesNo = Console.ReadLine().ToUpper();
                if (yesNo == "Y")
                {
                    SetColor("yellow");
                    Console.WriteLine($"VT number: VT_{ newVT}");
                    Console.WriteLine($"IP Address: {vtIP}");
                    Console.WriteLine($"Port: {vtPort}");
                    SetColor("white");
                    Console.Write("Commit changes? (Y/N)");
                    return Console.ReadLine().ToUpper();
                }
                else
                {
                    return null;
                }
            }

        }

        private static void SetMenuOptionsOnOff(bool writeToLog)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true);
            string checkMenuOptions = Convert.ToString(key.GetValue("DisplayMenuOptions"));
            if (checkMenuOptions == "1")
            {
                SetColor("red");
                key.SetValue("DisplayMenuOptions", "0");
                Console.WriteLine("Menu Options Disabled");
                Log.WriteLine("Menu Options: [Disabled]");
                for (int i = 0 ; i < 41 ; i++) { Log.Write("-"); }
                Log.Write("\n"); 
                writeToLog = false;
                SetColor("white");
                Console.WriteLine(enterToCont);
                Console.ReadLine();
                Menu();
            }
            else
            {
                SetColor("green");
                key.SetValue("DisplayMenuOptions", "1");
                Console.WriteLine("Menu Options Enabled");
                Log.WriteLine("Menu Options: [Enabled]");
                for (int i = 0 ; i < 41 ; i++) { Log.Write("-"); }
                Log.Write("\n"); 
                writeToLog = false;
                SetColor("white");
                Console.WriteLine(enterToCont);
                Console.ReadLine();
                Menu();
            }
        }

        private static void RemoveVT()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true))
            {
                Console.Write("\nEnter the VT number to be deleted: VT_");
                string vtToDelete = "VT_" + Console.ReadLine();
                Console.Write("Are you sure? (Y/N)");
                string chooseYesNo = Console.ReadLine().ToUpper();
                string keyIsThere = Convert.ToString(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\HighJump Software\" + vtToDelete));
                try
                {
                    if (chooseYesNo == "Y" && keyIsThere != null)
                    {
                        key.DeleteSubKeyTree(vtToDelete);
                        SetColor("green");
                        Console.WriteLine("\nThe VT deleted successfully ");
                        SetColor("white");
                        Log.WriteLine($"[{vtToDelete}] was removed from the machine");
                        Console.WriteLine(enterToCont);
                        Console.ReadLine();
                    }
                    else if (chooseYesNo == "N")
                    {
                        Console.WriteLine("\nThe VT was not deleted ");
                        Console.WriteLine(enterToCont);
                        Console.ReadLine();
                        Console.Clear();
                        GetRegistryInfo(false);
                        Menu();
                    }
                    else
                    {
                        ErrorHandler("MenuError");
                        Menu();
                    }
                }
                catch (ArgumentException)
                {
                    ErrorHandler("InvalidVTError");
                    Menu();
                }
                finally
                {
                    key.Close();
                }
                Menu();
            }
        }

        private static string CenterText(string text)
        {
            int center = Console.WindowWidth / 2 + text.Length / 2;
            Console.Write("{0," + center + "}", text);
            return "";
        }

        private static void SetColor(string color)
        {
            switch (color)
            {
                default:
                    break;

                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case "cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        private static void ErrorHandler(string errType)
        {
            switch (errType)
            {
                default:
                    Console.WriteLine("Unhandled exception. Please notify me so I can re-write this program. Again...");
                    Log.WriteLine("An unhandled exception occurred. Ernie needs to fix it.");
                    break;

                case "MenuError":
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.WriteLine("Please choose a valid menu option");
                    Console.Write(enterToCont);
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ReadLine();
                    break;

                case "InvalidVTError":
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.WriteLine("That VT does not exist on this system.");
                    Console.Write(enterToCont);
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ReadLine();
                    break;
            }
        }
    }
}