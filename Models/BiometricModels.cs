using System;

namespace BiometricAPI.Models
{
    /// <summary>
    /// Representa um template biométrico armazenado no banco de dados
    /// </summary>
    public class BiometricTemplate
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Template biométrico em formato Base64
        /// </summary>
        public string TemplateBase64 { get; set; }
        
        /// <summary>
        /// Data e hora de criação do template
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Resposta da verificação de status do dispositivo
    /// </summary>
    public class DeviceStatusResponse
    {
        public bool IsConnected { get; set; }
        public string DeviceModel { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string StatusMessage { get; set; }
    }

    /// <summary>
    /// Resposta da captura de biometria
    /// </summary>
    public class CaptureResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? BiometricId { get; set; }
        public string TemplateBase64 { get; set; }
    }

    /// <summary>
    /// Resultado da captura de biometria
    /// </summary>
    public class CaptureResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string TemplateBase64 { get; set; }
        public byte[] ImageBuffer { get; set; }
        public uint ImageWidth { get; set; }
        public uint ImageHeight { get; set; }
    }

    /// <summary>
    /// Resposta da identificação de biometria
    /// </summary>
    public class IdentifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? MatchedId { get; set; }
        public int? Score { get; set; }
        public int? Quality { get; set; }
    }
}
