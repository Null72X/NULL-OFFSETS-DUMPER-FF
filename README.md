# ⚡ NULL OFFSET DUMPER

<div align="center">

# 🔥 NULL OFFSET DUMPER

### Advanced Automatic dump.cs Offset & Bone Extraction Tool

![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows)
![Language](https://img.shields.io/badge/Language-C%2B%2B17-00599C?style=for-the-badge&logo=cplusplus)
![Architecture](https://img.shields.io/badge/Architecture-x64-success?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Active-red?style=for-the-badge)

### Extract Offsets • Dump Bones • Generate Ready-To-Use C# Files

</div>

---

## 📖 About

**NULL OFFSET DUMPER** is a lightweight and fast Windows utility written in **C++17** that automatically extracts offsets and bone identifiers from **dump.cs** files.

The application scans predefined classes and fields, locates their memory offsets, and generates clean C# output files that can be directly integrated into projects.

No manual searching.

No editing.

Just drop your dump and dump everything automatically.

---

## ✨ Features

### 🎯 Automatic dump.cs Detection

The program automatically searches for:

```text
dump.cs
```

inside the executable directory.

---

### 📂 File Picker Fallback

If dump.cs is not found, a Windows file dialog opens automatically.

---

### ⚡ Fast Offset Extraction

Automatically extracts:

- General Offsets
- Player Offsets
- Avatar Offsets
- Camera Offsets
- Weapon Offsets
- Observer Offsets
- Speed Offsets
- Silent Aim Offsets
- Aimbot Offsets

---

### 🦴 Bone Extraction

Automatically dumps:

```text
Head
Breast
Root
Hip

LeftBiceps
RightBiceps

LeftShoulder
RightShoulder

LeftWristJoint
RightWristJoint

LeftLeg
RightLeg

LeftFoot
RightFoot
```

---

### 📝 Automatic File Generation

Creates:

```text
offsets.txt
bones.txt
```

ready for copy-paste.

---

### 🖥️ Real-Time Console Logs

Displays:

```text
FOUND: CurrentMatch = 0x50
FOUND: LocalPlayer = 0x94

FOUND BONE: Head = 0x2A0
FOUND BONE: Breast = 0x2A4
```

while dumping.

---

## 📸 Preview

```text
==============================================================
                    NULL OFFSET DUMPER
                         BY GAURAV
==============================================================

[SUCCESS] Found dump.cs

Welcome to Gaurav X Offset Dumper!

Analyzing dump file...

Dumping Offsets...

FOUND: CurrentMatch = 0x50
FOUND: MatchStatus = 0x8C
FOUND: LocalPlayer = 0x94

SUCCESS: Dumped 35 offsets to offsets.txt

Dumping Bones...

FOUND BONE: Head = 0x2A0
FOUND BONE: Breast = 0x2A4

SUCCESS: Dumped 14 bones to bones.txt

FINAL RESULTS:
==================================
Offsets found: 35
Bones found: 14
Total: 49
```

---

# 📦 Output Example

## offsets.txt

```csharp
namespace Client
{
    internal static class Offsets
    {
        internal static uint CurrentMatch = 0x50;
        internal static uint MatchStatus = 0x8C;
        internal static uint LocalPlayer = 0x94;
        internal static uint Weapon = 0x3A4;
    }
}
```

## bones.txt

```csharp
namespace Client
{
    internal enum Bones : uint
    {
        Head = 0x2A0,
        Breast = 0x2A4,
        Root = 0x2B0
    }
}
```

---

# 📂 Extracted Categories

## General

```text
CurrentMatch
MatchStatus
LocalPlayer
DictionaryEntities
```

## Player

```text
Player_IsDead
Player_Name
Player_Data
Player_ShadowBase
XPose
PlayerAttributes
```

## Avatar

```text
AvatarManager
Avatar
Avatar_IsVisible
Avatar_Data
Avatar_Data_IsTeam
```

## Camera

```text
FollowCamera
Camera
MainCameraTransform
AimRotation
ViewMatrix
```

## Weapon

```text
Weapon
WeaponData
WeaponRecoil
InventoryManager
WeaponOnHand
WeaponInfo
WeaponID
BuffWeaponAmmoClip
```

## Observer

```text
CurrentObserver
ObserverPlayer
```

## Silent Aim

```text
sAim1
sAim2
sAim3
sAim4
```

## Aimbot

```text
AimbotVisible
HeadCollider
```

## Speed Internal

```text
FixedDeltaTime
GameTimer
```

---

# 🔨 Build

```bash
git clone https://github.com/YOUR_USERNAME/NULL-OFFSET-DUMPER.git
```

Open:

```text
NULL OFFSET DUMPER.sln
```

Build:

```text
Release x64
```

---

# ⚙️ Requirements

```text
Windows 10 / 11
Visual Studio 2022
C++17
Win32 API
```

---

# 📁 Project Structure

```text
NULL OFFSET DUMPER
│
├── main.cpp
├── dump.cs
├── offsets.txt
├── bones.txt
└── README.md
```

---

# 👨‍💻 Author

## GAURAV

Creator of:

- NULL OFFSET DUMPER

---

<div align="center">

### 🔥 NULL OFFSET DUMPER

Fast • Clean • Automatic

Made By Gaurav

</div>
