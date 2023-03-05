FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source

COPY . .
RUN dotnet restore

RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "chat-room-for-nft-owners.dll" ]


