# If/Else Early Exit Checklist

- [ ] Deep nesting has been reduced where possible
- [ ] The happy path stays at low indentation
- [ ] `else` is removed after `return`, `throw`, `continue`, or `break`
- [ ] Conditions are inverted when that makes the control flow clearer
- [ ] Validation, stop conditions, and main processing are clearly separated
- [ ] Loop filtering uses `continue` when appropriate
- [ ] Behavior is unchanged after the refactoring
- [ ] A helper method is extracted if one method still has too many branches
