using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RimWorldModDevProbe.Core
{
    public class ProbeContext
    {
        public string GameDllPath { get; private set; }
        public string GameDataPath { get; private set; }
        public string ModsPath { get; private set; }

        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies = new ConcurrentDictionary<string, Assembly>();

        public List<Assembly> LoadedAssemblies => _loadedAssemblies.Values.ToList();

        public ProbeContext()
        {
            InitializePaths();
        }

        private void InitializePaths()
        {
            var config = ProbeConfig.Load();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));

            if (!string.IsNullOrEmpty(config.GamePath) && Directory.Exists(config.GamePath))
            {
                GameDataPath = Path.Combine(config.GamePath, "Data");
                if (!string.IsNullOrEmpty(config.ModsPath) && Directory.Exists(config.ModsPath))
                {
                    ModsPath = config.ModsPath;
                }
                else
                {
                    ModsPath = Path.Combine(config.GamePath, "Mods");
                }
            }
            else
            {
                GameDataPath = FindPath(baseDir, "Data", new[] {
                    Path.Combine(projectDir, "Data"),
                    @"..\..\..\..\..\Data",
                    @"..\..\..\Data"
                });
                ModsPath = FindPath(baseDir, "Mods", new[] {
                    Path.Combine(projectDir, "Mods"),
                    @"..\..\..\..\..\Mods",
                    @"..\..\..\Mods"
                });
            }

            GameDllPath = FindPath(baseDir, "GameDll", new[] {
                Path.Combine(projectDir, "GameDll"),
                @"..\..\..\..\GameDll",
                @"..\GameDll",
                "GameDll"
            });

            if (!File.Exists(ProbeConfig.GetConfigPath()))
            {
                ProbeConfig.CreateDefault();
            }

            LoadModAssemblies();
        }

        private string FindPath(string baseDir, string name, string[] relativePaths)
        {
            foreach (var relPath in relativePaths)
            {
                var path = Path.GetFullPath(Path.Combine(baseDir, relPath));
                if (Directory.Exists(path)) return path;
            }
            return null;
        }

        public void LoadGameAssemblies()
        {
            if (GameDllPath == null || !Directory.Exists(GameDllPath)) return;

            foreach (var dll in Directory.GetFiles(GameDllPath, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    _loadedAssemblies[asm.FullName] = asm;
                }
                catch { }
            }
        }

        public void LoadModAssemblies()
        {
            if (ModsPath == null || !Directory.Exists(ModsPath)) return;

            var modDirs = Directory.GetDirectories(ModsPath);
            foreach (var modDir in modDirs)
            {
                var assembliesDir = Path.Combine(modDir, "Assemblies");
                if (Directory.Exists(assembliesDir))
                {
                    LoadDllsFromDirectory(assembliesDir);
                }

                foreach (var subDir in Directory.GetDirectories(modDir))
                {
                    var dirName = Path.GetFileName(subDir);
                    if (IsVersionDirectory(dirName))
                    {
                        var versionAssembliesDir = Path.Combine(subDir, "Assemblies");
                        if (Directory.Exists(versionAssembliesDir))
                        {
                            LoadDllsFromDirectory(versionAssembliesDir);
                        }
                    }
                }
            }
        }

        private bool IsVersionDirectory(string dirName)
        {
            if (string.IsNullOrEmpty(dirName)) return false;
            var parts = dirName.Split('.');
            if (parts.Length < 1) return false;
            foreach (var part in parts)
            {
                if (!float.TryParse(part, out _)) return false;
            }
            return true;
        }

        private void LoadDllsFromDirectory(string directory)
        {
            foreach (var dll in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    _loadedAssemblies[asm.FullName] = asm;
                }
                catch { }
            }
        }

        public T GetOrAddCache<T>(string key, Func<T> factory)
        {
            return (T)_cache.GetOrAdd(key, k => factory());
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        public bool TryGetCache<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out var obj))
            {
                value = (T)obj;
                return true;
            }
            value = default;
            return false;
        }

        public void SetCache<T>(string key, T value)
        {
            _cache[key] = value;
        }
    }
}
