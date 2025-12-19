using Unity.Entities;
public struct ContentBlobRegistry
{
    public BlobArray<CharacterDefinitionBlob> Characters;
    public BlobArray<SkillDefinitionBlob> Skills;
}

public struct ContentBlobRegistryComponent : IComponentData
{
    public BlobAssetReference<ContentBlobRegistry> BlobRegistryReference;
}

