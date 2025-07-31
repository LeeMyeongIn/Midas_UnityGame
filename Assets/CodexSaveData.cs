using System.Collections.Generic;

[System.Serializable]
public class CodexSaveData
{
    public List<string> unlockedRecipeIds = new List<string>();
    public List<string> seenMonsterIds = new List<string>();
    public List<string> seenDropItemIds = new List<string>();
    public List<string> cookedRecipeIds = new List<string>();
}
