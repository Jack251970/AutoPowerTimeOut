# AutoPowerTimeOut

One small tool for fixing Windows sleep option resetting issue

- https://learn.microsoft.com/en-us/answers/questions/4055471/in-windows-11-the-setting-for-screen-and-sleep-kee?forum=windows-all&referrer=answers
- https://techcommunity.microsoft.com/discussions/windows11/no-matter-what-settings-i-changed-my-laptop-still-goes-to-sleep-after-a-short-wh/4191506

# Usage

- Open the app and set your screen, sleep, & hibernate time-outs settings.
- This app will set these settings every time it starts and the system resumes.
- This app will register the logon task when it starts, so it will automatically start during system startup.
- This app will check the settings every 3 minutes and update them if they are changed.

# Build

- `dotnet publish AutoPowerTimeOut.csproj -p:PublishProfile=Net9.0-Win64.pubxml`
