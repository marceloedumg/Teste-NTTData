# Design QA — Gestão de pedidos

## Evidências

- Verdade visual normalizada: [`docs/design-reference.png`](docs/design-reference.png)
- Implementação desktop final: [`docs/implementation-desktop.png`](docs/implementation-desktop.png)
- Comparação integral final: [`docs/design-comparison.png`](docs/design-comparison.png)
- Captura mobile final: [`docs/implementation-mobile.png`](docs/implementation-mobile.png)
- Estado comparado: usuário autenticado, página 1, cinco pedidos reais e pedido pendente selecionado com detalhe lateral aberto.
- Viewport CSS: `1440 × 1024`; `devicePixelRatio: 1`.
- Fonte: `1487 × 1058` pixels, normalizada para `1440 × 1024` apenas para comparação de mesma proporção.
- Implementação: `1440 × 1024` pixels, sem normalização.

## Comparação integral

A composição final conserva a grade da referência: cabeçalho de 68 px, área de trabalho em aproximadamente 62% da largura, painel lateral em 38%, título editorial, CTA amarelo abaixo da introdução, barra de busca/filtro, tabela com seleção azul e detalhe branco com ação contextual. Não há conteúdo persistente cortado no viewport.

## Comparação focada

A região superior de 1440 × 720 foi comparada lado a lado para tornar legíveis tipografia, logo, CTA, campos, cabeçalhos, linhas, badges, divisores e detalhe. A comparação confirmou Noto Serif/Noto Sans, `#070F26`, azul de interação, amarelo institucional, pesos, alinhamentos e densidade equivalentes. Não foi necessário outro recorte: os elementos críticos ficam legíveis nessa região.

## Histórico de correções P0/P1/P2

1. **P2 — hierarquia do CTA no desktop.** Na primeira captura, o CTA ficava à direita do título e havia um eyebrow adicional. A referência posiciona a ação abaixo da introdução. O eyebrow foi removido e o CTA movido para baixo do texto. A captura `docs/implementation-desktop.png` confirma o alinhamento final.
2. **P2 — tabela mobile exigia rolagem horizontal.** A primeira captura em 390 × 844 deixava colunas importantes fora da área visível. No breakpoint mobile, a tabela agora prioriza Pedido, Status e Total; cliente e data continuam disponíveis no detalhe. A captura `docs/implementation-mobile.png` confirma que as ações permanecem visíveis.
3. **P2 — ID longo no detalhe mobile.** O identificador ocupava largura excessiva. A apresentação foi abreviada, mantendo o UUID completo em `title` e nos dados da API.

## Verificação das superfícies obrigatórias

- **Fontes e tipografia:** Noto Serif 500 nos títulos e Noto Sans 400–700 na interface, iguais às famílias identificadas no site oficial. Hierarquia, entrelinha e truncamento estão legíveis no desktop e mobile.
- **Espaçamento e ritmo:** margens, grid 62/38, altura do cabeçalho, CTA, toolbar, densidade de linhas, raio assimétrico do drawer e paginação acompanham a referência.
- **Cores e tokens:** azul-marinho `#070F26`, azul de interação e amarelo institucional reproduzidos; estados usam cores semânticas com contraste suficiente.
- **Imagens e ativos:** o único ativo de imagem é o logo oficial da NTT DATA, salvo localmente e renderizado com transparência preservada. Ícones vêm de uma biblioteca consistente; não há SVG artesanal, desenho com `div`, emoji ou placeholder visual.
- **Texto e conteúdo:** a redação está em português. Nomes fictícios e campos inexistentes do mock foram substituídos por UUIDs e itens reais porque a API do teste não possui cadastro de clientes, descontos ou frete.

## Interações e qualidade

- Login real com JWT verificado.
- Listagem, filtro local da página, busca, paginação, seleção e detalhe verificados.
- Criação real de pedido com UUID gerado, item, quantidade, preço e total verificada.
- Confirmação e cancelamento real verificados; o status foi atualizado na lista e no detalhe.
- Estados de loading, vazio, erro, modal, confirmação e toast estão implementados.
- Console e erros do navegador verificados após o fluxo: nenhum erro de aplicação.
- Auditoria axe WCAG 2 A/AA: 22 verificações aprovadas, 0 violações e 0 itens incompletos.

## Findings

Não restam diferenças P0, P1 ou P2 acionáveis. As diferenças de conteúdo são intencionais e decorrem do contrato real da API.

## Follow-up Polish

- **P3:** quando a API tiver cadastro de clientes, substituir o UUID por nome e documento enriquecerá o detalhe sem alterar o layout.
- **P3:** uma futura busca server-side poderá pesquisar todas as páginas; hoje a própria interface informa que a busca atua na página carregada.

## Implementation Checklist

- [x] Desktop fiel à referência selecionada.
- [x] Responsividade validada em 390 × 844.
- [x] Fluxo principal completo validado no navegador.
- [x] Sem erros de console ou violações WCAG 2 A/AA.
- [x] Build de produção e pacote Sites aprovados.

final result: passed
