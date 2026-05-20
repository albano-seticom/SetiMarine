# 🚢 Sistema de Gestão de Marina — Projeto Reformulado

> Documento vivo de planejamento. Atualizar conforme decisões forem sendo tomadas.

---

## 1. Visão Geral

Sistema web **Blazor Server** multitenant para gestão completa de marinas. Cada marina é uma **Empresa** com seu próprio espaço isolado de dados. O sistema cobre:

- Mapa visual interativo da marina (vagas, decks, corredores)
- Gestão de embarcações, clientes e contratos
- Ordens de serviço (próprios e terceirizados)
- Pedidos de consumo e empréstimo
- Checklist de embarcações
- Financeiro e fiscal (cupom, NF de serviço)
- WhatsApp e comunicação

**Stack base:**
- Blazor Server (.NET)
- PostgreSQL
- Entity Framework Core
- Mercado Pago
- Evolution API / Codechat (WhatsApp)
- SignalR (tempo real)

---

## 2. Módulos do Sistema

```
MARINA
├── 2.1  Cadastros Base (pré-requisito de tudo)
│   ├── Seções / Corredores
│   ├── Tamanhos Padrão de Embarcação
│   ├── Tipos de Embarcação
│   ├── Vagas
│   ├── Empresas (multitenant)
│   ├── Usuários / Funcionários
│   ├── Clientes (Armadores)
│   └── Produtos e Serviços
│
├── 2.2  Embarcações
│   ├── Cadastro de embarcação
│   ├── Vínculo cliente ↔ embarcação
│   ├── Documentos (PROA, seguro, vistoria...)
│   └── Checklist / Inventário da embarcação
│
├── 2.3  Mapa Visual (tela principal)
│   ├── Vista de cima da marina
│   ├── Vagas coloridas por status
│   ├── Drag & drop de embarcações → vagas
│   └── Painel lateral: modelos disponíveis
│
├── 2.4  Ordens de Serviço
│   ├── OS por funcionário próprio
│   ├── OS por terceiro
│   ├── Itens de serviço e produtos
│   └── Geração de NF de serviço (quando aplicável)
│
├── 2.5  Pedidos de Consumo
│   ├── Pedido em aberto (cliente pode devolver)
│   ├── Fechamento do pedido com total real
│   └── Geração de cupom fiscal
│
├── 2.6  Pedidos de Empréstimo
│   ├── Colete, boia, equipamentos
│   └── Controle de devolução
│
├── 2.7  Financeiro
│   ├── Contratos de permanência / mensalidades
│   ├── Cobranças e inadimplência
│   ├── Pagamentos via Mercado Pago (PIX / cartão)
│   └── Relatórios financeiros
│
└── 2.8  Comunicação
    ├── WhatsApp (Evolution API)
    ├── Comunicados da marina
    └── Chamados / atendimento
```

---

## 3. Cadastros Base — Detalhe

### 3.1 Seções / Corredores

Equivalente às "ruas" ou alas da marina. São as divisões físicas onde ficam as vagas.

```
Exemplo:
- Seção A — Píer Principal
- Seção B — Deck Flutuante Norte
- Seção C — Área Seca (guincho)
```

**Campos:**
- Código / Nome da seção
- Descrição / localização
- Tipo: molhada | seca | coberta
- Capacidade total de vagas
- Ativo / Inativo

---

### 3.2 Tamanhos Padrão de Embarcação

> **Ideia central:** Assim como carros têm categorias (compacto, SUV, minivan, caminhonete), embarcações têm tamanhos padrão por faixa de comprimento em pés. Isso facilita o cadastro rápido e define qual vaga comporta qual embarcação.

**Tabela de tamanhos padrão:**

| Categoria         | Comprimento (pés) | Comprimento (metros) | Boca aprox. (m) | Calado aprox. (m) | Exemplos típicos              |
|:------------------|:-----------------:|:--------------------:|:---------------:|:-----------------:|:------------------------------|
| Mini / Jet        | até 15 pés        | até 4,6 m            | 1,5             | 0,4               | Jet ski, bote pequeno         |
| Pequena           | 16 – 22 pés       | 4,9 – 6,7 m          | 2,2             | 0,6               | Lancha open, bote de pesca    |
| Média             | 23 – 30 pés       | 7,0 – 9,1 m          | 2,8             | 0,8               | Lancha cabin, veleiro pequeno |
| Média-Grande      | 31 – 40 pés       | 9,5 – 12,2 m         | 3,5             | 1,0               | Lancha cabinada, veleiro      |
| Grande            | 41 – 50 pés       | 12,5 – 15,2 m        | 4,5             | 1,2               | Iate de cruzeiro, catamarã    |
| Extra Grande      | 51 – 65 pés       | 15,5 – 19,8 m        | 5,5             | 1,5               | Iate, veleiro oceânico        |
| Superyacht        | acima de 65 pés   | acima de 20 m        | variável        | variável          | Mega yachts, navios           |

> ⚙️ **Esses tamanhos são configuráveis.** A marina pode ajustar os intervalos, nomes e medidas conforme sua realidade.

**Campos da entidade TamanhoEmbarcacao:**
- Código / Nome (ex: "Média 23–30 pés")
- Comprimento mínimo (pés)
- Comprimento máximo (pés)
- Comprimento médio referência (pés)
- Boca referência (metros)
- Calado referência (metros)
- Observações

---

### 3.3 Tipos de Embarcação

Classificação por tipo (independente do tamanho):

| Tipo          | Descrição                              |
|:--------------|:---------------------------------------|
| Lancha Open   | Lancha sem cabine                      |
| Lancha Cabin  | Lancha com cabine                      |
| Veleiro       | Embarcação à vela                      |
| Catamarã      | Casco duplo                            |
| Jet Ski       | Moto aquática                          |
| Bote          | Bote simples, alumínio, inflável       |
| Iate          | Embarcação de luxo grande              |
| Saveiro       | Embarcação de trabalho                 |
| Trawler       | Motor de cruzeiro lento                |
| Outro         | Livre                                  |

**Campos:**
- Nome do tipo
- Descrição
- Ícone (para exibição no mapa)
- Ativo / Inativo

---

### 3.4 Vagas

Cada vaga pertence a uma **Seção** e tem um **tamanho máximo suportado**.

**Campos:**
- Número / Código da vaga (ex: "A-12")
- Seção a que pertence
- Tipo: flutuante | seca | rampa | coberta
- Comprimento máximo suportado (pés)
- Largura (boca máxima em metros)
- Calado máximo suportado (metros)
- Tamanho padrão suportado (FK → TamanhoEmbarcacao)
- Possui energia elétrica (S/N)
- Possui água (S/N)
- Possui câmera (S/N)
- Valor da mensalidade base
- Status: livre | ocupada | reservada | manutenção | inativa
- Posição no mapa (X, Y — para o mapa visual)
- Observações

---

### 3.5 Empresas (Multitenant)

Cada marina é uma empresa no sistema.

**Campos relevantes:**
- Razão social / Nome fantasia
- CNPJ
- Endereço completo
- Logo
- Plano de assinatura ativo
- Data de vencimento do plano
- Bloqueada (S/N) + motivo
- Configurações da marina (moeda, fuso horário, etc.)

---

### 3.6 Usuários / Funcionários

**Perfis de acesso:**
- Administrador (acesso total)
- Gerente (acesso operacional completo)
- Atendente (balcão, pedidos, clientes)
- Técnico (ordens de serviço próprias)
- Portaria (movimentações e checklist)
- Armador (portal do cliente — acesso externo, dados próprios)

**Campos:**
- Nome completo
- CPF / E-mail
- Telefone / WhatsApp
- Perfil de acesso
- Senha (hash)
- Ativo / Inativo
- Empresa (FK)

---

### 3.7 Clientes (Armadores)

**Campos:**
- Nome / Razão Social
- CPF ou CNPJ
- E-mail
- Telefone / WhatsApp
- Endereço
- Tipo: pessoa física | pessoa jurídica
- Data de cadastro
- Observações
- Ativo / Inativo

> Um cliente pode ter **N embarcações** e uma embarcação pode ter **N clientes** (sócios, cônjuge, gerente).

---

### 3.8 Produtos e Serviços

Catálogo unificado usado em pedidos de consumo e ordens de serviço.

**Tipos:**
- Produto físico (cerveja, mantimentos, combustível, peças)
- Serviço (mão de obra, serviço de terceiro)
- Item de empréstimo (colete, boia — não gera cobrança direta)

**Campos:**
- Nome / Código
- Tipo (produto | serviço | empréstimo)
- Categoria (bebidas, peças, manutenção, segurança...)
- Unidade de medida
- Preço de venda
- Custo médio
- Estoque atual (para produtos físicos)
- Estoque mínimo (alerta)
- Fornecedor padrão
- Tributação (NCM, CFOP para cupom/NF)
- Pode ser terceirizado (S/N)
- Ativo / Inativo

---

## 4. Embarcações

### 4.1 Cadastro

**Campos:**
- Nome da embarcação
- Tipo de embarcação (FK → TipoEmbarcacao)
- Tamanho padrão (FK → TamanhoEmbarcacao)
- Comprimento real (pés)
- Boca real (metros)
- Calado real (metros)
- Fabricante / Estaleiro
- Modelo
- Ano de fabricação
- Material do casco (fibra, alumínio, madeira, aço)
- Motorização (quantidade e HP)
- Cor(es)
- Número de registro (PROA / INSCRIÇÃO)
- Ativo / Inativo
- Fotos da embarcação

### 4.2 Vínculo Cliente ↔ Embarcação

- Um cliente pode ser dono, sócio ou responsável
- Papel: Proprietário | Coproprietário | Responsável | Usuário autorizado
- Data início / fim do vínculo
- Notificações ativas (S/N) — recebe alertas via WhatsApp

### 4.3 Documentos da Embarcação

| Documento          | Descrição                          |
|:-------------------|:-----------------------------------|
| PROA               | Registro federal                   |
| Seguro             | Apólice de seguro                  |
| Vistoria           | Certificado de vistoria            |
| Habilitação        | Habilitação do responsável         |
| Outros             | Livre                              |

- Data de emissão
- Data de vencimento
- Arquivo (PDF / imagem)
- Status: válido | vencendo (30 dias) | vencido

> 🔴 Documentos vencidos geram alertas automáticos via WhatsApp e ficam destacados no sistema.

---

## 5. Mapa Visual — Tela Principal

Esta é a **tela central do sistema**. É uma vista de cima da marina mostrando toda a distribuição de vagas.

### 5.1 Layout

```
┌─────────────────────────────────────────────────┬────────────────────┐
│                                                 │  PAINEL LATERAL    │
│           MAPA VISUAL DA MARINA                 │                    │
│                                                 │  Embarcações       │
│   [Seção A]   [Seção B]   [Seção C]             │  sem vaga:         │
│                                                 │                    │
│   [ ][ ][ ]   [■][■][ ]   [ ][ ]               │  🚢 Mia Bella      │
│   [ ][ ][■]   [ ][■][ ]   [■][ ]               │  🚢 Sea Dream      │
│   [■][ ][ ]   [ ][ ][■]   [ ][ ]               │  ⛵ Vento Sul      │
│                                                 │                    │
│   Legenda:                                      │  [Arraste para     │
│   🟢 Livre  🟡 Pendência  🔴 Atraso  🔵 Reserva │   uma vaga]        │
│                                                 │                    │
└─────────────────────────────────────────────────┴────────────────────┘
```

### 5.2 Cores das vagas

| Cor      | Significado                                         |
|:---------|:----------------------------------------------------|
| 🟢 Verde | Ocupada — todos os serviços e documentos em dia     |
| 🟡 Amarelo | Ocupada — serviço ou documento vencendo em breve  |
| 🔴 Vermelho | Ocupada — serviço vencido ou documento vencido   |
| ⚫ Cinza  | Vaga livre                                          |
| 🔵 Azul  | Reservada (temporária, agendamento)                 |
| 🟠 Laranja | Em manutenção (vaga indisponível)                 |

### 5.3 Interação com as vagas

**Clicando em uma vaga ocupada:**
- Nome da embarcação
- Cliente(s) vinculado(s)
- Status dos serviços (lista rápida)
- Status dos documentos
- Botões: Ver OS | Novo Pedido | Checklist | Histórico

**Drag & Drop (painel lateral → mapa):**
- Arrasta embarcação sem vaga para uma vaga livre
- Sistema valida se a vaga comporta o tamanho da embarcação
- Confirma o vínculo e registra a movimentação

### 5.4 Filtros do mapa

- Por seção
- Por status (só os vermelhos, só os livres...)
- Por tipo de embarcação
- Busca por nome de embarcação ou cliente

---

## 6. Ordens de Serviço (OS)

### 6.1 Conceito

A OS é o coração da operação de manutenção. Pode ser gerada por:
- Funcionário próprio da marina
- Terceiro (empresa ou autônomo contratado)

### 6.2 Fluxo

```
Abertura da OS
    ↓
Definir: funcionário próprio ou terceiro?
    ↓
Adicionar itens de serviço e produtos utilizados
    ↓
Registrar fotos (antes / durante / depois)
    ↓
Executar (funcionário marca progresso)
    ↓
Concluída → cliente aprova (opcional, via WhatsApp)
    ↓
Gerar cobrança / NF de serviço (quando aplicável)
    ↓
OS encerrada
```

### 6.3 Tipos de Execução

| Tipo           | Descrição                                              |
|:---------------|:-------------------------------------------------------|
| Próprio        | Funcionário da marina executa                          |
| Terceiro       | Empresa ou autônomo externo                            |
| Misto          | Parte próprio, parte terceiro                          |

### 6.4 Campos da OS

- Número da OS (gerado automaticamente)
- Embarcação
- Vaga / localização
- Data de abertura
- Previsão de conclusão
- Tipo (próprio | terceiro | misto)
- Funcionário responsável (se próprio)
- Terceiro responsável (FK → Fornecedor, se terceiro)
- Descrição do serviço
- Itens de serviço (serviços + produtos)
- Fotos
- Checklist específico da OS
- Status: aberta | em andamento | aguardando peça | concluída | cancelada
- Valor total
- Gerar NF de serviço (S/N)
- Observações

### 6.5 Pedido de Produtos para OS

Quando a OS precisa de produtos:
- Produto disponível no estoque da marina → baixa no estoque
- Produto indisponível → gera **pedido de compra** para o fornecedor

---

## 7. Pedidos de Consumo

### 7.1 Conceito

Clientes podem consumir produtos da marina (bebidas, mantimentos, itens de conveniência) com o pedido ficando em aberto até o fechamento.

### 7.2 Regra de negócio — Pedido em Aberto

```
Exemplo:
Cliente solicita 6 caixas de cerveja.
→ Pedido aberto: 6 cx × R$ 80,00 = R$ 480,00

Cliente consome apenas 4 caixas e devolve 2.
→ Devolução registrada: -2 cx × R$ 80,00 = -R$ 160,00

Fechamento do pedido:
→ Total final: R$ 320,00 (4 caixas consumidas)
→ Gera cupom fiscal com o valor real
```

### 7.3 Status do Pedido

| Status          | Descrição                                    |
|:----------------|:---------------------------------------------|
| Aberto          | Itens entregues, aguardando consumo/devolução |
| Parcialmente devolvido | Parte devolvida, pedido ainda não fechado |
| Fechado         | Consumo finalizado, cupom fiscal gerado      |
| Cancelado       | Pedido cancelado antes do consumo            |

### 7.4 Campos

- Número do pedido
- Embarcação / cliente
- Data de abertura
- Itens (produto, quantidade, preço unitário)
- Devoluções (produto, quantidade devolvida, data)
- Total bruto / total de devoluções / total real
- Status
- Cupom fiscal gerado (FK → CupomFiscal)
- Funcionário que registrou

---

## 8. Pedidos de Empréstimo

### 8.1 Conceito

A marina pode emprestar equipamentos de segurança e outros itens que devem ser devolvidos.

**Exemplos de itens emprestáveis:**
- Coletes salva-vidas
- Boias
- Cabos / amarras
- Equipamentos de sinalização
- Âncoras

### 8.2 Fluxo

```
Solicitação do empréstimo
    ↓
Registrar itens e quantidade emprestada
    ↓
Registrar saída (data/hora + quem retirou)
    ↓
Devolução (total ou parcial)
    ↓
Conferir estado dos itens devolvidos
    ↓
Encerrar empréstimo
```

### 8.3 Campos

- Número do pedido
- Embarcação / cliente
- Funcionário que entregou
- Data/hora de saída
- Itens emprestados (produto, quantidade)
- Itens devolvidos (quantidade, condição: ok | com dano)
- Data/hora de devolução
- Status: em aberto | parcialmente devolvido | encerrado
- Observações / danos registrados

---

## 9. Checklist da Embarcação

### 9.1 Conceito

Registro do inventário e condições da embarcação em momentos específicos (entrada, saída, vistoria periódica).

### 9.2 Quando é feito

- Entrada da embarcação na marina (checklist de admissão)
- Saída para navegação (checklist de saída)
- Retorno (checklist de chegada)
- OS de vistoria
- Checklist periódico (mensal, semestral)

### 9.3 Itens do Checklist (exemplos)

**Segurança:**
- Quantidade de coletes a bordo
- Quantidade de boias salva-vidas
- Extintor (validade)
- Kit de primeiros socorros
- Sinalizadores (quantidade e validade)
- Âncora e cabo

**Documentação:**
- PROA a bordo (S/N)
- Habilitação do responsável (S/N)

**Estado geral:**
- Casco (ok | danos)
- Motor (funcionando | defeito | não verificado)
- Sistema elétrico
- Bilge (bomba de porão)
- Combustível (nível)
- Água doce (nível)

**Equipamentos:**
- GPS / Ploter (funcionando S/N)
- Rádio VHF (funcionando S/N)
- Âncora elétrica

### 9.4 Campos

- Template de checklist (configurável por tipo de embarcação)
- Data/hora do checklist
- Funcionário responsável
- Embarcação
- Tipo: admissão | saída | chegada | vistoria | periódico
- Itens (item, valor/resposta, observação)
- Fotos
- Assinatura digital do responsável (opcional)

---

## 10. Financeiro

### 10.1 Contratos de Permanência

- Embarcação + vaga + valor mensal
- Data de início e vencimento
- Forma de pagamento (boleto, PIX, cartão)
- Vencimento da mensalidade (dia do mês)
- Renovação automática (S/N)

### 10.2 Cobranças

- Geradas automaticamente todo mês
- Alertas de inadimplência (WhatsApp)
- Juros e multa configuráveis
- Integração Mercado Pago (PIX e cartão)

### 10.3 Cupom Fiscal

Gerado ao fechar pedidos de consumo.

### 10.4 NF de Serviço

Gerada ao concluir ordens de serviço (quando aplicável). Pode ser gerada pela marina ou pelo terceiro.

---

## 11. Status das Embarcações no Mapa — Regras de Negócio

```
VERDE (tudo ok):
  ✅ Nenhum serviço vencido
  ✅ Nenhum documento vencido
  ✅ Mensalidade em dia

AMARELO (atenção):
  ⚠️ Algum serviço com vencimento nos próximos 30 dias  OU
  ⚠️ Algum documento vencendo nos próximos 30 dias       OU
  ⚠️ Mensalidade com poucos dias para vencer

VERMELHO (crítico):
  🚨 Serviço vencido / OS não concluída no prazo         OU
  🚨 Documento vencido (seguro, PROA, vistoria)          OU
  🚨 Mensalidade em atraso
```

---

## 12. Banco de Dados — Entidades

### Prefixos das tabelas

| Prefixo | Módulo          |
|:--------|:----------------|
| `sis_`  | Sistema / base  |
| `cad_`  | Cadastros       |
| `ope_`  | Operação        |
| `fin_`  | Financeiro      |
| `com_`  | Comunicação     |
| `emp_`  | Empréstimos     |

### Listagem de tabelas

#### `sis_` — Sistema

| Tabela                  | Descrição                          |
|:------------------------|:-----------------------------------|
| `sis_empresas`          | Marinas (multitenant)              |
| `sis_planos`            | Planos de assinatura               |
| `sis_usuarios`          | Usuários internos                  |
| `sis_sessoes`           | Sessões autenticadas               |
| `sis_configuracoes`     | Parâmetros globais                 |
| `sis_log_auditoria`     | Log imutável de ações              |

#### `cad_` — Cadastros

| Tabela                        | Descrição                          |
|:------------------------------|:-----------------------------------|
| `cad_secoes`                  | Seções / corredores da marina      |
| `cad_tamanhos_embarcacao`     | Tamanhos padrão (categorias)       |
| `cad_tipos_embarcacao`        | Tipos (lancha, veleiro, jet...)    |
| `cad_vagas`                   | Vagas com posição no mapa          |
| `cad_clientes`                | Armadores e clientes               |
| `cad_embarcacoes`             | Embarcações                        |
| `cad_embarcacao_clientes`     | N:N embarcação ↔ cliente           |
| `cad_embarcacao_documentos`   | Documentos da embarcação           |
| `cad_vagas_embarcacoes`       | Histórico de alocação vaga ↔ emb. |
| `cad_contratos`               | Contratos de permanência           |
| `cad_produtos`                | Produtos e serviços (catálogo)     |
| `cad_fornecedores`            | Fornecedores e terceiros           |
| `cad_checklist_templates`     | Templates de checklist             |
| `cad_checklist_template_itens`| Itens dos templates                |
| `cad_equipamentos`            | Equipamentos da marina             |

#### `ope_` — Operação

| Tabela                        | Descrição                          |
|:------------------------------|:-----------------------------------|
| `ope_ordens_servico`          | Ordens de serviço                  |
| `ope_os_itens`                | Itens de cada OS                   |
| `ope_os_fotos`                | Fotos vinculadas à OS              |
| `ope_os_checklists`           | Checklist específico da OS         |
| `ope_checklists`              | Checklists de embarcação           |
| `ope_checklist_itens`         | Itens do checklist                 |
| `ope_movimentacoes`           | Entrada/saída de embarcações       |
| `ope_agendamentos`            | Agendamentos (içamento, rampa)     |
| `ope_abastecimentos`          | Abastecimento de combustível       |
| `ope_registros_acesso`        | Portaria: entrada/saída            |

#### `fin_` — Financeiro

| Tabela                        | Descrição                          |
|:------------------------------|:-----------------------------------|
| `fin_cobrancas`               | Cobranças mensais                  |
| `fin_pagamentos`              | Transações Mercado Pago            |
| `fin_pedidos_consumo`         | Pedidos de consumo (abertos)       |
| `fin_pedidos_consumo_itens`   | Itens do pedido                    |
| `fin_pedidos_consumo_devolucoes` | Devoluções de itens             |
| `fin_cupons_fiscais`          | Cupons fiscais emitidos            |
| `fin_notas_servico`           | NF de serviço emitidas             |
| `fin_lancamentos`             | Lançamentos financeiros            |

#### `emp_` — Empréstimos

| Tabela                        | Descrição                          |
|:------------------------------|:-----------------------------------|
| `emp_pedidos_emprestimo`      | Cabeçalho do empréstimo            |
| `emp_pedidos_emprestimo_itens`| Itens emprestados                  |
| `emp_registro_chaves`         | Controle de chaves                 |

#### `com_` — Comunicação

| Tabela                        | Descrição                          |
|:------------------------------|:-----------------------------------|
| `com_instancias_whatsapp`     | Instâncias Evolution API           |
| `com_mensagens_whatsapp`      | Log de mensagens                   |
| `com_comunicados`             | Comunicados da marina              |
| `com_templates_mensagem`      | Templates automáticos              |
| `com_chamados`                | Chamados / suporte                 |
| `com_chamado_mensagens`       | Mensagens do atendimento           |

---

## 13. Roadmap de Implementação

### Fase 1 — Fundação (cadastros e autenticação)

- [ ] Banco de dados: tabelas `sis_*`
- [ ] Autenticação, perfis e controle de acesso
- [ ] Multitenant (filtro por EmpresaId)
- [ ] Cadastro de Seções
- [ ] Cadastro de Tamanhos Padrão de Embarcação
- [ ] Cadastro de Tipos de Embarcação
- [ ] Cadastro de Vagas
- [ ] Cadastro de Clientes
- [ ] Cadastro de Embarcações + vínculo cliente
- [ ] Cadastro de Documentos
- [ ] Cadastro de Produtos e Serviços
- [ ] Cadastro de Fornecedores / Terceiros

### Fase 2 — Mapa Visual e Operação

- [ ] Mapa visual com vagas e seções
- [ ] Drag & drop embarcação → vaga
- [ ] Painel lateral de embarcações sem vaga
- [ ] Cores de status (verde, amarelo, vermelho)
- [ ] Checklist de embarcação
- [ ] Ordens de serviço (próprio e terceiro)
- [ ] Pedidos de consumo com devolução
- [ ] Pedidos de empréstimo

### Fase 3 — Financeiro e Fiscal

- [ ] Contratos e mensalidades
- [ ] Integração Mercado Pago (PIX, cartão)
- [ ] Geração de cupom fiscal
- [ ] Geração de NF de serviço
- [ ] Cobranças e inadimplência
- [ ] Alertas automáticos de vencimento

### Fase 4 — Comunicação e Automação

- [ ] Integração WhatsApp (Evolution API)
- [ ] Templates automáticos (vencimentos, cobranças, OS)
- [ ] Portal do armador (acesso cliente)
- [ ] SignalR — notificações em tempo real
- [ ] Chamados / atendimento

### Fase 5 — Analytics e Crescimento

- [ ] Painel de indicadores (ocupação, MRR, inadimplência)
- [ ] Alertas de churn
- [ ] Agendamentos (içamento, rampa)
- [ ] Controle de combustível
- [ ] Eventos e regatas
- [ ] NPS automático
- [ ] App mobile para técnicos

---

## 14. Decisões Pendentes / Pontos em Aberto

- [ ] **Mapa visual:** será desenhado manualmente (configurando X,Y de cada vaga) ou haverá um editor visual de planta?
- [ ] **Cupom fiscal:** qual sistema de emissão será integrado? (NF-e, SAT, MFe, PAF-ECF...)
- [ ] **NF de serviço:** emissão própria ou orientar o terceiro a emitir no nome dele?
- [ ] **Combustível:** a marina vende combustível diretamente? Integra com abastecedor terceiro?
- [ ] **Pedido de compra para terceiro:** fluxo de cotação / aprovação necessário?
- [ ] **Checklist:** haverá assinatura digital do cliente ao final?
- [ ] **Tamanhos padrão:** os intervalos da tabela acima fazem sentido para a realidade da marina?
- [ ] **Portal do armador:** será web ou app mobile?

---

*Documento criado em abril/2026. Atualizar conforme decisões forem tomadas.*
