# Scripts de Inicialização e Utilitários

Este diretório contém scripts para facilitar a inicialização e configuração da API.

## Windows

Para iniciar a aplicação em ambiente Windows:

```powershell
.\start-api.ps1
```

## Linux

Para iniciar a aplicação em ambiente Linux:

```bash
./start-api.sh
```

## Configuração de Dispositivos USB no Linux

Se estiver tendo problemas com a detecção do dispositivo biométrico no Linux, execute:

```bash
./setup-usb-permissions.sh
```

Este script configurará as regras udev necessárias para permitir o acesso ao dispositivo biométrico.
