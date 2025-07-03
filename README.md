# BiometricAPI

API para Gerenciamento de Dados Biométricos

Este projeto é uma API desenvolvida em .NET 6 para gerenciamento e processamento de dados biométricos, como digitais, utilizando Entity Framework Core e SQLite. O objetivo é fornecer endpoints REST para cadastro, consulta e manipulação de informações biométricas, podendo ser utilizada em sistemas de autenticação, controle de acesso, entre outros.

## Funcionalidades

- Cadastro de dados biométricos
- Consulta e listagem de registros biométricos
- Processamento e validação de dados biométricos
- Integração com biblioteca nativa para leitura biométrica

## Estrutura do Projeto

- `Controllers/`: Controllers da API (ex: `BiometricController.cs`)
- `Models/`: Modelos de dados (ex: `BiometricModels.cs`)
- `Data/`: Contexto do banco de dados (ex: `BiometricDbContext.cs`)
- `Services/`: Serviços de negócio e integração biométrica
- `lib/`: Bibliotecas nativas utilizadas
- `scripts/`: Scripts para facilitar execução e testes

## Requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Windows 10 ou superior
- (Opcional) Visual Studio 2022 ou VS Code

## Como Executar

1. **Clone o repositório:**

   ```powershell
   git clone https://github.com/seu-usuario/biometric-api.git
   cd biometric-api/app
   ```

2. **Restaure os pacotes:**

   ```powershell
   dotnet restore
   ```

3. **Compile o projeto:**

   ```powershell
   dotnet build
   ```

4. **Execute a API:**

   ```powershell
   dotnet run
   ```

   Ou utilize o script:

   ```powershell
   ./start.ps1
   ```

5. **Acesse a documentação Swagger:**
   Abra o navegador em [http://localhost:5000/swagger](http://localhost:5000/swagger) (ou porta configurada).

## Configuração

- As configurações estão nos arquivos `appsettings.json` e `appsettings.Development.json`.
- O banco de dados SQLite é criado automaticamente como `biometric.db`.
- Bibliotecas nativas para leitura biométrica estão em `lib/`.

## Testes

Utilize o script de teste para validar a API:

```powershell
./scripts/test-api.ps1
```

## Exemplos de Uso

Veja exemplos de requisições na documentação Swagger ou utilize ferramentas como Postman para testar os endpoints.

## Contribuição

Contribuições são bem-vindas! Siga os passos:

1. Fork este repositório
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas alterações (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

---

**Dúvidas ou sugestões?** Abra uma issue ou entre em contato!
