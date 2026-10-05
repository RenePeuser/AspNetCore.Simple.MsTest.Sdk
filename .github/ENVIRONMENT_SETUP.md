# Environment Setup - Production Approval

## Einmalige Einrichtung

### 1. Environment erstellen

Gehe zu: https://github.com/RenePeuser/AspNetCore.Simple.MsTest.Sdk/settings/environments

1. Klicke: **New environment**
2. Name: `production` (exakt so schreiben!)
3. Klicke: **Configure environment**

### 2. Protection Rules aktivieren

Im Environment `production`:

**✅ Required reviewers:**
- Klicke: **Add reviewer**
- Wähle dich selbst aus
- Optional: Weitere Team-Mitglieder hinzufügen

**✅ Deployment branches (optional, aber empfohlen):**
- Selected branches
- Klicke: **Add deployment branch rule**
  - Branch name pattern: `master` (für stable releases)
- Klicke: **Add deployment branch rule** (nochmal!)
  - Branch name pattern: `develop` (für alpha releases)
- **→ Nur master und develop können releasen!**

**Weitere Optionen (optional):**
- ⏱️ Wait timer: 0 Minuten (oder z.B. 5 Minuten Wartezeit)
- 🔒 Deployment protection rules: Keine zusätzlichen nötig

### 3. Speichern

Klicke: **Save protection rules**

---

## Wichtig: Beide Branches nutzen das gleiche Environment

- **`develop`** → Approval für Alpha-Pakete (`10.0.x-alpha.N`)
- **`master`** → Approval für Stable-Pakete (`10.0.x`)

**Beide erfordern deine manuelle Freigabe** bevor zu NuGet gepusht wird!

---

## ✅ Fertig!

Ab jetzt sieht der Workflow so aus:

```
┌─────────────────────────────────────────┐
│  1. Workflow gestartet                  │
│     ✓ Checkout                          │
│     ✓ Setup .NET                        │
│     ✓ Build (Debug)                     │
│     ✓ Tests                             │
│     ✓ Build (Release)                   │
│     ✓ Pack                              │
├─────────────────────────────────────────┤
│  ⏸️  WAITING FOR APPROVAL                │
│                                         │
│     👤 Review required                  │
│     🔔 Notification gesendet            │
│                                         │
│  [Approve] [Reject]                     │
├─────────────────────────────────────────┤
│  2. Nach Approval:                      │
│     ✓ Push to NuGet.org                 │
│     ✓ Create Git Tag                    │
│     ✓ Create GitHub Release             │
└─────────────────────────────────────────┘
```

---

## Visuell im GitHub UI

### Während des Wartens:

```
🟡 publish / production (waiting)
   ⏸️  Waiting for approval from @RenePeuser
   
   [Review pending deployments]
```

### Wenn du den Button klickst:

```
┌─────────────────────────────────────────┐
│ Review pending deployments              │
├─────────────────────────────────────────┤
│ Environment: production                 │
│ Version: 10.0.0-alpha.189               │
│                                         │
│ ✅ Approve deployment                   │
│ ❌ Reject deployment                    │
│                                         │
│ Comment (optional):                     │
│ ┌─────────────────────────────────────┐ │
│ │ Looks good, shipping it! 🚀         │ │
│ └─────────────────────────────────────┘ │
│                                         │
│           [Submit review]               │
└─────────────────────────────────────────┘
```

### Nach Approval:

```
✅ publish / production (approved by @RenePeuser)
   ✓ Push to NuGet.org
   ✓ Create Git Tag
   ✓ Summary
```

---

## Deployment History

Alle Deployments werden getrackt:

**Settings → Environments → production**

```
┌────────────────────────────────────────────────────┐
│ Deployment history                                 │
├────────────────────────────────────────────────────┤
│ ✅ 10.0.0        @RenePeuser    2 hours ago        │
│ ✅ 10.0.0-alpha  @RenePeuser    1 day ago          │
│ ❌ 9.5.16        @RenePeuser    3 days ago (rejected)│
└────────────────────────────────────────────────────┘
```

---

## Notifications

Du bekommst automatisch:
- 📧 **E-Mail**: "Deployment waiting for approval"
- 🔔 **GitHub Notification**: Im Notification-Center
- 📱 **Mobile**: Wenn GitHub App installiert

---

## Troubleshooting

### "Environment 'production' not found"
→ Stelle sicher, dass das Environment exakt `production` heißt (lowercase!)

### "No reviewers configured"
→ Gehe zu Environment Settings und füge mindestens einen Reviewer hinzu

### Kann nicht approven
→ Nur konfigurierte Reviewer können approven (check Environment settings)
