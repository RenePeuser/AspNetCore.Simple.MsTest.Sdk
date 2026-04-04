# Constructor Overloads

## Objective

Keep constructor chains predictable, readable, and safe.

Constructors must delegate in one clear direction until all calls end in one maximum constructor that performs the actual state initialization.

Follow the Highlander principle:

There must be exactly one constructor that initializes the object state.

## Apply These Rules

- Design constructors from minimal input to maximal input.
- Each smaller constructor must delegate to a richer constructor.
- Keep the real state initialization in exactly one maximum constructor.
- Do not duplicate initialization logic across multiple constructors.
- Do not let multiple constructors assign the same fields independently.
- Keep the constructor call order explicit and easy to follow.
- Add convenience constructors only when they clearly improve usability.
- Prefer constructor chaining over copying initialization logic into many constructors.
- Keep defaults and forwarded arguments consistent across the full chain.

## Preferred Constructor Flow

Use a strict constructor hierarchy like this:

- minimum constructor
- richer constructor
- richer constructor
- maximum constructor

The maximum constructor is the only place where the object state should be initialized.

## Rules for the Maximum Constructor

The maximum constructor should:

- contain the full state initialization
- assign fields or properties
- validate constructor arguments when needed
- be the only constructor that owns the initialization logic

All smaller constructors should only:

- provide defaults
- enrich arguments
- normalize inputs
- delegate to the next constructor

## Avoid

Duplicate initialization across constructors where each independently assigns all fields.

## Review Checklist

- Is there one clear constructor chain from small to large?
- Does each smaller constructor delegate to a richer constructor?
- Is there exactly one constructor that initializes the state?
- Is state assignment centralized in the maximum constructor?
- Are defaults forwarded consistently?
- Would changing initialization logic require editing only one constructor?
- Is the constructor structure easy to understand?

## Notes

The preferred repository pattern is:

- small constructors for convenience
- one clear chaining flow
- one maximum constructor
- one place for state initialization

This keeps object construction maintainable and prevents subtle divergence between constructor variants.
