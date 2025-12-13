using Unity.Entities;

public struct SkillDefinitionBlob
{
    public uint ID;
    public byte Rarity;
    public byte Speciality; 
    public int Duration;

    public SkillBlobBaseStats SkillBlobBaseStats;

    public uint NormalAbilityID;
    public uint DelayAndImprovementAbilityID;
}

public struct SkillBlobBaseStats
{
    public float ATK;
    public float DEF;
    public float HP;
}

public struct SkillDatabaseBlob
{
    public BlobArray<SkillDefinitionBlob> Skills;
}

public struct SkillDatabaseComponent : IComponentData
{
    public BlobAssetReference<SkillDatabaseBlob> Blob;
}
