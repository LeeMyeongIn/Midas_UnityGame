#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EditorPlayerPrefsUtility
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("PlayerPrefs 삭제", "모든 PlayerPrefs를 삭제하시겠습니까? (복구 불가)", "삭제", "취소"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Editor: PlayerPrefs 삭제 완료");
        }
    }
}
#endif