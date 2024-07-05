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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Diev.Extensions.Smtp;

public class SmtpService(SmtpConfig config) : ISmtpService, IDisposable
{
    private readonly SmtpClient client = new(config.Host, config.Port)
    {
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(config.UserName, config.Password),
        EnableSsl = config.UseTls
    };

    private static readonly char[] _separator = [' ', ',', ';'];
    //private ConcurrentQueue<MailMessage> _queue = new();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && client != null)
        {
            client.Dispose();
        }
    }

    public async Task SendMessageAsync(string? emails, string? subj = "", string? body = "", string[]? files = null)
    {
        if (emails is null || emails.Length == 0 || client is null)
            return;

        try
        {
            using MailMessage mail = new()
            {
                From = new(config.UserName, config.DisplayName, Encoding.UTF8),
                Subject = subj,
                Body = config.Header + body + config.Footer
            };

            foreach (var email in emails.Split(_separator,
                StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(email);
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    FileInfo fi = new(file);

                    if (fi.Exists)
                    {
                        Attachment attachment = new(fi.FullName);
                        ContentDisposition disposition = attachment.ContentDisposition!;

                        disposition.CreationDate = fi.CreationTime;
                        disposition.ModificationDate = fi.LastWriteTime;
                        disposition.ReadDate = fi.LastAccessTime;

                        mail.Attachments.Add(attachment);
                    }
                }
            }

            await client.SendMailAsync(mail);
            //_queue.Enqueue(mail);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Sending mail '{subj}' failed: {ex.Message}.");
        }
    }
}
