param(
  [Parameter(Mandatory=$true)] [string]$RepoUrl,
  [string]$SourcePath = "$PWD",
  [string]$UserName,
  [string]$UserEmail,
  [string]$Branch = 'main'
)

Write-Host "=== Iniciando fix e push do repositório ===" -ForegroundColor Cyan

if (-not (Get-Command git -ErrorAction SilentlyContinue)) { Write-Error "Git não encontrado. Instale Git antes de prosseguir."; exit 1 }
if (-not (Get-Command robocopy -ErrorAction SilentlyContinue)) { Write-Error "robocopy não encontrado. Execute no Windows PowerShell padrão."; exit 1 }

$TempPath = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath ([System.Guid]::NewGuid().ToString())
Write-Host "Pasta temporária: $TempPath"

# Clonar remoto
git clone $RepoUrl $TempPath
if ($LASTEXITCODE -ne 0) { Write-Error "Falha ao clonar o repositório remoto ($RepoUrl). Verifique a URL e credenciais."; exit 1 }

# Copiar código do SourcePath -> Temp (exclui .git e zip)
Write-Host "Copiando arquivos de $SourcePath para $TempPath (excluindo .git e ZIP)..." -ForegroundColor Cyan
robocopy $SourcePath $TempPath /MIR /XD ".git" /XF "MerchantCashFlow_project.zip" | Out-Null

Set-Location $TempPath

if ($UserName) { git config user.name "$UserName"; Write-Host "user.name definido: $UserName" }
if ($UserEmail) { git config user.email "$UserEmail"; Write-Host "user.email definido: $UserEmail" }

# Arquivos a remover do repositório público
$toRemove = @(
  'MerchantCashFlow_project.zip',
  'docs/INTERVIEW.md',
  'docs/DELIVERABLES.md',
  'scripts/push_to_github.ps1',
  'scripts/push_with_clone.ps1',
  'scripts/prepare_repo.ps1'
)

foreach ($f in $toRemove) {
  if (Test-Path $f) {
	Write-Host "Removendo $f" -ForegroundColor Yellow
	git rm --ignore-unmatch $f | Out-Null
  } else {
	Write-Host "Não encontrado (ignorar): $f" -ForegroundColor DarkGray
  }
}

# Atualizar .gitignore (adicionar entradas essenciais)
$gitignorePath = Join-Path $TempPath '.gitignore'
$linesToAdd = @(
  'MerchantCashFlow_project.zip',
  'docs/INTERVIEW.md',
  'docs/DELIVERABLES.md',
  'scripts/push_to_github.ps1',
  'scripts/push_with_clone.ps1',
  'scripts/prepare_repo.ps1',
  '*.user',
  '**/appsettings.Development.json',
  '*.db',
  '/bin/',
  '/obj/',
  '.vs/'
)

if (-not (Test-Path $gitignorePath)) { New-Item -Path $gitignorePath -ItemType File -Force | Out-Null }

$existing = Get-Content $gitignorePath -ErrorAction SilentlyContinue
foreach ($ln in $linesToAdd) {
  if ($existing -notcontains $ln) { Add-Content -Path $gitignorePath -Value $ln }
}
Write-Host ".gitignore atualizado" -ForegroundColor Green

# Atualizar README com conteúdo mínimo de execução
$readmeContent = @"
# MerchantCashFlow

Projeto demonstrativo para o desafio de Arquiteto de Soluções.

Run local (recomendado: Docker)
1. Instale Docker Desktop.
2. Na raiz do projeto execute:
   docker compose up --build
3. API de lançamentos:
   POST http://localhost:5100/api/launches

Exemplo curl:

curl -X POST http://localhost:5100/api/launches -H "Content-Type: application/json" -d '{\"merchantId\":\"00000000-0000-0000-0000-000000000001\",\"amount\":100.5,\"currency\":\"BRL\",\"occurredAt\":\"2026-06-01T12:00:00Z\"}'

Docs importantes:
- docs/architecture.md -> decisões arquiteturais
- tools/postman/Launches.postman_collection.json -> coleção Postman
- tools/k6/loadtest.js -> script k6 para carga

Nota: arquivos internos de entrevista foram removidos do repo público.
"@

Set-Content -Path (Join-Path $TempPath 'README.md') -Value $readmeContent -Force
Write-Host "README.md atualizado" -ForegroundColor Green

# Preparar commit
git add .
$hasChanges = (git status --porcelain)
if ($hasChanges) {
  git commit -m "chore: sanitize repo, update README, remove internal artifacts" | Out-Null
  Write-Host "Commit criado." -ForegroundColor Green
} else {
  Write-Host "Nenhuma mudança detectada após limpeza." -ForegroundColor Yellow
}

# Push
Write-Host "Fazendo push para origin/$Branch..." -ForegroundColor Cyan
try {
  git push origin $Branch
  Write-Host "Push realizado com sucesso." -ForegroundColor Green
} catch {
  Write-Host "Push falhou. Tentando pull --rebase e push..." -ForegroundColor Yellow
  git fetch origin
  git pull --rebase origin $Branch
  git push origin $Branch
  if ($LASTEXITCODE -eq 0) { Write-Host "Push após rebase realizado." -ForegroundColor Green }
  else { Write-Error "Push ainda falhou. Resolva manualmente no clone temporário: $TempPath" }
}

Write-Host "Operação finalizada. Verifique o repositório no GitHub: $RepoUrl" -ForegroundColor Cyan
Write-Host "Pasta temporária onde ocorreu a operação: $TempPath" -ForegroundColor Yellow
