using BiometricAPI.Models;
using ControliD;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BiometricAPI.Services
{
    /// <summary>
    /// Serviço para interação com o dispositivo biométrico usando o SDK iDBio
    /// </summary>
    public class BiometricService : IDisposable
    {
        private readonly ILogger<BiometricService> _logger;
        private readonly CIDBio _idbio;
        private bool _initialized = false;
        private bool _disposed = false;

        public BiometricService(ILogger<BiometricService> logger)
        {
            _logger = logger;
            _idbio = new CIDBio();
            
            // Configurar paths adicionais para a DLL nativa
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            _logger.LogInformation($"Diretório base da aplicação: {basePath}");
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _logger.LogInformation("Executando em ambiente Linux");
                if (File.Exists(Path.Combine(basePath, "libcidbio.so")))
                    _logger.LogInformation("Biblioteca libcidbio.so encontrada no diretório da aplicação");
                else
                    _logger.LogWarning("Biblioteca libcidbio.so NÃO encontrada no diretório da aplicação");
                
                if (File.Exists(Path.Combine(basePath, "libcidbio.so.0")))
                    _logger.LogInformation("Biblioteca libcidbio.so.0 encontrada no diretório da aplicação");
                else
                    _logger.LogWarning("Biblioteca libcidbio.so.0 NÃO encontrada no diretório da aplicação");
                
                if (File.Exists(Path.Combine(basePath, "libcidbio.so.1.4.3")))
                    _logger.LogInformation("Biblioteca libcidbio.so.1.4.3 encontrada no diretório da aplicação");
                else
                    _logger.LogWarning("Biblioteca libcidbio.so.1.4.3 NÃO encontrada no diretório da aplicação");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _logger.LogInformation("Executando em ambiente Windows");
                if (File.Exists(Path.Combine(basePath, "libcidbio.dll")))
                    _logger.LogInformation("Biblioteca libcidbio.dll encontrada no diretório da aplicação");
                else
                    _logger.LogWarning("Biblioteca libcidbio.dll NÃO encontrada no diretório da aplicação");
            }
        }

        /// <summary>
        /// Inicializa o SDK do dispositivo biométrico
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _logger.LogInformation("Inicializando SDK do dispositivo biométrico");
            
            var ret = CIDBio.Init();
            if (ret < RetCode.SUCCESS)
            {
                var errorMessage = CIDBio.GetErrorMessage(ret);
                _logger.LogError($"Erro ao inicializar o SDK: {errorMessage}");
                throw new Exception($"Falha ao inicializar o SDK: {errorMessage}");
            }

            _logger.LogInformation("SDK inicializado com sucesso");
            _initialized = true;
        }

        /// <summary>
        /// Verifica o status do dispositivo biométrico
        /// </summary>
        /// <returns>Informações sobre o status do dispositivo</returns>
        public DeviceStatusResponse GetDeviceStatus()
        {
            _logger.LogInformation("Verificando status do dispositivo");

            var response = new DeviceStatusResponse();
            var ret = _idbio.GetDeviceInfo(out string version, out string serialNumber, out string model);
            
            if (ret < RetCode.SUCCESS)
            {
                var errorMessage = CIDBio.GetErrorMessage(ret);
                _logger.LogWarning($"Erro ao obter informações do dispositivo: {errorMessage}");
                
                response.IsConnected = false;
                response.StatusMessage = $"Dispositivo não conectado ou erro de comunicação: {errorMessage}";
            }
            else
            {
                response.IsConnected = true;
                response.DeviceModel = model;
                response.SerialNumber = serialNumber;
                response.FirmwareVersion = version;
                response.StatusMessage = "Dispositivo conectado e funcionando";
                
                _logger.LogInformation($"Dispositivo conectado: {model}, S/N: {serialNumber}, Versão: {version}");
            }

            return response;
        }

        /// <summary>
        /// Captura uma biometria e converte para template base64
        /// </summary>
        /// <returns>Resultado da captura com template em base64</returns>
        public async Task<CaptureResult> CaptureBiometricAsync()
        {
            _logger.LogInformation("Iniciando captura de biometria");

            return await Task.Run(() =>
            {
                var result = new CaptureResult();

                try
                {
                    // Capturar imagem da digital
                    var captureRet = _idbio.CaptureImage(out byte[] imageBuf, out uint width, out uint height);
                    
                    if (captureRet < RetCode.SUCCESS)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Falha ao capturar imagem: {CIDBio.GetErrorMessage(captureRet)}";
                        _logger.LogWarning(result.ErrorMessage);
                        return result;
                    }

                    // Extrair template da imagem capturada
                    int quality;
                    var extractRet = _idbio.ExtractTemplateFromImage((uint)width, (uint)height, imageBuf, out string templateString, out quality);
                    
                    if (extractRet < RetCode.SUCCESS)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Falha ao extrair template: {CIDBio.GetErrorMessage(extractRet)}";
                        _logger.LogWarning(result.ErrorMessage);
                        return result;
                    }
                    
                    // Converter o template para base64
                    result.Success = true;
                    result.TemplateBase64 = templateString;
                    result.ImageBuffer = imageBuf;
                    result.ImageWidth = width;
                    result.ImageHeight = height;
                    
                    _logger.LogInformation("Biometria capturada e template extraído com sucesso");
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Erro durante a captura da biometria: {ex.Message}";
                    _logger.LogError(ex, "Exceção durante a captura da biometria");
                }

                return result;
            });
        }

        /// <summary>
        /// Identifica uma biometria capturada entre as cadastradas no dispositivo
        /// </summary>
        /// <returns>Resultado da identificação</returns>
        public async Task<IdentifyResponse> IdentifyBiometricAsync()
        {
            _logger.LogInformation("Iniciando identificação de biometria");

            return await Task.Run(() =>
            {
                var response = new IdentifyResponse();

                try
                {
                    var ret = _idbio.CaptureAndIdentify(out long id, out int score, out int quality);
                    
                    if (ret < RetCode.SUCCESS)
                    {
                        response.Success = false;
                        response.Message = $"Falha na identificação: {CIDBio.GetErrorMessage(ret)}";
                        response.Quality = quality;
                        
                        _logger.LogWarning($"Falha na identificação: {CIDBio.GetErrorMessage(ret)}, qualidade: {quality}");
                    }
                    else
                    {
                        response.Success = true;
                        response.Message = "Biometria identificada com sucesso";
                        response.MatchedId = (int)id;
                        response.Score = score;
                        response.Quality = quality;
                        
                        _logger.LogInformation($"Biometria identificada: ID {id}, score: {score}, qualidade: {quality}");
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Erro durante a identificação: {ex.Message}";
                    _logger.LogError(ex, "Exceção durante a identificação da biometria");
                }

                return response;
            });
        }

        /// <summary>
        /// Cadastra uma biometria no dispositivo
        /// </summary>
        /// <param name="id">ID para associar à biometria</param>
        /// <returns>true se o cadastro foi bem-sucedido</returns>
        public async Task<bool> EnrollBiometricAsync(long id)
        {
            _logger.LogInformation($"Iniciando cadastro de biometria com ID {id}");

            return await Task.Run(() =>
            {
                try
                {
                    var ret = _idbio.CaptureAndEnroll(id);
                    
                    if (ret < RetCode.SUCCESS)
                    {
                        _logger.LogWarning($"Falha no cadastro da biometria: {CIDBio.GetErrorMessage(ret)}");
                        return false;
                    }
                    
                    _logger.LogInformation($"Biometria cadastrada com sucesso: ID {id}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exceção durante o cadastro da biometria com ID {id}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Exclui todas as biometrias do dispositivo
        /// </summary>
        /// <returns>true se a exclusão foi bem-sucedida</returns>
        public bool DeleteAllBiometricsFromDevice()
        {
            _logger.LogInformation("Excluindo todas as biometrias do dispositivo");

            try
            {
                var ret = _idbio.DeleteAllTemplates();
                
                if (ret < RetCode.SUCCESS)
                {
                    _logger.LogWarning($"Falha ao excluir biometrias do dispositivo: {CIDBio.GetErrorMessage(ret)}");
                    return false;
                }
                
                _logger.LogInformation("Todas as biometrias excluídas do dispositivo com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao excluir biometrias do dispositivo");
                return false;
            }
        }

        /// <summary>
        /// Adiciona uma biometria ao dispositivo a partir de um template
        /// </summary>
        /// <param name="id">ID para associar à biometria</param>
        /// <param name="templateBase64">Template em base64</param>
        /// <returns>true se o cadastro foi bem-sucedido</returns>
        public bool AddTemplateToDevice(long id, string templateBase64)
        {
            _logger.LogInformation($"Adicionando template ao dispositivo com ID {id}");

            try
            {
                // O templateBase64 já é uma string no formato que a API espera
                var ret = _idbio.SaveTemplate(id, templateBase64);
                
                if (ret < RetCode.SUCCESS)
                {
                    _logger.LogWarning($"Falha ao adicionar template ao dispositivo: {CIDBio.GetErrorMessage(ret)}");
                    return false;
                }
                
                _logger.LogInformation($"Template adicionado ao dispositivo com sucesso: ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exceção ao adicionar template ao dispositivo com ID {id}");
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _initialized)
                {
                    _logger.LogInformation("Finalizando SDK do dispositivo biométrico");
                    CIDBio.Terminate();
                }

                _disposed = true;
            }
        }

        ~BiometricService()
        {
            Dispose(false);
        }
    }
}
