using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Analysis
{
    public class ResourceRecommendation
    {
        public string ResourceType { get; set; }
        public string Reason { get; set; }
        public List<string> RelatedDefs { get; set; } = new List<string>();
        public int Priority { get; set; }
        public RecommendationCategory Category { get; set; }
    }

    public enum RecommendationCategory
    {
        DirectlyRelated,
        CommonlyUsedTogether,
        Contextual,
        Suggested
    }

    public class ResourceRecommendationResult : ProbeResult
    {
        public Type SourceType { get; }
        public MethodInfo SourceMethod { get; }
        public List<ResourceRecommendation> Recommendations { get; } = new List<ResourceRecommendation>();

        public ResourceRecommendationResult(Type type)
        {
            SourceType = type;
            SourceMethod = null;
            Id = type.FullName;
            Name = type.Name;
            Type = "ResourceRecommendation";
            Source = type.Assembly.GetName().Name;
            Location = type.FullName;
        }

        public ResourceRecommendationResult(MethodInfo method)
        {
            SourceType = method.DeclaringType;
            SourceMethod = method;
            Id = $"{method.DeclaringType.FullName}.{method.Name}";
            Name = method.Name;
            Type = "ResourceRecommendation";
            Source = method.DeclaringType.Assembly.GetName().Name;
            Location = method.DeclaringType.FullName;
        }

        public override void PrintDetails()
        {
            if (SourceMethod != null)
            {
                Console.WriteLine($"\n=== Resource Recommendations for Method: {SourceMethod.Name} ===");
                Console.WriteLine($"Declaring Type: {SourceMethod.DeclaringType.FullName}");
                var parameters = string.Join(", ", SourceMethod.GetParameters().Select(p => p.ParameterType.Name));
                Console.WriteLine($"Signature: {SourceMethod.ReturnType.Name} {SourceMethod.Name}({parameters})");
            }
            else
            {
                Console.WriteLine($"\n=== Resource Recommendations for Type: {SourceType.Name} ===");
                Console.WriteLine($"Full Name: {SourceType.FullName}");
                Console.WriteLine($"Assembly: {SourceType.Assembly.GetName().Name}");
            }

            var groupedRecommendations = Recommendations.GroupBy(r => r.Category);
            foreach (var group in groupedRecommendations)
            {
                Console.WriteLine($"\n--- {group.Key} ({group.Count()}) ---");
                foreach (var rec in group.OrderByDescending(r => r.Priority))
                {
                    Console.WriteLine($"\n  [{rec.ResourceType}] (Priority: {rec.Priority})");
                    Console.WriteLine($"    Reason: {rec.Reason}");
                    if (rec.RelatedDefs.Count > 0)
                    {
                        Console.WriteLine($"    Related Defs ({rec.RelatedDefs.Count}):");
                        foreach (var def in rec.RelatedDefs.Take(10))
                        {
                            Console.WriteLine($"      - {def}");
                        }
                        if (rec.RelatedDefs.Count > 10)
                        {
                            Console.WriteLine($"      ... and {rec.RelatedDefs.Count - 10} more");
                        }
                    }
                }
            }
        }
    }

    public class ResourceRecommender
    {
        private readonly ProbeContext _context;
        private readonly DefsProbe _defsProbe;
        private readonly TypeDefMapper _typeDefMapper;
        private readonly Dictionary<string, List<TypeAssociation>> _typeAssociations;

        public ResourceRecommender(ProbeContext context, DefsProbe defsProbe, TypeDefMapper typeDefMapper)
        {
            _context = context;
            _defsProbe = defsProbe;
            _typeDefMapper = typeDefMapper;
            _typeAssociations = BuildTypeAssociations();
        }

        private Dictionary<string, List<TypeAssociation>> BuildTypeAssociations()
        {
            var associations = new Dictionary<string, List<TypeAssociation>>(StringComparer.OrdinalIgnoreCase);

            AddAssociation(associations, "Pawn", "SoundDef", "Pawn-related sound effects", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "Pawn", "JobDef", "Jobs that pawns can perform", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "Pawn", "HediffDef", "Health conditions and injuries", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "Pawn", "NeedDef", "Pawn needs and motivations", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "Pawn", "SkillDef", "Pawn skills and abilities", RecommendationCategory.DirectlyRelated, 8);
            AddAssociation(associations, "Pawn", "ThoughtDef", "Pawn thoughts and moods", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "Pawn", "TraitDef", "Pawn traits and personalities", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "Pawn", "BackstoryDef", "Pawn backstories", RecommendationCategory.CommonlyUsedTogether, 7);
            AddAssociation(associations, "Pawn", "MentalStateDef", "Mental break states", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "Building", "SoundDef", "Building sound effects", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "Building", "TerrainDef", "Terrain interactions", RecommendationCategory.DirectlyRelated, 8);
            AddAssociation(associations, "Building", "StuffDef", "Material properties", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "Building", "RecipeDef", "Building recipes and bills", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "Building", "WorkGiverDef", "Work assignments for buildings", RecommendationCategory.CommonlyUsedTogether, 7);
            AddAssociation(associations, "Building", "DesignationCategoryDef", "UI categorization", RecommendationCategory.Contextual, 6);

            AddAssociation(associations, "ThingDef", "SoundDef", "Thing-related sounds", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "ThingDef", "StuffDef", "Material properties", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "ThingDef", "TerrainDef", "Terrain interactions", RecommendationCategory.CommonlyUsedTogether, 7);
            AddAssociation(associations, "ThingDef", "RecipeDef", "Related recipes", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "Weapon", "SoundDef", "Weapon sound effects", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "Weapon", "ProjectileDef", "Projectile definitions", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "Weapon", "DamageDef", "Damage types", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "Weapon", "MoteDef", "Visual effects", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "ResearchProjectDef", "ResearchTabDef", "Research UI tab", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "ResearchProjectDef", "ThingDef", "Unlocked items", RecommendationCategory.CommonlyUsedTogether, 9);
            AddAssociation(associations, "ResearchProjectDef", "RecipeDef", "Unlocked recipes", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "ResearchProjectDef", "TerrainDef", "Unlocked terrain", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "RecipeDef", "ThingDef", "Produced items", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "RecipeDef", "SkillDef", "Required skills", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "RecipeDef", "WorkGiverDef", "Work assignments", RecommendationCategory.CommonlyUsedTogether, 7);
            AddAssociation(associations, "RecipeDef", "SoundDef", "Crafting sounds", RecommendationCategory.CommonlyUsedTogether, 6);

            AddAssociation(associations, "JobDef", "WorkGiverDef", "Work assignment", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "JobDef", "SoundDef", "Job sounds", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "HediffDef", "HediffGiverSetDef", "Hediff givers", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "HediffDef", "RecipeDef", "Medical recipes", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "HediffDef", "ThoughtDef", "Related thoughts", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "FactionDef", "PawnGroupKindDef", "Group types", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "FactionDef", "PawnKindDef", "Pawn types", RecommendationCategory.DirectlyRelated, 10);
            AddAssociation(associations, "FactionDef", "RaidStrategyDef", "Raid strategies", RecommendationCategory.CommonlyUsedTogether, 8);

            AddAssociation(associations, "IncidentDef", "FactionDef", "Related factions", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "IncidentDef", "StorytellerDef", "Storyteller settings", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "TerrainDef", "SoundDef", "Footstep sounds", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "TerrainDef", "FilthDef", "Related filth", RecommendationCategory.CommonlyUsedTogether, 6);

            AddAssociation(associations, "Apparel", "SoundDef", "Apparel sounds", RecommendationCategory.DirectlyRelated, 8);
            AddAssociation(associations, "Apparel", "BodyPartGroupDef", "Body part coverage", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "Apparel", "StuffDef", "Material options", RecommendationCategory.CommonlyUsedTogether, 8);

            AddAssociation(associations, "MentalStateDef", "ThoughtDef", "Related thoughts", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "MentalStateDef", "JobDef", "Related jobs", RecommendationCategory.Contextual, 7);

            AddAssociation(associations, "AbilityDef", "CompProperties_Ability", "Ability properties", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "AbilityDef", "SoundDef", "Ability sounds", RecommendationCategory.CommonlyUsedTogether, 7);
            AddAssociation(associations, "AbilityDef", "MoteDef", "Visual effects", RecommendationCategory.CommonlyUsedTogether, 7);

            AddAssociation(associations, "QuestDef", "FactionDef", "Related factions", RecommendationCategory.CommonlyUsedTogether, 8);
            AddAssociation(associations, "QuestDef", "RewardDef", "Quest rewards", RecommendationCategory.DirectlyRelated, 9);

            AddAssociation(associations, "RitualDef", "IdeoDef", "Ideology linkage", RecommendationCategory.DirectlyRelated, 9);
            AddAssociation(associations, "RitualDef", "SoundDef", "Ritual sounds", RecommendationCategory.CommonlyUsedTogether, 7);

            return associations;
        }

        private void AddAssociation(
            Dictionary<string, List<TypeAssociation>> associations,
            string typeName,
            string relatedDefType,
            string reason,
            RecommendationCategory category,
            int priority)
        {
            if (!associations.TryGetValue(typeName, out var list))
            {
                list = new List<TypeAssociation>();
                associations[typeName] = list;
            }
            list.Add(new TypeAssociation
            {
                RelatedDefType = relatedDefType,
                Reason = reason,
                Category = category,
                Priority = priority
            });
        }

        public ResourceRecommendationResult RecommendForType(Type type)
        {
            var result = new ResourceRecommendationResult(type);

            var recommendations = new List<ResourceRecommendation>();

            AddDirectTypeRecommendations(type, recommendations);
            AddInheritanceRecommendations(type, recommendations);
            AddFieldBasedRecommendations(type, recommendations);
            AddInterfaceRecommendations(type, recommendations);

            var groupedAndOrdered = recommendations
                .GroupBy(r => r.ResourceType)
                .Select(g => new ResourceRecommendation
                {
                    ResourceType = g.Key,
                    Reason = string.Join("; ", g.Select(r => r.Reason).Distinct()),
                    RelatedDefs = g.SelectMany(r => r.RelatedDefs).Distinct().ToList(),
                    Priority = g.Max(r => r.Priority),
                    Category = g.OrderBy(r => r.Category).First().Category
                })
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.Category)
                .ToList();

            result.Recommendations.AddRange(groupedAndOrdered);
            return result;
        }

        private void AddDirectTypeRecommendations(Type type, List<ResourceRecommendation> recommendations)
        {
            var typeName = type.Name;

            if (_typeAssociations.TryGetValue(typeName, out var associations))
            {
                foreach (var assoc in associations)
                {
                    var rec = new ResourceRecommendation
                    {
                        ResourceType = assoc.RelatedDefType,
                        Reason = assoc.Reason,
                        Priority = assoc.Priority,
                        Category = assoc.Category
                    };

                    rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                    recommendations.Add(rec);
                }
            }

            var defType = _typeDefMapper.GetDefType(type);
            if (defType != null)
            {
                var rec = new ResourceRecommendation
                {
                    ResourceType = defType,
                    Reason = "This type is a Def type",
                    Priority = 10,
                    Category = RecommendationCategory.DirectlyRelated
                };
                rec.RelatedDefs = GetRelatedDefs(defType);
                recommendations.Add(rec);
            }
        }

        private void AddInheritanceRecommendations(Type type, List<ResourceRecommendation> recommendations)
        {
            var currentType = type.BaseType;
            int inheritanceDepth = 1;

            while (currentType != null && currentType != typeof(object))
            {
                if (_typeAssociations.TryGetValue(currentType.Name, out var associations))
                {
                    foreach (var assoc in associations)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = assoc.RelatedDefType,
                            Reason = $"Inherited from {currentType.Name}: {assoc.Reason}",
                            Priority = Math.Max(1, assoc.Priority - inheritanceDepth * 2),
                            Category = RecommendationCategory.Contextual
                        };
                        rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                        recommendations.Add(rec);
                    }
                }

                currentType = currentType.BaseType;
                inheritanceDepth++;
            }
        }

        private void AddFieldBasedRecommendations(Type type, List<ResourceRecommendation> recommendations)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = type.GetFields(flags);

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;

                if (IsDefReference(fieldType))
                {
                    var defTypeName = GetDefTypeName(fieldType);
                    if (defTypeName != null)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = defTypeName,
                            Reason = $"Used in field '{field.Name}'",
                            Priority = 8,
                            Category = RecommendationCategory.DirectlyRelated
                        };
                        rec.RelatedDefs = GetRelatedDefs(defTypeName);
                        recommendations.Add(rec);
                    }
                }

                if (IsListOrEnumerable(fieldType, out var elementType))
                {
                    var elementDefType = GetDefTypeName(elementType);
                    if (elementDefType != null)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = elementDefType,
                            Reason = $"Used in collection field '{field.Name}'",
                            Priority = 7,
                            Category = RecommendationCategory.DirectlyRelated
                        };
                        rec.RelatedDefs = GetRelatedDefs(elementDefType);
                        recommendations.Add(rec);
                    }
                }
            }
        }

        private void AddInterfaceRecommendations(Type type, List<ResourceRecommendation> recommendations)
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (_typeAssociations.TryGetValue(iface.Name, out var associations))
                {
                    foreach (var assoc in associations)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = assoc.RelatedDefType,
                            Reason = $"Via interface {iface.Name}: {assoc.Reason}",
                            Priority = Math.Max(1, assoc.Priority - 3),
                            Category = RecommendationCategory.Contextual
                        };
                        rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                        recommendations.Add(rec);
                    }
                }
            }
        }

        public ResourceRecommendationResult RecommendForMethod(MethodInfo method)
        {
            var result = new ResourceRecommendationResult(method);

            var recommendations = new List<ResourceRecommendation>();

            AddParameterRecommendations(method, recommendations);
            AddReturnValueRecommendations(method, recommendations);
            AddMethodBodyRecommendations(method, recommendations);
            AddDeclaringTypeRecommendations(method, recommendations);

            var groupedAndOrdered = recommendations
                .GroupBy(r => r.ResourceType)
                .Select(g => new ResourceRecommendation
                {
                    ResourceType = g.Key,
                    Reason = string.Join("; ", g.Select(r => r.Reason).Distinct()),
                    RelatedDefs = g.SelectMany(r => r.RelatedDefs).Distinct().ToList(),
                    Priority = g.Max(r => r.Priority),
                    Category = g.OrderBy(r => r.Category).First().Category
                })
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.Category)
                .ToList();

            result.Recommendations.AddRange(groupedAndOrdered);
            return result;
        }

        private void AddParameterRecommendations(MethodInfo method, List<ResourceRecommendation> recommendations)
        {
            foreach (var param in method.GetParameters())
            {
                var paramType = param.ParameterType;

                if (IsDefReference(paramType))
                {
                    var defTypeName = GetDefTypeName(paramType);
                    if (defTypeName != null)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = defTypeName,
                            Reason = $"Used as parameter '{param.Name}'",
                            Priority = 9,
                            Category = RecommendationCategory.DirectlyRelated
                        };
                        rec.RelatedDefs = GetRelatedDefs(defTypeName);
                        recommendations.Add(rec);
                    }
                }

                if (_typeAssociations.TryGetValue(paramType.Name, out var associations))
                {
                    foreach (var assoc in associations)
                    {
                        var rec = new ResourceRecommendation
                        {
                            ResourceType = assoc.RelatedDefType,
                            Reason = $"Via parameter '{param.Name}' ({paramType.Name}): {assoc.Reason}",
                            Priority = Math.Max(1, assoc.Priority - 2),
                            Category = RecommendationCategory.Contextual
                        };
                        rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                        recommendations.Add(rec);
                    }
                }
            }
        }

        private void AddReturnValueRecommendations(MethodInfo method, List<ResourceRecommendation> recommendations)
        {
            var returnType = method.ReturnType;

            if (IsDefReference(returnType))
            {
                var defTypeName = GetDefTypeName(returnType);
                if (defTypeName != null)
                {
                    var rec = new ResourceRecommendation
                    {
                        ResourceType = defTypeName,
                        Reason = "Returned by this method",
                        Priority = 9,
                        Category = RecommendationCategory.DirectlyRelated
                    };
                    rec.RelatedDefs = GetRelatedDefs(defTypeName);
                    recommendations.Add(rec);
                }
            }

            if (IsListOrEnumerable(returnType, out var elementType))
            {
                var elementDefType = GetDefTypeName(elementType);
                if (elementDefType != null)
                {
                    var rec = new ResourceRecommendation
                    {
                        ResourceType = elementDefType,
                        Reason = "Returned as collection element by this method",
                        Priority = 8,
                        Category = RecommendationCategory.DirectlyRelated
                    };
                    rec.RelatedDefs = GetRelatedDefs(elementDefType);
                    recommendations.Add(rec);
                }
            }

            if (_typeAssociations.TryGetValue(returnType.Name, out var associations))
            {
                foreach (var assoc in associations)
                {
                    var rec = new ResourceRecommendation
                    {
                        ResourceType = assoc.RelatedDefType,
                        Reason = $"Via return type ({returnType.Name}): {assoc.Reason}",
                        Priority = Math.Max(1, assoc.Priority - 2),
                        Category = RecommendationCategory.Contextual
                    };
                    rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                    recommendations.Add(rec);
                }
            }
        }

        private void AddMethodBodyRecommendations(MethodInfo method, List<ResourceRecommendation> recommendations)
        {
            try
            {
                var body = method.GetMethodBody();
                if (body == null) return;

                var ilBytes = body.GetILAsByteArray();
                if (ilBytes == null) return;

                var accessedTypes = new HashSet<Type>();
                AnalyzeILForTypeAccesses(ilBytes, method.Module, accessedTypes);

                foreach (var accessedType in accessedTypes)
                {
                    if (IsDefReference(accessedType))
                    {
                        var defTypeName = GetDefTypeName(accessedType);
                        if (defTypeName != null)
                        {
                            var rec = new ResourceRecommendation
                            {
                                ResourceType = defTypeName,
                                Reason = $"Accessed in method body",
                                Priority = 6,
                                Category = RecommendationCategory.Contextual
                            };
                            rec.RelatedDefs = GetRelatedDefs(defTypeName);
                            recommendations.Add(rec);
                        }
                    }

                    if (_typeAssociations.TryGetValue(accessedType.Name, out var associations))
                    {
                        foreach (var assoc in associations.Take(2))
                        {
                            var rec = new ResourceRecommendation
                            {
                                ResourceType = assoc.RelatedDefType,
                                Reason = $"Via method body access ({accessedType.Name}): {assoc.Reason}",
                                Priority = Math.Max(1, assoc.Priority - 4),
                                Category = RecommendationCategory.Suggested
                            };
                            rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                            recommendations.Add(rec);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void AnalyzeILForTypeAccesses(byte[] ilBytes, Module module, HashSet<Type> accessedTypes)
        {
            for (int i = 0; i < ilBytes.Length;)
            {
                byte op1 = ilBytes[i++];

                if (op1 == 0x7B || op1 == 0x7C || op1 == 0x7D || op1 == 0x7E || op1 == 0x7F || op1 == 0x80)
                {
                    if (i + 4 <= ilBytes.Length)
                    {
                        int token = ilBytes[i] | (ilBytes[i + 1] << 8) | (ilBytes[i + 2] << 16) | (ilBytes[i + 3] << 24);
                        i += 4;

                        try
                        {
                            var field = module.ResolveField(token);
                            if (field != null)
                            {
                                accessedTypes.Add(field.FieldType);
                            }
                        }
                        catch { }
                    }
                }
                else if (op1 == 0x28 || op1 == 0x6F)
                {
                    if (i + 4 <= ilBytes.Length)
                    {
                        int token = ilBytes[i] | (ilBytes[i + 1] << 8) | (ilBytes[i + 2] << 16) | (ilBytes[i + 3] << 24);
                        i += 4;

                        try
                        {
                            var method = module.ResolveMethod(token);
                            if (method is MethodInfo mi)
                            {
                                accessedTypes.Add(mi.ReturnType);
                                foreach (var param in mi.GetParameters())
                                {
                                    accessedTypes.Add(param.ParameterType);
                                }
                            }
                        }
                        catch { }
                    }
                }
                else if (op1 == 0x74 || op1 == 0x73 || op1 == 0x8C || op1 == 0xA5)
                {
                    if (i + 4 <= ilBytes.Length)
                    {
                        i += 4;
                    }
                }
                else if (op1 == 0x20 || op1 == 0x21 || op1 == 0x22 || op1 == 0x23 || op1 == 0x72)
                {
                    if (i + 4 <= ilBytes.Length)
                    {
                        i += 4;
                    }
                }
                else if (op1 == 0x24)
                {
                    if (i + 8 <= ilBytes.Length)
                    {
                        i += 8;
                    }
                }
                else if (op1 == 0xFE)
                {
                    if (i < ilBytes.Length)
                    {
                        i++;
                    }
                }
            }
        }

        private void AddDeclaringTypeRecommendations(MethodInfo method, List<ResourceRecommendation> recommendations)
        {
            var declaringType = method.DeclaringType;
            if (declaringType == null) return;

            if (_typeAssociations.TryGetValue(declaringType.Name, out var associations))
            {
                foreach (var assoc in associations.Take(3))
                {
                    var rec = new ResourceRecommendation
                    {
                        ResourceType = assoc.RelatedDefType,
                        Reason = $"Via declaring type {declaringType.Name}: {assoc.Reason}",
                        Priority = Math.Max(1, assoc.Priority - 3),
                        Category = RecommendationCategory.Suggested
                    };
                    rec.RelatedDefs = GetRelatedDefs(assoc.RelatedDefType);
                    recommendations.Add(rec);
                }
            }
        }

        private List<string> GetRelatedDefs(string defType)
        {
            var defs = new List<string>();

            try
            {
                var options = new SearchOptions
                {
                    FilterType = defType,
                    MaxResults = 20,
                    ExactMatch = false,
                    CaseSensitive = false
                };

                var results = _defsProbe.Search("", options);
                defs.AddRange(results.Select(r => r.Name));
            }
            catch
            {
            }

            return defs;
        }

        private bool IsDefReference(Type type)
        {
            if (type == null) return false;

            if (type.Name.EndsWith("Def", StringComparison.Ordinal))
            {
                return true;
            }

            var current = type.BaseType;
            while (current != null)
            {
                if (current.Name == "Def" && current.Namespace == "Verse")
                {
                    return true;
                }
                current = current.BaseType;
            }

            return false;
        }

        private string GetDefTypeName(Type type)
        {
            if (type == null) return null;

            if (type.Name.EndsWith("Def", StringComparison.Ordinal))
            {
                return type.Name;
            }

            return _typeDefMapper.GetDefType(type);
        }

        private bool IsListOrEnumerable(Type type, out Type elementType)
        {
            elementType = null;

            if (type == null) return false;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                if (genericDef == typeof(List<>) ||
                    genericDef == typeof(IList<>) ||
                    genericDef == typeof(IEnumerable<>) ||
                    genericDef == typeof(ICollection<>))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            if (type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var enumInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (enumInterface != null)
                {
                    elementType = enumInterface.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> GetKnownTypeNames()
        {
            return _typeAssociations.Keys.OrderBy(k => k);
        }

        public IEnumerable<string> GetRelatedDefTypes(string typeName)
        {
            if (_typeAssociations.TryGetValue(typeName, out var associations))
            {
                return associations.Select(a => a.RelatedDefType).Distinct().OrderBy(t => t);
            }
            return Enumerable.Empty<string>();
        }
    }

    internal class TypeAssociation
    {
        public string RelatedDefType { get; set; }
        public string Reason { get; set; }
        public RecommendationCategory Category { get; set; }
        public int Priority { get; set; }
    }
}
