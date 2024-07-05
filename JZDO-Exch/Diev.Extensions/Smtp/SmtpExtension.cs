#region License
/*
Copyright 2022-2024 Dmitrii Evdokimov
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

using Diev.Extensions.Credentials;
using Diev.Extensions.Info;

namespace Diev.Extensions.Smtp;

public static class SmtpExtension
{
    public static SmtpConfig AddDefaults(this SmtpConfig config)
    {
        config.DisplayName = $"{App.Name} {Environment.MachineName}";
        config.Footer = $"{Environment.NewLine}--{Environment.NewLine}{App.Title}";
        return config;
    }

    public static SmtpConfig AddCredential(this SmtpConfig config, Credential windowsCredential)
    {
        //string filter = "SMTP *";
        //var cred = CredentialManager.ReadCredential(filter);
        string name = windowsCredential.TargetName;

        try
        {
            var p = name.Split(' ');
            config.Host = p[1];

            if (p.Length > 2)
            {
                config.Port = int.Parse(p[2]);
            }

            config.UseTls = name.EndsWith("tls", StringComparison.OrdinalIgnoreCase);
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
