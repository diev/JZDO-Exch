#region License
/*
Copyright 2021-2025 Dmitrii Evdokimov
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
using System.Collections.Generic;
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

        var section = config.GetSection("Subscribers");
        List<string> list = [];
        foreach (var item in section.GetChildren())
        {
            list.Add(item.Value!);
        }
        string[] emails = [.. list];

        //Mailer.SendMessage(emails, "New files test!", "TEST");

        var dir = Directory.GetCurrentDirectory();
        var ymd = DateTime.Now.ToString("yyyyMMdd");

        var cred = CredentialManager.ReadCredential("JZDO-Exch *");
        var p = cred.TargetName.Split(' ');

        var sftpConfig = new SftpConfig
        {
            Host = p[1],
            Port = p.Length > 2 ? int.Parse(p[2]) : 22,
            UserName = cred.UserName!,
            Password = cred.Password!,

            RemoteUploadDirectory = "/home/zdo/files/doc/out/cs/unknown",
            RemoteDownloadDirectory = "/home/zdo/files/doc/in/cs/unknown",

            LocalUploadDirectory = config["LocalOut"] ?? dir,
            LocalDownloadDirectory = config["LocalIn"] ?? dir,

            StoreUploadDirectory = Path.Combine(config["StoreOut"] ?? dir, ymd),
            StoreDownloadDirectory = Path.Combine(config["StoreIn"] ?? dir, ymd),

            DeleteRemoteDownloaded = true,
            DeleteLocalUploaded = true
        };
        //.AddCredential(CredentialManager.ReadCredential("JZDO-Exch *"));

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
                //int sent = sftp.UploadDirectory();
                sftp.UploadDirectory();

                //if (sent > 0)
                //{
                //    Trace.WriteLine($"Sent: {sent}");
                //}
                //else
                //{
                //    Console.WriteLine("No files to send.");
                //}

                int recv = sftp.DownloadDirectory();

                if (recv > 0)
                {
                    //Trace.WriteLine($"Recv: {recv}");
                    StringBuilder body = new();
                    body.AppendLine($"{DateTime.Now:G} {sftpConfig.LocalDownloadDirectory}");

                    //TODO attach log only?
                    var files = new DirectoryInfo(sftpConfig.LocalDownloadDirectory).GetFiles();

                    foreach (var file in files)
                    {
                        body.AppendLine($"> {file.Name} [{file.Length:#,##0}]");
                    }

                    Console.WriteLine("Send messages...");

                    Mailer.SendMessage(emails, "New files!", body.ToString());
                }
                else
                {
                    //Console.WriteLine("No files to recv.");
                }
            }
        }

        Console.WriteLine("Done.");
    }
}
