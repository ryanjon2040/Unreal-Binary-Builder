@echo off
set output_dir=Output
set binary_updater=UnrealBinaryBuilderUpdater\UnrealBinaryBuilderUpdater.csproj
set binary_builder=UnrealBinaryBuilder\UnrealBinaryBuilder.csproj

if not exist %output_dir% (mkdir %output_dir%)
rmdir /s /q UnrealBinaryBuilder\bin
rmdir /s /q UnrealBinaryBuilderUpdater\bin
dotnet publish -c Release -r win-x64 --output %output_dir% %binary_builder% --self-contained true -p:PublishSingleFile=true --force
pause