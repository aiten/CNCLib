;--------------------------------
;Include Modern UI 2

  !include "MUI2.nsh"

  ;--------------------------------
;global

  ;Define the main name of the installer
  !define PRODUCT "CNCLib"
  !define PRODUCT_SUBDIR "${PRODUCT}"
  !define PRODUCT_EXE "CNCLib.Wpf.Start.exe"
  !define PRODUCT_LNK "CNCLib.lnk"

  !include "CNCLib.Global.nsi"

  ;--------------------------------

  !ifndef CopyFromFolder
    !define CopyFromFolder "..\CNCLib\CNCLib.Wpf.Start\bin\Debug"
  !endif 
  
;--------------------------------
;General

  Name "${PRODUCT}"

  ;Define the directory where the installer should be saved
  OutFile ".\Output\${INSTALLERFILE}"

  ;Define the default installation folder (Windows ProgramFiles example)
  InstallDir "$PROGRAMFILES\${PRODUCT_SUBDIR}"

  ;Define optional a directory for program files that change (Windows AppData example)
  !define INSTDIR_DATA "$APPDATA\${PRODUCT_SUBDIR}"

  ;Define optional a directory for program files that change (Windows AppData example)
  !define INSTDIR_DATALOCAL "$LOCALAPPDATA\${PRODUCT_SUBDIR}"

   ;Request rights if you want to install the program to program files
  RequestExecutionLevel admin

  ;Show 'console' in installer and uninstaller
  ShowInstDetails "show"
  ShowUninstDetails "show"

  ;Get installation folder from registry if available
  InstallDirRegKey HKLM "${PRODUCT_REGKEY}" ""

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

;--------------------------------
;Installer Section

Section "Main Component"
  SectionIn RO # Just means if in component mode this is locked

  ;Set output path to the installation directory.
  SetOutPath $INSTDIR

  ;Put the following file in the SetOutPath

  File /r /x "*.pdb" "${CopyFromFolder}\*.*"
  
  ;Store installation folder in registry
  WriteRegStr HKLM "${PRODUCT_REGKEY}" "" $INSTDIR

  ;Registry information for add/remove programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayName" "${PRODUCT}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "UninstallString" '"$INSTDIR\${UNINSTALLERFILE}"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayIcon" "$\"$INSTDIR\${UNINSTALLERFILE}$\""
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
  CreateDirectory "$SMPROGRAMS\${PRODUCT_SUBDIR}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_SUBDIR}\${PRODUCT_LNK}" "$INSTDIR\${PRODUCT_EXE}" "" "$INSTDIR\${PRODUCT_EXE}" 0

  CreateShortCut "$DESKTOP\${PRODUCT_LNK}" "$INSTDIR\${PRODUCT_EXE}" "" "$INSTDIR\${PRODUCT_EXE}" 0

  ;Create ProgramData 
  CreateDirectory "${INSTDIR_DATA}"
  CreateDirectory "${INSTDIR_DATALOCAL}"
  CreateDirectory "${INSTDIR_DATALOCAL}\logs"

  ;Create uninstaller
  WriteUninstaller "${UNINSTALLERFILE}"

SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;Remove all registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}"
  DeleteRegKey HKLM "${PRODUCT_REGKEY}"

  ;Delete the installation directory + all files in it
  ;Add 'RMDir /r "$INSTDIR\folder\*.*"' for every folder you have added additionaly
  RMDir /r "$INSTDIR\*.*"
  RMDir "$INSTDIR"

  ;Delete the appdata directory + files
  RMDir /r "${INSTDIR_DATA}\*.*"
  RMDir "${INSTDIR_DATA}"

  ;Delete the appdata directory + files
  RMDir /r "${INSTDIR_DATALOCAL}\*.*"
  RMDir "${INSTDIR_DATALOCAL}"
  
  ;Delete Start Menu Shortcuts
  Delete "$SMPROGRAMS\${PRODUCT}\*.*"
  RmDir  "$SMPROGRAMS\${PRODUCT}"

  Delete "$DESKTOP\${PRODUCT_LNK}"
 
SectionEnd


;--------------------------------
;After Installation Function

Function .onInstSuccess

  ;Open 'Thank you for installing' site or something else
  ExecShell "open" "microsoft-edge:${AFTER_INSTALLATION_URL}"

FunctionEnd