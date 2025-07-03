# Script para inicializar a API no Windows
Write-Host "Iniciando API Biométrica..." -ForegroundColor Cyan

# Verificar se o Docker está instalado
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Docker não encontrado. Por favor, instale o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Ir para o diretório raiz do projeto
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = (Get-Item $scriptDir).Parent.FullName
Set-Location $rootDir

# Construir e iniciar os containers
Write-Host "Construindo e iniciando containers..." -ForegroundColor Yellow
docker-compose up -d --build

# Verificar se iniciou corretamente
if ($LASTEXITCODE -eq 0) {
    Write-Host "API iniciada com sucesso!" -ForegroundColor Green
    Write-Host "A API está disponível em: http://localhost:5000" -ForegroundColor Cyan
    Write-Host "Documentação Swagger: http://localhost:5000/swagger" -ForegroundColor Cyan
} else {
    Write-Host "Falha ao iniciar a API. Verifique os logs com: docker-compose logs -f" -ForegroundColor Red
}
