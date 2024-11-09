## 1. Пример config.json:
```
{
  "ThreadCount" : 5,
  "BaseDir": "D:\\YouTubeVideo\\",
  "VideoResolution" : 720,
  "VideoUrls": [
  //Случайные ссылки для теста.
    "https://www.youtube.com/watch?v=h9PUVQaeI0k",
    "https://www.youtube.com/watch?v=_kT_MbdRkRk",
    "https://www.youtube.com/watch?v=3uI2KWRZBXM",
    "https://www.youtube.com/watch?v=__-vp0g_BhA",
    "https://www.youtube.com/watch?v=0gE2wUFKmXo",
  ]
}
```

- ThreadCount - Количество паралельно загражающихся видео
- BaseDir - Директория куда загружать (Н: "D:\\YouTubeVideo\\")
- VideoResolution - ограничивает качество скачиваемых видео (от 144 до 5000)
- VideoUrls - URLs видео, которые необходимо скачать.

<u>PS: если видео не качается, возможно необходимо воспользоваться/сменить VPN страны.</u>

## 2. Настройка логирования NLog.config:
```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">
  <targets>
    <target xsi:type="File" name="f" fileName="${basedir}/logs/${shortdate}.log"
            layout="${longdate} ${uppercase:${level}} ${message}" />
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="f" />
  </rules>
</nlog>
```

	minlevel:
  	- Fatal — used for reporting about errors that are forcing shutdown of the application.
  	- Error — used for logging serious problems occurring during execution of the program.
  	- Warn  — used for reporting non-critical unusual behaviour.
  	- Info — used for informative messages highlighting the progress of the application for 
    		   sysadmins and end users.
  	- Debug — used for debugging messages with extended information about application 
    			processing.
  	- Trace — the noisiest level, used for tracing the code

## 3. После настройки config.json и NLog.config запустить YL.CLI.exe:
    dotnet YL.CLI.exe