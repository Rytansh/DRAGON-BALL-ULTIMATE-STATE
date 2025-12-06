using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using UnityEngine;

public class SkillDefinitionBaker : Baker<SkillDefinitionAuthoring>
{
    public override void Bake(SkillDefinitionAuthoring authoring)
    {
        if (authoring.SkillDefinitions == null || authoring.SkillDefinitions.Length == 0)
            return;

        Logging.System($"[Authoring] Baking Skill Database… ({authoring.SkillDefinitions.Length} skills)");

        Entity dbEntity = GetEntity(TransformUsageFlags.None);

        using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
        {
            ref SkillDatabaseBlob root = ref builder.ConstructRoot<SkillDatabaseBlob>();
            int count = authoring.SkillDefinitions.Length;
            BlobBuilderArray<SkillDefinitionBlob> skillsToBake = builder.Allocate(ref root.Skills, count);

            for (int i = 0; i < count; i++)
            {
                SkillDefinition def = authoring.SkillDefinitions[i];
                if (def == null) continue;

                ref SkillDefinitionBlob blob = ref skillsToBake[i];

                blob.ID = StableHash32.HashFromString(def.ID);
                blob.Rarity = (byte)def.Rarity;
                blob.Speciality = (byte)def.Speciality;

                blob.Duration = def.Duration;

                blob.SkillBlobBaseStats.ATK = def.SkillBaseStats.ATK;
                blob.SkillBlobBaseStats.DEF = def.SkillBaseStats.DEF;
                blob.SkillBlobBaseStats.HP  = def.SkillBaseStats.HP;

                blob.NormalAbilityID =!string.IsNullOrEmpty(def.NormalAbilityID) ? StableHash32.HashFromString(def.NormalAbilityID): 0;
                blob.DelayAndImprovementAbilityID =!string.IsNullOrEmpty(def.DelayAndImprovementAbilityID) ? StableHash32.HashFromString(def.DelayAndImprovementAbilityID): 0;

                Logging.System($"[Authoring] Baked Skill: {def.Name}");
            }

            BlobAssetReference<SkillDatabaseBlob> blobRef = builder.CreateBlobAssetReference<SkillDatabaseBlob>(Allocator.Persistent);
            AddBlobAsset(ref blobRef, out Unity.Entities.Hash128 blobHash);
            AddComponent(dbEntity, new SkillDatabaseComponent { Blob = blobRef });
        }
    }
}
