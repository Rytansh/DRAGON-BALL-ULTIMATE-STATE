using UnityEngine;

public static class SkillDefinitionValidator
{
    public static void Validate(SkillDefinition definition, ValidationContext validator)
    {
        ValidationTools.RequireNotNullOrEmpty(validator, definition.ID, "ID");
        ValidationTools.RequireNotNullOrEmpty(validator, definition.Name, "Name");

        ValidationTools.RequireEnumDefined(validator, definition.Rarity, "Rarity");
        ValidationTools.RequireEnumDefined(validator, definition.Speciality, "Speciality");

        ValidationTools.RequirePositive(validator, definition.Duration, "Duration");
        
        ValidationTools.RequireNonNegative(validator, definition.SkillBaseStats.HP, "Skill HP");
        ValidationTools.RequireNonNegative(validator, definition.SkillBaseStats.ATK, "Skill ATK");
        ValidationTools.RequireNonNegative(validator, definition.SkillBaseStats.DEF, "Skill DEF");
    }
}
