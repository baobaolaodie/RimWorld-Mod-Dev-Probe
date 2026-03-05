using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe.Analysis
{
    public class FieldUsageAnalyzer
    {
        private readonly ProbeContext _context;

        public FieldUsageAnalyzer(ProbeContext context)
        {
            _context = context;
        }

        public FieldUsageResult Analyze(FieldInfo field)
        {
            var result = new FieldUsageResult(field);
            var usages = GetAllUsages(field);

            foreach (var usage in usages)
            {
                result.AllUsages.Add(usage);
                if (usage.AccessType == FieldAccessType.Read)
                {
                    result.ReadPositions.Add(usage);
                }
                else if (usage.AccessType == FieldAccessType.Write)
                {
                    result.WritePositions.Add(usage);
                }
                else
                {
                    result.ReadPositions.Add(usage);
                    result.WritePositions.Add(usage);
                }
            }

            return result;
        }

        public List<FieldUsageLocation> GetReadPositions(FieldInfo field)
        {
            var results = new List<FieldUsageLocation>();

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    AnalyzeTypeForFieldReads(type, field, results);
                }
            }

            return results;
        }

        public List<FieldUsageLocation> GetWritePositions(FieldInfo field)
        {
            var results = new List<FieldUsageLocation>();

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    AnalyzeTypeForFieldWrites(type, field, results);
                }
            }

            return results;
        }

        public List<FieldUsageLocation> GetAllUsages(FieldInfo field)
        {
            var results = new List<FieldUsageLocation>();

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    AnalyzeTypeForFieldUsages(type, field, results);
                }
            }

            return results;
        }

        private void AnalyzeTypeForFieldReads(Type type, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            foreach (var method in type.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                try
                {
                    var body = method.GetMethodBody();
                    if (body == null) continue;

                    var ilBytes = body.GetILAsByteArray();
                    if (ilBytes == null) continue;

                    ParseILForFieldReads(ilBytes, method, type, targetField, results);
                }
                catch { }
            }
        }

        private void AnalyzeTypeForFieldWrites(Type type, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            foreach (var method in type.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                try
                {
                    var body = method.GetMethodBody();
                    if (body == null) continue;

                    var ilBytes = body.GetILAsByteArray();
                    if (ilBytes == null) continue;

                    ParseILForFieldWrites(ilBytes, method, type, targetField, results);
                }
                catch { }
            }
        }

        private void AnalyzeTypeForFieldUsages(Type type, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            foreach (var method in type.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                try
                {
                    var body = method.GetMethodBody();
                    if (body == null) continue;

                    var ilBytes = body.GetILAsByteArray();
                    if (ilBytes == null) continue;

                    ParseILForFieldUsages(ilBytes, method, type, targetField, results);
                }
                catch { }
            }
        }

        private void ParseILForFieldReads(byte[] ilBytes, MethodInfo method, Type declaringType, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            for (int i = 0; i < ilBytes.Length;)
            {
                var opCode = IlHelper.ReadOpCode(ilBytes, ref i);
                if (opCode == null || opCode == System.Reflection.Emit.OpCodes.Nop) break;

                if (IlHelper.IsFieldReadOpCode(opCode))
                {
                    int token = IlHelper.ReadToken(ilBytes, ref i);
                    try
                    {
                        var field = method.Module.ResolveField(token);
                        if (field != null && FieldMatches(field, targetField))
                        {
                            results.Add(new FieldUsageLocation
                            {
                                TypeName = declaringType.FullName,
                                MethodName = method.Name,
                                MethodSignature = GetMethodSignature(method),
                                AccessType = FieldAccessType.Read,
                                ILOffset = i - 5,
                                MethodInfo = method
                            });
                        }
                    }
                    catch { }
                }
                else
                {
                    IlHelper.SkipOperand(opCode, ilBytes, ref i);
                }
            }
        }

        private void ParseILForFieldWrites(byte[] ilBytes, MethodInfo method, Type declaringType, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            for (int i = 0; i < ilBytes.Length;)
            {
                var opCode = IlHelper.ReadOpCode(ilBytes, ref i);
                if (opCode == null || opCode == System.Reflection.Emit.OpCodes.Nop) break;

                if (IlHelper.IsFieldWriteOpCode(opCode))
                {
                    int token = IlHelper.ReadToken(ilBytes, ref i);
                    try
                    {
                        var field = method.Module.ResolveField(token);
                        if (field != null && FieldMatches(field, targetField))
                        {
                            results.Add(new FieldUsageLocation
                            {
                                TypeName = declaringType.FullName,
                                MethodName = method.Name,
                                MethodSignature = GetMethodSignature(method),
                                AccessType = FieldAccessType.Write,
                                ILOffset = i - 5,
                                MethodInfo = method
                            });
                        }
                    }
                    catch { }
                }
                else
                {
                    IlHelper.SkipOperand(opCode, ilBytes, ref i);
                }
            }
        }

        private void ParseILForFieldUsages(byte[] ilBytes, MethodInfo method, Type declaringType, FieldInfo targetField, List<FieldUsageLocation> results)
        {
            for (int i = 0; i < ilBytes.Length;)
            {
                var opCode = IlHelper.ReadOpCode(ilBytes, ref i);
                if (opCode == null || opCode == System.Reflection.Emit.OpCodes.Nop) break;

                if (IlHelper.IsFieldReadOpCode(opCode) || IlHelper.IsFieldWriteOpCode(opCode))
                {
                    int token = IlHelper.ReadToken(ilBytes, ref i);
                    try
                    {
                        var field = method.Module.ResolveField(token);
                        if (field != null && FieldMatches(field, targetField))
                        {
                            var accessType = IlHelper.IsFieldReadOpCode(opCode) ? FieldAccessType.Read : FieldAccessType.Write;
                            results.Add(new FieldUsageLocation
                            {
                                TypeName = declaringType.FullName,
                                MethodName = method.Name,
                                MethodSignature = GetMethodSignature(method),
                                AccessType = accessType,
                                ILOffset = i - 5,
                                MethodInfo = method
                            });
                        }
                    }
                    catch { }
                }
                else
                {
                    IlHelper.SkipOperand(opCode, ilBytes, ref i);
                }
            }
        }

        private bool FieldMatches(FieldInfo f1, FieldInfo f2)
        {
            if (f1 == null || f2 == null) return false;

            if (f1.Name != f2.Name) return false;
            if (f1.DeclaringType?.FullName != f2.DeclaringType?.FullName) return false;

            return true;
        }

        private string GetMethodSignature(MethodInfo method)
        {
            var visibility = method.IsPublic ? "public" : method.IsPrivate ? "private" : method.IsFamily ? "protected" : "internal";
            var staticStr = method.IsStatic ? "static " : "";
            var params_ = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            return $"[{visibility}] {staticStr}{method.ReturnType.Name} {method.Name}({params_})";
        }
    }
}
