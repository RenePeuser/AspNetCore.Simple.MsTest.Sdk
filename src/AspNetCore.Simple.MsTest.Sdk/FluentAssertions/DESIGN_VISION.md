# Fluent Assert API — Design-Vision & Ideensammlung

> Arbeitsstand-Dokument. Hier halten wir Ideen, Risiken und Entscheidungen fest,
> damit wir später nahtlos weitermachen können. **Noch kein Beschluss, nur Vision.**
>
> Stand: 2026-07-16 · Fokus dieser Runde: **HTTP-Assertions zuerst**, Value-Assertions (`Assert.That.*`) später.

---

## 1. Ausgangslage (was heute existiert)

Zwei Welten, die noch nicht zusammenpassen:

1. **Value-Assertions** — `Assert.That.AreEqual(expected, actual, because, fix, …)`
   - *Eager*: werfen sofort bei Fehlschlag.
   - Alleinstellungsmerkmal: Pflicht-Parameter `because` (warum) + `fix` (wie beheben) → KI-/menschenfreundliche Fehlermeldungen.
   - Kein Terminal-Problem, aber lange positionale Signaturen.

2. **HTTP-Assertions** — massive Overload-Explosion
   - Beispiel: `Client.Assert.Get.cs` allein hat ~15 Overloads von `AssertGetAsync<T>`.
   - Der Fluent-Prototyp (`FluentAssertions/`) soll das ablösen:
     `IHttpRequestConfiguring → IHttpResponseConfiguring<T> → Terminal`.

### Beobachtungen zum aktuellen Prototyp

- **Zwei konkurrierende Stile** existieren parallel:
  - *Neutral*: `WithBody` / `WithResponse` / `Expect…`
  - *Endpoint*: `Accepts` / `Produces` (spiegelt ASP.NET-Core-Endpoint-Definitionen)
  - → verdoppelt die kognitive Last und lädt zu Team-Fragmentierung ein.
- **Doku-Drift ist bereits eingetreten:** `ProducesCreated`, `ProducesOk`, `ProducesBadRequest`,
  `ProducesNotFound`, `ProducesConflict`, `ProducesUnauthorized` etc. stehen in
  `README.md` / `EXAMPLES.md` als fertig beschrieben, **existieren im Code aber nicht**.
- `HttpClientFluentExtensions` ist noch `internal` + `// ToDo: under construction`.
- `Produces(int statusCode, json)` castet hart auf `HttpResponseBuilder<T>` und wirft sonst → leaky abstraction.
- `WithResponse<T>(json, bool expectSuccess)` wirft **zur Laufzeit**, wenn `false` übergeben wird.

---

## 2. Das Kernrisiko: der vergessene Terminal-Befehl

Fluent-APIs brauchen einen Terminal-Befehl. Bei einem **Test-SDK** ist das Vergessen
gefährlicher als anderswo:

```csharp
// Vergisst .ExpectSuccess() → Request wird NIE gesendet → Test ist GRÜN (falsch!)
Client.AssertPost("api/persons").WithBody(person).WithResponse<Person>("Expected.json");
```

Weil `IHttpRequestConfiguring` / `IHttpResponseConfiguring<T>` **keine** `Task` sind,
gibt es **nicht einmal eine `CS4014`-Warnung**. Der Test wird stillschweigend grün.

**Zweite Stufe:** Selbst *mit* Terminal, wenn dieser `Task<TResult>` liefert und man `await`
vergisst → Fire-and-forget. `IHttpStatusAssertable.GetAwaiter()` ist clever gelöst (direkt
awaitable), aber die `Task<TResult>`-Terminals der Response-Kette nicht.

> **Leitsatz:** Ein Test-SDK hat *ein* Alleinstellungsmerkmal zu verlieren — **Vertrauen,
> dass Grün wirklich Grün heißt.** Die Fluent-API tauscht die (hässliche, aber ehrliche)
> Overload-Explosion gegen eine schönere Oberfläche mit einer **neuen Klasse stiller Fehler**.
> Dieser Tausch lohnt sich **nur**, wenn der Analyzer vor dem breiten Rollout steht.

---

## 3. Angestrebte API — „die eine Kette"

Ein Stil, ein Terminal-Präfix, Typ-State erzwingt die Reihenfolge:

```csharp
await Client.Post("api/persons")          // Entry       → RequestConfig
    .WithBody(person)                      //               RequestConfig
    .WithParameters(("$Id$", 0))           //               RequestConfig
    .Returns<Person>("Expected.json")      // Transition   → ResponseConfig<Person>
    .IgnoreProperty(p => p.CreatedDate)    //               ResponseConfig<Person>
    .ExpectingStatus(Created);             // Terminal     → Task<Person>   ← einziger Endpunkt
```

**Merksatz (in einem Satz lernbar):** *Jede Kette endet mit genau einem `Expecting…`.*
→ Auge und Analyzer scannen trivial: „Steht am Ende ein `Expecting…`? Nein → Fehler."

### Terminal-Familie (bewusst klein halten)

| Terminal | Bedeutung | Erlaubt auf |
|---|---|---|
| `ExpectingSuccess()` | beliebiger 2xx | Request- & Response-Config |
| `ExpectingStatus(code)` | genau dieser Code | beide |
| `ExpectingOneOf(codes)` | einer aus Menge | beide |
| `ExpectingError(code)` | 4xx/5xx | beide |
| `ExpectingNoContent()` | 204 | **nur** ohne `Returns<T>` (Typ-State!) |

> Namensschema (`Expecting…`) ist die am schwersten rückgängig zu machende Entscheidung,
> sobald Tests existieren. Vor Rollout einmal komplett durchdeklinieren.

---

## 4. Drei Verteidigungslinien gegen stille Fehler

Von „gratis & stark" nach „optional & schwach":

1. **Typ-State (Compiler, gratis, stärkste Linie)**
   Ungültige Kombinationen sollen **Compilerfehler** sein, nicht Runtime-`throw`.
   - `ExpectingNoContent()` existiert auf `ResponseConfig<T>` gar nicht.
   - Kein `WithResponse(json, expectSuccess:false)`-throw mehr, kein `Produces`-Cast-throw.
   - Faustregel: *Je mehr „ungültig" schon der Compiler abfängt, desto weniger muss der Analyzer tragen.*

2. **Roslyn-Analyzer (Compile-Time)**
   Fängt genau den Rest, den Typ-State nicht kann: die **nie terminierte Kette**.

3. **Runtime-Netz (optional, verzichtbar)**
   Builder mit „terminiert?"-Flag + `IDisposable`/Finalizer, der im Debug laut warnt.
   → **Nicht drauf verlassen:** GC-Timing macht es unzuverlässig und es meldet erst *nach*
   dem grünen Test. Nur erwähnt, damit klar ist, dass es die schwache Linie ist.

---

## 5. Roslyn-Analyzer — konkretes Design

Fundament, nicht Beiwerk.

- **Marker statt Heuristik:** Attribut `[FluentBuilder]` (bzw. `[MustTerminate]`) auf
  `IHttpRequestConfiguring` / `IHttpResponseConfiguring<T>`. Dann muss der Analyzer keine
  Typnamen raten.
- **Diagnostics:**
  - `MSTESTSDK001` (Error): `ExpressionStatementSyntax`, dessen Ergebnistyp ein
    `[FluentBuilder]`-Interface ist → *„Assertion-Kette nie ausgeführt — Terminal (`Expecting…`)
    fehlt. Test kann fälschlich grün werden."*
  - `MSTESTSDK002` (Error/Warning): Terminal liefert `Task`, wird nicht awaited/returned
    (bessere Message als CS4014).
  - `MSTESTSDK003` (Info): unerreichbare Konfiguration nach Terminal.
- **CodeFix:** „Add `.ExpectingSuccess()`" bzw. `await` einfügen — 1-Klick.
- **Auslieferung:** als Analyzer-Asset im NuGet-Paket, damit er automatisch mitkommt.
- **Risiko am Analyzer selbst:** Ketten, die in einer Variable gespeichert und *später*
  terminiert werden (`var chain = Client.Post(...); … await chain.Expecting…();`), dürfen
  keinen false positive erzeugen. → **Anfangs konservativ**: nur den offensichtlichen
  „dangling expression statement"-Fall melden. Lieber wenige sichere Diagnostics als Fehlalarme.
- **Warum Analyzer > Runtime:** Compile-Time ist die einzige verlässliche, deterministische
  Ebene. Runtime-Prüfung meldet zu spät und ist GC-abhängig.

---

## 6. Weitere Risiken

| Risiko | Wirkung |
|---|---|
| **Zwei Stile** (Neutral vs. Endpoint) | Team-Fragmentierung, doppelte Doku, Drift (bereits eingetreten). |
| **`Assembly.GetCallingAssembly()` im Entry** | Wird die Kette in eine Test-Helper-Methode gewrappt (bei Fluent-APIs *häufiger*, weil sie zum Extrahieren einladen), zeigt die Assembly aufs Falsche → `Expected.json` wird nicht gefunden. Subtiler Fehler, taucht erst beim Refactoring auf. Bewusste Entscheidung nötig (z. B. `[CallerFilePath]` als primäre Quelle, Assembly nur Fallback). |
| **`SetExpectedJsonAndExecute` via harten Cast** | `Produces(int, json)` castet auf `HttpResponseBuilder<T>` und wirft sonst → bricht, sobald das Interface anders implementiert wird. |
| **`params HttpStatusCode[]` + `bool`-Overloads** | Runtime-`throw` bei legalem Aufruf — genau das, was Fluent+Typ-State vermeiden soll. |

---

## 7. Value-Assertions (`Assert.That.*`) — später, aber vorgemerkt

Entscheidung dieser Runde: **erst HTTP stabilisieren, Value-API vorerst unangetastet.**

Wenn es später drankommt, die Kernfrage: eager oder deferred?

- **Empfehlung Tendenz — eager lassen:** `IsEqualTo` wirft sofort; `because`/`fix` nur als
  Dekoration *davor* (`Assert.That(x, because:…, fix:…).IsEqualTo(y)`).
  → Führt das Terminal-Risiko **nicht** in einen Bereich ein, der es heute nicht hat.
- **Deferred (`Assert.That(x).IsEqualTo(y).Because(…)`)** ist konsistenter „alles fluent",
  aber dann muss der Analyzer lückenlos auch Value-Ketten abdecken.
- Leitlinie: *Konsistenz „alles fluent" ist weniger wert als „keine stillen grünen Tests".*

`because`/`fix` in jedem Fall behalten — echtes Feature. In der Fluent-Welt ggf. als klar
benannte Schritte statt positionaler Endparameter (bessere IntelliSense-Führung).

---

## 8. Empfohlene Reihenfolge (wenn es ans Bauen geht)

1. **Analyzer + Typ-State-Härtung zuerst** (Fundament) — inkl. `[FluentBuilder]`/`[MustTerminate]`-Marker.
2. **Einen Stil finalisieren** — Neutral als Kanon; Endpoint-Style streichen oder klar als
   optionalen Alias-Layer im separaten Namespace.
3. **Doku-Drift schließen** — dokumentierte `ProducesCreated/Ok/…` entweder bauen oder aus Doku raus.
4. **`internal` → `public`** schalten (erst wenn 1–3 stehen).

---

## 9. Offene Design-Fragen (bewusst noch offen)

- [ ] Terminal-Vokabular final durchdeklinieren (`Expecting…` vs. Alternativen).
- [ ] Assembly-/`CallerFilePath`-Strategie für Embedded-Resource-Resolution beim Wrappen.
- [ ] Rückgabe des Terminals: `Task<TResult>` bleibt — „Response fließt in nächsten Request"
      als prominente UX-Story (`var created = await …Post…; await …Get($"…/{created.Id}")`),
      weil sie den Fluent-Ansatz ggü. der Overload-API rechtfertigt.
- [ ] Endpoint-Style: streichen oder als dünner optionaler Layer behalten?
- [ ] Wann Value-Assertions angehen — und dann eager oder deferred?

---

## 10. Entscheidungslog

| Datum | Entscheidung | Begründung |
|---|---|---|
| 2026-07-16 | HTTP zuerst, Value-Assertions später | Fokus; HTTP hat das größere Overload-/Terminal-Problem. |
| 2026-07-16 | Analyzer ist Fundament, nicht optional | Ohne ihn ist Fluent für ein Test-SDK riskanter als die Overload-API. |
