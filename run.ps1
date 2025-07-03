# Script simples para executar a API Biométrica diretamente no Windows

Write-Host "Iniciando Biometric API..." -ForegroundColor Cyan

# Verificar se .NET SDK está instalado
try {
    $dotnetVersion = & dotnet --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "SDK não encontrado"
    }
    Write-Host "✓ .NET SDK versão $dotnetVersion encontrado" -ForegroundColor Green
} catch {
    Write-Host "ERRO: .NET SDK não encontrado ou não está funcionando corretamente." -ForegroundColor Red
    Write-Host "Por favor, instale o .NET 6.0 SDK ou superior:" -ForegroundColor Yellow
    Write-Host "Download: https://dotnet.microsoft.com/download/dotnet/6.0" -ForegroundColor Yellow
    Write-Host "" -ForegroundColor Yellow
    Write-Host "Após a instalação, reinicie o PowerShell e tente novamente." -ForegroundColor Yellow
    exit 1
}

# Verificar se as DLLs necessárias estão presentes
$requiredFiles = @(
    "lib\CIDBio.dll",
    "lib\libcidbio.dll"
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "ERRO: Arquivo necessário não encontrado: $file" -ForegroundColor Red
        exit 1
    }
}

Write-Host "Dependências verificadas com sucesso!" -ForegroundColor Green

# Executar a aplicação
Write-Host "Executando aplicação em http://localhost:5000" -ForegroundColor Yellow
Write-Host "Swagger UI disponível em: http://localhost:5000/swagger" -ForegroundColor Yellow
Write-Host "Pressione Ctrl+C para parar a aplicação" -ForegroundColor Yellow
Write-Host ""

try {
    dotnet run --configuration Release --urls "http://localhost:5000"
} catch {
    Write-Host "Erro ao executar a aplicação: $_" -ForegroundColor Red
    exit 1
}
