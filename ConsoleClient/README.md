# ConsoleClient

This project demonstrates the happy-path client flow for the JWT playground.

## What it does

- Sends a login request to the auth server.
- Reads the returned bearer token.
- Applies the token to the `Authorization` header.
- Calls the protected `/secret` endpoint on the resource API.

## Notes

- The token is only kept in memory for the lifetime of the process; it is not saved to disk or any secure store.
- HTTPS certificate validation is bypassed for local development.
- The client currently uses hard-coded local URLs and sample credentials to keep the playground simple.
- Test running sequence as below to avoid runtime error of endpoint not available 
    1. AuthServer
    2. ResourceApi
    3. ConsoleClient