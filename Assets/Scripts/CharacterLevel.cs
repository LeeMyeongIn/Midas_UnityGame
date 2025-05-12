using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Woodcutting,
    Mining,
    Fighting
}

[Serializable]

public class Skill
{
    public int level = 1;
    public int experience = 0;

    public int NextLevel
    {
        get
        {
            return level * 1000;
        }
    }

    public SkillType skillType;

    public Skill(SkillType skillType)
    {
        level = 1;
        experience = 0;
        this.skillType = skillType;
    }

    public void AddExperience(int experience)
    {
        this.experience += experience;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if(experience >= NextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= NextLevel;
        level += 1;
    }
}

public class CharacterLevel : MonoBehaviour
{
    [SerializeField] Skill woodcutting;
    [SerializeField] Skill mining;
    [SerializeField] Skill fighting;

    private void Start()
    {
        woodcutting = new Skill(SkillType.Woodcutting);
        mining = new Skill(SkillType.Mining);
        fighting = new Skill(SkillType.Fighting);
    }

    public int GetLevel(SkillType skillType)
    {
        Skill s = GetSkill(skillType);

        if(s == null)
        {
            return -1;
        }

        return s.level;
    }

    public void AddExperience(SkillType skillType, int experience)
    {
        Skill s = GetSkill(skillType);
        s.AddExperience(experience);
    }

    public Skill GetSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Woodcutting:
                return woodcutting;
            case SkillType.Mining:
                return mining;
            case SkillType.Fighting:
                return fighting;
        }

        return null;
    }
}
