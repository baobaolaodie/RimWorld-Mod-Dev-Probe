using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Analysis
{
    public class TypeDefMapper
    {
        private readonly ProbeContext _context;
        private readonly Dictionary<Type, string> _typeToDefTypeCache = new Dictionary<Type, string>();
        private readonly Dictionary<string, Type> _defTypeToTypeCache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<Type> SimpleTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int), typeof(int?),
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(bool), typeof(bool?),
            typeof(long), typeof(long?),
            typeof(byte), typeof(byte?),
            typeof(short), typeof(short?),
            typeof(char), typeof(char?),
            typeof(decimal), typeof(decimal?)
        };

        private static readonly HashSet<string> SpecialFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "defName", "label", "description", "thingClass", "category", "parent",
            "abstract", "Name", "Class"
        };

        public TypeDefMapper(ProbeContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.LoadGameAssemblies();
            BuildTypeMappings();
        }

        private void BuildTypeMappings()
        {
            foreach (var asm in _context.LoadedAssemblies)
            {
                try
                {
                    foreach (var type in GetTypesSafe(asm))
                    {
                        if (IsDefType(type))
                        {
                            var defTypeName = type.Name;
                            if (!_typeToDefTypeCache.ContainsKey(type))
                            {
                                _typeToDefTypeCache[type] = defTypeName;
                            }
                            if (!_defTypeToTypeCache.ContainsKey(defTypeName))
                            {
                                _defTypeToTypeCache[defTypeName] = type;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private bool IsDefType(Type type)
        {
            if (type == null || type.IsAbstract || type.IsInterface) return false;
            if (!type.IsClass) return false;

            var current = type;
            while (current != null)
            {
                if (current.Name == "Def" && current.Namespace == "Verse")
                {
                    return true;
                }
                current = current.BaseType;
            }

            return type.Name.EndsWith("Def", StringComparison.Ordinal);
        }

        private IEnumerable<Type> GetTypesSafe(Assembly asm)
        {
            try { return asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
            catch { return Enumerable.Empty<Type>(); }
        }

        public string GetDefType(Type type)
        {
            if (type == null) return null;

            if (_typeToDefTypeCache.TryGetValue(type, out var defType))
            {
                return defType;
            }

            if (type.Name.EndsWith("Def", StringComparison.Ordinal))
            {
                return type.Name;
            }

            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (_typeToDefTypeCache.TryGetValue(baseType, out var baseDefType))
                {
                    return baseDefType;
                }
                baseType = baseType.BaseType;
            }

            return null;
        }

        public Type GetTypeByDefName(string defTypeName)
        {
            if (string.IsNullOrEmpty(defTypeName)) return null;

            if (_defTypeToTypeCache.TryGetValue(defTypeName, out var type))
            {
                return type;
            }

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var t in GetTypesSafe(asm))
                {
                    if (t.Name.Equals(defTypeName, StringComparison.OrdinalIgnoreCase))
                    {
                        _defTypeToTypeCache[defTypeName] = t;
                        return t;
                    }
                }
            }

            return null;
        }

        public string GetXmlStructure(Type type, int maxDepth = 3)
        {
            if (type == null) return "";

            var sb = new StringBuilder();
            var defType = GetDefType(type) ?? type.Name;

            sb.AppendLine($"<{defType}>");
            sb.AppendLine("  <defName>ExampleDefName</defName>");

            if (HasField(type, "label"))
            {
                sb.AppendLine("  <label>An Example Label</label>");
            }

            if (HasField(type, "description"))
            {
                sb.AppendLine("  <description>A description of this def.</description>");
            }

            var processedFields = new HashSet<string> { "defName", "label", "description" };
            var fields = GetSerializableFields(type);

            foreach (var field in fields)
            {
                if (processedFields.Contains(field.Name)) continue;
                if (field.IsStatic) continue;

                var xmlContent = GenerateFieldXml(field, 1, maxDepth, new HashSet<Type>());
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    sb.Append(xmlContent);
                    processedFields.Add(field.Name);
                }
            }

            sb.AppendLine($"</{defType}>");
            return sb.ToString();
        }

        private bool HasField(Type type, string fieldName)
        {
            var current = type;
            while (current != null && current != typeof(object))
            {
                var field = current.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null) return true;
                current = current.BaseType;
            }
            return false;
        }

        private IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            var fields = new List<FieldInfo>();
            var current = type;

            while (current != null && current != typeof(object))
            {
                var typeFields = current.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in typeFields)
                {
                    if (field.IsPublic || HasSerializeFieldAttribute(field))
                    {
                        fields.Add(field);
                    }
                }

                current = current.BaseType;
            }

            return fields.GroupBy(f => f.Name).Select(g => g.First());
        }

        private bool HasSerializeFieldAttribute(FieldInfo field)
        {
            foreach (var attr in field.GetCustomAttributes(false))
            {
                var attrTypeName = attr.GetType().Name;
                if (attrTypeName == "SerializeFieldAttribute" ||
                    attrTypeName == "SerializeField")
                {
                    return true;
                }
            }
            return false;
        }

        private string GenerateFieldXml(FieldInfo field, int indent, int maxDepth, HashSet<Type> visitedTypes)
        {
            var sb = new StringBuilder();
            var indentStr = new string(' ', indent * 2);
            var xmlName = GetFieldXmlName(field);
            var fieldType = field.FieldType;

            if (visitedTypes.Contains(fieldType)) return "";

            if (maxDepth <= 0)
            {
                sb.AppendLine($"{indentStr}<{xmlName}>...</{xmlName}>");
                return sb.ToString();
            }

            if (IsSimpleType(fieldType))
            {
                var exampleValue = GetExampleValue(fieldType);
                sb.AppendLine($"{indentStr}<{xmlName}>{exampleValue}</{xmlName}>");
            }
            else if (IsListType(fieldType, out var elementType))
            {
                sb.AppendLine($"{indentStr}<{xmlName}>");
                if (IsSimpleType(elementType))
                {
                    sb.AppendLine($"{indentStr}  <li>{GetExampleValue(elementType)}</li>");
                    sb.AppendLine($"{indentStr}  <li>{GetExampleValue(elementType)}</li>");
                }
                else if (IsDefType(elementType))
                {
                    sb.AppendLine($"{indentStr}  <li>Some{elementType.Name}</li>");
                }
                else
                {
                    var newVisited = new HashSet<Type>(visitedTypes) { fieldType };
                    sb.AppendLine($"{indentStr}  <li>");
                    GenerateTypeFieldsXml(elementType, sb, indent + 2, maxDepth - 1, newVisited);
                    sb.AppendLine($"{indentStr}  </li>");
                }
                sb.AppendLine($"{indentStr}</{xmlName}>");
            }
            else if (IsDefType(fieldType))
            {
                sb.AppendLine($"{indentStr}<{xmlName}>Some{fieldType.Name}</{xmlName}>");
            }
            else if (IsNullableType(fieldType, out var underlyingType))
            {
                if (IsSimpleType(underlyingType))
                {
                    sb.AppendLine($"{indentStr}<{xmlName}>{GetExampleValue(underlyingType)}</{xmlName}>");
                }
                else
                {
                    sb.AppendLine($"{indentStr}<{xmlName}>...</{xmlName}>");
                }
            }
            else if (fieldType.IsEnum)
            {
                var enumValues = Enum.GetNames(fieldType);
                var exampleValue = enumValues.Length > 0 ? enumValues[0] : "EnumValue";
                sb.AppendLine($"{indentStr}<{xmlName}>{exampleValue}</{xmlName}>");
            }
            else if (fieldType.IsClass || fieldType.IsValueType)
            {
                sb.AppendLine($"{indentStr}<{xmlName}>");
                var newVisited = new HashSet<Type>(visitedTypes) { fieldType };
                GenerateTypeFieldsXml(fieldType, sb, indent + 1, maxDepth - 1, newVisited);
                sb.AppendLine($"{indentStr}</{xmlName}>");
            }

            return sb.ToString();
        }

        private void GenerateTypeFieldsXml(Type type, StringBuilder sb, int indent, int maxDepth, HashSet<Type> visitedTypes)
        {
            var indentStr = new string(' ', indent * 2);
            var fields = GetSerializableFields(type).Take(5);

            foreach (var field in fields)
            {
                if (field.IsStatic) continue;

                var xmlContent = GenerateFieldXml(field, indent, maxDepth, visitedTypes);
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    sb.Append(xmlContent);
                }
            }

            if (GetSerializableFields(type).Count() > 5)
            {
                sb.AppendLine($"{indentStr}<!-- ... more fields ... -->");
            }
        }

        public string GetFieldXmlName(FieldInfo field)
        {
            if (field == null) return "";

            var name = field.Name;

            if (SpecialFields.Contains(name))
            {
                return name;
            }

            if (name.Length > 0)
            {
                return char.ToLowerInvariant(name[0]) + name.Substring(1);
            }

            return name;
        }

        public string GetFieldXmlName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return "";

            if (SpecialFields.Contains(fieldName))
            {
                return fieldName;
            }

            if (fieldName.Length > 0)
            {
                return char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
            }

            return fieldName;
        }

        private bool IsSimpleType(Type type)
        {
            if (type == null) return false;
            return SimpleTypes.Contains(type) || SimpleTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);
        }

        private bool IsListType(Type type, out Type elementType)
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
                    genericDef == typeof(IEnumerable<>))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            if (type.Name == "List`1" && type.Namespace == "System.Collections.Generic")
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        private bool IsNullableType(Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }

        private string GetExampleValue(Type type)
        {
            if (type == typeof(string)) return "SomeText";
            if (type == typeof(int) || type == typeof(int?)) return "0";
            if (type == typeof(float) || type == typeof(float?)) return "1.0";
            if (type == typeof(double) || type == typeof(double?)) return "1.0";
            if (type == typeof(bool) || type == typeof(bool?)) return "true";
            if (type == typeof(long) || type == typeof(long?)) return "0";
            if (type == typeof(byte) || type == typeof(byte?)) return "0";
            if (type == typeof(short) || type == typeof(short?)) return "0";
            if (type == typeof(char) || type == typeof(char?)) return "A";
            if (type == typeof(decimal) || type == typeof(decimal?)) return "0.0";

            return "...";
        }

        public IEnumerable<string> GetAllDefTypes()
        {
            return _defTypeToTypeCache.Keys.OrderBy(k => k);
        }

        public Dictionary<string, string> GetFieldMappings(Type type)
        {
            var mappings = new Dictionary<string, string>();

            if (type == null) return mappings;

            var fields = GetSerializableFields(type);
            foreach (var field in fields)
            {
                var xmlName = GetFieldXmlName(field);
                var fieldType = GetFriendlyTypeName(field.FieldType);
                mappings[field.Name] = $"{xmlName} ({fieldType})";
            }

            return mappings;
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type == null) return "unknown";

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                var args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));

                if (genericDef == typeof(List<>)) return $"List<{args}>";
                if (genericDef == typeof(Dictionary<,>)) return $"Dictionary<{args}>";

                return $"{type.Name.Split('`')[0]}<{args}>";
            }

            if (type.IsArray)
            {
                return $"{GetFriendlyTypeName(type.GetElementType())}[]";
            }

            return type.Name;
        }

        public TypeMappingInfo GetDetailedMapping(Type type)
        {
            if (type == null) return null;

            var info = new TypeMappingInfo
            {
                CSharpType = type,
                DefTypeName = GetDefType(type),
                XmlStructure = GetXmlStructure(type)
            };

            var fields = GetSerializableFields(type);
            foreach (var field in fields)
            {
                info.FieldMappings.Add(new FieldMappingInfo
                {
                    FieldName = field.Name,
                    XmlName = GetFieldXmlName(field),
                    FieldType = field.FieldType,
                    IsSimpleType = IsSimpleType(field.FieldType),
                    IsListType = IsListType(field.FieldType, out var elemType),
                    ElementType = elemType,
                    IsDefReference = IsDefType(field.FieldType),
                    IsSpecialField = SpecialFields.Contains(field.Name)
                });
            }

            return info;
        }
    }

    public class TypeMappingInfo
    {
        public Type CSharpType { get; set; }
        public string DefTypeName { get; set; }
        public string XmlStructure { get; set; }
        public List<FieldMappingInfo> FieldMappings { get; set; } = new List<FieldMappingInfo>();
    }

    public class FieldMappingInfo
    {
        public string FieldName { get; set; }
        public string XmlName { get; set; }
        public Type FieldType { get; set; }
        public bool IsSimpleType { get; set; }
        public bool IsListType { get; set; }
        public Type ElementType { get; set; }
        public bool IsDefReference { get; set; }
        public bool IsSpecialField { get; set; }
    }
}
