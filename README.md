# ⚡ NULL OFFSET FINDER / EXTRACTOR

<div align="center">

### Advanced Automatic `dump.cs` Offset & Bone Extraction Tool

![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows)
![Language](https://img.shields.io/badge/Language-C%23-239120?style=for-the-badge&logo=csharp)
![Framework](https://img.shields.io/badge/Framework-.NET%204.7.2-512BD4?style=for-the-badge&logo=.net)
![Architecture](https://img.shields.io/badge/Architecture-x64%20%7C%20x86-success?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Active-red?style=for-the-badge)

### Scan Dump Files • Locate Memory Offsets • Output Ready-to-Use C# Source Files

</div>

---

## 📖 About

**NULL OFFSET FINDER** is a lightweight, high-performance Windows console utility written in **C# (.NET Framework 4.7.2)**. It automates the tedious process of searching through large **`dump.cs`** files (IL2CPP dumps) to extract vital memory offsets and bone indexes. 

The tool scans predefined target classes and fields, maps their current memory offsets via optimized regular expression lookup, and generates clean C# namespace classes/enums that can be dropped directly into your game enhancement or reverse engineering projects.

---

## ✨ Features

* 🎯 **Automatic `dump.cs` Location**: Searches for `dump.cs` in both the application's base directory and the active working directory automatically on startup.
* 📂 **Interactive File Picker Fallback**: If no dump file is found locally, it gracefully opens a Windows Forms file selection dialog (`OpenFileDialog`) to let you browse for your file.
* ⚡ **Optimized Extraction Engine**: Employs compiled regular expressions to process millions of lines of dump files in milliseconds.
* 🦴 **Bone Identity Mapping**: Parses complex skeletal structures (Head, Hip, Breast, Shoulders, Limbs) and maps them directly into an enum.
* 📝 **Ready-to-Use C# Output Generation**:
  * `offsets.txt`: A structured class file (`namespace Client.Offsets`) categorized by module.
  * `bones.txt`: An enum wrapper (`namespace Client.Bones`) holding bone indices.
* 🖥️ **Verbose Console Diagnostics**: Displays real-time logging, showing lines processed, matches found, missing classes/offsets, and elapsed execution time.

---

## 📦 Extracted Categories & Fields

The extractor targets the following classes and fields from the IL2CPP dump:

### ⚙️ General & Core Offsets
* **`CurrentMatch`** (Class: `MatchGame`, Field: `m_Match`)
* **`MatchStatus`** (Class: `EMKJHAJNPDH`, Field: `MAOHIOEAMEA`)
* **`LocalPlayer`** (Class: `EMKJHAJNPDH`, Field: `PDBGEOANOEP`)
* **`DictionaryEntities`** (Class: `MatchGame`, Field: `m_ReplicationEntitis`)

### 👤 Player & Actor Details
* **`Player_IsDead`** (Class: `AttackableEntity`, Field: `PIKCADEGMOH`)
* **`Player_Name`** (Class: `Player`, Field: `OriginalNickName`)
* **`Player_Data`** (Class: `ReplicationEntity`, Field: `m_PRIDataPool`)
* **`Player_ShadowBase`** (Class: `PlayerNetwork`, Field: `m_ShadowState`)
* **`XPose`** (Class: `PlayerNetwork`, Field: `BGDKLEHDFJO`)
* **`LocalPlayerAttributes`** (Class: `Player`, Field: `KDJHNBAECLM`)

### 🎭 Avatar Properties
* **`AvatarManager`**, **`Avatar`**, **`Avatar_IsVisible`**, **`Avatar_Data`**, **`Avatar_Data_IsTeam`**, **`BaseProfileInfo`**

### 📹 Camera & Aim Rotation
* **`FollowCamera`**, **`Camera`**, **`MainCameraTransform`**, **`AimRotation`**, **`ViewMatrix`**

### 🔫 Weapon System & Recoil
* **`Weapon`**, **`WeaponData`**, **`WeaponRecoil`**, **`InventoryManager`**, **`WeaponOnHand`**, **`WeaponInfo`**, **`WeaponID`**, **`BuffWeaponAmmoClip`**, **`Rapidfire`**

### 🎯 Silent Aim & Aimbot
* **`sAim1`**, **`sAim2`**, **`sAim3`**, **`sAim4`**
* **`AimbotVisible`**, **`HeadCollider`**

### 🦴 Skeletal Bones
* `Head`, `Breast`, `Hip`, `Root`, `LeftBiceps`, `RightBiceps`, `LeftWristJoint`, `RightWristJoint`, `LeftShoulder`, `RightShoulder`, `LeftFoot`, `RightFoot`, `LeftLeg`, `RightLeg`

---

## 🚀 Usage & Execution Flow

```text
==========================================================================
                       NULL X OFFSET & BONES EXTRACTOR
==========================================================================

[SUCCESS] Found dump.cs automatically at: C:\Path\To\dump.cs

[*] Reading: dump.cs ...
[+] Loaded 1,450,230 lines into memory.

[+] Extracting Internal Offsets...
  [+] FOUND: CurrentMatch     ["m_Match"] = 0x50     (Line: 12,450)
  [+] FOUND: MatchStatus      ["MAOHIOEAMEA"] = 0x8C     (Line: 24,912)
  [-] NOT FOUND: PlayerNetwork::BGDKLEHDFJO (XPose)
...
[SUCCESS] Dumped 34 offsets to: C:\Path\To\offsets.txt

[+] Extracting Bones...
  [+] FOUND: Head             ["PEMOFNFCLFB"] = 0x2A0    (Line: 84,310)
...
[SUCCESS] Dumped 14 bones to: C:\Path\To\bones.txt

==========================================================================
                             FINAL SUMMARY
==========================================================================
  Offsets Extracted : 34 / 35
  Bones Extracted   : 14 / 14
  Execution Time    : 342 ms
  Output Directory  : C:\Path\To\
==========================================================================
```

---

## 🛠️ Build Requirements

* **Operating System**: Windows 10 or 11
* **Developer Environment**: Visual Studio 2022 (or any C# compiler supporting .NET Framework 4.7.2)
* **Dependencies**:
  * `System.Windows.Forms` (for file-picker GUI dialog)
  * `System.Text.RegularExpressions` (for fast parsing)

---

## 📂 Project Structure

```text
NULL-OFFSETS-DUMPER-FF
│   NULL OFFSET FINDER.sln             # Visual Studio Solution File
│   README.md                          # Project Documentation
│
└───NULL OFFSET FINDER                 # Core Project Folder
    │   App.config                     # Target Framework Configuration (.NET 4.7.2)
    │   NULL OFFSET FINDER.csproj      # C# Project File
    │   Program.cs                     # Core Extractor Engine
    │
    └───Properties
            AssemblyInfo.cs            # Assembly Metadata
```

---

## 👨‍💻 Author

**GAURAV**
* Creator of NULL OFFSET FINDER / EXTRACTOR.
