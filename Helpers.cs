using System;
using System.Collections.Generic;
using System.Linq;
using Console = Colorful.Console;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Media;

namespace urlbliss {
    public static class Helpers {
        public static Color red = Color.Red;
        /// <summary> Saves a BACKUP of given file with name "xx_BACKUP.txt" </summary>
        public static void BACKUP(string fileName) {
            string backupFile = fileName.Replace(".txt", "") + "_BACKUP.txt";
            try {
                File.Delete(backupFile);
                File.Copy(fileName, backupFile);
            }
            catch (Exception e) {
                Console.WriteFormatted("\n\n\t" + e.Message, red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
        /// <summary> Measures the strongness of given password </summary>
        public static int passwordStrength(string password) {
            int score = 0;
            try {
                if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password.Trim())) return score;
                if (!(password.Length >= 5)) return 1;
                if (password.Length >= 8) score++;
                /// score grows if the password has at least one upper or lower case letter
                if (password.Any(c => char.IsUpper(c)) && password.Any(c => char.IsLower(c))) score++;
                /// score grows if the password has at least one digit
                if (password.Any(c => char.IsDigit(c))) score++;
                /// score grows if the password has at least one special character
                if (password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1) score++;
            }
            catch (Exception e) {
                Console.WriteFormatted("\n\n\t" + e.Message, red);
                new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                Console.ReadLine();
                Environment.Exit(1);
            }
            return score;
        }
        /// <summary> H ides characters for privacy when writing password to console interface </summary>
        public static string darker() { // 
            string pass = "";
            do {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.KeyChar == '0')
                    break;
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0) {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter) {
                        if (passwordStrength(pass) < 4) { // 
                            Console.WriteFormatted("\n │\t│ PROBABLY wrong password. Not fits PC login conditions !!!", red);
                            Console.Write("\n │\t│ Try again. ('0' to exit) Password: ");
                            new SoundPlayer(URLbliss.Properties.Resources.gurg).Play();
                            pass = "";
                        }
                        else
                            break;
                    }
                }
            } while (true);
            return pass;
        }
        /// <summary> Writes out the given problem by visualizing it </summary>
        public static void printProblematic(string problem, int counterLines, string line) { // to fix visualization easly
            string visualized = "\n!│ [" + counterLines + "] Foul > " + "Probably " + problem + " is problematic" + " >> " + line.Replace("\r\n", "");
            Console.WriteFormatted(visualized, red);
        }
        /// <summary> Waits less than miliseconds(by making CPU busy). The greater "n" the longer wait </summary>
        public static void NOP(int n) {
            int count = 0;
            long a = 2;
            while (count < n) {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a) {
                    if (a % b == 0) {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0) {
                    count++;
                }
                a++;
            }
        }
        /// <summary> Write string array to file in better way </summary>
        public static void WriteAllLinesBetter(string path, params string[] lines) {
            if (path == null)
                throw new ArgumentNullException("path");
            if (lines == null)
                throw new ArgumentNullException("lines");

            using (var stream = File.OpenWrite(path)) {
                stream.SetLength(0);
                using (var writer = new StreamWriter(stream)) {
                    if (lines.Length > 0) {
                        for (var i = 0; i < lines.Length - 1; i++) {
                            writer.WriteLine(lines[i]);
                        }
                        writer.Write(lines[lines.Length - 1]);
                    }
                }
            }
        }
        public static System.Drawing.Color FromColor(System.ConsoleColor c) {
            int[] cColors = {   0x000000, //Black = 0
                        0x000080, //DarkBlue = 1
                        0x008000, //DarkGreen = 2
                        0x008080, //DarkCyan = 3
                        0x800000, //DarkRed = 4
                        0x800080, //DarkMagenta = 5
                        0x808000, //DarkYellow = 6
                        0xC0C0C0, //Gray = 7
                        0x808080, //DarkGray = 8
                        0x0000FF, //Blue = 9
                        0x00FF00, //Green = 10
                        0x00FFFF, //Cyan = 11
                        0xFF0000, //Red = 12
                        0xFF00FF, //Magenta = 13
                        0xFFFF00, //Yellow = 14
                        0xFFFFFF  //White = 15
                    };
            return Color.FromArgb(cColors[(int)c]);
        }
    }
}
