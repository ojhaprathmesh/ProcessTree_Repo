# 🌿 ProcessTree

A recursive item processing and hierarchy tracking system built with 
**ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**.

> Input items with a given weight can be processed into multiple output 
> items — recursively — with a live tree visualization of the full 
> processing hierarchy.

---

## ✨ Features

- Create root input items with name and weight
- Process any item into 1–20 output items
- Equal weight split or custom weight assignment
- Recursive processing — output items can be further processed
- Visual tree diagram showing the full parent-child hierarchy
- Depth guard prevents infinite processing chains (max 10 levels)
- Input validation with clear error messages throughout

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB) |
| UI | Bootstrap 5 |
| Tree Visualization | Recursive Razor Partials + CSS |

---

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022/2026 (Community or higher)
- ASP.NET and web development workload installed
- SQL Server LocalDB (included with Visual Studio)

### Setup
```bash
# 1. Clone the repo
git clone https://github.com/ojhaprathmesh/ProcessTree.git
cd ProcessTree

# 2. Restore packages
dotnet restore

# 3. Apply database migrations
dotnet ef database update

# 4. Run the app
dotnet run
```

Then open `https://localhost:7212` in your browser.

### Alternative: Manual SQL Setup
Run `database/schema.sql` in SQL Server Management Studio,
then update the connection string in `appsettings.json`.

---

## 📁 Project Structure
```
ProcessTree/
├── Controllers/
│   └── ProcessItemController.cs   # HTTP request handlers
├── Data/
│   └── AppDbContext.cs            # EF Core database context
├── database/
│   └── schema.sql                 # Manual DB setup script
├── Views/
│   └── ProcessItem/               # All feature views
│       ├── Index.cshtml           # Items list
│       ├── Create.cshtml          # Create form
│       ├── Process.cshtml         # Process form
│       ├── Details.cshtml         # Item details
│       ├── Tree.cshtml            # Tree visualization page
│       └── _TreeNode.cshtml       # Recursive tree partial
├── Models/
│   ├── ProcessItem.cs             # Core domain model
│   ├── ProcessingRule.cs          # Rule configuration
│   └── ViewModels/                # Form & tree view models
├── Services/
│   ├── IProcessingService.cs      # Service contract
│   └── ProcessingService.cs       # Business logic + recursion
└── wwwroot/css/
    └── tree.css                   # Tree visualization styles
```

---

## 🌲 How the Tree Works

Items form a parent-child hierarchy stored in a single self-referencing 
SQL table. When you process an item, it generates N child items. Each 
child can itself be processed, creating a recursive tree. The Tree view 
renders this using recursive Razor partial views and CSS connector lines.
```
Steel Batch (300 kg) — Processed
    ├── Steel Batch — Output 1 (100 kg) — Processed
    │       ├── Output 1-1 (50 kg) — Unprocessed
    │       └── Output 1-2 (50 kg) — Unprocessed
    ├── Steel Batch — Output 2 (100 kg) — Unprocessed
    └── Steel Batch — Output 3 (100 kg) — Unprocessed
```

---

## ⚙️ Key Design Decisions

- **Max depth of 10** — prevents infinite recursive processing chains
- **Restrict delete** — parent items can't be deleted while children exist
- **Weight rounding** — last output absorbs remainder to preserve total
- **Single table hierarchy** — self-referencing FK keeps schema simple
- **Load-all-then-build** — tree builder loads items once to avoid N+1

---

## 👨‍💻 Author

**Prathmesh Ojha** — [github.com/ojhaprathmesh](https://github.com/ojhaprathmesh)
