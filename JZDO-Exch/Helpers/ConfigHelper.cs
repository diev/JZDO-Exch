#region License
//------------------------------------------------------------------------------
// Copyright (c) Dmitrii Evdokimov
// Open ource software https://github.com/diev/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//------------------------------------------------------------------------------
#endregion

using System;
using System.Text.Json;

namespace JZDO_Exch.Helpers;

public static class ConfigHelper
{
    public static string GetString(string name, string value = "") => 
        AppContext.GetData(name) as string ?? value;

    public static int GetInt(string name, int value = 0)
    {
        var data = AppContext.GetData(name);

        if (data is null)
            return value;

        if (data is String)
            return int.Parse((string)data);

        return (int)data;
    }

    public static bool GetBool(string name, bool value = false)
    {
        var data = AppContext.GetData(name);

        if (data is null)
            return value;

        if (data is String)
            return bool.Parse((string)data);

        return (bool)data;
    }

    public static string[] GetList(string name, string[]? value = null)
    {
        var list = AppContext.GetData(name);

        if (list is null)
            return value is null
                ? Array.Empty<string>()
                : value;

        if (list is String)
        {
            var items = JsonSerializer.Deserialize<string[]>((string)list);

            if (items is null)
                return value is null
                    ? Array.Empty<string>()
                    : value;

            return items;
        }

        return (string[])list;
    }
}
