# EBOS.Core

EBOS.Core is a .NET class library that provides shared utilities for the EBOS solution, including cryptography helpers, validation utilities, mail sending abstractions, and common primitives used across services.

## Architecture Overview

- **Crypto**: Base64 helpers, AES encryption/decryption, SHA3 hashing, and HMAC token utilities.
- **Validators**: Spanish bank account (IBAN/BBAN), email address, and Spanish identification (DNI/NIE/CIF) validators.
- **Mail**: DTOs for mail messages/settings, SMTP abstraction adapters/factories, and the SendMail service.
- **Primitives**: Base entities, error results, and operation result containers.
- **Extensions**: Convenience extensions for dates, strings, collections, and operation results.
- **Caching**: A shared `MemoryCache` access point.

## Tests Overview

- **Framework**: xUnit (`tests/EBOS.Core.Test`).
- **Coverage Areas**:
  - Crypto round-trips and error handling.
  - Validators for valid/invalid inputs and edge cases.
  - Mail pipeline (message building, attachments, SMTP flow, error mapping).
  - Extensions, primitives, and repository interface shapes.

## How to Run Tests

```bash
cd C:\Proyectos\SITEC\EBOS\back\EBOS.Core
dotnet test
```

## Code Review Summary

- Review focused on correctness, exception safety, input validation, and unit test coverage.
- The codebase is modular with clear boundaries between utilities, validators, and mail abstractions.
- Unit tests cover a broad set of behaviors and edge cases across the library.
