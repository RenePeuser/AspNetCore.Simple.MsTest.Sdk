# Immutable Data and Services Checklist

- [ ] Pure data types use `record` where appropriate
- [ ] Behavior-rich types remain `class` services or handlers
- [ ] Data types are immutable by default
- [ ] Collections are immutable when the data should be stable or read-only
- [ ] Service logic and orchestration are not mixed into data carriers
- [ ] The chosen type shape makes the data-vs-behavior boundary clear
