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


namespace JZDO_Exch.AppSettings;

public sealed class SftpSettings
{
    public string Host { get; set; } = "10.**.**.**";
    public int Port { get; set; } = 22;

    public string User { get; set; } = "zdo";
    public string Pass { get; set; } = "****";

    public string RemoteIn { get; set; } = "zdo/files/doc/in/cs/unknown";
    public string RemoteOut { get; set; } = "zdo/files/doc/out/cs/unknown";

    public string TestRemoteIn { get; set; } = "zdo/files/doc/in/cs/test";
    public string TestRemoteOut { get; set; } = "zdo/files/doc/out/cs/test";

    public string LocalIn { get; set; } = @"ZDO\IN";
    public string LocalOut { get; set; } = @"ZDO\OUT";

    public string StoreIn { get; set; } = @"ZDO\ARCHIVE\IN";
    public string StoreOut { get; set; } = @"ZDO\ARCHIVE\OUT";
}
