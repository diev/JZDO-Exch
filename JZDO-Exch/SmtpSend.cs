using System;
using System.Collections.Generic;
using System.Linq;
#region License
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

using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JZDO_Exch
{
    public class SmtpSend : IDisposable
    {
        private readonly SmtpSettings _settings;
        private readonly SmtpClient _client = null;

        public SmtpSend(SmtpSettings settings)
        {
            _settings = settings;

            _client = new(_settings.Host, _settings.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.User,
                    Encoding.UTF8.GetString(Convert.FromBase64String(_settings.Pass))),
                EnableSsl = _settings.Tls
            };
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }

        public void SendMessage(string subj, StringBuilder body)
        {
            SendMessage(subj, body.ToString());
        }

        public void SendMessage(string subj, string body)
        {
            MailMessage mail = new();

            mail.From = new(_settings.User, _settings.Name, Encoding.UTF8);
            foreach (var email in _settings.Subscribers)
            {
                mail.To.Add(email);
            }
            mail.Subject = subj;
            mail.Body = body;

            _client.Send(mail);
        }
    }
}
