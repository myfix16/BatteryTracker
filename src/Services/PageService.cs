using System.Collections.Generic;
using System.Linq;
using BatteryTracker.Contracts.Services;
using BatteryTracker.ViewModels;
using BatteryTracker.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace BatteryTracker.Services;

public sealed class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<SettingsViewModel, SettingsPage>();
        Configure<AboutViewModel, AboutPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            string key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            Type type = typeof(V);
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException(
                    $"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
