using BiometricAPI.Data;
using BiometricAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiometricAPI.Services
{
    /// <summary>
    /// Serviço para manipulação dos dados de biometria no banco de dados
    /// </summary>
    public class BiometricDataService
    {
        private readonly BiometricDbContext _context;
        private readonly ILogger<BiometricDataService> _logger;

        public BiometricDataService(BiometricDbContext context, ILogger<BiometricDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Salva um template biométrico no banco de dados
        /// </summary>
        /// <param name="templateBase64">Template em formato Base64</param>
        /// <returns>ID do registro criado</returns>
        public async Task<int> SaveBiometricAsync(string templateBase64)
        {
            _logger.LogInformation("Salvando template biométrico no banco de dados");

            var template = new BiometricTemplate
            {
                TemplateBase64 = templateBase64,
                CreatedAt = DateTime.UtcNow
            };

            _context.BiometricTemplates.Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Template biométrico salvo com ID {template.Id}");
            return template.Id;
        }

        /// <summary>
        /// Obtém todos os templates biométricos do banco de dados
        /// </summary>
        /// <returns>Lista de templates biométricos</returns>
        public async Task<List<BiometricTemplate>> GetAllBiometricsAsync()
        {
            _logger.LogInformation("Consultando todos os templates biométricos no banco de dados");
            return await _context.BiometricTemplates.ToListAsync();
        }

        /// <summary>
        /// Obtém um template biométrico pelo ID
        /// </summary>
        /// <param name="id">ID do template</param>
        /// <returns>Template biométrico ou null se não encontrado</returns>
        public async Task<BiometricTemplate> GetBiometricByIdAsync(int id)
        {
            _logger.LogInformation($"Consultando template biométrico com ID {id}");
            return await _context.BiometricTemplates.FindAsync(id);
        }

        /// <summary>
        /// Exclui todos os templates biométricos do banco de dados
        /// </summary>
        public async Task DeleteAllBiometricsAsync()
        {
            _logger.LogInformation("Excluindo todos os templates biométricos do banco de dados");
            
            _context.BiometricTemplates.RemoveRange(_context.BiometricTemplates);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Todos os templates biométricos foram excluídos do banco de dados");
        }

        /// <summary>
        /// Exclui um template biométrico pelo ID
        /// </summary>
        /// <param name="id">ID do template</param>
        /// <returns>true se o template foi excluído, false se não foi encontrado</returns>
        public async Task<bool> DeleteBiometricAsync(int id)
        {
            _logger.LogInformation($"Excluindo template biométrico com ID {id}");
            
            var template = await _context.BiometricTemplates.FindAsync(id);
            if (template == null)
            {
                _logger.LogWarning($"Template biométrico com ID {id} não encontrado para exclusão");
                return false;
            }

            _context.BiometricTemplates.Remove(template);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Template biométrico com ID {id} excluído com sucesso");
            return true;
        }
    }
}
