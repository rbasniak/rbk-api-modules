# Squad Decisions

## Active Decisions

### Documentation Structure (2026-01-17)
**Decision Maker:** Wong (Documentation Writer)  
**Status:** Implemented

Restructured rbkApiModules documentation from single lengthy README to a hub-and-spoke model:
- **Root README.md** - Concise overview (under 200 lines) with package descriptions and links
- **docs/getting-started.md** - 10-minute quick start guide
- **Specialized docs** - One doc per major feature area

**Structure:**

1. **README.md** (root) - Overview only
   - What is rbkApiModules (2-3 sentences)
   - Package table with one-line descriptions
   - Quick install commands
   - Minimal code sample (10-15 lines)
   - Links to detailed docs

2. **docs/getting-started.md** - Quick start guide
   - Prerequisites, 8-step setup, first login test, first protected endpoint

3. **docs/identity-authentication.md** - Authentication setup
   - JWT, Windows Auth, API keys, multi-tenancy, endpoint protection

4. **docs/identity-management.md** - Identity management
   - Users, roles, claims, tenants CRUD operations

5. **docs/Commons.Core.md** - Core infrastructure
   - Base entities, SmartValidator, Dispatcher, Authentication, File storage, Email

6. **docs/Testing.md** - Testing framework (preserved)
7. **docs/Analyzers.md** - Code analyzers (preserved)
8. **README-ApiKeys.md** - API keys (preserved as-is)

**Rationale:** Hub-and-spoke model improves discoverability vs. monolithic README. Progressive disclosure: overview → quick start → feature-specific depth. Easier to maintain and search.

**Gaps for future documentation:** Messaging.Core, UI Definitions, Application Options, Database seeding, Email handler, File storage, Localization, Complete endpoint API reference, IEndpoint pattern.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
