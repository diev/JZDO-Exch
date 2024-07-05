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

namespace Diev.Extensions.Sftp;

public class SftpConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string? RemoteUploadDirectory { get; set; }
    public string? RemoteDownloadDirectory { get; set; }
    public string? LocalUploadDirectory { get; set; }
    public string? LocalDownloadDirectory { get; set; }
    public string? StoreUploadDirectory { get; set; }
    public string? StoreDownloadDirectory { get; set; }

    public bool DeleteLocalUploaded { get; set; }
    public bool DeleteRemoteDownloaded { get; set; }
}
