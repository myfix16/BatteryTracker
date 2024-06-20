// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Automation.Peers;

namespace BatteryTracker.Controls
{
    public sealed class SettingsGroupAutomationPeer(SettingsGroup owner) : FrameworkElementAutomationPeer(owner)
    {
        protected override string GetNameCore()
        {
            var selectedSettingsGroup = (SettingsGroup)Owner;
            return selectedSettingsGroup.Header;
        }
    }
}
