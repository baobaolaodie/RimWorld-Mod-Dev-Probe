using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class HarmonyExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetPrefixPatchExample());
            examples.Add(GetPostfixPatchExample());
            examples.Add(GetTranspilerExample());

            return examples;
        }

        private static Example GetPrefixPatchExample()
        {
            var example = new Example
            {
                Title = "Harmony Prefix Patch示例",
                Description = "使用Harmony Prefix在原方法执行前拦截并修改参数或跳过原方法执行。",
                Feature = "Harmony Patch",
                Keywords = new List<string> { "Prefix Patch", "prefix", "Harmony Prefix", "前置补丁" }
            };

            example.Files.Add(new ExampleFile(
                "PrefixPatchExample.cs",
                "Source/PrefixPatchExample.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;

namespace HarmonyPrefixExample
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(""Kill"")]
    public static class Pawn_Kill_Prefix
    {
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff hediff)
        {
            if (__instance == null)
            {
                return true;
            }

            if (__instance.RaceProps != null && __instance.RaceProps.Humanlike)
            {
                Log.Message($""[HarmonyExample] Humanlike pawn {__instance.LabelShort} is being killed."");
            }

            if (dinfo.HasValue && dinfo.Value.Def != null)
            {
                Log.Message($""[HarmonyExample] Damage type: {dinfo.Value.Def.defName}"");
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CompRefuelable))]
    [HarmonyPatch(""ConsumeFuel"")]
    public static class CompRefuelable_ConsumeFuel_Prefix
    {
        public static bool Prefix(CompRefuelable __instance, float amount)
        {
            if (__instance.parent.def.defName == ""CustomGenerator"")
            {
                float currentFuel = __instance.Fuel;
                if (currentFuel >= amount * 0.5f)
                {
                    Traverse.Create(__instance).Field(""fuel"").SetValue(currentFuel - amount * 0.5f);
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobGiver_Work))]
    [HarmonyPatch(""TryIssueJobPackage"")]
    public static class JobGiver_Work_TryIssueJobPackage_Prefix
    {
        public static void Prefix(Pawn pawn, bool __state)
        {
            __state = pawn.mindState.lastJobTag;
        }

        public static void Postfix(Pawn pawn, bool __state)
        {
            if (pawn.mindState.lastJobTag != __state)
            {
                Log.Message($""[HarmonyExample] {pawn.LabelShort} job changed"");
            }
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "HarmonyLoader.cs",
                "Source/HarmonyLoader.cs",
                @"using HarmonyLib;
using Verse;

namespace HarmonyPrefixExample
{
    [StaticConstructorOnStartup]
    public static class HarmonyLoader
    {
        static HarmonyLoader()
        {
            var harmony = new Harmony(""com.yourname.harmonyprefixexample"");
            harmony.PatchAll();
            Log.Message(""[HarmonyPrefixExample] Harmony patches loaded."");
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("添加 0Harmony.dll 或 HarmonyX 引用到项目");
            example.Steps.Add("创建 HarmonyPatch 类，使用 [HarmonyPatch] 特性标注目标方法");
            example.Steps.Add("实现 Prefix 方法，返回 true 继续执行原方法，返回 false 跳过原方法");
            example.Steps.Add("使用 __instance 访问当前实例，使用 __state 在 Prefix 和 Postfix 间传递数据");
            example.Steps.Add("创建 HarmonyLoader 类，在游戏启动时加载所有 Patch");
            example.Steps.Add("编译并测试：验证 Prefix 是否正确执行");

            return example;
        }

        private static Example GetPostfixPatchExample()
        {
            var example = new Example
            {
                Title = "Harmony Postfix Patch示例",
                Description = "使用Harmony Postfix在原方法执行后修改返回值或执行额外逻辑。",
                Feature = "Harmony Patch",
                Keywords = new List<string> { "Postfix Patch", "postfix", "Harmony Postfix", "后置补丁" }
            };

            example.Files.Add(new ExampleFile(
                "PostfixPatchExample.cs",
                "Source/PostfixPatchExample.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace HarmonyPostfixExample
{
    [HarmonyPatch(typeof(StatExtension))]
    [HarmonyPatch(""GetStatValue"")]
    public static class StatExtension_GetStatValue_Postfix
    {
        public static void Postfix(Thing thing, StatDef stat, ref float __result)
        {
            if (thing == null || stat == null)
            {
                return;
            }

            if (stat == StatDefOf.MoveSpeed && thing is Pawn pawn)
            {
                if (pawn.health != null && pawn.health.hediffSet != null)
                {
                    foreach (var hediff in pawn.health.hediffSet.hediffs)
                    {
                        if (hediff.def.defName == ""CustomSpeedBoostHediff"")
                        {
                            __result *= 1.5f;
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch(""AddHediff"")]
    [HarmonyPatch(new System.Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
    public static class Pawn_HealthTracker_AddHediff_Postfix
    {
        public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff, Pawn ___pawn)
        {
            if (___pawn == null || hediff == null)
            {
                return;
            }

            if (hediff.def.defName == ""CustomInfection"")
            {
                Log.Message($""[HarmonyExample] {___pawn.LabelShort} received custom infection."");

                if (___pawn.Faction != null && ___pawn.Faction.IsPlayer)
                {
                    Find.LetterStack.ReceiveLetter(
                        ""Custom Infection"",
                        $""{___pawn.LabelShortCap} has been infected with a custom disease."",
                        LetterDefOf.NegativeEvent,
                        ___pawn
                    );
                }
            }
        }
    }

    [HarmonyPatch(typeof(RecipeDef))]
    [HarmonyPatch(""Products"")]
    public static class RecipeDef_Products_Postfix
    {
        public static void Postfix(RecipeDef __instance, Pawn worker, List<Thing> __result)
        {
            if (__instance.defName == ""MakeCustomItem"")
            {
                if (worker.skills != null)
                {
                    int craftingLevel = worker.skills.GetSkill(SkillDefOf.Crafting)?.Level ?? 0;
                    if (craftingLevel >= 15)
                    {
                        Thing bonusItem = ThingMaker.MakeThing(ThingDefOf.Gold);
                        bonusItem.stackCount = 5;
                        __result.Add(bonusItem);
                    }
                }
            }
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "TranspilerAccessExample.cs",
                "Source/TranspilerAccessExample.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HarmonyPostfixExample
{
    [HarmonyPatch(typeof(TradeDeal))]
    [HarmonyPatch(""UpdateCurrencyCount"")]
    public static class TradeDeal_UpdateCurrencyCount_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var targetMethod = AccessTools.Method(typeof(TradeDeal), ""get_SilverToTrader"");

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(targetMethod))
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(TradeDeal_UpdateCurrencyCount_Transpiler), ""ModifyCurrency"")));
                }
                yield return codes[i];
            }
        }

        public static float ModifyCurrency(float original)
        {
            return original * 0.9f;
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("创建 Postfix 方法，使用 __result 参数修改返回值(ref)");
            example.Steps.Add("使用 ___fieldName 访问私有字段(三个下划线前缀)");
            example.Steps.Add("在 Postfix 中可以执行额外逻辑，如发送通知");
            example.Steps.Add("可以修改 __result 来改变原方法的返回值");
            example.Steps.Add("注意 Postfix 方法返回类型应为 void");
            example.Steps.Add("测试：验证 Postfix 是否正确修改了返回值或执行了额外逻辑");

            return example;
        }

        private static Example GetTranspilerExample()
        {
            var example = new Example
            {
                Title = "Harmony Transpiler示例",
                Description = "使用Harmony Transpiler在IL层面修改方法实现，实现更复杂的逻辑修改。",
                Feature = "Harmony Patch",
                Keywords = new List<string> { "Transpiler", "transpiler", "Harmony Transpiler", "IL修改", "代码注入" }
            };

            example.Files.Add(new ExampleFile(
                "TranspilerExample.cs",
                "Source/TranspilerExample.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace HarmonyTranspilerExample
{
    [HarmonyPatch(typeof(Pawn_MindState))]
    [HarmonyPatch(""CheckStartMentalStateCollaborator"")]
    public static class Pawn_MindState_CheckStartMentalStateCollaborator_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool found = false;

            for (int i = 0; i < codes.Count; i++)
            {
                if (!found && codes[i].opcode == OpCodes.Ldc_R4)
                {
                    float value = (float)codes[i].operand;
                    if (value == 0.5f)
                    {
                        codes[i].operand = 0.3f;
                        found = true;
                        Log.Message(""[Transpiler] Modified mental state threshold."");
                    }
                }
                yield return codes[i];
            }
        }
    }

    [HarmonyPatch(typeof(JobDriver_HaulToCell))]
    [HarmonyPatch(""MakeNewToils"")]
    public static class JobDriver_HaulToCell_MakeNewToils_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            var targetMethod = AccessTools.Method(typeof(Pawn), ""get_CurJob"");

            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];

                if (codes[i].Calls(targetMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JobDriver_HaulToCell_MakeNewToils_Transpiler), ""LogHaulJob""));
                }
            }
        }

        public static Job LogHaulJob(Job job)
        {
            if (job != null && job.targetA != null)
            {
                Log.Message($""[Transpiler] Hauling to: {job.targetA.Cell}"");
            }
            return job;
        }
    }

    [HarmonyPatch(typeof(Verb_MeleeAttack))]
    [HarmonyPatch(""TryCastShot"")]
    public static class Verb_MeleeAttack_TryCastShot_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            var damageMethod = AccessTools.Method(typeof(Verb_MeleeAttack), ""DoDamage"");

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].Calls(damageMethod))
                {
                    Label skipLabel = generator.DefineLabel();
                    
                    codes.Insert(i - 2, new CodeInstruction(OpCodes.Ldarg_0));
                    codes.Insert(i - 1, new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Verb_MeleeAttack_TryCastShot_Transpiler), ""ShouldSkipDamage"")));
                    codes.Insert(i, new CodeInstruction(OpCodes.Brtrue_S, skipLabel));

                    int insertIndex = i + 4;
                    while (insertIndex < codes.Count && codes[insertIndex].opcode != OpCodes.Ret)
                    {
                        insertIndex++;
                    }
                    
                    if (insertIndex < codes.Count)
                    {
                        codes[insertIndex].labels.Add(skipLabel);
                    }

                    break;
                }
            }

            return codes;
        }

        public static bool ShouldSkipDamage(Verb_MeleeAttack verb)
        {
            if (verb.CasterPawn != null && verb.CasterPawn.health?.hediffSet != null)
            {
                return verb.CasterPawn.health.hediffSet.HasHediff(HediffDefOf.Catatonia);
            }
            return false;
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "TranspilerHelper.cs",
                "Source/TranspilerHelper.cs",
                @"using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HarmonyTranspilerExample
{
    public static class TranspilerHelper
    {
        public static int FindInstructionIndex(List<CodeInstruction> codes, OpCode opcode, object operand = null)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == opcode)
                {
                    if (operand == null || codes[i].operand == operand)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int FindCallIndex(List<CodeInstruction> codes, MethodInfo method)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(method))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void InsertInstructions(List<CodeInstruction> codes, int index, params CodeInstruction[] newInstructions)
        {
            codes.InsertRange(index, newInstructions);
        }

        public static CodeInstruction CreateLoadArgument(int index)
        {
            return index switch
            {
                0 => new CodeInstruction(OpCodes.Ldarg_0),
                1 => new CodeInstruction(OpCodes.Ldarg_1),
                2 => new CodeInstruction(OpCodes.Ldarg_2),
                3 => new CodeInstruction(OpCodes.Ldarg_3),
                _ => new CodeInstruction(OpCodes.Ldarg_S, index)
            };
        }

        public static CodeInstruction CreateLoadField(FieldInfo field)
        {
            return new CodeInstruction(OpCodes.Ldfld, field);
        }

        public static CodeInstruction CreateCall(MethodInfo method)
        {
            return new CodeInstruction(OpCodes.Call, method);
        }

        public static CodeInstruction CreateCallVirtual(MethodInfo method)
        {
            return new CodeInstruction(OpCodes.Callvirt, method);
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("使用 dnSpy 或 ILSpy 查看目标方法的 IL 代码");
            example.Steps.Add("理解基本的 IL 指令(OpCodes)如 Ldarg, Call, Ret 等");
            example.Steps.Add("使用 ILGenerator 创建新的标签和局部变量");
            example.Steps.Add("遍历 CodeInstruction 列表，找到要修改的位置");
            example.Steps.Add("插入、删除或替换 IL 指令");
            example.Steps.Add("使用 AccessTools.Method 和 AccessTools.Field 获取反射信息");
            example.Steps.Add("测试：验证 Transpiler 是否正确修改了方法行为");

            return example;
        }
    }
}
