/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005 Olivier Marcoux (wiz0u@free.fr / http://wiz0u.free.fr/freexplorer)
 * 
 * Ce programme est libre, vous pouvez le redistribuer et/ou le modifier selon les 
 * termes de la Licence Publique Générale GNU publiée par la Free Software 
 * Foundation (version 2 ou bien toute autre version ultérieure choisie par vous).
 * 
 * Ce programme est distribué car potentiellement utile, mais SANS AUCUNE GARANTIE, 
 * ni explicite ni implicite, y compris les garanties de commercialisation ou d'adaptation 
 * dans un but spécifique. Reportez-vous à la Licence Publique Générale GNU pour 
 * plus de détails.
 * 
 * Vous devez avoir reçu une copie de la Licence Publique Générale GNU en même 
 * temps que ce programme ; si ce n'est pas le cas, écrivez à la Free Software 
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, États-Unis. 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

class SysWin32
{
    [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);
    
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetForegroundWindow();

    public const int WM_APPCOMMAND =              0x319;
    public const int APPCOMMAND_MEDIA_NEXTTRACK =        11;
    public const int APPCOMMAND_MEDIA_PREVIOUSTRACK =     12;
    public const int APPCOMMAND_MEDIA_STOP =           13;
    public const int APPCOMMAND_MEDIA_PLAY_PAUSE  =       14;
    public const int FAPPCOMMAND_MASK = 0x8000;

     
    [DllImport("user32.dll",
        EntryPoint = "ShowWindow",
        CallingConvention = CallingConvention.StdCall,
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_SHOW = 5;

    [DllImport("kernel32")]
    public static extern int GetVolumeInformation(
        string lpRootPathName,
        StringBuilder lpVolumeNameBuffer,
        int nVolumeNameSize,
        out int lpVolumeSerialNumber,
        out int lpMaximumComponentLength,
        out int lpFileSystemFlags,
        StringBuilder lpFileSystemNameBuffer,
        int nFileSystemNameSize);

    public static int GetVolumeInformation(
        string rootPathName,
        out string volumeName,
        out int volumeSerialNumber,
        out int maximumComponentLength,
        out int fileSystemFlags,
        out string fileSystemName)
    {
        StringBuilder lpVolumeNameBuffer = new StringBuilder(256);
        StringBuilder lpFileSystemNameBuffer = new StringBuilder(256);
        int iResult = GetVolumeInformation(rootPathName, lpVolumeNameBuffer, 256,
            out volumeSerialNumber, out maximumComponentLength, out fileSystemFlags,
            lpFileSystemNameBuffer, 256);
        volumeName = lpVolumeNameBuffer.ToString();
        fileSystemName = lpFileSystemNameBuffer.ToString();
        return iResult;
    }
}
