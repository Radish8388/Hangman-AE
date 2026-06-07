[Setup]
AppName=Hangman
AppVersion=1.0
DefaultDirName={autopf}\Radish\HangmanAE
DefaultGroupName=Radish
SetupIconFile=hm.ico
UninstallDisplayIcon={app}\Hangman.exe
LicenseFile=LICENSE.txt
OutputBaseFilename=HangmanAESetup
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
AppPublisher=Radish
AppPublisherURL=https://radish-vert.vercel.app
AppId={{1de6d98c-42b8-42f6-910c-e79128351ca4}

[Files]
Source: "bin\Release\net10.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\Hangman"; Filename: "{app}\Hangman.exe"
Name: "{commondesktop}\Hangman"; Filename: "{app}\Hangman.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\Hangman.exe"; Description: "Launch Hangman"; Flags: nowait postinstall skipifsilent
