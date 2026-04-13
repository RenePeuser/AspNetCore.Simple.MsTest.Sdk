# Request Validation Checklist

- [ ] The request DTO lives in `YourApi.Contracts`
- [ ] Baseline validation attributes are present on request properties
- [ ] Property-level rules are expressed with attributes where possible
- [ ] The chosen validator base type matches the validation needs
- [ ] Async validation is used for repository or service lookups
- [ ] Validator registration is explicit and colocated with the validator file
- [ ] `IAttributeValidator` is available in the DI graph when attribute-based validation is used
- [ ] Custom request validators keep attribute validation active instead of bypassing it
- [ ] Validation runs before mapping and main command or handler logic
- [ ] Custom validation attributes are reused before inventing imperative-only validation
- [ ] Sample values remain valid for the applied request validation rules
