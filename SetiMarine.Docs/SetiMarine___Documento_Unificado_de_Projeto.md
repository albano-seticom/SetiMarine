# SetiMarine — Documento Unificado de Projeto

Este documento combina a **Visão de Negócio** com o **Prompt de Continuidade Técnico**, servindo como um guia completo para o desenvolvimento do sistema SetiMarine.

---

## Parte 1: Visão de Negócio e Requisitos (A Minha Visão)

Estou desenvolvendo um sistema para uma marina que funciona como um estacionamento de embarcações, e preciso que ele seja robusto e detalhado o suficiente para que eu possa delegar tarefas específicas para uma IA menos potente, garantindo que ela entenda o contexto e as regras de negócio.

### 1. O Core do Negócio: Vagas e Embarcações

O ponto central é a gestão das vagas. Não é um estacionamento comum; as vagas são super flexíveis:

*   **Tamanhos Variáveis:** Cada vaga tem um tamanho em pés que pode ser editado, além de largura, comprimento e boca em metros.
*   **Corredores/Sessões:** As vagas são organizadas em "corredores" ou "sessões", que também têm tamanhos variáveis e uma ordem específica.
*   **Múltiplas Embarcações por Vaga:** Uma única vaga pode abrigar mais de uma embarcação (ex: um barco e um jet ski).
*   **Tipos e Status:** As vagas podem ser secas ou molhadas, e precisam ter um status claro (livre, ocupada, em movimentação, em manutenção).
*   **Layout Visual:** As vagas têm coordenadas (X, Y, largura, altura), indicando a necessidade de uma representação visual do layout da marina.

### 2. O Cliente no Centro: Propriedade e Relacionamentos

As interações com o cliente são chave, e a forma como eles possuem as embarcações é complexa:

*   **Copropriedade (Vários Donos, Um Barco):** Uma única embarcação pode estar vinculada a vários clientes (proprietários).
*   **Múltiplas Embarcações (Um Dono, Vários Barcos):** Um cliente pode ser dono de várias embarcações.
*   **Pedidos e Contratos:** O sistema deve gerenciar os contratos e planos de cada cliente, além de receber solicitações de uso via mensagem (como WhatsApp).

### 3. A Rotina da Marina: Serviços e Movimentações

A operação diária da marina envolve muitos serviços e movimentações, que podem ser categorizados em serviços inclusos no plano de aluguel da vaga e serviços cobrados à parte. A gestão eficiente desses serviços é crucial para a satisfação do cliente e a rentabilidade da marina.

#### 3.1. Serviços Inclusos no Plano de Aluguel da Vaga

Estes serviços são parte integrante do contrato de aluguel da vaga e são oferecidos aos clientes sem custo adicional, conforme o plano contratado. A flexibilidade na configuração desses serviços é essencial.

*   **Limpeza Periódica (Editável):** O sistema deve permitir a configuração de limpezas periódicas (ex: semanal, quinzenal, mensal) para cada embarcação, conforme o plano do cliente. A frequência e o tipo de limpeza devem ser editáveis pelo gerente da marina. Esta funcionalidade pode ser integrada ao `TipoServicoConfig` e `RegistroServico`.
*   **Vistorias Periódicas:** Agendamento automático de vistorias de segurança e manutenção preventiva, com base no plano do cliente. O `ChecklistTemplate` pode ser utilizado para padronizar essas vistorias.
*   **Checklist de Saída e Retorno:** Procedimentos padronizados antes da saída e após o retorno da embarcação, garantindo a segurança e a verificação de possíveis danos. Estes utilizam `ChecklistTemplate`.

#### 3.2. Serviços Cobrados à Parte (Extras)

Estes serviços são solicitados pelos clientes de forma avulsa e geram cobranças adicionais. O sistema deve ser capaz de gerenciar a solicitação, execução e faturamento desses serviços.

*   **Manutenção e Reparos:** Clientes podem solicitar reparos ou manutenções específicas para suas embarcações (ex: conserto de motor, pintura, elétrica). O sistema deve permitir o registro dessas solicitações, a atribuição a colaboradores ou terceiros, e o acompanhamento do status. Uma nova entidade `ServicoExtra` ou a expansão de `RegistroServico` pode ser necessária para diferenciar esses serviços.
*   **Abastecimento:** Serviço de reabastecimento de combustível, solicitado conforme a necessidade do cliente. Pode ser um `RegistroServico` com um `TipoServico` específico.
*   **Venda de Itens:** A marina pode vender itens como coletes salva-vidas, óleos, peças de reposição, etc. O sistema deve integrar um módulo de vendas para registrar essas transações. Isso pode requerer uma nova entidade `VendaProduto` e `Produto`.
*   **Outros Serviços Personalizados:** Qualquer outro serviço que o cliente possa solicitar e que não esteja incluso no plano padrão. A flexibilidade para adicionar novos tipos de serviços extras é importante.

#### 3.3. Fluxo de Pedido de Uso e Serviços

*   **Pedido de Uso:** O cliente avisa que vai usar a embarcação, gerando um "pedido de uso" (`PedidoUso`) no sistema.
*   **Preparativos Pré-Uso:** Antes da saída, a equipe realiza serviços como limpeza, abastecimento e um checklist de segurança. Estes são registrados como `RegistroServico`.
*   **Serviços Pós-Uso:** Quando a embarcação volta, é feita uma nova limpeza e um checklist de retorno para verificar danos, permitindo anexar fotos (`FotoServico`).
*   **Movimentação:** Toda a logística de "descer" e "subir" a embarcação é registrada como uma "movimentação" (`Movimentacao`) com status e histórico.

### 4. A Equipe em Ação: Gerentes e Colaboradores

O sistema precisa ter uma camada de gestão operacional para a equipe da marina:

*   **Perfis de Acesso:** Teremos pelo menos dois níveis de acesso: **Gerente (Admin)** e **Colaborador (Operacional)**.
*   **Painel do Gerente:** O gerente terá um painel de controle para ver todas as tarefas pendentes (ex: 5 limpezas quinzenais). A partir desse painel, ele poderá **atribuir tarefas específicas** para cada colaborador (ex: barcos 1, 3, 5 para o Colaborador A; barcos 2, 4 para o Colaborador B).
*   **Painel do Colaborador:** Cada colaborador terá seu próprio login e verá um painel simplificado, mostrando **apenas as ordens de serviço que foram atribuídas a ele**.
*   **Serviços para Terceiros:** Haverá situações em que um serviço será executado por um terceiro, que não tem acesso ao sistema. Nesses casos, o gerente precisa ter a opção de **imprimir uma versão física da ordem de serviço** para entregar ao prestador externo.

### 6. Painel do Gerente: Minha Empresa

O painel "Minha Empresa" é projetado para oferecer ao gerente da marina uma visão abrangente e ferramentas de gestão para sua própria empresa. Este módulo permite o acesso a informações cadastrais, gestão de usuários e visualização do plano atual, garantindo autonomia e controle sobre as operações internas.

#### 6.1. Visão Geral e Navegação

O gerente terá acesso a um dashboard principal que consolida informações relevantes, como consultas disponíveis e status de usuários. A navegação é intuitiva, com menus claros para:

*   **Dashboard:** Visão consolidada de métricas e status.
*   **Consulta Fiscal:** Acesso a informações fiscais.
*   **Histórico:** Registro de atividades e eventos passados.
*   **Calculadoras:** Ferramentas de cálculo úteis para a gestão.
*   **Tabelas:** Dados de referência.
*   **Gestão:** Funções administrativas.
*   **Relatórios:** Geração de relatórios diversos.
*   **Usuários:** Gerenciamento de contas de usuários da empresa.
*   **Parametrização:** Configurações específicas da empresa.
*   **Minha Empresa:** Detalhes cadastrais e do plano.

#### 6.2. Dados da Empresa

Esta subseção exibe as informações cadastrais da empresa, permitindo ao gerente revisar e manter os dados atualizados. Os campos incluem:

| Campo           | Descrição                                    |
| :-------------- | :------------------------------------------- |
| **Razão Social**  | Nome legal completo da empresa               |
| **Nome Fantasia** | Nome comercial da empresa                    |
| **CNPJ**          | Cadastro Nacional da Pessoa Jurídica         |
| **Telefone**      | Número de contato principal                  |
| **Email**         | Endereço de email para comunicações          |

#### 6.3. Plano Atual e Consumo

O gerente pode visualizar os detalhes do plano de assinatura atual da empresa, incluindo:

*   **Plano atual:** Nome do plano contratado (ex: Starter).
*   **Consultas disponíveis:** Limite de consultas ou recursos restantes.
*   **Vencimento:** Data de renovação ou expiração do plano.

#### 6.4. Gestão de Usuários

Este módulo permite ao gerente gerenciar os usuários associados à sua empresa. A interface exibe uma lista de usuários com as seguintes informações e ações:

| Campo           | Descrição                                    | Ações Disponíveis                                                              |
| :-------------- | :------------------------------------------- | :----------------------------------------------------------------------------- |
| **Nome**          | Nome do usuário                              |                                                                                |
| **Email**         | Endereço de email do usuário                 |                                                                                |
| **Perfil**        | Nível de acesso (ex: Operador, Admin)        |                                                                                |
| **Status**        | Situação da conta (Ativo, Inativo)           |                                                                                |
| **Último acesso** | Data e hora do último login do usuário       |                                                                                |
| **Ações**         | Opções para editar, desativar ou remover usuário | Criação de novos usuários, edição de perfis e parametrizações individuais.

### 7. Painel Exclusivo SETICOM: Super Admin

O painel "Super Admin" é uma interface de gestão global, exclusiva para uso da SETICOM Tecnologia. Ele oferece uma visão macro do sistema, permitindo o gerenciamento centralizado de todas as empresas cadastradas, usuários, planos e parametrizações em nível de sistema. Este painel é crucial para a administração e manutenção da plataforma SetiMarine.

#### 7.1. Visão Geral da Gestão Global

O dashboard do Super Admin apresenta um resumo executivo das principais métricas do sistema:

| Métrica             | Descrição                                                              |
| :------------------ | :--------------------------------------------------------------------- |
| **Total de empresas** | Número total de empresas cadastradas, com destaque para as ativas.     |
| **Total de usuários** | Soma de todos os usuários em todas as empresas.                        |
| **Planos cadastrados**| Quantidade de planos de serviço disponíveis, com destaque para os ativos. |
| **Consultas hoje**    | Total de consultas realizadas em todas as empresas no dia corrente.    |

#### 7.2. Gestão de Empresas

Esta seção permite à SETICOM gerenciar todas as empresas que utilizam o sistema. A interface de busca facilita a localização de empresas por razão social, CNPJ ou e-mail. A tabela de empresas exibe as seguintes informações:

| Campo         | Descrição                                                              |
| :------------ | :--------------------------------------------------------------------- |
| **Empresa**     | Razão Social e Nome Fantasia da empresa.                               |
| **CNPJ**        | Cadastro Nacional da Pessoa Jurídica da empresa.                       |
| **Email**       | E-mail de contato principal da empresa.                                |
| **Plano**       | Plano de assinatura atual da empresa.                                  |
| **Usuários**    | Número de usuários ativos associados à empresa.                        |
| **Consultas**   | Número de consultas realizadas pela empresa.                           |
| **Status**      | Situação da empresa no sistema (Ativa, Inativa, Suspensa).             |
| **Ações**       | Opções para visualizar detalhes, editar, ativar/inativar ou gerenciar usuários da empresa. |

#### 7.3. Gestão de Planos

O módulo de Planos permite à SETICOM cadastrar e gerenciar os diferentes planos de serviço oferecidos. Esses planos serão exibidos na home do site e no menu "Planos" dentro do sistema, onde os clientes poderão visualizar seu plano atual e comparar com outras opções. As funcionalidades incluem:

*   **Criação de Planos:** Definição de novos planos com nome, descrição, funcionalidades incluídas, limites de uso (ex: número de consultas, usuários) e preço.
*   **Edição de Planos:** Modificação de planos existentes.
*   **Ativação/Inativação:** Controle sobre quais planos estão visíveis e disponíveis para contratação.

#### 7.4. Parametrização do Sistema (Nível SETICOM)

Esta área é dedicada a configurações globais que afetam o comportamento do sistema para todas as empresas. Inclui:

*   **Configurações Gerais:** Parâmetros padrão para novas empresas, limites de sistema, etc.
*   **Integrações:** Gerenciamento de chaves de API e configurações para serviços externos.
*   **Modelos Padrão:** Definição de templates de e-mail, relatórios ou checklists que podem ser usados como base pelas empresas.

### 8. Fluxo de Onboarding e Login Multi-empresa

O sistema SetiMarine é concebido como uma plataforma SaaS multi-empresa, onde cada marina opera de forma independente, com seus dados isolados das demais. O processo de entrada no sistema é dividido em duas frentes principais: o **Login** para usuários já cadastrados e o **Onboarding Self-Service** para novas empresas.

#### 8.1. Portal de Acesso (Home do Site)

A home do site do SetiMarine servirá como o ponto de entrada principal para novos clientes e usuários existentes. No canto superior direito, estarão disponíveis dois botões distintos:

*   **Entrar:** Redireciona o usuário para a tela de login, onde ele poderá acessar o sistema com suas credenciais.
*   **Cadastrar:** Inicia o fluxo de onboarding para novas empresas, permitindo que elas se registrem e configurem sua conta de forma autônoma.

#### 8.2. Processo de Onboarding Self-Service (Cadastro de Nova Empresa)

O cadastro de uma nova empresa é um processo guiado em três etapas, projetado para ser intuitivo e coletar as informações essenciais para a criação da conta e a configuração inicial do ambiente multi-empresa.

##### 8.2.1. Etapa 1: Dados da Empresa

| Campo             | Descrição                                                                 | Obrigatoriedade |
| :---------------- | :------------------------------------------------------------------------ | :-------------- |
| **Razão Social**    | Nome legal completo da empresa (ex: Empresa Exemplo Ltda)                 | Sim             |
| **Nome Fantasia**   | Nome comercial da empresa                                                 | Não             |
| **CNPJ**            | Cadastro Nacional da Pessoa Jurídica (formato: 00.000.000/0001-00)        | Sim             |
| **Telefone**        | Telefone de contato da empresa (formato: (00) 00000-0000)                 | Não             |
| **E-mail da empresa** | Endereço de e-mail principal para comunicações e recuperação de conta     | Sim             |
| **Plano**           | Seleção do plano de assinatura (ex: Starter, Basic, Premium)              | Sim             |

##### 8.2.2. Etapa 2: Dados do Administrador

Nesta etapa, o usuário que está realizando o cadastro será configurado como o primeiro administrador da conta da empresa.

##### 8.2.3. Etapa 3: Confirmação

A etapa final consiste na revisão e confirmação de todos os dados fornecidos. Após a validação, a conta da empresa será criada e o administrador terá acesso ao painel.

#### 8.3. Isolamento Multi-empresa

*   Cada empresa possui seu próprio conjunto de dados (vagas, embarcações, clientes, serviços, etc.).
*   Usuários de uma empresa não podem visualizar ou interagir com dados de outras empresas.
*   Todas as consultas e operações dentro do sistema são automaticamente filtradas pelo `EmpresaId` do usuário logado.

### 5. Desafios Técnicos e Estrutura

Para que tudo isso funcione, o sistema precisa ser muito bem estruturado. Já temos a base com as entidades de domínio e a arquitetura multi-empresa. O desafio agora é pegar cada uma dessas regras e transformá-las em código, classe por classe, método por método, de forma que até uma IA mais simples consiga entender e implementar as tarefas que eu for passando.

---

## Parte 2: Prompt de Continuidade Técnico para IA

### Contexto

Você é um assistente técnico especializado em sistemas de gestão de marinas. Estamos desenvolvendo o **SetiMarine** — um sistema SaaS para gerenciar marinas, focando em vagas, embarcações, serviços e movimentações.

### Stack Tecnológica

- **Backend:** .NET 8, C#
- **Frontend:** Blazor Server
- **Banco de dados:** PostgreSQL
- **ORM:** Entity Framework Core 8 com Migrations

### Estrutura de Pastas do Projeto

```
C:\Projetos\SetiMarine\
  SetiMarine.Domain\
    Entities\          ← todas as entidades de domínio
    Enums\             ← todos os enums
    Data\
      ISetiMarineDbContext.cs
  SetiMarine.Application\
    Services\          ← serviços de aplicação (AuthService, VagaService, etc.)
  SetiMarine.Infrastructure\
    Data\
      AppDbContext.cs  ← DbContext concreto
    Migrations\        ← migrations do EF Core
    Services\
  SetiMarine.Web\
    Components\
      Pages\
        Public\        ← páginas públicas (Home, Login, Cadastro)
        App\           ← páginas internas autenticadas
          Vagas\       ← Index, Form, Layout
          Corredores\  ← Index, Form
      StatusVagaBadge.razor
    Endpoints\
      AuthEndpoints.cs ← endpoint POST /auth/login e GET /auth/logout
    wwwroot\
      js\
        layoutMarina.js
```

### Padrão de Acesso a Dados

- Os serviços em `Application` usam **`ISetiMarineDbContext`** (interface) — NUNCA `AppDbContext` diretamente.
- O `AppDbContext` só é referenciado em `Infrastructure` e no `Program.cs`.
- O `IDbContextFactory<AppDbContext>` é usado em páginas Blazor que precisam de contexto por operação.

### Autenticação

- **Cookie Authentication** via `Microsoft.AspNetCore.Authentication.Cookies`.
- Login via **endpoint HTTP POST `/auth/login`** (não via Blazor — cookies não podem ser escritos dentro de um circuito Blazor Server após a resposta ter sido enviada).
- Logout via **GET `/auth/logout`**.
- O `AuthService` faz hash de senha com SHA256 + salt `"SetiMarine_Salt_2026"`.
- Ao fazer login, cria um registro em `SessoesAtivas`.

### Entidades Implementadas

| Entidade | Tabela | Status |
|---|---|---|
| Empresa | sis_empresas | ✅ |
| Plano | cfg_planos | ✅ |
| Usuario | sis_usuarios | ✅ |
| SessaoAtiva | sis_sessoes | ✅ |
| Configuracao | cfg_configuracoes | ✅ |
| Cliente | mar_clientes | ✅ |
| Embarcacao | mar_embarcacoes | ✅ |
| **ClienteEmbarcacao** | **mar_cliente_embarcacoes** | ⚠️ **Pendente** |
| Contrato | mar_contratos | ✅ |
| Corredor | mar_corredores | ✅ |
| Vaga | mar_vagas | ✅ |
| VagaEmbarcacao | mar_vaga_embarcacoes | ✅ |
| Movimentacao | mar_movimentacoes | ✅ |
| HistoricoMovimentacao | mar_historico_movimentacoes | ✅ |
| PedidoUso | mar_pedidos_uso | ✅ |
| TipoServicoConfig | mar_tipos_servico_config | ✅ |
| ChecklistTemplate | mar_checklist_templates | ✅ |
| ChecklistTemplateItem | mar_checklist_template_itens | ✅ |
| RegistroServico | mar_registros_servico | ✅ |
| RegistroServicoItem | mar_registro_servico_itens | ✅ |
| FotoServico | mar_fotos_servico | ✅ |
| OrdemServico | mar_ordens_servico | ✅ |
| **ServicoExtra** | **mar_servicos_extras** | ⚠️ **Pendente** |
| **Produto** | **mar_produtos** | ⚠️ **Pendente** |
| **VendaProduto** | **mar_vendas_produtos** | ⚠️ **Pendente** |

### Serviços Implementados

| Serviço | Localização | Status |
|---|---|---|
| `AuthService` | Application/Services | ✅ |
| `CorredorService` | Application/Services | ✅ |
| `VagaService` | Application/Services | ✅ |
| `AlocacaoService` | Application/Services | ✅ |

### Páginas Implementadas

| Página | Rota | Localização | Status |
|---|---|---|---|
| Home (landing page) | `/` | Components/Pages/Public/Home.razor | ✅ |
| Login | `/login` | Components/Pages/Public/Login.razor | ✅ |
| Cadastro (onboarding 3 etapas) | `/cadastro` | Components/Pages/Public/Cadastro.razor | ✅ |
| Vagas - Listagem | `/vagas` | Components/Pages/App/Vagas/Index.razor | ✅ |
| Vagas - Formulário | `/vagas/nova`, `/vagas/{id}` | Components/Pages/App/Vagas/Form.razor | ✅ |
| Vagas - Layout Visual | `/vagas/layout` | Components/Pages/App/Vagas/Layout.razor | ✅ |
| Corredores - Listagem | `/corredores` | Components/Pages/App/Corredores/Index.razor | ✅ |
| Corredores - Formulário | `/corredores/novo`, `/corredores/{id}` | Components/Pages/App/Corredores/Form.razor | ✅ |

### Enums Implementados

```csharp
public enum PerfilUsuario { SuperAdmin, Admin, Operacional }
public enum TipoVaga { Agua, Seco }
public enum StatusVaga { Livre, Ocupada, EmMovimentacao, Manutencao }
public enum TipoEmbarcacao { Lancha, Veleiro, Escuna, JetSki, Outro }
public enum TipoContrato { Mensalista, Diaria, Visitante }
public enum StatusOS { Pendente, EmExecucao, Finalizado }
public enum StatusMovimentacao { Agendado, AguardandoPreparo, EmPreparo, Pronto, Descendo, NaAgua, Retornando, Subindo, Finalizado }
public enum TipoServico { LimpezaQuinzenal, LimpezaPosPasseio, VistoriaPeriodica, ChecklistSaida, ChecklistRetorno, Abastecimento, Outro }
public enum StatusServico { Pendente, EmExecucao, Finalizado, Cancelado }
public enum StatusItemChecklist { Pendente, Ok, Problema, NaoAplica }
public enum OrigemPedido { Manual, WhatsApp, Portal }
public enum StatusPedidoUso { Recebido, Confirmado, EmPreparo, Pronto, Cancelado }
public enum TipoServicoCobranca { InclusoNoPlano, CobradoAParte }
public enum FrequenciaPeriodica { Semanal, Quinzenal, Mensal, Anual }
```

### Regras de Negócio Essenciais

1.  **Gestão de Equipe e Atribuição de Tarefas:**
    *   O `Usuario` tem um `PerfilUsuario`: `Admin` (Gerente) e `Operacional` (Colaborador).
    *   O Gerente atribui um `RegistroServico` ou `OrdemServico` a um Colaborador preenchendo o campo `ResponsavelId`.
    *   O Colaborador só pode ver as tarefas onde ele é o `Responsavel`.
    *   O Gerente pode imprimir uma OS para prestadores de serviço externos (que não são usuários).

2.  **Relacionamento Cliente-Embarcação (N:N):**
    *   Será implementado através da entidade de junção `ClienteEmbarcacao` (pendente).

3.  **Gestão de Vagas Flexível:**
    *   `Vaga` tem tamanhos e coordenadas editáveis para um layout visual drag-and-drop.
    *   Uma `Vaga` pode ter múltiplas embarcações (relação N:N com `Embarcacao` via `VagaEmbarcacao`).
    *   Ao alocar embarcação em nova vaga, a alocação anterior é encerrada automaticamente (`SaidaEm` preenchido).

4.  **Pedidos, Serviços e Movimentações:**
    *   `PedidoUso` dispara `Movimentacao` e `RegistroServico`.
    *   Serviços periódicos são gerados automaticamente.
    *   `RegistroServico` usa `ChecklistTemplate` e permite anexar `FotoServico`.

5.  **Autenticação:**
    *   Login via POST `/auth/login` (endpoint HTTP, não Blazor).
    *   Cookie de autenticação com duração de 12 horas.
    *   Senha armazenada como SHA256 com salt fixo.

### Padrões Obrigatórios (CRÍTICOS)

1.  **Multi-empresa:** Todo dado deve ser filtrado por `EmpresaId`.
2.  **Camadas:** Serviços em `Application` usam `ISetiMarineDbContext`, nunca `AppDbContext`.
3.  **Login:** Sempre via endpoint HTTP, nunca `SignInAsync` dentro de componente Blazor.
4.  **Arquivos duplicados:** O projeto usa `Components/Pages/` como raiz das páginas Blazor. Não criar páginas em `Pages/` (pasta legada removida).
5.  **Status:** Utilizar os `Enums` definidos para todos os campos de status.

### Pendências / Próximos Passos Prioritários

#### Alta prioridade

- [ ] **Módulo de Gestão Operacional e Colaboradores:**
    - [ ] **Controle de Acesso por Perfil:** Implementar autorização baseada em `PerfilUsuario`. Gerente (`Admin`) acessa páginas de configuração, Colaborador (`Operacional`) não.
    - [ ] **Painel do Gerente (Atribuição de Tarefas):** Criar uma página onde o gerente possa ver todos os `RegistroServico` com status `Pendente`, selecionar um ou mais, e atribuí-los a um `Usuario` (colaborador) através do campo `ResponsavelId`.
    - [ ] **Painel do Colaborador (Minhas Tarefas):** Criar uma página onde o colaborador logado vê uma lista de `RegistroServico` e `OrdemServico` onde `ResponsavelId` é igual ao seu próprio ID.
    - [ ] **Impressão de Ordem de Serviço:** Criar uma view/componente de impressão para `OrdemServico` e `RegistroServico`, formatada para papel, que o gerente possa usar para terceiros.

- [ ] **Módulo de Gestão de Embarcações e Clientes (Refatoração N:N):**
    - [ ] **Nova Entidade:** Criar a entidade `ClienteEmbarcacao` com `ClienteId` e `EmbarcacaoId`.
    - [ ] **Ajustar Entidades:** Remover `ClienteId` de `Embarcacao` e a coleção de `Embarcacao` de `Cliente`. Substituir por coleções de `ClienteEmbarcacao` em ambas.
    - [ ] **Atualizar DbContext e Interface:** Adicionar `DbSet<ClienteEmbarcacao>` em `AppDbContext` e `ISetiMarineDbContext`.
    - [ ] **Criar Migration:** Gerar e aplicar a nova migration para a estrutura N:N.
    - [ ] **Lógica de Associação:** Desenvolver UI para associar/desassociar múltiplos `Cliente`s a uma `Embarcacao`.

- [x] **Módulo de Gestão de Vagas:** ✅ CONCLUÍDO
    - [x] Implementar CRUDs para `Corredor` e `Vaga`.
    - [x] Desenvolver lógica para alocar/desalocar `Embarcacao` em `Vaga` via `VagaEmbarcacao`.
    - [x] Desenvolver componente de UI para visualização gráfica do layout da marina com drag-and-drop.

#### Média prioridade

- [ ] **Módulo de Pedidos de Uso e Movimentação:**
    - [ ] Implementar funcionalidade para criar `PedidoUso`.
    - [ ] Desenvolver lógica para processar `PedidoUso`, gerando `Movimentacao` e `RegistroServico`.

- [ ] **Módulo de Gestão de Serviços:**
    - [ ] Implementar CRUDs para `ChecklistTemplate` e `TipoServicoConfig`.
    - [ ] Implementar geração automática de serviços periódicos.
    - [ ] **Serviços Cobrados à Parte:** Criar CRUD para `ServicoExtra`.
    - [ ] **Venda de Produtos:** Criar CRUDs para `Produto` e `VendaProduto`.
    - [ ] **Atualizar Registro de Serviço:** Adicionar campos `TipoServicoCobranca` e `Custo` a `RegistroServico`.
    - [ ] **Atualizar Configuração de Tipo de Serviço:** Adicionar campos `EhPeriodico` e `FrequenciaPeriodica` a `TipoServicoConfig`.

- [ ] **Módulo de Gestão de Planos e Contratos:**
    - [ ] Implementar CRUDs para `Plano` e `Contrato`.

---

## Histórico de Desenvolvimento

### Sessão 1 — 17 de Abril de 2026

**O que foi feito:**

1. **Análise do projeto e planejamento** do Módulo de Vagas como primeiro entregável.

2. **Serviços de aplicação criados:**
   - `CorredorService` — CRUD completo de corredores com reordenação.
   - `VagaService` — CRUD de vagas + atualização de posição X/Y para drag-and-drop.
   - `AlocacaoService` — aloca/desaloca embarcações com histórico via `VagaEmbarcacao`.
   - `AuthService` — login com cookie, cadastro de empresa + admin em transação.

3. **Páginas Blazor criadas:**
   - `Corredores/Index.razor` e `Form.razor`
   - `Vagas/Index.razor` e `Form.razor`
   - `Vagas/Layout.razor` — layout visual drag-and-drop com zoom/pan
   - `Login.razor` — reformulado para usar form HTML POST (solução para bug de cookie em Blazor Server)
   - `Cadastro.razor` — onboarding em 3 etapas funcional

4. **Infraestrutura:**
   - `AuthEndpoints.cs` — endpoint `/auth/login` e `/auth/logout` fora do circuito Blazor.
   - `ISetiMarineDbContext` expandida com todos os DbSets.
   - `SessaoAtiva` atualizada com campos `EmpresaId`, `ExpiraEm`, `IniciadaEm`.
   - `AppDbContext` corrigido — removidas 8 duplicações do relacionamento `VagaFixa`.
   - Autenticação por cookie reativada no `Program.cs`.
   - Migration `AddSessaoAtivaFix2` aplicada com sucesso usando SQL idempotente (`IF NOT EXISTS`).

5. **Problemas encontrados e resolvidos:**
   - Arquivos duplicados em `Pages/` e `Components/Pages/` causando `AmbiguousMatchException` — resolvido removendo a pasta `Pages/` legada.
   - `SignInAsync` dentro de circuito Blazor lançando `Headers are read-only` — resolvido movendo login para endpoint HTTP dedicado.
   - Migration falhando por colunas já existentes no banco — resolvido reescrevendo a migration com blocos `DO $$ IF NOT EXISTS`.
   - `AuthService` referenciando `AppDbContext` (Infrastructure) de dentro de Application — corrigido para usar `ISetiMarineDbContext`.

**Estado ao final da sessão:**
- Sistema rodando em `https://localhost:59056`
- Fluxo completo funcionando: `/` → `/cadastro` → `/login` → `/vagas`
- Primeira empresa e usuário admin criados com sucesso via onboarding

---

**Autor:** Manus AI / Claude Sonnet
**Criado em:** 16 de Abril de 2026
**Última atualização:** 17 de Abril de 2026
