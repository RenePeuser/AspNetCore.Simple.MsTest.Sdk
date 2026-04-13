# Avoid Sync-over-Async Checklist

- [ ] No task is consumed with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` without a documented reason
- [ ] Async methods are propagated upward where practical
- [ ] Blocking is avoided in request, UI, background, and thread-pool code
- [ ] `await` is used as the default task consumption style
- [ ] Any exception to the rule is clearly justified
