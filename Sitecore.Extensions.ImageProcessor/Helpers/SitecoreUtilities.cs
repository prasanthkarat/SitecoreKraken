using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Extensions.ImageProcessor.Helpers.Cache;
using System;
using System.Collections.Generic;

namespace Sitecore.Extensions.ImageProcessor.Helpers
{
    public class SitecoreUtilities
    {
        public static string KrakenSettingsItem { get { return Settings.GetSetting(KrakenConstants.KrakenSettingsItem, ""); } }
        public static string KrakenSettings(string key)
        {
            string value = string.Empty;
            Dictionary<string, string> KrakenSettings = null;
            if (KrakenSettingsItem != null)
            {
                KrakenSettings = CacheHelper.ApplicationCache.GetOrAdd(KrakenSettingsItem, () =>
                {
                    Dictionary<string, string> settings = null;
                    Item settingItem = GetItemByIdOrPath(KrakenConstants.KrakenSettingsItem);
                    settings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    if (settingItem != null)
                    {
                        settingItem.Fields.ReadAll();
                        var fields = settingItem.Fields;
                        foreach (Field field in fields)
                        {
                            if (settings.ContainsKey(field.Name))
                                settings[field.Name] = settingItem[field.Name];
                            else
                                settings.Add(field.Name, settingItem[field.Name]);
                        }
                    }
                    return settings;
                });
            }
            if (KrakenSettings != null)
            {

                if (KrakenSettings != null && KrakenSettings.ContainsKey(key))
                {
                    value = KrakenSettings[key];
                }
            }
            return value;
        }

        public static Item GetItemByIdOrPath(string idOrPath)
        {
            Database db = GetDataBase("");
            if (null != db && !string.IsNullOrEmpty(idOrPath))
            {
                var item = db.GetItem(idOrPath);
                return item;
            }
            return null;
        }

        private static Database GetDataBase(string database)
        {
            Database db = null;

            if (!string.IsNullOrEmpty(database))
            {
                db = Factory.GetDatabase(database);
            }

            if (db == null)
            {
                db = Factory.GetDatabase(KrakenConstants.DefaultDatabase);
            }
            return db;
        }
    }
}
