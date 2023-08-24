<br>

<div align="center">
  <img src="https://i.imgur.com/WfGUuSy.png" width="96" height="96" />
  <h3 align="center"><strike>RiotClient</strike></h3>
  <h1 align="center">RitoClient</h1>
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

- Visual Studio 2017
  - Desktop development with C++
  - Windows 8.1 SDK

### Build steps

1. Clone this repo
2. Open `vsproj/RitoClient.sln` in Visual Studio
3. Set configuration mode to `Release` and `Win32` (x86)
4. Press build

## 🌟 Getting started

### Usage

1. Run `bin/install.bat` (as admin) to activate the core module
2. Put your JavaScript files in `bin/preload` folder
3. Launch your RiotClient and enjoy!

### Key bindings

- <kbd>Ctrl + Shift + I</kbd> to open DevTools
- <kbd>Ctrl + Shift + R</kbd> to reload the client

## 🚀 Runtime API

### `DataStore`

Store your data locally like `localStorage`.

- `DataStore.get(key, fallback?)` - get value by key
- `DataStore.set(key, value)` - set value by key
- `DataStore.has(key)` - does the key exist?
- `DataStore.remove(key)` - remove the given key
