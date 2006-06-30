/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2005 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr) 
 * Copyright (C) 2006 Roncaglia Julien <freexplorer@virtualblackfox.net>
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Wizou.VLC;
using System.Reflection;

namespace Wizou.FreeXplorer
{
    class VLCLaunchException : ApplicationException
    {
        public VLCLaunchException(string msg) : base(msg) { }
    }


    class FreeXplorer : IDisposable
    {
        public static string ConfigurationFolder
        {
            get
            {
                switch (OSUtils.OSType)
                {
                    case OSType.Windows:
                        return Path.Combine(Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData), "FreeXplorer");
                    
                    case OSType.Unix :
                    default :
                        return Path.Combine(Environment.GetFolderPath(
                            Environment.SpecialFolder.Personal), ".freexplorer");
                }
            }
        }

        public bool VLCVisible
        {
            get { return m_vlcApp.ShowWindow; }
            set { m_vlcApp.ShowWindow = value; }
        }

        Configuration m_config = new Configuration();
        public Configuration Configuration { get { return m_config; } }

        VLCApp m_vlcApp = new VLCApp();
        FreeboxServer m_freeboxServer = new FreeboxServer(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "pages"));
        public FreeboxServer FreeboxServer { get { return m_freeboxServer; } }
        LIRC.LIRCServer m_lircServer = new LIRC.LIRCServer();

        IPHostEntry FreeBoxIP
        {
            get
            {
                
                try
                {
                    return Dns.GetHostEntry("freeplayer.freebox.fr");
                }
                catch (SocketException)
                {
                    return new IPHostEntry();
                }
            }
        }

        public FreeXplorer()
        {
            IPHostEntry freeboxIP = FreeBoxIP;

            if (freeboxIP.AddressList != null)
                m_freeboxServer.Init(freeboxIP.AddressList[0], m_vlcApp, m_lircServer);

            m_config.Load();
            ApplyConfig(true);

            if (freeboxIP.AddressList == null)
            {
                throw new Exception("Impossible de résoudre l'adresse IP de la Freebox\r\n" +
                                "Verifiez la configuration");
            }            
        }

        public void Dispose()
        {
            m_config.Save();
            m_freeboxServer.Stop();
            m_lircServer.Stop();
            m_vlcApp.Stop();
        }

        public void ApplyConfig(bool restartVLC)
        {
            // arrets des serveurs TCP
            m_freeboxServer.Stop();
            m_lircServer.Stop();

            if (restartVLC) m_vlcApp.Stop();

            // application des paramètres
            VLC.Utility.SoundExts = m_config.SoundExts;
            VLC.Utility.PictureExts = m_config.PictureExts;
            VLC.Utility.VideoExts = m_config.VideoExts;

            if (restartVLC)
            {
                try
                {
                    m_vlcApp.RC_host_port = Convert.ToInt32(m_config.VlcPort);
                    m_vlcApp.DVDLetter = m_config.DVDLetter[0];
                    m_vlcApp.AudioTranscode = m_config.Transcode;
                    m_vlcApp.SetConfig(m_config.VLCPath, m_config.AudioLanguage, m_config.SubLanguage, m_config.FFMpegInterlace, m_config.HalfScale ? 0.1 : 1.0, Convert.ToInt32(m_config.TranscodeVB));
                    m_vlcApp.Start();
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    throw new VLCLaunchException("Impossible de lancer l'executable vlc.exe\r\n\r\n" +
                        ex.Message + "\r\n\r\nVerifiez la configuration");
                }
                catch (VLCException ex)
                {
                    throw new VLCLaunchException("Problème au lancement de vlc.exe :\r\n\r\n" +
                        ex.Message + "\r\n\r\nRéessayez de lancer FreeXplorer");
                }
            }

            // activation eventuelle des serveurs TCP
            m_lircServer.Active = m_config.LIRCActive;
            m_freeboxServer.PCControlAllowed = m_config.PCControlAllowed;
            m_freeboxServer.BlackBkgnds = m_config.BlackBkgnds;

            try
            {
                m_freeboxServer.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Le port 8080 de cette machine est déjà occupé !\r\n" +
                                "Vérifiez que FreeXplorer, VLC, un autre Freeplayer ou un serveur proxy n'est pas déjà actif");
            }
        }
    }
}
