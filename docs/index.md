# JZDO-Exch

[![Build status]][appveyor]
[![GitHub Release]][releases]

A console .NET 8 program to exchange files through SFTP and
to send emails through SMTP.

Консольная программа для обмена файлами по SFTP и отправка
отчета по почте по SMTP.
Программа требует установленного .NET 8 (LTS).
Использует пакет SSH.NET Version=2024.0.0

## Settings / Параметры

Appsettings:

- `JZDO-Exch.config.json` (located with App `exe`)
- `%ProgramData%\Diev\JZDO-Exch.config.json` (overwrites if exists)

Windows Credential Manager:

- `JZDO-Exch *` (name: `JZDO-Exch {host}[ {port}]`, user: `{sftp user}`, pass: `{password}`)
- `SMTP *` (name: `SMTP {host} {port} tls`, user: `{sender}`, pass: `{password}`)

## Requirements / Требования

- .NET 8 Desktop Runtime

## Build / Построение

Build an app with many dlls  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr`

Build a single-file app when NET Desktop runtime required  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true --self-contained false`

Build a single-file app when no runtime required  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true`

## License

Licensed under the [Apache License, Version 2.0].

[JZDO-Exch]: https://diev.github.io/JZDO-Exch/
[Apache License, Version 2.0]: http://www.apache.org/licenses/LICENSE-2.0 "LICENSE"

[appveyor]: https://ci.appveyor.com/project/diev/jzdo-exch
[releases]: https://github.com/diev/JZDO-Exch/releases/latest

[Build status]: https://ci.appveyor.com/api/projects/status/dk0sf5bu4efe08kf?svg=true
[GitHub Release]: https://img.shields.io/github/release/diev/JZDO-Exch.svg
