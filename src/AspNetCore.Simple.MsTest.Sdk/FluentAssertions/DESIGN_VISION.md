# Fluent Assert API — Design-Vision & Ideensammlung

> Arbeitsstand-Dokument. Hier halten wir Ideen, Risiken und Entscheidungen fest,
> damit wir später nahtlos weitermachen können.
>
> Stand: 2026-07-16 · Fokus: **HTTP-Assertions zuerst**, Value-Assertions (`Assert.That.*`) später.
>
> **Kanon (entschieden, Details §12-Log):** Ein Stil — der **Endpoint-Stil**. Die funktionale Kette
> lautet `Client.AssertPost(url)` → `Accepts(…)` → `Produces<T>(code)` → `ExpectedResponse…(…)` →
> `ExecuteAsync()`. `ExecuteAsync()` ist das einzige Terminal. Der frühere Neutral-Stil
> (`WithBody`/`Returns…`/`Expecting…`) ist verworfen; wo er unten noch auftaucht, ist er als Historie
> markiert. Code ist noch `internal`; Namens-Rename + Analyzer stehen vor dem Rollout aus.

---

## 1. Ausgangslage (was heute existiert)

Zwei Welten, die noch nicht zusammenpassen:

1. **Value-Assertions** — `Assert.That.AreEqual(expected, actual, because, fix, …)`
    - *Eager*: werfen sofort bei Fehlschlag.
    - Alleinstellungsmerkmal: Pflicht-Parameter `because` (warum) + `fix` (wie beheben) → KI-/menschenfreundliche
      Fehlermeldungen.
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
// Vergisst .ExecuteAsync() → Request wird NIE gesendet → Test ist GRÜN (falsch!)
Client.AssertPost("api/persons").Accepts(person).Produces<Person>(Created);
```

Weil die Builder-Interfaces **keine** `Task` sind, gibt es **nicht einmal eine `CS4014`-Warnung**.
Der Test wird stillschweigend grün.

**Zweite Stufe:** Selbst *mit* `ExecuteAsync()`, wenn man das `await` davor vergisst →
Fire-and-forget. Das `Task<TResult>` des Terminals muss awaited/returned werden (Analyzer
MSTESTSDK002, §5).

> **Leitsatz:** Ein Test-SDK hat *ein* Alleinstellungsmerkmal zu verlieren — **Vertrauen,
> dass Grün wirklich Grün heißt.** Die Fluent-API tauscht die (hässliche, aber ehrliche)
> Overload-Explosion gegen eine schönere Oberfläche mit einer **neuen Klasse stiller Fehler**.
> Dieser Tausch lohnt sich **nur**, wenn der Analyzer vor dem breiten Rollout steht.

---

## 3. Angestrebte API — „die eine Kette"

Ein Stil (Endpoint, §15), ein Terminal, Typ-State erzwingt die Reihenfolge:

```csharp
var created = await Client.AssertPost("api/persons")  // Entry        → RequestConfig
    .Accepts(person)                                   //                RequestConfig
    .WithParameter("Id", 0)                            //                RequestConfig
    .Produces<Person>(Created)                         // Transition   → ResponseConfig<Person> (Typ+Status)
    .ExpectedResponseFromEmbeddedJson("Expected.json") //                ResponseConfig<Person>
        .IgnoreProperty(p => p.CreatedDate)            //                ComparisonConfig<Person>
    .ExecuteAsync();                                   // Terminal      → Task<Person>  ← einziger Endpunkt
```

**Merksatz (in einem Satz lernbar):** *Jede Kette endet mit genau einem `ExecuteAsync()`.* Es ist der
einzige `await`-Punkt → Auge und Analyzer scannen trivial: „Steht am Ende `ExecuteAsync()`? Nein → Fehler."

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

### Status + Typ: `Produces<T>(code)` (statt einer `Expecting…`-Familie)

Status **und** Body-Typ **und** Rückgabetyp tragen gemeinsam `Produces<T>(code)` — kein Zoo aus
`ExpectingSuccess/Status/Error/NoContent`. Details + durchgespielte Fälle in §15.6.

| Aufruf                   | Bedeutung                                                           | Ergebnis                                     |
|--------------------------|---------------------------------------------------------------------|----------------------------------------------|
| `Produces<T>(code)`      | genau dieser Code, Body als `T`                                     | `Task<T>`                                    |
| `Produces(code)`         | genau dieser Code, kein Body (z. B. 204)                            | `Task` — Typ-State: kein `ExpectedResponse…` |
| *(exakter Code Pflicht)* | „irgendein 2xx" gibt es bewusst NICHT — Vertragstest nennt den Code | —                                            |

---

## 4. Drei Verteidigungslinien gegen stille Fehler

Von „gratis & stark" nach „optional & schwach":

1. **Typ-State (Compiler, gratis, stärkste Linie)**
   Ungültige Kombinationen sollen **Compilerfehler** sein, nicht Runtime-`throw`.
    - `ExpectedResponse…` existiert nur nach `Produces<T>(code)` (generisch), nicht nach `Produces(code)`
      ohne Body — 204+Body-Vergleich ist so gar nicht erst tippbar.
    - `IgnoreProperty`/`ForProperty` hängen am Zustand, den `ExpectedResponse…` zurückgibt — ohne
      Expected keine Vergleichs-Konfiguration (siehe §15.6).
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
    - ✅ `MSTESTSDK001` (Error, **implementiert**): `ExpressionStatementSyntax`, dessen Ergebnistyp ein
      `[FluentBuilder]`-Interface ist → *„Assertion-Kette nie ausgeführt — `ExecuteAsync()`
      fehlt. Test kann fälschlich grün werden."*
    - ✅ `MSTESTSDK002` (Error, **implementiert**): `ExecuteAsync()` liefert `Task`, wird nicht
      awaited/returned. Deckt auch `…ExecuteAsync().ConfigureAwait(false);` ab (Terminal liegt dort
      eine Ebene tiefer). Nicht gemeldet: `await`, `return`, Zuweisung und explizites `_ =`.
      Wichtiger als „bessere Message als CS4014": in einer **synchronen** Testmethode gibt es gar
      keine CS4014 — der Test ist dann still grün, auch bei falscher Erwartung.
    - ~~`MSTESTSDK003` (Info): unerreichbare Konfiguration nach `ExecuteAsync()`~~ — **nicht nötig,
      bereits abgedeckt.** Die direkte Form (`….ExecuteAsync().IgnoreProperty(…)`) kompiliert gar
      nicht: `ExecuteAsync()` liefert `Task<T>`, und darauf gibt es keine Builder-Member — der
      Type-State erledigt das ohne Analyzer. Die einzige erreichbare Form ist der gespeicherte
      Builder (`await chain.ExecuteAsync(); chain.WriteSnapshot();`), und die ist bereits ein
      dangling Builder-Expression-Statement → **MSTESTSDK001 feuert dort schon** (Test:
      `Fires_On_Configuration_After_The_Chain_Already_Ran`). Nur die Message nannte den zweiten Fall
      nicht; das steht jetzt in der Description der Regel.
- **CodeFix:** siehe §5.1 — eigener `CodeFixProvider`, der die Terminals automatisch repariert.
- **Auslieferung:** als Analyzer-Asset im NuGet-Paket, damit er automatisch mitkommt.

### 5.1 CodeFixProvider — Terminals automatisch fixen (Ideensammlung)

Der Analyzer *findet* die dangling Kette (MSTESTSDK001) — ein `CodeFixProvider` (Roslyn
`ExportCodeFixProvider`) soll sie **per Klick / Bulk-Fixall reparieren**. Das ist die natürliche
Ergänzung: Diagnose + 1-Klick-Reparatur.

Was der Fix anbietet:

- **MSTESTSDK001 (dangling chain):** `.ExecuteAsync()` ans Ende anhängen UND `await` davorsetzen —
  in einem Fix. Je nach Kontext:
    - Ist die Kette schon vollständig konfiguriert (endet auf `Produces<T>(code)` bzw.
      `ExpectedResponse…`) → nur `.ExecuteAsync()` + `await` ergänzen.
    - Fehlt auch ein `Produces` → nicht blind ergänzen (Status/Typ sind nicht ratbar) → Diagnose
      stehen lassen, der Nutzer muss den Ausgang benennen.
    - Methoden-Rückgabetyp anpassen: `void`→`async Task` / `Task`→`async Task` falls nötig, damit
      `await` legal ist. (Sonst schlägt der Fix fehl — muss der Provider mitmachen.)
- **FixAll-Support:** über `WellKnownFixAllProviders.BatchFixer` → ganze Datei/Projekt/Solution auf
  einmal reparieren (relevant bei der `internal`→`public`-Umstellung, wenn Bestandscode migriert).

Offene Unterfragen:

- [ ] **`await` vs. `return`:** In `return Client.AssertPost(...)….ExecuteAsync();`-Tests (unser aktueller
  Stil!) darf der Fix KEIN `await` erzwingen — dort ist `return …ExecuteAsync()` korrekt. Der
  Provider muss Expression-body/`return`-Kontext erkennen und nur `.ExecuteAsync()` anhängen.
- [ ] **Scope:** erst der einfache „hänge `.ExecuteAsync()` an"-Fix (deckt den häufigsten Fall:
  `Produces` da, Terminal vergessen). `await`/Signatur-Umbau als Ausbaustufe.
- [x] **Weitere Diagnostics mitfixen:** MSTESTSDK002 (Task nicht awaited) → `await` einfügen —
  erledigt via `UnawaitedFluentTerminalCodeFixProvider`; teilt sich die Rewrite-Logik
  (`FluentChainFixer`) mit dem 001-Fix, setzt aber nur `await` davor statt ein zweites
  `.ExecuteAsync()` anzuhängen. Offen: MSTESTSDK003 (Config nach Terminal) → toten Aufruf entfernen.
- **Risiko am Analyzer selbst:** Ketten, die in einer Variable gespeichert und *später*
  terminiert werden (`var chain = Client.AssertPost(...); … await chain.ExecuteAsync();`), dürfen
  keinen false positive erzeugen. → **Anfangs konservativ**: nur den offensichtlichen
  „dangling expression statement"-Fall melden. Lieber wenige sichere Diagnostics als Fehlalarme.
- **Warum Analyzer > Runtime:** Compile-Time ist die einzige verlässliche, deterministische
  Ebene. Runtime-Prüfung meldet zu spät und ist GC-abhängig.

---

## 6. Weitere Risiken

| Risiko                                           | Wirkung                                                                                                                                                                                                                                        | Status                                                                               |
|--------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------|
| **Zwei Stile** (Neutral vs. Endpoint)            | Team-Fragmentierung, doppelte Doku, Drift.                                                                                                                                                                                                     | ✅ **entschärft** — Endpoint ist alleiniger Kanon (§15), Neutral verworfen.           |
| **`Assembly.GetCallingAssembly()` im Entry**     | Wird die Kette in eine Test-Helper-Methode gewrappt (bei Fluent-APIs *häufiger*, weil sie zum Extrahieren einladen), zeigt die Assembly aufs Falsche → `Expected.json` wird nicht gefunden. Subtiler Fehler, taucht erst beim Refactoring auf. | offen — Entscheidung nötig (z. B. `[CallerFilePath]` primär, Assembly nur Fallback). |
| **Hart-Cast im Expected-Pfad**                   | Ein Cast auf `HttpResponseBuilder<T>`, der sonst wirft → bricht, sobald das Interface anders implementiert wird.                                                                                                                               | offen — beim Umbau auf `Produces<T>` mit auflösen.                                   |
| **`params HttpStatusCode[]` + `bool`-Overloads** | Runtime-`throw` bei legalem Aufruf — genau das, was Fluent+Typ-State vermeiden soll.                                                                                                                                                           | offen.                                                                               |

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

1. **Analyzer + Typ-State-Härtung zuerst** (Fundament) — inkl. `[FluentBuilder]`-Marker; Terminal-Anker
   ist `ExecuteAsync()` (§5).
2. **Stil ist entschieden** (Endpoint, §15) → jetzt umsetzen: `Accepts`/`Produces<T>(code)`/
   `ExpectedResponse…` bauen, Neutral-Stil (`WithBody`/`Returns…`/`Expecting…`) entfernen.
3. **Doku-Drift schließen** — dokumentierte `ProducesCreated/Ok/…` entweder bauen oder aus Doku raus.
4. **`internal` → `public`** schalten (erst wenn 1–3 stehen).

---

## 9. Offene Design-Fragen (bewusst noch offen)

- [x] Terminal-Vokabular → **entschieden:** `ExecuteAsync()` als einziges Terminal, `Produces<T>(code)`
  trägt Status+Typ (§15.6).
- [x] Endpoint-Style vs. Neutral → **entschieden:** Endpoint ist alleiniger Kanon (§15).
- [x] Body-Input explizit statt Heuristik → **entschieden:** Schema A (§11).
- [x] Rückgabe des Terminals `Task<TResult>` → **verifiziert** (§14): liefert die echte Antwort;
  „Response fließt in nächsten Request" ist die Kern-UX-Story (CRUD-Lifecycle).
- [ ] Assembly-/`CallerFilePath`-Strategie für Embedded-Resource-Resolution beim Wrappen.
- [ ] Wann Value-Assertions angehen — und dann eager oder deferred? (§7)
- [ ] Typisierte Selectors + Matcher/`Any()`-Konzept → siehe §10.

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

**Beschluss 2026-07-16 — zwei Overloads unter EINEM Verb `WithParameter`.** Die typisierte Form
deckt Platzhalter ab, die einer Property entsprechen. Für Platzhalter **ohne** Property-Pendant
(`UniqueName`, `Timestamp`, `CorrelationId` — real massiv genutzt, §13.1) und URL-Platzhalter gibt es
die String-Form. Kein Duplikat wie die „zwei Stile" (§6): die Formen decken disjunkte Fälle ab.

```csharp
.WithParameter(p => p.Id, 0)             // typisiert  — Selektor gegen die Property
.WithParameter("UniqueName", uniqueName) // string     — Platzhalter OHNE Property-Pendant
```

**Ein Verb, nicht `With…`/`Add…`:** es ist dieselbe fachliche Operation („setze einen Platzhalter"),
nur zwei Adressierungen (Selektor vs. Key). Ein Verb + Overload hält sie zusammen; IntelliSense zeigt
beide Signaturen unter einem Namen. (Im Gegensatz zu Schema A §11, wo `…From…` verschiedene *Quellen*
unterscheidet — hier ist die Quelle dieselbe.)

**Kein `$…$` im Test-Code — die SDK escapt intern (Beschluss 2026-07-16).** Der Nutzer schreibt den
**nackten Namen** (`"UniqueName"`), die SDK ergänzt die Delimiter selbst. Das ist mehr als Kosmetik:
die Platzhalter-Konvention (`$…$` heute, evtl. `{{…}}` morgen) wird zur reinen **Implementierungs-
Entscheidung** — änderbar, ohne einen einzigen Test anzufassen. `$` im Test-Code wäre ein Leak der
internen Repräsentation; derselbe Leitgedanke wie „Intention im Namen statt Magic-String" aus §4/§11.

```csharp
.WithParameter("UniqueName", x)   // ✓ EMPFOHLEN — SDK macht intern "$UniqueName$" (Konvention beliebig)
.WithParameter("$UniqueName$", x) // ✓ toleriert — SDK erkennt „schon escaped" und lässt es unverändert
```

**Toleranz gegenüber bereits-escapten Eingaben (Beschluss 2026-07-16, Postel's Law).** Nackt ist die
*Empfehlung*, aber die SDK **erkennt bereits gesetzte Delimiter und behält sie bei** (kein doppeltes
`$$UniqueName$$`). Zwei Gründe:

1. **Migration:** die ~1000 Bestands-Tests nutzen `("$Id$", 0)` → der Sweep muss die `$` NICHT strippen,
   bleibt ein reiner Signatur-Umbau (vgl. [[feedback-mechanical-signature-sweep]]).
2. **Robustheit:** Copy-Paste aus JSON-Dateien (wo `$…$` wörtlich steht) knallt nicht.

Bewusster Trade-off: „beide Formen erlaubt" weicht die *eine kanonische Form* leicht auf — akzeptiert,
weil der Migrationsnutzen überwiegt und die Erkennungsregel trivial ist (Name in bekannten Delimitern
gewrappt → unverändert; sonst → wrappen). Doku/Analyzer dürfen die nackte Form als Stil empfehlen.

Offene Unterfrage:

- [ ] **Kollision/Doppelung:** Wenn derselbe Platzhalter typisiert UND per String gesetzt wird — Fehler
  oder „letzter gewinnt"? Tendenz: früh und laut fehlschlagen (still-grün vermeiden, §2).

### 10.1.1 Objekt-Bulk: `WithParameters(obj)` — alle Properties auf einmal (Ideensammlung 2026-07-16)

Statt N Einzelaufrufe ein ganzes Objekt reingeben; jede Property wird zu einem Platzhalter:

```csharp
// statt:
.WithParameter("Name", user.Name)
.WithParameter("Age", user.Age)
.WithParameter("Email", user.Email)

// ein Objekt, alle Properties → Parameter:
.WithParameters(user)   // Name→$Name$, Age→$Age$, Email→$Email$
```

Real nützlich beim Arrange-Muster aus §13.1, wo oft ein ganzes Objekt in `$…$`-Platzhalter fließt
(Request- UND Expected-Dateien UND URL).

**Zwei Design-Entscheidungen (2026-07-16, bewusst die einfachen/vorhersehbaren Varianten):**

1. **Property-Name = C#-Name (PascalCase).** `user.CreatedAt → $CreatedAt$`, 1:1 wie im Test-Code
   lesbar. Bewusst NICHT durch die `JsonSerializerOptions`-CamelCase-Policy geschickt.
   > ⚠️ **Falle:** Wenn Platzhalter aus JSON-Templates stammen und dort `$createdAt$` heißen (camelCase,
   > wie serialisiert), matcht `$CreatedAt$` **nicht** → Platzhalter bleibt als Literal stehen →
   > still-grüner Fehler (§2). Muss laut dokumentiert werden; ggf. später eine Diagnose „Platzhalter
   > nach Substitution noch vorhanden" als Sicherung.

2. **Alle Properties, auch `default`/`null`/`0`.** Vorhersehbar (kein „mal ist die Property da, mal
   nicht"), aber:
   > ⚠️ **Falle:** `WithParameters(user)` mit `Age = 0` setzt `$Age$ = "0"`, auch wenn „nicht gesetzt"
   > gemeint war — das value-type-Problem aus §10.3. Der User muss wissen: das befüllt **jedes** Feld.
   > Wer selektiv will, nimmt die Einzel-`WithParameter`-Form.

**Naming:** `WithParameters` (Plural) = Objekt-Bulk, `WithParameter` (Singular) = ein Platzhalter →
trennt die Overloads natürlich. Koexistiert mit dem bestehenden `WithParameters(params (string,object)[])`
(Tupel-Array) — Overload-Set bewusst ordnen.

**Abgrenzung zu `Accepts(obj)` (wichtig, sonst Verwirrung):** Beide nehmen dasselbe Objekt und zerlegen
es — aber `Accepts(user)` → Request-**Body** (ganzer Payload), `WithParameters(user)` → **Platzhalter-Werte**
für `$…$` in Template-Dateien/URL. Anderes Ziel, klar dokumentieren.

Offene Unterfragen:

- [ ] **Verschachtelung:** `user.Address.City` → `$City$` / `$Address.City$` / gar nicht? Tendenz:
  **nur top-level flach**, Verschachtelung NICHT automatisch (sonst unvorhersehbar + `City`-Kollision
  aus mehreren Sub-Objekten).
- [ ] **Kollision `WithParameters(obj)` + Einzel-`WithParameter`:** hier eher „letzter gewinnt"
  (Bulk + bewusstes Override), NICHT laut fehlschlagen — sonst ist „Objekt + eine Ausnahme"
  unmöglich. (Steht im Spannungsverhältnis zur Einzel-Kollisionsregel oben — bewusst getrennt.)

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

- **C# Objekt** (`ExpectedResponse(expectedObject)`): typisiert, aber wird intern serialisiert, um
  gegen die JSON-Response zu diffen.
- **JSON** (`ExpectedResponseFromEmbeddedJson("Expected.json")` / Roh-String): kein typisiertes Objekt,
  evtl. mit `$Platzhalter$` und `filterFunc`/`differenceFunc`-Vorverarbeitung.

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
.ExpectedResponse(new Person { Name = "Son Goku" })   // Id/Age/CreatedDate sind null/0/default
```

Exakter Vergleich würde `Id: null`, `Age: 0` verlangen — fast nie gewollt. Und „nur gesetzte Felder
vergleichen" ist unmöglich, weil `Age = 0` nicht von „nicht gesetzt" unterscheidbar ist (value
types). **Die Matcher lösen genau das:**

```csharp
await Client.AssertPost(...)
    .Accepts(request)
    .Produces<Person>(Accepted)
    .ExpectedResponse(new Person { Name = "Son Goku" })  // exakte Felder: Name
        .ForProperty(p => p.Id == Guid.Any())            // server-generiert: irgendeine Guid
        .ForProperty(p => p.CreatedDate == Date.Any())   // server-generiert: irgendein Datum
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
  erleichtert Discovery (wie das scanbare `Produces…`/`ExpectedResponse…`-Vokabular).
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
await Client.AssertPost(...).Accepts(req).Produces<Person>(Created).ExpectedResponse(obj).Using(settings).ExecuteAsync();
await Client.AssertGet(...).Produces<Person>(OK).ExpectedResponse(obj).Using(settings).ExecuteAsync();
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

## 11. Body-Input: explizit statt „Magic" (Schema A)

> **Namen aktualisiert:** Der Abschnitt entstand am Neutral-Stil (`WithBody`/`Returns…`). Das
> **Prinzip Schema A** (gemeinsames Präfix, Objekt kurz, `…From…` für indirekte Quellen) gilt
> unverändert — es wurde nur auf die kanonischen Endpoint-Namen übertragen: **`Accepts…`** (Request)
> und **`ExpectedResponse…`** (Response). Konkret: `Accepts(obj)` / `AcceptsFromJsonString` /
> `AcceptsFromEmbeddedJson` und `ExpectedResponse(obj)` / `ExpectedResponseFromJsonString` /
> `ExpectedResponseFromEmbeddedJson`. Die `WithBody`/`Returns`-Beispiele unten zeigen die Herleitung.

### 11.1 Das Problem mit der `WithBody`-Heuristik (historisch)

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

- [x] **Namensschema final:** **entschieden 2026-07-16 → Schema A**, angewandt auf die Endpoint-Namen:
  `Accepts` / `AcceptsFromJsonString` / `AcceptsFromEmbeddedJson` (Request) und `ExpectedResponse` /
  `ExpectedResponseFromJsonString` / `ExpectedResponseFromEmbeddedJson` (Response). Gemeinsames
  Präfix + Objekt kurz, indirekte Quellen mit `…From…`. Begründung + verworfene Schemata B/C/D §11.2.
  **Code noch nicht umbenannt** (heute: `WithBody`/`WithJsonString`/`WithEmbeddedJson`/`Returns…`) →
  mechanischer Rename-Sweep offen, vgl. [[feedback-mechanical-signature-sweep]].
- [ ] **`…FromEmbeddedJson` + Parameter:** Zusammenspiel mit Platzhalter-Substitution und der
  `CallerFilePath`/Assembly-Auflösung (§9) klar definieren.
- [ ] **`Accepts`-Verb vs. `ExpectedResponse`-Substantiv:** die `…From…`-Anhängung liest sich
  unterschiedlich (`AcceptsFromEmbeddedJson` vs. `ExpectedResponseFromEmbeddedJson`) — offen, ob
  `Accepts` ein Substantiv-Pendant braucht (siehe §15.7).

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

- [x] **Error-Ausgänge als Teil der EINEN Kette.** Heute getrennte Methoden-Familien pro Ausgang:
  `AssertXAsErrorAsync<T>("NotFound.json")`, `AssertXAsUnauthorizedAsync()`,
  `AssertXAsValidationErrorAsync<ValidationProblemDetailsExtended>()`, `AsForbidden` — mal ×Verb.
  **Gelöst durch `Produces<T>(code)` (§15.6):** derselbe Übergang, nur anderer Typ + Code →
  `.Produces<ProblemDetails>(NotFound).ExpectedResponseFromEmbeddedJson("NotFound.json")`,
  `.Produces<ProblemDetails>(Unauthorized)`, `.Produces<ValidationProblemDetailsExtended>(BadRequest)`.
  Fehler-Response hat eigenen Typ → `<T>` ist generisch über den Fehlertyp (der `504 → string`-Fund
  aus §15.3 beweist: nicht auf `ProblemDetails` festnageln).

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
  einsetzen lassen (z. B. `.ForAllEndpoints().Produces<ProblemDetails>(…)` als denkbare Erweiterung).
  Nur als Fernziel notieren, nicht Kern-Scope.

- [ ] **AppSync-/Event-Assertions** (`AppSyncMessagesClient` — „wurde Event X publiziert?"). Eigene
  Domäne (nicht HTTP-Response), aber dieselbe Diff-/Expected-Philosophie. Fern; nur erwähnt.

### 13.3 Bewusst NICHT in die Fluent-Kette

- **Domain-Wrapper** (`TestClient.Capabilities.V1.AssertCreateAsync`) bleiben projektspezifisch —
  die Fluent-API ist die Basis, auf der solche Wrapper dünner werden, ersetzt sie aber nicht.
- **Mock-Setup / Test-Host-Bootstrap** (`MockRegistry`, `WebApplicationFactory`) — außerhalb Scope.
- **Reuse der bestehenden Pipeline:** Fluent-Kette soll `IAssertableHttpClient` (aus dem Container,
  aufgelöst über `HttpClientAssertExtensions.GetService`) + `IEmbeddedFileLocalizer` +
  `Difference`-Modell WIEDERVERWENDEN, nicht neu bauen (die Fassade-über-Engine-Strategie steht schon
  so im Prototyp — beibehalten).

---

## 14. Das Rückgabewert-Problem — `ExecuteAsync()` liefert die ECHTE Antwort

> Aufgeworfen + am Code verifiziert 2026-07-16. **Gelöst, kein Umbau nötig.** `<T>` ist EIN
> gemeinsamer generischer Faden für Vergleich UND Rückgabe — ein Feature, kein Konflikt.

### 14.1 Verifiziert: Der Rückgabewert ist bereits die echte Antwort (kein Breaking Change)

Am Code nachgewiesen — die Pipeline gibt die **echte, deserialisierte HTTP-Antwort** zurück, NICHT das
Expected:

- `HttpResponseBuilder<T>.ExecuteAsync()` → `Task<T>` → `AssertHttpCallAsync<TResult>` (`HttpResponseBuilder.cs:153`),
  baut `HttpAssertContext<TResult>`, deserialisiert die echte Antwort in `T` (`Client.Assert.HttpCall.cs:666-668`).
- `HttpAssertionPipeline.Execute` gibt `context.CurrentResult` zurück — wörtlich: *„All assertions
  passed — return the original deserialized result"* (`HttpAssertionPipeline.cs:45-46`), *„Steps are
  validators only"* (`:39`); `AssertableHttpClient.AssertAsync` reicht denselben `result` durch (`:206-216`).

Also gibt `ExecuteAsync()` das **echte, deserialisierte Objekt** zurück (mit server-generierter
`Id`/`CreatedDate`) — das Expected fließt nur als Vergleichs-Vorlage in die Pipeline. Das eine `<T>`
bindet den kompletten Kanal `Builder<T> → HttpAssertContext<T> → Task<T>`: **Vergleichstyp und
Rückgabetyp sind bewusst dasselbe `T`** (gleiche Fehlerklasse wie [[bug-primitive-type-comparison]] wäre
es, das Expected zurückzugeben — passiert aber nicht).

### 14.2 Die zwei Rollen von `<T>` — im Endpoint-Stil sauber getrennt

`<T>` trägt zwei Rollen: **Rückgabetyp** und (optionale) **Vergleichs-Vorlage**. Der Endpoint-Stil
(§15.6) trennt sie im Vokabular:

- **Rückgabetyp** = `Produces<T>(code)` (Pflicht) → immer `Task<T>`, immer die echte Antwort.
- **Vergleich** = `ExpectedResponse…` (optional) → weglassen = body-loser Ergebnis-Pfad. **Das ersetzt
  das früher hier geplante `Reading<T>()`** — das Weglassen *ist* der body-lose Pfad.
- **Fehler mit anderem Typ** = `Produces<ProblemDetails>(400)` → eigenes `<T>`, kein Sonderterminal.
- **Collection vs. Element** (§10.4): `Produces<List<Person>>(…)` fürs Ergebnis, Selector-Overload mit
  explizitem `<Person>` fürs Element.

Offen (klein, nicht blockierend):

- [ ] **Rückgabewert vor/nach `FilterResponse`?** Diff und Rückgabe operieren auf demselben
  deserialisierten Objekt — sicherstellen, dass der zurückgegebene Wert nicht von einer
  `FilterResponse`-Normalisierung „verbogen" ist (oder bewusst roh vs. normalisiert entscheiden).

### 14.3 Leitplanke

Der Rückgabewert ist die **Kern-Rechtfertigung** der Fluent-API ggü. der Overload-Welt
(`var created = await …AssertPost…; await …AssertGet($"…/{created.Id}")`). Er ist verifiziert — die
Story trägt.

---

## 15. Endpoint-Contract als Test-Surface — zwei getrennte Assert-Ebenen

> Ausgangspunkt war ein „Super-Endpoint" mit voller Vertragsdeklaration (Minimal-API `MapPost` +
> `Accepts` + 10× `Produces` + 5× `With…`-Metadata) — der ideale Stresstest, weil er **die komplette
> Oberfläche eines Endpoints in einer Kette** zeigt. Ergebnis: der **Endpoint-Stil ist der alleinige
> Kanon** (§15.6), funktionale Kette `Accepts` → `Produces<T>(code)` → `ExpectedResponse…` →
> `ExecuteAsync()`. Das durchgespielte finale Modell steht in **§15.6**.

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

| Ebene                             | Frage                                                     | Braucht echten HTTP-Call?                     | Assert-Familie                                                          |
|-----------------------------------|-----------------------------------------------------------|-----------------------------------------------|-------------------------------------------------------------------------|
| **Funktional** (Request/Response) | „Was passiert, wenn ich den Endpoint *wirklich* aufrufe?" | **Ja**                                        | die EINE Kette `Client.AssertPost(…).Produces<T>(code)….ExecuteAsync()` |
| **Meta** (Contract/Doku)          | „Ist der Endpoint *so deklariert*, wie er soll?"          | **Nein** — liest Endpoint-Metadaten / OpenAPI | eigene Familie `Client.AssertEndpoint(…).Has…()`                        |

> **Leitsatz:** Funktional = Verhalten (Runtime, ein `Produces` je Ausgang, Terminal `ExecuteAsync()`).
> Meta = Deklaration (statisch, kein Terminal-Risiko). Ein `Produces<T>(code)` assertet *funktional*
> genau einen Ausgang UND spiegelt *meta* den dokumentierten Contract — dieselbe Zeile, zwei Blickwinkel.

### 15.2 Der Endpoint, Zeile für Zeile

| #  | Verb / Wording            | Argument                       | Status | Body-Typ                           | Ebene                   |
|----|---------------------------|--------------------------------|--------|------------------------------------|-------------------------|
| 1  | `MapPost`                 | `"users"`, `HandleAsync`       | —      | —                                  | funktional (Route)      |
| 2  | `Accepts`                 | `Application.Json`             | —      | `CreateUserRequest`                | funktional (Request)    |
| 3  | `Produces`                | —                              | `200`  | `CreateUserResponse`               | funktional (Response ✓) |
| 4  | `Produces`                | `Status400BadRequest`          | `400`  | `ValidationProblemDetailsExtended` | funktional (Response ✗) |
| 5  | `Produces`                | `Status401Unauthorized`        | `401`  | `ProblemDetails`                   | funktional (Response ✗) |
| 6  | `Produces`                | `Status403Forbidden`           | `403`  | `ProblemDetails`                   | funktional (Response ✗) |
| 7  | `Produces`                | `Status404NotFound`            | `404`  | `ProblemDetails`                   | funktional (Response ✗) |
| 8  | `Produces`                | `Status409Conflict`            | `409`  | `ProblemDetails`                   | funktional (Response ✗) |
| 9  | `Produces`                | `Status422UnprocessableEntity` | `422`  | `ValidationProblemDetailsExtended` | funktional (Response ✗) |
| 10 | `Produces`                | `Status500InternalServerError` | `500`  | `ProblemDetails`                   | funktional (Response ✗) |
| 11 | `Produces`                | `Status503ServiceUnavailable`  | `503`  | `ProblemDetails`                   | funktional (Response ✗) |
| 12 | `Produces`                | `Status504GatewayTimeout`      | `504`  | **`string`**                       | funktional (Response ✗) |
| 13 | `WithTags`                | `"Users"`                      | —      | —                                  | **meta**                |
| 14 | `WithName`                | `"createUserV1"`               | —      | —                                  | **meta**                |
| 15 | `MapToApiVersion`         | `1`                            | —      | —                                  | **meta**                |
| 16 | `WithDescriptionFromFile` | `"Description.md"`             | —      | —                                  | **meta**                |
| 17 | `WithSummaryFromFile`     | `"Summary.md"`                 | —      | —                                  | **meta**                |

### 15.3 Funktionale Assertions — jeder Ausgang wird ein `Produces<T>(code)`

Jeder Endpoint-`Produces` spiegelt 1:1 auf einen Test-`Produces<T>(code)` (je ein eigener Test, §15.6).
Der Body-Typ hängt am Code — **jeder Code hat seinen eigenen `<T>`**:

| Body-Typ                           | Codes                        | Häufigkeit |
|------------------------------------|------------------------------|------------|
| `ProblemDetails`                   | 401, 403, 404, 409, 500, 503 | 6×         |
| `ValidationProblemDetailsExtended` | 400, 422                     | 2×         |
| `CreateUserResponse`               | 200                          | 1×         |
| `string`                           | 504                          | 1×         |

**Wichtigster Fund:** `504 → string` beweist, dass `Produces<T>(code)` **generisch über den Body-Typ**
sein muss — NICHT auf `ProblemDetails` festgenagelt. Genau deshalb trägt `Produces<T>(code)` den Typ
als `<T>` (ein Übergang für Erfolg UND Fehler), statt einer `Expecting…`-Familie pro Code. Bestätigt
§13.2 und §14.2.

**Optionale benannte Aliase** (Ausbaustufe, §15.7): `.ProducesOk<T>()` / `.ProducesNotFound()` als
reine Abkürzungen auf `Produces<T>(code)` — kein neuer Engine-Pfad. Achtung: genau diese Namen sind die
bereits eingetretene Doku-Drift (§1) → erst bauen, dann dokumentieren.

### 15.4 Meta-Assertions — die Contract-Familie (`With…` ×5)

Kein HTTP-Call, liest Endpoint-Metadaten / OpenAPI. Bewusst **eine separate Kette**, damit sich
funktional und meta nicht vermischen. Einstiegs-Selektor ist oft der `WithName` statt der URL:

| Deklaration                                  | Meta-Assert (Vorschlag)                                                                                      |
|----------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| `.WithName("createUserV1")`                  | `Client.AssertEndpoint("createUserV1")` *(Selektor)* / `.HasName("createUserV1")`                            |
| `.WithTags("Users")`                         | `…HasTag("Users")`                                                                                           |
| `.MapToApiVersion(1)`                        | `…MapsToApiVersion(1)`                                                                                       |
| `.WithSummaryFromFile("Summary.md")`         | `…HasSummaryFromEmbedded("Summary.md")` *(spiegelt `…FromEmbedded`, §11)*                                    |
| `.WithDescriptionFromFile("Description.md")` | `…HasDescriptionFromEmbedded("Description.md")`                                                              |
| *(gesamter `Produces`-Block)*                | `…DeclaresStatuses(200,400,401,403,404,409,422,500,503,504)` — Contract-Snapshot der dokumentierten Ausgänge |

**Namensschema meta:** durchgängig `Has…` / `Maps…` / `Declares…` — bewusst anders als funktional,
damit Auge und Analyzer die beiden Familien sofort trennen (funktional: `Produces…` + `ExecuteAsync()`;
meta: `Has…`, kein `ExecuteAsync`). Ob die Meta-Kette ein eigenes Terminal braucht oder eager prüft,
ist offen (§15.5).

### 15.5 Offene Fragen zu §15

- [ ] **Meta-Familie eager oder terminiert?** `AssertEndpoint(…).HasTag(…)` könnte eager werfen
  (kein Terminal-Risiko, kein Analyzer nötig) oder derselben `ExecuteAsync`-Disziplin folgen.
  Tendenz: eager — Meta hat keinen Request, das Terminal-Argument aus §2 greift hier nicht.
- [ ] **Woher die Metadaten?** `EndpointDataSource` (In-Process, `WebApplicationFactory`) vs.
  generiertes OpenAPI-Dokument. Ersteres ist näher an der Deklaration, Letzteres testet das,
  was der Client wirklich sieht.
- [ ] **`Accepts` als eigene Assertion?** Content-Type-Erwartung (`.WithContentType(Json)`) ist
  funktional (Request-Seite), aber selten explizit gebraucht — als optionaler Request-Schritt halten.
- [ ] **Data-driven Contract-Katalog (Fernziel):** Der `Produces`-Block ist maschinenlesbar → ein
  Test könnte „ruf jeden deklarierten Ausgang ab, Body-Typ aus der Deklaration" fahren
  (ein `Produces<T>(code)` pro deklariertem Ausgang, generiert). Passt zum
  `[DynamicRequestLocator]`-Katalog (§13.2). Nur Fernziel.

### 15.6 Das finale funktionale Modell — durchgespielt (KANONISCH, 2026-07-16)

Vier Entscheidungen fixiert (Details in §15.7). Die eine funktionale Kette:

```csharp
var created = await Client.AssertPost("api/v1/users")
    .Accepts(user)                                     // Request-Body (echte Instanz)
    .Produces<User>(StatusCodes.Status201Created)      // Typ + Status + RÜCKGABETYP in einem
    .ExpectedResponseFromEmbeddedJson("CreateUser.json") // optionale Body-Vergleichsvorlage
    .ExecuteAsync();                                   // EINZIGES Terminal → Task<User> (echte Antwort)
```

**Line-by-line-Spiegelung Endpoint ↔ Test** (der pädagogische Kern):

| Endpoint-Deklaration               | Test-DSL                         | Rolle                                                         |
|------------------------------------|----------------------------------|---------------------------------------------------------------|
| `.Accepts<User>(Application.Json)` | `.Accepts(user)`                 | Vertrag *beschreibt* Request ↔ Test *sendet* echten Body      |
| `.Produces<User>(201)`             | `.Produces<User>(201)`           | Vertrag *deklariert* Ausgang ↔ Test *assertet diesen* Ausgang |
| `.Produces<ProblemDetails>(400)`   | `.Produces<ProblemDetails>(400)` | dito, anderer Ausgang → eigener Test                          |

**Vier Entscheidungen (durchgespielt an allen §13-Fällen):**

1. **`Produces<T>(code)` trägt Typ + Status + Rückgabetyp.** Ein `<T>`, ein Kanal →
   `HttpResponseBuilder<T>` → `Task<T>`. Ersetzt die `Expecting…`-Familie vollständig.
2. **`ExpectedResponse…` ist OPTIONAL** — weglassen = body-loser Ergebnis-Pfad (trennt „Rückgabetyp"
   von „Vergleich", §14.2; das Weglassen *ist* der body-lose Pfad):
   ```csharp
   // mit Body-Diff:  .Produces<User>(201).ExpectedResponseFromEmbeddedJson("x.json")
   // ohne (nur Typ+Status, echte Antwort zurück):  .Produces<User>(201)
   ```
   Quell-Varianten spiegeln Schema A (§11): `ExpectedResponse(obj)` / `ExpectedResponseFromJsonString` /
   `ExpectedResponseFromEmbeddedJson`.
3. **Ein `Produces` pro Kette** — jeder Call assertet GENAU einen Ausgang → `ExecuteAsync()` bleibt
   `Task<T>` eindeutig. Multi-Output-Endpoint → mehrere Tests (je einer pro `Produces`-Zeile):
   ```csharp
   // Test 1: .Accepts(valid).Produces<User>(201).ExecuteAsync()          → Task<User>
   // Test 2: .Accepts(invalid).Produces<ProblemDetails>(400).ExecuteAsync() → Task<ProblemDetails>
   ```
   Kosten: bei Multi-Output-Endpoints geht die „exakt dieselben Zeilen"-Symmetrie verloren — die
   Spiegelung bleibt als *mentales Modell* (jede `Produces`-Zeile bekommt ihren Test).
4. **`ExecuteAsync()` ist der einzige Terminal-Anker** (ersetzt den `Expecting…`-Merksatz aus §3).
   Merksatz neu: *Jede Kette endet mit genau einem `ExecuteAsync()`.* Es ist der einzige `await`-Punkt
   → für Auge UND Analyzer noch trivialer scanbar. **Konsequenz: §2/§3/§5 (Analyzer MSTESTSDK001) müssen
   umgeschrieben werden** — Marker ist „fehlendes `ExecuteAsync()`", nicht mehr „fehlendes `Expecting…`".

**Durchgespielte Fälle (alle §13-realen Muster tragen):**

| Fall                   | Kette                                                                                                                                  | Ergebnis                                                                               |
|------------------------|----------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
| Erfolg + Diff          | `.Accepts(u).Produces<User>(201).ExpectedResponseFromEmbeddedJson("x").ExecuteAsync()`                                                 | `Task<User>` ✓                                                                         |
| Erfolg ohne Diff       | `.Accepts(u).Produces<User>(201).ExecuteAsync()`                                                                                       | `Task<User>` ✓, kein Golden-File                                                       |
| Fehler (anderer Typ)   | `.Accepts(bad).Produces<ProblemDetails>(400).ExpectedResponseFromEmbeddedJson("v").ExecuteAsync()`                                     | `Task<ProblemDetails>` ✓                                                               |
| 204 NoContent          | `.Produces(204).ExecuteAsync()` (nicht-generisch, kein `<T>`)                                                                          | `Task` ✓; Typ-State: ohne `<T>` kein `ExpectedResponse…` möglich                       |
| CRUD-Lifecycle         | `create→Produces<User>(201)` → `get/{id}→Produces<User>(200)` → `delete/{id}→Produces(204)` → `get/{id}→Produces<ProblemDetails>(404)` | `created.Id` fließt durch ✓ (Kern-Rechtfertigung §9)                                   |
| GET/DELETE (kein Body) | `AssertGet(url).Produces<User>(200)…`                                                                                                  | `Accepts` ist optionaler Zwischenschritt → `Produces` auch direkt nach Entry verfügbar |

**Typ-State-Erkenntnis:** `IgnoreProperty` / `ForProperty` / `FilterResponse` / `Using` hängen am
Zustand, den `ExpectedResponse…` zurückgibt — **nicht** am `Produces<T>(code)`-Zustand. Ohne Expected
gibt es keine Vergleichs-Konfiguration → der **Compiler** (nicht erst der Analyzer) verhindert das
sinnlose „Ignore ohne Vergleich":

```csharp
.Produces<User>(201)
    .IgnoreProperty(u => u.Id)   // ❌ Compilerfehler — kein Vergleich konfigurierbar
    .ExecuteAsync();
```

Der body-lose Pfad (`Produces<T>(code).ExecuteAsync()`) hat diese Methoden gar nicht erst sichtbar.
IntelliSense führt dadurch die richtige Reihenfolge vor (§4, stärkste Verteidigungslinie).

### 15.7 Offene Unterfragen zu §15.6 (klein, nicht blockierend)

- [ ] **`Accepts`-Quell-Varianten:** `Accepts(obj)` ist ein Verb, `ExpectedResponse` ein Substantiv →
  `AcceptsFromEmbeddedJson` vs. `ExpectedResponseFromEmbeddedJson` lesen sich unterschiedlich.
  Prüfen, ob `Accepts` auch ein Substantiv-Pendant braucht (`RequestBody…`?) oder ob die
  Verb-Form trägt. Content-Type als optionaler Zusatz (`.Accepts(u, Application.Json)`)?
- [ ] **`Produces(204)` nicht-generisch vs. `Produces<T>(code)`** — zwei Overloads; Typ-State stellt
  sicher, dass `ExpectedResponse…` nur nach der generischen Variante erreichbar ist.
- [ ] **Benannte Status-Aliase?** `.ProducesOk<T>()` / `.ProducesCreated<T>()` / `.ProducesNotFound()`
  als Abkürzungen auf `Produces<T>(code)` — ABER Achtung: genau diese Namen sind die bereits
  eingetretene Doku-Drift (§1). Erst bauen, wenn der Kern (`Produces<T>(code)`) steht; nicht wieder
  dokumentieren bevor implementiert.
- [ ] **Analyzer-Umschreibung** (§2/§3/§5): Marker `ExecuteAsync()` statt `Expecting…`. `[FluentBuilder]`
  bleibt auf den Builder-Interfaces; die dangling-chain-Diagnose (MSTESTSDK001) ändert nur den
  Terminal-Namen, nicht das Prinzip.
- [ ] **Meta-Ebene (§15.4) unberührt** — `Client.AssertEndpoint(…).Has…()` bleibt wie beschrieben die
  zweite, eager Familie ohne Terminal-Risiko.

---

## 12. Entscheidungslog

> Chronologische Historie — **maßgeblich bei Zweifeln.** Abschnitts-Nummern sind entstehungs-, nicht
> lesereihenfolge (11 → 13 → 14 → 15 → 12); Querverweise gelten trotzdem. Einträge werden nicht
> nachträglich umgeschrieben, nur ergänzt.

| Datum      | Entscheidung                                                                                                                                                                                                                        | Begründung                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
|------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 2026-07-16 | HTTP zuerst, Value-Assertions später                                                                                                                                                                                                | Fokus; HTTP hat das größere Overload-/Terminal-Problem.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| 2026-07-16 | Analyzer ist Fundament, nicht optional                                                                                                                                                                                              | Ohne ihn ist Fluent für ein Test-SDK riskanter als die Overload-API.                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| 2026-07-16 | Body-Input explizit (`WithBody`/`WithJsonString`/`WithEmbeddedJson`), keine rate-Heuristik                                                                                                                                          | Fluent-API ist neu + `internal` → keine Kompatibilität nötig; Intention im Namen statt Laufzeit-Raten schließt einen still-grünen Fehlerfall.                                                                                                                                                                                                                                                                                                                                                                                             |
| 2026-07-16 | Response-Seite spiegelt die Dreiteilung: `Returns<T>(object)` / `ReturnsJsonString<T>` / `ReturnsEmbeddedJson<T>`                                                                                                                   | Symmetrie zur Body-Seite, gleiches mentales Modell; Objekt-Expected wird erst durch Matcher (§10.3) praktisch nutzbar. `<T>` bei allen dreien Pflicht (treibt Deserialisierung + Ketten-Zustand `IHttpResponseConfiguring<T>`).                                                                                                                                                                                                                                                                                                           |
| 2026-07-16 | **Namensschema final = Schema A**: `WithBody`/`WithBodyFromJsonString`/`WithBodyFromEmbeddedJson` ↔ `Returns`/`ReturnsFromJsonString`/`ReturnsFromEmbeddedJson`                                                                     | Gemeinsames scanbares Präfix (`WithBody…`/`Returns…`, wie `Expecting…`) UND Default-Fall (Objekt) bleibt kürzester Name. `…From…` nur für indirekte Quellen — das Objekt IST der Body/das Expected. Verworfen: B (voll-symmetrisch, killt `Returns<T>(obj)`), C (Suffix, Herkunft zu schwach), D (`FromFile`, verliert Embedded-Präzision). Code-Rename noch offen.                                                                                                                                                                       |
| 2026-07-16 | Rückgabewert verifiziert: `ExecuteAsync()` liefert die ECHTE deserialisierte Antwort (`context.CurrentResult`), NICHT das Expected. `<T>` an `Returns…<T>` ist EIN Faden für Vergleich UND Rückgabe.                                | Am Code nachgewiesen (`HttpAssertionPipeline.cs:45`, `AssertableHttpClient.cs:206`, `Client.Assert.HttpCall.cs:666`). Kern-Rechtfertigung der Fluent-API (§9, CRUD-Lifecycle) steht damit ohne Umbau. Kein Breaking Change.                                                                                                                                                                                                                                                                                                               |
| 2026-07-16 | Body-loser Ergebnis-Pfad `Reading<T>()` als additiver dritter Übergang                                                                                                                                                              | Schließt die einzige echte Lücke („Ergebnis ohne Golden-File-Diff") zwischen `ExpectingResponse()` (Task, kein Ergebnis) und `Returns…<T>` (Ergebnis, aber Diff-Zwang). Additiv → kein Breaking. Nutzt intern denselben `HttpResponseBuilder<T>`-Kanal + vorhandenen `IgnoreResponse`-Pfad.                                                                                                                                                                                                                                               |
| 2026-07-16 | Entry heißt `AssertPost/AssertGet/AssertPut/AssertPatch/AssertDelete` (flach), NICHT `Post(…)` und nicht `Assert.Post(…)`                                                                                                           | `Assert` sichtbar am Zeilenanfang → macht Assert-Charakter klar (sonst wirkt es wie ein bloßer POST) und ist eine Verteidigungslinie gegen die still-grüne Kette (§2). Flach statt Gateway-Property (`.A.B.C`) = direkter + matcht die alte `AssertPostAsync`-Signatur → mechanische Migration der ~1000 Tests.                                                                                                                                                                                                                           |
| 2026-07-16 | Zwei getrennte Assert-Ebenen: **funktional** (Request/Response, echter HTTP-Call, `Client.AssertPost…Expecting…().ExecuteAsync()`) vs. **meta** (Contract/Doku, kein Call, `Client.AssertEndpoint(…).Has…()`) — siehe §15           | Der „Super-Endpoint" (`Accepts` + 10× `Produces` + 5× `With…`) mischt Verhalten und Deklaration. Trennung verhindert die „zwei Stile"-Fragmentierung (§6); `Has…`/`Maps…`/`Declares…` (meta) vs. `Expecting…` (funktional) macht die Familien für Auge + Analyzer trennscharf. Fund: jeder Status-Code hat eigenen Body-Typ (`504 → string`!) → Fehler-Terminal MUSS `ExpectingError<T>` generisch sein (bestätigt §13.2/§14.2).                                                                                                          |
| 2026-07-16 | **Endpoint-Stil ist ALLEINIGER Kanon** (§15.6). Neutral-Stil `WithBody`/`Returns…`/`Expecting…` (§3/§11) VERWORFEN. Funktionale Kette: `Accepts` → `Produces<T>(code)` → `ExpectedResponse…` → `ExecuteAsync()`.                    | „Zwei Stile" (§1/§6, Risiko Nr. 1) nach Rollout in ~1000 Tests nur mit Breaking-Sweep lösbar → jetzt festnageln, solange `internal`. Endpoint-Stil spiegelt die Minimal-API-Spec line-by-line (Endpoint *beschreibt*, Test *verifiziert* denselben Vertrag) → selbsterklärender als der Mix aus `WithBody`/`ReturnsEmbeddedJson`/`ExpectingStatus`.                                                                                                                                                                                       |
| 2026-07-16 | `Produces<T>(code)` trägt Typ + Status + Rückgabetyp in EINEM; `ExpectedResponse…` optional (weglassen = body-loser Pfad, macht `Reading<T>()` überflüssig)                                                                         | Ein `<T>`-Kanal → `Task<T>`. Optionales `ExpectedResponse` trennt „Rückgabetyp" (Pflicht via `Produces<T>`) sauber von „Vergleichs-Vorlage" (optional) → löst §14.2/§14.3 im selben Vokabular. `ExpectedResponse(obj)`/`…FromJsonString`/`…FromEmbeddedJson` spiegeln Schema A (§11).                                                                                                                                                                                                                                                     |
| 2026-07-16 | Ein `Produces` pro Kette (nicht mehrere) — jeder Call assertet genau einen Ausgang                                                                                                                                                  | Hält `ExecuteAsync()` als `Task<T>` eindeutig (mehrere `Produces` → `Task<object>`/Union → Rückgabetyp-Verlust). Multi-Output-Endpoint → mehrere Tests; line-by-line-Spiegelung bleibt mentales Modell (jede `Produces`-Zeile bekommt ihren Test).                                                                                                                                                                                                                                                                                        |
| 2026-07-16 | `ExecuteAsync()` ist der einzige Terminal-Anker; Merksatz „jede Kette endet mit `ExecuteAsync()`" ersetzt „endet mit `Expecting…`"                                                                                                  | Endpoint-Stil hat keine `Expecting…`-Familie mehr. `ExecuteAsync` ist der einzige `await`-Punkt → für Auge + Analyzer noch trivialer. **Konsequenz: §2/§3/§5 (Analyzer MSTESTSDK001) auf Marker „fehlendes `ExecuteAsync()`" umschreiben** (Prinzip unverändert, nur Terminal-Name).                                                                                                                                                                                                                                                      |
| 2026-07-16 | `WithParameter`: EIN Verb, zwei Overloads — typisiert (`p => p.Id, 0`) + string (`"UniqueName", x`). String-Form nutzt NACKTE Namen, SDK escapt intern (kein `$…$` im Test-Code), toleriert aber bereits-escapte Eingaben (Postel). | Typisierte Form deckt Property-Platzhalter, String-Form die ohne Property-Pendant (`UniqueName`/`Timestamp`/URL, §13.1) → disjunkt, kein „zwei Stile". Ein Verb, weil dieselbe Operation (nur Selektor vs. Key). Nackte Namen: Delimiter-Konvention (`$…$`) wird interne Implementierungssache → änderbar ohne Test-Anfassen; `$` im Test wäre Leak der internen Repräsentation (Leitgedanke §4/§11). Bereits-escapt wird erkannt + beibehalten → Migration der ~1000 `("$Id$",…)`-Tests bleibt reiner Signatur-Sweep, kein `$`-Strippen. |
| 2026-07-16 | `WithParameters(obj)` (Objekt-Bulk, §10.1.1): jede Property → Platzhalter. Property-Name = C#-PascalCase; ALLE Properties inkl. `default`/`null`.                                                                                   | Ergonomie beim Arrange-Muster (§13.1). Einfache/vorhersehbare Varianten für die Ideensammlung — PascalCase ist im Test-Code lesbar (Caveat: matcht evtl. nicht camelCase-Platzhalter aus JSON → still-grün), „alle Properties" vermeidet „mal da, mal nicht" (Caveat: value-type `Age=0` wird gesetzt, §10.3). Beide Fallen dokumentiert; Verschachtelung + Bulk/Einzel-Kollision offen.                                                                                                                                                  |
