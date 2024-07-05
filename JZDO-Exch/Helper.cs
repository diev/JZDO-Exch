#region License
/*
Copyright 2021-2024 Dmitrii Evdokimov
Open source software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

#define TRACE

using System;
using System.Diagnostics;
using System.IO;

using Diev.Extensions.Info;

using Microsoft.Extensions.Configuration;

namespace JZDO_Exch;

public static class Helper
{
    public static IConfiguration GetAppConfig()
    {
        string appsettings = Path.ChangeExtension(Environment.ProcessPath!, ".config.json");
        string comsettings = Path.Combine(App.CompanyData, Path.GetFileName(appsettings));

        if (File.Exists(comsettings))
        {
            Console.WriteLine(@$"ВНИМАНИЕ: Настройки в файле ""{appsettings}""");
            Console.WriteLine(@$"могут изменяться настройками в файле ""{comsettings}""!");
        }

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(appsettings, true, false) // optional app path\{appsettings}
            .AddJsonFile(comsettings, true, false) // optional C:\ProgramData\{company}\{appsettings}
            .Build();
        return config;
    }

    public static string GetLogFile(string? logDirectory)
    {
        string logs = logDirectory ?? nameof(logs);
        var now = DateTime.Now;
        var logsPath = Directory.CreateDirectory(Path.Combine(logs, now.ToString("yyyy")));
        var file = Path.Combine(logsPath.FullName, now.ToString("yyyyMMdd") + ".log");
        return file;
    }

    public static void TraceError(string message, Exception ex)
    {
        Console.WriteLine("Вывод информации об ошибке.");

        Trace.WriteLine($"{DateTime.Now:G} {message}: {ex.Message}.");
        string text = $"{DateTime.Now:G} Exception:{Environment.NewLine}{ex}{Environment.NewLine}";

        if (ex.InnerException != null)
        {
            text += $"Inner Exception:{Environment.NewLine}{ex.InnerException}{Environment.NewLine}";
        }

        File.AppendAllText("error.log", text);
    }
}
