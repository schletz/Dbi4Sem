# Installation von .NET Core SDK für Linux

Besuchen Sie die .NET Core Downloadseite unter
[dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet-core).

![](donetcore_download.png)

Wählen Sie die .NET Core Version 3.1 (1) und danach im Bereich SDK unter Linux die Binaries für
x64 (2). Es öffnet sich ein Downloaddialog, den sie aber abbrechen. Wir wollen nämlich das Paket in
unserer VM direkt laden.

Dafür kopieren Sie den angebotenen Direct link mit dem Copy Button (3) in die Zwischenablage. Nun
wechseln Sie mit *cd* im Terminal in Ihr Homeverzeichnis. Danach schreiben Sie im Terminal den
Befehl *wget* und fügen den kopierten Link nach einer Leerstelle ein (rechte Maustaste und *Paste*).

Nun kopieren Sie die Befehle für die Installation mit dem Copy Button (4) in die Zwischenablage und
führen Sie diese im Terminal aus. Prüfen Sie mit dem Befehl *dotnet --info*, ob die Installation
geklappt hat.

Damit der Pfad zur .NET Installation automatisch durchsucht wird. müssen Sie die Datei
*.bash_profile* in ihrem Homeverzeichnis editieren. Starten Sie hierfür den Editor nano mit folgenden
Befehlen im Terminal:

```bash
cd
nano .bash_profile

```

Suchen Sie nach der Zeile, die die Veriable *PATH* definiert und fügen Sie nach einem Doppelpunkt
*$HOME/dotnet* an:

```bash
PATH=$PATH:$HOME/.local/bin:$HOME/bin:$HOME/dotnet
```

Sie beenden den Editor, indem Sie *CTRL + X*, danach *y* und *ENTER* drücken.