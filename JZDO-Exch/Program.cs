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

#define TRACE

using System;
using System.Diagnostics;
using System.IO;

namespace JZDO_Exch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using ConsoleTraceListener ConTracer = new() { Name = nameof(ConTracer) };
            Trace.Listeners.Add(ConTracer);
            Trace.AutoFlush = true;

            try
            {
                Settings settings = SettingsReader.Read("appsettings.json");
                bool test = false;

                foreach (var arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "--test":
                        case "-test":
                        case "/test":
                            test = true;
                            break;

                        default:
                            if (arg.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                settings = SettingsReader.Read(arg);
                            }
                            break;
                    }
                }

                var logs = Directory.CreateDirectory(Path.Combine(settings.Logs, $"{DateTime.Now:yyyy}"));
                var file = Path.Combine(logs.FullName, $"{DateTime.Now:yyyyMMdd}.log");

                using TextWriterTraceListener FileTracer = new(file, nameof(FileTracer));
                Trace.Listeners.Add(FileTracer);

                //Trace.WriteLine($"{DateTime.Now:G} Start");
                Worker.Run(settings, test);
                //Trace.WriteLine($"{DateTime.Now:G} End");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:G} Error");
                Trace.TraceError(ex.Message);

                File.AppendAllText("error.log", ex.ToString());
            }
        }
    }
}