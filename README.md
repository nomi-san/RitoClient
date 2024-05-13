<br>

<div align="center">
  <img src="https://i.imgur.com/WfGUuSy.png" width="96" height="96" />
  <h3 align="center"><strike>RiotClient</strike></h3>
  <h1 align="center">RitoClient Electron</h1>
  <p align="center">
    🤿 Deep dive into your <strong>RiotClient</strong>
  </p>
  <a href="https://github.com/nomi-san/RitoClient">
    <img src="https://img.shields.io/github/stars/nomi-san/RitoClient.svg?style=for-the-badge&logo=github" />
  </a>
  <a href="https://github.com/nomi-san/RitoClient/blob/main/LICENSE">
    <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg?style=for-the-badge" />
  </a>
</div>

<br>

## 🔨 Building

### Prerequisites

The current RiotClient is 32-bit app, so you need the latest preview .NET 9.0 SDK that supports x86.
  - https://aka.ms/dotnet/9.0/daily/dotnet-sdk-win-x64.exe

### Build steps

```
git clone https://github.com/nomi-san/RitoClient.git
cd RitoClient
dotnet publish -r win-x86
```

## 🌟 Getting started

### Installation

```
bin/
  |__ RitoClient.dll
  |__ preload/
     |__ hello.js
```

1. Create a symlink named `dwrite.dll` in your **RiotClientElectron** folder that points to the output `RitoClient.dll`.

```
admin$ mklink "path\to\RiotClientElectron\dwrite.dll" "path\to\RitoClient.dll"
```

2. Put your **JavaScript** files in `preload` folder that relative to `RitoClient.dll`.

3. Launch your **RiotClient** and enjoy!

### Key bindings
- <kbd>Ctrl + Shift + R</kbd> to reload the client
- <kbd>Ctrl + Shift + I</kbd> to open the **remote DevTools**

## 🤔 FAQs

#### Why C# AOT?
For testing the latest .NET SDK and Native AOT. Some code in this repo come from [PenguLoader](https://github.com/PenguLoader/PenguLoader) but C# version.

#### Why no more IFEO?
The new RiotClient Electron uses single executable for both browser process and renderer process, so the IFEO debugger will not work due to chrome_elf.

#### Why remote DevTools?
The new method is to enable remote debugger inside the Electron app, however the app has disabled built-in DevTools. Cannot create a BrowserWindow, so the remote DevTools should be opened in your web browser.
