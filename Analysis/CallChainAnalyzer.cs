using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe.Analysis
{
    public class CallChainAnalyzer
    {
        private readonly ProbeContext _context;
        private readonly Dictionary<string, List<CallChainResult>> _callerCache = new Dictionary<string, List<CallChainResult>>();
        private readonly Dictionary<string, List<CallChainResult>> _calleeCache = new Dictionary<string, List<CallChainResult>>();

        public CallChainAnalyzer(ProbeContext context)
        {
            _context = context;
        }

        public List<CallChainResult> GetCallers(MethodInfo targetMethod)
        {
            if (targetMethod == null) return new List<CallChainResult>();

            var cacheKey = GetMethodCacheKey(targetMethod);
            if (_callerCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var callers = new List<CallChainResult>();

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                    foreach (var method in type.GetMethods(flags))
                    {
                        if (method.IsSpecialName) continue;

                        try
                        {
                            var callees = GetCallees(method);
                            if (callees.Any(c => MethodMatches(c.MethodInfo, targetMethod)))
                            {
                                callers.Add(new CallChainResult(method, type, "Caller"));
                            }
                        }
                        catch { }
                    }
                }
            }

            _callerCache[cacheKey] = callers;
            return callers;
        }

        public List<CallChainResult> GetCallees(MethodInfo sourceMethod)
        {
            if (sourceMethod == null) return new List<CallChainResult>();

            var cacheKey = GetMethodCacheKey(sourceMethod);
            if (_calleeCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var callees = new List<CallChainResult>();

            try
            {
                var body = sourceMethod.GetMethodBody();
                if (body == null)
                {
                    _calleeCache[cacheKey] = callees;
                    return callees;
                }

                var ilBytes = body.GetILAsByteArray();
                if (ilBytes == null || ilBytes.Length == 0)
                {
                    _calleeCache[cacheKey] = callees;
                    return callees;
                }

                var calledMethods = ParseILForMethodCalls(ilBytes, sourceMethod.Module);
                foreach (var calledMethod in calledMethods)
                {
                    callees.Add(new CallChainResult(calledMethod, calledMethod.DeclaringType, "Callee"));
                }
            }
            catch { }

            _calleeCache[cacheKey] = callees;
            return callees;
        }

        private List<MethodInfo> ParseILForMethodCalls(byte[] ilBytes, Module module)
        {
            var methods = new List<MethodInfo>();

            for (int i = 0; i < ilBytes.Length;)
            {
                var opCode = IlHelper.ReadOpCode(ilBytes, ref i);
                if (opCode == null || opCode == System.Reflection.Emit.OpCodes.Nop) break;

                if (IlHelper.IsMethodCallOpCode(opCode))
                {
                    int token = IlHelper.ReadToken(ilBytes, ref i);
                    try
                    {
                        var methodBase = module.ResolveMethod(token);
                        if (methodBase is MethodInfo methodInfo)
                        {
                            methods.Add(methodInfo);
                        }
                    }
                    catch { }
                }
                else
                {
                    IlHelper.SkipOperand(opCode, ilBytes, ref i);
                }
            }

            return methods.Distinct().ToList();
        }

        private string GetMethodCacheKey(MethodInfo method)
        {
            return $"{method.DeclaringType.FullName}.{method.Name}";
        }

        private bool MethodMatches(MethodInfo m1, MethodInfo m2)
        {
            if (m1 == null || m2 == null) return false;

            if (m1.Name != m2.Name) return false;
            if (m1.DeclaringType.FullName != m2.DeclaringType.FullName) return false;

            var p1 = m1.GetParameters();
            var p2 = m2.GetParameters();
            if (p1.Length != p2.Length) return false;

            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i].ParameterType.FullName != p2[i].ParameterType.FullName)
                    return false;
            }

            return true;
        }

        public void ClearCache()
        {
            _callerCache.Clear();
            _calleeCache.Clear();
        }
    }
}
