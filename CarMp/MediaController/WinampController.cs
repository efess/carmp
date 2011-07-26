using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace CarMP.MediaController
{
    public class WinampController : IMediaController
    {
        private const string WINAMP_PROCESS = "winamp";

        private bool DebugIgnoreCommands;
        private string WinampPath;
        private Process WinampApp;
        private int WinampUpdateInterval;

        public bool Initialized
        {
            get { return WinampApp != null; }
        }

        public WinampController()
        {
            WinampUpdateInterval = 1000;

            Process[] processes = Process.GetProcessesByName(WINAMP_PROCESS);

            // if there is more than one process...

            if (processes.Length > 0)
            {
                WinampApp = processes[0];
            }
            else
            {
                //Start winamp myself, since the SDK is bad at it.
                if (Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\\Applications\\winamp.exe\\shell\\open\\command", null, null) != null)
                {
                }

                if (Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Winamp", "UninstallString", null) != null)
                {
                    WinampPath = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Winamp", "UninstallString", null);
                    WinampPath = WinampPath.Split(new char[] { '"' })[1];
                    WinampPath = WinampPath.Substring(0, WinampPath.LastIndexOf('\\'));

                }
                if (!String.IsNullOrEmpty(WinampPath))
                {
                    ProcessStartInfo WinampPsi = new ProcessStartInfo();
                    WinampPsi.FileName = WinampPath + "\\winamp.exe";
                    //WinampPsi.Arguments = "";
                    //WinampPsi.WindowStyle = ProcessWindowStyle.Normal;
                    WinampApp = System.Diagnostics.Process.Start(WinampPsi);
                    WinampApp.Start();
                }
            }
            System.Threading.Thread.Sleep(500);
            SetDefaultSettings();
        }

        
        
        public void SetDefaultSettings()
        {
            SendUserMessageToWinamp(WA_IPC.IPC_SET_REPEAT, 0);
            SendUserMessageToWinamp(WA_IPC.IPC_SET_SHUFFLE, 0);
        }

        public void PlayFile(string s)
        {
            if (DebugIgnoreCommands) { return; }

            int result;
            //clears the playlist
            SendUserMessageToWinamp(WA_IPC.IPC_DELETE, 0);
            //adds song to playlist
            AddSongToPlayList(s);
            //send winamp the Play command
            StartPlayback();
            //result = Win32Helpers.SendMessage(WA_MSGTYPE.WM_USER, 0, (int)WA_IPC.IPC_ISPLAYING);
            //if (result == 0)
            //{
            //    reloadWinamp();
            //    result = wa.SendToWinamp(0, WM_USER_MSGS.WA_CLEAR_PLAYLIST);
            //    //adds song to playlist
            //    wa.AppendToPlayList(s);
            //    //send winamp the Play command
            //    result = wa.SendToWinamp(WA_MsgTypes.WM_COMMAND, (int)WinAmpSDK.WM_COMMAND_MSGS.WINAMP_BUTTON2, 0);
            //}
        }

        public void AddSongToPlayList(string pFileName)
        {
            Win32Helpers.COPYDATASTRUCT fileStruct = new Win32Helpers.COPYDATASTRUCT();
            fileStruct.dwData = (IntPtr)WA_IPC.IPC_ENQUEUEFILE;
            fileStruct.lpData = pFileName;
            fileStruct.cbData = pFileName.Length + 1;

            SendCopyDataToWinamp(fileStruct, 0);
        }

        public void StartPlayback()
        {
            SendUserMessageToWinamp(WA_IPC.IPC_STARTPLAY, 4);
        }

        public void StopPlayback()
        {
            SendUserMessageToWinamp(WA_IPC.IPC_DELETE, 0);
            SendUserMessageToWinamp(WA_IPC.IPC_STARTPLAY, 4);
            //SendUserMessageToWinamp(WA_IPC.IPC_SPAWNBUTTONPOPUP, 5);
            //if (DebugIgnoreCommands) { return; }

            //int sdj = GetCurrentPos();

            //wa.SendToWinamp(WA_MSGTYPE.WM_COMMAND, (int)WinAmpSDK.WM_COMMAND_MSGS.WINAMP_BUTTON4, 0);
            
            //IntPtr hwnd = FindWindow(m_windowName, null);

            //SendMessageA(hwnd, WM_WA_IPC, 0, 102);

        }

        public void PausePlayback()
        {
            SendUserMessageToWinamp(WA_IPC.IPC_SPAWNBUTTONPOPUP, 3);
        }

        public void SetCurrentPos(int pos)
        {
            if (DebugIgnoreCommands) { return; }
            SendUserMessageToWinamp(WA_IPC.IPC_JUMPTOTIME, pos);
        }
        public int GetCurrentPos()
        {
            if (DebugIgnoreCommands) { return 10000/2; }

            return SendUserMessageToWinamp(WA_IPC.IPC_GETOUTPUTTIME, 0);
        }
        public int GetSongLength()
        {
            // Appears to make winamp jump back some ms in the song
            if (DebugIgnoreCommands) { return 10000; }
            return SendUserMessageToWinamp(WA_IPC.IPC_GETOUTPUTTIME, 1) * 1000;
        }

        private int SendUserMessageToWinamp(WA_IPC pMessageType, int pParameter)
        {
            //return 0;
            if (!Initialized)
            {
                DebugHandler.DebugPrint("Winamp not initialized, cannot send message");
                return -1;
            }
            int returnInt = Win32Helpers.SendMessage(WinampApp.MainWindowHandle, (int)WA_MSGTYPE.WM_USER, pParameter, (int)pMessageType);
            //DebugHandler.DebugPrint("Winamp = " + pMessageType.ToString() + "(" + pParameter.ToString() + ") returned " + returnInt.ToString());
            return returnInt;
        }

        private int SendCopyDataToWinamp(Win32Helpers.COPYDATASTRUCT pData, int pParameter)
        {
            //return 0;
            if (!Initialized)
            {
                DebugHandler.DebugPrint("Winamp not initialized, cannot send message");
                return -1;
            }
            int returnInt = Win32Helpers.SendMessageA(WinampApp.MainWindowHandle, (int)WA_MSGTYPE.WM_COPYDATA, pParameter, ref pData);
            DebugHandler.DebugPrint("Winamp = " + pData.dwData.ToString() + "(" + pData.lpData.ToString() + ", " + pParameter.ToString() + ") returned " + returnInt.ToString());
            return returnInt;
        }

#region Winamp Win32 Message Enums

       // WINAMP SDK COMMAND TYPES
        public enum WA_MSGTYPE
        {
            WM_COPYDATA = 74,
            WM_COMMAND = 273,
            WM_USER = 1024,
        }

        /// <summary>
        /// IPC Messages taken from the Winamp SDK
        /// </summary>
        public enum WA_IPC
        {
            IPC_PLAYFILE = 100  // dont be fooled, this is really the same as enqueufile
            ,IPC_ENQUEUEFILE = 100 
            ,IPC_PLAYFILEW = 1100
            ,IPC_ENQUEUEFILEW = 1100
            /* This is sent as a WM_COPYDATA with IPC_PLAYFILE as the dwData member and the string
            ** of the file / playlist to be enqueued into the playlist editor as the lpData member.
            ** This will just enqueue the file or files since you can use this to enqueue a playlist.
            ** It will not clear the current playlist or change the playback state.
            **
            ** COPYDATASTRUCT cds = {0};
            **   cds.dwData = IPC_ENQUEUEFILE;
            **   cds.lpData = (void*)"c:\\test\\folder\\test.mp3";
            **   cds.cbData = lstrlen((char*)cds.lpData)+1;  // include space for null char
            **   SendMessage(hwnd_winamp,WM_COPYDATA,0,(LPARAM)&cds);
            **
            **
            ** With 2.9+ and all of the 5.x versions you can send this as a normal WM_WA_IPC
            ** (non WM_COPYDATA) with an enqueueFileWithMetaStruct as the param.
            ** If the title member is null then it is treated as a "thing" otherwise it will be
            ** assumed to be a file (for speed).
            **
            ** enqueueFileWithMetaStruct eFWMS = {0};
            **   eFWMS.filename = "c:\\test\\folder\\test.mp3";
            **   eFWMS.title = "Whipping Good";
            **   eFWMS.length = 300;  // this is the number of seconds for the track
            **   SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)&eFWMS,IPC_ENQUEUEFILE);
            */

            ,IPC_DELETE = 101
            ,IPC_DELETE_INT = 1101 
            /* SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_DELETE);
            ** Use this api to clear Winamp's internal playlist.
            ** You should not need to use IPC_DELETE_INT since it is used internally by Winamp when
            ** it is dealing with some lame Windows Explorer issues (hard to believe that!).
            */

            ,IPC_STARTPLAY = 102   
            ,IPC_STARTPLAY_INT = 1102 
            /* SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_STARTPLAY);
            ** Sending this will start playback and is almost the same as hitting the play button.
            ** The IPC_STARTPLAY_INT version is used internally and you should not need to use it
            ** since it won't be any fun.
            */

            ,IPC_CHDIR = 103
            /* This is sent as a WM_COPYDATA type message with IPC_CHDIR as the dwData value and the
            ** directory you want to change to as the lpData member.
            **
            ** COPYDATASTRUCT cds = {0};
            **   cds.dwData = IPC_CHDIR;
            **   cds.lpData = (void*)"c:\\download";
            **   cds.cbData = lstrlen((char*)cds.lpData)+1; // include space for null char
            **   SendMessage(hwnd_winamp,WM_COPYDATA,0,(LPARAM)&cds);
            **
            ** The above example will make Winamp change to the directory 'C:\download'.
            */
            ,IPC_ISPLAYING = 104
            /* int res = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_ISPLAYING);
            ** This is sent to retrieve the current playback state of Winamp.
            ** If it returns 1, Winamp is playing.
            ** If it returns 3, Winamp is paused.
            ** If it returns 0, Winamp is not playing.
            */
            ,IPC_GETOUTPUTTIME = 105
            /* int res = SendMessage(hwnd_winamp,WM_WA_IPC,mode,IPC_GETOUTPUTTIME);
            ** This api can return two different sets of information about current playback status.
            **
            ** If mode = 0 then it will return the position (in ms) of the currently playing track.
            ** Will return -1 if Winamp is not playing.
            **
            ** If mode = 1 then it will return the current track length (in seconds).
            ** Will return -1 if there are no tracks (or possibly if Winamp cannot get the length).
            **
            ** If mode = 2 then it will return the current track length (in milliseconds).
            ** Will return -1 if there are no tracks (or possibly if Winamp cannot get the length).
            */

            ,IPC_JUMPTOTIME = 106
            /* (requires Winamp 1.60+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,ms,IPC_JUMPTOTIME);
            ** This api sets the current position (in milliseconds) for the currently playing song.
            ** The resulting playback position may only be an approximate time since some playback
            ** formats do not provide exact seeking e.g. mp3
            ** This returns -1 if Winamp is not playing, 1 on end of file, or 0 if it was successful.
            */

            ,IPC_GETMODULENAME = 109
            ,IPC_EX_ISRIGHTEXE = 666
            /* usually shouldnt bother using these, but here goes:
            ** send a WM_COPYDATA with IPC_GETMODULENAME, and an internal
            ** flag gets set, which if you send a normal WM_WA_IPC message with
            ** IPC_EX_ISRIGHTEXE, it returns whether or not that filename
            ** matches. lame, I know.
            */

            ,IPC_WRITEPLAYLIST = 120
            /* (requires Winamp 1.666+)
            ** int cur = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_WRITEPLAYLIST);
            **
            ** IPC_WRITEPLAYLIST will write the current playlist to '<winampdir>\\Winamp.m3u' and
            ** will also return the current playlist position (see IPC_GETLISTPOS).
            **
            ** This is kinda obsoleted by some of the newer 2.x api items but it still is good for
            ** use with a front-end program (instead of a plug-in) and you want to see what is in the
            ** current playlist.
            **
            ** This api will only save out extended file information in the #EXTINF entry if Winamp
            ** has already read the data such as if the file was played of scrolled into view. If
            ** Winamp has not read the data then you will only find the file with its filepath entry
            ** (as is the base requirements for a m3u playlist).
            */

            ,IPC_SETPLAYLISTPOS = 121
            /* (requires Winamp 2.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,position,IPC_SETPLAYLISTPOS)
            ** IPC_SETPLAYLISTPOS sets the playlist position to the specified 'position'.
            ** It will not change playback status or anything else. It will just set the current
            ** position in the playlist and will update the playlist view if necessary.
            **
            ** If you use SendMessage(hwnd_winamp,WM_COMMAND,MAKEWPARAM(WINAMP_BUTTON2,0),0);
            ** after using IPC_SETPLAYLISTPOS then Winamp will start playing the file at 'position'.
            */

            ,IPC_SETVOLUME = 122
            /* (requires Winamp 2.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,volume,IPC_SETVOLUME);
            ** IPC_SETVOLUME sets the volume of Winamp (between the range of 0 to 255).
            **
            ** If you pass 'volume' as -666 then the message will return the current volume.
            ** int curvol = SendMessage(hwnd_winamp,WM_WA_IPC,-666,IPC_SETVOLUME);
            */
            ,IPC_SETPANNING = 123
            /* (requires Winamp 2.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,panning,IPC_SETPANNING);
            ** IPC_SETPANNING sets the panning of Winamp from 0 (left) to 255 (right).
            **
            ** At least in 5.x+ this works from -127 (left) to 127 (right).
            **
            ** If you pass 'panning' as -666 to this api then it will return the current panning.
            ** int curpan = SendMessage(hwnd_winamp,WM_WA_IPC,-666,IPC_SETPANNING);
            */

            ,IPC_GETLISTLENGTH = 124
            /* (requires Winamp 2.0+)
            ** int length = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLISTLENGTH);
            ** IPC_GETLISTLENGTH returns the length of the current playlist as the number of tracks.
            */

            ,IPC_GETLISTPOS = 125
            /* (requires Winamp 2.05+)
            ** int pos=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLISTPOS);
            ** IPC_GETLISTPOS returns the current playlist position (which is shown in the playlist
            ** editor as a differently coloured text entry e.g is yellow for the classic skin).
            **
            ** This api is a lot like IPC_WRITEPLAYLIST but a lot faster since it does not have to
            ** write out the whole of the current playlist first.
            */

            ,IPC_GETINFO = 126
            /* (requires Winamp 2.05+)
            ** int inf=SendMessage(hwnd_winamp,WM_WA_IPC,mode,IPC_GETINFO);
            ** IPC_GETINFO returns info about the current playing song. The value
            ** it returns depends on the value of 'mode'.
            ** Mode      Meaning
            ** ------------------
            ** 0         Samplerate, in kilohertz (i.e. 44)
            ** 1         Bitrate  (i.e. 128)
            ** 2         Channels (i.e. 2)
            ** 3 (5+)    Video LOWORD=w HIWORD=h
            ** 4 (5+)    > 65536, string (video description)
            ** 5 (5.25+) Samplerate, in hertz (i.e. 44100)
            */

            ,IPC_GETEQDATA = 127
            /* (requires Winamp 2.05+)
            ** int data=SendMessage(hwnd_winamp,WM_WA_IPC,pos,IPC_GETEQDATA);
            ** IPC_GETEQDATA queries the status of the EQ. 
            ** The value returned depends on what 'pos' is set to:
            ** Value      Meaning
            ** ------------------
            ** 0-9        The 10 bands of EQ data. 0-63 (+20db - -20db)
            ** 10         The preamp value. 0-63 (+20db - -20db)
            ** 11         Enabled. zero if disabled, nonzero if enabled.
            ** 12         Autoload. zero if disabled, nonzero if enabled.
            */

            ,IPC_SETEQDATA = 128
            /* (requires Winamp 2.05+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,pos,IPC_GETEQDATA);
            ** SendMessage(hwnd_winamp,WM_WA_IPC,value,IPC_SETEQDATA);
            ** IPC_SETEQDATA sets the value of the last position retrieved
            ** by IPC_GETEQDATA. This is pretty lame, and we should provide
            ** an extended version that lets you do a MAKELPARAM(pos,value).
            ** someday...
              new (2.92+): 
                if the high byte is set to 0xDB, then the third byte specifies
                which band, and the bottom word specifies the value.
            */

            ,IPC_ADDBOOKMARK = 129
            ,IPC_ADDBOOKMARKW = 131
            /* (requires Winamp 2.4+)
            ** This is sent as a WM_COPYDATA using IPC_ADDBOOKMARK as the dwData value and the
            ** directory you want to change to as the lpData member. This will add the specified
            ** file / url to the Winamp bookmark list.
            **
            ** COPYDATASTRUCT cds = {0};
            **   cds.dwData = IPC_ADDBOOKMARK;
            **   cds.lpData = (void*)"http://www.blah.com/listen.pls";
            **   cds.cbData = lstrlen((char*)cds.lpData)+1; // include space for null char
            **   SendMessage(hwnd_winamp,WM_COPYDATA,0,(LPARAM)&cds);
            **
            **
            ** In Winamp 5.0+ we use this as a normal WM_WA_IPC and the string is null separated as
            ** the filename and then the title of the entry.
            **
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)(char*)"filename\0title\0",IPC_ADDBOOKMARK);
            **
            ** This will notify the library / bookmark editor that a bookmark was added.
            ** Note that using this message in this context does not actually add the bookmark.
            ** Do not use, it is essentially just a notification type message :)
            */

            ,IPC_INSTALLPLUGIN = 130
            /* This is not implemented (and is very unlikely to be done due to safety concerns).
            ** If it was then you could do a WM_COPYDATA with a path to a .wpz and it would then
            ** install the plugin for you.
            **
            ** COPYDATASTRUCT cds = {0};
            **   cds.dwData = IPC_INSTALLPLUGIN;
            **   cds.lpData = (void*)"c:\\path\\to\\file.wpz";
            **   cds.cbData = lstrlen((char*)cds.lpData)+1; // include space for null char
            **   SendMessage(hwnd_winamp,WM_COPYDATA,0,(LPARAM)&cds);
            */

            ,IPC_RESTARTWINAMP = 135
            /* (requires Winamp 2.2+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_RESTARTWINAMP);
            ** IPC_RESTARTWINAMP will restart Winamp (isn't that obvious ? :) )
            ** If this fails to make Winamp start after closing then there is a good chance one (or
            ** more) of the currently installed plugins caused Winamp to crash on exit (either as a
            ** silent crash or a full crash log report before it could call itself start again.
            */

            ,IPC_ISFULLSTOP = 400
            /* (requires winamp 2.7+ I think)
            ** int ret=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_ISFULLSTOP);
            ** This is useful for when you're an output plugin and you want to see if the stop/close
            ** happening is a full stop or if you are just between tracks. This returns non zero if
            ** it is a full stop or zero if it is just a new track.
            ** benski> i think it's actually the other way around - 
            **         !0 for EOF and 0 for user pressing stop
            */

            ,IPC_INETAVAILABLE = 242
            /* (requires Winamp 2.05+)
            ** int val=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_INETAVAILABLE);
            ** IPC_INETAVAILABLE will return 1 if an Internet connection is available for Winamp and
            ** relates to the internet connection type setting on the main general preferences page
            ** in the Winamp preferences.
            */

            ,IPC_UPDTITLE = 243
            /* (requires Winamp 2.2+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_UPDTITLE);
            ** IPC_UPDTITLE will ask Winamp to update the information about the current title and
            ** causes GetFileInfo(..) in the input plugin associated with the current playlist entry
            ** to be called. This can be called such as when an input plugin is buffering a file so
            ** that it can cause the buffer percentage to appear in the playlist.
            */

            ,IPC_REFRESHPLCACHE = 247
            /* (requires Winamp 2.2+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_REFRESHPLCACHE);
            ** IPC_REFRESHPLCACHE will flush the playlist cache buffer and you send this if you want
            ** Winamp to go refetch the titles for all of the entries in the current playlist.
            **
            ** 5.3+: pass a wchar_t * string in wParam, and it'll do a strnicmp() before clearing the cache
            */

            ,IPC_GET_SHUFFLE = 250
            /* (requires Winamp 2.4+)
            ** int val=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GET_SHUFFLE);
            ** IPC_GET_SHUFFLE returns the status of the shuffle option.
            ** If set then it will return 1 and if not set then it will return 0.
            */

            ,IPC_GET_REPEAT = 251
            /* (requires Winamp 2.4+)
            ** int val=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GET_REPEAT);
            ** IPC_GET_REPEAT returns the status of the repeat option.
            ** If set then it will return 1 and if not set then it will return 0.
            */

            ,IPC_SET_SHUFFLE = 252
            /* (requires Winamp 2.4+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,value,IPC_SET_SHUFFLE);
            ** IPC_SET_SHUFFLE sets the status of the shuffle option.
            ** If 'value' is 1 then shuffle is turned on.
            ** If 'value' is 0 then shuffle is turned off.
            */

            ,IPC_SET_REPEAT = 253
            /* (requires Winamp 2.4+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,value,IPC_SET_REPEAT);
            ** IPC_SET_REPEAT sets the status of the repeat option.
            ** If 'value' is 1 then shuffle is turned on.
            ** If 'value' is 0 then shuffle is turned off.
            */

            ,IPC_ENABLEDISABLE_ALL_WINDOWS = 259 // = 0xdeadbeef to disable
            /* (requires Winamp 2.9+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(enable?0:0xdeadbeef),IPC_ENABLEDISABLE_ALL_WINDOWS);
            ** Sending this message with 0xdeadbeef as the param will disable all winamp windows and
            ** any other values will enable all of the Winamp windows again. When disabled you won't
            ** get any response on clicking or trying to do anything to the Winamp windows. If the
            ** taskbar icon is shown then you may still have control ;)
            */

            ,IPC_GETWND = 260
            /* (requires Winamp 2.9+)
            ** HWND h=SendMessage(hwnd_winamp,WM_WA_IPC,IPC_GETWND_xxx,IPC_GETWND);
            ** returns the HWND of the window specified.
            */
            ,IPC_GETWND_EQ = 0 // use one of these for the param
            ,IPC_GETWND_PE = 1
            ,IPC_GETWND_MB = 2
             ,IPC_GETWND_VIDEO = 3
            ,IPC_ISWNDVISIBLE = 261 // same param as IPC_GETWND

            /************************************************************************
            ***************** in-process only (WE LOVE PLUGINS)
            ************************************************************************/
            ,IPC_SETSKINW = 199
            ,IPC_SETSKIN = 200
            /* (requires Winamp 2.04+, only usable from plug-ins (not external apps))
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)"skinname",IPC_SETSKIN);
            ** IPC_SETSKIN sets the current skin to "skinname". Note that skinname 
            ** can be the name of a skin, a skin .zip file, with or without path. 
            ** If path isn't specified, the default search path is the winamp skins 
            ** directory.
            */

            ,IPC_GETSKIN = 201
            ,IPC_GETSKINW = 1201
            /* (requires Winamp 2.04+, only usable from plug-ins (not external apps))
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)skinname_buffer,IPC_GETSKIN);
            ** IPC_GETSKIN puts the directory where skin bitmaps can be found 
            ** into  skinname_buffer.
            ** skinname_buffer must be MAX_PATH characters in length.
            ** When using a .zip'd skin file, it'll return a temporary directory
            ** where the ZIP was decompressed.
            */

            ,IPC_EXECPLUG = 202
            /* (requires Winamp 2.04+, only usable from plug-ins (not external apps))
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)"vis_file.dll",IPC_EXECPLUG);
            ** IPC_EXECPLUG executes a visualization plug-in pointed to by WPARAM.
            ** the format of this string can be:
            ** "vis_whatever.dll"
            ** "vis_whatever.dll,0" // (first mod, file in winamp plug-in dir)
            ** "C:\\dir\\vis_whatever.dll,1" 
            */

            ,IPC_GETPLAYLISTFILE = 211
            ,IPC_GETPLAYLISTFILEW = 214
            /* (requires Winamp 2.04+, only usable from plug-ins (not external apps))
            ** char *name=SendMessage(hwnd_winamp,WM_WA_IPC,index,IPC_GETPLAYLISTFILE);
            ** IPC_GETPLAYLISTFILE gets the filename of the playlist entry [index].
            ** returns a pointer to it. returns NULL on error.
            */

            ,IPC_GETPLAYLISTTITLE = 212
            ,IPC_GETPLAYLISTTITLEW = 213
            /* (requires Winamp 2.04+, only usable from plug-ins (not external apps))
            ** char *name=SendMessage(hwnd_winamp,WM_WA_IPC,index,IPC_GETPLAYLISTTITLE);
            **
            ** IPC_GETPLAYLISTTITLE gets the title of the playlist entry [index].
            ** returns a pointer to it. returns NULL on error.
            */

            ,IPC_GETHTTPGETTER = 240
            /* retrieves a function pointer to a HTTP retrieval function.
            ** if this is unsupported, returns 1 or 0.
            ** the function should be:
            ** int (*httpRetrieveFile)(HWND hwnd, char *url, char *file, char *dlgtitle);
            ** if you call this function, with a parent window, a URL, an output file, and a dialog title,
            ** it will return 0 on successful download, 1 on error.
            */

            ,IPC_GETHTTPGETTERW = 1240
            /* int (*httpRetrieveFileW)(HWND hwnd, char *url, wchar_t *file, wchar_t *dlgtitle); */

            ,IPC_MBOPEN = 241
            /* (requires Winamp 2.05+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_MBOPEN);
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)url,IPC_MBOPEN);
            ** IPC_MBOPEN will open a new URL in the minibrowser. if url is NULL, it will open the Minibrowser window.
            */

            ,IPC_CHANGECURRENTFILE = 245
            /* (requires Winamp 2.05+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)file,IPC_CHANGECURRENTFILE);
            ** IPC_CHANGECURRENTFILE will set the current playlist item.
            */

            ,IPC_CHANGECURRENTFILEW = 1245
            /* (requires Winamp 5.3+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)file,IPC_CHANGECURRENTFILEW);
            ** IPC_CHANGECURRENTFILEW will set the current playlist item.
            */

            ,IPC_GETMBURL = 246
            /* (requires Winamp 2.2+)
            ** char buffer[4096]; // Urls can be VERY long
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)buffer,IPC_GETMBURL);
            ** IPC_GETMBURL will retrieve the current Minibrowser URL into buffer.
            ** buffer must be at least 4096 bytes long.
            */

            ,IPC_MBBLOCK = 248
            /* (requires Winamp 2.4+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,value,IPC_MBBLOCK);
            **
            ** IPC_MBBLOCK will block the Minibrowser from updates if value is set to 1
            */

            ,IPC_MBOPENREAL = 249
            /* (requires Winamp 2.4+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)url,IPC_MBOPENREAL);
            **
            ** IPC_MBOPENREAL works the same as IPC_MBOPEN except that it will works even if 
            ** IPC_MBBLOCK has been set to 1
            */

            ,IPC_ADJUST_OPTIONSMENUPOS = 280
            /* (requires Winamp 2.9+)
            ** int newpos=SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)adjust_offset,IPC_ADJUST_OPTIONSMENUPOS);
            ** moves where winamp expects the Options menu in the main menu. Useful if you wish to insert a
            ** menu item above the options/skins/vis menus.
            */

            ,IPC_GET_HMENU = 281
            /* (requires Winamp 2.9+)
            ** HMENU hMenu=SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)0,IPC_GET_HMENU);
            ** values for data:
            ** 0 : main popup menu 
            ** 1 : main menubar file menu
            ** 2 : main menubar options menu
            ** 3 : main menubar windows menu
            ** 4 : main menubar help menu
            ** other values will return NULL.
            */

            ,IPC_GET_EXTENDED_FILE_INFO = 290 //pass a pointer to the following struct in wParam
            ,IPC_GET_EXTENDED_FILE_INFO_HOOKABLE = 296       
            // the following IPC use waSpawnMenuParms as parameter
            , IPC_SPAWNEQPRESETMENU = 933
            , IPC_SPAWNFILEMENU = 934 //menubar
            , IPC_SPAWNOPTIONSMENU = 935 //menubar
            , IPC_SPAWNWINDOWSMENU = 936 //menubar
            , IPC_SPAWNHELPMENU = 937 //menubar
            , IPC_SPAWNPLAYMENU = 938 //menubar
            , IPC_SPAWNPEFILEMENU = 939 //menubar
            , IPC_SPAWNPEPLAYLISTMENU = 940 //menubar
            , IPC_SPAWNPESORTMENU = 941 //menubar
            , IPC_SPAWNPEHELPMENU = 942 //menubar
            , IPC_SPAWNMLFILEMENU = 943 //menubar
            , IPC_SPAWNMLVIEWMENU = 944 //menubar
            , IPC_SPAWNMLHELPMENU = 945 //menubar
            , IPC_SPAWNPELISTOFPLAYLISTS = 946
            , IPC_IS_PLAYING_VIDEO = 501 // returns >1 if playing, 0 if not, 1 if old version (so who knows):)
            , IPC_GET_IVIDEOOUTPUT = 500 // see below for IVideoOutput interface
            , VIDUSER_SET_INFOSTRING = 0x1000
            , VIDUSER_GET_VIDEOHWND  = 0x1001
            , VIDUSER_SET_VFLIP      = 0x1002
            , VIDUSER_SET_TRACKSELINTERFACE = 0x1003 // give your ITrackSelector interface as param2
            , VIDUSER_OPENVIDEORENDERER = 0x1004
            , VIDUSER_CLOSEVIDEORENDERER = 0x1005
            , VIDUSER_GETPOPUPMENU = 0x1006
            , VIDUSER_SET_INFOSTRINGW = 0x1007
            /* Example of using IPC_CB_MISC_STATUS to detect the start of track playback with 5.x
            **
            ** if(lParam == IPC_CB_MISC && wParam == IPC_CB_MISC_STATUS)
            ** {
            **   if(SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_ISPLAYING) == 1 &&
            **      !SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETOUTPUTTIME))
            **   {
            **     char* file = (char*)SendMessage(hwnd_winamp,WM_WA_IPC,
            **                  SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLISTPOS),IPC_GETPLAYLISTFILE);
            **     // only output if a valid file was found
            **     if(file)
            **     {
            **       MessageBox(hwnd_winamp,file,"starting",0);
            **       // or do something else that you need to do
            **     }
            **   }
            ** }
            */

            , IPC_CB_CONVERT_STATUS = 604 // param value goes from 0 to 100 (percent)
            , IPC_CB_CONVERT_DONE   = 605

            , IPC_ADJUST_FFWINDOWSMENUPOS = 606
            /* (requires Winamp 2.9+)
            ** int newpos=SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)adjust_offset,IPC_ADJUST_FFWINDOWSMENUPOS);
            ** This will move where Winamp expects the freeform windows in the menubar windows main
            ** menu. This is useful if you wish to insert a menu item above extra freeform windows.
            */

            , IPC_ISDOUBLESIZE = 608
            /* (requires Winamp 5.0+)
            ** int dsize=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_ISDOUBLESIZE);
            ** You send this to Winamp to query if the double size mode is enabled or not.
            ** If it is on then this will return 1 otherwise it will return 0.
            */

            , IPC_ADJUST_FFOPTIONSMENUPOS = 609
            /* (requires Winamp 2.9+)
            ** int newpos=SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)adjust_offset,IPC_ADJUST_FFOPTIONSMENUPOS);
            ** moves where winamp expects the freeform preferences item in the menubar windows main
            ** menu. This is useful if you wish to insert a menu item above the preferences item.
            **
            ** Note: This setting was ignored by gen_ff until it was fixed in 5.1
            **       gen_ff would assume thatthe menu position was 11 in all cases and so when you
            **       had two plugins attempting to add entries into the main right click menu it
            **       would cause the 'colour themes' submenu to either be incorrectly duplicated or
            **       to just disappear.instead.
            */

            , IPC_GETTIMEDISPLAYMODE = 610
            /* (requires Winamp 5.0+)
            ** int mode=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETTIMEDISPLAYMODE);
            ** This will return the status of the time display i.e. shows time elapsed or remaining.
            ** This returns 0 if Winamp is displaying time elapsed or 1 for the time remaining.
            */

            , IPC_SETVISWND = 611
            /* (requires Winamp 5.0+)
            ** int viswnd=(HWND)SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)(HWND)viswnd,IPC_SETVISWND);
            ** This allows you to set a window to receive the following message commands (which are
            ** used as part of the modern skin integration).
            ** When you have finished or your visualisation is closed then send wParam as zero to
            ** ensure that things are correctly tidied up.
            */
            /* The following messages are received as the LOWORD(wParam) of the WM_COMMAND message.
            ** See %SDK%\winamp\wa5vis.txt for more info about visualisation integration in Winamp.
            */
            , ID_VIS_NEXT                     = 40382
            , ID_VIS_PREV                     = 40383
            , ID_VIS_RANDOM                   = 40384
            , ID_VIS_FS                       = 40389
            , ID_VIS_CFG                      = 40390
            , ID_VIS_MENU                     = 40391


            , IPC_GETVISWND = 612
            /* (requires Winamp 5.0+)
            ** int viswnd=(HWND)SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETVISWND);
            ** This returns a HWND to the visualisation command handler window if set by IPC_SETVISWND.
            */


            , IPC_ISVISRUNNING = 613
            /* (requires Winamp 5.0+)
            ** int visrunning=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_ISVISRUNNING);
            ** This will return 1 if a visualisation is currently running and 0 if one is not running.
            */


            , IPC_CB_VISRANDOM = 628 // param is status of random


            , IPC_SETIDEALVIDEOSIZE = 614
            /* (requires Winamp 5.0+)
            ** This is sent by Winamp back to itself so it can be trapped and adjusted as needed with
            ** the desired width in HIWORD(wParam) and the desired height in LOWORD(wParam).
            **
            ** if(uMsg == WM_WA_IPC){
            **   if(lParam == IPC_SETIDEALVIDEOSIZE){
            **      wParam = MAKEWPARAM(height,width);
            **   }
            ** }
            */


            , IPC_GETSTOPONVIDEOCLOSE = 615
            /* (requires Winamp 5.0+)
            ** int sovc=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETSTOPONVIDEOCLOSE);
            ** This will return 1 if 'stop on video close' is enabled and 0 if it is disabled.
            */


            , IPC_SETSTOPONVIDEOCLOSE = 616
            /* (requires Winamp 5.0+)
            ** int sovc=SendMessage(hwnd_winamp,WM_WA_IPC,enabled,IPC_SETSTOPONVIDEOCLOSE);
            ** Set enabled to 1 to enable and 0 to disable the 'stop on video close' option.
            */
                        
            , IPC_CB_ONTOGGLEAOT = 618 


            , IPC_GETPREFSWND = 619
            /* (requires Winamp 5.0+)
            ** HWND prefs = (HWND)SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETPREFSWND);
            ** This will return a handle to the preferences dialog if it is open otherwise it will
            ** return zero. A simple check with the OS api IsWindow(..) is a good test if it's valid.
            **
            ** e.g.  this will open (or close if already open) the preferences dialog and show if we
            **       managed to get a valid 
            ** SendMessage(hwnd_winamp,WM_COMMAND,MAKEWPARAM(WINAMP_OPTIONS_PREFS,0),0);
            ** MessageBox(hwnd_winamp,(IsWindow((HWND)SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETPREFSWND))?"Valid":"Not Open"),0,MB_OK);
            */


            , IPC_SET_PE_WIDTHHEIGHT = 620
            /* (requires Winamp 5.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,(WPARAM)&point,IPC_SET_PE_WIDTHHEIGHT);
            ** You pass a pointer to a POINT structure which holds the width and height and Winamp
            ** will set the playlist editor to that size (this is used by gen_ff on skin changes).
            ** There does not appear to be any bounds limiting with this so it is possible to create
            ** a zero size playlist editor window (which is a pretty silly thing to do).
            */


            , IPC_GETLANGUAGEPACKINSTANCE = 621
            /* (requires Winamp 5.0+)
            ** HINSTANCE hInst = (HINSTANCE)SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLANGUAGEPACKINSTANCE);
            ** This will return the HINSTANCE to the currently used language pack file for winamp.exe
            **
            ** (5.5+)
            ** If you pass 1 in wParam then you will have zero returned if a language pack is in use.
            ** if(!SendMessage(hwnd_winamp,WM_WA_IPC,1,IPC_GETLANGUAGEPACKINSTANCE)){
            **   // winamp is currently using a language pack
            ** }
            **
            ** If you pass 2 in wParam then you will get the path to the language pack folder.
            ** wchar_t* lngpackfolder = (wchar_t*)SendMessage(hwnd_winamp,WM_WA_IPC,2,IPC_GETLANGUAGEPACKINSTANCE);
            **
            ** If you pass 3 in wParam then you will get the path to the currently extracted language pack.
            ** wchar_t* lngpack = (wchar_t*)SendMessage(hwnd_winamp,WM_WA_IPC,3,IPC_GETLANGUAGEPACKINSTANCE);
            **
            ** If you pass 4 in wParam then you will get the name of the currently used language pack.
            ** wchar_t* lngname = (char*)SendMessage(hwnd_winamp,WM_WA_IPC,4,IPC_GETLANGUAGEPACKINSTANCE);
            */
           

            , IPC_CB_PEINFOTEXT = 622 // data is a string, ie: "04:21/45:02"


            , IPC_CB_OUTPUTCHANGED = 623 // output plugin was changed in config


            , IPC_GETOUTPUTPLUGIN = 625
            /* (requires Winamp 5.0+)
            ** char* outdll = (char*)SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETOUTPUTPLUGIN);
            ** This returns a string of the current output plugin's dll name.
            ** e.g. if the directsound plugin was selected then this would return 'out_ds.dll'.
            */


            , IPC_SETDRAWBORDERS = 626
            /* (requires Winamp 5.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,enabled,IPC_SETDRAWBORDERS);
            ** Set enabled to 1 to enable and 0 to disable drawing of the playlist editor and winamp
            ** gen class windows (used by gen_ff to allow it to draw its own window borders).
            */


            , IPC_DISABLESKINCURSORS = 627
            /* (requires Winamp 5.0+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,enabled,IPC_DISABLESKINCURSORS);
            ** Set enabled to 1 to enable and 0 to disable the use of skinned cursors.
            */


            , IPC_GETSKINCURSORS = 628
            /* (requires Winamp 5.36+)
            ** data = (WACURSOR)cursorId. (check wa_dlg.h for values)
            */


            , IPC_CB_RESETFONT = 629


            , IPC_IS_FULLSCREEN = 630
            /* (requires Winamp 5.0+)
            ** int val=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_IS_FULLSCREEN);
            ** This will return 1 if the video or visualisation is in fullscreen mode or 0 otherwise.
            */


            , IPC_SET_VIS_FS_FLAG = 631
            /* (requires Winamp 5.0+)
            ** A vis should send this message with 1/as param to notify winamp that it has gone to or has come back from fullscreen mode
            */


            , IPC_SHOW_NOTIFICATION = 632


            , IPC_GETSKININFO = 633
            , IPC_GETSKININFOW = 1633
            /* (requires Winamp 5.0+)
            ** This is a notification message sent to the main Winamp window by itself whenever it
            ** needs to get information to be shown about the current skin in the 'Current skin
            ** information' box on the main Skins page in the Winamp preferences.
            **
            ** When this notification is received and the current skin is one you are providing the
            ** support for then you return a valid buffer for Winamp to be able to read from with
            ** information about it such as the name of the skin file.
            **
            ** if(uMsg == WM_WA_IPC && lParam == IPC_GETSKININFO){
            **   if(is_our_skin()){
            **      return is_our_skin_name();
            **   }
            ** }
            */


            , IPC_GET_MANUALPLADVANCE = 634
            /* (requires Winamp 5.03+)
            ** int val=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GET_MANUALPLADVANCE);
            ** IPC_GET_MANUALPLADVANCE returns the status of the Manual Playlist Advance.
            ** If enabled this will return 1 otherwise it will return 0.
            */


            , IPC_SET_MANUALPLADVANCE = 635
            /* (requires Winamp 5.03+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,value,IPC_SET_MANUALPLADVANCE);
            ** IPC_SET_MANUALPLADVANCE sets the status of the Manual Playlist Advance option.
            ** Set value = 1 to turn it on and value = 0 to turn it off.
            */


            , IPC_GET_NEXT_PLITEM = 636
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_EOF_GET_NEXT_PLITEM);
            **
            ** Sent to Winamp's main window when an item has just finished playback or the next
            ** button has been pressed and requesting the new playlist item number to go to.
            **
            ** Subclass this message in your application to return the new item number.
            ** Return -1 for normal Winamp operation (default) or the new item number in
            ** the playlist to be played instead of the originally selected next track.
            **
            ** This is primarily provided for the JTFE plugin (gen_jumpex.dll).
            */


            , IPC_GET_PREVIOUS_PLITEM = 637
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_EOF_GET_PREVIOUS_PLITEM);
            **
            ** Sent to Winamp's main window when the previous button has been pressed and Winamp is
            ** requesting the new playlist item number to go to.
            **
            ** Return -1 for normal Winamp operation (default) or the new item number in
            ** the playlist to be played instead of the originally selected previous track.
            **
            ** This is primarily provided for the JTFE plugin (gen_jumpex.dll).
            */


            , IPC_IS_WNDSHADE = 638
            /* (requires Winamp 5.04+)
            ** int is_shaded=SendMessage(hwnd_winamp,WM_WA_IPC,wnd,IPC_IS_WNDSHADE);
            ** Pass 'wnd' as an id as defined for IPC_GETWND or pass -1 to query the status of the
            ** main window. This returns 1 if the window is in winshade mode and 0 if it is not.
            ** Make sure you only test for this on a 5.04+ install otherwise you get a false result.
            ** (See the notes about unhandled WM_WA_IPC messages).
            */


            , IPC_SETRATING = 639 
            /* (requires Winamp 5.04+ with ML)
            ** int rating=SendMessage(hwnd_winamp,WM_WA_IPC,rating,IPC_SETRATING);
            ** This will allow you to set the 'rating' on the current playlist entry where 'rating'
            ** is an integer value from 0 (no rating) to 5 (5 stars).
            **
            ** The following example should correctly allow you to set the rating for any specified
            ** playlist entry assuming of course that you're trying to get a valid playlist entry.
            **
            ** void SetPlaylistItemRating(int item_to_set, int rating_to_set){
            ** int cur_pos=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLISTPOS);
            **   SendMessage(hwnd_winamp,WM_WA_IPC,item_to_set,IPC_SETPLAYLISTPOS);
            **   SendMessage(hwnd_winamp,WM_WA_IPC,rating_to_set,IPC_SETRATING);
            **   SendMessage(hwnd_winamp,WM_WA_IPC,cur_pos,IPC_SETPLAYLISTPOS);
            ** }
            */


            , IPC_GETRATING = 640 
            /* (requires Winamp 5.04+ with ML)
            ** int rating=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETRATING);
            ** This returns the current playlist entry's rating between 0 (no rating) to 5 (5 stars).
            **
            ** The following example should correctly allow you to get the rating for any specified
            ** playlist entry assuming of course that you're trying to get a valid playlist entry.
            **
            ** int GetPlaylistItemRating(int item_to_get, int rating_to_set){
            ** int cur_pos=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETLISTPOS), rating = 0;
            **   SendMessage(hwnd_winamp,WM_WA_IPC,item_to_get,IPC_SETPLAYLISTPOS);
            **   rating = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETRATING);
            **   SendMessage(hwnd_winamp,WM_WA_IPC,cur_pos,IPC_SETPLAYLISTPOS);
            **   return rating;
            ** }
            */


            , IPC_GETNUMAUDIOTRACKS = 641
            /* (requires Winamp 5.04+)
            ** int n = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETNUMAUDIOTRACKS);
            ** This will return the number of audio tracks available from the currently playing item.
            */


            , IPC_GETNUMVIDEOTRACKS = 642
            /* (requires Winamp 5.04+)
            ** int n = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETNUMVIDEOTRACKS);
            ** This will return the number of video tracks available from the currently playing item.
            */


            , IPC_GETAUDIOTRACK = 643
            /* (requires Winamp 5.04+)
            ** int cur = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETAUDIOTRACK);
            ** This will return the id of the current audio track for the currently playing item.
            */


            , IPC_GETVIDEOTRACK = 644
            /* (requires Winamp 5.04+)
            ** int cur = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETVIDEOTRACK);
            ** This will return the id of the current video track for the currently playing item.
            */


            , IPC_SETAUDIOTRACK = 645
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,track,IPC_SETAUDIOTRACK);
            ** This allows you to switch to a new audio track (if supported) in the current playing file.
            */


            , IPC_SETVIDEOTRACK = 646
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,track,IPC_SETVIDEOTRACK);
            ** This allows you to switch to a new video track (if supported) in the current playing file.
            */


            , IPC_PUSH_DISABLE_EXIT = 647
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_PUSH_DISABLE_EXIT);
            ** This will let you disable or re-enable the UI exit functions (close button, context
            ** menu, alt-f4). Remember to call IPC_POP_DISABLE_EXIT when you are done doing whatever
            ** was required that needed to prevent exit otherwise you have to kill the Winamp process.
            */


            , IPC_POP_DISABLE_EXIT  = 648
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_POP_DISABLE_EXIT);
            ** See IPC_PUSH_DISABLE_EXIT
            */


            , IPC_IS_EXIT_ENABLED = 649
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_IS_EXIT_ENABLED);
            ** This will return 0 if the 'exit' option of Winamp's menu is disabled and 1 otherwise.
            */


            , IPC_IS_AOT = 650
            /* (requires Winamp 5.04+)
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_IS_AOT);
            ** This will return the status of the always on top flag.
            ** Note: This may not match the actual TOPMOST window flag while another fullscreen
            ** application is focused if the user has the 'Disable always on top while fullscreen
            ** applications are focused' option under the  General Preferences page is checked.
            */


            , IPC_USES_RECYCLEBIN = 651
            /* (requires Winamp 5.09+)
            ** int use_bin=SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_USES_RECYCLEBIN);
            ** This will return 1 if the deleted file should be sent to the recycle bin or
            ** 0 if deleted files should be deleted permanently (default action for < 5.09).
            **
            ** Note: if you use this on pre 5.09 installs of Winamp then it will return 1 which is
            ** not correct but is due to the way that SendMessage(..) handles un-processed messages.
            ** Below is a quick case for checking if the returned value is correct.
            **
            ** if(SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_USES_RECYCLEBIN) &&
            **    SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_GETVERSION)>=0x5009)
            ** {
            **   // can safely follow the option to recycle the file
            ** }
            ** else
            *  {
            **   // need to do a permanent delete of the file
            ** }
            */


            , IPC_FLUSHAUDITS = 652
            /*
            ** SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_FLUSHAUDITS);
            ** 
            ** Will flush any pending audits in the global audits queue
            **
            */

            , IPC_GETPLAYITEM_START = 653
            , IPC_GETPLAYITEM_END   = 654


            , IPC_GETVIDEORESIZE = 655
            , IPC_SETVIDEORESIZE = 656


            , IPC_INITIAL_SHOW_STATE = 657
            /* (requires Winamp 5.36+)
            ** int show_state = SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_INITIAL_SHOW_STATE);
            ** returns the processed value of nCmdShow when Winamp was started
            ** (see MSDN documentation the values passed to WinMain(..) for what this should be)
            **
            ** e.g.
            ** if(SendMessage(hwnd_winamp,WM_WA_IPC,0,IPC_INITIAL_SHOW_STATE) == SW_SHOWMINIMIZED){
            **   // we are starting minimised so process as needed (keep our window hidden)
            ** }
            **
            ** Useful for seeing if winamp was run minimised on startup so you can act accordingly.
            ** On pre-5.36 versions this will effectively return SW_NORMAL/SW_SHOWNORMAL due to the
            ** handling of unknown apis returning 1 from Winamp.
            */
            
            , IPC_SPAWNBUTTONPOPUP = 361 // param =
            // 0 = eject
            // 1 = previous
            // 2 = next
            // 3 = pause
            // 4 = play
            // 5 = stop
        }

        // WINAMP SDK COMMANDS
        public enum WM_COMMAND_MSGS
        {
            WINAMP_FILE_QUIT = 40001,
            WINAMP_OPTIONS_PREFS = 40012,
            WINAMP_OPTIONS_AOT = 40019,
            WINAMP_FILE_REPEAT = 40022,
            WINAMP_FILE_SHUFFLE = 40023,
            WINAMP_HIGH_PRIORITY = 40025,
            WINAMP_FILE_PLAY = 40029,
            WINAMP_OPTIONS_EQ = 40036,
            WINAMP_OPTIONS_ELAPSED = 40037,
            WINAMP_OPTIONS_REMAINING = 40038,
            WINAMP_OPTIONS_PLEDIT = 40040,
            WINAMP_HELP_ABOUT = 40041,
            WINAMP_MAINMENU = 40043,
            WINAMP_BUTTON1 = 40044,
            WINAMP_BUTTON2 = 40045,
            WINAMP_BUTTON3 = 40046,
            WINAMP_BUTTON4 = 40047,
            WINAMP_BUTTON5 = 40048,
            WINAMP_VOLUMEUP = 40058,
            WINAMP_VOLUMEDOWN = 40059,
            WINAMP_FFWD5S = 40060,
            WINAMP_REW5S = 40061,
            WINAMP_NEXT_WINDOW = 40063,
            WINAMP_OPTIONS_WINDOWSHADE = 40064,
            WINAMP_BUTTON1_SHIFT = 40144,
            WINAMP_BUTTON2_SHIFT = 40145,
            WINAMP_BUTTON3_SHIFT = 40146,
            WINAMP_BUTTON4_SHIFT = 40147,
            WINAMP_BUTTON5_SHIFT = 40148,
            WINAMP_BUTTON1_CTRL = 40154,
            WINAMP_BUTTON2_CTRL = 40155,
            WINAMP_BUTTON3_CTRL = 40156,
            WINAMP_BUTTON4_CTRL = 40157,
            WINAMP_BUTTON5_CTRL = 40158,
            WINAMP_OPTIONS_DSIZE = 40165,
            IDC_SORT_FILENAME = 40166,
            IDC_SORT_FILETITLE = 40167,
            IDC_SORT_ENTIREFILENAME = 40168,
            IDC_SELECTALL = 40169,
            IDC_SELECTNONE = 40170,
            IDC_SELECTINV = 40171,
            IDM_EQ_LOADPRE = 40172,
            IDM_EQ_LOADMP3 = 40173,
            IDM_EQ_LOADDEFAULT = 40174,
            IDM_EQ_SAVEPRE = 40175,
            IDM_EQ_SAVEMP3 = 40176,
            IDM_EQ_SAVEDEFAULT = 40177,
            IDM_EQ_DELPRE = 40178,
            IDM_EQ_DELMP3 = 40180,
            IDC_PLAYLIST_PLAY = 40184,
            WINAMP_FILE_LOC = 40185,
            WINAMP_OPTIONS_EASYMOVE = 40186,
            WINAMP_FILE_DIR = 40187,
            WINAMP_EDIT_ID3 = 40188,
            WINAMP_TOGGLE_AUTOSCROLL = 40189,
            WINAMP_VISSETUP = 40190,
            WINAMP_PLGSETUP = 40191,
            WINAMP_VISPLUGIN = 40192,
            WINAMP_JUMP = 40193,
            WINAMP_JUMPFILE = 40194,
            WINAMP_JUMP10FWD = 40195,
            WINAMP_JUMP10BACK = 40197,
            WINAMP_PREVSONG = 40198,
            WINAMP_OPTIONS_EXTRAHQ = 40200,
            ID_PE_NEW = 40201,
            ID_PE_OPEN = 40202,
            ID_PE_SAVE = 40203,
            ID_PE_SAVEAS = 40204,
            ID_PE_SELECTALL = 40205,
            ID_PE_INVERT = 40206,
            ID_PE_NONE = 40207,
            ID_PE_ID3 = 40208,
            ID_PE_S_TITLE = 40209,
            ID_PE_S_FILENAME = 40210,
            ID_PE_S_PATH = 40211,
            ID_PE_S_RANDOM = 40212,
            ID_PE_S_REV = 40213,
            ID_PE_CLEAR = 40214,
            ID_PE_MOVEUP = 40215,
            ID_PE_MOVEDOWN = 40216,
            WINAMP_SELSKIN = 40219,
            WINAMP_VISCONF = 40221,
            ID_PE_NONEXIST = 40222,
            ID_PE_DELETEFROMDISK = 40223,
            ID_PE_CLOSE = 40224,
            WINAMP_VIS_SETOSC = 40226,
            WINAMP_VIS_SETANA = 40227,
            WINAMP_VIS_SETOFF = 40228,
            WINAMP_VIS_DOTSCOPE = 40229,
            WINAMP_VIS_LINESCOPE = 40230,
            WINAMP_VIS_SOLIDSCOPE = 40231,
            WINAMP_VIS_NORMANA = 40233,
            WINAMP_VIS_FIREANA = 40234,
            WINAMP_VIS_LINEANA = 40235,
            WINAMP_VIS_NORMVU = 40236,
            WINAMP_VIS_SMOOTHVU = 40237,
            WINAMP_VIS_FULLREF = 40238,
            WINAMP_VIS_FULLREF2 = 40239,
            WINAMP_VIS_FULLREF3 = 40240,
            WINAMP_VIS_FULLREF4 = 40241,
            WINAMP_OPTIONS_TOGTIME = 40242,
            EQ_ENABLE = 40244,
            EQ_AUTO = 40245,
            EQ_PRESETS = 40246,
            WINAMP_VIS_FALLOFF0 = 40247,
            WINAMP_VIS_FALLOFF1 = 40248,
            WINAMP_VIS_FALLOFF2 = 40249,
            WINAMP_VIS_FALLOFF3 = 40250,
            WINAMP_VIS_FALLOFF4 = 40251,
            WINAMP_VIS_PEAKS = 40252,
            ID_LOAD_EQF = 40253,
            ID_SAVE_EQF = 40254,
            ID_PE_ENTRY = 40255,
            ID_PE_SCROLLUP = 40256,
            ID_PE_SCROLLDOWN = 40257,
            WINAMP_MAIN_WINDOW = 40258,
            WINAMP_VIS_PFALLOFF0 = 40259,
            WINAMP_VIS_PFALLOFF1 = 40260,
            WINAMP_VIS_PFALLOFF2 = 40261,
            WINAMP_VIS_PFALLOFF3 = 40262,
            WINAMP_VIS_PFALLOFF4 = 40263,
            ID_PE_TOP = 40264,
            ID_PE_BOTTOM = 40265,
            WINAMP_OPTIONS_WINDOWSHADE_PL = 40266,
            EQ_INC1 = 40267,
            EQ_INC2 = 40268,
            EQ_INC3 = 40269,
            EQ_INC4 = 40270,
            EQ_INC5 = 40271,
            EQ_INC6 = 40272,
            EQ_INC7 = 40273,
            EQ_INC8 = 40274,
            EQ_INC9 = 40275,
            EQ_INC10 = 40276,
            EQ_INCPRE = 40277,
            EQ_DECPRE = 40278,
            EQ_DEC1 = 40279,
            EQ_DEC2 = 40280,
            EQ_DEC3 = 40281,
            EQ_DEC4 = 40282,
            EQ_DEC5 = 40283,
            EQ_DEC6 = 40284,
            EQ_DEC7 = 40285,
            EQ_DEC8 = 40286,
            EQ_DEC9 = 40287,
            EQ_DEC10 = 40288,
            ID_PE_SCUP = 40289,
            ID_PE_SCDOWN = 40290,
            WINAMP_REFRESHSKIN = 40291,
            ID_PE_PRINT = 40292,
            ID_PE_EXTINFO = 40293,
            WINAMP_PLAYLIST_ADVANCE = 40294,
            WINAMP_VIS_LIN = 40295,
            WINAMP_VIS_BAR = 40296,
            WINAMP_OPTIONS_MINIBROWSER = 40298,
            MB_FWD = 40299,
            MB_BACK = 40300,
            MB_RELOAD = 40301,
            MB_OPENMENU = 40302,
            MB_OPENLOC = 40303,
            WINAMP_NEW_INSTANCE = 40305,
            MB_UPDATE = 40309,
            WINAMP_OPTIONS_WINDOWSHADE_EQ = 40310,
            EQ_PANLEFT = 40313,
            EQ_PANRIGHT = 40314,
            WINAMP_GETMORESKINS = 40316,
            WINAMP_VIS_OPTIONS = 40317,
            WINAMP_PE_SEARCH = 40318,
            ID_PE_BOOKMARK = 40319,
            WINAMP_EDIT_BOOKMARKS = 40320,
            WINAMP_MAKECURBOOKMARK = 40321,
            ID_MAIN_PLAY_BOOKMARK_NONE = 40322,
            ID_MAIN_PLAY_AUDIOCD = 40323,
            ID_MAIN_PLAY_AUDIOCD2 = 40324,
            ID_MAIN_PLAY_AUDIOCD3 = 40325,
            ID_MAIN_PLAY_AUDIOCD4 = 40326,
            WINAMP_OPTIONS_VIDEO = 40328,
            ID_VIDEOWND_ZOOMFULLSCREEN = 40329,
            ID_VIDEOWND_ZOOM100 = 40330,
            ID_VIDEOWND_ZOOM200 = 40331,
            ID_VIDEOWND_ZOOM50 = 40332,
            ID_VIDEOWND_VIDEOOPTIONS = 40333,
            WINAMP_MINIMIZE = 40334,
            ID_PE_FONTBIGGER = 40335,
            ID_PE_FONTSMALLER = 40336,
            WINAMP_VIDEO_TOGGLE_FS = 40337,
            WINAMP_VIDEO_TVBUTTON = 40338,
            WINAMP_LIGHTNING_CLICK = 40339,
            ID_FILE_ADDTOLIBRARY = 40344,
            ID_HELP_HELPTOPICS = 40347,
            ID_HELP_GETTINGSTARTED = 40348,
            ID_HELP_WINAMPFORUMS = 40349,
            ID_PLAY_VOLUMEUP = 40351,
            ID_PLAY_VOLUMEDOWN = 40352,
            ID_PEFILE_OPENPLAYLISTFROMLIBRARY_NOPLAYLISTSINLIBRARY = 40355,
            ID_PEFILE_ADDFROMLIBRARY = 40356,
            ID_PEFILE_CLOSEPLAYLISTEDITOR = 40357,
            ID_PEPLAYLIST_PLAYLISTPREFERENCES = 40358,
            ID_MLFILE_NEWPLAYLIST = 40359,
            ID_MLFILE_LOADPLAYLIST = 40360,
            ID_MLFILE_SAVEPLAYLIST = 40361,
            ID_MLFILE_ADDMEDIATOLIBRARY = 40362,
            ID_MLFILE_CLOSEMEDIALIBRARY = 40363,
            ID_MLVIEW_NOWPLAYING = 40364,
            ID_MLVIEW_LOCALMEDIA_ALLMEDIA = 40366,
            ID_MLVIEW_LOCALMEDIA_AUDIO = 40367,
            ID_MLVIEW_LOCALMEDIA_VIDEO = 40368,
            ID_MLVIEW_PLAYLISTS_NOPLAYLISTINLIBRARY = 40369,
            ID_MLVIEW_INTERNETRADIO = 40370,
            ID_MLVIEW_INTERNETTV = 40371,
            ID_MLVIEW_LIBRARYPREFERENCES = 40372,
            ID_MLVIEW_DEVICES_NOAVAILABLEDEVICE = 40373,
            ID_MLFILE_IMPORTCURRENTPLAYLIST = 40374,
            ID_MLVIEW_MEDIA = 40376,
            ID_MLVIEW_PLAYLISTS = 40377,
            ID_MLVIEW_MEDIA_ALLMEDIA = 40377,
            ID_MLVIEW_DEVICES = 40378,
            ID_FILE_SHOWLIBRARY = 40379,
            ID_FILE_CLOSELIBRARY = 40380,
            ID_POST_PLAY_PLAYLIST = 40381,
            ID_VIS_NEXT = 40382,
            ID_VIS_PREV = 40383,
            ID_VIS_RANDOM = 40384,
            ID_MANAGEPLAYLISTS = 40385,
            ID_PREFS_SKIN_SWITCHTOSKIN = 40386,
            ID_PREFS_SKIN_DELETESKIN = 40387,
            ID_PREFS_SKIN_RENAMESKIN = 40388,
            ID_VIS_FS = 40389,
            ID_VIS_CFG = 40390,
            ID_VIS_MENU = 40391,
            ID_VIS_SET_FS_FLAG = 40392,
            ID_PE_SHOWPLAYING = 40393,
            ID_HELP_REGISTERWINAMPPRO = 40394,
            ID_PE_MANUAL_ADVANCE = 40395,
            WA_SONG_5_STAR_RATING = 40396,
            WA_SONG_4_STAR_RATING = 40397,
            WA_SONG_3_STAR_RATING = 40398,
            WA_SONG_2_STAR_RATING = 40399,
            WA_SONG_1_STAR_RATING = 40400,
            WA_SONG_NO_RATING = 40401,
            PL_SONG_5_STAR_RATING = 40402,
            PL_SONG_4_STAR_RATING = 40403,
            PL_SONG_3_STAR_RATING = 40404,
            PL_SONG_2_STAR_RATING = 40405,
            PL_SONG_1_STAR_RATING = 40406,
            PL_SONG_NO_RATING = 40407,
            AUDIO_TRACK_ONE = 40408,
            VIDEO_TRACK_ONE = 40424,
            ID_SWITCH_COLOURTHEME = 44500,
            ID_GENFF_LIMIT = 45000,
        }
#endregion
    }
}
