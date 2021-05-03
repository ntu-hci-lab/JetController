using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.Diagnostics;

namespace JetControllerCHI21Interactivity
{
    class SteamAPIHelper
    {
        public string SteamLocation
        {
            get
            {
                return _SteamLocation;
            }
        }

        public Dictionary<int, string> Steam_GameID_Path_Table
        {
            get
            {
                return _Steam_GameID_Path_Table;
            }
        }

        private static Dictionary<int, string> _Steam_GameID_Path_Table;
        private static string _SteamLocation = null;
        private static volatile bool IsSteamGameSearchEnded = false;
        public static string GetSteamFolder()
        {
            if (IntPtr.Size == 4)
                return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            else if (IntPtr.Size == 8)
                return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
            else
                throw new Exception("Wow! Interesting!");
        }
        static private void SearchSteamGamesInPC()
        {
            _SteamLocation = GetSteamFolder();
            if (_SteamLocation == null)
                throw new FileNotFoundException("Cannot find Steam Library Location!");
            _Steam_GameID_Path_Table = new Dictionary<int, string>();
            string[] All_Lists = File.ReadAllLines(_SteamLocation + @"\steamapps\libraryfolders.vdf");

            List<string> AllPossiblePath = new List<string>(All_Lists.Length);
            AllPossiblePath.Add(_SteamLocation);

            for (int i = All_Lists.Length - 1; i >= 0; --i)
            {
                string Str = All_Lists[i].Replace("\"", "").Trim().Replace("\t\t", "\t");
                string[] KeyValue = Str.Split('\t');

                if (int.TryParse(KeyValue[0], out _))
                    AllPossiblePath.Add(KeyValue[1]);
            }

            foreach (var PossiblePath in AllPossiblePath)
            {
                var GameManifestPaths = Directory.GetFiles(PossiblePath + "\\steamapps\\", "appmanifest_*.acf");
                foreach (var GameManifestLocation in GameManifestPaths)
                {
                    int GameID_Str_StartIndex = GameManifestLocation.LastIndexOf("appmanifest_") + ("appmanifest_").Length;
                    int GameID_Str_EndIndex = GameManifestLocation.LastIndexOf(".acf");
                    int GameID_Str_Length = GameID_Str_EndIndex - GameID_Str_StartIndex;
                    string GameID_Str = GameManifestLocation.Substring(GameID_Str_StartIndex, GameID_Str_Length);
                    int GameID = int.Parse(GameID_Str);
                    if (_Steam_GameID_Path_Table.ContainsKey(GameID))
                    {
                        var CurrentTarget = File.GetLastWriteTime(PossiblePath + $"\\steamapps\\appmanifest_{GameID}.acf");
                        var PreviousTarget = File.GetLastWriteTime(_Steam_GameID_Path_Table[GameID] + $"\\appmanifest_{GameID}.acf");
                        if (CurrentTarget > PreviousTarget)
                            _Steam_GameID_Path_Table[GameID] = PossiblePath + "\\steamapps";
                    }
                    else
                    {
                        _Steam_GameID_Path_Table.Add(GameID, PossiblePath + "\\steamapps");
                    }
                }
            }
            IsSteamGameSearchEnded = true;
        }
        public SteamAPIHelper()
        {
            if (SteamLocation == null)
                SearchSteamGamesInPC();
            while (IsSteamGameSearchEnded != true)
                Thread.Sleep(50);
        }
        public void StartGame(int ID, string Argument = null)
        {
            Process.Start($"{SteamLocation}\\Steam.exe", $"-applaunch {ID} {Argument}");
        }

    }
}
