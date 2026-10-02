# Gradil

Aplicação desktop para orçamento de cercas Gradil: calcula os componentes,
mostra um esboço da cerca na cor escolhida e guarda os pedidos confirmados.

![.NET 8] ![WPF] ![SQLite]

## Como rodar

Requisitos: Windows 10/11 e [.NET 8 SDK] (ou mais recente).

```bash
dotnet run --project src/Gradil.App
```

No Visual Studio: abrir `Gradil.sln`, definir **Gradil.App** como projeto de inicialização e pressionar F5.

Na primeira execução são criados, ao lado do executável:

- `gradil.db`: banco SQLite com os pedidos (não precisa instalar servidor);
- `logs/gradil-AAAAMMDD.log`: log do dia.

### Gerar o executável

```bash
dotnet publish src/Gradil.App -c Release -r win-x64 --self-contained false -o publicar
```

A pasta `publicar` contém o `Gradil.exe`. Para rodar, a máquina precisa do .NET 8 Desktop Runtime.

### Gerar o pacote NuGet da regra de cálculo

```bash
dotnet pack src/Gradil.Core -c Release -o pacotes
```

Gera `Gradil.Core.1.0.0.nupkg`. Outro sistema pode instalar esse pacote e usar a `CalculadoraGradil`
sem levar a tela nem o banco junto.

## Testes

```bash
dotnet test
```

| Grupo | O que garante |
|---|---|
| `Regras/CalculadoraGradilTests` | Quantidades, arredondamento, diferença e validação do comprimento |
| `ViewModels/OrcamentoViewModelTests` | Comportamento da tela: entrada com vírgula ou ponto, erros, confirmação, falha do banco, desenho |
| `Infraestrutura/SqlitePedidoRepositorioTests` | Gravação e leitura reais em SQLite, ordenação do mais recente |

Os nomes dos testes descrevem a regra em linguagem de negócio, então a lista de testes também serve como especificação.

## Arquitetura

```
src/
├── Gradil.Core             Domínio: modelos, regras e contratos. Não depende de nada.
├── Gradil.Infraestrutura   SQLite e logs. Implementa os contratos do Core.
└── Gradil.App              WPF (MVVM). Liga as camadas na inicialização.
tests/
└── Gradil.Tests            xUnit
```

As dependências apontam para dentro: **App → Infraestrutura → Core**.
O Core não sabe que existe tela nem banco, então a regra pode ser reaproveitada em outra aplicação.

**MVVM:** a janela (`MainWindow.xaml`) só tem layout e `{Binding}`; o code-behind está vazio.
Toda a lógica de tela fica no `OrcamentoViewModel`, que pode ser testado sem abrir janela.
`ViewModelBase` e `Comando` foram escritos à mão, sem bibliotecas de MVVM.

## Regras de cálculo

| Componente | Regra |
|---|---|
| Telas | comprimento ÷ 2,5 m, **arredondado para cima** (a tela não é cortada) |
| Postes | telas + 1 |
| Fixadores | telas × 3, 4 ou 6 (alturas 1,03 / 1,53 / 2,03 m) |
| Parafusos | postes × 4 |

Exemplo: 10,1 m na altura 1,53 m → 5 telas, 6 postes, 20 fixadores, 24 parafusos.
Vendido: 12,50 m (sobra de 2,40 m).

### Premissas

- **Cerca em linha reta.** Se fosse um perímetro fechado, postes = telas.
- **Fixadores por tela.** O enunciado não deixa claro se é por tela ou por lado. Ajustável em `Catalogo.Alturas`.
- **Só as entradas são gravadas** (comprimento, altura, pintura e data), como pede o enunciado.
  As quantidades são sempre recalculáveis a partir delas.
- **Comprimento máximo de 10.000 m.** Acima disso o pedido não é realista e a quantidade de telas estouraria o `int`.

## Tratamento de erros e logs

- **Entrada inválida** (texto, zero, negativo ou acima de 10.000 m): mensagem em vermelho abaixo dos campos
  e botão de confirmar desabilitado.
- **Falha no banco:** o repositório registra o erro técnico no log e lança `RepositorioException`
  com uma mensagem amigável. A tela mostra a mensagem e mantém o orçamento para nova tentativa.
- **Erro inesperado:** capturado globalmente no `App.xaml.cs` e registrado no log. Se o erro acontecer na tela,
  o usuário vê um aviso e o programa continua aberto. Se acontecer fora da tela, o programa fecha,
  mas o erro fica registrado no log.
- **Logs** com Serilog, um arquivo por dia, mantendo os últimos 14.
