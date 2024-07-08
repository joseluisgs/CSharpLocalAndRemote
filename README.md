# C# Local and Remote

![CSharp](./images/csharp.webp)



## Acerca de

Este proyecto es un ejemplo de como crear un servicio para el almacenamiento de datos C#.

Tendremos un repositorio local usando una base de datos, y un repositorio remoto utilizando una API REST.

Ambos repositorios estarán encapsulados en un solo servicio, que será el que utilicemos en nuestra aplicación.
Es decir, cada cierto tiempo se actualizará la base de datos local con los datos de la API REST. Otro caso de uso es que
no haya conexión, o simplemente
queramos trabajar con los datos locales o si no se encuentran actualizados, con los datos de la API REST.
Además, otra caché en memoria se encargará de almacenar los últimos datos obtenidos de la base de datos local o de la
API REST para mejorar el rendimiento de la aplicación. Es decir, si la base de datos local no está actualizada, se
obtendrán los datos de la API REST cada cierto intervalo de refresco y se almacenarán en la base de datos local. En la
caché
en memoria se almacenarán los últimos datos obtenidos de la base de datos local o de la API REST.

El intervalo de refresco de la base de datos local y el de la caché en memoria se puede configurar.

Por otro lado, el servicio podrá importar/exportar datos en CSV y JSON.

Finalmente, tendremos un servicio de notificaciones, que nos permitirá recibir información de los cambios realizados.

El objetivo docente es mostrar implementaciones asíncronas y reactiva en el procesamiento de la información.

Puedes seguir el proyecto en [GitHub](https://github.com/joseluisgs/CSharpLocalAndRemote) y en los commits indicados.

```
El proyecto está sobre-exagerado para mostrar diferentes técnicas y no es un ejemplo de cómo hacer una aplicación real.
Se trata de elementos aislados que se pueden utilizar en una aplicación real con el objetivo que el alumnado identifique
y asimile nuevas técnicas y herramientas de programación. No tiene que ser la mejor forma ni la más eficiente, pero sí
intenta acercar otras formas de programar nuevas que se están viendo en clase. El objetivo es aprender y no hacer una
aplicación real y eficiente. 
```

La idea subyacente es el famoso patrón de diseño Repository usado en Android, pero llevado a un nivel superior.

![Repository Pattern](./images/pattern.webp)

## Test

A la hora de realizar los tests, hemos usado [NUnit](https://nunit.org/), pero antes de nada asegurate de seguir los pasos en:
[Getting Started with NUnit](https://www.jetbrains.com/help/rider/Getting_Started_with_Unit_Testing.html#step-1-add-unit-test-project)
y configura tu proyecto para que use NUnit.
Para ello en tu directorio de usuario crea un archivo `nuget.config` con el siguiente contenido:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    </packageSources>
</configuration>
```

O desde Rider, ve a NuGet y configura el origen de los paquetes.
![NuGet](./images/nuget.png)

Ahora ya puedes instalar NUnit en tu proyecto.

```bash
dotnet add package NUnit
```

Lo mejor es es crear un proyecto de pruebas unitarias en tu solución y añadir los tests allí. Automáticamente tendrás 
las referencias necesarias.

```xml
<ItemGroup>
        <PackageReference Include="coverlet.collector" Version="6.0.0"/>
        <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0"/>
        <PackageReference Include="NUnit" Version="3.14.0"/>
        <PackageReference Include="NUnit.Analyzers" Version="3.9.0"/>
        <PackageReference Include="NUnit3TestAdapter" Version="4.5.0"/>
    </ItemGroup>
```
