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

    public bool Connected => client.IsConnected;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && client != null)
        {
            client.Disconnect();
        }
    }

    public bool Connect()
    {
        try
        {
            client.Connect();
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

        bool result = Connected;
        Log(result, $"Connect to {config.Host}:{config.Port} as {config.UserName}");
        return result;
    }

    public void Disconnect()
    {
        if (client.IsConnected)
        {
            client.Disconnect();
        }
    }

    public bool DeleteFile(string remoteFile)
    {
        bool result = false;

        try
        {
            client.DeleteFile(remoteFile);
            result = true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        //Log(result, $"del {remoteFile}");
        return result;
    }

    public bool DownloadFile(string remoteFile, string localFile)
    {
        try
        {
            using var stream = File.Create(localFile);
            client.DownloadFile(remoteFile, stream);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        bool result = File.Exists(localFile);
        Log(result, $"> {remoteFile}");
        return result;
    }

    public IEnumerable<ISftpFile> GetFiles(string remoteDirectory = ".")
    {
        IEnumerable<ISftpFile> result = [];

        try
        {
            result = client.ListDirectory(remoteDirectory);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        //Trace.WriteLine(@$"List remote ""{remoteDirectory}"" counts {result.Count<ISftpFile>() - 2} files."); //. && ..
        return result;
    }

    public bool UploadFile(string localFile, string remoteFile)
    {
        bool result = false;

        try
        {
            using var stream = File.OpenRead(localFile);
            client.UploadFile(stream, remoteFile);
            result = true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        Log(result, $"< {remoteFile}");
        return result;
    }

    public bool ChangeDirectory(string remoteDirectory)
    {
        bool result = false;

        try
        {
            client.ChangeDirectory(remoteDirectory);
            result = true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        Log(result, $"cd {remoteDirectory}");
        return result;
    }

    public int DownloadDirectory()
    {
        if (!ChangeDirectory(config.RemoteDownloadDirectory))
        {
            Trace.WriteLine("Download failed.");
            return 0;
        }

        int result = 0;
        var remoteFiles = GetFiles(client.WorkingDirectory);

        string local = config.LocalDownloadDirectory;
        string store = config.StoreDownloadDirectory;

        //TODO if (remoteFiles.Count > 2) //skip . and ..

        foreach (var sftpFile in remoteFiles)
        {
            string filename = sftpFile.Name;
            string remoteFile = sftpFile.FullName;

            if (sftpFile.IsRegularFile && filename[0] != '.')
            {
                string localFile = Path.Combine(local, filename);

                //if (DownloadFile(remoteFile, localFile))
                if (DownloadFile(filename, localFile))
                {
                    Directory.CreateDirectory(store);
                    string storeFile = Path.Combine(store, filename);
                    File.Copy(localFile, storeFile, true);

                    if (config.DeleteRemoteDownloaded)
                    {
                        //DeleteFile(remoteFile);
                        DeleteFile(filename);
                    }

                    result++;
                }
            }
            else
            {
                //Trace.WriteLine(@$"Download ""{remoteFile}"" passed.");
            }
        }

        Trace.WriteLine($"Download total {result} files.");
        return result;
    }

    public int UploadDirectory()
    {
        int result = 0;
        var localFiles = new DirectoryInfo(config.LocalUploadDirectory).GetFiles();

        if (localFiles.Length > 0)
        {
            if (!ChangeDirectory(config.RemoteUploadDirectory))
            {
                Trace.WriteLine("Upload failed.");
                return 0;
            }

            string store = config.StoreUploadDirectory;
            Directory.CreateDirectory(store);
            bool deleteLocalUploaded = config.DeleteLocalUploaded;

            foreach (var file in localFiles)
            {
                string filename = file.Name;
                string localFile = file.FullName;

                //if (UploadFile(localFile, remoteFile))
                if (UploadFile(localFile, filename))
                {
                    string storeFile = Path.Combine(store, filename);
                    File.Copy(localFile, storeFile, true);

                    if (deleteLocalUploaded)
                    {
                        File.Delete(localFile);
                    }

                    result++;
                }
            }

            Trace.WriteLine($"Upload total {result} files.");
        }

        return result;
    }

    private static void Log(bool result, string text)
    {
        if (result)
        {
            Trace.WriteLine(text);
        }
        else
        {
            Trace.WriteLine(text + " FAIL");
        }
    }
}
