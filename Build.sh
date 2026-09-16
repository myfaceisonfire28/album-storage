dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
echo "Copying to desktop"
cp bin/Release/net10.0/linux-x64/publish/Music ~/Desktop