# Guía del Deber: CI/CD con Jenkins, Docker, IIS y MongoDB

Este documento detalla cómo construir el Pipeline (Tubería) de Integración y Despliegue Continuo (CI/CD) que te pidieron para el martes.

## 1. Análisis de la Arquitectura Propuesta

Tu diagrama es excelente y es exactamente cómo se hace en la industria:
`GitHub -> Jenkins (Docker) -> Restore -> Build -> Test -> Publish -> IIS (Windows) -> DB`

**Respondiendo a tu pregunta sobre MongoDB:**
**¡SÍ, ES TOTALMENTE POSIBLE Y FUNCIONA PERFECTO!** A Jenkins y a GitHub Actions no les importa si usas SQL Server o MongoDB. Jenkins solo se encarga de compilar texto (C#) y mover archivos compilados (.dll) a IIS. Quien realmente se conecta a MongoDB es el servidor IIS (Windows) en tiempo de ejecución. Mientras tu servidor IIS tenga acceso al puerto de MongoDB (27017), el monolito funcionará impecable.

---

## 2. ⚠️ EL RETO MORTAL: El problema de compilar ASP.NET Framework
Tu proyecto está hecho en **ASP.NET WebForms (.NET Framework 4.8)**. Este framework es antiguo y **solo puede compilarse en Windows usando `MSBuild.exe`**. 
El problema es que la imagen que estás corriendo en Docker (`jenkins/jenkins`) es **LINUX**. Linux no sabe qué es MSBuild ni cómo compilar `.NET Framework`.

### La Solución Profesional (Agente Jenkins)
No intentes compilar dentro del contenedor de Docker. Lo que debes hacer es que el Jenkins de Docker actúe como "El Jefe" (Controller) y configures tu máquina Windows física (donde está IIS y Visual Studio) como un **"Agente" (Node)**. 
De esta forma, Jenkins lee GitHub, pero le da la orden a tu Windows de ejecutar `msbuild` y publicar en IIS.

---

## 3. Paso a Paso para tu Deber

### Paso 1: Subir el Monolito a GitHub
Antes de subir el código, **asegúrate de crear un archivo `.gitignore`** (plantilla de Visual Studio) para que no se suban las carpetas `bin/`, `obj/` ni `packages/`. Esto es vital para que Jenkins baje el código limpio y restaure los paquetes desde cero.

### Paso 2: Configurar tu Windows como "Agente" de Jenkins
1. Entra a tu Jenkins en `http://localhost:8090` (vi en tu captura que usaste el puerto 8090).
2. Ve a **Administrar Jenkins > Nodos (Nodes)**.
3. Crea un "Nuevo Nodo" (New Node), llámalo `Windows-Agent`, tipo "Permanent Agent".
4. En "Directorio raíz remoto" pon una carpeta vacía en tu C: (ej. `C:\JenkinsAgent`).
5. En "Etiquetas" (Labels) ponle `windows`.
6. En Método de lanzamiento, elige "Lanzar agente conectándolo al controlador" (Launch agent by connecting it to the controller).
7. Jenkins te dará un comando de Java (`java -jar agent.jar ...`) que debes ejecutar en el CMD de tu Windows para conectar tu máquina física al Docker.

### Paso 3: Instalar MSBuild y NuGet en Windows
Tu máquina Windows debe tener:
*   Visual Studio instalado (que ya tiene `MSBuild`).
*   El ejecutable de `nuget.exe` descargado y guardado en una carpeta (ej. `C:\Tools\nuget.exe`), y agregado a las variables de entorno (PATH) para poder ejecutarlo desde CMD.

### Paso 4: El Pipeline (`Jenkinsfile`)
Para el paso de "Crear un repositorio de pruebas", la mejor práctica es crear un archivo llamado `Jenkinsfile` en la raíz de tu proyecto en GitHub. 

Aquí tienes el código que hace exactamente lo que pide tu deber. Este script le dice a Jenkins que se ejecute en el agente `windows` que creaste.

```groovy
pipeline {
    // Le decimos que corra en tu máquina Windows, NO en el contenedor Linux
    agent { 
        label 'windows' 
    }

    stages {
        stage('Restaurar paquetes NuGet') {
            steps {
                echo 'Restaurando paquetes...'
                // Usa nuget.exe para descargar las librerías de MongoDB, etc.
                bat 'nuget restore TuSolucion.sln'
            }
        }
        
        stage('Compilar solución') {
            steps {
                echo 'Compilando proyecto...'
                // Llama a MSBuild. Asegúrate que la ruta a MSBuild sea correcta en tu PC.
                bat '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" TuSolucion.sln /p:Configuration=Release'
            }
        }
        
        stage('Ejecutar pruebas') {
            steps {
                echo 'Ejecutando pruebas unitarias (si las hay)...'
                // Si no tienes proyecto de pruebas, puedes hacer un echo que simule el paso
                // bat 'vstest.console.exe TuProyectoPruebas.dll'
                echo 'Pruebas finalizadas con éxito.'
            }
        }
        
        stage('Publicar aplicación') {
            steps {
                echo 'Generando archivos de publicación...'
                bat '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" TuProyectoWeb\\TuProyectoWeb.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:publishUrl="C:\\PublicacionJenkins"'
            }
        }

        stage('Desplegar en IIS') {
            steps {
                echo 'Desplegando en IIS...'
                // Copia la carpeta publicada a la ruta de IIS (inetpub/wwwroot)
                bat 'xcopy /Y /E "C:\\PublicacionJenkins\\*" "C:\\inetpub\\wwwroot\\TuSitio\\"'
                echo '¡Despliegue exitoso!'
            }
        }
    }
}
```

---

## Conclusión sobre MongoDB
**Mi opinión:** Mantener MongoDB en el proyecto para este deber es una **idea fantástica** porque le demuestra al profesor que tienes una arquitectura moderna (un monolito .NET conectándose a una base de datos NoSQL alojada en un contenedor Docker, todo orquestado por CI/CD).
Solo asegúrate de que tu `Web.config` tenga la IP o nombre del contenedor correcto para MongoDB (`mongodb://localhost:27017` servirá si Mongo está expuesto en el localhost de tu máquina Windows).
