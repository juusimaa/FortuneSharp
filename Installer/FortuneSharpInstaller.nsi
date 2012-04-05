;--------------------------------
;Definitions
!define COMPANY_NAME ""
!define VERSION "1.8.0"
!define PRODUCT_NAME "FortuneSharp ${VERSION}"
!define PRODUCT_NAME_NO_VERSION "FortuneSharp"
!define OUTPUT_FILE "FortuneSharpInstall.exe"
!define INSTALL_DIR "FortuneSharp"
!define REG_ROOT "Software\${COMPANY_NAME}"
!define REG_APP_PATH "${REG_ROOT}\${PRODUCT_NAME}"
!define REQ_NET_VERSION "2.0"
!define DOTNET_REQUIRED "${PRODUCT_NAME} requires that the .NET Framework 2.0 \
	is installed.$\nThe .NET Framework will be installed automatically$\nduring \
	installation of ${PRODUCT_NAME}."
!define MULTIUSER_MUI
!define MULTIUSER_EXECUTIONLEVEL Standard
!define MULTIUSER_INSTALLMODE_DEFAULT_CURRENTUSER
!define MULTIUSER_INSTALLMODE_INSTDIR "${INSTALL_DIR}"

;--------------------------------
;Includes
!include MultiUser.nsh
!include MUI2.nsh
!include LogicLib.nsh
!include WordFunc.nsh
!include EnvVarUpdate.nsh
!insertmacro VersionCompare
	
;--------------------------------
; Modern UI data
!define MUI_ICON "Farnsworth.ico"

;--------------------------------
;Variables
Var InstallDotNET

;--------------------------------
;Macros
!macro InstallDotNET
	${If} $InstallDotNET == "Yes"

    ExecWait "$INSTDIR\dotnetfx.exe"
    Delete "$INSTDIR\dotnetfx.exe"
 
    SetDetailsView show
	${EndIf}
!macroend

!macro UpdatePath
	${EnvVarUpdate} $0 "PATH" "A" "HKCU" "$INSTDIR"
!macroend

!macro UpdateReg
	WriteRegStr HKCU "Software\${PRODUCT_NAME}" "FortunePath" "$INSTDIR\"
!macroend

;--------------------------------
;Functions
Function .onInit	
	!insertmacro MULTIUSER_INIT
	
	StrCpy $InstallDotNET "No"
	Call GetDotNETVersion
	Pop $0
	
	${If} $0 == "not found"
		StrCpy $InstallDotNET "Yes"
		MessageBox MB_OK|MB_ICONINFORMATION "${DOTNET_REQUIRED}"
	Return
	${EndIf}

	StrCpy $0 $0 "" 1 # skip "v"

	${VersionCompare} $0 "${REQ_NET_VERSION}" $1
	${If} $1 == 2
		StrCpy $InstallDotNET "Yes"
		MessageBox MB_OK|MB_ICONINFORMATION "${DOTNET_REQUIRED}"
		Return
	${EndIf}
FunctionEnd

Function un.onInit
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

Function GetDotNETVersion
	Push $0
	Push $1
 
	System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i) i .r1 ?u"
	StrCmp $1 "error" 0 +2
	StrCpy $0 "not found"
 
	Pop $1
	Exch $0
FunctionEnd

;--------------------------------
;General
Name "${PRODUCT_NAME}"
OutFile "${OUTPUT_FILE}"

;Default installation folder
InstallDir "${MULTIUSER_INSTALLMODE_INSTDIR}"

;Get installation folder from registry if available
InstallDirRegKey HKCU "${REG_APP_PATH}" "InstallDir"

;Request application privileges for Windows Vista/7
RequestExecutionLevel admin

;--------------------------------
;Interface Settings
!define MUI_ABORTWARNING

;--------------------------------
;Pages (install)
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE license.txt
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

;--------------------------------
;Pages (uninstall)
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages 
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections
Section "FortuneSharp 1.8.0" SecFiles
	SetOverwrite on
    SetOutPath "$INSTDIR"
  
	;Installed files
	File "..\bin\Release\Fortune.exe"
	
	CreateDirectory "$INSTDIR\Cookies"	
	CreateDirectory "$INSTDIR\Off"

	SetOutPath "$INSTDIR\Cookies" 
	File "..\resources\Cookies\*.*"
	
	SetOutPath "$INSTDIR\Off"
	File "..\resources\Off\*.*"
	 
	;Store installation folder
	WriteRegStr HKCU "${REG_APP_PATH}" "InstallDir" $INSTDIR
	 
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	!insertmacro UpdatePath	
	!insertmacro UpdateReg	
	!insertmacro InstallDotNET

SectionEnd

;--------------------------------
;Descriptions
;Language strings
LangString DESC_SecFiles ${LANG_ENGLISH} "${PRODUCT_NAME} required files."

;--------------------------------
;Uninstaller Section
Section "Uninstall"
	
	;Delete files	
	Delete "$INSTDIR\Fortune.exe"
	RMDir /r "$INSTDIR\Cookies"
	RMDir /r "$INSTDIR\Off"
	RMDir /r "$INSTDIR"

  DeleteRegKey /ifempty HKCU "${REG_APP_PATH}"

SectionEnd