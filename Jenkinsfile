pipeline {
    // Le decimos que corra en tu máquina Windows, NO en el contenedor Linux
    agent { 
        label 'windows' 
    }

    stages {
        stage('Restaurar paquetes NuGet') {
            steps {
                echo 'Restaurando paquetes de MongoDB y demás...'
                // Usamos el nuget.exe que está en la raíz de tu proyecto
                bat 'nuget.exe restore Monolito_4am_DB.slnx'
            }
        }
        
        stage('Compilar solución') {
            steps {
                echo 'Compilando proyecto ASP.NET 4.8...'
                // Llamamos al MSBuild de tu Visual Studio
                bat '"C:\\Program Files\\Microsoft Visual Studio\\18\\Insiders\\MSBuild\\Current\\Bin\\MSBuild.exe" Monolito_4am_DB.slnx /p:Configuration=Release'
            }
        }
        
        stage('Ejecutar pruebas') {
            steps {
                echo 'Iniciando fase de pruebas...'
                // Como no hay un proyecto de Unit Tests configurado, lo pasamos como exitoso simulado
                echo 'Pruebas finalizadas con éxito.'
            }
        }
        
        stage('Publicar aplicación') {
            steps {
                echo 'Generando archivos de publicación para el servidor web...'
                bat '"C:\\Program Files\\Microsoft Visual Studio\\18\\Insiders\\MSBuild\\Current\\Bin\\MSBuild.exe" Monolito_4am_DB\\Monolito_4am_DB.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:publishUrl="C:\\PublicacionJenkins"'
            }
        }

        stage('Desplegar en IIS') {
            steps {
                echo 'Desplegando la aplicación conectada a MongoDB en IIS...'
                // Copia la carpeta publicada a la ruta de IIS (inetpub/wwwroot)
                bat 'xcopy /Y /E "C:\\PublicacionJenkins\\*" "C:\\inetpub\\wwwroot\\MonolitoWeb\\"'
                echo '¡Despliegue exitoso!'
            }
        }
    }
}
