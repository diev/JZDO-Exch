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

using Renci.SshNet;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JZDO_Exch;

public class ExchangePoint : IDisposable
{
    private readonly SftpClient? _client;

    public bool Connected { get; private set; } = false;
    public int NumSent { get; private set; } = 0;
    public int NumReceived { get; private set; } = 0;

    public ExchangePoint()
    {
        try
        {
            _client = new SftpClient(Config.Sftp.Host, Config.Sftp.Port, Config.Sftp.User,
                Encoding.UTF8.GetString(Convert.FromBase64String(Config.Sftp.Pass)));
            _client.Connect();
            Connected = _client.IsConnected;
        }
        catch (Renci.SshNet.Common.SshConnectionException)
        {
            Trace.WriteLine("Cannot connect to the server.");
        }
        catch (System.Net.Sockets.SocketException)
        {
            Trace.WriteLine("Unable to establish the socket.");
        }
        catch (Renci.SshNet.Common.SshAuthenticationException)
        {
            Trace.WriteLine("Authentication of SSH session failed.");
        }
        catch (Exception)
        {
            Trace.WriteLine("SFTP settings failed.");
        }
    }

    public void ReceiveFiles(string remotePath)
    {
        if (_client is null)
        {
            Trace.WriteLine("SFTP client failed.");
            return;
        }

        _client.ChangeDirectory(remotePath);

        string backupRecvPath = Path.Combine(Config.Sftp.StoreIn, $"{DateTime.Now:yyyyMMdd}");
        var remoteFiles = _client.ListDirectory(".");

        foreach (var file in remoteFiles)
        {
            if (file.IsRegularFile)
            {
                string filename = file.Name;
                string remoteFile = file.FullName;
                string recvFile = Path.Combine(Config.Sftp.LocalIn, filename);

                if (File.Exists(recvFile))
                {
                    Trace.WriteLine($"{DateTime.Now:G} > {filename} [del]");

                    File.Delete(recvFile);
                }

                using (var stream = File.Create(recvFile))
                {
                    _client.DownloadFile(remoteFile, stream);
                }

                if (File.Exists(recvFile))
                {
                    Trace.WriteLine($"{DateTime.Now:G} > {filename} [{file.Length:#,##0}]");

                    _client.DeleteFile(remoteFile);

                    Directory.CreateDirectory(backupRecvPath);
                    File.Copy(recvFile, Path.Combine(backupRecvPath, $"{DateTime.Now:HHmmss}.{filename}"));
                    NumReceived++;
                }
            }
        }
    }

    public void SendFiles(string remotePath)
    {
        if (_client is null)
        {
            Trace.WriteLine("SFTP client failed.");
            return;
        }

        _client.ChangeDirectory(remotePath);

        string backupSentPath = Path.Combine(Config.Sftp.StoreOut, $"{DateTime.Now:yyyyMMdd}");
        var localFiles = new DirectoryInfo(Config.Sftp.LocalOut).GetFiles();

        if (localFiles.Length > 0 && !Directory.Exists(backupSentPath))
        {
            Directory.CreateDirectory(backupSentPath);
        }

        foreach (var file in localFiles)
        {
            string filename = file.Name;
            string sendFile = file.FullName;

            using (var stream = File.OpenRead(sendFile))
            {
                _client.UploadFile(stream, filename, true);
            }

            if (_client.Exists(filename))
            {
                Trace.WriteLine($"{DateTime.Now:G} < {filename} [{file.Length:#,##0}]");

                File.Move(sendFile, Path.Combine(backupSentPath, $"{filename}.{DateTime.Now:HHmmss}"), true);
                NumSent++;
            }
        }
    }

    public void SelfTest(string remotePath)
    {
        if (_client is null)
        {
            Trace.WriteLine("SFTP client failed.");
            return;
        }

        const string filename = "zdo_test.txt";

        string sendFile = Path.Combine(Config.Sftp.LocalOut, filename);
        string remoteFile = $"{remotePath}/{filename}";

        File.WriteAllText(sendFile, $"Test {DateTime.Now:G}");

        using var stream = File.OpenRead(sendFile);
        _client.UploadFile(stream, remoteFile, true);

        Trace.WriteLine($"{DateTime.Now:G} < {filename} [test]");
    }

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

            if (_client != null)
            {
                _client.Disconnect();
                _client.Dispose();
            }
        }
    }
}
