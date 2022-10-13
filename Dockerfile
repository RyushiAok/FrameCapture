#############################################################
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app  
COPY ./App/FrameCapture ./
# Restore as distinct layers 
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -r linux-x64 --self-contained false -c Release -o out
 
# Build runtime image 
# FROM mcr.microsoft.com/dotnet/aspnet:6.0 
# https://github.com/shimat/opencvsharp
# https://hub.docker.com/r/shimat/ubuntu20-dotnet5-opencv4.5.3/tags
FROM shimat/ubuntu20-dotnet5-opencv4.5.3:20210821
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FrameCapture.dll"] 