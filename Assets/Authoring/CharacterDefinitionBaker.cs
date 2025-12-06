using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using UnityEngine;

public class CharacterDefinitionBaker : Baker<CharacterDefinitionAuthoring>
{
    public override void Bake(CharacterDefinitionAuthoring authoring)
    {
        if (authoring.CharacterDefinitions == null || authoring.CharacterDefinitions.Length == 0)
            return;

        Logging.System($"[Authoring] Baking Character Database… ({authoring.CharacterDefinitions.Length} characters)");

        Entity dbEntity = GetEntity(TransformUsageFlags.None);

        using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
        {
            ref CharacterDatabaseBlob root = ref builder.ConstructRoot<CharacterDatabaseBlob>();
            int count = authoring.CharacterDefinitions.Length;
            BlobBuilderArray<CharacterDefinitionBlob> charactersToBake = builder.Allocate(ref root.Characters, count);

            for (int i = 0; i < count; i++)
            {
                CharacterDefinition def = authoring.CharacterDefinitions[i];
                if (def == null) continue;

                ref CharacterDefinitionBlob blob = ref charactersToBake[i];

                blob.ID = StableHash32.HashFromString(def.ID);
                blob.Rarity = (byte)def.Rarity;
                blob.BattleType = (byte)def.BattleType;
                blob.CharacterType = (byte)def.CharacterType;
                blob.Speciality = (byte)def.Speciality;

                blob.CharacterBlobBaseStats.HP = def.CharacterBaseStats.HP;
                blob.CharacterBlobBaseStats.ATK = def.CharacterBaseStats.ATK;
                blob.CharacterBlobBaseStats.DEF = def.CharacterBaseStats.DEF;
                blob.CharacterBlobBaseStats.CritDMG = def.CharacterBaseStats.CritDMG;
                blob.CharacterBlobBaseStats.CritRATE = def.CharacterBaseStats.CritRATE;
                blob.CharacterBlobBaseStats.Reactivity = def.CharacterBaseStats.Reactivity;
                blob.CharacterBlobBaseStats.PowerLEVEL = def.CharacterBaseStats.PowerLEVEL;
                blob.CharacterBlobBaseStats.SustainPOWER = def.CharacterBaseStats.SustainPOWER;
                blob.CharacterBlobBaseStats.MagicalDMGBonus = def.CharacterBaseStats.MagicalDMGBonus;
                blob.CharacterBlobBaseStats.PowerfulDMGBonus = def.CharacterBaseStats.PowerfulDMGBonus;
                blob.CharacterBlobBaseStats.TacticalDMGBonus = def.CharacterBaseStats.TacticalDMGBonus;
                blob.CharacterBlobBaseStats.FlexibleDMGBonus = def.CharacterBaseStats.FlexibleDMGBonus;
                blob.CharacterBlobBaseStats.DOTDMGBonus = def.CharacterBaseStats.DOTDMGBonus;
                blob.CharacterBlobBaseStats.ALLDMGBonus = def.CharacterBaseStats.ALLDMGBonus;

                blob.NormalAttackID = !string.IsNullOrEmpty(def.NormalAttackID) ? StableHash32.HashFromString(def.NormalAttackID) : 0u;
                blob.SuperchargedAttackID = !string.IsNullOrEmpty(def.SuperchargedAttackID) ? StableHash32.HashFromString(def.SuperchargedAttackID) : 0u;
                blob.FinalTrumpSkillID = !string.IsNullOrEmpty(def.FinalTrumpSkillID) ? StableHash32.HashFromString(def.FinalTrumpSkillID) : 0u;

                Logging.System($"[Authoring] Prepared Character: {def.Name}");
            }

            BlobAssetReference<CharacterDatabaseBlob> blobRef = builder.CreateBlobAssetReference<CharacterDatabaseBlob>(Allocator.Persistent);
            AddBlobAsset(ref blobRef, out Unity.Entities.Hash128 blobHash);
            AddComponent(dbEntity, new CharacterDatabaseComponent { Blob = blobRef });
        }
    }
}
