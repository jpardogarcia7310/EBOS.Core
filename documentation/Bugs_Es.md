# Bugs y Riesgos

## Alta

- **Formato de email sin validar puede lanzar excepcion al crear el mensaje**: `SendMail` solo comprueba presencia de remitentes/destinatarios. `MailboxAddress` puede lanzar si el formato es invalido y la excepcion no se traduce a un error de dominio.
  - Referencia: `sources/EBOS.Core/Mail/SendMail.cs:106`

## Media

- **No se validan los parametros SMTP antes de conectar**: `Server` y `Port` no se validan; se pasan tal cual a `ConnectAsync`, lo que puede fallar con excepciones de bajo nivel y diagnostico mas dificil.
  - Referencia: `sources/EBOS.Core/Mail/SendMail.cs:157`

- **No se valida longitud/codificacion de la clave AES**: la clave se codifica en ASCII sin comprobar longitud. Caracteres no ASCII se truncan y longitudes invalidas pueden provocar `CryptographicException` en tiempo de ejecucion.
  - Referencia: `sources/EBOS.Core/Crypto/CryptoEngine.cs:73`
