# Refactoring Step 2: ObjectsAreEqual Context-based Implementation

## Übersicht
Dieser Schritt vervollständigt das Context-Pattern für die `ObjectsAreEqual` Methoden in `Assert.That.ObjectAreEqual.cs`.

## Was wurde geändert

### 1. Master-Methode konvertiert zu Wrapper

Die Master-Methode mit allen Parametern (vormals Zeile 895-1056) wurde umgewandelt in einen einfachen Wrapper:

#### Vorher:
```csharp
public static void ObjectsAreEqual<T>(this Assert assert,
                                      string expectedObjectAsJson,
                                      T currentObject,
                                      Func<T, T> orderFunc,
                                      string title,
                                      Assembly callingAssembly,
                                      Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                      string curl,
                                      (string Key, object? Value)[] parameters,
                                      bool writeResponse = false,
                                      [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                      string expectedResultParameterName = "",
                                      [CallerArgumentExpression(nameof(currentObject))]
                                      string currentResultParameterName = "",
                                      [CallerFilePath] string callerFilePath = "")
{
    // ... 150+ Zeilen Implementierung ...
}
```

#### Nachher:
```csharp
public static void ObjectsAreEqual<T>(this Assert assert,
                                      string expectedObjectAsJson,
                                      T currentObject,
                                      Func<T, T> orderFunc,
                                      string title,
                                      Assembly callingAssembly,
                                      Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                      string curl,
                                      (string Key, object? Value)[] parameters,
                                      bool writeResponse = false,
                                      [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                      string expectedResultParameterName = "",
                                      [CallerArgumentExpression(nameof(currentObject))]
                                      string currentResultParameterName = "",
                                      [CallerFilePath] string callerFilePath = "")
{
    // Convert parameters to context and call context-based implementation
    var context = new ObjectAssertContext<T>
                  {
                      OrderFunc = orderFunc,
                      Title = title,
                      CallingAssembly = callingAssembly,
                      DifferenceFunc = differenceFunc,
                      Curl = curl,
                      Parameters = parameters,
                      WriteResponse = writeResponse,
                      ExpectedResultParameterName = expectedResultParameterName,
                      CurrentResultParameterName = currentResultParameterName,
                      CallerFilePath = callerFilePath
                  };

    ObjectsAreEqualInternal(assert, expectedObjectAsJson, currentObject, context);
}
```

### 2. Neue private Context-basierte Implementierung

Eine neue private Methode `ObjectsAreEqualInternal` enthält die komplette Implementierungslogik:

```csharp
private static void ObjectsAreEqualInternal<T>(Assert assert,
                                               string expectedObjectAsJson,
                                               T currentObject,
                                               ObjectAssertContext<T> context)
{
    // Alle Context-Properties extrahieren mit Defaults
    var expectedResultParameterName = context.ExpectedResultParameterName ?? nameof(expectedObjectAsJson);
    var currentResultParameterName = context.CurrentResultParameterName ?? nameof(currentObject);
    var callerFilePath = context.CallerFilePath ?? string.Empty;
    var callingAssembly = context.CallingAssembly ?? Assembly.GetCallingAssembly();
    var orderFunc = context.OrderFunc ?? (item => item);
    var differenceFunc = context.DifferenceFunc ?? (difference => difference);
    var title = context.Title ?? string.Empty;
    var curl = context.Curl ?? string.Empty;
    var parameters = context.Parameters;
    var writeResponse = context.WriteResponse;

    // ... komplette Implementierung (150+ Zeilen) ...
}
```

### 3. Öffentliche Context-API vereinfacht

Die öffentliche Context-Überladung wurde drastisch vereinfacht:

#### Vorher:
```csharp
public static void ObjectsAreEqual<T>(this Assert assert,
                                      string expectedObjectAsJson,
                                      T currentObject,
                                      ObjectAssertContext<T> context)
{
    assert.ObjectsAreEqual(expectedObjectAsJson,
                           currentObject,
                           context.OrderFunc ?? (item => item),
                           context.Title ?? string.Empty,
                           context.CallingAssembly ?? Assembly.GetCallingAssembly(),
                           context.DifferenceFunc ?? (difference => difference),
                           context.Curl ?? string.Empty,
                           context.Parameters,
                           context.WriteResponse,
                           context.ExpectedResultParameterName ?? nameof(expectedObjectAsJson),
                           context.CurrentResultParameterName ?? nameof(currentObject),
                           context.CallerFilePath ?? string.Empty);
}
```

#### Nachher:
```csharp
public static void ObjectsAreEqual<T>(this Assert assert,
                                      string expectedObjectAsJson,
                                      T currentObject,
                                      ObjectAssertContext<T> context)
{
    ObjectsAreEqualInternal(assert, expectedObjectAsJson, currentObject, context);
}
```

## Aufrufhierarchie

Alle ~30 Überladungen der `ObjectsAreEqual` Methode folgen jetzt diesem Muster:

```
Überladung 1 ──┐
Überladung 2 ──┤
Überladung 3 ──┤
...            ├──> Master-Methode (Parameter → Context) ──> ObjectsAreEqualInternal (Implementierung)
Überladung 28 ─┤
Überladung 29 ─┤
Überladung 30 ─┘
                │
Context-API ────┘
```

## Vorteile

### ✅ Konsistenz mit HttpCall Pattern
- Folgt dem gleichen Muster wie `AssertHttpCallAsync<TResult>`
- Einheitliche Architektur im gesamten Projekt

### ✅ Single Point of Implementation
- Nur eine Methode enthält die Implementierungslogik
- Einfacher zu warten und zu erweitern

### ✅ Vereinfachte Context-API
- Die öffentliche Context-API ist jetzt nur 3 Zeilen lang
- Kein Auseinanderziehen des Context-Objekts mehr

### ✅ 100% Backward Compatible
- Alle bestehenden Aufrufe funktionieren unverändert
- Keine Breaking Changes
- Alle Tests bestehen (0 Warnings, 0 Errors)

### ✅ Vorbereitung für Phase 3
- Ermöglicht einfaches Refactoring von Helper-Methoden
- Context kann jetzt direkt an Helper übergeben werden

## Code-Qualität

### Vorher:
- Master-Methode: 12 Parameter, 150+ Zeilen
- Context-API: 12 Zeilen Code zum Auseinanderziehen des Context

### Nachher:
- Master-Methode: 12 Parameter, 12 Zeilen (Wrapper)
- Context-API: 3 Zeilen (direkter Aufruf)
- Private Implementierung: 1 Context-Parameter, 150+ Zeilen

## Status

✅ **Komplett abgeschlossen**
- Build erfolgreich (0 Warnings, 0 Errors)
- Alle Methoden zeigen auf Context-basierte Implementierung
- Code ist sauber und wartbar

## Nächste Schritte (Phase 3)

1. **EmbeddedFileLocalizer refactoren**
   - Neue Überladungen, die `ObjectAssertContext<T>` akzeptieren
   - Reduziert Parameter-Passing weiter

2. **OutputFormatter refactoren**
   - `OutputContext` Objekt erstellen
   - Statt mehreren Überladungen mit vielen Parametern

3. **WriteResponseService refactoren**
   - Context-basierte Methode hinzufügen

## Vergleich: Vorher vs. Nachher

### Aufrufe innerhalb der Codebasis

#### Vorher:
```csharp
assert.ObjectsAreEqual(expectedObjectAsJson,
                       currentObject,
                       orderFunc,
                       title,
                       callingAssembly,
                       differenceFunc,
                       curl,
                       parameters,
                       writeResponse,
                       expectedResultParameterName,
                       currentResultParameterName,
                       callerFilePath);
```

#### Nachher (intern):
```csharp
var context = new ObjectAssertContext<T> { ... };
ObjectsAreEqualInternal(assert, expectedObjectAsJson, currentObject, context);
```

#### Nachher (öffentliche API):
```csharp
var context = new ObjectAssertContext<User>
{
    FilterFunc = user => user with { Id = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
};

assert.ObjectsAreEqual("expected-user.json", currentUser, context);
```

## Zusammenfassung

Dieser Refactoring-Schritt bringt `ObjectsAreEqual` auf das gleiche Niveau wie `AssertHttpCallAsync`:
- ✅ Konsistentes Context-Pattern
- ✅ Single Point of Implementation
- ✅ Einfach zu erweitern
- ✅ 100% Backward Compatible
- ✅ Sauberer, wartbarer Code

Die Basis für Phase 3 (Helper-Methoden Refactoring) ist jetzt gelegt! 🎉
