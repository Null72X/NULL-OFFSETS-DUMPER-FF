#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <windows.h>
#include <map>
#include <set>
#include <algorithm>
#include <cctype>
#include <filesystem>
#include <commdlg.h>
#pragma comment(lib, "Comdlg32.lib")

class NULL_OFFSET_DUMPER {
private:
    // all the stuff we gona search in dump, class, field, name we call it
    std::vector<std::tuple<std::string, std::string, std::string>> targets = {
        {"MatchGame", "m_Match", "CurrentMatch"},
        {"NFJPHMKKEBF", "ILGECLEFCCO", "MatchStatus"},
        {"NFJPHMKKEBF", "FJPEHEGICBO", "LocalPlayer"},
        {"MatchGame", "m_ReplicationEntitis", "DictionaryEntities"},

        {"AttackableEntity", "FHMPKFMFEPM", "Player_IsDead"},
        {"Player", "OIAJCBLDHKP", "Player_Name"},
        {"ReplicationEntity", "m_PRIDataPool", "Player_Data"},
        {"PlayerNetwork", "m_ShadowState", "Player_ShadowBase"},
        {"PlayerNetwork.HHCBNAPCKHF", "ADFIDIPODGK", "XPose"},
        {"Player", "JKPFFNEMJIF", "PlayerAttributes"},

        {"Player", "FOGJNGDMJKJ", "AvatarManager"},
        {"AvatarManager", "EEAGBKBMBLD", "Avatar"},
        {"UmaAvatarSimple", "IsVisible", "Avatar_IsVisible"},
        {"UMAAvatarBase", "umaData", "Avatar_Data"},
        {"UMAData", "isTeammate", "Avatar_Data_IsTeam"},

        {"Player", "CHDOHNOEBML", "FollowCamera"},
        {"CameraControllerBase", "FCKFGJMEECI", "Camera"},
        {"Player", "MainCameraTransform", "MainCameraTransform"},
        {"Player", "<KCFEHMAIINO>k__BackingField", "AimRotation"},
        {"BaseRuntimePanel", "panelToWorld", "ViewMatrix"},

        {"NFJPHMKKEBF", "BGGJJKKKFDC", "CurrentObserver"},
        {"FNCMBMMKLLI", "NJMDHHGDNPJ", "ObserverPlayer"},

        {"Player", "ActiveUISightingWeapon", "Weapon"},
        {"GPBDEDFKJNA", "<NOAOCMKGLAH>k__BackingField", "WeaponData"},
        {"OACEDDHKLIM", "EFMCDHABKGP", "WeaponRecoil"},
        {"Player", "COLEAPKGFLK", "InventoryManager"},
        {"NPCNMJAGIKI", "LFEPIIENLAF", "WeaponOnHand"},
        {"GPBDEDFKJNA", "LAEMLAPIAFD", "WeaponInfo"},
        {"NPCNMJAGIKI.CHBEAKBLDPI", "HEONOMOEOLN", "WeaponID"},
        {"PlayerAttributes", "BuffWeaponAmmoClip", "BuffWeaponAmmoClip"},

        {"Player", "IsClientBot", "IsClientBot"},
        {"PlayerAttributes", "DPFCEOKBPPP", "InfinitySkyler"},
        {"PlayerNetwork", "OJAFLKJINPJ", "BaseProfileInfo"},

        {"TimeService", "m_DeltaTime", "FixedDeltaTime"},
        {"TimeService", "m_FixedDeltaTime", "GameTimer"},

        {"Player", "<LPEIEILIKGC>k__BackingField", "sAim1"},
        {"Player", "GEGFCFDGGGP", "sAim2"},
        {"MADMMIICBNN", "BOGOIAMJFDN", "sAim3"},
        {"MADMMIICBNN", "NHKKHPLFMNG", "sAim4"},

        {"Player", "HECFNHJKOMN", "AimbotVisible"},
        {"AttackableEntity", "<INICDNFOFJB>k__BackingField", "HeadCollider"}
    };

    std::vector<std::tuple<std::string, std::string, std::string>> bones_targets = {
        {"Player", "OLCJOGDHJJJ", "Head"},
        {"Player", "HCLMADAFLPD", "Breast"},
        {"Player", "MPJBGDJJJMJ", "Root"},
        {"Player", "OLJBCONDGLO", "Hip"},

        {"Player", "JHIBMHEMJOL", "LeftBiceps"},
        {"Player", "NJDDAPKPILB", "RightBiceps"},

        {"Player", "FGECMMJKFNC", "LeftWristJoint"},
        {"Player", "JBACCHNMGNJ", "RightWristJoint"},

        {"Player", "LIBEIIIAGIK", "LeftShoulder"},
        {"Player", "HDEPJIBNIIK", "RightShoulder"},

        {"Player", "FDMBKCKMODA", "LeftFoot"},
        {"Player", "CKABHDJDMAP", "RightFoot"},

        {"Player", "BMGCHFGEDDA", "LeftLeg"},
        {"Player", "AGHJLIMNPJA", "RightLeg"}
    };

public:
    // change console size so text fit nicer
    void setConsoleSize(int columns = 130, int lines = 30) {
        std::string command = "mode con: cols=" + std::to_string(columns) + " lines=" + std::to_string(lines);
        system(command.c_str());
    }

    // type text slow for drama lol
    void typewrite(const std::string& text, int delay_ms = 50) {
        for (char c : text) {
            std::cout << c << std::flush;
            Sleep(delay_ms);
        }
        std::cout << std::endl;
    }

    void printBanner()
    {
        std::cout << R"(

==============================================================
                    NULL OFFSET DUMPER
                         BY GAURAV
==============================================================

)";
    }

    // lowercase helper
    std::string toLower(const std::string& str) {
        std::string result = str;
        for (char& c : result) {
            c = std::tolower(static_cast<unsigned char>(c));
        }
        return result;
    }

    // trim spaces cus dumps messy
    std::string trim(const std::string& str) {
        size_t start = str.find_first_not_of(" \t\n\r");
        size_t end = str.find_last_not_of(" \t\n\r");
        if (start == std::string::npos) return "";
        return str.substr(start, end - start + 1);
    }

    // look in a line for field then find 0x after ; and return hex digits only
    std::string extractOffset(const std::string& line, const std::string& field) {
        size_t field_pos = line.find(field);
        if (field_pos == std::string::npos) {
            return "";
        }

        size_t semicolon_pos = line.find(';', field_pos);
        if (semicolon_pos == std::string::npos) {
            return "";
        }

        size_t hex_pos = line.find("0x", semicolon_pos);
        if (hex_pos == std::string::npos) {
            return "";
        }

        std::string hex_value;
        for (size_t i = hex_pos + 2; i < line.size(); i++) {
            char c = line[i];
            if (isxdigit(static_cast<unsigned char>(c))) {
                hex_value += c;
            }
            else {
                break;
            }
        }

        return hex_value;
    }

    // parse dump.cs and search for offsets using targets vector
    std::vector<std::pair<std::string, std::string>> extractOffsets(const std::string& filePath) {
        std::vector<std::pair<std::string, std::string>> extracted;

        std::ifstream file(filePath);
        if (!file.is_open()) {
            std::cout << "ERROR: Cannot open file: " << filePath << std::endl;
            return extracted;
        }

        std::string line;
        std::vector<std::string> lines;
        while (std::getline(file, line)) {
            lines.push_back(line);
        }
        file.close();

        std::cout << "Reading " << lines.size() << " lines..." << std::endl;

        for (const auto& target : targets) {
            std::string class_def = std::get<0>(target);
            std::string field = std::get<1>(target);
            std::string result_name = std::get<2>(target);

            bool inside_class = false;
            int brace_count = 0;
            bool found = false;

            for (const auto& current_line : lines) {
                std::string trimmed_line = trim(current_line);

                // try find class
                if (!inside_class && trimmed_line.find("class " + class_def) != std::string::npos) {
                    inside_class = true;
                    brace_count = countOccurrences(current_line, '{') - countOccurrences(current_line, '}');
                    continue;
                }

                if (inside_class) {
                    brace_count += countOccurrences(current_line, '{') - countOccurrences(current_line, '}');

                    // left class
                    if (brace_count <= 0) {
                        inside_class = false;
                        continue;
                    }

                    // look for field
                    std::string offset = extractOffset(current_line, field);
                    if (!offset.empty()) {
                        extracted.push_back({ result_name, offset });
                        found = true;
                        std::cout << "FOUND: " << result_name << " = 0x" << offset << std::endl;
                        break;
                    }
                }
            }

            if (!found) {
                std::cout << "NOT FOUND: " << class_def << "::" << field << std::endl;
            }
        }

        return extracted;
    }

    // extract bones using bones_targets
    std::vector<std::pair<std::string, std::string>> extractBones(const std::string& filePath) {
        std::vector<std::pair<std::string, std::string>> extracted;

        std::ifstream file(filePath);
        if (!file.is_open()) {
            return extracted;
        }

        std::string line;
        std::vector<std::string> lines;
        while (std::getline(file, line)) {
            lines.push_back(line);
        }
        file.close();

        for (const auto& target : bones_targets) {
            std::string class_def = std::get<0>(target);
            std::string field = std::get<1>(target);
            std::string result_name = std::get<2>(target);

            bool inside_class = false;
            int brace_count = 0;
            bool found = false;

            for (const auto& current_line : lines) {
                std::string trimmed_line = trim(current_line);

                if (!inside_class && trimmed_line.find("class " + class_def) != std::string::npos) {
                    inside_class = true;
                    brace_count = countOccurrences(current_line, '{') - countOccurrences(current_line, '}');
                    continue;
                }

                if (inside_class) {
                    brace_count += countOccurrences(current_line, '{') - countOccurrences(current_line, '}');

                    if (brace_count <= 0) {
                        inside_class = false;
                        continue;
                    }

                    std::string offset = extractOffset(current_line, field);
                    if (!offset.empty()) {
                        extracted.push_back({ result_name, offset });
                        found = true;
                        std::cout << "FOUND BONE: " << result_name << " = 0x" << offset << std::endl;
                        break;
                    }
                }
            }

            if (!found) {
                std::cout << "BONE NOT FOUND: " << class_def << "::" << field << std::endl;
            }
        }

        return extracted;
    }

    // write offsets to file with groups
    void writeOffsets(const std::vector<std::pair<std::string, std::string>>& extracted, const std::string& outputFile) {
        std::ofstream file(outputFile);
        if (!file.is_open()) {
            std::cout << "ERROR: Cannot create file: " << outputFile << std::endl;
            return;
        }
        file << "namespace Client\n";
        file << "{\n";
        file << "    internal static class Offsets\n";
        file << "    {\n";

        file << "        // Core\n";
        file << "        internal static uint Il2Cpp;\n";
        file << "        internal static uint InitBase = 0x9EC1C48;\n";
        file << "        internal static uint StaticClass = 0x5C;";
        file << "\n";

        std::vector<std::pair<std::string, std::vector<std::string>>> groups =
        {
            {"General", {
                "CurrentMatch",
                "MatchStatus",
                "LocalPlayer",
                "DictionaryEntities"
            }},

            {"Player", {
                "Player_IsDead",
                "Player_Name",
                "Player_Data",
                "Player_ShadowBase",
                "XPose",
                "PlayerAttributes",
            }},

            {"Avatar", {
                "AvatarManager",
                "Avatar",
                "Avatar_IsVisible",
                "Avatar_Data",
                "Avatar_Data_IsTeam",
            }},

            {"Camera", {
                "FollowCamera",
                "Camera",
                "MainCameraTransform",
                "AimRotation",
                "ViewMatrix"
            }},

            {"Weapon", {
                "Weapon",
                "WeaponData",
                "WeaponRecoil",
                "InventoryManager",
                "WeaponOnHand",
                "WeaponInfo",
                "WeaponID",
                "BuffWeaponAmmoClip"
            }},

            {"Extra", {
                "IsClientBot",
                "InfinitySkyler",
                "BaseProfileInfo"
            }},

            {"Observer", {
                "CurrentObserver",
                "ObserverPlayer"
            }},

            {"Speed Internal", {
                "FixedDeltaTime",
                "GameTimer"
            }},

            {"Silent Aim", {
                "sAim1",
                "sAim2",
                "sAim3",
                "sAim4"
            }},

            {"Aimbot", {
                "AimbotVisible",
                "HeadCollider"
            }},

        };

        std::map<std::string, std::string> offsetMap;

        for (const auto& entry : extracted)
        {
            offsetMap[entry.first] = entry.second;
        }

        for (const auto& group : groups)
        {
            file << "        // " << group.first << "\n";

            for (const auto& name : group.second)
            {
                auto it = offsetMap.find(name);

                if (it != offsetMap.end())
                {
                    file << "        internal static uint "
                        << name
                        << " = 0x"
                        << it->second
                        << ";\n";
                }
            }

            file << "\n";
        }

        file << "    }\n";
        file << "}\n";
    }

    void writeBones(const std::vector<std::pair<std::string, std::string>>& extracted, const std::string& outputFile)
    {
        std::ofstream file(outputFile);

            if (!file.is_open())
            {
                std::cout << "ERROR: Cannot create file: " << outputFile << std::endl;
                return;
            }

        file << "namespace Client\n";
        file << "{\n";
        file << "    internal enum Bones : uint\n";
        file << "    {\n";

        for (size_t i = 0; i < extracted.size(); i++)
        {
            file << "        "
                << extracted[i].first
                << " = 0x"
                << extracted[i].second;

            if (i + 1 < extracted.size())
                file << ",";

            file << "\n";
        }

        file << "    }\n";
        file << "}\n";

        file.close();

    }


private:
    // little helper to count braces
    int countOccurrences(const std::string& str, char ch) {
        int count = 0;
        for (char c : str) {
            if (c == ch) count++;
        }
        return count;
    }
};

std::string FindDumpFile()
{
    char exePath[MAX_PATH];
    GetModuleFileNameA(NULL, exePath, MAX_PATH);

    // Remove exe name
    char* lastSlash = strrchr(exePath, '\\');
    if (lastSlash)
        *(lastSlash + 1) = '\0';

    std::string dumpPath = std::string(exePath) + "dump.cs";

    DWORD attrib = GetFileAttributesA(dumpPath.c_str());

    if (attrib != INVALID_FILE_ATTRIBUTES &&
        !(attrib & FILE_ATTRIBUTE_DIRECTORY))
    {
        std::cout << "[SUCCESS] Found dump.cs\n";
        return dumpPath;
    }

    OPENFILENAMEA ofn;
    char fileName[MAX_PATH] = "";

    ZeroMemory(&ofn, sizeof(ofn));

    ofn.lStructSize = sizeof(ofn);
    ofn.hwndOwner = NULL;
    ofn.lpstrFilter =
        "C# Files (*.cs)\0*.cs\0"
        "All Files (*.*)\0*.*\0";
    ofn.lpstrFile = fileName;
    ofn.nMaxFile = MAX_PATH;
    ofn.Flags = OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST;
    ofn.lpstrTitle = "Select dump.cs";

    if (GetOpenFileNameA(&ofn))
        return std::string(fileName);

    return "";
}

int main() {
    NULL_OFFSET_DUMPER dumper;

    // set console big so text not wrap
    dumper.setConsoleSize(130, 30);
    system("title NULL OFFSET DUMPER");

    // banner coz why not
    dumper.printBanner();

    // ask for dump file path, user paste path
    std::string dumpFilePath = FindDumpFile();

    if (dumpFilePath.empty())
    {
        std::cout << "\nNo dump.cs selected.\n";
        std::cout << "Press Enter to exit...";
        std::cin.get();
        return 1;
    }

    // check file exist else bail
    std::ifstream testFile(dumpFilePath);
    if (!testFile.is_open()) {
        std::cout << std::endl;
        std::cout << "ERROR: File not found: " << dumpFilePath << std::endl;
        std::cout << "Please check the path and try again." << std::endl;
        std::cout << std::endl;
        std::cout << "Press Enter to exit...";
        std::cin.get();
        return 1;
    }
    testFile.close();

    std::cout << std::endl;
    std::cout << "SUCCESS: Found file: " << dumpFilePath << std::endl;
    std::cout << std::endl;

    // some silly messages for user
    dumper.typewrite("Welcome to Gaurav X Offset Dumper!");
    Sleep(500);

    dumper.typewrite("Analyzing dump file...");
    Sleep(500);

    std::cout << "Dumping Offsets..." << std::endl;
    std::cout << std::endl;

    // get offsets and write file
    auto extracted = dumper.extractOffsets(dumpFilePath);
    dumper.writeOffsets(extracted, "offsets.txt");

    Sleep(500);
    std::cout << std::endl;
    std::cout << "SUCCESS: Dumped " << extracted.size() << " offsets to 'offsets.txt'" << std::endl;

    std::cout << std::endl;
    std::cout << "Dumping Bones..." << std::endl;
    std::cout << std::endl;

    // bones
    auto extractedBones = dumper.extractBones(dumpFilePath);
    dumper.writeBones(extractedBones, "bones.txt");

    Sleep(500);
    std::cout << std::endl;
    std::cout << "SUCCESS: Dumped " << extractedBones.size() << " bones to 'bones.txt'" << std::endl;

    std::cout << std::endl;
    std::cout << "FINAL RESULTS:" << std::endl;
    std::cout << "==================================" << std::endl;
    std::cout << "Offsets found: " << extracted.size() << std::endl;
    std::cout << "Bones found: " << extractedBones.size() << std::endl;
    std::cout << "Total: " << (extracted.size() + extractedBones.size()) << std::endl;
    std::cout << std::endl;

    std::cout << "Output files created:" << std::endl;
    std::cout << "- offsets.txt" << std::endl;
    std::cout << "- bones.txt" << std::endl;
    std::cout << std::endl;

    std::cout << "Press Enter to exit...";
    std::cin.get();

    return 0;
}
