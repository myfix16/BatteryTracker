// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace BatteryTracker.Controls
{
    /// <summary>
    /// Represents a control that can contain multiple settings (or other) controls
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
    public sealed class SettingsGroup : ItemsControl
    {
        private const string PartDescriptionPresenter = "DescriptionPresenter";
        private ContentPresenter? _descriptionPresenter;
        private SettingsGroup? _settingsGroup;

        public SettingsGroup()
        {
            DefaultStyleKey = typeof(SettingsGroup);
        }

        [Localizable(true)]
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(SettingsGroup),
            new PropertyMetadata(default(string)));

        [Localizable(true)]
        public object? Description
        {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(object),
            typeof(SettingsGroup),
            new PropertyMetadata(null, OnDescriptionChanged));

        protected override void OnApplyTemplate()
        {
            IsEnabledChanged -= SettingsGroup_IsEnabledChanged;
            _settingsGroup = this;
            _descriptionPresenter = (ContentPresenter)_settingsGroup.GetTemplateChild(PartDescriptionPresenter);
            SetEnabledState();
            IsEnabledChanged += SettingsGroup_IsEnabledChanged;
            base.OnApplyTemplate();
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsGroup)d).Update();
        }

        private void SettingsGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetEnabledState();
        }

        private void SetEnabledState()
        {
            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
        }

        private void Update()
        {
            if (_settingsGroup == null)
            {
                return;
            }

            if (_settingsGroup._descriptionPresenter != null)
            {
                _settingsGroup._descriptionPresenter.Visibility = _settingsGroup.Description == null
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SettingsGroupAutomationPeer(this);
        }
    }
}
