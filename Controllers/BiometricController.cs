using BiometricAPI.Models;
using BiometricAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BiometricAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BiometricController : ControllerBase
    {
        private readonly BiometricService _biometricService;
        private readonly BiometricDataService _dataService;
        private readonly ILogger<BiometricController> _logger;

        public BiometricController(
            BiometricService biometricService, 
            BiometricDataService dataService,
            ILogger<BiometricController> logger)
        {
            _biometricService = biometricService;
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// Verifica o status do dispositivo biométrico
        /// </summary>
        /// <returns>Informações do dispositivo se conectado</returns>
        [HttpGet("status")]
        public ActionResult<DeviceStatusResponse> GetDeviceStatus()
        {
            try
            {
                _logger.LogInformation("Verificando status do dispositivo");
                var status = _biometricService.GetDeviceStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status do dispositivo");
                return StatusCode(500, new { message = "Erro ao verificar status do dispositivo", error = ex.Message });
            }
        }

        /// <summary>
        /// Inicia a captura de uma biometria
        /// </summary>
        /// <returns>Template da biometria capturada em Base64</returns>
        [HttpPost("capture")]
        public async Task<ActionResult<CaptureResponse>> CaptureBiometric()
        {
            try
            {
                _logger.LogInformation("Iniciando captura de biometria");
                
                var result = await _biometricService.CaptureBiometricAsync();
                
                if (result.Success)
                {
                    // Salvar no banco de dados
                    var id = await _dataService.SaveBiometricAsync(result.TemplateBase64);
                    
                    return Ok(new CaptureResponse 
                    { 
                        Success = true, 
                        Message = "Biometria capturada com sucesso",
                        BiometricId = id,
                        TemplateBase64 = result.TemplateBase64
                    });
                }
                else
                {
                    // Se falhar, apaga todas as biometrias do banco
                    await _dataService.DeleteAllBiometricsAsync();
                    
                    return BadRequest(new CaptureResponse 
                    { 
                        Success = false, 
                        Message = $"Falha ao capturar biometria: {result.ErrorMessage}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao capturar biometria");
                return StatusCode(500, new { message = "Erro ao capturar biometria", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todas as biometrias salvas no banco de dados
        /// </summary>
        /// <returns>Lista de biometrias</returns>
        [HttpGet("templates")]
        public async Task<ActionResult<List<BiometricTemplate>>> GetBiometrics()
        {
            try
            {
                _logger.LogInformation("Obtendo todas as biometrias");
                var templates = await _dataService.GetAllBiometricsAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter biometrias");
                return StatusCode(500, new { message = "Erro ao obter biometrias", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma biometria específica pelo ID
        /// </summary>
        /// <param name="id">ID da biometria</param>
        /// <returns>Template da biometria</returns>
        [HttpGet("templates/{id}")]
        public async Task<ActionResult<BiometricTemplate>> GetBiometricById(int id)
        {
            try
            {
                _logger.LogInformation($"Obtendo biometria {id}");
                var template = await _dataService.GetBiometricByIdAsync(id);
                
                if (template == null)
                    return NotFound(new { message = $"Biometria com ID {id} não encontrada" });
                
                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter biometria {id}");
                return StatusCode(500, new { message = $"Erro ao obter biometria {id}", error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se uma biometria corresponde a alguma das cadastradas
        /// </summary>
        /// <returns>Resultado da verificação</returns>
        [HttpPost("identify")]
        public async Task<ActionResult<IdentifyResponse>> IdentifyBiometric()
        {
            try
            {
                _logger.LogInformation("Iniciando identificação de biometria");
                
                var result = await _biometricService.IdentifyBiometricAsync();
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao identificar biometria");
                return StatusCode(500, new { message = "Erro ao identificar biometria", error = ex.Message });
            }
        }

        /// <summary>
        /// Exclui todas as biometrias do banco de dados
        /// </summary>
        /// <returns>Confirmação da exclusão</returns>
        [HttpDelete("templates")]
        public async Task<ActionResult> DeleteAllBiometrics()
        {
            try
            {
                _logger.LogInformation("Excluindo todas as biometrias");
                await _dataService.DeleteAllBiometricsAsync();
                return Ok(new { message = "Todas as biometrias foram excluídas com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir biometrias");
                return StatusCode(500, new { message = "Erro ao excluir biometrias", error = ex.Message });
            }
        }

        /// <summary>
        /// Exclui uma biometria específica pelo ID
        /// </summary>
        /// <param name="id">ID da biometria</param>
        /// <returns>Confirmação da exclusão</returns>
        [HttpDelete("templates/{id}")]
        public async Task<ActionResult> DeleteBiometric(int id)
        {
            try
            {
                _logger.LogInformation($"Excluindo biometria {id}");
                var success = await _dataService.DeleteBiometricAsync(id);
                
                if (!success)
                    return NotFound(new { message = $"Biometria com ID {id} não encontrada" });
                
                return Ok(new { message = $"Biometria {id} excluída com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao excluir biometria {id}");
                return StatusCode(500, new { message = $"Erro ao excluir biometria {id}", error = ex.Message });
            }
        }
    }
}
