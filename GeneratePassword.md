# GeneratePassword

Utilería que permite encriptar un password para ser utilizado en la applicación

### Prerrequisitos

- Windows Vista SP2 o superior
- .Net Framework 4.5.2 o superior

### Uso

```
GENERATEPASSWORD /password:password [/key:[encryptkey]] /?

Descripción:
        Esta herramienta encripta una contraseña para ser
        utilizada por el archivo de configuración del
        servicio de windows NetMonitor.

Lista de parámetros:
        /password:contraseña    La contraseña que se encriptara este parámetro es obligatorio
        /key:llave      Llave utilizada para encriptar la contraseña
        /?      Mostrar ayuda


Ejemplos:
        GENERATEPASSWORD /password:mypassword
        GENERATEPASSWORD /password:mypassword /key:llaveencriptación
```

### Nota
Para generar una contraseña que sea compatible con la aplicación es necesario no utilizar ninguna llave predeterminada.

## Author

René Emmanuel Zamorano Flores
