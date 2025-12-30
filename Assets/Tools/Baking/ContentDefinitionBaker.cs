using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using UnityEngine;
using System.Collections.Generic;

public class ContentDefinitionBaker : Baker<ContentDefinitionAuthoring>
{
    public override void Bake(ContentDefinitionAuthoring authoring)
    {

        List<CharacterDefinition> validCharacterDefinitions = CharacterDefinitionParser.FilterValidCharacterDefinitions(authoring.CharacterDefinitions);
        List<SkillDefinition> validSkillDefinitions = SkillDefinitionParser.FilterValidSkillDefinitions(authoring.SkillDefinitions);
        Entity ContentRegistryEntity = GetEntity(TransformUsageFlags.None);

        using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
        {
            ref ContentBlobRegistry root = ref builder.ConstructRoot<ContentBlobRegistry>();
            BlobBuilderArray<CharacterDefinitionBlob> charactersToBake = builder.Allocate(ref root.Characters, validCharacterDefinitions.Count);
            BlobBuilderArray<SkillDefinitionBlob> skillsToBake = builder.Allocate(ref root.Skills, validSkillDefinitions.Count);

            BakeAllCharacters(validCharacterDefinitions, ref charactersToBake);
            BakeAllSkills(validSkillDefinitions, ref skillsToBake);

            BlobAssetReference<ContentBlobRegistry> registryReference = builder.CreateBlobAssetReference<ContentBlobRegistry>(Allocator.Persistent);
            AddBlobAsset(ref registryReference, out Unity.Entities.Hash128 blobHash);
            AddComponent(ContentRegistryEntity, new ContentBlobRegistryComponent { BlobRegistryReference = registryReference });
        }
    }
    private static void BakeAllCharacters(List<CharacterDefinition> characterDefs, ref BlobBuilderArray<CharacterDefinitionBlob> outputArray)
    {
        for (int i = 0; i < characterDefs.Count; i++)
        {
            WriteCharacter(ref outputArray[i], characterDefs[i]);
            Logging.System("Baked character " + characterDefs[i].Name + " successfully.");
        }
    }

    private static void BakeAllSkills(List<SkillDefinition> skillDefs, ref BlobBuilderArray<SkillDefinitionBlob> outputArray)
    {
        for (int i = 0; i < skillDefs.Count; i++)
        {
            WriteSkill(ref outputArray[i], skillDefs[i]);
            Logging.System("Baked skill " + skillDefs[i].Name + " successfully.");
        }
    }

    //########### Writers ###########//
    private static void WriteCharacter(ref CharacterDefinitionBlob blob, CharacterDefinition def)
    {
        blob.ID = StableHash32.HashFromString(def.ID);
        blob.Rarity = (byte)def.Rarity;
        blob.BattleType = (byte)def.BattleType;
        blob.CharacterType = (byte)def.CharacterType;
        blob.Speciality = (byte)def.Speciality;

        ref var stats = ref blob.CharacterBlobBaseStats;

        stats.HP = def.CharacterBaseStats.HP;
        stats.ATK = def.CharacterBaseStats.ATK;
        stats.DEF = def.CharacterBaseStats.DEF;
        stats.CritDMG = def.CharacterBaseStats.CritDMG;
        stats.CritRATE = def.CharacterBaseStats.CritRATE;
        stats.Reactivity = def.CharacterBaseStats.Reactivity;
        stats.SustainPOWER = def.CharacterBaseStats.SustainPOWER;
        stats.MagicalDMGBonus = def.CharacterBaseStats.MagicalDMGBonus;
        stats.PowerfulDMGBonus = def.CharacterBaseStats.PowerfulDMGBonus;
        stats.TacticalDMGBonus = def.CharacterBaseStats.TacticalDMGBonus;
        stats.FlexibleDMGBonus = def.CharacterBaseStats.FlexibleDMGBonus;
        stats.DOTDMGBonus = def.CharacterBaseStats.DOTDMGBonus;
        stats.ALLDMGBonus = def.CharacterBaseStats.ALLDMGBonus;

        blob.NormalAttackID = HashOrZero(def.NormalAttackID);
        blob.SuperchargedAttackID = HashOrZero(def.SuperchargedAttackID);
        blob.FinalTrumpSkillID = HashOrZero(def.FinalTrumpSkillID);
    }

    private static void WriteSkill(ref SkillDefinitionBlob blob, SkillDefinition def)
    {
        blob.ID = StableHash32.HashFromString(def.ID);
        blob.Rarity = (byte)def.Rarity;
        blob.Speciality = (byte)def.Speciality;

        blob.Duration = def.Duration;

        blob.SkillBlobBaseStats.ATK = def.SkillBaseStats.ATK;
        blob.SkillBlobBaseStats.DEF = def.SkillBaseStats.DEF;
        blob.SkillBlobBaseStats.HP  = def.SkillBaseStats.HP;

        blob.NormalAbilityID = HashOrZero(def.NormalAbilityID);
        blob.DelayAndImprovementAbilityID = HashOrZero(def.DelayAndImprovementAbilityID);
    }

    private static uint HashOrZero(string id)
    {
        return string.IsNullOrEmpty(id) ? 0u : StableHash32.HashFromString(id);
    }

}
