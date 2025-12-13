using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using UnityEngine;
using System.Collections.Generic;

public class SkillDefinitionBaker : Baker<SkillDefinitionAuthoring>
{
    public override void Bake(SkillDefinitionAuthoring authoring)
    {
        if (authoring.SkillDefinitions == null || authoring.SkillDefinitions.Length == 0)
            return;

        Logging.System($"[Authoring] Baking Skill Database… ({authoring.SkillDefinitions.Length} skills)");

        Entity dbEntity = GetEntity(TransformUsageFlags.None);

        List<SkillDefinition> validDefs = FilterValidDefinitions(authoring);

        using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
        {
            ref SkillDatabaseBlob root = ref builder.ConstructRoot<SkillDatabaseBlob>();
            BlobBuilderArray<SkillDefinitionBlob> skillsToBake = builder.Allocate(ref root.Skills, validDefs.Count);

            for (int i = 0; i < validDefs.Count; i++)
            {
                WriteSkill(ref skillsToBake[i], validDefs[i]);
                Logging.System($"[Authoring] Baked Skill: {validDefs[i].Name}");
            }

            BlobAssetReference<SkillDatabaseBlob> blobRef = builder.CreateBlobAssetReference<SkillDatabaseBlob>(Allocator.Persistent);
            AddBlobAsset(ref blobRef, out Unity.Entities.Hash128 blobHash);
            AddComponent(dbEntity, new SkillDatabaseComponent { Blob = blobRef });
        }
    }

    private static List<SkillDefinition> FilterValidDefinitions(SkillDefinitionAuthoring authoring)
    {
        List<SkillDefinition> validDefs = new List<SkillDefinition>();
        for (int i = 0; i < authoring.SkillDefinitions.Length; i++)
        {
            SkillDefinition def = authoring.SkillDefinitions[i];
            if (def == null)
            {
                continue;
            }

            ValidationContext validator = new ValidationContext(def);
            SkillDefinitionValidator.Validate(def, validator);

            if (validator.HasErrors)
            {
                Logging.Warning(def + " will not be baked due to existing errors in the asset.");
                continue;
            }

            validDefs.Add(def);
        }
        return validDefs;
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
