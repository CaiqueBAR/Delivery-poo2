$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$delivery = Join-Path $root "DeliverySystem"

if (!(Test-Path $delivery)) {
  throw "Pasta 'DeliverySystem' não encontrada em: $delivery"
}

Set-Location $delivery
.\start.ps1

