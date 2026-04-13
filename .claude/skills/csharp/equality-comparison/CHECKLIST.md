# Equality Comparison Checklist

- [ ] Value equality uses `EqualsTo` or `NotEqualsTo`
- [ ] Identity checks use `ReferenceEquals` only when identity is the intent
- [ ] `==` and `!=` are not used as the default repository style for value equality
- [ ] The equality style is consistent within the surrounding code
- [ ] The chosen comparison makes intent explicit
