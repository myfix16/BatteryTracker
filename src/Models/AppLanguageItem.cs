using System.Globalization;
using BatteryTracker.Helpers;

namespace BatteryTracker.Models;

public class AppLanguageItem
{
    public string LanguageId { get; }

    public string LanguageName { get; }

    public AppLanguageItem(string languageId)
    {
        if (!string.IsNullOrEmpty(languageId))
        {
            var info = new CultureInfo(languageId);
            LanguageId = info.Name;
            LanguageName = info.NativeName;
        }
        else
        {
            LanguageId = string.Empty;
            var systemDefaultLanguageOptionStr = "SettingsPreferencesSystemDefaultLanguageOption".Localized();

            LanguageName = string.IsNullOrEmpty(systemDefaultLanguageOptionStr) ? "System default" : systemDefaultLanguageOptionStr;
        }
    }

    public override string ToString() => LanguageName;
}
