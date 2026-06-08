param(
  [Parameter(Mandatory=$true)] [string]$RepoUrl,
  [string]$SourcePath = "$PWD",
  [string]$UserName,
  [string]$UserEmail,
  [string]$Branch = 'main'
)

Write-Host "Iniciando processo de push via clone temporário..." -ForegroundColor Cyan

if (-not (Get-Command git -ErrorAction SilentlyContinue)) { Write-Error "Git não encontrado. Instale Git antes de prosseguir."; exit 1 }

# Ajustar usuário localmente no repositório temporário após clone
$TempPath = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath ([System.Guid]::NewGuid().ToString())
Write-Host "Pasta temporária: $TempPath"

git clone $RepoUrl $TempPath
if ($LASTEXITCODE -ne 0) { Write-Error "Falha ao clonar o repositório remoto ($RepoUrl). Verifique a URL e credenciais."; exit 1 }

# Copiar arquivos do SourcePath para o clone (exclui .git)
Write-Host "Copiando arquivos de $SourcePath para $TempPath (excluindo .git)..." -ForegroundColor Cyan
robocopy $SourcePath $TempPath /MIR /XD ".git" /XF "MerchantCashFlow_project.zip" | Out-Null

Set-Location $TempPath

if ($UserName) { git config user.name "$UserName" }
if ($UserEmail) { git config user.email "$UserEmail" }

# Remover zip antigo caso exista
if (Test-Path "MerchantCashFlow_project.zip") { git rm --ignore-unmatch MerchantCashFlow_project.zip; git commit -m "chore: remove project zip" -q 2>$null }

git add .
$hasChanges = (git status --porcelain)
if ($hasChanges) {
  git commit -m "scaffold: initial project"
} else {
  Write-Host "Nenhuma alteração detectada para commitar." -ForegroundColor Yellow
}

Write-Host "Tentando push para origin/$Branch..." -ForegroundColor Cyan
try {
  git push origin $Branch
  Write-Host "Push realizado com sucesso." -ForegroundColor Green
} catch {
  Write-Host "Push falhou. Tentando sincronizar (pull --rebase) e enviar novamente..." -ForegroundColor Yellow
  git fetch origin
  git pull --rebase origin $Branch
  git push origin $Branch
  if ($LASTEXITCODE -eq 0) { Write-Host "Push após rebase realizado." -ForegroundColor Green }
  else { Write-Error "Push ainda falhou. Resolva conflitos manualmente no clone em: $TempPath" }
}

Write-Host "Operação finalizada. Pasta temporária: $TempPath" -ForegroundColor Cyan
Write-Host "Se quiser manter o clone para inspeção, não o apague; caso contrário, remova-o manualmente." -ForegroundColor Yellow
