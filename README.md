# Order Manager — Checkpoint 3

Aplicativo WPF em **.NET 8** para gerenciamento de pedidos, desenvolvido para o Checkpoint 3 da disciplina, demonstrando **POO, Interfaces e Injeção de Dependência**.

## 🚀 Como executar

Pelo Visual Studio / Rider: abrir `OrderManager.sln` e executar (F5).

Pelo terminal:

```bash
cd OrderManager
dotnet run
```

Requisitos: **.NET 8 SDK** em Windows (WPF).

## 🧱 Estrutura do projeto

```
OrderManager/
├── OrderManager.sln
├── OrderManager/
│   ├── OrderManager.csproj
│   ├── App.xaml / App.xaml.cs        ← entry point
│   ├── MainWindow.xaml / .xaml.cs    ← UI WPF + composição de DI
│   ├── Models/
│   │   ├── Product.cs                ← classe abstrata
│   │   ├── PhysicalProduct.cs        ← preço + frete
│   │   └── DigitalProduct.cs         ← preço + taxa de plataforma
│   └── Services/
│       ├── INotifier.cs              ← interface
│       ├── EmailNotifier.cs          ← implementação por e-mail
│       ├── SmsNotifier.cs            ← implementação por SMS
│       └── OrderService.cs           ← recebe INotifier via construtor
└── .gitignore
```

## 🎯 Conceitos demonstrados

### Programação Orientada a Objetos

- **Abstração**: `Product` é uma classe `abstract` que define o contrato (`Id`, `Name`, `Price`, `GetFinalPrice()`, `GetDescription()`) sem implementar a lógica de cálculo.
- **Herança**: `PhysicalProduct` e `DigitalProduct` herdam de `Product`.
- **Polimorfismo**: `OrderService.CalculateTotal()` faz `Products.Sum(p => p.GetFinalPrice())` — cada subclasse calcula o próprio total, sem `if/else` por tipo.
- **Encapsulamento**: a lista de produtos é exposta apenas como `ObservableCollection<Product>` somente-leitura; alterações passam pelos métodos do serviço.

### Interfaces

- `INotifier` define o contrato `Send(customerName, message)`.
- `EmailNotifier` e `SmsNotifier` implementam o contrato de forma intercambiável (princípio de Liskov).

### Injeção de Dependência

- `OrderService` **recebe `INotifier` pelo construtor** — não há construtor padrão, é obrigatório injetar.
- `OrderService` **nunca instancia notificadores com `new`** internamente — o serviço depende da abstração, não das implementações concretas (Inversão de Dependência - SOLID).
- A composição (criar `EmailNotifier` ou `SmsNotifier`) acontece em `MainWindow`, que funciona como a raiz de composição da aplicação.
- O método `ChangeNotifier(INotifier)` permite trocar o notificador em tempo de execução **mantendo a DI**: o novo notificador ainda vem de fora.

## 🖥️ Funcionalidades

- Cadastrar produtos físicos (com frete) ou digitais (com taxa de plataforma).
- Listar produtos em um `DataGrid` com colunas para ID, tipo, nome, preço base, frete/taxa e total.
- Remover produto selecionado.
- Calcular o total do pedido a qualquer momento.
- Escolher canal de notificação (E-mail ou SMS).
- Finalizar pedido enviando uma notificação detalhada ao cliente e limpando a lista.
- Validação de entrada (nome obrigatório, valores numéricos, lista não vazia para finalizar).
