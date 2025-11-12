using UnityEngine;
using DBUS.Gameplay.Stats;

[CreateAssetMenu(fileName = "SkillDefinition", menuName = "Scriptable Objects/SkillDefinition")]
public class SkillDefinition : ScriptableObject
{
    public string ID;
    public string Name;
    public SkillRarity Rarity;
    public Speciality Speciality; 
    public SkillBaseStats SkillBaseStats;
    public int Duration;
    public string NormalAbilityID;
    public string SpecialisedAbilityID;
    public string SpecificAbilityID;
    public string DelayAndImprovementAbilityID;
}

public struct SkillBaseStats
{
    public float ATK;
    public float DEF;
    public float HP;
}
