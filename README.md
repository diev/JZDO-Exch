# [JZDO-Exch]

[![Build status]][appveyor]
[![GitHub Release]][releases]

A console cross-platform .NET8 program to exchange files through SFTP and
to send emails through SMTP.

Программа требует установленного .NET8 (LTS).
Использует пакет SSH.NET Version=2023.0.0

Настройки в файле `JZDO-Exch.runtimeconfig.json`
(пример в папке `sample`, секция `"configProperties"`).

## License

Licensed under the [Apache License, Version 2.0].

[JZDO-Exch]: https://diev.github.io/JZDO-Exch/
[Apache License, Version 2.0]: LICENSE

[appveyor]: https://ci.appveyor.com/project/diev/jzdo-exch
[releases]: https://github.com/diev/JZDO-Exch/releases/latest

[Build status]: https://ci.appveyor.com/api/projects/status/dk0sf5bu4efe08kf?svg=true
[GitHub Release]: https://img.shields.io/github/release/diev/JZDO-Exch.svg
