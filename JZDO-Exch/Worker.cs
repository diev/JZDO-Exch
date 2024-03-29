﻿#region License
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

using JZDO_Exch.Helpers;

using System;
using System.IO;
using System.Text;

namespace JZDO_Exch;

public static class Worker
{
    public static void Run(bool test)
    {
        using ExchangePoint exchPoint = new();

        if (exchPoint.Connected)
        {
            if (test)
            {
                exchPoint.SelfTest(Config.Sftp.TestRemoteIn);

                exchPoint.SendFiles(Config.Sftp.TestRemoteOut);
                exchPoint.ReceiveFiles(Config.Sftp.TestRemoteIn);
            }
            else
            {
                exchPoint.SendFiles(Config.Sftp.RemoteOut);
                exchPoint.ReceiveFiles(Config.Sftp.RemoteIn);
            }

            if (exchPoint.NumReceived > 0)
            {
                StringBuilder body = new();
                body.AppendLine($"{DateTime.Now:G} {Config.Sftp.LocalIn}");

                var files = new DirectoryInfo(Config.Sftp.LocalIn).GetFiles();

                foreach (var file in files)
                {
                    body.AppendLine($"> {file.Name} [{file.Length:#,##0}]");
                }

                using SmtpSend smtp = new();
                smtp.SendMessage("New files!", body);
            }
        }
    }
}
