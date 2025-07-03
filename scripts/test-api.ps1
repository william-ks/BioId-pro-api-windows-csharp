# Script para testar a API Biométrica no Windows

$BaseUrl = "http://localhost:5000/api/Biometric"

# Função para formatar saída JSON
function Format-Json {
    param([string]$json)
    try {
        $formattedJson = $json | ConvertFrom-Json | ConvertTo-Json -Depth 10
        return $formattedJson
    }
    catch {
        return $json
    }
}

# Verificar status do dispositivo
Write-Host "Verificando status do dispositivo..." -ForegroundColor Cyan
$response = Invoke-RestMethod -Uri "$BaseUrl/status" -Method Get
$response | ConvertTo-Json -Depth 10
Write-Host ""

# Aguardar
Write-Host "Pressione Enter para testar captura de biometria..." -ForegroundColor Yellow
$null = Read-Host

# Capturar biometria
Write-Host "Iniciando captura de biometria..." -ForegroundColor Cyan
Write-Host "Por favor, coloque o dedo no leitor quando solicitado." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "$BaseUrl/capture" -Method Post
$response | ConvertTo-Json -Depth 10
Write-Host ""

# Aguardar
Write-Host "Pressione Enter para listar biometrias armazenadas..." -ForegroundColor Yellow
$null = Read-Host

# Listar biometrias
Write-Host "Listando biometrias armazenadas..." -ForegroundColor Cyan
$response = Invoke-RestMethod -Uri "$BaseUrl/templates" -Method Get
$response | ConvertTo-Json -Depth 10
Write-Host ""

# Aguardar
Write-Host "Pressione Enter para identificar uma biometria..." -ForegroundColor Yellow
$null = Read-Host

# Identificar biometria
Write-Host "Iniciando identificação de biometria..." -ForegroundColor Cyan
Write-Host "Por favor, coloque o dedo no leitor quando solicitado." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "$BaseUrl/identify" -Method Post
$response | ConvertTo-Json -Depth 10
Write-Host ""

Write-Host "Teste concluído!" -ForegroundColor Green
