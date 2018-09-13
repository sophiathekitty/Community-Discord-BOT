cd ~/NetCore/Community-Discord-BOT/src/CommunityBot
dotnet publish CommunityBot -r win-arm -c Release
ssh Administrator@192.168.0.101 "kill.exe CommunityBot"
scp -r ~/NetCore/Community-Discord-BOT/CommunityBot/bin/Release/netcoreapp2.0/win-arm/publish/* Administrator@192.168.0.101:./../../../miunie
ssh Administrator@192.168.0.101
