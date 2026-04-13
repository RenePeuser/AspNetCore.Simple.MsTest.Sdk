# Method Overloads Checklist

- [ ] Overloads form one clear forwarding chain from smaller to richer signatures
- [ ] Exactly one maximum overload contains the central logic
- [ ] Smaller overloads only provide defaults, normalization, or forwarding
- [ ] Defaults and forwarded metadata remain consistent across overloads
- [ ] Changing the core behavior would require editing only the maximum overload
