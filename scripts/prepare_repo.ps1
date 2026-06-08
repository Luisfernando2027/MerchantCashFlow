# Script para preparar repositório antes de subir ao GitHub
Set-StrictMode -Version Latest

dotnet restore
dotnet build MerchantCashFlow.sln

git init -q
git add .
git commit -m "scaffold: initial project" -q
Write-Host "Commit criado. Agora crie o repo no GitHub e adicione remote, então execute: git push -u origin main" -ForegroundColor Green
