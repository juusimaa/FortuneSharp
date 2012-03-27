;--------------------------------
;Definitions
!define COMPANY_NAME ""
!define VERSION "1.6.0"
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

;Get EB Promsim installation folder from registry if available
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
Section "FortuneSharp 1.6.0" SecFiles
	SetOverwrite on
  SetOutPath "$INSTDIR"
  
  ;Installed files
	File "..\bin\Release\Fortune.exe"
	
	CreateDirectory "$INSTDIR\Cookies"	
	CreateDirectory "$INSTDIR\Off"
	
	;CopyFiles "..\resources\Cookies\*" "$INSTDIR\Cookies"
	;CopyFiles /FILESONLY "..\resources\Off\" "$INSTDIR\Off\"
	
	File "/oname=Cookies\art" "..\resources\Cookies\art"
	File "/oname=Cookies\ascii-art" "..\resources\Cookies\ascii-art"
	File "/oname=Cookies\bofh" "..\resources\Cookies\bofh"
	File "/oname=Cookies\chalkboard" "..\resources\Cookies\chalkboard"
	File "/oname=Cookies\chucknorris" "..\resources\Cookies\chucknorris"
	File "/oname=Cookies\computers" "..\resources\Cookies\computers"
	File "/oname=Cookies\cookie" "..\resources\Cookies\cookie"
	File "/oname=Cookies\definitions" "..\resources\Cookies\definitions"
	File "/oname=Cookies\drugs" "..\resources\Cookies\drugs"
	File "/oname=Cookies\education" "..\resources\Cookies\education"
	File "/oname=Cookies\ethnic" "..\resources\Cookies\ethnic"
	File "/oname=Cookies\familyguy" "..\resources\Cookies\familyguy"
	File "/oname=Cookies\food" "..\resources\Cookies\food"
	File "/oname=Cookies\fortunes" "..\resources\Cookies\fortunes"
	File "/oname=Cookies\futurama" "..\resources\Cookies\futurama"
	File "/oname=Cookies\goedel" "..\resources\Cookies\goedel"
	File "/oname=Cookies\hitchhiker" "..\resources\Cookies\hitchhiker"
	File "/oname=Cookies\homer" "..\resources\Cookies\homer"
	File "/oname=Cookies\humorists" "..\resources\Cookies\humorists"
	File "/oname=Cookies\kids" "..\resources\Cookies\kids"
	File "/oname=Cookies\law" "..\resources\Cookies\law"
	File "/oname=Cookies\linuxcookie" "..\resources\Cookies\linuxcookie"
	File "/oname=Cookies\literature" "..\resources\Cookies\literature"
	File "/oname=Cookies\love" "..\resources\Cookies\love"
	File "/oname=Cookies\magic" "..\resources\Cookies\magic"
	File "/oname=Cookies\medicine" "..\resources\Cookies\medicine"
	File "/oname=Cookies\men-women" "..\resources\Cookies\men-women"
	File "/oname=Cookies\miscellaneous" "..\resources\Cookies\miscellaneous"
	File "/oname=Cookies\news" "..\resources\Cookies\news"
	File "/oname=Cookies\people" "..\resources\Cookies\people"
	File "/oname=Cookies\pets" "..\resources\Cookies\pets"
	File "/oname=Cookies\platitudes" "..\resources\Cookies\platitudes"
	File "/oname=Cookies\politics" "..\resources\Cookies\politics"
	File "/oname=Cookies\riddles" "..\resources\Cookies\riddles"
	File "/oname=Cookies\science" "..\resources\Cookies\science"
	File "/oname=Cookies\songs-poems" "..\resources\Cookies\songs-poems"
	File "/oname=Cookies\sports" "..\resources\Cookies\sports"
	File "/oname=Cookies\startrek" "..\resources\Cookies\startrek"
	File "/oname=Cookies\starwars" "..\resources\Cookies\starwars"
	File "/oname=Cookies\translate-me" "..\resources\Cookies\translate-me"
	File "/oname=Cookies\wisdom" "..\resources\Cookies\wisdom"
	File "/oname=Cookies\work" "..\resources\Cookies\work"
	File "/oname=Cookies\zippy" "..\resources\Cookies\zippy"
	
	File "/oname=Off\astrology" "..\resources\Off\astrology"
	File "/oname=Off\black-humor" "..\resources\Off\black-humor"
	File "/oname=Off\definitions" "..\resources\Off\definitions"
	File "/oname=Off\drugs" "..\resources\Off\drugs"
	File "/oname=Off\ethnic" "..\resources\Off\ethnic"
	File "/oname=Off\hphobia" "..\resources\Off\hphobia"
	File "/oname=Off\limerick" "..\resources\Off\limerick"
	File "/oname=Off\misandry" "..\resources\Off\misandry"
	File "/oname=Off\miscellaneous" "..\resources\Off\miscellaneous"
	File "/oname=Off\misogyny" "..\resources\Off\misogyny"
	File "/oname=Off\politics" "..\resources\Off\politics"
	File "/oname=Off\privates" "..\resources\Off\privates"
	File "/oname=Off\racism" "..\resources\Off\racism"
	File "/oname=Off\religion" "..\resources\Off\religion"
	File "/oname=Off\riddles" "..\resources\Off\riddles"
	File "/oname=Off\sex" "..\resources\Off\sex"
	File "/oname=Off\songs-poems" "..\resources\Off\songs-poems"
	File "/oname=Off\vulgarity" "..\resources\Off\vulgarity"
	 
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