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
await Client.AssertPost("api/persons")    // Entry       → RequestConfig
    .WithBody(person)                      //               RequestConfig
    .WithParameters(("$Id$", 0))           //               RequestConfig
    .Returns<Person>("Expected.json")      // Transition   → ResponseConfig<Person>
    .IgnoreProperty(p => p.CreatedDate)    //               ResponseConfig<Person>
    .ExpectingStatus(Created);             // Terminal     → Task<Person>   ← einziger Endpunkt
```

**Merksatz (in einem Satz lernbar):** *Jede Kette endet mit genau einem `Expecting…`.*
→ Auge und Analyzer scannen trivial: „Steht am Ende ein `Expecting…`? Nein → Fehler."

### Entry-Naming — `AssertPost` statt `Post` (übersehener Punkt, 2026-07-16)

Der Einstieg heißt bewusst **`Client.AssertPost/AssertGet/AssertPut/AssertPatch/AssertDelete`** —
nicht `Client.Post(…)`. Zwei Gründe:

1. **Assert sichtbar am Zeilenanfang.** `Client.Post("api/persons")` liest sich wie ein normaler
   Request-Helper (neben `PostAsync`) — der Assert-Charakter wäre unsichtbar, man bekäme den Eindruck
   „hier wird nur ein POST abgesetzt". Der `Assert`-Präfix macht die Absicht sofort klar und ist damit
   selbst eine **Verteidigungslinie** gegen die still-grüne Kette aus §2: eine dangling
   `Client.AssertPost(...).WithBody(x)`-Zeile schreit „da fehlt die Assertion", `Client.Post(...)` sähe
   wie legitimes fire-and-forget aus.
2. **Kompatibel zur alten Signatur.** Matcht direkt das bestehende `AssertPostAsync`/`AssertGetAsync` →
   Migration der ~1000 Bestands-Tests bleibt ein mechanischer Präfix-erhaltender Sweep
   (`AssertPostAsync(…)` → `AssertPost(…)….ExecuteAsync()`, vgl. [[feedback-mechanical-signature-sweep]]).

**Verworfene Alternativen:** `Client.Assert.Post(…)` (Gateway-Property — sauberere Gruppierung, aber
`.A.B.C`-Verschachtelung und kein direkter Match zur alten Signatur) und `Assert.Post(client, …)`
(MSTest-Stil — Client als Parameter bricht den „alles hängt am Client"-Fluss). **Flach + direkt** gewinnt.

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
- **CodeFix:** siehe §5.1 — eigener `CodeFixProvider`, der die Terminals automatisch repariert.
- **Auslieferung:** als Analyzer-Asset im NuGet-Paket, damit er automatisch mitkommt.

### 5.1 CodeFixProvider — Terminals automatisch fixen (Ideensammlung)

Der Analyzer *findet* die dangling Kette (MSTESTSDK001) — ein `CodeFixProvider` (Roslyn
`ExportCodeFixProvider`) soll sie **per Klick / Bulk-Fixall reparieren**. Das ist die natürliche
Ergänzung: Diagnose + 1-Klick-Reparatur.

Was der Fix anbietet (auf die reale Model-B-API gemünzt):
- **MSTESTSDK001 (dangling chain):** `.ExecuteAsync()` ans Ende anhängen UND `await` davorsetzen —
  in einem Fix. Je nach Kontext:
  - Ist die Kette schon terminiert bis auf `ExecuteAsync` (endet auf `Expecting…`) → nur
    `.ExecuteAsync()` + `await` ergänzen.
  - Fehlt auch eine Erwartung → zusätzlich `.ExpectingSuccess()` einschieben (als Default-Angebot;
    der Nutzer kann danach präzisieren).
  - Methoden-Rückgabetyp anpassen: `void`→`async Task` / `Task`→`async Task` falls nötig, damit
    `await` legal ist. (Sonst schlägt der Fix fehl — muss der Provider mitmachen.)
- **FixAll-Support:** über `WellKnownFixAllProviders.BatchFixer` → ganze Datei/Projekt/Solution auf
  einmal reparieren (relevant bei der `internal`→`public`-Umstellung, wenn Bestandscode migriert).

Offene Unterfragen:
- [ ] **`await` vs. `return`:** In `return Client.AssertPost(...)….ExecuteAsync();`-Tests (unser aktueller
      Stil!) darf der Fix KEIN `await` erzwingen — dort ist `return …ExecuteAsync()` korrekt. Der
      Provider muss Expression-body/`return`-Kontext erkennen und nur `.ExecuteAsync()` anhängen.
- [ ] **Welche Erwartung als Default?** `.ExpectingSuccess()` ist der sichere Default, aber ein
      blind eingefügtes Success kann eine andere Absicht überdecken. Evtl. mehrere CodeActions
      anbieten (Success / Status / Error) statt einer.
- [ ] **Scope:** erst der einfache „hänge `.ExecuteAsync()` an"-Fix (deckt den häufigsten Fall:
      Erwartung da, Terminal vergessen). `await`/Signatur-Umbau als Ausbaustufe.
- [ ] **Weitere Diagnostics mitfixen:** MSTESTSDK002 (Task nicht awaited) → `await` einfügen;
      MSTESTSDK003 (Config nach Terminal) → toten Aufruf entfernen.
- **Risiko am Analyzer selbst:** Ketten, die in einer Variable gespeichert und *später*
  terminiert werden (`var chain = Client.AssertPost(...); … await chain.Expecting…();`), dürfen
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
      als prominente UX-Story (`var created = await …AssertPost…; await …AssertGet($"…/{created.Id}")`),
      weil sie den Fluent-Ansatz ggü. der Overload-API rechtfertigt.
- [ ] Endpoint-Style: streichen oder als dünner optionaler Layer behalten?
- [ ] Wann Value-Assertions angehen — und dann eager oder deferred?
- [ ] Typisierte Selectors + Matcher/`Any()`-Konzept → siehe Abschnitt 10.
- [ ] Body-Input: explizite Methoden statt ratender `WithBody`-Heuristik → siehe Abschnitt 11.

---

## 10. Typisierte Selectors & Matcher (`Any()`) — Ideensammlung

Ausgangsbeobachtung (2026-07-16): Die Vision-Kette zeigt `IgnoreProperty(p => p.CreatedDate)`
**ohne** `<T>` — heute erzwingt der Prototyp aber `IgnoreProperty<Person>(...)`, weil `T` nicht an
`TResult` gebunden ist (der Lambda-Parameter `p` hat sonst keinen abgeleiteten Typ). Wenn wir `T`
an `TResult` koppeln, wird das `<T>` für Objekt-Responses redundant. Diese Selector-Ergonomie
soll durchgängig gelten — **nicht nur für `IgnoreProperty`, sondern auch für Parameter.**

### 10.1 Typisierte Parameter statt String-Platzhalter

Heute: `WithParameters(("$Id$", 0))` — Platzhalter als Magic-String, kein Refactoring-Support,
kein Compile-Check gegen die tatsächliche Property.

Vision: dieselbe Selector-Signatur wie `IgnoreProperty`:

```csharp
.WithParameter(p => p.Id, 0)                 // typsicher, statt ("$Id$", 0)
.WithParameter(p => p.Name, "Goku")
```

→ Refactoring-fest, IntelliSense-geführt, Tippfehler im Platzhalter werden Compilerfehler.

### 10.2 Matcher / `Any()` — die dritte Stufe zwischen „ignorieren" und „exakt"

Heute gibt es nur zwei Extreme:
- **`IgnoreProperty(p => p.Id)`** — Wert *und* Existenz/Form komplett egal.
- **exakter Vergleich** — Wert muss auf den Punkt stimmen.

Fehlt die Mitte: *„der Wert ist egal, ABER er muss eine gültige Guid / ein Datum / non-null sein."*

**Bevorzugte Form (Idee) — Matcher im Prädikat, eine `p => …`-Zeile:**

```csharp
.ForProperty(p => p.Id == Guid.Any())        // muss eine gültige Guid sein, Wert egal
.ForProperty(p => p.CreatedDate == Date.Any())
.ForProperty(p => p.Name != null)            // liest sich wie normale Bedingung
.ForProperty(p => p.Age is >= 0 and <= 120)
```

Liest sich wie eine gewöhnliche Boolean-Bedingung — kein zweites Argument, kein separates
Matcher-Vokabular im Vordergrund. `Guid.Any()` / `Date.Any()` sind dabei **Marker-Token**, keine
echten Laufzeitwerte.

> ⚠️ **Technischer Vorbehalt (entscheidend):** Das funktioniert NUR als
> `Expression<Func<T, bool>>`, nicht als kompiliertes `Func<T, bool>`. Würde `p.Id == Guid.Any()`
> real ausgeführt, gäbe `Guid.Any()` irgendeinen Wert zurück und `==` wäre schlicht `false`. Die
> SDK muss den **Ausdrucksbaum** entgegennehmen und selbst zerlegen: „linke Seite = Selector auf
> `Id`, rechte Seite = `Any`-Marker → prüfe nur, dass ein parsbarer Guid da ist." Das ist mehr
> Implementierungsaufwand (Expression-Visitor, der `== Guid.Any()`, `!= null`, `is`-Pattern etc.
> erkennt) als die Zwei-Argument-Form.

**Fallback-Form (einfacher zu bauen) — Matcher als zweites Argument:**

```csharp
.ForProperty(p => p.Id, Guid.Any())          // Selector + Matcher getrennt
.ForProperty(p => p.Name, Value.NotNull())
```

Hier ist der Selector ein normaler `Expression<Func<T, object?>>` (wie `IgnoreProperty`) und der
Matcher ein separates Objekt — kein Baum-Parsing der rechten Seite nötig. Weniger elegant, aber
deutlich billiger und robuster.

Kernnutzen (beide Formen): Server-generierte Felder (IDs, Timestamps) werden **nicht blind
ignoriert**, sondern auf *Typ/Form* geprüft — ein `Id: null` oder `Id: "abc"` fällt weiterhin durch,
obwohl der konkrete Guid-Wert nicht vorhersehbar ist. Das schließt genau die Lücke, die
`IgnoreProperty` heute offen lässt (dort würde auch Müll durchrutschen).

Methodenname noch offen (`.ForProperty` Arbeitsname — **`.Verify`/`.Match` verworfen**).

### 10.3 Das Kombinations-Problem (der spannende Teil)

Der wirklich knifflige Teil ist **nicht** der einzelne Matcher, sondern wie alles zusammenspielt.
Zwei Achsen kreuzen sich:

**Achse A — Woher kommt das Expected?**
- **C# Objekt** (`Returns<Person>(expectedObject)`): typisiert, aber wird intern serialisiert, um
  gegen die JSON-Response zu diffen.
- **JSON** (`Returns<Person>("Expected.json")` / Roh-String): kein typisiertes Objekt, evtl. mit
  `$Platzhalter$` und `filterFunc`/`differenceFunc`-Vorverarbeitung.

**Achse B — Wie wird verglichen?**
- exakter Diff (heute) · `IgnoreProperty` (Diff verwerfen) · **Matcher** (Form prüfen, Wert frei).

Der Bruch: Der Matcher ist **typisiert** (`p => p.Id`, Compile-Zeit gegen `TResult`), die Diff-Engine
arbeitet aber auf **`MemberPath`-Strings** (`"Id"`, `"Emails[0].CreatedDate"`). Es braucht also eine
**Selector→MemberPath-Übersetzung**, damit der Matcher an `ApplyDifferenceFiltering` andocken kann
(dieselbe Stelle wie `differenceFilter`/`differenceFunc`, siehe
[[project-differencefilter-object-response-gaps]]).

**Der Schlüssel: leere Objekt-Felder → Matcher füllt die Lücke.** Objekt-Expected ist *für sich
allein* fast unbrauchbar, weil ungesetzte Felder `null`/`default`/`0` sind:

```csharp
.Returns<Person>(new Person { Name = "Son Goku" })   // Id/Age/CreatedDate sind null/0/default
```

Exakter Vergleich würde `Id: null`, `Age: 0` verlangen — fast nie gewollt. Und „nur gesetzte Felder
vergleichen" ist unmöglich, weil `Age = 0` nicht von „nicht gesetzt" unterscheidbar ist (value
types). **Die Matcher lösen genau das:**

```csharp
await Client.AssertPost(...)
    .Returns<Person>(new Person { Name = "Son Goku" })  // exakte Felder: Name
    .ForProperty(p => p.Id == Guid.Any())               // server-generiert: irgendeine Guid
    .ForProperty(p => p.CreatedDate == Date.Any())      // server-generiert: irgendein Datum
    .ExpectingStatus(Accepted)
    .ExecuteAsync();
```

Das Objekt liefert die *bekannten* Felder, die Matcher decken die *server-generierten* ab — der Rest
muss exakt stimmen. **Deshalb** gehören Objekt-Expected und Matcher zusammen: das eine macht das
andere erst praktisch nutzbar. Das ist die eigentliche Antwort auf „warum überhaupt Matcher".

Offene Kernfragen dazu:
- [ ] **Wo greift der Matcher?** Vermutlich als weiterer Schritt in `ApplyDifferenceFiltering`: eine
      Difference an `MemberPath X` wird verworfen, WENN der Matcher für `X` die Form akzeptiert —
      und bleibt (= Fehler), wenn die Form nicht passt. Verhältnis/Reihenfolge zu global
      `DifferenceFunc` → per-assert func → filter klären.
- [ ] **„Nicht gesetzt"-Erkennung bei Objekt-Expected:** Wie unterscheidet die Engine ein bewusst
      gesetztes `Age = 0` von „egal"? Optionen: (a) alles exakt außer den Matcher-Feldern (heutiges
      Diff-Verhalten), (b) nullable-Wrapper/Sentinel. Tendenz (a) — Matcher sind die explizite
      Opt-out-Liste, kein implizites „leere Felder ignorieren".
- [ ] **JSON-Expected + Matcher:** Platzhalter (`$Id$`) vs. Matcher — konkurrieren die oder ergänzen
      sie sich? Ein `$Id$`-Platzhalter, der per Matcher als „irgendeine Guid" validiert wird, wäre
      die Brücke zwischen beiden Welten.
- [ ] **String-Vergleichspfad** (`StringComparisonStrategy`, `MemberPath "Line N"`): dort gibt es
      keine Properties → Matcher greift nur im JSON-/Objekt-Pfad, im reinen String-Pfad nicht. Klar
      dokumentieren.

### 10.4 Offene Unterfragen

- [ ] **Collection-Responses:** bei `TResult = List<Person>` zeigt `p` auf die Liste, nicht auf das
      Element. → Vermutlich zwei Überladungen nötig: bequem ohne `<T>` fürs Objekt, explizit `<T>`
      fürs Collection-Element (analog gilt das schon für `IgnoreProperty`).
- [ ] **Prädikat-Form vs. Zwei-Argument-Form:** `p => p.Id == Guid.Any()` (elegant, braucht
      Expression-Visitor) vs. `(p => p.Id, Guid.Any())` (billiger, robuster). Evtl. beide anbieten —
      Prädikat als Zucker, Zwei-Argument als Basis.
- [ ] **Matcher-Namespace/Vokabular:** `Guid.Any()` / `Date.Any()` / `Value.NotNull()` vs. ein
      einheitliches `Match.Guid()` / `Match.AnyDate()` / `Match.NotNull()`. Einheitliches Präfix
      erleichtert Discovery (wie beim `Expecting…`-Leitsatz).
- [ ] **Integration mit der Diff-Engine:** Matcher als spezielle `Difference`-Behandlung
      (`ApplyDifferenceFiltering`) modellieren — ein Matcher, der die Form prüft und die Difference
      nur dann verwirft, wenn die Form stimmt. Verhältnis zu `differenceFilter`/`differenceFunc` klären.
- [ ] **Eigene Matcher:** erweiterbar für Custom-Prüfungen (`Match.Custom(v => …)`)?
- [ ] **Methodenname:** `.ForProperty` ist Arbeitsname (`.Verify`, `.Match` verworfen). Alternativen
      erwägen (`.Ensure`, `.Require`, `.Where`; `.Expect…` kollidiert mit Terminals).

### 10.5 Wiederverwendbares Vergleichsprofil (`Settings`-Objekt) — Ideensammlung

Idee (2026-07-16): ein wiederverwendbares Konfig-Objekt, das Ignores UND Matcher bündelt, einmal
gebaut und an viele Ketten weitergegeben — statt `.IgnoreMember<Person>(p => p.Id)` in jedem Test zu
wiederholen. `IgnoreMember` ist im Kern eine **deklarative Skip-Anweisung** am Feld selbst (statt
einer handgeschriebenen `MemberPath.Contains(...)`-Closure).

```csharp
// einmal (z. B. pro Test-Klasse / Feature) bauen:
var settings = new CompareSettings()
    .IgnoreMember<Person>(p => p.Age)              // = IgnoreProperty, aber am Profil
    .IgnoreMember<Person>(p => p.CreatedDate)
    .ForProperty<Person>(p => p.Id == Guid.Any()); // Matcher gehört mit rein

// in beliebig vielen Ketten wiederverwenden:
await Client.AssertPost(...).Returns<Person>(obj).Using(settings).ExecuteAsync();
await Client.AssertGet(...).Returns<Person>(obj).Using(settings).ExecuteAsync();
```

**Warum das genau eine Lücke aus §13 schließt:** In den ~1000 Sdc-Tests gibt es heute nur zwei
Extreme — den EINEN globalen statischen `DifferenceFunc` (`ApiTestBase`, ignoriert
`CreatedAt`/`Id`/… überall) ODER inline-per-Kette. Das `Settings`-Objekt ist die komponierbare
**Mitte**: feingranularer als global, wiederverwendbarer als inline. Und es bündelt Ignores +
Matcher zu EINEM „Vergleichsprofil" statt zwei getrennten Mechanismen.

**Zwei Skip-Stärken, bewusst getrennt:** `IgnoreMember(p => p.Age)` = HARTER Skip (Wert *und* Form
egal → `Age: null`/`Age: "Müll"` rutscht durch). `ForProperty(p => p.Id == Guid.Any())` = WEICHER
Skip (Form geprüft, Wert frei → `Id: null` fällt durch). Beide docken an `ApplyDifferenceFiltering`
an, aber der harte Skip ist die Sorte Anweisung, die zu großzügig gesetzt einen Bug versteckt →
Matcher ist der Default für server-generierte Felder, `IgnoreMember` nur der „wirklich egal"-Notausgang.

Offene Unterfragen:
- [ ] **Name:** `CompareSettings` / `AssertProfile` / `ComparisonProfile`. Member-Methode heißt
      `IgnoreMember` (eigenständig, NICHT `VerifySettings`/`IgnoreProperty` — kein Verify-Klon-Eindruck;
      idealerweise GLEICHER Name an Profil und Kette).
- [ ] **Anwenden:** `.Using(settings)` / `.With(settings)` / `.Apply(settings)` als Ketten-Schritt.
- [ ] **Merge-Semantik:** Profil + zusätzliche Inline-Regeln in derselben Kette → additiv? Und
      Verhältnis zum globalen `DifferenceFunc` (Reihenfolge global → Profil → inline). Andockpunkt ist
      wieder `ApplyDifferenceFiltering` (siehe [[project-differencefilter-object-response-gaps]]).
- [ ] **Fluent + immutable?** `IgnoreMember` gibt neues Profil zurück (record `with`) statt zu mutieren
      → thread-safe bei parallelen Tests (relevant: Controllers.Test ist `Parallelize(ClassLevel)`).

---

## 11. Body-Input: explizit statt „Magic" — Ideensammlung

### 11.1 Das Problem mit dem heutigen `WithBody`

Der Prototyp hat **schon heute versteckte Magic**: `WithBody(string)` nimmt Datei*name*, Roh-JSON
und (via Generic-Overload) C#-Objekt entgegen und rät per `IsRawJson`-Heuristik, was gemeint ist
(beginnt mit `{`/`"` → Roh-JSON, sonst → Embedded-Resource-Dateiname):

```csharp
.WithBody(person)                    // C# Objekt  (Generic-Overload)
.WithBody("CreatePersonFull.json")   // Dateiname → Embedded Resource (geraten!)
.WithBody("{ \"name\": \"x\" }")     // Roh-JSON            (geraten!)
```

Der still-grüne Fehlerfall: `.WithBody("Person.json")` mit Tippfehler kann als „Roh-Text-Body"
durchrutschen statt als „Datei nicht gefunden" zu knallen → Request geht mit Müll raus → evtl.
grüner Test. Genau das Vertrauensleck aus Abschnitt 2 — nur an der Request-Seite.

### 11.2 Entscheidung: drei explizite Methoden, keine Heuristik

**Wir müssen NICHT kompatibel bleiben** — die Fluent-API ist neu und noch `internal`, kein
Bestandskunde hängt dran. Also lassen wir die rate-Heuristik komplett fallen und machen die
Intention im Methodennamen explizit. **Beschluss 2026-07-16 (Schema A):** gemeinsames Präfix
`WithBody…`, das Objekt bleibt der kürzeste Name (häufigster Fall), die *indirekten* Quellen tragen
`…From…`:

```csharp
.WithBody(person)                            // C# Objekt — Generic, typsicher (KEIN "From": das Objekt IST der Body)
.WithBodyFromJsonString("{ \"name\": \"x\" }") // Roh-JSON — explizit, kein Raten
.WithBodyFromEmbeddedJson("CreatePersonFull.json") // Embedded Resource — explizit
```

> **Warum Schema A** (vs. voll-symmetrisch `WithBodyFromObject` / vs. Suffix-only `WithBodyJson`):
> Es erfüllt beide Leitprinzipien gleichzeitig — gemeinsames, in IntelliSense scanbares Präfix
> (`WithBody…`, wie das `Expecting…`-Motiv) UND der Default-Fall bleibt der kürzeste, natürlichste
> Name. Die Asymmetrie „Objekt hat kein `From`" ist kein Makel, sondern korrekt: das Objekt *ist* der
> Body, es kommt nicht *von woanders*. `From` liest sich als „der Body wird *aus* dieser Quelle
> gebildet". Verworfen: Schema B (voll symmetrisch — macht den häufigsten Fall unnötig lang, killt die
> schöne `Returns<Person>(obj)`-Zeile), Schema C (`WithBodyJson`/`WithBodyEmbedded` — Herkunft zu
> schwach benannt), Schema D (`WithBodyFromString`/`WithBodyFromFile` — verliert „das ist eine
> Embedded Resource"-Präzision).

Gewinn (passt zum Leitsatz „Compiler > Analyzer > Runtime"): Die Absicht steht im Namen, nicht in
einer Laufzeit-Heuristik. `WithEmbeddedJson("Persons.json")` mit Tippfehler kann nur *eins*
bedeuten → sofortiger, klarer „Resource nicht gefunden"-Fehler statt eines Ratefalls. Der Analyzer
muss diese Ambiguität gar nicht erst tragen.

**Symmetrie-Entscheidung: Response-Seite spiegelt die Dreiteilung.** Damit Request- und
Expected-Seite dasselbe mentale Modell haben, bekommt `Returns` dieselbe explizite Aufteilung —
kein ratender `Returns<T>(string)`:

```csharp
.Returns<Person>(personObject)                            // C# Objekt (→ Matcher füllen leere Felder, siehe 10.3)
.ReturnsFromJsonString<Person>("{ \"name\": \"...\" }")   // Roh-JSON, explizit
.ReturnsFromEmbeddedJson<Person>("Expected.json")         // Embedded Resource, explizit
```

> **Response spiegelt Schema A** (Beschluss 2026-07-16): gemeinsames Präfix `Returns…`, Objekt =
> kürzester Name (`Returns<T>(obj)` — die Vorzeigezeile bleibt erhalten), indirekte Quellen tragen
> `…From…`. Symmetrisch zur Body-Seite: `WithBody`/`WithBodyFromJsonString`/`WithBodyFromEmbeddedJson`
> ↔ `Returns`/`ReturnsFromJsonString`/`ReturnsFromEmbeddedJson`.

**Das `<T>` ist bei ALLEN drei Pflicht** — auch bei den String-Varianten. Es treibt zweierlei:
(1) die Deserialisierung des Response-Body in `TResult`, (2) den Ketten-Zustand
`IHttpResponseConfiguring<TResult>`, ohne den `.ForProperty(p => p.Name)` keinen typisierten
`p` hätte. Bei der Objekt-Variante ist `<T>` aus dem Argument ableitbar, bei den String-Varianten
nicht → dort **muss** es explizit stehen, sonst kein Typ.

→ verbindet sich mit dem Objekt-vs-JSON-Problem in 10.3 (Objekt-Expected wird erst durch Matcher nutzbar).

### 11.3 Offene Unterfragen

- [x] **Namensschema final:** **entschieden 2026-07-16 → Schema A** (`WithBody` /
      `WithBodyFromJsonString` / `WithBodyFromEmbeddedJson` und `Returns` / `ReturnsFromJsonString` /
      `ReturnsFromEmbeddedJson`). Gemeinsames Präfix + Objekt kurz, indirekte Quellen mit `…From…`.
      Begründung + verworfene Schemata B/C/D siehe §11.2. **Code noch nicht umbenannt** (heute:
      `WithJsonString`/`WithEmbeddedJson`/`ReturnsJsonString`/`ReturnsEmbeddedJson`) → mechanischer
      Rename-Sweep offen, vgl. [[feedback-mechanical-signature-sweep]].
- [ ] **`WithEmbeddedJson` + Parameter:** Zusammenspiel mit `$Platzhalter$`-Substitution und der
      `CallerFilePath`/Assembly-Auflösung (Abschnitt 9) klar definieren.

---

## 13. Feature-Kandidaten aus echter Nutzung (Sdc.Console.Test, ~1000 Tests)

Erhoben 2026-07-16 aus der realen Consumer-Codebase `Sdc.Console.Test`. Diese Tests laufen HEUTE
auf der Overload-API + einer handgeschriebenen Domain-Wrapper-Schicht (`SdcTestClient`, `CapabilitiesV1.cs`
~1650 Zeilen). Was die Wrapper mühsam kapseln, ist das stärkste Signal dafür, was die Fluent-API können muss.

### 13.1 Bestätigt unsere bisherigen Ideen
- **Body/Expected aus Objekt / JSON / Datei** — alle drei kommen real vor (Objekt via `.ToJson()`,
  anonyme Inline-Objekte, `"UseCase_01.json"`-Dateien). ✅ deckt §11 ab.
- **Platzhalter-Substitution** `("$UniqueName$", x), ("$Id$", id)` — massiv genutzt, in Request- UND
  Expected-Dateien UND URLs. ✅ deckt §10.1 ab (typisierte Parameter würden das ersetzen).
- **differenceFunc/-filter zum Ignorieren dynamischer Felder** — allgegenwärtig. ✅ deckt §10.2/10.3 ab.

### 13.2 NEU — bedenkenswerte Features, die wir noch nicht hatten

- [ ] **Fluent Route- & Query-Params statt String-Interpolation.** Heute überall
      `$"api/.../{id}?stage={stage}&name={name}"` mit ad-hoc Null-Behandlung
      (`x.IsNullOrWhiteSpace() ? "" : $"&name={x}"`). Vision: `.Route(id).Query("stage", stage).Query("name", name)`
      — zentrale Kodierung + Null-Skipping. **Hoher Nutzen, hohe Häufigkeit.**

- [ ] **Per-Request Auth / Identität.** Heute NICHT am Call-Site möglich — Auth klebt am statischen
      `HttpClient`; Identitätswechsel nur über Mock der User-Directory. Vision: `.AsUser(token)` /
      `.AsUnauthorized()` / `.WithHeader(k, v)`. **Echte neue Fähigkeit, nicht nur Zucker.**
      Verzahnt sich mit den `AsUnauthorizedAsync`-Terminals (siehe unten).

- [ ] **Error-Terminals als Teil der EINEN Kette.** Heute getrennte Methoden-Familien pro Ausgang:
      `AssertXAsErrorAsync<T>("NotFound.json")`, `AssertXAsUnauthorizedAsync()`,
      `AssertXAsValidationErrorAsync<ValidationProblemDetailsExtended>()`, `AsForbidden` — mal ×Verb.
      Vision: dieselbe Kette, nur anderes Terminal → `.ExpectingError<ProblemDetails>(NotFound, "NotFound.json")`,
      `.ExpectingUnauthorized()`, `.ExpectingValidationError<T>(...)`. Passt exakt zur „kleine
      Terminal-Familie"-Idee (§3). Fehler-Response hat eigenen Typ (`ProblemDetails` /
      `ValidationProblemDetailsExtended`) → Terminal ist generisch über den Fehlertyp.

- [ ] **Snapshot-/Golden-File-Modus** (`writeResponse: true`). Real genutzt, um Expected-`.json`
      neu zu schreiben. Vision: `.WriteSnapshot()` existiert im Prototyp schon — als bewusstes Feature
      im Vokabular verankern (nicht nur bool-Flag).

- [ ] **Globale vs. per-Assert Ignore-Regeln.** `ApiTestBase` installiert einen GLOBALEN
      `DifferenceFunc`, der `CreatedAt`/`LastModifiedAt`/`Id`/`Tenant`/`ProjectId`/… überall ignoriert;
      per-Test kommen lokale dazu. Muss mit der Matcher-/Filter-Semantik aus §10.3 zusammenspielen
      (Reihenfolge global → per-assert ist in `ApplyDifferenceFiltering` schon geklärt, siehe
      [[project-differencefilter-object-response-gaps]]). Fluent: `.IgnoringPaths(".samples", ".name")`
      als Kurzform der handgeschriebenen `MemberPath.Contains(...)`-Closures.

- [ ] **`filterFunc` (Response normalisieren vor Vergleich).** Reorder/Normalize der deserialisierten
      Antwort (z. B. Listen sortieren) — existiert als `FilterResponse` im Prototyp. Bestätigt als
      nötig; im Vokabular halten.

- [ ] **Response-Objekt fließt in Folge-Requests (CRUD-Lifecycle).** Realer Dominant-Pattern:
      `create → id merken → get(id) → delete(id) → get(id)==404`, plus mehrstufiges Arrange
      (`capability.Id` in nächste Calls fädeln). ✅ bestätigt die `Task<TResult>`-Rückgabe-Story aus §9
      als KERN-Rechtfertigung der Fluent-API — nicht nur nette Deko.

- [ ] **Data-driven / Endpoint-Katalog** (`[DynamicRequestLocator]`, `[EnumTestCase<T>]`,
      OpenAPI-`AllEndpointsClient` „mach X gegen JEDEN Endpoint"). Wahrscheinlich AUSSERHALB der
      Fluent-Assert-Kette (MSTest-Attribut-Ebene), aber die Kette muss sich sauber in solche Loops
      einsetzen lassen (z. B. `.ForAllEndpoints().ExpectingRejected()` als denkbare Erweiterung). Nur
      als Fernziel notieren, nicht Kern-Scope.

- [ ] **AppSync-/Event-Assertions** (`AppSyncMessagesClient` — „wurde Event X publiziert?"). Eigene
      Domäne (nicht HTTP-Response), aber dieselbe Diff-/Expected-Philosophie. Fern; nur erwähnt.

### 13.3 Bewusst NICHT in die Fluent-Kette

- **Domain-Wrapper** (`TestClient.Capabilities.V1.AssertCreateAsync`) bleiben projektspezifisch —
  die Fluent-API ist die Basis, auf der solche Wrapper dünner werden, ersetzt sie aber nicht.
- **Mock-Setup / Test-Host-Bootstrap** (`MockRegistry`, `WebApplicationFactory`) — außerhalb Scope.
- **Reuse der bestehenden Pipeline:** Fluent-Kette soll `IAssertableHttpClient` /
  `HttpClientAssertExtensions.CustomAssertableHttpClient` + `IEmbeddedFileLocalizer` +
  `Difference`-Modell WIEDERVERWENDEN, nicht neu bauen (die Fassade-über-Engine-Strategie steht schon
  so im Prototyp — beibehalten).

---

## 14. Das Rückgabewert-Problem — `ExecuteAsync()` liefert die ECHTE Antwort

> Aufgeworfen 2026-07-16. **Am Code verifiziert 2026-07-16 → gelöst für den Hauptfall, kein Umbau
> nötig.** Es entscheidet, was `<T>` in der Kette bedeutet — und die Antwort ist: `<T>` ist EIN
> gemeinsamer generischer Faden für Vergleich UND Rückgabe, und das ist ein Feature, kein Konflikt.

### 14.0 Verifiziert: Der Rückgabewert ist bereits die echte Antwort (kein Bug, kein Breaking Change)

Am Code nachgewiesen (2026-07-16) — die aktuelle Signatur liefert das Gewünschte schon:

- `HttpResponseBuilder<T>.ExecuteAsync()` → `Task<T>` ruft `AssertHttpCallAsync<TResult>` (`HttpResponseBuilder.cs:153`).
- Dort wird ein `HttpAssertContext<TResult>` gebaut, die **echte HTTP-Antwort in `T` deserialisiert**
  und via Pipeline zurückgereicht (`Client.Assert.HttpCall.cs:666-668`).
- `HttpAssertionPipeline.Execute` gibt `context.CurrentResult` zurück — Kommentar dort wörtlich:
  *„All assertions passed — return the original deserialized result"* (`HttpAssertionPipeline.cs:45-46`);
  *„Steps are validators only — they don't modify or return results"* (`:39`).
- `AssertableHttpClient.AssertAsync` reicht denselben `result` durch (`AssertableHttpClient.cs:206-216`).

**Fazit:** `var person = await …ReturnsEmbeddedJson<Person>("Expected.json").ExpectingSuccess().ExecuteAsync();`
gibt HEUTE schon das **echte, deserialisierte `Person`** zurück (mit server-generierter `Id`/`CreatedDate`) —
NICHT den Inhalt von `Expected.json`. Das Expected fließt nur als Vergleichs-Vorlage in die Pipeline, wird
nie als Rückgabewert ausgegeben. Das eine `<T>` an `Returns…<T>` bindet den kompletten generischen Kanal
`HttpResponseBuilder<T> → AssertHttpCallAsync<T> → HttpAssertContext<T> → Task<T>` — **Vergleichstyp und
Rückgabetyp sind bewusst dasselbe `T`**.

> **Kein Breaking Change.** Für den Hauptfall (Vergleich + typisiertes Ergebnis) ist NICHTS zu ändern —
> die Rückgabe steht. Die Lücken aus §14.2 werden **rein additiv** geschlossen (neue Methoden, keine
> geänderten Signaturen). Zudem ist die Fluent-API noch `internal` → selbst additiv-inkompatible
> Schritte hätten keinen Enduser-Impact.

### 14.1 Das Symptom

### 14.1 Das Symptom

```csharp
var result = await Client.AssertPost("api/v1/persons")
    .WithBody(person)
    .ReturnsEmbeddedJson<Person>("CreatePersonFull.json")  // ← das <Person> tut ZWEI Jobs
    .ExpectingSuccess()
    .ExecuteAsync();                                        // → Task<Person>
```

`result` **muss die echte, deserialisierte Server-Antwort sein** — NICHT der Inhalt von
`CreatePersonFull.json`. Sonst ist der Rückgabewert wertlos: die server-generierte `Id`/`CreatedDate`
sind ja genau das, was der CRUD-Lifecycle in den Folge-Request fädeln will (§9, §13.2 „Response-Objekt
fließt in Folge-Requests"). Bekäme man das Expected zurück, hätte man nur, was man ohnehin schon
hatte. (Gleiche Fehlerklasse wie [[bug-primitive-type-comparison]]: *actual* vs. *expected*
verwechselt — hier auf Ebene des Rückgabewerts.)

### 14.2 Das eine `<T>` trägt zwei Rollen — bewusst, und für den Hauptfall gelöst

Das `<T>` an `ReturnsEmbeddedJson<Person>` trägt **gleichzeitig zwei Rollen**:

1. **Vergleichs-Vorlage** — „diffe die Antwort gegen `CreatePersonFull.json`, interpretiert als `Person`".
2. **Rückgabetyp** — „`ExecuteAsync()` gibt mir ein `Person` zurück".

Solange beide Rollen dasselbe `T` teilen, ist die Kette elegant — und **das ist der Normalfall, der
laut §14.0 bereits funktioniert.** Es bleiben genau **drei** Fälle, in denen die Rollen auseinanderfallen —
alle **rein additiv** lösbar (neue Methode/Terminal, keine geänderte Signatur):

- **Ergebnis OHNE Body-Vergleich** ist heute nicht ausdrückbar. Um `result: Person` zu bekommen, MUSS
  man ein Expected liefern und diffen. Der häufige CRUD-Fall „create, assert 201, gib mir das erzeugte
  Objekt — ohne Golden-File" hat heute nur den body-losen Pfad `ExpectingResponse()` → `Task` (also
  GAR kein Ergebnis). **→ additiver neuer Übergang `Reading<T>()` (siehe §14.3).**
- **Fehler-Response hat einen anderen Typ.** `.ExpectingError<ProblemDetails>(NotFound)` (§13.2) will
  ein `ProblemDetails` zurück, nicht den `Person`-Typ der Erfolgs-Kette. **→ additives generisches
  Fehler-Terminal, eigenes `T`.**
- **Collection vs. Element** (§10.4): `TResult = List<Person>` fürs Ergebnis, aber der Selector-`p`
  soll auf `Person` zeigen. **→ zwei Selector-Overloads (schon für `IgnoreProperty` vorgemerkt).**

### 14.3 Beschluss: additiver body-loser Pfad `Reading<T>()`

**Entschieden 2026-07-16.** Der Hauptfall bleibt unangetastet (`Returns…<T>` = Vergleich UND Rückgabe,
ein `T`). Für „Ergebnis ohne Vergleich" kommt ein **neuer, dritter Übergang** auf `IHttpRequestConfiguring`
dazu — er führt `<T>` ein (für Deserialisierung + Ketten-Zustand + `Task<T>`), difft aber nichts:

```csharp
// Hauptfall (unverändert): Vergleich + typisiertes Ergebnis
var person = await Client.AssertPost("api/v1/persons")
    .WithBody(create)
    .ReturnsFromEmbeddedJson<Person>("Expected.json")  // T: Vergleich + Rückgabe
    .ExpectingSuccess()
    .ExecuteAsync();                                    // Task<Person> ✓ (echte Antwort)

// NEU (additiv): nur deserialisieren & zurückgeben, KEIN Body-Diff
var person = await Client.AssertPost("api/v1/persons")
    .WithBody(create)
    .Reading<Person>()          // T: nur Rückgabetyp — kein Vergleich
    .ExpectingStatus(Created)
    .ExecuteAsync();            // Task<Person> ✓ (echte Antwort, kein Golden-File nötig)
```

Damit schließt sich die Lücke zwischen `ExpectingResponse()` (Task, kein Ergebnis) und `Returns…<T>`
(Ergebnis, aber Diff-Zwang) — ohne die bestehende Signatur zu brechen.

Offene Unterfragen (klein, nicht blockierend):
- [ ] **Name des body-losen Übergangs:** `Reading<T>()` (Arbeitsname) vs. `Deserializes<T>()` /
      `ReadingBody<T>()` / `Returning<T>()`. Konsistenz mit dem `Returns…`/`Expecting…`-Vokabular prüfen.
- [ ] **Fehler-Terminal generisch:** `ExpectingError<ProblemDetails>(NotFound)` als eigener Übergang mit
      eigenem `T` (nicht mit der Erfolgs-`Returns…`-Familie kollidierend) — Detaildesign siehe §13.2.
- [ ] **Intern:** `Reading<T>()` erzeugt denselben `HttpResponseBuilder<T>`-Kanal wie `Returns…<T>`, nur
      mit „kein Expected" (analog zum vorhandenen `IgnoreResponse`/`ExpectedObjectAsJson = IgnoreResponse`-
      Pfad in `Client.Assert.HttpCall.cs`). Kein neuer Engine-Pfad nötig — nur Fassade.
- [ ] **Fehler-Terminals mit eigenem Typ** (`ExpectingError<ProblemDetails>`) müssen konsistent in
      dasselbe Modell passen — sonst hat die Kette zwei widersprüchliche `<T>`-Quellen.
- [ ] **Verhältnis zu Matcher/Objekt-Expected (§10.3):** Objekt-Expected liefert bekannte Felder,
      Matcher die server-generierten. Der Rückgabewert ist die *ungefilterte* echte Antwort — Diff und
      Rückgabe operieren also auf demselben deserialisierten Objekt, aber mit unterschiedlichem Zweck.
      Sicherstellen, dass der zurückgegebene Wert vor keiner `FilterResponse`-Normalisierung „verbogen"
      ist (oder bewusst entscheiden, welchen der beiden man zurückgibt: roh vs. normalisiert).

### 14.4 Leitplanke

Der Rückgabewert ist laut §9 die **Kern-Rechtfertigung** der Fluent-API gegenüber der Overload-Welt
(`var created = await …AssertPost…; await …AssertGet($"…/{created.Id}")`). Wenn `ExecuteAsync()` nicht
zuverlässig die *echte* Antwort liefert — oder ein Ergebnis nur um den Preis eines erzwungenen
Body-Diffs zu haben ist — verliert die ganze Kette ihr stärkstes Argument. Diese Frage gehört
**vor** den breiten Rollout und den Namens-Rename geklärt.

---

## 15. Endpoint-Contract als Test-Surface — zwei getrennte Assert-Ebenen

> Aufgeworfen 2026-07-16. Ausgangspunkt war ein „Super-Endpoint" mit voller Vertragsdeklaration
> (Minimal-API `MapPost` + `Accepts` + 10× `Produces` + 5× `With…`-Metadata). Er ist der ideale
> Stresstest, weil er **die komplette Oberfläche eines Endpoints in einer Kette** zeigt.
> (Beispiel neutral auf `User` gehalten, damit direkt kopierbar.)

### 15.0 Der Referenz-Endpoint

```csharp
endpoints.MapPost("users", HandleAsync)
    .Accepts<CreateUserRequest>(MediaTypeNames.Application.Json)
    .Produces<CreateUserResponse>()                                        // 200
    .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
    .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
    .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
    .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
    .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
    .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
    .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)
    .Produces<string>(StatusCodes.Status504GatewayTimeout)                 // ← String-Body!
    .WithTags("Users")
    .WithName("createUserV1")
    .MapToApiVersion(1)
    .WithDescriptionFromFile("Description.md")
    .WithSummaryFromFile("Summary.md");
```

### 15.1 Die Kernunterscheidung: funktional vs. meta

Der Endpoint mischt **zwei grundverschiedene Assert-Welten**. Die Fluent-API muss sie bewusst
trennen — sonst entsteht genau die „zwei Stile"-Fragmentierung aus §6:

| Ebene | Frage | Braucht echten HTTP-Call? | Assert-Familie |
|---|---|---|---|
| **Funktional** (Request/Response) | „Was passiert, wenn ich den Endpoint *wirklich* aufrufe?" | **Ja** | die EINE Kette `Client.AssertPost(…)…Expecting…().ExecuteAsync()` |
| **Meta** (Contract/Doku) | „Ist der Endpoint *so deklariert*, wie er soll?" | **Nein** — liest Endpoint-Metadaten / OpenAPI | eigene Familie `Client.AssertEndpoint(…).Has…()` |

> **Leitsatz:** Funktional = Verhalten (Runtime, ein Terminal je Ausgang). Meta = Deklaration
> (statisch, kein Terminal-Risiko). Ein `Produces<T>(code)` erzeugt *funktional* genau einen
> möglichen Ausgang UND ist *meta* Teil des dokumentierten Contracts — dieselbe Zeile, zwei Blickwinkel.

### 15.2 Der Endpoint, Zeile für Zeile

| # | Verb / Wording | Argument | Status | Body-Typ | Ebene |
|---|---|---|---|---|---|
| 1 | `MapPost` | `"users"`, `HandleAsync` | — | — | funktional (Route) |
| 2 | `Accepts` | `Application.Json` | — | `CreateUserRequest` | funktional (Request) |
| 3 | `Produces` | — | `200` | `CreateUserResponse` | funktional (Response ✓) |
| 4 | `Produces` | `Status400BadRequest` | `400` | `ValidationProblemDetailsExtended` | funktional (Response ✗) |
| 5 | `Produces` | `Status401Unauthorized` | `401` | `ProblemDetails` | funktional (Response ✗) |
| 6 | `Produces` | `Status403Forbidden` | `403` | `ProblemDetails` | funktional (Response ✗) |
| 7 | `Produces` | `Status404NotFound` | `404` | `ProblemDetails` | funktional (Response ✗) |
| 8 | `Produces` | `Status409Conflict` | `409` | `ProblemDetails` | funktional (Response ✗) |
| 9 | `Produces` | `Status422UnprocessableEntity` | `422` | `ValidationProblemDetailsExtended` | funktional (Response ✗) |
| 10 | `Produces` | `Status500InternalServerError` | `500` | `ProblemDetails` | funktional (Response ✗) |
| 11 | `Produces` | `Status503ServiceUnavailable` | `503` | `ProblemDetails` | funktional (Response ✗) |
| 12 | `Produces` | `Status504GatewayTimeout` | `504` | **`string`** | funktional (Response ✗) |
| 13 | `WithTags` | `"Users"` | — | — | **meta** |
| 14 | `WithName` | `"createUserV1"` | — | — | **meta** |
| 15 | `MapToApiVersion` | `1` | — | — | **meta** |
| 16 | `WithDescriptionFromFile` | `"Description.md"` | — | — | **meta** |
| 17 | `WithSummaryFromFile` | `"Summary.md"` | — | — | **meta** |

### 15.3 Funktionale Assertions — die Response-Familie (`Produces` ×10)

Jedes `Produces<T>(code)` ⇒ genau ein Terminal auf der EINEN Kette. Body-Typ hängt am Code:

| `Produces`-Deklaration | Fluent-Terminal (Vorschlag) | Body-`<T>` |
|---|---|---|
| `Produces<CreateUserResponse>()` (200) | `.Returns<CreateUserResponse>(exp).ExpectingSuccess()` · Kurzform `.ExpectingOk<CreateUserResponse>()` | `CreateUserResponse` |
| `Produces<ValidationProblemDetailsExtended>(400)` | `.ExpectingValidationError<ValidationProblemDetailsExtended>(BadRequest, exp)` | `ValidationProblemDetailsExtended` |
| `Produces<ProblemDetails>(401)` | `.ExpectingUnauthorized()` | `ProblemDetails` (implizit) |
| `Produces<ProblemDetails>(403)` | `.ExpectingForbidden()` | `ProblemDetails` (implizit) |
| `Produces<ProblemDetails>(404)` | `.ExpectingNotFound()` · generisch `.ExpectingError<ProblemDetails>(NotFound, exp)` | `ProblemDetails` |
| `Produces<ProblemDetails>(409)` | `.ExpectingConflict()` | `ProblemDetails` (implizit) |
| `Produces<ValidationProblemDetailsExtended>(422)` | `.ExpectingValidationError<…>(UnprocessableEntity, exp)` | `ValidationProblemDetailsExtended` |
| `Produces<ProblemDetails>(500)` | `.ExpectingServerError()` · `.ExpectingError<ProblemDetails>(InternalServerError)` | `ProblemDetails` |
| `Produces<ProblemDetails>(503)` | `.ExpectingError<ProblemDetails>(ServiceUnavailable)` | `ProblemDetails` |
| `Produces<string>(504)` | `.ExpectingError<string>(GatewayTimeout, exp)` | **`string`** |

**Wichtigster Fund — jeder Code hat seinen eigenen Body-Typ**, und `504 → string` beweist: das
Fehler-Terminal **muss generisch über den Fehlertyp** sein (`ExpectingError<T>`), darf NICHT auf
`ProblemDetails` festgenagelt werden. Das bestätigt hart §13.2 und §14.2.

Body-Typen nach Häufigkeit (der Beweis fürs generische Terminal):

| Body-Typ | Codes | Häufigkeit |
|---|---|---|
| `ProblemDetails` | 401, 403, 404, 409, 500, 503 | 6× |
| `ValidationProblemDetailsExtended` | 400, 422 | 2× |
| `CreateUserResponse` | 200 | 1× |
| `string` | 504 | 1× |

**Terminal-Familie klein halten (Verfeinerung zu §3):** Der Reflex „für jeden Code ein `Expecting…`"
führt zurück in die Overload-Explosion, vor der wir fliehen. Stattdessen:
- **Generischer Kern:** `ExpectingStatus(code)`, `ExpectingError<T>(code, expected?)`, `ExpectingSuccess()`.
- **Benannte Aliase nur für die Alltags-Codes:** `ExpectingNotFound / Unauthorized / Forbidden /
  Conflict / ValidationError<T>` — reine Abkürzungen auf den Kern (impliziter `ProblemDetails`-Typ),
  kein neuer Engine-Pfad.
- `ExpectingValidationError<T>` verdient einen eigenen Namen: 400+422 mit
  `ValidationProblemDetailsExtended` sind hier 2 von 11 Ausgängen und real der häufigste Fehlerfall.

### 15.4 Meta-Assertions — die Contract-Familie (`With…` ×5)

Kein HTTP-Call, liest Endpoint-Metadaten / OpenAPI. Bewusst **eine separate Kette**, damit sich
funktional und meta nicht vermischen. Einstiegs-Selektor ist oft der `WithName` statt der URL:

| Deklaration | Meta-Assert (Vorschlag) |
|---|---|
| `.WithName("createUserV1")` | `Client.AssertEndpoint("createUserV1")` *(Selektor)* / `.HasName("createUserV1")` |
| `.WithTags("Users")` | `…HasTag("Users")` |
| `.MapToApiVersion(1)` | `…MapsToApiVersion(1)` |
| `.WithSummaryFromFile("Summary.md")` | `…HasSummaryFromEmbedded("Summary.md")` *(spiegelt `…FromEmbedded`, §11)* |
| `.WithDescriptionFromFile("Description.md")` | `…HasDescriptionFromEmbedded("Description.md")` |
| *(gesamter `Produces`-Block)* | `…DeclaresStatuses(200,400,401,403,404,409,422,500,503,504)` — Contract-Snapshot der dokumentierten Ausgänge |

**Namensschema meta:** durchgängig `Has…` / `Maps…` / `Declares…` — bewusst NICHT `Expecting…`,
damit Auge und Analyzer die beiden Familien sofort trennen (funktional endet auf `Expecting…` +
`ExecuteAsync()`, meta auf `Has…`). Ob die Meta-Kette ein eigenes Terminal braucht oder eager prüft,
ist offen (§15.5).

### 15.5 Offene Fragen zu §15

- [ ] **Meta-Familie eager oder terminiert?** `AssertEndpoint(…).HasTag(…)` könnte eager werfen
      (kein Terminal-Risiko, kein Analyzer nötig) oder derselben `Expecting…/ExecuteAsync`-Disziplin
      folgen. Tendenz: eager — Meta hat keinen Request, das Terminal-Argument aus §2 greift hier nicht.
- [ ] **Woher die Metadaten?** `EndpointDataSource` (In-Process, `WebApplicationFactory`) vs.
      generiertes OpenAPI-Dokument. Ersteres ist näher an der Deklaration, Letzteres testet das,
      was der Client wirklich sieht.
- [ ] **`Accepts` als eigene Assertion?** Content-Type-Erwartung (`.WithContentType(Json)`) ist
      funktional (Request-Seite), aber selten explizit gebraucht — als optionaler Request-Schritt halten.
- [ ] **Data-driven Contract-Katalog (Fernziel):** Der `Produces`-Block ist maschinenlesbar → ein
      Test könnte „ruf jeden deklarierten Ausgang ab, Body-Typ aus der Deklaration" fahren
      (`ExpectingDeclaredStatus(code)`). Passt zum `[DynamicRequestLocator]`-Katalog (§13.2). Nur Fernziel.

---

## 12. Entscheidungslog

| Datum | Entscheidung | Begründung |
|---|---|---|
| 2026-07-16 | HTTP zuerst, Value-Assertions später | Fokus; HTTP hat das größere Overload-/Terminal-Problem. |
| 2026-07-16 | Analyzer ist Fundament, nicht optional | Ohne ihn ist Fluent für ein Test-SDK riskanter als die Overload-API. |
| 2026-07-16 | Body-Input explizit (`WithBody`/`WithJsonString`/`WithEmbeddedJson`), keine rate-Heuristik | Fluent-API ist neu + `internal` → keine Kompatibilität nötig; Intention im Namen statt Laufzeit-Raten schließt einen still-grünen Fehlerfall. |
| 2026-07-16 | Response-Seite spiegelt die Dreiteilung: `Returns<T>(object)` / `ReturnsJsonString<T>` / `ReturnsEmbeddedJson<T>` | Symmetrie zur Body-Seite, gleiches mentales Modell; Objekt-Expected wird erst durch Matcher (§10.3) praktisch nutzbar. `<T>` bei allen dreien Pflicht (treibt Deserialisierung + Ketten-Zustand `IHttpResponseConfiguring<T>`). |
| 2026-07-16 | **Namensschema final = Schema A**: `WithBody`/`WithBodyFromJsonString`/`WithBodyFromEmbeddedJson` ↔ `Returns`/`ReturnsFromJsonString`/`ReturnsFromEmbeddedJson` | Gemeinsames scanbares Präfix (`WithBody…`/`Returns…`, wie `Expecting…`) UND Default-Fall (Objekt) bleibt kürzester Name. `…From…` nur für indirekte Quellen — das Objekt IST der Body/das Expected. Verworfen: B (voll-symmetrisch, killt `Returns<T>(obj)`), C (Suffix, Herkunft zu schwach), D (`FromFile`, verliert Embedded-Präzision). Code-Rename noch offen. |
| 2026-07-16 | Rückgabewert verifiziert: `ExecuteAsync()` liefert die ECHTE deserialisierte Antwort (`context.CurrentResult`), NICHT das Expected. `<T>` an `Returns…<T>` ist EIN Faden für Vergleich UND Rückgabe. | Am Code nachgewiesen (`HttpAssertionPipeline.cs:45`, `AssertableHttpClient.cs:206`, `Client.Assert.HttpCall.cs:666`). Kern-Rechtfertigung der Fluent-API (§9, CRUD-Lifecycle) steht damit ohne Umbau. Kein Breaking Change. |
| 2026-07-16 | Body-loser Ergebnis-Pfad `Reading<T>()` als additiver dritter Übergang | Schließt die einzige echte Lücke („Ergebnis ohne Golden-File-Diff") zwischen `ExpectingResponse()` (Task, kein Ergebnis) und `Returns…<T>` (Ergebnis, aber Diff-Zwang). Additiv → kein Breaking. Nutzt intern denselben `HttpResponseBuilder<T>`-Kanal + vorhandenen `IgnoreResponse`-Pfad. |
| 2026-07-16 | Entry heißt `AssertPost/AssertGet/AssertPut/AssertPatch/AssertDelete` (flach), NICHT `Post(…)` und nicht `Assert.Post(…)` | `Assert` sichtbar am Zeilenanfang → macht Assert-Charakter klar (sonst wirkt es wie ein bloßer POST) und ist eine Verteidigungslinie gegen die still-grüne Kette (§2). Flach statt Gateway-Property (`.A.B.C`) = direkter + matcht die alte `AssertPostAsync`-Signatur → mechanische Migration der ~1000 Tests. |
| 2026-07-16 | Zwei getrennte Assert-Ebenen: **funktional** (Request/Response, echter HTTP-Call, `Client.AssertPost…Expecting…().ExecuteAsync()`) vs. **meta** (Contract/Doku, kein Call, `Client.AssertEndpoint(…).Has…()`) — siehe §15 | Der „Super-Endpoint" (`Accepts` + 10× `Produces` + 5× `With…`) mischt Verhalten und Deklaration. Trennung verhindert die „zwei Stile"-Fragmentierung (§6); `Has…`/`Maps…`/`Declares…` (meta) vs. `Expecting…` (funktional) macht die Familien für Auge + Analyzer trennscharf. Fund: jeder Status-Code hat eigenen Body-Typ (`504 → string`!) → Fehler-Terminal MUSS `ExpectingError<T>` generisch sein (bestätigt §13.2/§14.2). |
