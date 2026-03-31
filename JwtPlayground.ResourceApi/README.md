# JwtPlayground.ResourceApi

This project protects a sample API endpoint with JWT bearer authentication.

## What it does

- Downloads the auth server public key during startup.
- Configures JWT bearer validation for issuer, audience, lifetime, and signature.
- Protects `/secret` with `[Authorize]`.

## Notes

- The API currently fetches the public key from `/publickey` instead of using the JWKS endpoint.
- Certificate validation is bypassed for local development in the startup HTTP client. That is convenient for a playground, but it should not be kept in production code.
- When the auth server restarts, previously issued tokens stop validating because the signing key changes.
