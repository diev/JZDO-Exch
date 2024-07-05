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
using System.Text;

using Microsoft.Extensions.Configuration;

namespace JZDO_Exch;

internal class Program
{
    public static int ExitCode { get; set; } = 0;

    private static void Main(string[] args)
    {
        bool test = false;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // required for 1251

        IConfiguration config = Helper.GetAppConfig();

        using ConsoleTraceListener ConTracer = new() { Name = nameof(ConTracer) };
        Trace.Listeners.Add(ConTracer);

        string log = Helper.GetLogFile(config["Logs"]);
        using TextWriterTraceListener FileTracer = new(log, nameof(FileTracer));
        Trace.Listeners.Add(FileTracer);
        Trace.AutoFlush = true;

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

        try
        {
            Worker.Run(test, config);
        }
        catch (Exception ex)
        {
            Helper.TraceError("Program Error", ex);
            ExitCode = 4;
        }
        finally
        {
            Trace.Listeners.Clear();
            Environment.Exit(ExitCode);
        }
    }

    private static void Usage() //TODO
    {
        Console.WriteLine(@"Use ""-test"" to check functionality.");
        ExitCode = 1;
    }

    private static void WrongArg(string arg)
    {
        Console.WriteLine(@$"Wrong arg ""{arg}"".");
        ExitCode = 1;
    }
}
