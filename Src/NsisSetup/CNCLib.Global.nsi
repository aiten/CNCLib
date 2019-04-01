;--------------------------------
;General

Unicode true

;Define name of the product
!define COMPANYNAME "Aitenbichler Herbert"
!define DESCRIPTION "CNCLib - a program to control your Arduino based CNC machine"

# These three must be integers
!define VERSIONMAJOR 1
!define VERSIONMINOR 2
!define VERSIONBUILD 0 

!define HELPURL "https://github.com/aiten/CNCLib" # "Support Information" link
!define UPDATEURL "https://github.com/aiten/CNCLib" # "Product Updates" link
!define ABOUTURL "https://github.com/aiten/CNCLib" # "Publisher" link

;Define optional URL that will be opened after the installation was successful
!define AFTER_INSTALLATION_URL "https://github.com/aiten/CNCLib"

VIAddVersionKey "Comments" "${DESCRIPTION}"
VIAddVersionKey "CompanyName" "${COMPANYNAME}"
VIAddVersionKey "LegalCopyright" "(c) by ${COMPANYNAME}"
VIAddVersionKey "FileVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
VIAddVersionKey "ProductName" "${PRODUCT}"
VIAddVersionKey "FileDescription" "${PRODUCT}"

VIFileVersion ${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}.0
VIProductVersion ${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}.0

;Request rights if you want to install the program to program files
RequestExecutionLevel admin

;--------------------------------

!define INSTALLERFILE "${PRODUCT}_install.exe"
!define UNINSTALLERFILE "${PRODUCT}_uninstaller.exe"

!define PRODUCT_REGKEY "Software\${PRODUCT}"
