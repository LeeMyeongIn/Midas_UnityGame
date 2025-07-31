using System.IO;
using UnityEngine;

public static class CodexSaveManager
{
    private const string SaveFileName = "codex.json";

    public static void Save(CodexSaveData data)
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
        Debug.Log($"[CodexSaveManager] 저장 완료: {path}");
    }

    public static CodexSaveData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<CodexSaveData>(json);
        }

        Debug.Log("[CodexSaveManager] 저장 파일 없음. 새 데이터 생성");
        return new CodexSaveData();
    }
}
