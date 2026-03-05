using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class SoundExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetDeathSoundExample());
            examples.Add(GetDamageSoundExample());
            examples.Add(GetCustomSoundExample());

            return examples;
        }

        private static Example GetDeathSoundExample()
        {
            var example = new Example
            {
                Title = "死亡音效修改示例",
                Description = "修改 Pawn 死亡时播放的音效，通过 Harmony Prefix 拦截死亡方法并播放自定义音效。",
                Feature = "音效修改",
                Keywords = new List<string> { "死亡音效", "death sound", "pawn death" }
            };

            example.Files.Add(new ExampleFile(
                "PawnDeathSoundPatch.cs",
                "Source/PawnDeathSoundPatch.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace CustomDeathSound
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(""Kill"")]
    public static class Pawn_DeathSoundPatch
    {
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff hediff)
        {
            if (__instance == null || __instance.Dead)
            {
                return true;
            }

            if (__instance.RaceProps == null || !__instance.RaceProps.Humanlike)
            {
                return true;
            }

            SoundDef customDeathSound = DefDatabase<SoundDef>.GetNamedSilentFail(""CustomHumanDeathSound"");
            
            if (customDeathSound != null)
            {
                SoundInfo soundInfo = SoundInfo.InMap(__instance, MaintenanceType.None);
                customDeathSound.PlayOneShot(soundInfo);
            }

            return true;
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "CustomDeathSoundDef.xml",
                "Defs/SoundDefs/CustomDeathSoundDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <SoundDef>
        <defName>CustomHumanDeathSound</defName>
        <context>MapOnly</context>
        <eventNames />
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Death/death_sound_1</clipPath>
                    </li>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Death/death_sound_2</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>30</min>
                    <max>40</max>
                </volumeRange>
                <pitchRange>
                    <min>0.9</min>
                    <max>1.1</max>
                </pitchRange>
                <repeatMode>NeverTwice</repeatMode>
            </li>
        </subSounds>
    </SoundDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "About.xml",
                "About/About.xml",
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModMetaData>
    <name>Custom Death Sound</name>
    <author>YourName</author>
    <packageId>YourName.CustomDeathSound</packageId>
    <description>修改角色死亡时的音效</description>
    <supportedVersions>
        <li>1.5</li>
    </supportedVersions>
    <modDependencies>
        <li>
            <packageId>brrainz.harmony</packageId>
            <displayName>Harmony</displayName>
            <steamWorkshopUrl>steam://url/CommunityFilePage/2009463077</steamWorkshopUrl>
        </li>
    </modDependencies>
    <loadAfter>
        <li>brrainz.harmony</li>
    </loadAfter>
</ModMetaData>",
                FileType.Xml
            ));

            example.Steps.Add("创建 C# 项目，添加对 Assembly-CSharp.dll 和 UnityEngine.dll 的引用");
            example.Steps.Add("添加 0Harmony.dll 或 HarmonyX 引用");
            example.Steps.Add("创建 PawnDeathSoundPatch.cs 文件，实现 Harmony Prefix Patch");
            example.Steps.Add("在 Defs/SoundDefs/ 目录下创建自定义 SoundDef XML 文件");
            example.Steps.Add("将音效文件(.wav/.ogg/.mp3)放置在 Sounds/Custom/Death/ 目录下");
            example.Steps.Add("编译项目，将生成的 DLL 放入 Assemblies/ 目录");
            example.Steps.Add("创建 About.xml 文件，声明 Mod 依赖关系");
            example.Steps.Add("测试：在游戏中让角色死亡，验证自定义音效是否播放");

            return example;
        }

        private static Example GetDamageSoundExample()
        {
            var example = new Example
            {
                Title = "受伤音效修改示例",
                Description = "修改 Pawn 受伤时播放的音效，通过 Harmony Postfix 在受伤方法执行后播放自定义音效。",
                Feature = "音效修改",
                Keywords = new List<string> { "受伤音效", "damage sound", "受伤" }
            };

            example.Files.Add(new ExampleFile(
                "PawnDamageSoundPatch.cs",
                "Source/PawnDamageSoundPatch.cs",
                @"using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace CustomDamageSound
{
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch(""TakeDamage"")]
    public static class Pawn_HealthTracker_DamageSoundPatch
    {
        private static Dictionary<int, float> lastDamageTime = new Dictionary<int, float>();

        public static void Postfix(Pawn_HealthTracker __instance, DamageInfo dinfo, Pawn ___pawn)
        {
            if (___pawn == null || ___pawn.Dead)
            {
                return;
            }

            if (dinfo.Def == null)
            {
                return;
            }

            int pawnId = ___pawn.thingIDNumber;
            float currentTime = Time.time;

            if (lastDamageTime.ContainsKey(pawnId))
            {
                if (currentTime - lastDamageTime[pawnId] < 0.5f)
                {
                    return;
                }
            }

            lastDamageTime[pawnId] = currentTime;

            SoundDef customDamageSound = GetDamageSoundForDamageType(dinfo.Def);
            
            if (customDamageSound != null)
            {
                SoundInfo soundInfo = SoundInfo.InMap(___pawn, MaintenanceType.None);
                customDamageSound.PlayOneShot(soundInfo);
            }
        }

        private static SoundDef GetDamageSoundForDamageType(DamageDef damageDef)
        {
            if (damageDef == null) return null;

            string soundDefName = $""Custom_{damageDef.defName}_DamageSound"";
            return DefDatabase<SoundDef>.GetNamedSilentFail(soundDefName);
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "CustomDamageSoundDef.xml",
                "Defs/SoundDefs/CustomDamageSoundDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <SoundDef>
        <defName>Custom_Bullet_DamageSound</defName>
        <context>MapOnly</context>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Damage/bullet_hit_1</clipPath>
                    </li>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Damage/bullet_hit_2</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>20</min>
                    <max>30</max>
                </volumeRange>
                <pitchRange>
                    <min>0.95</min>
                    <max>1.05</max>
                </pitchRange>
            </li>
        </subSounds>
    </SoundDef>

    <SoundDef>
        <defName>Custom_Burn_DamageSound</defName>
        <context>MapOnly</context>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Damage/burn_sizzle</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>25</min>
                    <max>35</max>
                </volumeRange>
            </li>
        </subSounds>
    </SoundDef>

    <SoundDef>
        <defName>Custom_HeavyDamageSound</defName>
        <context>MapOnly</context>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Damage/heavy_damage_scream</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>35</min>
                    <max>45</max>
                </volumeRange>
                <pitchRange>
                    <min>0.85</min>
                    <max>1.15</max>
                </pitchRange>
            </li>
        </subSounds>
    </SoundDef>
</Defs>",
                FileType.Xml
            ));

            example.Steps.Add("创建 Harmony Patch 类，拦截 Pawn_HealthTracker.TakeDamage 方法");
            example.Steps.Add("实现音效冷却机制，避免音效过于频繁播放");
            example.Steps.Add("根据伤害类型(DamageDef)动态选择音效");
            example.Steps.Add("创建对应的 SoundDef XML 文件，命名格式: Custom_{DamageDefName}_DamageSound");
            example.Steps.Add("将音效文件放置在 Sounds/Custom/Damage/ 目录下");
            example.Steps.Add("编译并测试：在游戏中让角色受伤，验证音效是否正确播放");

            return example;
        }

        private static Example GetCustomSoundExample()
        {
            var example = new Example
            {
                Title = "自定义音效添加示例",
                Description = "添加全新的自定义音效，并在代码中播放。包括创建 SoundDef、音效文件放置和代码调用。",
                Feature = "音效添加",
                Keywords = new List<string> { "自定义音效", "custom sound", "添加音效", "音效添加" }
            };

            example.Files.Add(new ExampleFile(
                "CustomSoundPlayer.cs",
                "Source/CustomSoundPlayer.cs",
                @"using Verse;
using RimWorld;
using UnityEngine;

namespace CustomSoundMod
{
    public static class CustomSoundPlayer
    {
        public static void PlayCustomSound(string soundDefName, Thing thing)
        {
            if (thing == null || thing.Map == null)
            {
                return;
            }

            SoundDef sound = DefDatabase<SoundDef>.GetNamedSilentFail(soundDefName);
            if (sound == null)
            {
                Log.Warning($""[CustomSound] SoundDef '{soundDefName}' not found."");
                return;
            }

            SoundInfo soundInfo = SoundInfo.InMap(thing, MaintenanceType.None);
            sound.PlayOneShot(soundInfo);
        }

        public static void PlayCustomSoundAtPosition(string soundDefName, IntVec3 position, Map map)
        {
            if (map == null)
            {
                return;
            }

            SoundDef sound = DefDatabase<SoundDef>.GetNamedSilentFail(soundDefName);
            if (sound == null)
            {
                Log.Warning($""[CustomSound] SoundDef '{soundDefName}' not found."");
                return;
            }

            TargetInfo target = new TargetInfo(position, map);
            SoundInfo soundInfo = SoundInfo.InMap(target, MaintenanceType.None);
            sound.PlayOneShot(soundInfo);
        }

        public static void PlayUISound(string soundDefName)
        {
            SoundDef sound = DefDatabase<SoundDef>.GetNamedSilentFail(soundDefName);
            if (sound == null)
            {
                Log.Warning($""[CustomSound] SoundDef '{soundDefName}' not found."");
                return;
            }

            sound.PlayOneShotOnCamera();
        }
    }

    public class CustomSoundDefOf
    {
        public static SoundDef CustomEventSound;
        public static SoundDef CustomAmbientSound;
        public static SoundDef CustomUISound;

        public static void ResolveReferences()
        {
            CustomEventSound = DefDatabase<SoundDef>.GetNamedSilentFail(""CustomEventSound"");
            CustomAmbientSound = DefDatabase<SoundDef>.GetNamedSilentFail(""CustomAmbientSound"");
            CustomUISound = DefDatabase<SoundDef>.GetNamedSilentFail(""CustomUISound"");
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "CustomSoundDef.xml",
                "Defs/SoundDefs/CustomSoundDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <SoundDef>
        <defName>CustomEventSound</defName>
        <context>MapOnly</context>
        <eventNames />
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Events/event_sound</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>40</min>
                    <max>50</max>
                </volumeRange>
                <pitchRange>
                    <min>0.9</min>
                    <max>1.1</max>
                </pitchRange>
                <repeatMode>NeverTwice</repeatMode>
            </li>
        </subSounds>
    </SoundDef>

    <SoundDef>
        <defName>CustomAmbientSound</defName>
        <context>MapOnly</context>
        <sustain>true</sustain>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Ambient/ambient_loop</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>15</min>
                    <max>25</max>
                </volumeRange>
                <pitchRange>
                    <min>1</min>
                    <max>1</max>
                </pitchRange>
                <sustainLoop>true</sustainLoop>
            </li>
        </subSounds>
    </SoundDef>

    <SoundDef>
        <defName>CustomUISound</defName>
        <context>MapOnly</context>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/UI/ui_click</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>30</min>
                    <max>35</max>
                </volumeRange>
            </li>
        </subSounds>
    </SoundDef>

    <SoundDef>
        <defName>CustomMusicTrack</defName>
        <context>MapOnly</context>
        <subSounds>
            <li>
                <grains>
                    <li Class=""AudioGrain_Clip"">
                        <clipPath>Custom/Music/custom_music_track</clipPath>
                    </li>
                </grains>
                <volumeRange>
                    <min>50</min>
                    <max>50</max>
                </volumeRange>
                <pitchRange>
                    <min>1</min>
                    <max>1</max>
                </pitchRange>
            </li>
        </subSounds>
    </SoundDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "SoundFilePlacement.txt",
                "Sounds/README.txt",
                @"音效文件放置说明
==================

1. 目录结构:
   YourMod/
   └── Sounds/
       └── Custom/
           |-- Events/
           |   -- event_sound.wav
           |-- Ambient/
           |   -- ambient_loop.ogg
           |-- UI/
           |   -- ui_click.wav
           -- Music/
               -- custom_music_track.ogg

2. 支持的音频格式:
   - .wav (推荐用于短音效)
   - .ogg (推荐用于音乐和循环音效)
   - .mp3 (支持但不推荐)

3. 音频质量建议:
   - 短音效: 44100 Hz, 16-bit, Mono
   - 音乐: 44100 Hz, 16-bit, Stereo
   - 循环音效: 确保首尾平滑过渡

4. 文件大小建议:
   - 短音效: < 100KB
   - 音乐: < 5MB
   - 循环音效: < 500KB

5. 注意事项:
   - 文件名必须与 XML 中的 clipPath 匹配
   - 不要使用特殊字符或空格
   - 建议使用小写字母和下划线
",
                FileType.Text
            ));

            example.Steps.Add("准备音效文件(.wav 或 .ogg 格式)");
            example.Steps.Add("在 Mod 目录下创建 Sounds/Custom/ 子目录结构");
            example.Steps.Add("将音效文件放置到对应的子目录中");
            example.Steps.Add("创建 SoundDef XML 文件，定义音效属性");
            example.Steps.Add("在代码中使用 DefDatabase<SoundDef>.GetNamed() 获取 SoundDef");
            example.Steps.Add("使用 SoundDef.PlayOneShot() 或 PlayOneShotOnCamera() 播放音效");
            example.Steps.Add("测试：在游戏中触发播放条件，验证音效是否正确播放");

            return example;
        }
    }
}
