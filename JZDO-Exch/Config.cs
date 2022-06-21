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

using static JZDO_Exch.Helpers.ConfigHelper;

namespace JZDO_Exch;

public static class Config
{
    public static string Logs { get; } = AppContext.GetData(nameof(Logs)) as string ?? "logs";

    public static class Sftp
    {
        private static readonly string section = nameof(Sftp) + '.';

        public static string Host => GetString(section + nameof(Host));
        public static int Port => GetInt(section + nameof(Port), 22);

        public static string User => GetString(section + nameof(User), "zdo");
        public static string Pass => GetString(section + nameof(Pass));

        public static string RemoteIn => GetString(section + nameof(RemoteIn), "/home/zdo/files/doc/in/cs/unknown");
        public static string RemoteOut => GetString(section + nameof(RemoteOut), "/home/zdo/files/doc/out/cs/unknown");

        public static string TestRemoteIn => GetString(section + nameof(TestRemoteIn), "/home/zdo/files/doc/in/cs/test");
        public static string TestRemoteOut => GetString(section + nameof(TestRemoteOut), "/home/zdo/files/doc/out/cs/test");

        public static string LocalIn => GetString(section + nameof(LocalIn), "ZDO/IN");
        public static string LocalOut => GetString(section + nameof(LocalOut), "ZDO/OUT");

        public static string StoreIn => GetString(section + nameof(StoreIn), "ZDO/ARCHIVE/IN");
        public static string StoreOut => GetString(section + nameof(StoreOut), "ZDO/ARCHIVE/OUT");
    }

    public static class Smtp
    {
        private static readonly string section = nameof(Smtp) + '.';

        public static string Host => GetString(section + nameof(Host));
        public static int Port => GetInt(section + nameof(Port), 25);
        public static bool Tls => GetBool(section + nameof(Tls), true);
        public static int Timeout => GetInt(section + nameof(Timeout), 60000);

        public static string User => GetString(section + nameof(User));
        public static string Name => GetString(section + nameof(Name), "ZDO");
        public static string Pass => GetString(section + nameof(Pass));

        public static string[] Subscribers => GetList(section + nameof(Subscribers));
    }
}
