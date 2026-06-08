param(
  [Parameter(Mandatory=$true)] [string]$RepoUrl,
  [string]$UserName,
  [string]$UserEmail,
  [string]$Branch = 'main'
)

Write-Host "Iniciando preparação do repositório..." -ForegroundColor Cyan

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
  Write-Error "Git não encontrado na máquina. Instale Git antes de prosseguir."; exit 1
}

if ($UserName) { git config user.name "$UserName"; Write-Host "user.name definido para $UserName" }
if ($UserEmail) { git config user.email "$UserEmail"; Write-Host "user.email definido para $UserEmail" }

# Inicializar git se necessário
if (-not (Test-Path .git)) {
  git init
  git checkout -b $Branch
  Write-Host "Repositório Git inicializado e branch '$Branch' criado." -ForegroundColor Green
} else {
  Write-Host "Repositório Git já inicializado." -ForegroundColor Yellow
}

# Adicionar remote se não existir
$remoteUrl = git remote get-url origin 2>$null
if (-not $remoteUrl) {
  git remote add origin $RepoUrl
  Write-Host "Remote 'origin' adicionado: $RepoUrl" -ForegroundColor Green
} elseif ($remoteUrl -ne $RepoUrl) {
  Write-Host "Remote 'origin' já existe e aponta para: $remoteUrl" -ForegroundColor Yellow
  Write-Host "Atualizando remote para: $RepoUrl" -ForegroundColor Cyan
  git remote set-url origin $RepoUrl
}

# Adicionar e commitar alterações (se houver)
$status = git status --porcelain
if ($status) {
  git add .
  git commit -m "scaffold: initial project"
  Write-Host "Arquivos comitados." -ForegroundColor Green
} else {
  Write-Host "Nenhuma alteração a commitar." -ForegroundColor Yellow
}

# Push com tentativa de rebase se necessário
Write-Host "Enviando para o remoto ($Branch)..." -ForegroundColor Cyan
try {
  git push -u origin $Branch
  Write-Host "Push realizado com sucesso." -ForegroundColor Green
} catch {
  Write-Host "Push falhou. Tentando sincronizar com remote e reenvio (pull --rebase)." -ForegroundColor Yellow
  git fetch origin
  git pull --rebase origin $Branch
  git push -u origin $Branch
  Write-Host "Push após rebase executado." -ForegroundColor Green
}

Write-Host "Pronto. Verifique o repositório no GitHub: $RepoUrl" -ForegroundColor Cyan
Write-Host "Se for solicitado usuário/senha, use seu usuário GitHub e um Personal Access Token (PAT) como senha." -ForegroundColor Yellow
