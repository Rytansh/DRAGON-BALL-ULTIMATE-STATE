using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using UnityEngine;
using System.Collections.Generic;

public class CharacterDefinitionBaker : Baker<CharacterDefinitionAuthoring>
{
    public override void Bake(CharacterDefinitionAuthoring authoring)
    {
        if (authoring.CharacterDefinitions == null || authoring.CharacterDefinitions.Length == 0)
            return;

        Logging.System($"[Authoring] Baking Character Database… ({authoring.CharacterDefinitions.Length} characters)");

        Entity dbEntity = GetEntity(TransformUsageFlags.None);

        List<CharacterDefinition> validDefs = FilterValidDefinitions(authoring);

        using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
        {
            ref CharacterDatabaseBlob root = ref builder.ConstructRoot<CharacterDatabaseBlob>();
            BlobBuilderArray<CharacterDefinitionBlob> charactersToBake = builder.Allocate(ref root.Characters, validDefs.Count);

            for (int i = 0; i < validDefs.Count; i++)
            {
                WriteCharacter(ref charactersToBake[i], validDefs[i]);
                Logging.System($"[Authoring] Prepared Character: {validDefs[i].Name}");
            }

            BlobAssetReference<CharacterDatabaseBlob> blobRef = builder.CreateBlobAssetReference<CharacterDatabaseBlob>(Allocator.Persistent);
            AddBlobAsset(ref blobRef, out Unity.Entities.Hash128 blobHash);
            AddComponent(dbEntity, new CharacterDatabaseComponent { Blob = blobRef });
        }
    }

    private static List<CharacterDefinition> FilterValidDefinitions(CharacterDefinitionAuthoring authoring)
    {
        List<CharacterDefinition> validDefs = new List<CharacterDefinition>();
        for (int i = 0; i < authoring.CharacterDefinitions.Length; i++)
        {
            CharacterDefinition def = authoring.CharacterDefinitions[i];
            if (def == null)
            {
                continue;
            }

            ValidationContext validator = new ValidationContext(def);
            CharacterDefinitionValidator.Validate(def, validator);

            if (validator.HasErrors)
            {
                Logging.Warning(def + " will not be baked due to existing errors in the asset.");
                continue;
            }

            validDefs.Add(def);
        }
        return validDefs;
    }

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
        stats.PowerLEVEL = def.CharacterBaseStats.PowerLEVEL;
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

    private static uint HashOrZero(string id)
    {
        return string.IsNullOrEmpty(id) ? 0u : StableHash32.HashFromString(id);
    }

}
