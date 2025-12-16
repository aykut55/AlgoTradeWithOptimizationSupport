using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Config
{
    /// <summary>
    /// Thread-safe JSON Konfigürasyon Yöneticisi - Singleton
    ///
    /// KULLANIM:
    ///
    /// // 1. Config dosyası yükle
    /// var config = ConfigManager.Instance.LoadConfig<AppConfig>("config/app.json");
    ///
    /// // 2. Config dosyası yükle (default değerle)
    /// var config = ConfigManager.Instance.LoadConfig("config/app.json", new AppConfig());
    ///
    /// // 3. Config kaydet
    /// ConfigManager.Instance.SaveConfig("config/app.json", config);
    ///
    /// // 4. Config güncelle (oku, değiştir, kaydet)
    /// ConfigManager.Instance.UpdateConfig<AppConfig>("config/app.json", cfg => {
    ///     cfg.SomeSetting = newValue;
    /// });
    ///
    /// // 5. Config var mı kontrol et
    /// bool exists = ConfigManager.Instance.ConfigExists("config/app.json");
    /// </summary>
    public sealed class ConfigManager
    {
        private static readonly Lazy<ConfigManager> _instance = new Lazy<ConfigManager>(() => new ConfigManager());
        private readonly ConcurrentDictionary<string, object> _configCache;
        private readonly object _lockObject = new object();

        /// <summary>
        /// JSON serialization options
        /// </summary>
        public JsonSerializerOptions JsonOptions { get; set; }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ConfigManager Instance => _instance.Value;

        private ConfigManager()
        {
            _configCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            // Default JSON options
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        // ====================================================================
        // CONFIG OKUMA
        // ====================================================================

        /// <summary>
        /// Config dosyasını yükle (generic)
        /// </summary>
        /// <typeparam name="T">Config class tipi</typeparam>
        /// <param name="filePath">Config dosya yolu</param>
        /// <param name="useCache">Cache kullan (default: true)</param>
        /// <returns>Config nesnesi</returns>
        public T? LoadConfig<T>(string filePath, bool useCache = true) where T : class
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Config dosya yolu boş olamaz.", nameof(filePath));

            string fullPath = GetFullPath(filePath);

            // Cache'den kontrol et
            if (useCache && _configCache.TryGetValue(fullPath, out var cachedConfig))
            {
                return cachedConfig as T;
            }

            // Dosya yoksa null dön
            if (!File.Exists(fullPath))
            {
                return null;
            }

            lock (_lockObject)
            {
                try
                {
                    // JSON oku ve deserialize et
                    string json = File.ReadAllText(fullPath);
                    T? config = JsonSerializer.Deserialize<T>(json, JsonOptions);

                    // Cache'e ekle
                    if (useCache && config != null)
                    {
                        _configCache[fullPath] = config;
                    }

                    return config;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Config dosyası okunamadı: {fullPath}", ex);
                }
            }
        }

        /// <summary>
        /// Config dosyasını yükle, yoksa default değer kullan
        /// </summary>
        /// <typeparam name="T">Config class tipi</typeparam>
        /// <param name="filePath">Config dosya yolu</param>
        /// <param name="defaultConfig">Default config nesnesi</param>
        /// <param name="createIfMissing">Yoksa oluştur ve kaydet (default: true)</param>
        /// <returns>Config nesnesi</returns>
        public T LoadConfig<T>(string filePath, T defaultConfig, bool createIfMissing = true) where T : class
        {
            if (defaultConfig == null)
                throw new ArgumentNullException(nameof(defaultConfig));

            var config = LoadConfig<T>(filePath);

            if (config == null)
            {
                config = defaultConfig;

                if (createIfMissing)
                {
                    SaveConfig(filePath, config);
                }
            }

            return config;
        }

        // ====================================================================
        // CONFIG YAZMA
        // ====================================================================

        /// <summary>
        /// Config dosyasını kaydet
        /// </summary>
        /// <typeparam name="T">Config class tipi</typeparam>
        /// <param name="filePath">Config dosya yolu</param>
        /// <param name="config">Config nesnesi</param>
        public void SaveConfig<T>(string filePath, T config) where T : class
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Config dosya yolu boş olamaz.", nameof(filePath));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            string fullPath = GetFullPath(filePath);

            lock (_lockObject)
            {
                try
                {
                    // Dizin yoksa oluştur
                    string? directory = Path.GetDirectoryName(fullPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // JSON serialize et ve yaz
                    string json = JsonSerializer.Serialize(config, JsonOptions);
                    File.WriteAllText(fullPath, json);

                    // Cache'i güncelle
                    _configCache[fullPath] = config;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Config dosyası kaydedilemedi: {fullPath}", ex);
                }
            }
        }

        // ====================================================================
        // CONFIG GÜNCELLEME
        // ====================================================================

        /// <summary>
        /// Config dosyasını oku, güncelle ve kaydet
        /// </summary>
        /// <typeparam name="T">Config class tipi</typeparam>
        /// <param name="filePath">Config dosya yolu</param>
        /// <param name="updateAction">Config'i güncelleyen action</param>
        /// <param name="createIfMissing">Yoksa oluştur (default: false)</param>
        public void UpdateConfig<T>(string filePath, Action<T> updateAction, bool createIfMissing = false) where T : class, new()
        {
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            lock (_lockObject)
            {
                // Config'i yükle
                T? config = LoadConfig<T>(filePath, useCache: false);

                if (config == null)
                {
                    if (!createIfMissing)
                    {
                        throw new FileNotFoundException($"Config dosyası bulunamadı: {filePath}");
                    }

                    config = new T();
                }

                // Güncelle
                updateAction(config);

                // Kaydet
                SaveConfig(filePath, config);
            }
        }

        // ====================================================================
        // YARDIMCI METODLAR
        // ====================================================================

        /// <summary>
        /// Config dosyası var mı kontrol et
        /// </summary>
        /// <param name="filePath">Config dosya yolu</param>
        /// <returns>True ise dosya var</returns>
        public bool ConfigExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            string fullPath = GetFullPath(filePath);
            return File.Exists(fullPath);
        }

        /// <summary>
        /// Config dosyasını sil
        /// </summary>
        /// <param name="filePath">Config dosya yolu</param>
        /// <returns>True ise silindi</returns>
        public bool DeleteConfig(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            string fullPath = GetFullPath(filePath);

            lock (_lockObject)
            {
                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        _configCache.TryRemove(fullPath, out _);
                        return true;
                    }
                }
                catch
                {
                    // Silme hatası - ignore
                }

                return false;
            }
        }

        /// <summary>
        /// Cache'i temizle
        /// </summary>
        public void ClearCache()
        {
            _configCache.Clear();
        }

        /// <summary>
        /// Belirli bir config'i cache'den kaldır
        /// </summary>
        /// <param name="filePath">Config dosya yolu</param>
        public void RemoveFromCache(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            string fullPath = GetFullPath(filePath);
            _configCache.TryRemove(fullPath, out _);
        }

        /// <summary>
        /// Tam dosya yolunu al (relative path'i absolute'a çevir)
        /// </summary>
        private string GetFullPath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
                return filePath;

            return Path.GetFullPath(filePath);
        }
    }
}
