  $ErrorActionPreference = "Stop"

Set-Location $PSScriptRoot

dotnet build .\DeliverySystem.sln

dotnet dotnet-ef database update `
  --project .\DeliverySystem.Infrastructure\DeliverySystem.Infrastructure.csproj `
  --startup-project .\DeliverySystem.Api\DeliverySystem.Api.csproj

Start-Process powershell -ArgumentList @(
  "-NoExit",
  "-Command",
  "Set-Location '$PSScriptRoot'; dotnet run --project .\DeliverySystem.Api\DeliverySystem.Api.csproj --launch-profile http"
)

Start-Process powershell -ArgumentList @(
  "-NoExit",
  "-Command",
  "Set-Location '$PSScriptRoot'; dotnet run --project .\DeliverySystem.Presentation.Wpf\DeliverySystem.Presentation.Wpf.csproj"
)
