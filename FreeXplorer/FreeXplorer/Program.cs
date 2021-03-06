/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005-2006 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
 * 
 * Ce programme est libre, vous pouvez le redistribuer et/ou le modifier selon les 
 * termes de la Licence Publique G�n�rale GNU publi�e par la Free Software 
 * Foundation (version 2 ou bien toute autre version ult�rieure choisie par vous).
 * 
 * Ce programme est distribu� car potentiellement utile, mais SANS AUCUNE GARANTIE, 
 * ni explicite ni implicite, y compris les garanties de commercialisation ou d'adaptation 
 * dans un but sp�cifique. Reportez-vous � la Licence Publique G�n�rale GNU pour 
 * plus de d�tails.
 * 
 * Vous devez avoir re�u une copie de la Licence Publique G�n�rale GNU en m�me 
 * temps que ce programme ; si ce n'est pas le cas, �crivez � la Free Software 
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, �tats-Unis. 
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Wizou.FreeXplorer
{
    static class Program
    {
        public static string appVersionText;
        public static bool autostart = false;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            appVersionText = appVersion.ToString(appVersion.Build == 0 ? 2 : 3);
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (String.Compare(arg, "-autostart", true) == 0)
                    autostart = true;
            }
            
            MainForm mainForm;
            try
            {
                mainForm = new MainForm();
            }
            catch (ApplicationException e)
            {
                MessageBox.Show("Une erreur inattendue s'est produite lors du d�marrage de FreeXplorer:\r\n\r\n" +
                    e.Message + "\r\n\r\nLe programme ne peut pas se lancer correctement et va s'arreter", "Initialisation",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#if !DEBUG
            catch (Exception e) // en mode RELEASE, catch toutes les exceptions pour afficher un message d'erreur
            {
                MessageBox.Show("Une erreur inattendue s'est produite lors du d�marrage de FreeXplorer:\r\n\r\n"+
                    e+"\r\n\r\nLe programme ne peut pas se lancer correctement et va s'arreter\r\n(tapez Ctrl-C pour copier ces informations)", "Initialisation",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif
            Application.Run(mainForm);
        }
    }
}