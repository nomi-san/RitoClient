<br>

<div align="center">
  <img src="https://i.imgur.com/WfGUuSy.png" width="96" height="96" />
  <h3 align="center"><strike>RiotClient</strike></h3>
  <h1 align="center">RitoClient</h1>
  <p align="center">
    🤿 Deep dive into your RiotClient UX
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

- .NET 8.0 SDK
- Windows SDK 10.0

### Build steps

- Clone the repo
- Open terminal in repo and run

```
dotnet publish -c Release -o bin\
```

## 🌟 Getting started

### Installation

1. Run `bin/install.bat` (as admin) to activate the core module
2. Put your **JavaScript** files in `bin/preload` folder

```
bin/
  |__ RitoClient.dll
  |__ preload/
     |__ hello.js
     |__ ... .js
```

3. Launch your **Riot Client** and enjoy!

### Key bindings
- <kbd>Ctrl + Shift + R</kbd> to reload the client
- <kbd>Ctrl + Shift + I</kbd> to open the **remote DevTools**

![image](https://github.com/nomi-san/RitoClient/assets/38210249/8d1adc0e-9a52-4b06-93e0-660aa84ab9a5)

## 🤔 FAQs

#### Why C# AOT?
For testing the latest .NET SDK and Native AOT. Some code in this repo come from [PenguLoader](https://github.com/PenguLoader/PenguLoader) but C# version.

#### Why no more IFEO?
~~The new RiotClient Electron uses single executable for both browser process and renderer process, so the IFEO debugger will not work due to chrome_elf.~~

IFEO mode is back! [#90c4f72](https://github.com/nomi-san/RitoClient/commit/90c4f7269dcd8771242583eb3dab12b93bf718ba)

#### Why remote DevTools?
The new method is to enable remote debugger inside the Electron app, however the app has disabled built-in DevTools. Cannot create a BrowserWindow, so the remote DevTools should be opened in your web browser.
