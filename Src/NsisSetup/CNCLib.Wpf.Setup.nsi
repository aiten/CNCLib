;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

  ;--------------------------------
;global

  !define CopyFromFolder "..\CNCLib\CNCLib.Wpf.Start\bin\Debug"

;--------------------------------
;General

  ;Properly display all languages
  Unicode true

  ;Define name of the product
  !define PRODUCT "CNCLib"
  !define COMPANYNAME "Aitenbichler Herbert"
  !define DESCRIPTION "CNCLib - a program to control your Arduino based CNC machine"
  # These three must be integers
  !define VERSIONMAJOR 1
  !define VERSIONMINOR 0
  !define VERSIONBUILD 10 
  
  !define HELPURL "https://github.com/aiten/CNCLib" # "Support Information" link
  !define UPDATEURL "https://github.com/aiten/CNCLib" # "Product Updates" link
  !define ABOUTURL "https://github.com/aiten/CNCLib" # "Publisher" link

  ;Define optional URL that will be opened after the installation was successful
  !define AFTER_INSTALLATION_URL "https://github.com/aiten/CNCLib"


  ;Define the main name of the installer
  Name "${PRODUCT}"

  ;Define the directory where the installer should be saved
  OutFile ".\Output\${PRODUCT}_install.exe"


  ;Define the default installation folder (Windows ProgramFiles example)
  InstallDir "$PROGRAMFILES\${PRODUCT}"

  ;Define optional a directory for program files that change (Windows AppData example)
  !define INSTDIR_DATA "$APPDATA\${PRODUCT}"


  ;Request rights if you want to install the program to program files
  RequestExecutionLevel admin

  ;Properly display all languages
  Unicode true

  ;Show 'console' in installer and uninstaller
  ShowInstDetails "show"
  ShowUninstDetails "show"

  ;Get installation folder from registry if available
  InstallDirRegKey HKLM "Software\${PRODUCT}" ""


;--------------------------------
;Interface Settings

  ;Show warning if user wants to abort
  !define MUI_ABORTWARNING

  ;Show all languages, despite user's codepage
  ;!define MUI_LANGDLL_ALLLANGUAGES

  ;Use optional a custom icon:
  !define MUI_ICON ".\SetupFiles\CNCLib_installer.ico" # for the Installer
  !define MUI_UNICON ".\SetupFiles\CNCLib_uninstaller.ico" # for the later created UnInstaller

  ;Use optional a custom picture for the 'Welcome' and 'Finish' page:
  !define MUI_HEADERIMAGE_RIGHT
  !define MUI_WELCOMEFINISHPAGE_BITMAP ".\SetupFiles\CNCLib_installer.bmp"  # for the Installer
  !define MUI_UNWELCOMEFINISHPAGE_BITMAP ".\SetupFiles\CNCLib_uninstaller.bmp"  # for the later created UnInstaller

  ;Optional no descripton for all components
  !define MUI_COMPONENTSPAGE_NODESC


;--------------------------------
;Pages

  ;For the installer
  !insertmacro MUI_PAGE_WELCOME # simply remove this and other pages if you don't want it
  !insertmacro MUI_PAGE_LICENSE ".\SetupFiles\LICENSE" # link to an ANSI encoded license file
  ;!insertmacro MUI_PAGE_COMPONENTS # remove if you don't want to list components
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

  ;For the uninstaller
  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH


;--------------------------------
;Languages

  ;At start will be searched if the current system language is in this list,
  ;if not the first language in this list will be chosen as language
  !insertmacro MUI_LANGUAGE "English"
  ;!insertmacro MUI_LANGUAGE "French"
  ;!insertmacro MUI_LANGUAGE "German"
  ;!insertmacro MUI_LANGUAGE "Spanish"
  ;!insertmacro MUI_LANGUAGE "SpanishInternational"
  ;!insertmacro MUI_LANGUAGE "SimpChinese"
  ;!insertmacro MUI_LANGUAGE "TradChinese"
  ;!insertmacro MUI_LANGUAGE "Japanese"
  ;!insertmacro MUI_LANGUAGE "Korean"
  ;!insertmacro MUI_LANGUAGE "Italian"
  ;!insertmacro MUI_LANGUAGE "Dutch"
  ;!insertmacro MUI_LANGUAGE "Danish"
  ;!insertmacro MUI_LANGUAGE "Swedish"
  ;!insertmacro MUI_LANGUAGE "Norwegian"
  ;!insertmacro MUI_LANGUAGE "NorwegianNynorsk"
  ;!insertmacro MUI_LANGUAGE "Finnish"
  ;!insertmacro MUI_LANGUAGE "Greek"
  ;!insertmacro MUI_LANGUAGE "Russian"
  ;!insertmacro MUI_LANGUAGE "Portuguese"
  ;!insertmacro MUI_LANGUAGE "PortugueseBR"
  ;!insertmacro MUI_LANGUAGE "Polish"
  ;!insertmacro MUI_LANGUAGE "Ukrainian"
  ;!insertmacro MUI_LANGUAGE "Czech"
  ;!insertmacro MUI_LANGUAGE "Slovak"
  ;!insertmacro MUI_LANGUAGE "Croatian"
  ;!insertmacro MUI_LANGUAGE "Bulgarian"
  ;!insertmacro MUI_LANGUAGE "Hungarian"
  ;!insertmacro MUI_LANGUAGE "Thai"
  ;!insertmacro MUI_LANGUAGE "Romanian"
  ;!insertmacro MUI_LANGUAGE "Latvian"
  ;!insertmacro MUI_LANGUAGE "Macedonian"
  ;!insertmacro MUI_LANGUAGE "Estonian"
  ;!insertmacro MUI_LANGUAGE "Turkish"
  ;!insertmacro MUI_LANGUAGE "Lithuanian"
  ;!insertmacro MUI_LANGUAGE "Slovenian"
  ;!insertmacro MUI_LANGUAGE "Serbian"
  ;!insertmacro MUI_LANGUAGE "SerbianLatin"
  ;!insertmacro MUI_LANGUAGE "Arabic"
  ;!insertmacro MUI_LANGUAGE "Farsi"
  ;!insertmacro MUI_LANGUAGE "Hebrew"
  ;!insertmacro MUI_LANGUAGE "Indonesian"
  ;!insertmacro MUI_LANGUAGE "Mongolian"
  ;!insertmacro MUI_LANGUAGE "Luxembourgish"
  ;!insertmacro MUI_LANGUAGE "Albanian"
  ;!insertmacro MUI_LANGUAGE "Breton"
  ;!insertmacro MUI_LANGUAGE "Belarusian"
  ;!insertmacro MUI_LANGUAGE "Icelandic"
  ;!insertmacro MUI_LANGUAGE "Malay"
  ;!insertmacro MUI_LANGUAGE "Bosnian"
  ;!insertmacro MUI_LANGUAGE "Kurdish"
  ;!insertmacro MUI_LANGUAGE "Irish"
  ;!insertmacro MUI_LANGUAGE "Uzbek"
  ;!insertmacro MUI_LANGUAGE "Galician"
  ;!insertmacro MUI_LANGUAGE "Afrikaans"
  ;!insertmacro MUI_LANGUAGE "Catalan"
  ;!insertmacro MUI_LANGUAGE "Esperanto"
  ;!insertmacro MUI_LANGUAGE "Asturian"
  ;!insertmacro MUI_LANGUAGE "Basque"
  ;!insertmacro MUI_LANGUAGE "Pashto"
  ;!insertmacro MUI_LANGUAGE "Georgian"
  ;!insertmacro MUI_LANGUAGE "Vietnamese"
  ;!insertmacro MUI_LANGUAGE "Welsh"
  ;!insertmacro MUI_LANGUAGE "Armenian"
  ;!insertmacro MUI_LANGUAGE "Corsican"


;--------------------------------
;Installer Section

Section "Main Component"
  SectionIn RO # Just means if in component mode this is locked

  ;Set output path to the installation directory.
  SetOutPath $INSTDIR

  ;Put the following file in the SetOutPath

  File /r /x "*.pdb" "${CopyFromFolder}\*.*"
  
  ;Store installation folder in registry
  WriteRegStr HKLM "Software\${PRODUCT}" "" $INSTDIR

  ;Registry information for add/remove programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayName" "${PRODUCT}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "UninstallString" '"$INSTDIR\${PRODUCT}_uninstaller.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayIcon" "$\"$INSTDIR\CNCLib_uninstaller.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "Publisher" "${COMPANYNAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "HelpLink" "${HELPURL}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "URLUpdateInfo" "${UPDATEURL}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "URLInfoAbout" "${ABOUTURL}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "VersionMajor" ${VERSIONMAJOR}
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "VersionMinor" ${VERSIONMINOR}

  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoRepair" 1

  ;Create optional start menu shortcut for uninstaller and Main component
  CreateDirectory "$SMPROGRAMS\${PRODUCT}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT}\CNCLib.Wpf.lnk" "$INSTDIR\CNCLib.Wpf.Start.exe" "" "$INSTDIR\CNCLib.Wpf.Start.exe" 0
  ;CreateShortCut "$SMPROGRAMS\${PRODUCT}\Uninstall ${PRODUCT}.lnk" "$INSTDIR\${PRODUCT}_uninstaller.exe" "" "$INSTDIR\${PRODUCT}_uninstaller.exe" 0

  CreateShortCut "$DESKTOP\CNCLib.Wpf.lnk" "$INSTDIR\CNCLib.Wpf.Start.exe" "" "$INSTDIR\CNCLib.Wpf.Start.exe" 0

  ;Create uninstaller
  WriteUninstaller "${PRODUCT}_uninstaller.exe"

SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;Remove all registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}"
  DeleteRegKey HKLM "Software\${PRODUCT}"

  ;Delete the installation directory + all files in it
  ;Add 'RMDir /r "$INSTDIR\folder\*.*"' for every folder you have added additionaly
  RMDir /r "$INSTDIR\*.*"
  RMDir "$INSTDIR"

  ;Delete the appdata directory + files
  RMDir /r "${INSTDIR_DATA}\*.*"
  RMDir "${INSTDIR_DATA}"

  ;Delete Start Menu Shortcuts
  Delete "$SMPROGRAMS\${PRODUCT}\*.*"
  RmDir  "$SMPROGRAMS\${PRODUCT}"

  Delete "$DESKTOP\CNCLib.Wpf.lnk"
  
SectionEnd


;--------------------------------
;After Installation Function

Function .onInstSuccess

  ;Open 'Thank you for installing' site or something else
  ExecShell "open" "microsoft-edge:${AFTER_INSTALLATION_URL}"

FunctionEnd