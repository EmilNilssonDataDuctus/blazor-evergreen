FROM mcr.microsoft.com/dotnet/sdk:7.0 as restore
WORKDIR /src

COPY ./BlazorAppEvergreenOIDC/*.csproj .

RUN dotnet restore

FROM restore as publish
COPY ./BlazorAppEvergreenOIDC .
RUN dotnet publish -c Release -o ./out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=publish /src/out  /app

ENV ASPNETCORE_URLS=http://+:5172
CMD ["dotnet", "BlazorAppEvergreenOIDC.dll"]