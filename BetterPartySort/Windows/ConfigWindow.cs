﻿using System;
using System.Numerics;
using BetterPartySort.Config;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BetterPartySort.Windows;

public class ConfigWindow : Window, IDisposable {
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("BetterPartySort settings###With a constant ID") {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(300, 90);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw() {
        // Flags must be added or removed before Draw() is being called, or they won't apply
    }

    public override void Draw() {
        // can't ref a property, so use a local copy
        var configValue = Configuration.SortOnDutyCommence;
        if (ImGui.Checkbox("Sort party list upon duty commencement", ref configValue)) {
            Configuration.SortOnDutyCommence = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            Configuration.Save();
        }
    }
}
