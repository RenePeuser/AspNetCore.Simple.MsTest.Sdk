# Constructor Overloads Checklist

- [ ] There is one clear constructor chain from smaller to richer overloads
- [ ] Smaller constructors delegate instead of duplicating initialization
- [ ] Exactly one maximum constructor owns the real state initialization
- [ ] Defaults and forwarded arguments are consistent across the chain
- [ ] Changing initialization logic would require editing only one constructor
