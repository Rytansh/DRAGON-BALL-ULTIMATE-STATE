using DBUS.Gameplay.Stats;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Card Definition/Character")]
public class CharacterDefinition : ScriptableObject
{
    public string ID;   
    public string Name;  
    public CharacterRarity Rarity;           
    public BattleType BattleType; 
    public CharacterType CharacterType;      
    public Speciality Speciality;               
    public CharacterBaseStats CharacterBaseStats;   
    public string NormalAttackID;
    public string SuperchargedAttackID;  
    public string FinalTrumpSkillID;  
}
[System.Serializable]
public struct CharacterBaseStats
{
    public float HP;
    public float ATK;
    public float DEF;
    public float CritDMG;
    public float CritRATE;
    public float Reactivity;
    public float SustainPOWER;
    public float MagicalDMGBonus;
    public float PowerfulDMGBonus;
    public float TacticalDMGBonus;
    public float FlexibleDMGBonus;
    public float DOTDMGBonus;
    public float ALLDMGBonus;
}


