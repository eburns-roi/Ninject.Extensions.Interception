..\contrib\nant\nant.exe -buildfile:Ninject.build clean %1 %2 %3 %4 %5 %6 %7 %8
..\contrib\nant\nant.exe -buildfile:Ninject.build package-source %1 %2 %3 %4 %5 %6 %7 %8
..\contrib\nant\nant.exe -buildfile:Ninject.build "-D:build.config=release" "-D:build.platform=net-3.5" package-bin %1 %2 %3 %4 %5 %6 %7 %8
..\contrib\nant\nant.exe -buildfile:Ninject.build "-D:build.config=release" "-D:build.platform=silverlight-3.0" package-bin %1 %2 %3 %4 %5 %6 %7 %8
..\contrib\nant\nant.exe -buildfile:Ninject.build "-D:build.config=release" "-D:build.platform=mono-2.0" package-bin %1 %2 %3 %4 %5 %6 %7 %8