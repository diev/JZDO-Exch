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

using System.Collections.Generic;

using Renci.SshNet.Sftp;

namespace Diev.Extensions.Sftp;

public interface ISftpService
{
    bool Connect();
    void Disconnect();
    bool ChangeDirectory(string remoteDirectory);
    IEnumerable<ISftpFile> GetFiles(string remoteDirectory = ".");
    bool DeleteFile(string remoteFile);
    bool UploadFile(string localFile, string remoteFile);
    bool DownloadFile(string remoteFile, string localFile);
    int UploadDirectory();
    int DownloadDirectory();
}
