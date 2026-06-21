# Guía de Estudio Definitiva: Examen ASP.NET + SQL Server (LINQ to SQL)

Esta guía resume los temas clave de tu proyecto en su versión clásica con SQL Server, explicados de la forma más sencilla posible para que puedas escribirlos a mano o completarlos en un examen.

---

## 1. El famoso "DataContext" (Conexión a SQL Server)

Cuando trabajas con LINQ to SQL, Visual Studio genera un archivo `.dbml` que automáticamente crea una clase llamada `DataClasses1DataContext` (o similar). 

**¿Qué es el DataContext?**
Es el puente o túnel entre tu código C# y tu base de datos SQL Server. Imagina que es una "caja" que tiene cargadas todas tus tablas listas para ser usadas como listas de objetos en C#. Al instanciarlo (`new DataClasses1DataContext()`), él abre la conexión a SQL automáticamente por ti usando la cadena de conexión del `Web.config`.

**Lo que te pueden pedir escribir:**
```csharp
// 1. Instanciamos el DataContext (Abre el túnel a SQL)
DataClasses1DataContext db = new DataClasses1DataContext();

// 2. Accedemos a la tabla (Por ejemplo, tbl_producto)
var listaProductos = db.tbl_productos.ToList();
```

---

## 2. Expresiones Lambda (`=>`) y LINQ

Una expresión lambda es una función anónima pequeña. Se lee como "tal que" o "va hacia". 
Por ejemplo: `p => p.pro_estado == 'A'` se lee *"Un producto 'p', tal que su estado sea igual a la letra 'A'"*.

**Métodos clásicos de LINQ que debes saberte de memoria:**
*   `.ToList()`: Convierte el resultado en una lista.
*   `.FirstOrDefault()`: Devuelve la primera fila encontrada, o `null` si no existe.
*   `.Where()`: Filtra la tabla y te devuelve todos los que cumplan la condición.
*   `.Any()`: Devuelve un booleano (`true` o `false`) si encuentra al menos un elemento.

**Ejemplo en papel:**
```csharp
DataClasses1DataContext db = new DataClasses1DataContext();

// Buscar un producto por su ID (Select * from producto where id = 123)
var producto = db.tbl_productos.FirstOrDefault(p => p.pro_id == 123);

// Obtener solo los usuarios activos
List<tbl_usuario> lista = db.tbl_usuarios.Where(u => u.usu_estado == 'A').ToList();
```

---

## 3. El CRUD Completo con SQL (Capa de Negocio)

### Crear (Insert)
Para insertar, agregas el objeto a la tabla virtual del DataContext y le dices a la base de datos que guarde los cambios (`SubmitChanges`).
```csharp
public static void CrearCategoria(tbl_categoria nuevaCat)
{
    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        nuevaCat.cat_estado = 'A'; 
        
        // Lo añades a la tabla
        db.tbl_categorias.InsertOnSubmit(nuevaCat);
        
        // Disparas el comando a SQL Server
        db.SubmitChanges();
    }
}
```

### Leer (Select)
```csharp
public static List<tbl_categoria> ListarCategorias()
{
    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        // En SQL puro sería: SELECT * FROM tbl_categoria WHERE cat_estado = 'A'
        return db.tbl_categorias.Where(c => c.cat_estado == 'A').ToList();
    }
}
```

### Actualizar (Update)
Para actualizar, primero buscas el objeto viejo en la BD, modificas sus propiedades y guardas los cambios.
```csharp
public static void ActualizarCategoria(tbl_categoria catEditada)
{
    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        // 1. Buscamos el original en la base de datos
        var catOriginal = db.tbl_categorias.FirstOrDefault(c => c.cat_id == catEditada.cat_id);
        
        if (catOriginal != null)
        {
            // 2. Modificamos los campos
            catOriginal.cat_nombre = catEditada.cat_nombre;
            // (No modificamos el estado ni el ID)
            
            // 3. Guardamos los cambios
            db.SubmitChanges();
        }
    }
}
```

### Eliminar (Delete - Borrado Lógico)
En sistemas reales casi nunca hacemos un `DeleteOnSubmit()`. Hacemos un borrado lógico cambiando el estado a 'I' (Inactivo).
```csharp
public static void EliminarLogico(int id)
{
    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        var cat = db.tbl_categorias.FirstOrDefault(c => c.cat_id == id);
        if(cat != null)
        {
            // Borrado lógico
            cat.cat_estado = 'I';
            db.SubmitChanges();
        }
    }
}
```

### Eliminar (Delete - Borrado Físico)
Si en el examen te piden eliminar un registro **para siempre** de la base de datos (borrado físico), debes usar el método `DeleteOnSubmit`. ¡Ten cuidado con las llaves foráneas! Si intentas borrar una categoría que ya tiene productos asociados, SQL Server arrojará un error de integridad referencial.
```csharp
public static void EliminarFisico(int id)
{
    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        // 1. Buscamos el registro que queremos borrar
        var cat = db.tbl_categorias.FirstOrDefault(c => c.cat_id == id);
        
        if(cat != null)
        {
            // 2. Marcamos el objeto para ser eliminado
            db.tbl_categorias.DeleteOnSubmit(cat);
            
            // 3. Impactamos los cambios en SQL Server (¡Adiós para siempre!)
            db.SubmitChanges();
        }
    }
}
```

---

## 4. FileUpload: Guardar Imágenes (Path vs Varbinary) y Previsualización

Existen dos formas de guardar una imagen:
1. **Ruta (Path):** Guardas el archivo en el disco del servidor (`/Uploads/`) y en SQL Server solo guardas un texto tipo `VARCHAR` con la ruta.
2. **Base de Datos (Varbinary):** Conviertes la imagen en un arreglo de bytes (`byte[]`) y la metes directo a una columna `VARBINARY(MAX)` en SQL Server.

### A) Previsualización en el Cliente (JavaScript + SweetAlert)
Antes de darle a "Guardar", es clave validar mediante JavaScript que el usuario subió una imagen y mostrarla en pantalla.
```html
<!-- El input dispara la función previsualizarImagen cada que cambia -->
<asp:FileUpload ID="fuImagen" runat="server" onchange="previsualizarImagen(this);" />

<!-- Etiqueta img vacía que se mostrará cuando se cargue la imagen -->
<img id="imgPreview" src="#" alt="Previsualización" style="display:none; width:200px; margin-top:10px;" />

<script>
function previsualizarImagen(input) {
    if (input.files && input.files[0]) {
        var archivo = input.files[0];
        
        // 1. Validación estricta con SweetAlert: ¿Es realmente una imagen?
        if (!archivo.type.match('image.*')) {
            Swal.fire('Error', 'Por favor selecciona solo imágenes (JPG, PNG).', 'error');
            input.value = ''; // Vaciamos el fileupload para bloquearlo
            document.getElementById('imgPreview').style.display = 'none';
            return;
        }

        // 2. Si pasó la validación, la dibujamos en pantalla usando FileReader
        var reader = new FileReader();
        reader.onload = function (e) {
            var img = document.getElementById('imgPreview');
            img.src = e.target.result;
            img.style.display = 'block'; // Mostramos la etiqueta img oculta
        };
        reader.readAsDataURL(archivo);
    }
}
</script>
```

### B) Método 1: Guardar como Ruta Físicamente (Path)
El código C# para guardar el archivo en el disco y su ruta en la BD.
```csharp
if (fuImagen.HasFile)
{
    string nombreArchivo = fuImagen.FileName;
    
    // Convertimos la ruta virtual ("~/Uploads/") en la ruta de tu disco duro
    string rutaFisica = Server.MapPath("~/Uploads/Productos/") + nombreArchivo;
    
    // Guardamos el archivo físicamente en el servidor
    fuImagen.SaveAs(rutaFisica);

    // Guardamos la ruta virtual en la base de datos (Columna VARCHAR)
    string rutaParaBD = "~/Uploads/Productos/" + nombreArchivo;
    
    // producto.pro_ruta_imagen = rutaParaBD;
    // db.tbl_productos.InsertOnSubmit(producto);
}
```

### C) Método 2: Guardar como VARBINARY en SQL Server (`byte[]`)
Si te piden guardar la foto cruda dentro de la base de datos sin usar carpetas.
```csharp
if (fuImagen.HasFile)
{
    // 1. Obtenemos cuánto pesa la imagen (tamaño en bytes)
    int tamanoBytes = fuImagen.PostedFile.ContentLength;
    
    // 2. Creamos un arreglo vacío de bytes exacto para ese tamaño
    byte[] imagenBytes = new byte[tamanoBytes];
    
    // 3. Leemos todo el archivo y rellenamos nuestro arreglo de bytes
    fuImagen.PostedFile.InputStream.Read(imagenBytes, 0, tamanoBytes);
    
    // 4. Guardamos ese arreglo directamente en la columna VARBINARY
    // producto.pro_foto_varbinary = imagenBytes;
    // db.tbl_productos.InsertOnSubmit(producto);
}
```

---

## 5. GridView (Tablas de Datos)

El GridView sirve para dibujar tablas HTML llenas de datos sin escribir ciclos `for` en el HTML.

**Cómo llenarlo desde C#:**
```csharp
// Obtienes tu lista de la base de datos
List<tbl_producto> lista = CN_tbl_producto.ListarProductos();

// Se lo asignas al GridView y obligas a que se dibuje (DataBind)
gvProductos.DataSource = lista;
gvProductos.DataBind();
```

**El evento más preguntado: `RowCommand`**
Se dispara cuando das clic a un botón dentro de la tabla (por ejemplo, el botón "Eliminar").
```csharp
protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
{
    // e.CommandName te dice QUÉ botón presionaron
    if (e.CommandName == "Eliminar")
    {
        // e.CommandArgument trae el ID del producto que presionaste (int en SQL)
        int id = Convert.ToInt32(e.CommandArgument);
        CN_tbl_producto.EliminarLogico(id);
        
        // Refrescamos la tabla para que desaparezca visualmente
        CargarTabla();
    }
}
```

---

## 6. SweetAlert (Popups de JS desde C#)

SweetAlert es puro JavaScript. Como ASP.NET se procesa en el servidor, usamos `ScriptManager` para "inyectar" un script de JavaScript hacia el navegador del cliente.

**Ejemplo de código para examen:**
```csharp
string titulo = "Éxito";
string mensaje = "El registro se guardó correctamente.";
string tipo = "success"; // Puede ser success, error, warning

// Construimos el script en texto
string script = $"Swal.fire('{titulo}', '{mensaje}', '{tipo}');";

// Se lo mandamos al navegador
ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", script, true);
```

---

## 7. Login, Roles y Sesiones (Session)

El Login es la parte de seguridad web más importante. Cuando un usuario navega entre páginas, el servidor "se olvida" de quién es. Para solucionarlo, usamos las **Variables de Sesión (`Session`)**, que guardan los datos del usuario en la memoria del servidor.

**El botón de Login (Buscar al usuario y guardar su sesión):**
```csharp
protected void btnLogin_Click(object sender, EventArgs e)
{
    string cedula = txtCedula.Text.Trim();
    string clave = txtClave.Text.Trim(); 

    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        // 1. Buscamos al usuario que tenga esa cédula, esa clave y esté activo
        var usuario = db.tbl_usuarios.FirstOrDefault(u => u.usu_cedula == cedula && u.usu_contraseña == clave && u.usu_estado == 'A');

        if (usuario != null)
        {
            // 2. ¡Login exitoso! Guardamos todo el objeto usuario en la Sesión
            Session["usuario"] = usuario;

            // 3. Redirigimos a la página principal (Dashboard)
            Response.Redirect("~/Default.aspx", false);
        }
        else
        {
            // 4. Usuario no encontrado o clave incorrecta
            string script = "Swal.fire('Error', 'Credenciales incorrectas', 'error');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", script, true);
        }
    }
}
```

**Validar en las demás páginas (Ej. `Site.Master.cs` o en cualquier `Page_Load`):**
En las demás páginas debes verificar si el usuario entró legalmente por el Login, o si intentó saltarse la seguridad escribiendo la URL directamente.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // 1. Verificamos si la variable de sesión existe (si no es null)
    if (Session["usuario"] != null)
    {
        // 2. Transformamos el objeto guardado a su tipo original (Casteo)
        tbl_usuario usuarioLogueado = (tbl_usuario)Session["usuario"];
        
        // 3. Ya podemos usar sus datos para mostrar su nombre
        lblNombreMenu.Text = "Bienvenido, " + usuarioLogueado.usu_nombres;
        
        // 4. Validación de Roles (Ej. 1 = Admin, 2 = Usuario Normal)
        if (usuarioLogueado.tusu_id == 1) 
        {
            // Es admin, le mostramos el panel de Mantenimiento
            panelAdmin.Visible = true;
        }
        else
        {
            // Es usuario normal, se lo ocultamos
            panelAdmin.Visible = false;
        }
    }
    else
    {
        // Si la sesión es null, el usuario no está logueado y quiso entrar copiando la URL.
        // Lo mandamos expulsado a la pantalla de Login.
        Response.Redirect("~/Seguridad/Login.aspx");
    }
}
```

---

## 8. Registro de Usuarios (Validación e Insert)

El Registro de usuarios es parecido a crear una Categoría (Insert), pero con un paso vital antes: **validar que el usuario no exista previamente** en la base de datos (por cédula o correo) para evitar duplicados.

**El botón de Registrar (`btnRegistrar_Click`):**
```csharp
protected void btnRegistrar_Click(object sender, EventArgs e)
{
    string cedula = txtCedula.Text.Trim();
    string nombres = txtNombres.Text.Trim();
    string clave = txtClave.Text.Trim(); 

    using (DataClasses1DataContext db = new DataClasses1DataContext())
    {
        // 1. VALIDACIÓN: ¿Ya existe alguien con esta cédula?
        // Any() nos devuelve TRUE si encuentra al menos 1 registro coincidente.
        bool existeUsuario = db.tbl_usuarios.Any(u => u.usu_cedula == cedula);

        if (existeUsuario)
        {
            // Detenemos el proceso y lanzamos alerta
            string scriptError = "Swal.fire('Error', 'Ya existe un usuario con esta cédula.', 'warning');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", scriptError, true);
            
            return; // Sale del método inmediatamente para no ejecutar el Insert
        }

        // 2. CREACIÓN: Instanciamos el objeto de la tabla
        tbl_usuario nuevoUsuario = new tbl_usuario();
        nuevoUsuario.usu_cedula = cedula;
        nuevoUsuario.usu_nombres = nombres;
        nuevoUsuario.usu_contraseña = clave;
        
        // Asignamos datos automáticos por debajo de la mesa
        nuevoUsuario.tusu_id = 2; // Le forzamos el ID de Rol 2 (Usuario Normal)
        nuevoUsuario.usu_estado = 'A'; // Activo por defecto

        // 3. GUARDADO: Lo mandamos al DataContext
        db.tbl_usuarios.InsertOnSubmit(nuevoUsuario);
        db.SubmitChanges(); // ¡Impacta en SQL Server!

        // 4. ÉXITO: Mensaje de confirmación y limpiamos cajas de texto
        string scriptExito = "Swal.fire('¡Registrado!', 'Tu cuenta ha sido creada con éxito.', 'success');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", scriptExito, true);
        
        txtCedula.Text = "";
        txtNombres.Text = "";
        txtClave.Text = "";
    }
}
```

---

## 9. Arquitectura N-Capas: La Capa de Presentación

Tu proyecto usa una Arquitectura de 3 Capas (Datos, Negocio y Presentación). Para el examen, debes saber que la **Capa de Presentación** es la "cara" del sistema (los archivos `.aspx` y `.aspx.cs`). 

**Regla de oro de la Arquitectura:** La Capa de Presentación **NUNCA** guarda cosas directamente en SQL Server. Su único trabajo es recolectar lo que el usuario escribió en la pantalla, armar un "paquete" y pasárselo a la Capa de Negocio para que haga el trabajo sucio.

**El código importante que debes dominar (Ejemplo de un Botón "Guardar"):**
```csharp
protected void btnGuardar_Click(object sender, EventArgs e)
{
    // 1. RECOLECCIÓN: Capturamos lo que el usuario escribió en las cajas de texto (HTML)
    string nombreIngresado = txtNombre.Text.Trim();
    string descripcionIngresada = txtDescripcion.Text.Trim();

    // 2. EMPAQUETADO: Armamos el objeto que vamos a mandar
    tbl_categoria nuevaCat = new tbl_categoria();
    nuevaCat.cat_nombre = nombreIngresado;
    nuevaCat.cat_descripcion = descripcionIngresada;
    
    // 3. DELEGACIÓN: Le pasamos la pelota a la Capa de Negocio.
    // Fíjate que aquí NO usamos DataContext ni SubmitChanges porque somos la Capa de Presentación. 
    // Solo llamamos al método estático de la Capa de Negocio.
    CN_tbl_categoria.CrearCategoria(nuevaCat);

    // 4. RESPUESTA AL USUARIO: Mostramos el SweetAlert y limpiamos la pantalla para el siguiente registro
    string script = "Swal.fire('Guardado', 'Categoría registrada exitosamente.', 'success');";
    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", script, true);
    
    txtNombre.Text = "";
    txtDescripcion.Text = "";
}
```
