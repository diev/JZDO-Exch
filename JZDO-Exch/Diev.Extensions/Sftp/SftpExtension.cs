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

using Diev.Extensions.Credentials;
using System;

namespace Diev.Extensions.Sftp;

public static class SftpExtension
{
    public static SftpConfig AddCredential(this SftpConfig config, Credential windowsCredential)
    {
        string name = windowsCredential.TargetName;

        try
        {
            var p = name.Split(' ');
            config.Host = p[1];

            if (p.Length > 2)
            {
                config.Port = int.Parse(p[2]);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Windows Credential Manager '{name}' has wrong format.", ex);
        }

        config.UserName = windowsCredential.UserName
            ?? throw new Exception($"Windows Credential Manager '{name}' has no UserName.");
        config.Password = windowsCredential.Password
            ?? throw new Exception($"Windows Credential Manager '{name}' has no Password.");

        return config;
    }
}
