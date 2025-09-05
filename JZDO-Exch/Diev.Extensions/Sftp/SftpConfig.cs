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

namespace Diev.Extensions.Sftp;

public class SftpConfig
{
    public required string Host { get; set; }
    public int Port { get; set; } = 22;
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public required string RemoteUploadDirectory { get; set; }
    public required string RemoteDownloadDirectory { get; set; }
    public required string LocalUploadDirectory { get; set; }
    public required string LocalDownloadDirectory { get; set; }
    public required string StoreUploadDirectory { get; set; }
    public required string StoreDownloadDirectory { get; set; }

    public bool DeleteLocalUploaded { get; set; }
    public bool DeleteRemoteDownloaded { get; set; }
}
