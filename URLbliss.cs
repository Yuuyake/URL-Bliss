/*                                              
│    ██╗   ██╗██████╗ ██╗         ██████╗ ██╗     ██╗███████╗███████╗
│    ██║   ██║██╔══██╗██║         ██╔══██╗██║     ██║██╔════╝██╔════╝
│    ██║   ██║██████╔╝██║         ██████╔╝██║     ██║███████╗███████╗
│    ██║   ██║██╔══██╗██║         ██╔══██╗██║     ██║╚════██║╚════██║
│    ╚██████╔╝██║  ██║███████╗    ██████╔╝███████╗██║███████║███████║
│     ╚═════╝ ╚═╝  ╚═╝╚══════╝    ╚═════╝ ╚══════╝╚═╝╚══════╝╚══════╝
│
│ yunusemrem@windowslive.com
│ 05550453000                                       
│        
│      TODO:
           > dosyada benzerlikleri bul algoritma ile
           > backup alma yazısı kısmını array e ekle         
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Console = Colorful.Console;
using System.Drawing;
using Colorful;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Media;

namespace urlbliss {
    class Program {
        // Relative path can be more reliable rather than absulate
        // .exe and "urlbliss.txt" files should be at the same location
        private static string urlblissFileName = @"urlbliss.txt"; // 
        private static string urlblissBACKUP = @"urlbliss_BACKUP.txt"; 
        // Because of that the urlbliss file and its content might be too large to handle 
        // alteration operations on memory can be problematic, so using a tampon file is better
        private static string inputFileName = @"import.txt";
        private static string inputFileName_fouls = @"import_temp.txt";
        private static string bannerFile = URLbliss.Properties.Resources.urlblissBanner;
        private static Color red = Color.Red;
        private static string urlFakingAdress = "http://www.hurriyet.com.tr/";
        private static DateTime backupTime = DateTime.Now;
        private static List<string> menuItems = new List<string>(){
            "\n ╟ [1] Insert url          ",
            "\n ╟ [2] Search url          ",
            "\n ╟ [3] Delete url          ",
            "\n ╟ [4] Check last url      ",
            "\n ╟ [5] Check file content  \n                              ",
            "\n ╟ [6] Show Last lines     ",
            "\n ╟ [7] EXIT                ",};
        private static List<string> descItems = new List<string>(){
            "\t│ ● use arrows ↑↓ to select ",
            "\t│ ● " + urlblissFileName + " and " + inputFileName + " must be at the same loc with .exe",
            "\t│ ",
            "\t│ ● INSERTION SYNTAX: url;comment ",
            "\t│ ● !!! Insertion a url to " + urlblissFileName + " by this app",
            "\t│   will put *(star) before and after the url ",
            "\t│",
            "\t│   Example: \"*url*\" \"comment dd.mm.yy username\""
        };

        // ==========================================   MAIN FUNC  ===============================================
        //
        static void Main(string[] args) {
            Console.Title = "URLbliss";
            Console.OutputEncoding = Encoding.UTF8;
            Helpers.BACKUP(urlblissFileName);
            // Set windows size relative to the screen, setting is for visualization
            Console.SetWindowSize(Console.LargestWindowWidth/2 + 8, Console.LargestWindowHeight - 1);
            while (true) {
                makeChoice2();
                Console.ReadLine();
                Console.Clear();
            }
        }
        // =======================================================================================================
        /// <summary> Initializes proxy configs for connection or call to net </summary>
        private static WebProxy initializeProxyConfigs() {
            try {
                Console.Write("\n │ Initializing proxy configs...");
                // setting PROXY config and Network CREDENTIALS
                // this is standing (not in its own function) but here to get proxy settings
                HttpWebRequest tempReq = (HttpWebRequest)WebRequest.Create("http://google.com");
                WebProxy myProxySetting = new WebProxy();
                // there is little ambiguity about whether getting/setting proxy properly or not
                IWebProxy proxy = tempReq.Proxy;
                if (proxy != null) { // set grabbed proxy settings to myproxy
                    Console.Write("\n │\t├─ Proxy: {0}", proxy.GetProxy(tempReq.RequestUri));
                    myProxySetting.Address = proxy.GetProxy(tempReq.RequestUri);
                }
                else {
                    // i did not find this "else" is working properly, so Exit(1)
                    throw new ArgumentException("\n │\t├─ !No proxy detected.\n\tSet proxy to MYPROXY");
                    Uri newUri = new Uri("MYPROXY");
                    // Associate the newUri object to 'myProxySetting' object so that new myProxy settings can be set.
                    myProxySetting.Address = newUri;
                }
                // Setting User Creds to pass proxy
                string userID = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                Console.Write("\n │\t├─ UserID: ");
                if(userID == "unknown")
                           userID = Console.ReadLine();
                else
                           Console.Write(userID);
                Console.Write("\n │\t├─ Password: ");
                string pass = Helpers.darker(); // ask and save user password on the quiet
                myProxySetting.Credentials = new NetworkCredential(userID, pass);
                return myProxySetting;
            }
            catch (Exception e) {
                Console.WriteFormatted("\n\n\t" + e.Message, red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                Console.ReadLine();
                Environment.Exit(1);
            }
            return new WebProxy();
        }
        /// <summary> Prints selective Main Menu </summary>
        static void makeChoice2() {
            new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
            int currChoice = 0;
            Console.WriteFormatted(bannerFile, Color.LightGoldenrodYellow);
            Console.WriteFormatted("\n (Backup created " + backupTime + ")",Color.Cyan);
            Console.Write("\n ╔═════════════════════════════════════════════════════════════════════════════════════════════════\n ║");
            while (true) {
                // menuItems.Count element is the length of selective menu, so look for module menuItems.Count
                currChoice = currChoice < 0 ? currChoice + menuItems.Count : currChoice % menuItems.Count; 
                //setting cursor position to 0 gives slightly higher performance than Console.Clear()
                Console.SetCursorPosition(0, bannerFile.Count(f => f == '\n') + 2 ); // normally 18 is the height of URLBLISS banner
                // make the URLbliss Banner dissappear
                // Console.SetCursorPosition(0, 3); // 18 is the height of URLBLISS banner
                menuItems[currChoice] = menuItems[currChoice].Insert(3, "►");
                for (int i = 0; i < menuItems.Count; i++) { // 6 element is the length of selective menu
                    if (currChoice == i)
                        Console.WriteFormatted(menuItems[i], Color.White);
                    else
                        Console.WriteFormatted(menuItems[i], Color.FromArgb(0, 255, 0));
                    if (i >= descItems.Count)
                        continue;//descItems.Add("\t│ ");
                    if (i == 3)
                        Console.WriteFormatted(descItems[i], Color.Orange);
                    else
                        Console.WriteFormatted(descItems[i], Color.FromArgb(255, 0, 0));
                }
                menuItems[currChoice] = menuItems[currChoice].Remove(3, 1);

                ConsoleKeyInfo pressed = Console.ReadKey(true);
                if (pressed.Key == ConsoleKey.DownArrow) {
                    currChoice++;
                    new SoundPlayer(URLbliss.Properties.Resources.slideup).Play();
                }
                else if (pressed.Key == ConsoleKey.UpArrow) {
                    currChoice--;
                    new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressed.Key == ConsoleKey.Enter) {
                    new SoundPlayer(URLbliss.Properties.Resources.select).Play();
                    break; // if Enters exit from Choice screen and call proper function
                }
            } // while 
            switch ((currChoice+1).ToString()) {
                case "1":
                    makeInsertChoice();
                    break;
                case "2":
                    Console.Write("\n\n\t╟ URL to search: ");
                    string urlS = Console.ReadLine();
                    searchUrl(urlS);
                    break;
                case "3":
                    Console.Write("\n\n\t╟ URL to search: ");
                    string urlD = Console.ReadLine();
                    deleteUrl(urlD);
                    break;
                case "4":
                    checkLastLine();
                    break;
                case "5":
                    checkFileContent();
                    break;
                case "6":
                    showLastLines();
                    break;
                case "7":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteFormatted("\n\t ╟ Wrong Input!!!", red);
                    new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                    break;
            } // switch 
            Console.Write("\n\n\t│ Press Enter to try Again. ");
        }
        /// <summary>
        /// shows last 3 line of urlbliss.txt
        /// </summary>
        private static void showLastLines() {
            Console.WriteFormatted("\n\n\n │ \"urlbliss.txt\": ", Color.DarkOrange);
            Console.WriteFormatted("\n └───────────────────────────────────────────────────────────────────────────────────── ", Color.DarkOrange);
            Console.WriteFormatted("\n\t  ... ", Color.White);
            Console.WriteFormatted("\n\t  ... ", Color.White);
            List<string> lines = File.ReadLines(urlblissFileName).ToList();
            lines = lines.Skip(Math.Max(0, lines.Count() - 3)).ToList();
            foreach (string ss in lines) {
                Console.BackgroundColor = Color.Black;
                Console.WriteFormatted("\n\t  ", Color.White);
                Console.BackgroundColor = Color.Blue;
                Console.WriteFormatted(ss , Color.White);
            }
            Console.BackgroundColor = Color.Black;
            Console.WriteFormatted("\n\t  END OF FILE ", Color.DarkOrange);
            Console.WriteFormatted("\n ────────────────────────────────────────────────────────────────────────────────────── ", Color.DarkOrange);

        }
        /// <summary> Prints subMenu to insert URL(s) from File or Manuel </summary>
        static void makeInsertChoice() {
            int currSubChoice = 0;
            while (true) {
                // if u want to add something more to insertURL menu, add it to "subInsert" list
                List<string> subInsert = new List<string>() {
                    "\n\t╟ From File ",
                    "\n\t╟ Manuel    ",
                    "\n\t  Back       "};
                currSubChoice = currSubChoice < 0 ? currSubChoice + subInsert.Count : currSubChoice % subInsert.Count;
                subInsert[currSubChoice] = subInsert[currSubChoice].Insert(3, "►");
                // print subMenu
                // normally bannerFile.Count(f => f == '\n') is the height of URLBLISS banner
                Console.SetCursorPosition(0, menuItems.Count + bannerFile.Count(f => f == '\n') + 3);
                Console.WriteFormatted("\n\n\n\t│ ● INSERTION SYNTAX: url;comment \n\t│",Color.Orange);
                for (int i = 0; i < subInsert.Count; i++) { // subInsert.Count is the length of sub selective menu
                    if (currSubChoice == i)
                        Console.WriteFormatted(subInsert[i], Color.LightYellow, Color.White, 1);
                    else
                        Console.WriteFormatted(subInsert[i], Color.LightYellow, Color.FromArgb(0, 255, 0), 1);
                }
                //subInsert[currSubChoice] = subInsert[currSubChoice].Remove(13);

                ConsoleKeyInfo pressedSub = Console.ReadKey(true);
                if (pressedSub.Key == ConsoleKey.DownArrow) {
                    currSubChoice++;
                    new SoundPlayer(URLbliss.Properties.Resources.slidedown).Play();
                }
                else if (pressedSub.Key == ConsoleKey.UpArrow) {
                    currSubChoice--;
                    new SoundPlayer(URLbliss.Properties.Resources.slideup).Play();
                }
                else if (pressedSub.Key == ConsoleKey.Enter) {
                    new SoundPlayer(URLbliss.Properties.Resources.select).Play();
                    break;
                }
            } // while
            switch (currSubChoice.ToString()) {
                case "0":
                    insertFromFile();
                    break;
                case "1":
                    Console.Write("\n\t│ Enter (url;comment): ");
                    string line = Console.ReadLine();
                    insertOneUrl(line, 1);
                    break;
                case "2":
                    break;
                default:
                    Console.Write("\n\t│ Error Code 222 !!! ");
                    new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                    break;
            }
        }
        /// <summary> Searches(contains) given URL in txt </summary>
        static bool searchUrl(string URL) {
            bool found = false;
            string[] dots = { ".", "..", "..." };
            try {
                using (StreamReader file = new StreamReader(urlblissFileName)) {
                    if (file.ReadLine() == null)
                        Console.WriteLineFormatted("\n │ ! Failure when reading file.", red);
                    else {
                        short counterLines = 0;
                        string currLine;
                        while ((currLine = file.ReadLine()) != null) {
                            Helpers.NOP(200); // waits a little bit, the greater the longer
                            Console.Write("\r\t│ [{0}] Looking {1}   ", counterLines, dots[(counterLines / 600) % 3]);
                            counterLines++;
                            if (currLine.Contains(URL)) {
                                Console.Write("\n\t│ Found at line [{0}] > {1}\n", counterLines, currLine);
                                found = true;
                            }
                        }// loop
                    }// if-else
                }// file read
            }// try
            catch (Exception e){
                Console.WriteLineFormatted("\n!│ Reading exception: "+ e.Message, red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
            }
            return found;
        }
        /// <summary> Searches exactly given URL in txt </summary>
        static bool searchExactUrl(string URL) {
            bool found = false;
                using (StreamReader file = new StreamReader(urlblissFileName)) {
                    short counterLines = 0;
                    string currLine;
                    file.ReadLine();
                    while ((currLine = file.ReadLine()) != null) {
                        counterLines++;
                        if (currLine.Split('"')[1].Replace("*", "") == URL) {
                            Console.Write("\n\t │ Found at line [{0}] > {1}\n", counterLines, currLine);
                            found = true;
                        }
                    }// loop
                }// file read     
            return found;
        }
        /// <summary> Deletes a given URL in txt </summary>
        static void deleteUrl(string URL) {
            string tempFileName = @"new_urlbliss.txt";
            File.Delete(tempFileName); // delete previously created temp file 
            File.Create(tempFileName).Close();
            int counter = 1;
            string[] dots = { ".", "..", "..." };
            Console.WriteLine("\t │ Looking to delete...");
            List<string> newContent = new List<string>();
            List<string> deletions = new List<string>();
            string currLine = null;
            bool terminate = false;
            //List<string> linesToDelete = new List<string>(new string[] { }); for further use
            using (StreamReader reader = new StreamReader(urlblissFileName)) { //open the file that will be read to delete an url
                while ( (currLine = reader.ReadLine()) != null) {
                    Helpers.NOP(500); // waits a little bit, the greater the longer
                    Console.Write("\r\t │ [{0}] Looking {1}   ", counter, dots[(counter/600)%3]);
                    try {
                        if (currLine.Contains(URL) == true && terminate != true) {
                            while (true) {
                                Console.Write("\n\t │ Found > [{0}] {1} DELETE(y/n/q)? ", counter,currLine);
                                string choice = Console.ReadLine();
                                if (choice == "y") {
                                    Console.Write("\t │       > will be DELETED\n", counter, currLine);
                                    deletions.Add("\n\t\t │ " + currLine);
                                    Console.Write("\n │");
                                    break;
                                }
                                else if (choice == "n") {
                                    newContent.Add(currLine);
                                    Console.Write("\n │");
                                    break;
                                }
                                else if (choice == "q") {
                                    newContent.Add(currLine);
                                    terminate = true;
                                    break;
                                }
                                else {
                                    Console.WriteLineFormatted("\t │ Wrong input!!!", red);
                                    new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                                }
                            }
                        }
                        else
                            newContent.Add(currLine);
                    } catch (Exception e) {
                        Console.WriteLineFormatted("\t!│ Exception: " + e.Message, red);
                        new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                    }
                    counter++;
                }//while
            }// reader
            writeListToFile(tempFileName, newContent);
            File.Delete(urlblissFileName);//new URLbliss will be placed
            File.Move(tempFileName, urlblissFileName); // Initialize new original 
            if (deletions.Count == 0) {
                Console.Write("\n\t │ Nothing Found.", counter, deletions.Count);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
            }
            else
                Console.Write("\r\t │ DONE > {1} line(s) are DELETED  ", counter, deletions.Count);
            deletions.ForEach(Console.Write);

        }
        /// <summary> Inserts one url to txt </summary>
        static bool insertOneUrl(string line, int counterLines) {
            List<string> success = new List<string>();
            try {
                if (line.Split(';').Count() != 2)
                    throw new ArgumentException("number of (;)seperator");
                string comment = line.Split(';')[1];
                string URL = line.Split(';')[0];
                Regex regx = new Regex("^[a-zA-Z0-9]*$");
                // only alphanumeric chars allowed when inserting from this App
                // otherwise (like old inputs) this control is not in charge
                if (regx.IsMatch(comment.Replace(" ","")) == false)
                    throw new ArgumentException("(not alphanumeric)comment");
                if (searchExactUrl(URL) == true) // if not already exists, add it
                    throw new ArgumentException("\t!│ URL already exist");

                // syntax is like = "*GivenUrl*" "GivenComment dd.mm.yy username" + linebreak 
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                string textToWrite = "\"*"
                    + URL
                    + "*\" \""
                    + comment
                    + " "
                    + DateTime.Now.ToString("dd.MM.yy HH:mm")
                    + " "
                    + userName
                    + "\"";
                // double check can be safer, so check the text that ll be written to file
                if (checkLine(textToWrite, counterLines) == true) {
                    success.Add(textToWrite);
                    writeListToFile(urlblissFileName, success);
                    return true;
                }
            } // try
            catch (Exception e) {
                Helpers.printProblematic(e.Message, counterLines, line);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
            }
            return false;
        }
        /// <summary> Inserts bulk file to txt </summary>
        static void insertFromFile() {

            int counterLines = 1;
            List<string> fails = new List<string>() { };

            if (File.Exists(inputFileName_fouls) == true) {
                File.Delete(inputFileName_fouls);
                File.Create(inputFileName_fouls).Close();
            }
            if (File.Exists(urlblissFileName) == false) { 
                Console.WriteLineFormatted("\n\t│ The output file \"" + urlblissFileName + "\" not exists. Check.", red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                return; }
            if (File.Exists(inputFileName) == false) {
                Console.WriteLineFormatted("\n\t│ The input file \"" + inputFileName + "\" not exists. Check.", red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                return; }

            var lines = File.ReadLines(inputFileName);
            Console.Write("\n\n │ URLs are being checked {0} ... \n");
            foreach (var line in lines) { // read and insert 1 URL each time
                // success returns ,garanteed url that ll be written to file is handled in "insertOneUrl()" bc it is parsed in there
                if ( insertOneUrl(line, counterLines) == false ) 
                    fails.Add(line);
                counterLines++;
            }
            writeListToFile(inputFileName_fouls, fails);
            File.Delete(inputFileName);
            File.Move(inputFileName_fouls, inputFileName);
            Console.WriteFormatted("\n │ DONE.",red);
        }
        /// <summary> Checks whole txt for error/fouls </summary>
        static void checkFileContent() {
            //Checking file contents for invalid syntax's
            int counterLines = 0;
            int counterValid = 0;
            int counterFoul = 0;
            string currLine;
            Dictionary<int, string> foulList = new Dictionary<int, string>();
            // Read the file and display it line by line. 
            Console.Write("\n");
            using (StreamReader file = new StreamReader(urlblissFileName)) {
                if (file.EndOfStream == true) {
                    Console.WriteFormatted("\n │ ! File is empty as my hearth.", red);
                    return;
                }
                if (file.ReadLine() == null) {
                    Console.WriteFormatted("\n │ ! Failure when reading file.", red);
                    new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                    return;
                }
                while ((currLine = file.ReadLine()) != null) {
                    counterLines++;
                    if (checkLine(currLine, counterLines))
                        counterValid++;
                    else {
                        counterFoul++;
                        foulList.Add(counterLines, currLine);
                    }
                }
            }
            System.Console.WriteLine("\n\n │ {0} EXIST lines.", counterLines);
            System.Console.WriteLine(" │ {0} VALID lines.", counterValid);
            System.Console.WriteLine(" │ {0} FOUL lines are:", counterFoul);
            foreach (KeyValuePair<int, string> foul in foulList)
                Console.WriteLineFormatted("\t│ line {0} > {1}", red, red, foul.Key, foul.Value);
        }
        /// <summary> Checks only the last txt for error/fouls </summary>
        static void checkLastLine() {
            try {
                string lastLine = File.ReadLines(urlblissFileName).Last();
                string url = lastLine.Split('"')[1].Replace("*", "");
                if (url.Length < 2) {
                    Console.WriteFormatted("\n\n!│ Last line is Foul > {0} " + lastLine, red);
                    throw new ArgumentException("URL\\IP length is problematic");
                }
                Console.Write("\n │ Opening at Browser > {0}", urlFakingAdress + url);
                System.Diagnostics.Process.Start(urlFakingAdress + url);
            }
            catch (Exception e) {
                Helpers.printProblematic(e.Message, -1, "?");
            }
            return;
        }
        /// <summary> Checks given URL for error/fouls in syntax </summary>
        static bool checkURL(string URL) {
            // check whether given url is valid or not
            string unwise = "!$'()* ^+,;"; //unsafe characters for an URL
            foreach (char c in unwise) {
                if (URL.Contains(c)) {
                    //Console.WriteLine("Probably invalid: {0}", URL);
                    return false;
                }
            }
            return true;
        }
        /// <summary> Checks given IP for error/fouls in syntax </summary>
        static bool checkIP(string IP) {
            // check whether given url is valid or not
            IPAddress address;
            if (IPAddress.TryParse(IP, out address)) {
                switch (address.AddressFamily) {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        Console.Write("\n │ " + IP + " is IPv4");
                        return true;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        Console.Write("\n │ " + IP + " is IPv6");
                        return true;
                    default:
                        Console.WriteFormatted("\n ! " + IP + " is NOT valid",red);
                        return false;
                }
            }
            else
                return false;
        }
        /// <summary> Checks given whole line for error/fouls in syntax </summary>
        static bool checkLine(string line, int counterLines) { // second argument is standing to print line number when error occurs
            // to check file content, gets whole line then inspects 
            string problem = "NONE";
            try {
                int countDQuote = line.Count(f => f == '"');
                int countStar = line.Count(f => f == '*');
                int countSpace = line.Count(f => f == ' ');
                int countSeperator = line.Count(f => f == ';');

                string url = line.Split('"')[1].Replace("*","");
                string comment = line.Split('"')[3];
                // Those if statements can be enhanced
                if (url.Length < 2)
                    problem = "URL\\IP length";

                else if (!(checkURL(url) == true || checkIP(url) == true))
                    problem = "URL or IP";

                else if (countStar > 2)
                    problem = "*(star)";

                else if (countDQuote != 4)
                    problem = "\"(DQuote)";

                else if (line.Contains("\" \"") != true)
                    problem = "\" \"(space)";

                else if (line.Contains('\t') == true)
                    problem = "\\t(Tab)";

                else if (comment == "" || comment == " " || comment == null || comment == String.Empty)
                    problem = "empty or disformed comment";

                else if (line.Split('"').Count() > 5)
                    problem = "-> " + line.Split('"')[5] + " <-";

                else { // NO PROBLEM at ALL
                    System.Console.Write("\n\t│ [{0}] OK > {1} ", counterLines, line.Replace("\r\n", ""));
                    return true;
                }
            }
            catch {
                problem = ">> failed when parsing line * (star)";
            }
            Helpers.printProblematic(problem, counterLines, line);
            return false;
        }
        /// <summary> Writes the given URL to give file </summary>
        static void writeListToFile(string path, List<string> content) {
            FileInfo f = new FileInfo(path);
            if (f == null)
                Console.WriteFormatted("\n\t │ Cannot find {0} " + path, red);
            bool isFirst = f.Length > 10 ? false : true;
            try {
                using (StreamWriter writer = new StreamWriter(path, append: true)) { //open the file that will be written as updated version of before
                    foreach (string ss in content) {
                        if (isFirst == true) {
                            writer.Write(ss);
                            isFirst = false;
                        }
                        else
                            writer.Write(Environment.NewLine + ss);
                    }
                } // using
            }
            catch (Exception e) {
                Console.WriteFormatted("\n\t │ Exception: " + e.Message, red);
                Console.WriteLineFormatted("\t │ Operation failed. Nothing changed.", red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                return;
            }
        }
        /// <summary> Printing menu in old style </summary>
        public static void printMenu(int choice = 0) {
            Console.WriteFormatted("\n ╟ [{0}] Insert url",        Color.LightYellow, Color.FromArgb(0, 255, 0), 1);
            Console.WriteFormatted("\n ╟ [{0}] Search url",        Color.LightYellow, Color.FromArgb(0, 255, 0), 2);
            Console.WriteFormatted("\n ╟ [{0}] Delete url",        Color.LightYellow, Color.FromArgb(0, 255, 0), 3);
            Console.WriteFormatted("\n ╟ [{0}] Check last url",    Color.LightYellow, Color.FromArgb(0, 255, 0), 4);
            Console.WriteFormatted("\n ╟ [{0}] Check file content",Color.LightYellow, Color.FromArgb(0, 255, 0), 5);
            Console.WriteFormatted("\n ╟ [{0}] EXIT",              Color.Red,         Color.FromArgb(0, 255, 0), 6);
        }
    }
}
