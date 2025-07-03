# Script principal para construção e inicialização da API Biométrica no Windows

# Definir cores para output
function Write-ColorOutput {
    param (
        [string]$text,
        [string]$color = "White"
    )
    Write-Host $text -ForegroundColor $color
}

Write-ColorOutput "=== Biometric API - Processo de Inicialização ===" "Cyan"

# Verificar dependências
Write-ColorOutput "Verificando dependências..." "Yellow"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-ColorOutput ".NET SDK não encontrado. Por favor, instale o .NET 6.0 SDK." "Red"
    exit 1
}

Write-ColorOutput "Todas as dependências estão instaladas!" "Green"

# Construir a aplicação
Write-ColorOutput "Construindo a aplicação..." "Yellow"
try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "Falha na construção da aplicação." "Red"
        exit 1
    }
    Write-ColorOutput "Aplicação construída com sucesso!" "Green"
} catch {
    Write-ColorOutput "Erro durante a construção: $_" "Red"
    exit 1
}

# Iniciar a aplicação
Write-ColorOutput "Iniciando a aplicação..." "Yellow"
try {
    Start-Process -FilePath "dotnet" -ArgumentList "run --configuration Release --urls http://localhost:5000" -PassThru
    Start-Sleep -Seconds 3
    
    Write-ColorOutput "API iniciada com sucesso!" "Green"
    Write-ColorOutput "A API está disponível em: http://localhost:5000" "Cyan"
    Write-ColorOutput "Documentação Swagger: http://localhost:5000/swagger" "Cyan"
    
    # Verificar se o dispositivo biométrico está conectado
    Write-ColorOutput "Verificando dispositivo biométrico..." "Yellow"
    Start-Sleep -Seconds 3 # Aguardar a inicialização completa da API
    
    try {
        $statusResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Biometric/status" -Method Get -TimeoutSec 10
        if ($statusResponse.IsConnected) {
            Write-ColorOutput "Dispositivo biométrico conectado e pronto para uso!" "Green"
        } else {
            Write-ColorOutput "Dispositivo biométrico não detectado." "Red"
            Write-ColorOutput "Verifique se o dispositivo está conectado corretamente." "Yellow"
        }
    } catch {
        Write-ColorOutput "Não foi possível verificar o status do dispositivo biométrico." "Red"
        Write-ColorOutput "A API pode ainda estar inicializando. Tente novamente em alguns segundos." "Yellow"
    }
    
    # Mostrar menu de opções
    Write-ColorOutput "`n=== Opções disponíveis ===" "Cyan"
    Write-ColorOutput "1. Executar testes da API" "Yellow"
    Write-ColorOutput "2. Abrir documentação Swagger no navegador" "Yellow"
    Write-ColorOutput "3. Parar a API" "Yellow"
    Write-ColorOutput "4. Sair (deixar API executando)" "Yellow"
    
    $option = Read-Host "Escolha uma opção (1-4)"
    
    switch ($option) {
        "1" {
            Write-ColorOutput "Executando testes básicos da API..." "Yellow"
            try {
                $healthCheck = Invoke-RestMethod -Uri "http://localhost:5000/api/Biometric/status" -Method Get
                Write-ColorOutput "Teste de status: OK" "Green"
            } catch {
                Write-ColorOutput "Teste de status: FALHOU - $_" "Red"
            }
        }
        "2" {
            Start-Process "http://localhost:5000/swagger"
            Write-ColorOutput "Abrindo Swagger no navegador..." "Green"
        }
        "3" {
            $processes = Get-Process | Where-Object { $_.ProcessName -eq "dotnet" -and $_.CommandLine -like "*BiometricAPI*" }
            if ($processes) {
                $processes | Stop-Process -Force
                Write-ColorOutput "API parada com sucesso!" "Green"
            } else {
                Write-ColorOutput "Processo da API não encontrado." "Yellow"
            }
        }
        "4" {
            Write-ColorOutput "Saindo. A API continuará em execução." "Green"
        }
        default {
            Write-ColorOutput "Saindo. A API continuará em execução." "Green"
        }
    }
} catch {
    Write-ColorOutput "Erro ao iniciar a aplicação: $_" "Red"
    exit 1
}
