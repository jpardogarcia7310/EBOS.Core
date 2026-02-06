# EBOS.Core

EBOS.Core es una biblioteca de clases .NET que ofrece utilidades compartidas para la solución EBOS, incluyendo ayuda criptográfica, validadores, abstracciones de envío de correo y primitivas comunes usadas en los servicios.

## Resumen de Arquitectura

- **Crypto**: utilidades de Base64, cifrado/descifrado AES, hashing SHA3 y tokens HMAC.
- **Validators**: validación de cuenta bancaria española (IBAN/BBAN), email e identificaciones españolas (DNI/NIE/CIF).
- **Mail**: DTO de mensajes y configuración, adaptadores/fabricas SMTP y el servicio SendMail.
- **Primitives**: entidades base, resultados de error y contenedores de resultado de operación.
- **Extensions**: extensiones para fechas, strings, colecciones y resultados de operación.
- **Caching**: punto de acceso común a `MemoryCache`.

## Resumen de Tests

- **Framework**: xUnit (`tests/EBOS.Core.Test`).
- **Areas cubiertas**:
  - Round-trips y manejo de errores en criptografía.
  - Validadores con entradas válidas/inválidas y casos límite.
  - Flujo de correo (creación de mensajes, adjuntos, SMTP, mapeo de errores).
  - Extensiones, primitivas y contratos de interfaces.

## Como ejecutar los tests

```bash
cd C:\Proyectos\SITEC\EBOS\back\EBOS.Core
dotnet test
```

## Resumen de Code Review

- El análisis se centró en corrección, seguridad de excepciones, validación de entradas y cobertura de tests.
- El código está modularizado con límites claros entre utilidades, validadores y correo.
- Los tests cubren un conjunto amplio de comportamientos y casos borde.
