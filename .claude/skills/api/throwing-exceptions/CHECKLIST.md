# Throwing Exceptions Checklist

Use this checklist when reviewing or implementing exception handling.

## Exception Selection

- [ ] `ProblemDetailsException` is used instead of generic `Exception`
- [ ] The HTTP status code reflects responsibility correctly: 4xx for client/domain request issues, 5xx for server or downstream failures
- [ ] `HttpStatusCode` enum is used for type safety (not raw integers)

## Title, Detail, Extensions

- [ ] The title is constant and contains no dynamic values
- [ ] The detail explains what happened with useful dynamic values
- [ ] The extensions contain the minimum useful IDs, state, and external references
- [ ] All extension values are converted to strings

## Safety and Observability

- [ ] No secrets, credentials, or oversized payloads are logged
- [ ] Titles are consistent with existing wording in the same domain
- [ ] The extensions support dashboarding and troubleshooting without reading the full stack trace

## RFC 7807 Compliance

- [ ] Exception follows Problem Details for HTTP APIs (RFC 7807)
- [ ] Response will be properly serialized as JSON with standard fields (type, title, status, detail, extensions)

## Quick Mapping

- [ ] Invalid input → `HttpStatusCode.BadRequest` (400)
- [ ] Missing auth → `HttpStatusCode.Unauthorized` (401)
- [ ] Permission denied → `HttpStatusCode.Forbidden` (403)
- [ ] Missing entity → `HttpStatusCode.NotFound` (404)
- [ ] Duplicate/conflict → `HttpStatusCode.Conflict` (409)
- [ ] Business rule violation → `HttpStatusCode.UnprocessableEntity` (422)
- [ ] Unexpected internal failure → `HttpStatusCode.InternalServerError` (500)
- [ ] Downstream failure → `HttpStatusCode.BadGateway` (502)
- [ ] Temporary unavailability → `HttpStatusCode.ServiceUnavailable` (503)
- [ ] Downstream timeout → `HttpStatusCode.GatewayTimeout` (504)
