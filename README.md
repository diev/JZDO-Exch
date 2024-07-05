# [JZDO-Exch]

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
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true --no-self-contained`

Build a single-file app when no runtime required  
`dotnet publish JZDO-Exch\JZDO-Exch.csproj -o Distr -r win-x64 -p:PublishSingleFile=true`

Или просто используйте `build.cmd`.

## Versioning / Порядок версий

Номер версии программы указывается по нарастающему принципу:

* Требуемая версия .NET (8);
* Год текущей разработки (2024);
* Месяц без первого нуля и день редакции (624 - 24.06.2024);
* Номер билда - просто нарастающее число для внутренних отличий.
Если настроен сервис AppVeyor, то это его автоинкремент.

Продукт развивается для собственных нужд, и поэтому
Breaking Changes могут случаться чаще, чем это принято в SemVer.

При обновлении программы рекомендуется сохранить предыдущий конфиг,
удалить его из папки с программой, чтобы она создала новый, перенести
необходимые старые значения в новый конфиг перед новым запуском
программы.

## License

Licensed under the [Apache License, Version 2.0].

[JZDO-Exch]: https://diev.github.io/JZDO-Exch/
[Apache License, Version 2.0]: LICENSE

[appveyor]: https://ci.appveyor.com/project/diev/jzdo-exch
[releases]: https://github.com/diev/JZDO-Exch/releases/latest

[Build status]: https://ci.appveyor.com/api/projects/status/dk0sf5bu4efe08kf?svg=true
[GitHub Release]: https://img.shields.io/github/release/diev/JZDO-Exch.svg
