#region License
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

using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace JZDO_Exch
{
    public static class SettingsReader
    {
        public static Settings Read(string config)
        {
            if (!File.Exists(config)) //Write default settings
            {
                JsonSerializerOptions writeOptions = new() { WriteIndented = true };
                string writeJson = JsonSerializer.Serialize(new Settings(), writeOptions);
                File.WriteAllText(config, writeJson);

                Trace.TraceWarning($"Check settings in {config}.");
                Environment.Exit(2);
            }

            string readJson = File.ReadAllText(config);
            JsonSerializerOptions readOptions = new() { AllowTrailingCommas = true };

            return JsonSerializer.Deserialize<Settings>(readJson, readOptions);
        }
    }
}
