#**PROYECTO DE BANCO ASP.NET Core MVC**

Este repositorio contiene un proyecto de un banco desarrollado utilizando Visual Studio y ASP.NET Core MVC como framework principal, junto con una base de datos SQL gestionada a través de migraciones. El proyecto está diseñado para ofrecer funcionalidades básicas de un sistema bancario, incluyendo la gestión de cuentas, transferencias y otras operaciones comunes de un banco.

##**Características**

- **Gestión de Cuentas:** Permite a los usuarios crear, ver, editar y eliminar cuentas bancarias.
  
- **Transferencias:** Los usuarios pueden realizar transferencias de fondos entre cuentas bancarias, con validación de saldo y detalles de la transacción.

- **Historial de Transacciones:** Registro completo de todas las transacciones realizadas, incluyendo depósitos, retiros y transferencias.

##**Configuración del Proyecto**

1. **Configurar la Base de Datos:**
   
   - Abre el archivo `appsettings.json`.
   - Actualiza la cadena de conexión en la sección `ConnectionStrings` con los detalles de tu base de datos SQL.

2. **Aplicar Migraciones:**
   
   - Abre la consola del Administrador de paquetes en Visual Studio.
   - Ejecuta el comando `Update-Database` para aplicar las migraciones y crear la estructura de la base de datos.

3. **Compilar y Ejecutar:**
   
   - Compila la solución en Visual Studio.
   - Ejecuta la aplicación y prueba las funcionalidades del sistema bancario.
