// Code goes here
var keySize = 256;
var ivSize = 128;
var iterations = 100;

function encrypt(msg, pass) {
    var salt = CryptoJS.lib.WordArray.random(128 / 8);

    //var key = CryptoJS.PBKDF2(pass, salt, {
    //    keySize: keySize / 32,
    //    iterations: iterations
    //});

    //var iv = CryptoJS.lib.WordArray.random(128 / 8);
    var key = CryptoJS.enc.Utf8.parse('8080808080808080');
    var iv = CryptoJS.enc.Utf8.parse('8080808080808080');

    var encrypted = CryptoJS.AES.encrypt(msg, key, {
        iv: iv,
        padding: CryptoJS.pad.Pkcs7,
        mode: CryptoJS.mode.CBC

    });

    // salt, iv will be hex 32 in length
    // append them to the ciphertext for use  in decryption
    // var transitmessage = salt.toString() + iv.toString() + encrypted.toString();
    var transitmessage = encrypted.toString();
    return transitmessage;
}

function decrypt(transitmessage, pass) {
    var salt = CryptoJS.enc.Hex.parse(transitmessage.substr(0, 32));
    var iv = CryptoJS.enc.Hex.parse(transitmessage.substr(32, 32))
    var encrypted = transitmessage.substring(64);

    var key = CryptoJS.PBKDF2(pass, salt, {
        keySize: keySize / 32,
        iterations: iterations
    });

    var decrypted = CryptoJS.AES.decrypt(encrypted, key, {
        iv: iv,
        padding: CryptoJS.pad.Pkcs7,
        mode: CryptoJS.mode.CBC

    })
    return decrypted;
}

function IrAutherize() {
    mostrarCargando();
    var password = "abc123";
    var text = $("#claveCuenta").val();
    var encryptedpassword = encrypt(text);
    $.ajax({
        url: "/Home/Autherize",
        type: "Post",
        data: {
            usuarioCuenta: $("#usuarioCuenta").val(),
            claveCuenta: encryptedpassword,
            tipoAcceso: 1
        },
        beforeSend: function () {
            $("#btnCpbte").prop("disabled", true);
        },
        success: function (data) {
            if (data.result.Estado == "0") {
                location.href = data.result.ToUrl;
            }
            else if (data.result.Estado == "2") {
                esconderCargando();
                $("#modalBloqueo").modal("show");
                toastr.error(data.result.Mensaje);
            }
            else if (data.result.Estado == "3") {
                esconderCargando();
                $("#modalBloqueoCliente").modal("show");
            }
            else {
                esconderCargando();
                //alert("aqui")
                toastr.error(data.result.Mensaje);
            }
        },
        error: function () {
            esconderCargando();
            toastr.error("Error al intentar ingresar al sistema");
        }
    });
    return false;
}
function IrCambioClave() {
    mostrarCargando();
    var password = "abc123";
    var text = $("#claveCuenta").val();
    var encryptedpassword = encrypt(text);
    var text = $("#ClaveConfirm").val();
    var encryptedConfirm = encrypt(text);
    $.ajax({
        url: "/Usuario/ActualizarClave",
        type: "Post",
        data: {
            usuarioCuenta: $("#usuarioCuenta").val(),
            Clave: encryptedpassword,
            ClaveConfirm: encryptedConfirm,
            IdUsuario: $("#IdUsuario").val()
        },
        beforeSend: function () {
            $("#btnCpbte").prop("disabled", true);
        },
        success: function (data) {
            if (data.result.Estado == "0") {
                IrAutherize();
                //location.href = data.result.ToUrl;
            }
            else {
                esconderCargando();
                toastr.error(data.result.Mensaje);
            }
        },
        error: function () {
            esconderCargando();
            toastr.error("Error al intentar ingresar al sistema");
        }
    });
    return false;
}
function IrModificarClave() {
    mostrarCargando();
    var password = "abc123";
    var text = $("#Clave").val();
    var encryptedpassword = encrypt(text);
    var text = $("#ClaveConfirm").val();
    var encryptedConfirm = encrypt(text);
    var newClave = $("#NuevaClave").val();
    var newClaveencrypted = encrypt(newClave);
    $.ajax({
        url: "/Usuario/ModificarClave",
        type: "Post",
        data: {
            usuarioCuenta: $("#usuarioCuenta").val(),
            Clave: encryptedpassword,
            ClaveConfirm: encryptedConfirm,
            NuevaClave: newClaveencrypted,
            IdUsuario: $("#IdUsuario").val()
        },
        beforeSend: function () {
            // $("#btnCrear").prop("disabled", true);
        },
        success: function (data) {
            if (data.Estado == "0") {
                esconderCargando();
                $(".inputSol").val('');
                toastr.success(data.Mensaje);
            }
            else {
                esconderCargando();
                toastr.error(data.Mensaje);
            }
        },
        error: function () {
            esconderCargando();
            toastr.error("Error al intentar ingresar al sistema");
        }
    });
    return false;
}
function IrEditUser() {
    mostrarCargando();
    var password = "abc123";
    var text = $("#Clave").val();
    var encryptedpassword = encrypt(text);
    var RequestVerificationToken = $("[name='__RequestVerificationToken']").val();
    //alert($("#ActivoUser").prop("checked"))
    $.ajax({
        url: "/Usuario/Create",
        type: "Post",
        data: {
            IdUsuario: $("#IdUsuario").val(),
            RutUsuario: $("#RutUsuario").val(),
            NombreUsuario: $("#NombreUsuario").val(),
            ApellidoUsuario: $("#ApellidoUsuario").val(),
            CorreoElectronico: $("#CorreoElectronico").val(),
            Activo: $("#ActivoUser").prop("checked"),
            Clave: encryptedpassword,
            perfiles: $("#perfiles").val(),
            __RequestVerificationToken: RequestVerificationToken
        },
        beforeSend: function () {
            $("#btnCpbte").prop("disabled", true);
        },
        success: function (data) {
            esconderCargando();
            if (data.Estado == "0") {
                toastr.success(data.Mensaje);
                $('#modal-registro').modal('hide');
                RefreshLista();
            }
            else {
                toastr.error(data.Mensaje);
            }
        },
        error: function () {
            esconderCargando();
            toastr.error("Error al intentar ingresar al sistema");
        }
    });
    return false;
}