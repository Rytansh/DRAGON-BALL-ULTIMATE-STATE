using Unity.Entities;

public struct CharacterDefinitionBlob
{
    public uint ID;   
    public byte Rarity;
    public byte BattleType;
    public byte CharacterType;
    public byte Speciality;
    public CharacterBlobBaseStats CharacterBlobBaseStats;

    public uint NormalAttackID;   
    public uint SuperchargedAttackID;
    public uint FinalTrumpSkillID;
}

public struct CharacterBlobBaseStats
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
