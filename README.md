# Null-Offsets-Dumper

## Overview
Automated C# console utility designed to scan memory signatures, parse structures, and output updated game offsets and pointers.

## Features
- **Pattern Scanning Engine**: Automated memory pattern searching using wildcard byte masks.
- **Struct Offset Resolution**: Resolves critical game structures, player matrices, and coordinate pointers.
- **Structured Export**: Exports updated offsets in human-readable format and C# class definitions.
- **Lightweight Execution**: Fast console-based scanning with immediate feedback.

## Structure
```text
Null-Offsets-Dumper/
├── Null-Offsets-Dumper/
│   ├── Null-Offsets-Dumper.csproj
│   ├── Program.cs
│   └── App.config
├── Null-Offsets-Dumper.sln
├── README.md
└── .gitignore
```

## Requirements
- Windows 10 / 11 (64-bit)
- .NET Framework 4.7.2+ or .NET SDK
- Visual Studio 2022

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/Null72X/Null-Offsets-Dumper.git
   ```
2. Open `Null-Offsets-Dumper.sln` in Visual Studio 2022.

## Usage
1. Launch the target game or emulator.
2. Run `Null-Offsets-Dumper.exe` as Administrator.
3. Check the console output or generated file for updated offset values.

## Build
Compile in Visual Studio 2022 under **Release | Any CPU** (**Ctrl + Shift + B**).
