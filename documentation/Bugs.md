# Bugs and Risks

## High

- **Unvalidated email address format can throw at message creation**: `SendMail` only checks for presence of From/To entries. `MailboxAddress` can throw if the address format is invalid, and the exception is not converted to a domain error.
  - Reference: `sources/EBOS.Core/Mail/SendMail.cs:106`

## Medium

- **SMTP settings are not validated before connection**: `Server` and `Port` are not validated; invalid values are passed to `ConnectAsync`, which can fail with lower-level exceptions that are harder to diagnose.
  - Reference: `sources/EBOS.Core/Mail/SendMail.cs:157`

- **AES key length and encoding are not validated**: the key is encoded as ASCII without length checks. Non-ASCII characters are truncated and invalid lengths can raise `CryptographicException` at runtime.
  - Reference: `sources/EBOS.Core/Crypto/CryptoEngine.cs:73`
