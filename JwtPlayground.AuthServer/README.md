# JwtPlayground.AuthServer

This project issues RSA-signed JWT access tokens for the playground.

## What it does

- Accepts login requests at `/token`.
- Generates a short response payload containing `access_token` and `token_type`.
- Exposes the current public key at `/publickey`.
- Exposes a JWKS document at `/.well-known/jwks.json`.

## Notes

- The RSA key is created in memory on startup, so restarting the auth server invalidates previously issued tokens.
- The current login endpoint only validates that a username is present; it does not perform real credential verification.
- `TokenService` contains the central token-generation logic and is the main unit-test seam.
