; FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
; Copyright (C) 2005-2007 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
; 
; Ce programme est libre, vous pouvez le redistribuer et/ou le modifier selon les 
; termes de la Licence Publique Générale GNU publiée par la Free Software 
; Foundation (version 2 ou bien toute autre version ultérieure choisie par vous).
; 
; Ce programme est distribué car potentiellement utile, mais SANS AUCUNE GARANTIE, 
; ni explicite ni implicite, y compris les garanties de commercialisation ou d'adaptation 
; dans un but spécifique. Reportez-vous à la Licence Publique Générale GNU pour 
; plus de détails.
; 
; Vous devez avoir reçu une copie de la Licence Publique Générale GNU en même 
; temps que ce programme ; si ce n'est pas le cas, écrivez à la Free Software 
; Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, États-Unis. 


!include Setup.nsh
!addplugindir "."

!define RELEASE_DIR		"..\FreeXplorer\bin"
!define VLCPACK "vlc-0.8.6"
OutFile "archive\${TAGNAME}-win32-setup.exe"



;--------------------------------
;Modern UI

	!include "MUI.nsh"

	ShowInstDetails hide
	ShowUninstDetails hide
	SpaceTexts none

;--------------------------------
;Interface Settings

	!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\orange-install.ico"
	!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\orange-uninstall.ico"

	!define MUI_HEADERIMAGE
	!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\orange.bmp"
	!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange.bmp"
	!define MUI_BGCOLOR 0xFEF8ED

	!define MUI_HEADERIMAGE_UNBITMAP "${NSISDIR}\Contrib\Graphics\Header\orange-uninstall.bmp"
	!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange-uninstall.bmp"

	!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAINEXENAME}"
	!define MUI_FINISHPAGE_NOREBOOTSUPPORT

;--------------------------------
;Language Selection Dialog Settings

;--------------------------------
;Pages

	!define MUI_PAGE_CUSTOMFUNCTION_LEAVE CheckInstalled
	!define MUI_WELCOMEPAGE_TITLE_3LINES
	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_LICENSE $(license)
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
	!insertmacro MUI_PAGE_FINISH
	
	!define MUI_WELCOMEPAGE_TITLE_3LINES
	!insertmacro MUI_UNPAGE_WELCOME
	!insertmacro MUI_UNPAGE_INSTFILES
	!insertmacro MUI_UNPAGE_FINISH
	
;--------------------------------
;Reserve Files
  
  ;These files should be inserted before other files in the data block
  ;Keep these lines before any File command
  ;Only for solid compression (by default, solid compression is enabled for BZIP2 and LZMA)
  

;--------------------------------
;Languages

	!insertmacro MUI_LANGUAGE "French"

	LangString AskUninstall ${LANG_FRENCH}			"Une précédente installation de ${PRODUCT} (version $R2)$\na été détectée et va être desinstallée.$\n$\nVoulez-vous conserver vos réglages ?"
	LangString StillActive	${LANG_FRENCH}			"Impossible de continuer car ${PRODUCT} est en cours d'exécution.$\nVeuillez arreter le programme d'abord"

	LicenseLangString license ${LANG_FRENCH}		"..\license-FR.txt"


;--------------------------------
;Installer Functions

Var KEEPCONFIG

#####################################################
# Fonctions dont le sript à besoin pour fonctionner #
#####################################################
; IsDotNET2Installed
;
; Usage:
;   Call IsDotNET2Installed
;   Pop $0
;   StrCmp $0 1 found.NETFramework no.NETFramework
Function IsDotNET2Installed
  Push $0
  Push $1
  Push $2
  Push $3
  ReadRegStr $3 HKEY_LOCAL_MACHINE \
    "Software\Microsoft\.NETFramework" "InstallRoot"
  # remove trailing back slash
  Push $3
  Exch $EXEDIR
  Exch $EXEDIR
  Pop $3
  # if the root directory doesn't exist .NET is not installed
  IfFileExists $3 0 noDotNET
   StrCpy $0 0
   EnumPolicy:
     EnumRegValue $2 HKEY_LOCAL_MACHINE \
      "Software\Microsoft\.NETFramework\Policy\v2.0" $0
    IntOp $0 $0 + 1
     StrCmp $2 "" noDotNET
      IfFileExists "$3\v2.0.$2" foundDotNET EnumPolicy
   noDotNET:
    StrCpy $0 0
    Goto done
   foundDotNET:
    StrCpy $0 1
   done:
    Pop $3
    Pop $2
    Pop $1
    Exch $0
FunctionEnd


Function CheckInstalled
	Call IsDotNET2Installed
	Pop $0
	StrCmp $0 1 found.NETFramework
	MessageBox MB_OK|MB_ICONSTOP "Il vous faut d'abord installer le .NET Framework 2.0$\navant de pouvoir utiliser FreeXplorer !$\n$\nCliquez sur OK pour aller sur la page de téléchargement" /SD IDOK
	ExecShell "open" "http://www.microsoft.com/downloads/details.aspx?displaylang=fr&FamilyID=7ABD8C8F-287E-4C7E-9A4A-A4ECFF40FC8E"
	Abort
found.NETFramework:

	ReadRegStr $R0 HKLM "${UNINSTALLKEY}" "UninstallString"
	IfFileExists "$R0" 0 noUninstall
	ReadRegStr $R1 HKLM "${UNINSTALLKEY}" "InstallLocation"
	ReadRegStr $R2 HKLM "${UNINSTALLKEY}" "DisplayVersion"
	InitPluginsDir
	CreateDirectory "$APPDATA\FreeXplorer"
	StrCpy $KEEPCONFIG "/KEEPCONFIG"
	MessageBox MB_YESNOCANCEL|MB_ICONQUESTION $(AskUninstall) /SD IDYES IDYES uninstall IDCANCEL noUninstall
	; IDNO: on supprime les réglages précédents
	StrCpy $KEEPCONFIG ""
	Delete "$APPDATA\FreeXplorer\*"
uninstall:
	CopyFiles $R0 $PLUGINSDIR\Uninstall.exe
	ExecWait '"$PLUGINSDIR\Uninstall.exe" $KEEPCONFIG /S _?=$R1'
	IfErrors uninstallError
	MessageBox MB_OK|MB_ICONINFORMATION $(MUI_UNTEXT_FINISH_SUBTITLE) /SD IDOK
	Return
uninstallError:
	MessageBox MB_RETRYCANCEL|MB_ICONSTOP $(MUI_UNTEXT_ABORT_SUBTITLE) /SD IDOK IDRETRY uninstall
	Abort
noUninstall: 
FunctionEnd


;--------------------------------
;Installer Sections

Section "Main program" SecCopyUI
	IfSilent 0 +2
	call CheckInstalled

	SetShellVarContext current
	SetOutPath "$INSTDIR"
	ClearErrors
	File "${RELEASE_DIR}\${MAINEXENAME}"
	IfErrors 0 installOk
	MessageBox MB_OK|MB_ICONSTOP $(StillActive)
	SetAutoClose true
	Quit
installOk:

	SetCompress off
	File "/oname=$PLUGINSDIR\vlc.exe" "vlc.exe"
	SetCompress auto
	ExecWait '"$PLUGINSDIR\vlc.exe" x "-o$INSTDIR" -y'

	CreateDirectory "$APPDATA\FreeXplorer"
	IfFileExists $APPDATA\FreeXplorer\favorites.xml noNewFavorites
	FileOpen $0 $APPDATA\FreeXplorer\favorites.xml w
	FileWrite $0 "<Favorites></Favorites>"
	FileClose $0
noNewFavorites:
	IfFileExists $APPDATA\FreeXplorer\recents.xml noNewRecents
	FileOpen $0 $APPDATA\FreeXplorer\recents.xml w
	FileWrite $0 "<Recents></Recents>"
	FileClose $0
noNewRecents:
	nsisXML::create
	IfFileExists $APPDATA\FreeXplorer\config.xml noNewConfig
	nsisXML::createElement "Config"
	nsisXML::appendChild
	StrCpy $1 $2
	nsisXML::createElement "VLCPath"
	nsisXML::appendChild
	nsisXML::createElement "StartMinimized"
	nsisXML::appendChild
	nsisXML::createElement "MinimizeToTray"
	nsisXML::appendChild
	nsisXML::save "$APPDATA\FreeXplorer\config.xml"
noNewConfig:
	nsisXML::load $APPDATA\FreeXplorer\config.xml
	nsisXML::select '/Config/VLCPath'
	nsisXML::setText "$INSTDIR\${VLCPACK}\vlc.exe"
	nsisXML::select '/Config/StartMinimized'
	nsisXML::setText "True"
	nsisXML::select '/Config/MinimizeToTray'
	nsisXML::setText "True"
	nsisXML::save "$APPDATA\FreeXplorer\config.xml"

	File "${RELEASE_DIR}\ImageManipulation.dll"
	File "${RELEASE_DIR}\vlcrc"
	File "Lisez-Moi.html"
	File "..\license.txt"
	File "..\license-FR.txt"
	File /r /x CVS /x test "${RELEASE_DIR}\pages"

	
	;Create shortcuts
	CreateShortCut  "$SMPROGRAMS\${PRODUCT} ${VERSION}.lnk" "$INSTDIR\${MAINEXENAME}"
	CreateShortCut  "$INSTDIR\FreeXplorer.lnk" "$INSTDIR\${MAINEXENAME}" "-autostart"
	
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"

	;Setup Add/Remove Program information
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "DisplayName" "${PRODUCT} ${VERSION}"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "InstallLocation" "$INSTDIR"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "DisplayIcon" "$INSTDIR\${MAINEXENAME}"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "Publisher" "${FULLMFG}"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "InstallSource" "$EXEDIR"
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "Readme" "$INSTDIR\Lisez-Moi.html"
!ifdef URLINFOABOUT
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "URLInfoAbout" "${URLINFOABOUT}"
!endif
!ifdef URLUPDATEINFO
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "URLUpdateInfo" "${URLUPDATEINFO}"
!endif
	WriteRegStr	  HKLM "${UNINSTALLKEY}" "DisplayVersion" "${VERSION}"
	WriteRegDWORD HKLM "${UNINSTALLKEY}" "VersionMajor" ${VERSION_MAJOR}
	WriteRegDWORD HKLM "${UNINSTALLKEY}" "VersionMinor" ${VERSION_MINOR}
	WriteRegDWORD HKLM "${UNINSTALLKEY}" "NoModify" 1
	WriteRegDWORD HKLM "${UNINSTALLKEY}" "NoRepair" 1

SectionEnd

;--------------------------------
;Uninstaller Functions

Function un.GetParent
	Exch $R0 ; old $R0 is on top of stack
	Push $R1
	Push $R2
	StrCpy $R1 -1
	loop:
		StrCpy $R2 $R0 1 $R1
		StrCmp $R2 "" exit
		StrCmp $R2 "\" exit
		IntOp $R1 $R1 - 1
	Goto loop
	exit:
		StrCpy $R0 $R0 $R1
		Pop $R2
		Pop $R1
		Exch $R0 ; put $R0 on top of stack, restore $R0 to original value
FunctionEnd

Function un.StrStr
   Exch $R1
   Exch 
   Exch $R2
   Push $R3
   Push $R4
   Push $R5
   StrLen $R3 $R1
   StrCpy $R4 0
   loop:
     StrCpy $R5 $R2 $R3 $R4
     StrCmp $R5 $R1 done
     StrCmp $R5 "" done
     IntOp $R4 $R4 + 1
     Goto loop
 done:
   StrCpy $R1 $R2 "" $R4
   Pop $R5
   Pop $R4
   Pop $R3
   Pop $R2
   Exch $R1
FunctionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

	SetShellVarContext current

	;Remove program files
	ClearErrors
	Delete "$INSTDIR\${MAINEXENAME}"
	IfErrors 0 removeOK
	MessageBox MB_OK|MB_ICONSTOP $(StillActive)
	SetAutoClose true
	Quit
removeOk:
	Push $CMDLINE
	Push '/KEEPCONFIG'
	Call un.StrStr
    Pop $0
	StrCmp $0 "" 0 keepConfig
	; uninstaller was not called with /KEEPCONFIG option:
	; delete user config files
	RMDir /r "$APPDATA\FreeXplorer"
keepConfig:
	RMDir /r "$INSTDIR\pages"
	Delete "$INSTDIR\ImageManipulation.dll"
	RMDir /r "$INSTDIR\${VLCPACK}"
	Delete "$INSTDIR\FreeXplorer.lnk"
	Delete "$INSTDIR\vlcrc"
	Delete "$INSTDIR\Lisez-Moi.html"
	Delete "$INSTDIR\license.txt"
	Delete "$INSTDIR\license-FR.txt"
	Delete "$INSTDIR\Uninstall.exe"
	;Remove program directory (only if directory is empty)
	RMDir "$INSTDIR"
	;Remove manufacturer directory (only if directory is empty)
	Push "$INSTDIR"
	Call un.GetParent
	Pop $R0
	RMDir "$R0"

	;Remove shortcuts
	Delete "$SMPROGRAMS\${PRODUCT} ${VERSION}.lnk"

	;Remove registration
	DeleteRegKey HKLM "${UNINSTALLKEY}"

	DeleteRegKey /ifempty HKLM "Software\${MFG}\${PRODUCT}"
	DeleteRegKey /ifempty HKLM "Software\${MFG}"
SectionEnd
