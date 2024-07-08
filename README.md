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

## Programación asíncrona y reactiva

La
programación [asíncrona](https://sunscrapers.com/blog/programming-async-vs-sync-best-approach/)
es un modelo de programación que permite realizar tareas en segundo plano sin bloquear el hilo
principal de
la aplicación.
La [reactividad](https://www.baeldung.com/cs/reactive-programming#:~:text=Reactive%20programming%20is%20a%20declarative,or%20reactive%20systems%20in%20general.)
es un modelo de programación que permite reaccionar a eventos de forma rápida y eficiente.
La programación reactiva es un paradigma de programación declarativa que se basa en la idea del procesamiento asíncrono
de eventos y flujos de datos.

En C#, la programación asíncrona y reactiva se puede realizar
con [Async/Await](https://learn.microsoft.com/es-es/dotnet/csharp/asynchronous-programming/)
y [IAsyncEnumerable y Linq](https://learn.microsoft.com/es-es/shows/on-net/supporting-iasyncenumerable-with-linq) y [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0)
Con Async/Await se pueden realizar tareas en segundo plano de forma sencilla y eficiente. Con IAsyncEnumerable y Linq se
pueden realizar operaciones asíncronas de forma sencilla y eficiente.

## Railway Oriented Programming

El [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) (ROP) es un estilo de programación que se basa
en el uso de funciones que devuelven un
resultado. De esta manera, se pueden encadenar operaciones de forma sencilla y eficiente. En C#, el ROP se puede
realizar con la clase Result. La clase Result es una clase que representa un resultado exitoso o un resultado fallido.
De
esta manera, se pueden realizar operaciones de forma sencilla y eficiente.

Para ello debemos instalar las extensiones funcionales de C#.

```bash
dotnet add package CSharpFunctionalExtensions
````

Para ello debemos entender que es el Happy Path y el Error Path. El Happy Path es el camino feliz, es decir, el camino
que se espera que se siga. El Error Path es el camino de error, es decir, el camino que se sigue cuando se produce un
error.

De esta manera podemos encadenar operaciones de forma sencilla y eficiente. Si una operación falla, se sigue el Error
Path. Si una operación tiene éxito, se sigue el Happy Path.

## Almacenamiento y Serialización

El primer paso es crear un servicio de almacenamiento y serialización de datos para realizar las operaciones de lectura
y escritura de datos. Para ello, crearemos una interfaz `StorageService` que definirá las operaciones de lectura y
escritura.

Luego, crearemos una implementación de esta interfaz para almacenar los datos en formatos CSV y JSON.
Para facilitar la serialización y deserialización de los datos, utilizaremos la librería de
Kotlin [Newtonsoft.Json (también conocida como Json.NET)](https://www.newtonsoft.com/json).

Haremos uso de mapeadores para convertir los datos de un formato a otro. Para ello haremos uso de las [funciones de
extensión de C#](https://learn.microsoft.com/es-es/dotnet/csharp/programming-guide/classes-and-structs/extension-methods). Estas funciones nos permiten añadir nuevas
funcionalidades a las clases sin modificarlas. En este
caso añadiremos
funciones de extensión para convertir los datos de un formato a otro.

En todo momento trabajaremos la asincronía con [Task]https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0) y trabajaremos ROP con
[Result](https://github.com/vkhorikov/CSharpFunctionalExtensions).

Enlace a
los [commit de la sección](https://github.com/joseluisgs/KotlinLocalAndRemote/tree/f4cf8d903a9ecb80f36822d870144ea2b8defd57).


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
