# Argument Check Checklist

- [ ] Guard clauses are placed at meaningful boundaries
- [ ] The most specific `Throw.*` helper is used
- [ ] Checks improve safety or clarity instead of adding noise
- [ ] Domain or business validation is not mixed into guard clauses
- [ ] Already-guaranteed values are not re-validated without reason
