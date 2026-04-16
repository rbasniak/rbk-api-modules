# Jarvis — History

## Project Context
- **Project:** rbk-api-modules
- **Description:** A comprehensive set of .NET 10 libraries for accelerating ASP.NET Core Web API development. Provides core infrastructure, JWT/API key authentication, EF Core integration, FluentValidation, testing utilities, and static code analyzers.
- **Stack:** C# / .NET 10 / ASP.NET Core / Entity Framework Core / FluentValidation / xUnit / SQLite (testing) / SQL Server
- **Key packages:** rbkApiModules.Commons.Core, rbkApiModules.Identity.Core, rbkApiModules.Identity.Relational, rbkApiModules.Commons.Testing, rbkApiModules.Analysers
- **Important files:** Directory.Build.props, Directory.Packages.props, global.json, nuget.config
- **User:** Rodrigo Basniak
- **Team hired:** 2026-04-16

## Learnings

### Code Quality Review (2026-04-16)

**Most Common Issues Found:**
1. **Inconsistent null-checking patterns** — Mix of old-style (`== null`) and modern (`is null`) across 50+ instances
2. **TODO/FIXME comments** — 28 instances, including "DONE, REVIEWED" markers that should be deleted
3. **Magic numbers** — 20+ hardcoded timeouts, ports, dimensions scattered throughout (especially in Playwright extensions)
4. **Missing `sealed` keywords** — 10+ classes not designed for inheritance but unsealed
5. **Large methods** — Several 100+ line methods with deep nesting (CoreAuthenticationBuilder, Dispatcher)
6. **Old commented code** — Entire `Old_Dispatcher.cs` file (227 lines) and other commented blocks
7. **Manual null argument checks** — 15+ instances that should use `ArgumentNullException.ThrowIfNull()`
8. **Filename typos** — `ExceptionExtrensions.cs` and `EmumExtensions.cs`

**Packages with Most Debt:**
1. **rbkApiModules.Commons.Core** — Highest complexity, most issues (Dispatcher, SmartValidator, legacy commented code)
2. **rbkApiModules.Identity.Core** — Massive 290-line authentication builder needs refactoring
3. **rbkApiModules.Messaging.Core** — Pervasive "TODO: DONE, REVIEWED" comments to remove
4. **rbkApiModules.Commons.Testing** — Magic numbers in Playwright extensions
5. **rbkApiModules.Analysers** — Cleanest package, minimal issues

**Key Files Needing Attention:**
- `rbkApiModules.Commons.Core\Messaging\Old_Dispatcher.cs` — DELETE (fully commented)
- `rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs` — REFACTOR (290 lines, 8 nesting levels)
- `rbkApiModules.Commons.Core\Messaging\Dispatcher.cs` — REFACTOR (438 lines, extract concerns)
- `rbkApiModules.Commons.Core\Validation\SmartValidator.cs` — MODERNIZE (null checks, simplify)
- `rbkApiModules.Commons.Testing\Playwright\Extensions.cs` — EXTRACT CONSTANTS (magic timeouts)

**Patterns Done Well (to preserve):**
- ✅ **Primary constructors** — Used effectively in newer code (e.g., Dispatcher, handlers)
- ✅ **Record types** — Good use for DTOs and value objects
- ✅ **Pattern matching** — Switch expressions well utilized (ImageUtilities)
- ✅ **Modern async/await** — No `async void` or `.Result`/`.Wait()` blocking found
- ✅ **Dependency injection** — Clean constructor injection throughout, no service locator anti-pattern
- ✅ **Roslyn analyzers** — Clean implementation with minimal technical debt

**Modernization Opportunities:**
- Collection expressions `[]` (C# 12) — Not yet adopted, could simplify array/list initialization
- `required` keyword — Not used, could enforce mandatory properties at compile-time
- `init`-only setters — Limited use, could improve immutability
- `ArgumentNullException.ThrowIfNull()` — Only 2 uses, should be everywhere (C# 11+)
