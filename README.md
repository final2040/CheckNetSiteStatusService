# Network Monitor

Servicio de Windows que permite monitorear el estado de una o varias conexiones de red. Y notificar vía correo electrónico cuando se pierde o restablece una conexión.

### Prerrequisitos

- Windows Vista SP2 o superior
- .Net Framework 4.5.2 o superior


### Instalación

Solo descargue el instalador más actual de la sección [Releases] (https://github.com/final2040/CheckNetSiteStatusService/releases)

### Configuración
Network Monitor utiliza un archivo XML para almacenar las configuraciones del servicio de Windows a continuación se describen las secciones de configuración que componen el archivo.

#### Configuración de la prueba

Configura las opciones de prueba para la conexión, esta sección se compone de dos miembros:

- **WaitTimeSecons**.- Especifica el tiempo que se debe esperar entre cada prueba de la conexión.
- **TimeoutSecons**.- Especifica el tiempo máximo que se requiere antes de enviar el correo electrónico.
```
 <TestConfig>
    <WaitTimeSeconds>1</WaitTimeSeconds>
    <TimeOutSeconds>120</TimeOutSeconds>
  </TestConfig>
```

#### Configuración de las pruebas de red

Configura las redes a probar, el servicio web permite configurar hasta 250 monitores de red, se aceptan dos tipos de monitores:

- **TCP**.- Prueba que un puerto en una conexión esté disponible, este tipo de prueba requiere los siguientes atributos: **Host**: El host a probar, **Port**: El puerto que se desea probar, **TimeOutMilliSeconds**: Tiempo de espera antes de considerar la conexión como fallida.
- **IP**: Prueba la conectividad del host mediante ping, acepta los siguientes atributos: **Host**: el Host a probar.
```
 <Tests>
    <TCP Host="216.58.194.46" Port="80" TimeOutMilliSeconds="500">GOOGLE.COM</TCP>
    <IP Host="98.139.183.24">YAHOO.COM</IP>
  </Tests>
```
#### Configuración del correo electrónico

Configura el cliente de correo electrónico de la aplicación, la configuración se compone de los siguientes miembros:

- **SendFrom**.- La dirección de correo electrónico que aparecerá en el apartado **"DE:"** del mensaje.
- **Recipients**.- Contiene una lista de los correos electrónicos a los que será enviado el mensaje.
- **Subject**.- El asunto del mensaje, este campo admite *Tags* de las que se hablara más adelante.
- **Body**.- El contenido del mensaje admite *Tags*.
- **IsHtml**.- Indica si el correo será enviado en formato HTML (no testeado)
- **SmtpCredentials**.- Especifica las credenciales que se utilizarán para enviar el correo electrónico, este miembro consta de dos atributos forzosos, **UserName**: El nombre de usuario para iniciar sesión, **Password**: La contraseña a utilizar para iniciar sesión, esta contraseña deberá de estar encriptada para poder encriptar una nueva contraseña por favor vea la documentación de la herramienta [generatepassword](GeneratePassword.md) incluida en esta aplicación.
- **SmtpConfiguration**.- Especifica la configuración del servidor SMTP que será utilizado, consta de tres atributos: **Host**: Dirección del servidor smtp, **Port**: Puerto a utilizar para la conexión, **UseSsl**: Boleano que indica si el servidor smtp utiliza SSL, acepta true y false.
```
  <MailConfiguration>
    <SendFrom>noreply@someserver.com</SendFrom>
    <Recipients>
      <Email>recipient@anotherserver.com</Email>	  
    </Recipients>
    <Subject>Se ha {status} la conexión con {hostname}</Subject>
    <Body>
        El equipo {computername} ha {status} la conexión con el equipo {hostname} durante mas de {timeout} segundos.

        Configuración de la de la prueba: {testconfig}

        Favor de verificar el estado de la conexión.

        {testresults}

        Mensaje enviado de forma automatica por {appname} version {appversion}
    </Body>
    <IsHtml>false</IsHtml>
    <SmtpCredentials UserName="some@someserver.com" Password="JhYr3FQLn1q1lKLEzmYhbeC9y4IaxcbDSn9+XzB6tGk=" />
    <SmtpConfiguration Host="smtp.someserver.com" Port="587" UseSsl="true" />
  </MailConfiguration>
```

#### Tags

La aplicación permite introducir información dinámica en el cuerpo del correo electrónico, para esto se utilizan los siguientes Tags.

- **{sendfrom}**.- Nombre del correo desde donde se envía el mensaje, equivale al valor de sendfrom de la configuración.
- **{status}**.- Estado de la conexión "Restablecido", "Perdido".
- **{timeout}**.- Tiempo máximo que se requiere antes de enviar el correo electrónico.
- **{hostname}**.- Nombre del Host al que se le realiza la prueba.
- **{computername}**.- Nombre del equipo que envía la alerta.
- **{testconfig}**.- Configuración de la prueba.
- **{testresults}**.- Resultados de la prueba.
- **{appname}**.- Nombre de la aplicación.
- **{appversion}**.- Versión de la aplicación.
- **{date}**.- Fecha en la que se generó el correo.
- **{host}**.- Dirección o nombre del host que se está probando.

### Ejemplo del archivo de configuración.
```
<?xml version="1.0"?>
<Configuration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TestConfig>
    <WaitTimeSeconds>1</WaitTimeSeconds>
    <TimeOutSeconds>120</TimeOutSeconds>
  </TestConfig>

  <Tests>
    <TCP Host="173.194.219.138" Port="80" TimeOutMilliSeconds="500">GOOGLE TEST</TCP>
    <IP Host="206.190.36.45">YAHOO.COM</IP>
  </Tests>
  
  <MailConfiguration>
    <SendFrom>noreply@someserver.com</SendFrom>
    <Recipients>
      <Email>yourmail@someserver.com</Email>
    </Recipients>
    <Subject>Se ha {status} la conexion con {hostname}</Subject>
    <Body>El equipo {computername} ha {status} la conexión con el equipo {hostname} con ip {host} durante mas de {timeout} segundos.
    
Configuración de la de la prueba: {testconfig}
      
Favor de verificar el estado de la conexión.
      
{testresults}
    </Body>
    <IsHtml>false</IsHtml>
    <SmtpCredentials UserName="some@someserver.com" Password="JhYr3FQLn1q1lKLEzmYhbeC9y4IaxcbDSn9+XzB6tGk=" />
    <SmtpConfiguration Host="smtp.someserver.com" Port="587" UseSsl="true" />
  </MailConfiguration>
</Configuration>
```

## Nota a desarrolladores

Para poder probar la aplicación es necesario crear el archivo *config.xml* en los proyectos **ApplicationTest** y **UnitTests**, utilice el archivo *config.example.xml* como guía.

## Autor

René Emmanuel Zamorano Flores

