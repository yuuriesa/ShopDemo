# CustomerManagement

## Descrição

O **CustomerManagement** é um sistema para gerenciar clientes. Ele oferece funcionalidades para buscar clientes com paginação, visualizar informações de um cliente específico, adicionar novos clientes, atualizar registros de clientes, realizar importação em lote de clientes, deletar clientes e gerenciar endereços associados. Cada endpoint possui validações robustas para garantir a integridade dos dados e evitar erros no sistema.

---

## Funcionalidades

- **Buscar todos os clientes com paginação:** Permite listar clientes de forma organizada, com suporte a paginação.
- **Pegar um cliente:** Recupera informações detalhadas de um cliente específico pelo ID.
- **Adicionar cliente:** Insere novos clientes no sistema, com validação de dados.
- **Atualizar cliente:** Atualiza informações de um cliente existente.
- **Importação em lote:** Suporte para adicionar vários clientes de uma só vez por meio de importação em lote.
- **Deletar cliente:** Remove um cliente do sistema.
- **Gerenciamento de endereços:** Adiciona ou deleta endereços associados aos clientes.
- **Validações:** Impede a entrada de dados incorretos ou inválidos em todos os endpoints.

---

## Tecnologias Utilizadas

- **.NET** com **C#**
- **Entity Framework Core**
- **SQL Server**
- **Docker**
- **Swagger** (para documentação da API)
- **Postman** (para testes manuais da API, opcional)

---

## Pré-requisitos

- **Docker** instalado na máquina
- **Docker Compose** instalado

---

## Como Executar o Projeto

1. Clone este repositório:

   ```bash
   git clone https://github.com/yurisalgado21/CustomerManagement.git
   cd CustomerManagement
   ```

2. Configure o ambiente com o Docker Compose:

   ```bash
   docker-compose up -d
   ```

3. Acesse a documentação da API (Swagger) para testar os endpoints:

   - URL padrão: `http://localhost:5000/swagger`

4. Para encerrar o ambiente Docker:

   ```bash
   docker-compose down
   ```

---

## Endpoints Principais

### 1. **Clientes**

- `GET /api/customers` - Buscar todos os clientes com paginação
- `GET /api/customers/{id}` - Obter informações de um cliente pelo ID
- `POST /api/customers` - Adicionar um novo cliente
- `PUT /api/customers/{id}` - Atualizar informações de um cliente existente
- `DELETE /api/customers/{id}` - Deletar um cliente

### 2. **Importação de Clientes**

- `POST /api/customers/batch` - Importar clientes em lote
- `POST /api/customers/batch2` - Importar clientes em lote com retorno único com clientes cadastrados corretamente e os que estão inválidos.

### 3. **Endereços**

- `POST /api/customers/{id}/addresses` - Adicionar endereço a um cliente
- `DELETE /api/customers/{id}/addresses/{addressId}` - Deletar um endereço de um cliente

---

## Validações

- Todos os campos obrigatórios são validados para evitar entradas inválidas.
- Limitações para formatos de dados, como CPF, CEP, e-mail, entre outros.
- Verificação de duplicidade de dados ao cadastrar clientes ou endereços.

---

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir *issues* ou enviar *pull requests* para melhorias e correções.

---

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

## Autor

- Desenvolvido por [Yuri Torres] - Github: https://github.com/yurisalgado21/
- Com base nas mentorias que tive com meu mentor, que me auxiliou e orientou em todas as etapas do desenvolvimento, o [Marcelo Castelo] - Github: https://github.com/MarceloCas/

