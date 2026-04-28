# Copilot Instructions

## Project Guidelines
- For this repo's xUnit v3 migration work, run the AddDescriptionFromDisposeHtmlFromTest-focused validation after each edit and only push when pass count is at least 50/65. Treat 44/65 as the message-only ceiling; proceed with explicit v3 runtime scope API, update only runtime-mutation tests, and document the v3 runtime boundary instead of relying on patching.
- Ensure cleanup focuses on a homogeneous project coding style, including removing leftover diagnostics/log messages and normalizing formatting (tabs vs spaces).