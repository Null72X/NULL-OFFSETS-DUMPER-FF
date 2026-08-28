using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Console.Title = "NULL OFFSET EXTRACTOR v3.3";

        PrintHeader();

        string filePath = FindDumpFile();
        if (string.IsNullOrEmpty(filePath))
        {
            PrintError("No dump file selected. Exiting...");
            Console.ReadKey();
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[*] Reading: {Path.GetFileName(filePath)} ...");
            Console.ResetColor();

            string[] allLines = File.ReadAllLines(filePath);
            Console.WriteLine($"[+] Loaded {allLines.Length:N0} lines into memory.");

            // ----------------------------------------------------
            // TARGET DEFINITIONS (Original Order Preserved)
            // ----------------------------------------------------
            var internalTargets = new List<ExtractionTarget>
            {
                // General
                new ExtractionTarget("MatchGame", "m_Match", "CurrentMatch", "General"),
                new ExtractionTarget("EMKJHAJNPDH", "MAOHIOEAMEA", "MatchStatus", "General"),
                new ExtractionTarget("EMKJHAJNPDH", "PDBGEOANOEP", "LocalPlayer", "General"),
                new ExtractionTarget("MatchGame", "m_ReplicationEntitis", "DictionaryEntities", "General"),

                // Player
                new ExtractionTarget("AttackableEntity", "PIKCADEGMOH", "Player_IsDead", "Player"),
                new ExtractionTarget("Player", "OriginalNickName", "Player_Name", "Player"),
                new ExtractionTarget("ReplicationEntity", "m_PRIDataPool", "Player_Data", "Player"),
                new ExtractionTarget("PlayerNetwork", "m_ShadowState", "Player_ShadowBase", "Player"),
                new ExtractionTarget("PlayerNetwork", "BGDKLEHDFJO", "XPose", "Player"),
                new ExtractionTarget("Player", "KDJHNBAECLM", "LocalPlayerAttributes", "Player"),

                // Avatar
                new ExtractionTarget("Player", "KPMDIPJINJO", "AvatarManager", "Avatar"),
                new ExtractionTarget("AvatarManager", "GIAMMAADHFN", "Avatar", "Avatar"),
                new ExtractionTarget("UmaAvatarSimple", "IsVisible", "Avatar_IsVisible", "Avatar"),
                new ExtractionTarget("UMAAvatarBase", "umaData", "Avatar_Data", "Avatar"),
                new ExtractionTarget("UMAData", "isTeammate", "Avatar_Data_IsTeam", "Avatar"),
                new ExtractionTarget("PlayerNetwork", "KAKEEBABBIP", "BaseProfileInfo", "Avatar"),

                // Camera
                new ExtractionTarget("Player", "OHEPMNHMJMK", "FollowCamera", "Camera"),
                new ExtractionTarget("CameraControllerBase", "ALGPBKBHFAL", "Camera", "Camera"),
                new ExtractionTarget("Player", "MainCameraTransform", "MainCameraTransform", "Camera"),
                new ExtractionTarget("Player", "<MDCADLIAJIH>k__BackingField", "AimRotation", "Camera"),
                new ExtractionTarget("BaseRuntimePanel", "panelToWorld", "ViewMatrix", "Camera"),

                // Observer
                new ExtractionTarget("EMKJHAJNPDH", "MPMAGJDHNBI", "CurrentObserver", "Observer"),
                new ExtractionTarget("PHLHIEGPMMK", "JAGIFDNJJFD", "ObserverPlayer", "Observer"),

                // Weapon
                new ExtractionTarget("Player", "ActiveUISightingWeapon", "Weapon", "Weapon"),
                new ExtractionTarget("FDAEPHMIEPC", "<FLCGCBLDMLK>k__BackingField", "WeaponData", "Weapon"),
                new ExtractionTarget("BNFFFLEJGMA", "HKNJLOBGIDP", "WeaponRecoil", "Weapon"),
                new ExtractionTarget("Player", "LPEALCPGJBL", "InventoryManager", "Weapon"),
                new ExtractionTarget("OMELKCOGCBK", "CHAFOMFBKEG", "WeaponOnHand", "Weapon"),
                new ExtractionTarget("FDAEPHMIEPC", "DJMMOHAJFPB", "WeaponInfo", "Weapon"),
                new ExtractionTarget("AGACNOCEEFP", "FPANKJIJBIH", "WeaponID", "Weapon"),
                new ExtractionTarget("PlayerAttributes", "BuffWeaponAmmoClip", "BuffWeaponAmmoClip", "Weapon"),
                new ExtractionTarget("PlayerAttributes", "BONAJJEOPNF", "Rapidfire", "Weapon"),

                // Speed Internal
                new ExtractionTarget("TimeService", "m_DeltaTime", "FixedDeltaTime", "Speed Internal"),
                new ExtractionTarget("TimeService", "m_FixedDeltaTime", "GameTimer", "Speed Internal"),

                // Silent Aim
                new ExtractionTarget("Player", "NNFKGNCILNK>k__BackingField", "sAim1", "Silent Aim"),
                new ExtractionTarget("Player", "AKFLHNOIHED", "sAim2", "Silent Aim"),
                new ExtractionTarget("GMPGMPFNMFP", "LMAEGPEAECO", "sAim3", "Silent Aim"),
                new ExtractionTarget("GMPGMPFNMFP", "IKDEGKIICJP", "sAim4", "Silent Aim"),

                // Aimbot
                new ExtractionTarget("Player", "NFDNMIOPILM", "AimbotVisible", "Aimbot"),
                new ExtractionTarget("AttackableEntity", "<ACAGMJJNMCN>k__BackingField", "HeadCollider", "Aimbot")
            };

            // Original Bone Order
            var boneTargets = new List<ExtractionTarget>
            {
                new ExtractionTarget("Player", "PEMOFNFCLFB", "Head", "Bones"),
                new ExtractionTarget("Player", "KAKOKIHEPCF", "Breast", "Bones"),
                new ExtractionTarget("Player", "DIDHPFKMJJE", "Hip", "Bones"),
                new ExtractionTarget("Player", "KNFKIDHJCCO", "Root", "Bones"),

                new ExtractionTarget("Player", "PNPBBNDANEM", "RightBiceps", "Bones"),
                new ExtractionTarget("Player", "DIHJDDNIJHP", "LeftBiceps", "Bones"),

                new ExtractionTarget("Player", "KNBJLEHOPIL", "RightWristJoint", "Bones"),
                new ExtractionTarget("Player", "KMIANNCLNOJ", "LeftWristJoint", "Bones"),

                new ExtractionTarget("Player", "OEJFBHIIBBG", "RightShoulder", "Bones"),
                new ExtractionTarget("Player", "NBHOEOOCIIG", "LeftShoulder", "Bones"),

                new ExtractionTarget("Player", "JLLMBADGKJP", "RightFoot", "Bones"),
                new ExtractionTarget("Player", "INHGPBHOKPF", "LeftFoot", "Bones"),

                new ExtractionTarget("Player", "BOHFCEHMJBD", "RightLeg", "Bones"),
                new ExtractionTarget("Player", "BIPBNNIFCNO", "LeftLeg", "Bones")
            };

            // ----------------------------------------------------
            // EXTRACTION EXECUTION
            // ----------------------------------------------------
            Console.WriteLine("\n[+] Extracting Internal Offsets...");
            var extractedOffsets = ExtractTargets(allLines, internalTargets);
            string offsetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offsets.txt");
            WriteOffsetsFile(extractedOffsets, offsetsPath);
            PrintSuccess($"Dumped {extractedOffsets.Count} offsets to: {offsetsPath}");

            Console.WriteLine("\n[+] Extracting Bones...");
            var extractedBones = ExtractTargets(allLines, boneTargets);
            string bonesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bones.txt");
            WriteBonesFile(extractedBones, bonesPath);
            PrintSuccess($"Dumped {extractedBones.Count} bones to: {bonesPath}");

            stopwatch.Stop();

            // ----------------------------------------------------
            // FINAL SUMMARY
            // ----------------------------------------------------
            DisplaySummary(extractedOffsets, extractedBones, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            PrintError($"EXTRACTION FAILED: {ex.Message}\n{ex.StackTrace}");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n[Press any key to exit...]");
        Console.ResetColor();
        Console.ReadKey();
    }

    #region Fast Extraction Engine (Preserves Defined Order)
    static List<ExtractionResult> ExtractTargets(string[] lines, List<ExtractionTarget> targets)
    {
        var targetLookup = targets.ToLookup(t => t.ClassName);
        var resultMap = new Dictionary<ExtractionTarget, ExtractionResult>();

        var classRegex = new Regex(@"^\s*(?:public|private|protected|internal)?\s*(?:static|sealed|abstract|partial)?\s*class\s+([A-Za-z0-9_<>]+)", RegexOptions.Compiled);
        var offsetRegex = new Regex(@"//\s*(0x[0-9a-fA-F]+)", RegexOptions.Compiled);

        string currentClass = null;
        int braceDepth = 0;
        var foundTargets = new HashSet<ExtractionTarget>();

        for (int i = 0; i < lines.Length; i++)
        {
            int lineNumber = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line)) continue;

            if (currentClass == null)
            {
                var classMatch = classRegex.Match(line);
                if (classMatch.Success)
                {
                    string className = classMatch.Groups[1].Value;
                    if (targetLookup.Contains(className))
                    {
                        currentClass = className;
                        braceDepth = CountChar(line, '{') - CountChar(line, '}');
                        continue;
                    }
                }
            }
            else
            {
                braceDepth += CountChar(line, '{') - CountChar(line, '}');
                if (braceDepth <= 0)
                {
                    currentClass = null;
                    braceDepth = 0;
                    continue;
                }

                foreach (var target in targetLookup[currentClass])
                {
                    if (foundTargets.Contains(target)) continue;

                    if (line.Contains(target.FieldName))
                    {
                        var offsetMatch = offsetRegex.Match(line);
                        if (offsetMatch.Success)
                        {
                            string hex = offsetMatch.Groups[1].Value;
                            var res = new ExtractionResult(target, hex, lineNumber, true);
                            resultMap[target] = res;
                            foundTargets.Add(target);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"  [+] FOUND: {target.ResultName,-16} [\"{target.FieldName}\"] = {hex,-8} (Line: {lineNumber:N0})");
                            Console.ResetColor();
                        }
                    }
                }
            }
        }

        // Return results matching the exact order of the original target list
        var results = new List<ExtractionResult>();
        foreach (var target in targets)
        {
            if (resultMap.TryGetValue(target, out var res))
            {
                results.Add(res);
            }
            else
            {
                var missingRes = new ExtractionResult(target, "0x0", -1, false);
                results.Add(missingRes);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [-] NOT FOUND: {target.ClassName}::{target.FieldName} ({target.ResultName})");
                Console.ResetColor();
            }
        }

        return results;
    }
    #endregion

    #region File Output Writers
    static void WriteOffsetsFile(List<ExtractionResult> results, string outputFile)
    {
        using (var writer = new StreamWriter(outputFile))
        {
            writer.WriteLine("namespace Client");
            writer.WriteLine("{");
            writer.WriteLine("    internal static class Offsets");
            writer.WriteLine("    {");
            writer.WriteLine("        // Core");
            writer.WriteLine("        internal static uint Il2Cpp;");
            writer.WriteLine("        internal static uint InitBase = 0x9EC1C48;");
            writer.WriteLine("        internal static uint StaticClass = 0x5C;\n");

            var grouped = results.GroupBy(r => r.Target.GroupName);

            foreach (var group in grouped)
            {
                writer.WriteLine($"        // {group.Key}");
                foreach (var item in group)
                {
                    string comment = item.IsFound
                        ? $"// {item.Target.ClassName}.{item.Target.FieldName} [Line {item.LineNumber:N0}]"
                        : $"// NOT FOUND in dump ({item.Target.ClassName}.{item.Target.FieldName})";

                    writer.WriteLine($"        internal static uint {item.Target.ResultName,-24} = {item.OffsetHex}; {comment}");
                }
                writer.WriteLine();
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }
    }

    static void WriteBonesFile(List<ExtractionResult> results, string outputFile)
    {
        using (var writer = new StreamWriter(outputFile))
        {
            writer.WriteLine("namespace Client");
            writer.WriteLine("{");
            writer.WriteLine("    internal enum Bones : uint");
            writer.WriteLine("    {");

            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                string comma = (i + 1 < results.Count) ? "," : "";
                string comment = item.IsFound
                    ? $"// Raw String: \"{item.Target.FieldName}\" [Line {item.LineNumber:N0}]"
                    : "// NOT FOUND";

                writer.WriteLine($"        {item.Target.ResultName,-16} = {item.OffsetHex}{comma,-2}  {comment}");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }
    }
    #endregion

    #region Helper & UI Methods
    static string FindDumpFile()
    {
        string[] searchPaths = {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dump.cs"),
            Path.Combine(Directory.GetCurrentDirectory(), "dump.cs")
        };

        foreach (var path in searchPaths)
        {
            if (File.Exists(path))
            {
                PrintSuccess($"Found dump.cs automatically at: {path}");
                return path;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\n[!] dump.cs not found in local directory. Opening file picker dialog...");
        Console.ResetColor();

        using (var dialog = new OpenFileDialog
        {
            Title = "Select IL2CPP Dump File (dump.cs)",
            Filter = "C# Dump Files (*.cs)|*.cs|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            Multiselect = false
        })
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
        }

        return null;
    }

    static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("==========================================================================");
        Console.WriteLine("                       NULL X OFFSET & BONES EXTRACTOR");
        Console.WriteLine("==========================================================================");
        Console.ResetColor();
    }

    static void DisplaySummary(List<ExtractionResult> offsets, List<ExtractionResult> bones, long elapsedMs)
    {
        int totalOffsets = offsets.Count;
        int foundOffsets = offsets.Count(o => o.IsFound);
        int totalBones = bones.Count;
        int foundBones = bones.Count(b => b.IsFound);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n==========================================================================");
        Console.WriteLine("                             FINAL SUMMARY");
        Console.WriteLine("==========================================================================");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  Offsets Extracted : {foundOffsets} / {totalOffsets}");
        Console.WriteLine($"  Bones Extracted   : {foundBones} / {totalBones}");
        Console.WriteLine($"  Execution Time    : {elapsedMs} ms");
        Console.WriteLine($"  Output Directory  : {AppDomain.CurrentDomain.BaseDirectory}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("==========================================================================");
        Console.ResetColor();

        var missing = offsets.Concat(bones).Where(r => !r.IsFound).ToList();
        if (missing.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[!] {missing.Count} targets were not found in dump:");
            foreach (var item in missing)
            {
                Console.WriteLine($"    - {item.Target.ClassName}::{item.Target.FieldName} ({item.Target.ResultName})");
            }
            Console.ResetColor();
        }
    }

    static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[SUCCESS] {message}");
        Console.ResetColor();
    }

    static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {message}");
        Console.ResetColor();
    }

    static int CountChar(string str, char ch)
    {
        int count = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ch) count++;
        }
        return count;
    }
    #endregion
}

#region Data Models
class ExtractionTarget
{
    public string ClassName { get; }
    public string FieldName { get; }
    public string ResultName { get; }
    public string GroupName { get; }

    public ExtractionTarget(string className, string fieldName, string resultName, string groupName = "General")
    {
        ClassName = className;
        FieldName = fieldName;
        ResultName = resultName;
        GroupName = groupName;
    }
}

class ExtractionResult
{
    public ExtractionTarget Target { get; }
    public string OffsetHex { get; }
    public int LineNumber { get; }
    public bool IsFound { get; }

    public ExtractionResult(ExtractionTarget target, string offsetHex, int lineNumber, bool isFound)
    {
        Target = target;
        OffsetHex = offsetHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? offsetHex : "0x" + offsetHex;
        LineNumber = lineNumber;
        IsFound = isFound;
    }
}
#endregion