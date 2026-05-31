-- ══════════════════════════════════════════════════════════════════
-- LIMPEZA: Seções sem tipo e corredores sem seção (dados de teste)
-- Banco: SetiMarine (PostgreSQL)
-- Rode os SELECTs abaixo ANTES para confirmar o que será apagado.
-- ══════════════════════════════════════════════════════════════════

-- ── VERIFICAÇÃO prévia ──────────────────────────────────────────
-- SELECT id, nome FROM mar_corredores WHERE secao_id IS NULL;
-- SELECT id, nome FROM mar_secoes WHERE tipo_preferencial IS NULL;
-- SELECT v.id, v.codigo FROM mar_vagas v
--   WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
--      OR v.corredor_id IN (
--          SELECT c.id FROM mar_corredores c
--          JOIN mar_secoes s ON c.secao_id = s.id
--          WHERE s.tipo_preferencial IS NULL);

BEGIN;

-- 1. Corredores sem seção + corredores de seções sem tipo → IDs das vagas afetadas
-- (usados como subquery nos passos seguintes)

-- 2. Registros de serviço: vaga_id é nullable → só zera a referência
UPDATE mar_registros_servico
SET vaga_id = NULL
WHERE vaga_id IN (
    SELECT v.id FROM mar_vagas v
    WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
       OR v.corredor_id IN (
           SELECT c.id FROM mar_corredores c
           JOIN mar_secoes s ON c.secao_id = s.id
           WHERE s.tipo_preferencial IS NULL)
);

-- 3. Histórico de movimentações
DELETE FROM mar_historico_movimentacoes
WHERE movimentacao_id IN (
    SELECT m.id FROM mar_movimentacoes m
    WHERE m.vaga_id IN (
        SELECT v.id FROM mar_vagas v
        WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
           OR v.corredor_id IN (
               SELECT c.id FROM mar_corredores c
               JOIN mar_secoes s ON c.secao_id = s.id
               WHERE s.tipo_preferencial IS NULL)
    )
);

-- 4. Movimentações
DELETE FROM mar_movimentacoes
WHERE vaga_id IN (
    SELECT v.id FROM mar_vagas v
    WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
       OR v.corredor_id IN (
           SELECT c.id FROM mar_corredores c
           JOIN mar_secoes s ON c.secao_id = s.id
           WHERE s.tipo_preferencial IS NULL)
);

-- 5. Contratos
DELETE FROM mar_contratos
WHERE vaga_id IN (
    SELECT v.id FROM mar_vagas v
    WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
       OR v.corredor_id IN (
           SELECT c.id FROM mar_corredores c
           JOIN mar_secoes s ON c.secao_id = s.id
           WHERE s.tipo_preferencial IS NULL)
);

-- 6. Ocupações de vaga (embarcações alocadas)
DELETE FROM mar_vaga_embarcacoes
WHERE vaga_id IN (
    SELECT v.id FROM mar_vagas v
    WHERE v.corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
       OR v.corredor_id IN (
           SELECT c.id FROM mar_corredores c
           JOIN mar_secoes s ON c.secao_id = s.id
           WHERE s.tipo_preferencial IS NULL)
);

-- 7. Vagas
DELETE FROM mar_vagas
WHERE corredor_id IN (SELECT id FROM mar_corredores WHERE secao_id IS NULL)
   OR corredor_id IN (
       SELECT c.id FROM mar_corredores c
       JOIN mar_secoes s ON c.secao_id = s.id
       WHERE s.tipo_preferencial IS NULL);

-- 8. Corredores sem seção
DELETE FROM mar_corredores WHERE secao_id IS NULL;

-- 9. Corredores de seções sem tipo
DELETE FROM mar_corredores
WHERE secao_id IN (SELECT id FROM mar_secoes WHERE tipo_preferencial IS NULL);

-- 10. Seções sem tipo
DELETE FROM mar_secoes WHERE tipo_preferencial IS NULL;

COMMIT;
