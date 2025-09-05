# [JZDO-Exch](https://diev.github.io/JZDO-Exch/)

[![Build status](https://ci.appveyor.com/api/projects/status/dk0sf5bu4efe08kf?svg=true)](https://ci.appveyor.com/project/diev/jzdo-exch)
[![.NET8 Desktop](https://github.com/diev/JZDO-Exch/actions/workflows/dotnet8-desktop.yml/badge.svg)](https://github.com/diev/JZDO-Exch/actions/workflows/dotnet8-desktop.yml)
[![GitHub Release](https://img.shields.io/github/release/diev/JZDO-Exch.svg)](https://github.com/diev/JZDO-Exch/releases/latest)

A console .NET 8 program to exchange files through SFTP and
to send emails through SMTP.

Консольная программа для обмена файлами по SFTP и отправка
отчета по почте по SMTP.
Программа требует установленного .NET 8 (LTS) - 9.
Использует пакет SSH.NET Version=2025.0.0

## Settings / Параметры

Appsettings:

- `JZDO-Exch.config.json` (located with App `exe`)
- `%ProgramData%\Diev\JZDO-Exch.config.json` (overwrites if exists)

Windows Credential Manager:

- `JZDO-Exch *` (name: `JZDO-Exch {host}[ {port}]`, user: `{sftp user}`, pass: `{password}`)
- `SMTP *` (name: `SMTP {host} {port} tls`, user: `{sender}`, pass: `{password}`)

## Requirements / Требования

- .NET 8-9 Desktop Runtime

## Build / Построение

Build an app with many dlls  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr`

Build a single-file app when NET Desktop runtime required  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true --no-self-contained`

Build a single-file app when no runtime required  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true`

Или просто используйте `build.cmd`.

## Versioning / Порядок версий

Номер версии программы указывается по нарастающему принципу:

* Протестированная максимальная версия .NET (9);
* Год текущей разработки (2025);
* Месяц без первого нуля и день редакции (902 - 02.09.2025);
* Номер билда - просто нарастающее число для внутренних отличий.
Если настроен сервис AppVeyor, то это его автоинкремент.
Или часто просто 0.

Продукт развивается для собственных нужд, а не по коробочной
стратегии, и поэтому *Breaking Changes* могут случаться чаще,
чем это принято в *SemVer*.

## License / Лицензия

Licensed under the [Apache License, Version 2.0](LICENSE).  
Вы можете использовать эти материалы под свою ответственность.

[![Telegram](https://img.shields.io/badge/t.me-dievdo-blue?logo=telegram)](https://t.me/dievdo)
