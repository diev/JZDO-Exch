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

#define TRACE

using System;
using System.Diagnostics;
using System.IO;

namespace JZDO_Exch;

internal class Program
{
    private static void Main(string[] args)
    {
        using ConsoleTraceListener ConTracer = new() { Name = nameof(ConTracer) };
        Trace.Listeners.Add(ConTracer);
        Trace.AutoFlush = true;

        try
        {
            bool test = false;

            foreach (var arg in args)
            {
                if (arg[0] == '/' || arg[0] == '-')
                {
                    string a = arg.ToLower()[1..];

                    if (arg.StartsWith("--"))
                    {
                        a = a[1..];
                    }

                    switch (a)
                    {
                        case "?":
                        case "h":
                        case "help":
                            Usage();
                            break;

                        case "test":
                            test = true;
                            break;

                        default:
                            WrongArg(arg);
                            break;
                    }
                }
                else
                {
                    WrongArg(arg);
                }
            }

            var logsPath = Config.Logs;
            var logs = Directory.CreateDirectory(Path.Combine(logsPath, $"{DateTime.Now:yyyy}"));
            var file = Path.Combine(logs.FullName, $"{DateTime.Now:yyyyMMdd}.log");

            using TextWriterTraceListener FileTracer = new(file, nameof(FileTracer));
            Trace.Listeners.Add(FileTracer);

            Worker.Run(test);
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"{DateTime.Now:G} Error");
            Trace.TraceError(ex.Message);

            File.AppendAllText("error.log", ex.ToString());
            Environment.Exit(4);
        }
    }

    private static void Usage() //TODO
    {
        Console.WriteLine("Use \"-test\" to check functionality.");
        Environment.Exit(1);
    }

    private static void WrongArg(string arg)
    {
        Console.WriteLine($"Wrong arg \"{arg}\".");
        Environment.Exit(1);
    }
}
