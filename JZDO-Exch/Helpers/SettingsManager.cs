﻿#region License
//------------------------------------------------------------------------------
// Copyright (c) Dmitrii Evdokimov
// Source https://github.com/diev/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//------------------------------------------------------------------------------
#endregion

using JZDO_Exch.AppSettings;

using System.IO;
using System.Text.Json;

namespace JZDO_Exch.Helpers;

public static class SettingsManager
{
    public static string FileName { get; set; } = "appsettings.json";

    public static Settings? Read(string? filename = null)
    {
        if (!string.IsNullOrEmpty(filename))
        {
            FileName = filename;
        }

        string readJson = File.ReadAllText(FileName);
        JsonSerializerOptions readOptions = new() { AllowTrailingCommas = true };
        return JsonSerializer.Deserialize<Settings>(readJson, readOptions);
    }

    public static void Write(Settings settings)
    {
        Write(settings, FileName);
    }

    public static void Write(Settings settings, string? filename = null)
    {
        if (string.IsNullOrEmpty(filename))
        {
            filename = FileName;
        }

        JsonSerializerOptions writeOptions = new() { WriteIndented = true };
        string writeJson = JsonSerializer.Serialize(settings, writeOptions);
        File.WriteAllText(filename, writeJson);
    }

    public static void WriteDefault(string? filename = null)
    {
        Write(new Settings(), filename);
    }
}
