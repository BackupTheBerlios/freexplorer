!define VERSION_MAJOR   1
!define VERSION_MINOR   3
!define VERSION         "1.3.2"
!define MAINEXENAME     "FreeXplorer.exe"
!define TAGNAME         "FreeXplorer-1-3-2"

!define MFG             "Wizou"
!define FULLMFG         "Olivier Marcoux"
!define URLUPDATEINFO   "http://wiz0u.free.fr/freexplorer"

!define PRODUCT         "FreeXplorer"
!define COPYRIGHT       "Copyright © 2005 Olivier Marcoux"

!define APP_GUID		"FreeXplorer-{91279ebe-f9c1-11d9-949c-00e08161165f}"
!define UNINSTALLKEY    "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_GUID}"

;--------------------------------
;General

    ;Name and file
    Name "${PRODUCT} ${VERSION}" "${PRODUCT}"
    OutFile "FreeXplorer-setup.exe"
    SetCompressor lzma
    
    ;Default installation folder
    InstallDir "$PROGRAMFILES\${MFG}\${PRODUCT}"
    
    ;Get installation folder from registry if available
    InstallDirRegKey HKLM "${UNINSTALLKEY}" "InstallLocation"
    
    BrandingText " "
    XPStyle on

;--------------------------------
;Version Information

    VIProductVersion "${VERSION}.0.0"
    VIAddVersionKey /LANG=1036 "ProductName"     "${PRODUCT}"
    VIAddVersionKey /LANG=1036 "CompanyName"     "${FULLMFG}"
    VIAddVersionKey /LANG=1036 "LegalCopyright"  "${COPYRIGHT}"
    VIAddVersionKey /LANG=1036 "FileDescription" "${PRODUCT}"
    VIAddVersionKey /LANG=1036 "FileVersion"     "${VERSION}"

