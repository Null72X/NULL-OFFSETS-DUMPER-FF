using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Console.Title = "NULL OFFSET EXTRACTOR";

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n==========================================================================");
        Console.WriteLine("                       NULL X OFFSET EXTRACTOR");
        Console.WriteLine("                       SELECT EXTRACTION METHOD");
        Console.WriteLine("==========================================================================");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  [1] Aimkill Offsets Extractor");
        Console.WriteLine("  [2] Internal Offsets & Bones Extractor");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("==========================================================================");
        Console.Write("Enter your choice (1 or 2): ");

        string choice = Console.ReadLine();
        if (choice != "1" && choice != "2")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[!] Invalid Selection. Exiting...");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        string filePath = FindDumpFile();
        if (string.IsNullOrEmpty(filePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[!] No dump file selected. Exiting...");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        try
        {
            string[] allLines = File.ReadAllLines(filePath);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n[+] File Selected: {Path.GetFileName(filePath)}");
            Console.ResetColor();

            if (choice == "1")
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n[+] Running Aimkill Offsets Dumper...");
                Console.ResetColor();

                var results = ExecuteSilentExtraction(allLines);
                SaveToHeaderFile(results);
                DisplayFinalResults(results);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n[+] Running Internal Offset & Bones Dumper...");
                Console.ResetColor();

                // Option 2 Targets
                var targetsList = new List<(string ClassDef, string Field, string ResultName)>
                {
                    ("MatchGame", "m_Match", "CurrentMatch"),
                    ("NFJPHMKKEBF", "ILGECLEFCCO", "MatchStatus"),
                    ("NFJPHMKKEBF", "FJPEHEGICBO", "LocalPlayer"),
                    ("MatchGame", "m_ReplicationEntitis", "DictionaryEntities"),

                    ("AttackableEntity", "FHMPKFMFEPM", "Player_IsDead"),
                    ("Player", "OIAJCBLDHKP", "Player_Name"),
                    ("ReplicationEntity", "m_PRIDataPool", "Player_Data"),
                    ("PlayerNetwork", "m_ShadowState", "Player_ShadowBase"),
                    ("HHCBNAPCKHF", "ADFIDIPODGK", "XPose"),
                    ("Player", "JKPFFNEMJIF", "PlayerAttributes"),

                    ("Player", "FOGJNGDMJKJ", "AvatarManager"),
                    ("AvatarManager", "EEAGBKBMBLD", "Avatar"),
                    ("UmaAvatarSimple", "IsVisible", "Avatar_IsVisible"),
                    ("UMAAvatarBase", "umaData", "Avatar_Data"),
                    ("UMAData", "isTeammate", "Avatar_Data_IsTeam"),

                    ("Player", "CHDOHNOEBML", "FollowCamera"),
                    ("CameraControllerBase", "FCKFGJMEECI", "Camera"),
                    ("Player", "MainCameraTransform", "MainCameraTransform"),
                    ("Player", "<KCFEHMAIINO>k__BackingField", "AimRotation"),
                    ("BaseRuntimePanel", "panelToWorld", "ViewMatrix"),

                    ("NFJPHMKKEBF", "BGGJJKKKFDC", "CurrentObserver"),
                    ("FNCMBMMKLLI", "NJMDHHGDNPJ", "ObserverPlayer"),

                    ("Player", "ActiveUISightingWeapon", "Weapon"),
                    ("GPBDEDFKJNA", "<NOAOCMKGLAH>k__BackingField", "WeaponData"),
                    ("OACEDDHKLIM", "EFMCDHABKGP", "WeaponRecoil"),
                    ("Player", "COLEAPKGFLK", "InventoryManager"),
                    ("NPCNMJAGIKI", "LFEPIIENLAF", "WeaponOnHand"),
                    ("GPBDEDFKJNA", "LAEMLAPIAFD", "WeaponInfo"),
                    ("CHBEAKBLDPI", "HEONOMOEOLN", "WeaponID"),
                    ("PlayerAttributes", "BuffWeaponAmmoClip", "BuffWeaponAmmoClip"),

                    ("Player", "IsClientBot", "IsClientBot"),
                    ("PlayerAttributes", "DPFCEOKBPPP", "InfinitySkyler"),
                    ("PlayerNetwork", "OJAFLKJINPJ", "BaseProfileInfo"),

                    ("TimeService", "m_DeltaTime", "FixedDeltaTime"),
                    ("TimeService", "m_FixedDeltaTime", "GameTimer"),

                    ("Player", "<LPEIEILIKGC>k__BackingField", "sAim1"),
                    ("Player", "GEGFCFDGGGP", "sAim2"),
                    ("MADMMIICBNN", "BOGOIAMJFDN", "sAim3"),
                    ("MADMMIICBNN", "NHKKHPLFMNG", "sAim4"),

                    ("Player", "HECFNHJKOMN", "AimbotVisible"),
                    ("AttackableEntity", "<INICDNFOFJB>k__BackingField", "HeadCollider")
                };

                var bonesTargetsList = new List<(string ClassDef, string Field, string ResultName)>
                {
                    ("Player", "OLCJOGDHJJJ", "Head"),
                    ("Player", "HCLMADAFLPD", "Breast"),
                    ("Player", "MPJBGDJJJMJ", "Root"),
                    ("Player", "OLJBCONDGLO", "Hip"),

                    ("Player", "JHIBMHEMJOL", "LeftBiceps"),
                    ("Player", "NJDDAPKPILB", "RightBiceps"),

                    ("Player", "FGECMMJKFNC", "LeftWristJoint"),
                    ("Player", "JBACCHNMGNJ", "RightWristJoint"),

                    ("Player", "LIBEIIIAGIK", "LeftShoulder"),
                    ("Player", "HDEPJIBNIIK", "RightShoulder"),

                    ("Player", "FDMBKCKMODA", "LeftFoot"),
                    ("Player", "CKABHDJDMAP", "RightFoot"),

                    ("Player", "BMGCHFGEDDA", "LeftLeg"),
                    ("Player", "AGHJLIMNPJA", "RightLeg")
                };

                Console.WriteLine("\n[+] Extracting Internal Offsets...");
                var extractedOffsets = ExtractOffsets(allLines, targetsList);
                string offsetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offsets.txt");
                WriteOffsets(extractedOffsets, offsetsPath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n[SUCCESS] Dumped {extractedOffsets.Count} offsets to: {offsetsPath}");
                Console.ResetColor();

                Console.WriteLine("\n[+] Extracting Bones...");
                var extractedBones = ExtractOffsets(allLines, bonesTargetsList);
                string bonesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bones.txt");
                WriteBones(extractedBones, bonesPath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n[SUCCESS] Dumped {extractedBones.Count} bones to: {bonesPath}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n========================================================");
                Console.WriteLine("                     FINAL RESULTS");
                Console.WriteLine("========================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"  Offsets Found : {extractedOffsets.Count(o => o.Value != "0")}/{extractedOffsets.Count}");
                Console.WriteLine($"  Bones Found   : {extractedBones.Count(b => b.Value != "0")}/{extractedBones.Count}");
                Console.WriteLine($"  Saved files to: {AppDomain.CurrentDomain.BaseDirectory}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("========================================================");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[+] EXTRACTION FAILED: {ex.Message}");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n\n[Press any key to exit...]");
        Console.ResetColor();
        Console.ReadKey();
    }

    static string FindDumpFile()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string localDump = Path.Combine(currentDir, "dump.cs");

        if (File.Exists(localDump))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Found dump.cs automatically in startup directory.");
            Console.ResetColor();
            return localDump;
        }

        string workingDump = Path.Combine(Directory.GetCurrentDirectory(), "dump.cs");
        if (File.Exists(workingDump))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] Found dump.cs automatically in working directory.");
            Console.ResetColor();
            return workingDump;
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\n[!] dump.cs not found in local directory. Opening file browser...");
        Console.ResetColor();

        using (OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Select IL2CPP Dump.cs",
            Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
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

    #region Option 1 (Aimkill Offsets)
    static Dictionary<string, string> ExecuteSilentExtraction(string[] allLines)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n[+] Extracting Aimkill Offset...");
        Console.ResetColor();

        var results = new Dictionary<string, string>();

        var targets = new Dictionary<string, SearchPattern>
        {
            // RVA OFFSETS (Methods)
            {"get_IsFiringFromPRI", new SearchPattern("public bool get_IsFiringFromPRI()", PatternType.RVA)},
            {"get_IsSighting", new SearchPattern("public bool get_IsSighting()", PatternType.RVA)},
            {"SetResolution", new SearchPattern("public static void SetResolution(int width, int height, FullScreenMode fullscreenMode, RefreshRate preferredRefreshRate)", PatternType.RVA)},
            {"get_isVisible", new SearchPattern("public override bool IsVisible()", PatternType.RVA)},
            {"get_isVisibleMoita", new SearchPattern("public override bool IsStreamerVisible()", PatternType.RVA)},
            {"MyPhsXData", new SearchPattern("public Player.KHDMPGBLNCM get_MyPhsXData()", PatternType.RVA)},
            {"Curent_Match", new SearchPattern("public static NFJPHMKKEBF CurrentMatch()", PatternType.RVA)},
            {"get_IsFiring", new SearchPattern("public bool IsFiring()", PatternType.RVA)},
            {"TransformNode", new SearchPattern("public Transform get_transform()", PatternType.RVA, "TransformNode")},
            {"Current_Local_Player", new SearchPattern("public static Player CurrentLocalPlayer()", PatternType.RVA)},
            {"Camera_main", new SearchPattern("public static Camera get_main()", PatternType.RVA)},
            {"Component_GetTransform", new SearchPattern("public Transform get_transform()", PatternType.RVA, "Component")},
            {"Transform_INTERNAL_GetPosition", new SearchPattern("private void get_position_Injected(out Vector3 ret)", PatternType.RVA)},
            {"WorldToScreenPoint", new SearchPattern("public Vector3 WorldToScreenPoint(Vector3 position)", PatternType.RVA)},
            {"set_aim", new SearchPattern("protected void JPNJCAONHME(Quaternion", PatternType.RVA)},
            {"get_forward", new SearchPattern("public Vector3 get_forward()", PatternType.RVA)},
            {"Transform_SetPosition", new SearchPattern("public void set_position(Vector3 value)", PatternType.RVA)},
            {"GCommon_BitArrayBoolean__op_Implicit", new SearchPattern("public static bool op_Implicit(BitArrayBoolean ba)", PatternType.RVA)},
            {"HeadTF2", new SearchPattern("public virtual Transform GetHeadTF()", PatternType.RVA)},
            {"HipTF2", new SearchPattern("public virtual Transform GetHipTF()", PatternType.RVA)},
            {"GetHp", new SearchPattern("public int get_CurHP()", PatternType.RVA)},
            {"CreateString", new SearchPattern("private string CreateString(sbyte*", PatternType.RVA)},
            {"IsLocalTeammate", new SearchPattern("public virtual bool IsLocalTeammate(bool BMEJMCHBLDA = False)", PatternType.RVA)},
            {"get_IsDieing", new SearchPattern("public bool get_IsDieing()", PatternType.RVA)},
            {"SetStartDamage", new SearchPattern("protected int BLAGCMCGEJG(MADMMIICBNN", PatternType.RVA)},
            {"hook_GetGravity", new SearchPattern("public Player.DFKKGHCFGNM JDNPPHMDCFE()", PatternType.RVA)},
            {"swapweapon", new SearchPattern("public override void SwapWeapon(BMGBKEENCJH FANMJANBFIL, bool GDKLMFLNNGM = False)", PatternType.RVA)},
            {"VehicleIAmIn", new SearchPattern("public bool get_IsPassenger()", PatternType.RVA)},
            {"CurrentGameSimulationTimer11", new SearchPattern("public static TimeService CurrentGameSimulationTimer()", PatternType.RVA)},
            {"GetLocalPlayer", new SearchPattern("private Player GetLocalPlayer()", PatternType.RVA)},
            {"get_NickName", new SearchPattern("public string get_NickName()", PatternType.RVA)},
            {"CurrentUIScene", new SearchPattern("public static UICOWBaseScene CurrentUIScene()", PatternType.RVA)},
            {"AddTeammateHud", new SearchPattern("public void ShowAssistantText(string playerName, string line)", PatternType.RVA)},
            {"get_IsCatapultFalling", new SearchPattern("public bool IsCatapultFalling()", PatternType.RVA)},
            {"OnStopCatapultFalling", new SearchPattern("public void OnStopCatapultFalling()", PatternType.RVA)},
            {"get_HeadCollider", new SearchPattern("public virtual Collider get_HeadCollider()", PatternType.RVA)},
            {"get_gameObject", new SearchPattern("public GameObject get_gameObject()", PatternType.RVA, "Component")},
            {"Physics_Raycast", new SearchPattern("public static bool PLDCHDBCOBF(Vector3 OOFIJHADLNI, Vector3 CHGADBAMMOP, uint ONEDHFJBCMJ, ref MADMMIICBNN JEEIBOEGGPD)", PatternType.RVA)},
            {"spofNick", new SearchPattern("protected void KGLHIDLDMKD(string IMGNBGJDAHG)", PatternType.RVA)},
            {"get_Range1", new SearchPattern("public virtual float JDGGIFMKIKF()", PatternType.RVA)},
            {"WeaponType", new SearchPattern("public uint IDOGDPOPGAI()", PatternType.RVA)},
            {"GetDamage", new SearchPattern("public virtual int MEMAEFCDOFL()", PatternType.RVA)},
            {"GetplayerID", new SearchPattern("public IHAAMHPPLMG get_PlayerID()", PatternType.RVA)},
            {"GetWeaponOnHand", new SearchPattern("public GPBDEDFKJNA GetWeaponOnHand()", PatternType.RVA)},
            {"SwapWeapon", new SearchPattern("public override void SwapWeapon(int POFFNNMOOBM, bool GDKLMFLNNGM = False, List<int> HACDOKBPCHJ)", PatternType.RVA)},
            {"GameFacade_Send", new SearchPattern("public static bool Send(uint messageID, UDPClientMessageBase msg, byte sendOption = 0, bool cacheMsgAnyWay = False)", PatternType.RVA)},
            {"CFFPIACECIG", new SearchPattern("public static uint CFFPIACECIG(IHAAMHPPLMG IDNEFEOPGIF)", PatternType.RVA)},
            {"GKHECDLGAJA", new SearchPattern("private OPILIBBOEAC GKHECDLGAJA(MADMMIICBNN NLEGOPLNDBB)", PatternType.RVA)},
            {"Health", new SearchPattern("private bool System.IConvertible.ToBoolean(IFormatProvider provider)", PatternType.RVA)},

            // FIELD OFFSETS
            {"IsFiring", new SearchPattern("private bool <LPEIEILIKGC>k__BackingField;", PatternType.FieldOffset)},
            {"OIAJCBLDHKP", new SearchPattern("protected string OIAJCBLDHKP;", PatternType.FieldOffset)},
            {"TransformNode_k__BackingField", new SearchPattern("private Transform <transform>k__BackingField;", PatternType.FieldOffset, "TransformNode")},
            {"HeadTF", new SearchPattern("protected ITransformNode OLCJOGDHJJJ;", PatternType.FieldOffset)},
            {"PesTF", new SearchPattern("protected ITransformNode MPJBGDJJJMJ;", PatternType.FieldOffset)},
            {"MainCameraTransform", new SearchPattern("public Transform MainCameraTransform;", PatternType.FieldOffset)},
            {"CurrentMatchGame", new SearchPattern("public static MatchGame CurrentMatchGame;", PatternType.FieldOffset)},
            {"m_Match", new SearchPattern("protected NFJPHMKKEBF m_Match;", PatternType.FieldOffset)},
            {"LICPHHNNPPF", new SearchPattern("protected NFJPHMKKEBF.LICPHHNNPPF ILGECLEFCCO;", PatternType.FieldOffset)},
            {"DictionaryEntities", new SearchPattern("private List<ulong> m_CurrrentMemebers;", PatternType.FieldOffset)},
            {"IPRIDataPool", new SearchPattern("m_PRIDataPool;", PatternType.FieldOffset)},
            {"ReplicationDataPoolUnsafe", new SearchPattern("m_Datas;", PatternType.FieldOffset)},
            {"ReplicationDataUnsafe", new SearchPattern("m_Int8Handlers;", PatternType.FieldOffset)},
            {"KHDMPGBLNCM", new SearchPattern("Player.KHDMPGBLNCM IBHJOIGFAEH;", PatternType.FieldOffset)},
            {"GHGCGGOLKIP", new SearchPattern("<NPEONONOGEO>k__BackingField;", PatternType.FieldOffset)},
            {"FBCAHNCLMDC", new SearchPattern("protected FBCAHNCLMDC BDHNFGPDEBH;", PatternType.FieldOffset)},
            {"JNGKBJICFLK", new SearchPattern("JNGKBJICFLK;", PatternType.FieldOffset)},
            {"GLGDFGIKLJC", new SearchPattern("GLGDFGIKLJC;", PatternType.FieldOffset)},
            {"NNFDFMBDGMO", new SearchPattern("NNFDFMBDGMO;", PatternType.FieldOffset)},
            {"KCFEHMAIINO", new SearchPattern("<KCFEHMAIINO>k__BackingField;", PatternType.FieldOffset)},
            {"AcessClass", new SearchPattern("m_CameraModeManager;", PatternType.FieldOffset)},
            {"LocalPlayer", new SearchPattern("m_UmaDcs;", PatternType.FieldOffset)},
            {"undercam", new SearchPattern("public Transform MainCameraTransform;", PatternType.FieldOffset)},
            {"GameTimer", new SearchPattern("m_GameTimer;", PatternType.FieldOffset)},
            {"FixedDeltaTime", new SearchPattern("m_FixedDeltaTime;", PatternType.FieldOffset)},
        };

        foreach (var target in targets)
        {
            string key = target.Key;
            SearchPattern pattern = target.Value;
            bool found = false;

            string currentClass = "";
            string currentRVA = "";

            for (int i = 0; i < allLines.Length; i++)
            {
                string line = allLines[i];

                if (line.Contains("class ") || line.Contains("struct "))
                {
                    Match classMatch = Regex.Match(line, @"(?:class|struct)\s+([^\s\:]+)", RegexOptions.IgnoreCase);
                    if (classMatch.Success)
                    {
                        currentClass = classMatch.Groups[1].Value;
                    }
                }

                if (line.Contains("// RVA:"))
                {
                    Match rvaMatch = Regex.Match(line, @"RVA:\s*(0x[0-9A-F]+)", RegexOptions.IgnoreCase);
                    if (rvaMatch.Success)
                    {
                        currentRVA = rvaMatch.Groups[1].Value;
                    }
                }

                if (pattern.Type == PatternType.RVA)
                {
                    if (!string.IsNullOrEmpty(currentRVA))
                    {
                        for (int j = 0; j < 3 && (i + j) < allLines.Length; j++)
                        {
                            string targetLine = allLines[i + j];
                            if (targetLine.Contains(pattern.Pattern))
                            {
                                if (string.IsNullOrEmpty(pattern.ClassName) || currentClass == pattern.ClassName)
                                {
                                    results[key] = currentRVA;
                                    found = true;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"[+] FOUND: {key} = {currentRVA}");
                                    Console.ResetColor();
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (pattern.Type == PatternType.FieldOffset)
                {
                    if (line.Contains("// 0x"))
                    {
                        Match offsetMatch = Regex.Match(line, @"//\s*(0x[0-9A-F]+)\b");
                        if (offsetMatch.Success)
                        {
                            string offsetValue = offsetMatch.Groups[1].Value;
                            if (line.Contains(pattern.Pattern.Replace(";", "").Trim()))
                            {
                                if (string.IsNullOrEmpty(pattern.ClassName) || currentClass == pattern.ClassName)
                                {
                                    results[key] = offsetValue;
                                    found = true;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"[+] FOUND: {key} = {offsetValue}");
                                    Console.ResetColor();
                                    break;
                                }
                            }
                        }
                    }
                }

                if (found) break;
            }

            if (!found)
            {
                results[key] = "0x0";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[-] NOT FOUND: {key}");
                Console.ResetColor();
            }

            System.Threading.Thread.Sleep(30);
        }

        return results;
    }

    static void DisplayFinalResults(Dictionary<string, string> results)
    {
        int total = results.Count;
        int found = results.Count(r => r.Value != "0x0");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n========================================================");
        Console.WriteLine("                     FINAL RESULTS");
        Console.WriteLine("========================================================");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  Offsets Found : {found}/{total}");
        Console.WriteLine($"  Saved files to: {AppDomain.CurrentDomain.BaseDirectory}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("========================================================");
        Console.ResetColor();

        var missing = results.Where(r => r.Value == "0x0").ToList();
        if (missing.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[+] Missing {missing.Count} offsets:");
            foreach (var miss in missing)
            {
                Console.WriteLine($"   • {miss.Key}");
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[SUCCESS] All offsets extracted successfully!");
            Console.ResetColor();
        }
    }

    static void SaveToHeaderFile(Dictionary<string, string> results)
    {
        string headerContent = GenerateHeaderContent(results);
        string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aimkill offsets.txt");

        File.WriteAllText(savePath, headerContent);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[SUCCESS] Dumped {results.Count(r => r.Value != "0x0")} offsets to: {savePath}");
        Console.ResetColor();
    }

    static string GenerateHeaderContent(Dictionary<string, string> results)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("=========================================================");
        sb.AppendLine("NULL AIMKILL - OFFSETS EXTRACTOR V2 - OB53");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("=========================================================");
        sb.AppendLine();
        sb.AppendLine("// MAIN OFFSETS");
        sb.AppendLine();
        sb.AppendLine("uintptr_t BaseDamage = 0x6C;");

        foreach (var kvp in results)
        {
            sb.AppendLine($"uintptr_t {kvp.Key} = {kvp.Value};");
        }

        return sb.ToString();
    }
    #endregion

    #region Option 2 (Gaurav's Internal Offsets & Bones)
    static string ExtractOffset(string line, string field)
    {
        int fieldPos = line.IndexOf(field);
        if (fieldPos == -1) return "";

        int semicolonPos = line.IndexOf(';', fieldPos);
        if (semicolonPos == -1) return "";

        int hexPos = line.IndexOf("0x", semicolonPos);
        if (hexPos == -1) return "";

        StringBuilder hexValue = new StringBuilder();
        for (int i = hexPos + 2; i < line.Length; i++)
        {
            char c = line[i];
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
            {
                hexValue.Append(c);
            }
            else
            {
                break;
            }
        }
        return hexValue.ToString();
    }

    static List<KeyValuePair<string, string>> ExtractOffsets(string[] lines, List<(string ClassDef, string Field, string ResultName)> targets)
    {
        var extracted = new List<KeyValuePair<string, string>>();

        foreach (var target in targets)
        {
            string classDef = target.ClassDef;
            string field = target.Field;
            string resultName = target.ResultName;

            bool insideClass = false;
            int braceCount = 0;
            bool found = false;

            foreach (var currentLine in lines)
            {
                string trimmedLine = currentLine.Trim();

                if (!insideClass && trimmedLine.Contains("class " + classDef))
                {
                    insideClass = true;
                    braceCount = CountOccurrences(currentLine, '{') - CountOccurrences(currentLine, '}');
                    continue;
                }

                if (insideClass)
                {
                    braceCount += CountOccurrences(currentLine, '{') - CountOccurrences(currentLine, '}');

                    if (braceCount <= 0)
                    {
                        insideClass = false;
                        continue;
                    }

                    string offset = ExtractOffset(currentLine, field);
                    if (!string.IsNullOrEmpty(offset))
                    {
                        extracted.Add(new KeyValuePair<string, string>(resultName, offset));
                        found = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[+] FOUND: {resultName} = 0x{offset}");
                        Console.ResetColor();
                        break;
                    }
                }
            }

            if (!found)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[-] NOT FOUND: {classDef}::{field}");
                Console.ResetColor();
                extracted.Add(new KeyValuePair<string, string>(resultName, "0"));
            }
        }

        return extracted;
    }

    static void WriteOffsets(List<KeyValuePair<string, string>> extracted, string outputFile)
    {
        using (StreamWriter file = new StreamWriter(outputFile))
        {
            file.WriteLine("namespace Client");
            file.WriteLine("{");
            file.WriteLine("    internal static class Offsets");
            file.WriteLine("    {");
            file.WriteLine("        // Core");
            file.WriteLine("        internal static uint Il2Cpp;");
            file.WriteLine("        internal static uint InitBase = 0x9EC1C48;");
            file.WriteLine("        internal static uint StaticClass = 0x5C;\n");

            var groups = new List<(string GroupName, string[] Fields)>
            {
                ("General", new[] { "CurrentMatch", "MatchStatus", "LocalPlayer", "DictionaryEntities" }),
                ("Player", new[] { "Player_IsDead", "Player_Name", "Player_Data", "Player_ShadowBase", "XPose", "PlayerAttributes" }),
                ("Avatar", new[] { "AvatarManager", "Avatar", "Avatar_IsVisible", "Avatar_Data", "Avatar_Data_IsTeam" }),
                ("Camera", new[] { "FollowCamera", "Camera", "MainCameraTransform", "AimRotation", "ViewMatrix" }),
                ("Weapon", new[] { "Weapon", "WeaponData", "WeaponRecoil", "InventoryManager", "WeaponOnHand", "WeaponInfo", "WeaponID", "BuffWeaponAmmoClip" }),
                ("Extra", new[] { "IsClientBot", "InfinitySkyler", "BaseProfileInfo" }),
                ("Observer", new[] { "CurrentObserver", "ObserverPlayer" }),
                ("Speed Internal", new[] { "FixedDeltaTime", "GameTimer" }),
                ("Silent Aim", new[] { "sAim1", "sAim2", "sAim3", "sAim4" }),
                ("Aimbot", new[] { "AimbotVisible", "HeadCollider" })
            };

            var offsetMap = extracted.ToDictionary(x => x.Key, x => x.Value);

            foreach (var group in groups)
            {
                file.WriteLine($"        // {group.GroupName}");
                foreach (var name in group.Fields)
                {
                    if (offsetMap.TryGetValue(name, out string val))
                    {
                        file.WriteLine($"        internal static uint {name} = 0x{val};");
                    }
                }
                file.WriteLine();
            }

            file.WriteLine("    }");
            file.WriteLine("}");
        }
    }

    static void WriteBones(List<KeyValuePair<string, string>> extracted, string outputFile)
    {
        using (StreamWriter file = new StreamWriter(outputFile))
        {
            file.WriteLine("namespace Client");
            file.WriteLine("{");
            file.WriteLine("    internal enum Bones : uint");
            file.WriteLine("    {");

            for (int i = 0; i < extracted.Count; i++)
            {
                file.Write($"        {extracted[i].Key} = 0x{extracted[i].Value}");
                if (i + 1 < extracted.Count)
                    file.Write(",");
                file.WriteLine();
            }

            file.WriteLine("    }");
            file.WriteLine("}");
        }
    }

    static int CountOccurrences(string str, char ch)
    {
        int count = 0;
        foreach (char c in str)
        {
            if (c == ch) count++;
        }
        return count;
    }
    #endregion
}

enum PatternType { RVA, FieldOffset }

class SearchPattern
{
    public string Pattern { get; set; }
    public PatternType Type { get; set; }
    public string ClassName { get; set; }

    public SearchPattern(string pattern, PatternType type, string className = null)
    {
        Pattern = pattern;
        Type = type;
        ClassName = className;
    }
}
