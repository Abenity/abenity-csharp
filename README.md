<<<<<<< HEAD
# Abenity C&#35;

This library communicates with the Abenity API to make it easier to integrate with Abenity, specifically in C&#35;.

## Nuget Feed

The Nuget package is available here: <a href="https://www.nuget.org/packages/abenity-csharp/">https://www.nuget.org/packages/abenity-csharp/</a>

## Projects

In this repository there are two projects:

1. **Abenity.Api**: This is the source code for the Nuget package. It handles the security aspects of communicating data to Abenity's API.
2. **Abenity.TestClient**: This is a sample project to show how to use (1).

## Requirements

Before making calls you will need the following information. The Abenity API package will require each piece of information.

1. API credentials. Specifically: **username**, **password** and **key**. You get these values from Abenity.
2. A public and private key. _You_ need to generate these, and send your public key to Abenity to associate with your account.

## Generating keys

Both keys should be in the PKCS1 format. A sample public.pem might look like this:

```
-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEA0+5ulw0FHaD3yCw8zPhav2lVdasl5waVSyl6r6QFX0RzmtGwvpJ1
...
MKvJ+YJuE/9hifO3R8XslKWYW2cOTg0QPQIDAQAB
-----END RSA PUBLIC KEY-----
```

A sample private.pem might look like this:

```
-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA0+5ulw0FHaD3yCw8zPhav2lVdasl5waVSyl6r6QFX0RzmtGw
...
vPn0wjZ7mgn3qGbqSCoPonh7iJH15Rw7CLJgPntRvsk0oqNxj1/N
-----END RSA PRIVATE KEY-----
```

**This library only supports PKCS1 formats. Please ensure your key files match the format above!**
=======
# abenity-csharp
>>>>>>> 1de48ee38da936bd83e919a32a8e12c01588651f
