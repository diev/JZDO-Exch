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

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Diev.Extensions.Credentials;
using Diev.Extensions.Sftp;
using Diev.Extensions.Smtp;

using Microsoft.Extensions.Configuration;

namespace JZDO_Exch;

public static class Worker
{
    public static void Run(bool test, IConfiguration config)
    {
        Console.WriteLine(test ? "Run test..." : "Run...");

        var dir = Directory.GetCurrentDirectory();
        var ymd = DateTime.Now.ToString("yyyyMMdd");
        var sftpConfig = new SftpConfig()
        {
            RemoteUploadDirectory = "/home/zdo/files/doc/in/cs/unknown",
            RemoteDownloadDirectory = "/home/zdo/files/doc/out/cs/unknown",
            LocalUploadDirectory = config["LocalOut"] ?? dir,
            LocalDownloadDirectory = config["LocalIn"] ?? dir,
            StoreUploadDirectory = Path.Combine(config["StoreOut"] ?? dir, ymd),
            StoreDownloadDirectory = Path.Combine(config["StoreIn"] ?? dir, ymd),
            DeleteRemoteDownloaded = true,
            DeleteLocalUploaded = true
        }
        .AddCredential(CredentialManager.ReadCredential("JZDO-Exch *"));
        using var sftp = new SftpService(sftpConfig);

        if (sftp.Connect())
        {
            if (test)
            {
                string testfile = "zdo_test.txt";
                string localFile = Path.Combine(sftpConfig.LocalUploadDirectory ?? dir, testfile);
                string remoteFile = "/home/zdo/files/doc/in/cs/test/" + testfile;
                File.WriteAllText(localFile, "TEST");
                bool result = sftp.UploadFile(localFile, remoteFile);

                Trace.WriteLine(result ? "Test ok." : "Test fail.");
            }
            else
            {
                int sent = sftp.UploadDirectory();

                if (sent > 0)
                {
                    Trace.WriteLine($"Sent: {sent}");
                }
                else
                {
                    Console.WriteLine("No files to send.");
                }

                int recv = sftp.DownloadDirectory();

                if (recv > 0)
                {
                    Trace.WriteLine($"Recv: {recv}");
                    StringBuilder body = new();
                    body.AppendLine($"{DateTime.Now:G} {sftpConfig.LocalDownloadDirectory}");

                    var files = new DirectoryInfo(sftpConfig.LocalDownloadDirectory ?? dir).GetFiles();

                    foreach (var file in files)
                    {
                        body.AppendLine($"> {file.Name} [{file.Length:#,##0}]");
                    }

                    var smtpConfig = new SmtpConfig()
                        .AddDefaults()
                        .AddCredential(CredentialManager.ReadCredential("SMTP *"));
                    using var smtp = new SmtpService(smtpConfig);

                    Console.WriteLine("Send messages...");

                    smtp.SendMessageAsync(
                        config["Subscribers"],
                        "New files!",
                        body.ToString()
                    ).Wait();
                }
                else
                {
                    Console.WriteLine("No files to recv.");
                }
            }
        }

        Console.WriteLine("Done.");
    }
}
