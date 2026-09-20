using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Console.Title = "Null Offsets Dumper";
        Ui.Init();
        Ui.WriteHeader();

        string filePath = FindDumpFile();
        if (string.IsNullOrEmpty(filePath))
        {
            Ui.Error("No dump file selected. Exiting...");
            Ui.Pause();
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();

            Ui.Section("Reading dump file");
            Ui.Info($"File: {filePath}");

            string[] allLines = File.ReadAllLines(filePath);
            Ui.Success($"Loaded {allLines.Length:N0} lines into memory.");

            // ----------------------------------------------------
            // TARGET DEFINITIONS (Original Order Preserved)
            // ----------------------------------------------------
            var internalTargets = new List<ExtractionTarget>
            {
                // General
                new ExtractionTarget("MatchGame", "m_Match", "CurrentMatch", "General"),
                new ExtractionTarget("JMAGGLCNGIG", "LOEPAKMFNJO", "MatchStatus", "General"),
                new ExtractionTarget("JMAGGLCNGIG", "MHJCLOPOBAA", "LocalPlayer", "General"),
                new ExtractionTarget("MatchGame", "m_ReplicationEntitis", "DictionaryEntities", "General"),

                // Player
                new ExtractionTarget("AttackableEntity", "CGJLEBOGBNP", "Player_IsDead", "Player"),
                new ExtractionTarget("Player", "OriginalNickName", "Player_Name", "Player"),
                new ExtractionTarget("ReplicationEntity", "m_PRIDataPool", "Player_Data", "Player"),
                new ExtractionTarget("PlayerNetwork", "m_ShadowState", "Player_ShadowBase", "Player"),
                new ExtractionTarget("PlayerNetwork", "JDFKCAAFFKN", "XPose", "Player"),
                new ExtractionTarget("Player", "IODLOCEJIOK", "LocalPlayerAttributes", "Player"),

                // Avatar
                new ExtractionTarget("Player", "GGJOLDEEIEN", "AvatarManager", "Avatar"),
                new ExtractionTarget("AvatarManager", "LHEDBMJJOLM", "Avatar", "Avatar"),
                new ExtractionTarget("UmaAvatarSimple", "IsVisible", "Avatar_IsVisible", "Avatar"),
                new ExtractionTarget("UMAAvatarBase", "umaData", "Avatar_Data", "Avatar"),
                new ExtractionTarget("UMAData", "isTeammate", "Avatar_Data_IsTeam", "Avatar"),
                new ExtractionTarget("PlayerNetwork", "GFBBIPJFPAD", "BaseProfileInfo", "Avatar"),

                // Camera
                new ExtractionTarget("Player", "AHNCPOJPPCL", "FollowCamera", "Camera"),
                new ExtractionTarget("CameraControllerBase", "AFMGCAELKBD", "Camera", "Camera"),
                new ExtractionTarget("Player", "MainCameraTransform", "MainCameraTransform", "Camera"),
                new ExtractionTarget("Player", "<AKKPEKIHGAH>k__BackingField", "AimRotation", "Camera"),
                new ExtractionTarget("BaseRuntimePanel", "panelToWorld", "ViewMatrix", "Camera"),
                new ExtractionTarget("FollowCamera", "FOVOffset", "FovIncrease", "Camera"),

                // Observer
                new ExtractionTarget("JMAGGLCNGIG", "DGDPMNMOAFP", "CurrentObserver", "Observer"),
                new ExtractionTarget("NGOJGBFHDIM", "EPLJDLHLHCH", "ObserverPlayer", "Observer"),

                // Weapon
                new ExtractionTarget("Player", "ActiveUISightingWeapon", "Weapon", "Weapon"),
                new ExtractionTarget("HBIBDMMOOOK", "<BDKDBILOMDE>k__BackingField", "WeaponData", "Weapon"),
                new ExtractionTarget("MBJCPLOAJJA", "IHCKHAFILED", "WeaponRecoil", "Weapon"),
                new ExtractionTarget("Player", "BKEPAKCAGNJ", "InventoryManager", "Weapon"),
                new ExtractionTarget("GMOCOOEIFMK", "LMFNCLKHIIO", "WeaponOnHand", "Weapon"),
                new ExtractionTarget("HBIBDMMOOOK", "PIILKNEHNLB", "WeaponInfo", "Weapon"),
                new ExtractionTarget("IOKEALMHBKN", "KDCCLEKOOAJ", "WeaponID", "Weapon"),
                new ExtractionTarget("IOKEALMHBKN", "KEGNGBDFACA", "WeaponType", "Weapon"),
                new ExtractionTarget("PlayerAttributes", "BuffWeaponAmmoClip", "BuffWeaponAmmoClip", "Weapon"),
                new ExtractionTarget("PlayerAttributes", "NOPODIILEKG", "Rapidfire", "Weapon"),

                // Silent Aim
                new ExtractionTarget("Player", "<AHDLLHMLNFI>k__BackingField", "sAim1", "Silent Aim"),
                new ExtractionTarget("Player", "FDMIEDDNCEC", "sAim2", "Silent Aim"),
                new ExtractionTarget("CGKJLKPMGDJ", "EKPLMDDKOGB", "sAim3", "Silent Aim"),
                new ExtractionTarget("CGKJLKPMGDJ", "HLDECMJFKJK", "sAim4", "Silent Aim"),

                // Aimbot
                new ExtractionTarget("Player", "BANPEGJEAKE", "AimbotVisible", "Aimbot"),
                new ExtractionTarget("AttackableEntity", "<AONPKGBGHHI>k__BackingField", "HeadCollider", "Aimbot")
            };

            // Original Bone Order
            var boneTargets = new List<ExtractionTarget>
            {
                new ExtractionTarget("Player", "GBKFHDFCPMD", "Head", "Bones"),
                new ExtractionTarget("Player", "FFFPCADFFGA", "Breast", "Bones"),
                new ExtractionTarget("Player", "COBFGNOIPMF", "Hip", "Bones"),
                new ExtractionTarget("Player", "IIPBIDIBJDK", "Root", "Bones"),

                new ExtractionTarget("Player", "CLCEEJMGBGE", "RightBiceps", "Bones"),
                new ExtractionTarget("Player", "JLAECGNPCHC", "LeftBiceps", "Bones"),

                new ExtractionTarget("Player", "IMLDFGHNLBP", "RightWristJoint", "Bones"),
                new ExtractionTarget("Player", "DJAOGHBOBOJ", "LeftWristJoint", "Bones"),

                new ExtractionTarget("Player", "FFDPGNIKAEK", "RightShoulder", "Bones"),
                new ExtractionTarget("Player", "FOELKAHABCD", "LeftShoulder", "Bones"),

                new ExtractionTarget("Player", "GMJLIMFHMAE", "RightFoot", "Bones"),
                new ExtractionTarget("Player", "JMHBBGIKBNO", "LeftFoot", "Bones"),

                new ExtractionTarget("Player", "GPIBDOCMHBE", "RightLeg", "Bones"),
                new ExtractionTarget("Player", "EMNIJJCEKDD", "LeftLeg", "Bones")
            };

            // ----------------------------------------------------
            // EXTRACTION EXECUTION
            // ----------------------------------------------------
            Ui.Section("Extracting internal offsets");
            var extractedOffsets = ExtractTargets(allLines, internalTargets);
            string offsetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offsets.txt");
            WriteOffsetsFile(extractedOffsets, offsetsPath);
            Ui.Success($"Dumped {extractedOffsets.Count(r => r.IsFound)}/{extractedOffsets.Count} offsets to: {offsetsPath}");

            Ui.Section("Extracting bones");
            var extractedBones = ExtractTargets(allLines, boneTargets);
            string bonesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bones.txt");
            WriteBonesFile(extractedBones, bonesPath);
            Ui.Success($"Dumped {extractedBones.Count(r => r.IsFound)}/{extractedBones.Count} bones to: {bonesPath}");

            stopwatch.Stop();

            // ----------------------------------------------------
            // FINAL SUMMARY
            // ----------------------------------------------------
            DisplaySummary(extractedOffsets, extractedBones, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            Ui.Error($"EXTRACTION FAILED: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Ui.Pause();
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

                            Ui.Found(target.ResultName, target.FieldName, hex, lineNumber);
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

                Ui.Missing(target.ClassName, target.FieldName, target.ResultName);
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
            writer.WriteLine("        internal static uint InitBase = 0xA342EFC;");
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
                Ui.Success($"Found dump.cs automatically at: {path}");
                return path;
            }
        }

        Ui.Warning("dump.cs not found in local directory. Opening file picker dialog...");

        using (var dialog = new OpenFileDialog
        {
            Title = "Select IL2CPP Dump File (dump.cs)",
            Filter = "C# Dump Files (*.cs)|*.cs|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            Multiselect = false
        })
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Ui.Info($"Selected: {dialog.FileName}");
                return dialog.FileName;
            }
        }

        return null;
    }

    static void DisplaySummary(List<ExtractionResult> offsets, List<ExtractionResult> bones, long elapsedMs)
    {
        int totalOffsets = offsets.Count;
        int foundOffsets = offsets.Count(o => o.IsFound);
        int totalBones = bones.Count;
        int foundBones = bones.Count(b => b.IsFound);

        int offsetPercent = totalOffsets == 0 ? 0 : (foundOffsets * 100 / totalOffsets);
        int bonePercent = totalBones == 0 ? 0 : (foundBones * 100 / totalBones);

        Ui.Section("Final summary");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  Offsets extracted : {foundOffsets,3} / {totalOffsets,-3}  ({offsetPercent}%)");
        Console.WriteLine($"  Bones extracted   : {foundBones,3} / {totalBones,-3}  ({bonePercent}%)");
        Console.WriteLine($"  Execution time    : {elapsedMs:N0} ms");
        Console.WriteLine($"  Output directory  : {AppDomain.CurrentDomain.BaseDirectory}");
        Console.ResetColor();

        var missing = offsets.Concat(bones).Where(r => !r.IsFound).ToList();
        if (missing.Any())
        {
            Ui.Section("Missing targets");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var item in missing)
            {
                Console.WriteLine($"  - {item.Target.ClassName}::{item.Target.FieldName} ({item.Target.ResultName})");
            }
            Console.ResetColor();
        }
        else
        {
            Ui.Success("All requested targets were found.");
        }
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

#region UI Helper
static class Ui
{
    public static void Init()
    {
        try { Console.OutputEncoding = Encoding.UTF8; } catch { }
        Console.CursorVisible = true;
    }

    public static void WriteHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                          Null Offsets Dumper                             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void Section(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"┌─ {title} " + new string('─', Math.Max(0, 70 - title.Length)));
        Console.ResetColor();
    }

    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"  [i] {message}");
        Console.ResetColor();
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [✓] {message}");
        Console.ResetColor();
    }

    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  [!] {message}");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [x] {message}");
        Console.ResetColor();
    }

    public static void Found(string resultName, string fieldName, string hex, int lineNumber)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  ✓ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{resultName,-24}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($" {fieldName,-28}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($" {hex,-10}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($" line {lineNumber:N0}");
        Console.ResetColor();
    }

    public static void Missing(string className, string fieldName, string resultName)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("  ✗ ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{className}::{fieldName} ({resultName})");
        Console.ResetColor();
    }

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n[Press any key to exit...]");
        Console.ResetColor();
        Console.ReadKey();
    }
}
#endregion

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