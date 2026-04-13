# Loops Use Continue Checklist

- [ ] Nested conditions inside loops have been reduced where possible
- [ ] Skip conditions are handled near the top of the loop body
- [ ] `else` is removed after `continue`
- [ ] The actual loop work is easy to spot
- [ ] `foreach` and `for` loops use `continue` when that improves readability
- [ ] Complex skip logic is extracted if the loop remains branch-heavy
- [ ] Behavior is unchanged after the refactoring
- [ ] The loop is smaller, flatter, and easier to scan
