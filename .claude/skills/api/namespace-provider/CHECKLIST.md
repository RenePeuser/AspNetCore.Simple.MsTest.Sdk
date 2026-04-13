# Namespace Provider Checklist

- [ ] Namespaces stay focused on domain and version
- [ ] Technical folders such as `Commands`, `Queries`, `Endpoints`, `Requests`, and `Responses` are hidden from namespaces where appropriate
- [ ] Production and test projects follow the same namespace convention
- [ ] Namespace provider settings are stored in the correct `*.csproj.DotSettings` file
- [ ] `*.sln.DotSettings` is not used for project-specific namespace provider rules
- [ ] Namespace changes reduce noise instead of creating churn
