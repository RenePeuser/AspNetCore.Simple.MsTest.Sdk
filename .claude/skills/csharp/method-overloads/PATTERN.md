# Method Overloads

## Objective

Keep overload chains predictable, readable, and safe.

Overloads must delegate in one clear direction until all calls end in one maximum overload that contains the central logic.

## Apply These Rules

- Design overloads from minimal input to maximal input.
- Each smaller overload must forward to the next richer overload.
- Keep the real implementation in exactly one maximum overload.
- Do not duplicate core logic across multiple overloads.
- Do not let multiple overloads independently build the same core execution flow.
- Keep the call order explicit and easy to follow.
- Add convenience overloads only when they clearly improve usability.
- Prefer overload chains over copying argument preparation logic into many methods.
- Keep defaults and forwarded metadata consistent across the full chain.

## Preferred Overload Flow

Use a strict call hierarchy like this:

- minimum overload
- richer overload
- richer overload
- maximum overload

The maximum overload is the only place where the central logic should exist.

## Rules for the Maximum Overload

The maximum overload should:

- contain the central orchestration
- create the final request or context object
- perform the actual execution
- be the only overload that owns the full logic

All smaller overloads should only:

- provide defaults
- enrich arguments
- normalize inputs
- forward to the next overload

## Review Checklist

- Is there one clear overload chain from small to large?
- Does each smaller overload forward to a richer overload?
- Is there exactly one maximum overload with the real logic?
- Is request or context creation centralized in the maximum overload?
- Are defaults forwarded consistently?
- Would changing the core logic require editing only one overload?
- Is the overload structure easy to understand?

## Notes

The preferred repository pattern is:

- small overloads for convenience
- one clear forwarding chain
- one maximum overload
- one place for the actual logic

This keeps overloads maintainable and prevents subtle divergence between similar method variants.
