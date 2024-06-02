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

Note that the using SDK in this project is `9.0.0-preview.5.24262.1`, if you get different SDK version, please change it in the `.csproj` project file.

```xml
   ...
   <ItemGroup>
     <PackageReference
       Include="Microsoft.DotNet.ILCompiler; runtime.win-x64.Microsoft.DotNet.ILCompiler"
       Version="9.0.0-preview.5.24262.1"
     />
     ...
```

### Build steps

```
git clone https://github.com/nomi-san/RitoClient.git
cd RitoClient
dotnet publish -c Release -r win-x86 -o bin\
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
admin$ mklink "path\to\Riot Client\RiotClientElectron\dwrite.dll" "bin\RitoClient.dll"
```

2. Put your **JavaScript** files in `preload` folder

```js
console.info('%c RitoClient ', 'background: #eb0029; color: #fff', 'Hi Im Gosu :)')
```

3. Launch your **RiotClient** and enjoy!

### Key bindings
- <kbd>Ctrl + Shift + R</kbd> to reload the client
- <kbd>Ctrl + Shift + I</kbd> to open the **remote DevTools**

![image](https://github.com/nomi-san/RitoClient/assets/38210249/8d1adc0e-9a52-4b06-93e0-660aa84ab9a5)

## 🤔 FAQs

#### Why C# AOT?
For testing the latest .NET SDK and Native AOT. Some code in this repo come from [PenguLoader](https://github.com/PenguLoader/PenguLoader) but C# version.

#### Why no more IFEO?
The new RiotClient Electron uses single executable for both browser process and renderer process, so the IFEO debugger will not work due to chrome_elf.

#### Why remote DevTools?
The new method is to enable remote debugger inside the Electron app, however the app has disabled built-in DevTools. Cannot create a BrowserWindow, so the remote DevTools should be opened in your web browser.
