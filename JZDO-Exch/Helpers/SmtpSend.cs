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
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace JZDO_Exch.Helpers;

public class SmtpSend : IDisposable
{
    private readonly SmtpClient? _client;

    public SmtpSend()
    {
        try
        {
            _client = new(Config.Smtp.Host, Config.Smtp.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Config.Smtp.User,
                    Encoding.UTF8.GetString(Convert.FromBase64String(Config.Smtp.Pass))),
                EnableSsl = Config.Smtp.Tls
            };
        }
        catch (Exception)
        {
            Trace.WriteLine("SMTP settings failed.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _client != null)
        {
            _client.Dispose();
        }
    }

    public void SendMessage(string subj, StringBuilder body)
    {
        SendMessage(subj, body.ToString());
    }

    public void SendMessage(string subj, string body)
    {
        if (_client is null)
        {
            Trace.WriteLine("SMTP client failed.");
            return;
        }

        MailMessage mail = new()
        {
            From = new(Config.Smtp.User, Config.Smtp.Name, Encoding.UTF8),
            Subject = subj,
            Body = body
        };

        foreach (var email in Config.Smtp.Subscribers)
        {
            mail.To.Add(email);
        }

        _client.Send(mail);
    }
}
