# Overriding Virtual Methods Checklist

- [ ] The override calls `base` by default
- [ ] Any missing `base` call is intentional and documented
- [ ] Important base initialization, validation, or hooks are not skipped accidentally
- [ ] Framework or SDK overrides are reviewed with extra care
- [ ] The override clearly extends or intentionally replaces base behavior
