function validar_Cliente() {
    var clave_Unica = document.getElementById("txtClaveUnica").value;
    var nombre = document.getElementById("txtNombre").value;
    var paterno = document.getElementById("txtAPaterno").value;
    var materno = document.getElementById("txtAMaterno").value;
    var sexo = document.getElementById("ddlSexo");
    var ddlSexo = sexo.options[sexo.selectedIndex].value;
    var calendar = document.getElementById("txtHiden");
    var codigoPostal = document.getElementById("txtCodigoPostal");
    var rfc = document.getElementById("txtRFC");
    var fecha = calendar.defaultValue;
    var bandera = true;
    var mensaje = "";
     if (clave_Unica < 99 || clave_Unica > 1000) {
        mensaje = "Clave Unica Solo De Tres Digitos";
        bandera = false;
     }else if (isNaN(clave_Unica) == true) {
        mensaje="Clave Unica Solo Numeros";
        bandera = false;
    }
    if (!(/^([aA-záéíóúZÁÉÍÓÚ]{1}[a-zñáéíóú]+[\s]*)+$/.test(nombre))) {
        mensaje = 'Ingresa Un Nombre maximo de 15 caracteres ';
        bandera = false;
    } else if (!(/^([aA-záéíóúZÁÉÍÓÚ]{1}[a-zñáéíóú]+[\s]*)+$/.test(paterno))) {
        mensaje = 'Ingresa Un Apellido Paterno maximo 15 caracteres ';
        bandera = false;
    } else if (!(/^([aA-záéíóúZÁÉÍÓÚ]{1}[a-zñáéíóú]+[\s]*)+$/ /.test(materno))) {
        mensaje = 'Ingresa Un Apellido Materno maximo 15 caracteres ';
        bandera = false;
    } else if (ddlSexo < 0 || ddlSexo > 2) {
        mensaje = 'Ingresa Un Sexo';
        bandera = false;

    } else if (calendar == null || calendar == undefined || calentar == "") {
        mensaje = 'Ingrese Una fecha';
        bandera = false;

    } else if (!(/^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$/.test(rfc))) {
        mensaje = 'Ingrese Un RFC Valido';
        bandera = false;

    } else if (isNaN(codigoPostal) == true) {
        mensaje = "Codigo Postal Solo Numeros";
        bandera = false;
    } else if (codigoPostal < 9999 || codigoPostal >99999) {
        mensaje = "Codigo Postal De 5 Digitos";
        bandera = false;
    }


    if (bandera == false) {
        alert(mensaje);
    }
   
   
   
    return bandera;

}