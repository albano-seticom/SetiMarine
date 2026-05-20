using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    public partial class AddSessaoAtivaFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FKs e indexes que ja foram dropados na migration anterior parcialmente aplicada
            // Usando TryCatch via SQL direto para ignorar se nao existirem
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    ALTER TABLE mar_embarcacoes DROP CONSTRAINT IF EXISTS ""FK_mar_embarcacoes_mar_vagas_VagaFixaId"";
                    ALTER TABLE mar_movimentacoes DROP CONSTRAINT IF EXISTS ""FK_mar_movimentacoes_sis_usuarios_ResponsavelId"";
                    ALTER TABLE mar_vagas DROP CONSTRAINT IF EXISTS ""FK_mar_vagas_mar_embarcacoes_EmbarcacaoAtualId"";
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    DROP INDEX IF EXISTS ""IX_mar_vagas_EmbarcacaoAtualId"";
                    DROP INDEX IF EXISTS ""IX_mar_embarcacoes_VagaFixaId"";
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Renomeia CriadaEm -> IniciadaEm somente se ainda nao foi renomeado
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='sis_sessoes' AND column_name='CriadaEm'
                    ) THEN
                        ALTER TABLE sis_sessoes RENAME COLUMN ""CriadaEm"" TO ""IniciadaEm"";
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Adiciona EmpresaId em sis_sessoes se nao existir
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='sis_sessoes' AND column_name='EmpresaId'
                    ) THEN
                        ALTER TABLE sis_sessoes ADD ""EmpresaId"" integer NOT NULL DEFAULT 0;
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Adiciona ExpiraEm em sis_sessoes se nao existir
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='sis_sessoes' AND column_name='ExpiraEm'
                    ) THEN
                        ALTER TABLE sis_sessoes ADD ""ExpiraEm"" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Adiciona CorredorId em mar_vagas se nao existir
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='mar_vagas' AND column_name='CorredorId'
                    ) THEN
                        ALTER TABLE mar_vagas ADD ""CorredorId"" integer;
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Adiciona LarguraMaxPes em mar_vagas se nao existir
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='mar_vagas' AND column_name='LarguraMaxPes'
                    ) THEN
                        ALTER TABLE mar_vagas ADD ""LarguraMaxPes"" numeric;
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Adiciona TamanhoMaxPes em mar_vagas se nao existir
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='mar_vagas' AND column_name='TamanhoMaxPes'
                    ) THEN
                        ALTER TABLE mar_vagas ADD ""TamanhoMaxPes"" numeric NOT NULL DEFAULT 0;
                    END IF;
                EXCEPTION WHEN OTHERS THEN NULL;
                END $$;
            ");

            // Cria mar_checklist_templates se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_checklist_templates (
                    ""Id"" serial PRIMARY KEY,
                    ""EmpresaId"" integer NOT NULL,
                    ""Nome"" text NOT NULL,
                    ""Descricao"" text,
                    ""Ativo"" boolean NOT NULL,
                    ""CriadoEm"" timestamp with time zone NOT NULL,
                    CONSTRAINT ""FK_mar_checklist_templates_sis_empresas_EmpresaId""
                        FOREIGN KEY (""EmpresaId"") REFERENCES sis_empresas(""Id"") ON DELETE CASCADE
                );
            ");

            // Cria mar_corredores se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_corredores (
                    ""Id"" serial PRIMARY KEY,
                    ""EmpresaId"" integer NOT NULL,
                    ""Nome"" text NOT NULL,
                    ""Descricao"" text,
                    ""TamanhoMaxPes"" numeric NOT NULL,
                    ""Ordem"" integer NOT NULL,
                    ""Ativo"" boolean NOT NULL,
                    ""CriadoEm"" timestamp with time zone NOT NULL,
                    CONSTRAINT ""FK_mar_corredores_sis_empresas_EmpresaId""
                        FOREIGN KEY (""EmpresaId"") REFERENCES sis_empresas(""Id"") ON DELETE CASCADE
                );
            ");

            // Cria mar_pedidos_uso se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_pedidos_uso (
                    ""Id"" serial PRIMARY KEY,
                    ""EmpresaId"" integer NOT NULL,
                    ""EmbarcacaoId"" integer NOT NULL,
                    ""ClienteId"" integer NOT NULL,
                    ""DataPrevista"" timestamp with time zone NOT NULL,
                    ""Observacao"" text,
                    ""Origem"" integer NOT NULL,
                    ""MensagemOriginal"" text,
                    ""Status"" integer NOT NULL,
                    ""MovimentacaoId"" integer,
                    ""CriadoEm"" timestamp with time zone NOT NULL
                );
            ");

            // Cria mar_vaga_embarcacoes se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_vaga_embarcacoes (
                    ""Id"" serial PRIMARY KEY,
                    ""VagaId"" integer NOT NULL,
                    ""EmbarcacaoId"" integer NOT NULL,
                    ""EntradaEm"" timestamp with time zone NOT NULL,
                    ""SaidaEm"" timestamp with time zone,
                    ""Ativa"" boolean NOT NULL,
                    CONSTRAINT ""FK_mar_vaga_embarcacoes_mar_vagas_VagaId""
                        FOREIGN KEY (""VagaId"") REFERENCES mar_vagas(""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_mar_vaga_embarcacoes_mar_embarcacoes_EmbarcacaoId""
                        FOREIGN KEY (""EmbarcacaoId"") REFERENCES mar_embarcacoes(""Id"") ON DELETE CASCADE
                );
            ");

            // Cria mar_checklist_template_itens se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_checklist_template_itens (
                    ""Id"" serial PRIMARY KEY,
                    ""ChecklistTemplateId"" integer NOT NULL,
                    ""Descricao"" text NOT NULL,
                    ""Obrigatorio"" boolean NOT NULL,
                    ""Ordem"" integer NOT NULL,
                    ""Ativo"" boolean NOT NULL,
                    CONSTRAINT ""FK_mar_checklist_template_itens_mar_checklist_templates_Checkl~""
                        FOREIGN KEY (""ChecklistTemplateId"") REFERENCES mar_checklist_templates(""Id"") ON DELETE CASCADE
                );
            ");

            // Cria mar_tipos_servico_config se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_tipos_servico_config (
                    ""Id"" serial PRIMARY KEY,
                    ""EmpresaId"" integer NOT NULL,
                    ""Nome"" text NOT NULL,
                    ""Tipo"" integer NOT NULL,
                    ""FrequenciaDias"" integer NOT NULL,
                    ""ChecklistTemplateId"" integer,
                    ""Ativo"" boolean NOT NULL,
                    ""CriadoEm"" timestamp with time zone NOT NULL
                );
            ");

            // Cria mar_registros_servico se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_registros_servico (
                    ""Id"" serial PRIMARY KEY,
                    ""EmpresaId"" integer NOT NULL,
                    ""EmbarcacaoId"" integer NOT NULL,
                    ""VagaId"" integer,
                    ""TipoServicoConfigId"" integer,
                    ""MovimentacaoId"" integer,
                    ""Tipo"" integer NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""ResponsavelId"" integer,
                    ""Observacao"" text,
                    ""AgendadoPara"" timestamp with time zone NOT NULL,
                    ""IniciadoEm"" timestamp with time zone,
                    ""FinalizadoEm"" timestamp with time zone,
                    ""CriadoEm"" timestamp with time zone NOT NULL
                );
            ");

            // Cria mar_fotos_servico se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_fotos_servico (
                    ""Id"" serial PRIMARY KEY,
                    ""RegistroServicoId"" integer NOT NULL,
                    ""Url"" text NOT NULL,
                    ""Descricao"" text,
                    ""EhAvaria"" boolean NOT NULL,
                    ""TiradaEm"" timestamp with time zone NOT NULL,
                    ""UsuarioId"" integer,
                    CONSTRAINT ""FK_mar_fotos_servico_mar_registros_servico_RegistroServicoId""
                        FOREIGN KEY (""RegistroServicoId"") REFERENCES mar_registros_servico(""Id"") ON DELETE CASCADE
                );
            ");

            // Cria mar_registro_servico_itens se nao existir
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS mar_registro_servico_itens (
                    ""Id"" serial PRIMARY KEY,
                    ""RegistroServicoId"" integer NOT NULL,
                    ""ChecklistTemplateItemId"" integer,
                    ""Descricao"" text NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""Observacao"" text,
                    CONSTRAINT ""FK_mar_registro_servico_itens_mar_registros_servico_RegistroSe~""
                        FOREIGN KEY (""RegistroServicoId"") REFERENCES mar_registros_servico(""Id"") ON DELETE CASCADE
                );
            ");

            // Indexes - usando IF NOT EXISTS
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_sis_sessoes_UsuarioId"" ON sis_sessoes (""UsuarioId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_vagas_CorredorId"" ON mar_vagas (""CorredorId"");");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_mar_embarcacoes_VagaFixaId"" ON mar_embarcacoes (""VagaFixaId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_checklist_template_itens_ChecklistTemplateId"" ON mar_checklist_template_itens (""ChecklistTemplateId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_checklist_templates_EmpresaId"" ON mar_checklist_templates (""EmpresaId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_corredores_EmpresaId"" ON mar_corredores (""EmpresaId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_fotos_servico_RegistroServicoId"" ON mar_fotos_servico (""RegistroServicoId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_vaga_embarcacoes_EmbarcacaoId"" ON mar_vaga_embarcacoes (""EmbarcacaoId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_mar_vaga_embarcacoes_VagaId"" ON mar_vaga_embarcacoes (""VagaId"");");

            // FKs finais - usando IF NOT EXISTS via DO
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints
                        WHERE constraint_name='FK_mar_embarcacoes_mar_vagas_VagaFixaId'
                    ) THEN
                        ALTER TABLE mar_embarcacoes ADD CONSTRAINT ""FK_mar_embarcacoes_mar_vagas_VagaFixaId""
                            FOREIGN KEY (""VagaFixaId"") REFERENCES mar_vagas(""Id"") ON DELETE SET NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints
                        WHERE constraint_name='FK_mar_vagas_mar_corredores_CorredorId'
                    ) THEN
                        ALTER TABLE mar_vagas ADD CONSTRAINT ""FK_mar_vagas_mar_corredores_CorredorId""
                            FOREIGN KEY (""CorredorId"") REFERENCES mar_corredores(""Id"") ON DELETE SET NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints
                        WHERE constraint_name='FK_sis_sessoes_sis_usuarios_UsuarioId'
                    ) THEN
                        ALTER TABLE sis_sessoes ADD CONSTRAINT ""FK_sis_sessoes_sis_usuarios_UsuarioId""
                            FOREIGN KEY (""UsuarioId"") REFERENCES sis_usuarios(""Id"") ON DELETE CASCADE;
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_fotos_servico;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_registro_servico_itens;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_registros_servico;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_tipos_servico_config;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_checklist_template_itens;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_checklist_templates;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_vaga_embarcacoes;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_pedidos_uso;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS mar_corredores;");
        }
    }
}
