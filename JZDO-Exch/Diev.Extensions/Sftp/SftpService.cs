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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Diev.Extensions.Sftp;

public class SftpService(SftpConfig config) : ISftpService, IDisposable
{
    private readonly SftpClient client = new(config.Host, config.Port, config.UserName, config.Password);

    public bool Connected { get; private set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Connected = false;

            if (client != null)
            {
                client.Disconnect();
                client.Dispose();
            }
        }
    }

    public bool Connect()
    {
        try
        {
            client.Connect();
            Connected = client.IsConnected;
            return Connected;
        }
        catch (Renci.SshNet.Common.SshConnectionException ex)
        {
            Trace.WriteLine($"Cannot connect to the server: {ex.Message}.");
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            Trace.WriteLine($"Unable to establish the socket: {ex.Message}.");
        }
        catch (Renci.SshNet.Common.SshAuthenticationException ex)
        {
            Trace.WriteLine($"Authentication of SSH session failed: {ex.Message}.");
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"SFTP settings failed: {ex.Message}.");
        }

        return false;
    }

    public void Disconnect()
    {
        if (client.IsConnected)
        {
            client.Disconnect();
            Connected = client.IsConnected;
        }
    }

    public bool DeleteFile(string remoteFile)
    {
        try
        {
            client.DeleteFile(remoteFile);
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(@$"Fail to delete ""{remoteFile}"": {ex.Message}.");
            return false;
        }
    }

    public bool DownloadFile(string remoteFile, string localFile)
    {
        try
        {
            using var stream = File.Create(localFile);
            client.DownloadFile(remoteFile, stream);
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(@$"Fail to download ""{localFile}"" from ""{remoteFile}"": {ex.Message}.");
            return false;
        }
    }

    public IEnumerable<ISftpFile> GetFiles(string remoteDirectory = ".")
    {
        try
        {
            return client.ListDirectory(remoteDirectory);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(@$"Fail to list ""{remoteDirectory}"": {ex.Message}.");
            return [];
        }
    }

    public bool UploadFile(string localFile, string remoteFile)
    {
        try
        {
            using var stream = File.OpenRead(localFile);
            client.UploadFile(stream, remoteFile);
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(@$"Fail to upload ""{localFile}"" to ""{remoteFile}"": {ex.Message}.");
            return false;
        }
    }

    public bool ChangeDirectory(string? remoteDirectory)
    {
        if (remoteDirectory is null || remoteDirectory.Equals(".") || remoteDirectory.Equals(client.WorkingDirectory))
        {
            return true;
        }

        try
        {
            client.ChangeDirectory(remoteDirectory);
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(@$"Fail to change remote directory to ""{remoteDirectory}"": {ex.Message}.");
            return false;
        }
    }

    public int DownloadDirectory()
    {
        if (!ChangeDirectory(config.RemoteDownloadDirectory))
        {
            return 0;
        }

        int result = 0;
        var remoteFiles = GetFiles();
        var store = config.StoreDownloadDirectory;

        foreach (var sftpFile in remoteFiles)
        {
            if (sftpFile.IsRegularFile)
            {
                string filename = sftpFile.Name;
                string remoteFile = sftpFile.FullName;
                string localFile = Path.Combine(config.LocalDownloadDirectory ?? ".", filename);

                if (DownloadFile(remoteFile, localFile))
                {
                    result++;

                    if (store != null)
                    {
                        Directory.CreateDirectory(store);
                        File.Copy(localFile, Path.Combine(store, filename), true);
                    }

                    if (config.DeleteRemoteDownloaded)
                    {
                        DeleteFile(remoteFile);
                    }
                }
            }
        }

        return result;
    }

    public int UploadDirectory()
    {
        if (!ChangeDirectory(config.RemoteUploadDirectory))
        {
            return 0;
        }

        int result = 0;
        var localFiles = new DirectoryInfo(config.LocalUploadDirectory ?? ".").GetFiles();
        var store = config.StoreUploadDirectory;

        foreach (var file in localFiles)
        {
            string filename = file.Name;
            string localFile = file.FullName;
            string remoteFile = filename;

            if (UploadFile(localFile, remoteFile))
            {
                result++;

                if (store != null)
                {
                    Directory.CreateDirectory(store);

                    if (config.DeleteLocalUploaded)
                    {
                        File.Move(localFile, Path.Combine(store, filename), true);
                    }
                    else
                    {
                        File.Copy(localFile, Path.Combine(store, filename), true);
                    }
                }
                else if (config.DeleteLocalUploaded)
                {
                    File.Delete(localFile);
                }
            }
        }

        return result;
    }
}
