using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JetControllerCHI21Interactivity
{
    class HalfLife2_Manager : SteamAPIHelper
    {
        private const string Autoexec_Content = "cl_rumblescale 1\r\njoystick 1\r\njoy_active 1";
        string _HalfLife2Path;
        public string HalfLife2Path
        {
            get
            {
                return _HalfLife2Path;
            }
        }
        string HalfLife2AutoexecPath
        {
            get
            {
                return ($"{_HalfLife2Path}\\hl2\\cfg\\autoexec.cfg");
            }
        }
        string HalfLife2SDL2Path
        {
            get
            {
                return ($"{_HalfLife2Path}\\bin\\SDL2.dll");
            }
        }
        private bool IsHalfLife2Running()
        {
            var HL2_List = Process.GetProcessesByName("hl2");
            return HL2_List.Length != 0;
        }
        public static void KillHalfLife2()
        {
            var HL2_List = Process.GetProcessesByName("hl2");
            foreach (var p in HL2_List)
                p.Kill();
        }
        private bool CheckHalfLIfe2AutoExec()
        {
            if (File.Exists(HalfLife2AutoexecPath))
            {
                if (File.ReadAllText(HalfLife2AutoexecPath).Equals(Autoexec_Content))
                    return true;
            }
            return false;
        }

        private void CreateAutoexec()
        {
            if (File.Exists(HalfLife2AutoexecPath))
                File.Delete(HalfLife2AutoexecPath);
            File.WriteAllText(HalfLife2AutoexecPath, Autoexec_Content);
        }
        private bool IsSDL2_Updated()
        {
            using (var md5 = MD5.Create())
            {
                using (var GameVersion = File.OpenRead(HalfLife2SDL2Path))
                {
                    using (var MyVersion = File.OpenRead("SDL2.dll"))
                    {
                        var hash1 = md5.ComputeHash(GameVersion);
                        var hash2 = md5.ComputeHash(MyVersion);
                        var strHash1 = BitConverter.ToString(hash1);
                        var strHash2 = BitConverter.ToString(hash2);
                        return (strHash1.Equals(strHash2));
                    }
                }
            }
        }
        private bool UpdateSDL2()
        {
            try
            {
                File.Delete(HalfLife2SDL2Path);
                File.Copy("SDL2.dll", HalfLife2SDL2Path);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public HalfLife2_Manager()
        {
            if (!Steam_GameID_Path_Table.Keys.Contains(220))
                throw new FileNotFoundException("Cannot Find Half Life 2!");
            var HalfLife2_SteamPath = Steam_GameID_Path_Table[220];
            if (HalfLife2_SteamPath.EndsWith("\\"))
                _HalfLife2Path = Steam_GameID_Path_Table[220] + "common\\Half-Life 2";
            else
                _HalfLife2Path = Steam_GameID_Path_Table[220] + "\\common\\Half-Life 2";
            if (IsHalfLife2Running())
                KillHalfLife2();
            if (!CheckHalfLIfe2AutoExec())
                CreateAutoexec();
            if (!IsSDL2_Updated())
                UpdateSDL2();
        }
        static string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }
        public bool RunHalfLife2()
        {
            while (IsHalfLife2Running())
                    KillHalfLife2();
            StartGame(220, "-console -vconsole -autoexec");
            for (int i = 0; i < 40; ++i)
            {
                if (!IsHalfLife2Running())
                    Thread.Sleep(500);
                else if (CheckHalfLife2CommandLine())
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static bool CheckHalfLife2CommandLine()
        {
            var HL2_List = Process.GetProcessesByName("hl2");
            Console.WriteLine(GetCommandLine(HL2_List[0]));
            return (GetCommandLine(HL2_List[0]).Contains("-console -vconsole -autoexec"));
        }

    }
}
