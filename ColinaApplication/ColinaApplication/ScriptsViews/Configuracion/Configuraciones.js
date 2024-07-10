
$(document).ready(function () {
    $(".TablaList").DataTable({
        "language": {
            "info": "",
            "lengthMenu": "Mostrar _MENU_ registros",
            "paginate": {
                "next": ">>",
                "previous": "<<"
            },
        },
        lengthMenu: [[5, 10, 20, 25, 50, -1], [5, 10, 20, 25, 50, "Todos"]],
        
    });
    ValidaResultado();
});

function ValidaResultado() {
    switch (Resultado) {
        case "True":
            Resultado = "";
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-check',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'green',
                title: 'Exitoso !',
                content: 'Se guardo/actualizó exitosamente el registro',
                buttons: {
                    Continuar: {
                        btnClass: 'btn btn-success btn2',
                        action: function () {
                            window.location.href = '#' + Posicion;
                        }
                    }
                }
            });
            break;
        case "False":
            Resultado = "";
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-times',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'red',
                title: 'Error !',
                content: 'Hubo un error al guardar/actualizar el registro. Intentelo nuevamente',
                buttons: {
                    Continuar: {
                        btnClass: 'btn btn-danger btn2',
                        action: function () {
                            window.location.href = '#' + Posicion;
                        }
                    }
                }
            });
            break;
        
        default:
            break;
    }
    
    
}

function EditarCategoria(id, categoria, estado) {
    $("#IdCategoria").val(id);
    $("#NomCategoria").val(categoria);
    $("#EstadoCategoria").val(estado);
}
function LimpiaCamposCategoria() {
    $("#IdCategoria").val("");
    $("#NomCategoria").val("");
    $("#EstadoCategoria").val("");
}
function EditarProducto(id, IdDian, idCategoria, producto, precio, cantidad, descripcion, idImpresora, idAccountGroup) {
    $("#IdProducto").val(id);
    $("#ID_DIAN").val(IdDian);
    $("#IdCategoriaProducto").val(idCategoria);
    $("#NomProducto").val(producto);
    $("#PrecioProducto").val(precio);
    $("#CantidadProducto").val(cantidad);
    $("#DescripcionProducto").val(descripcion);
    $("#IdImpresoraProductos").val(idImpresora);
    $("#IdAccountGroup").val(idAccountGroup);
    
}
function LimpiaCamposProductos() {
    $("#IdProducto").val("");
    $("#ID_DIAN").val("");
    $("#IdCategoriaProducto").val("");
    $("#NomProducto").val("");
    $("#PrecioProducto").val("");
    $("#CantidadProducto").val("");
    $("#DescripcionProducto").val("");
    $("#IdImpresoraProductos").val("");
    $("#IdAccountGroup").val("");
}
function EditarMesa(id, numeromesa, idusuario, mesa, descripcion, estado) {
    $("#IdMesa").val(id);
    $("#NumeroMesa").val(numeromesa);
    $("#IdUsuarioMesa").val(idusuario);
    $("#NombreMesa").val(mesa);
    $("#DescripcionMesa").val(descripcion);
    $("#EstadoMesa").val(estado);
}
function LimpiaCamposMesa() {
    $("#IdMesa").val("");
    $("#NumeroMesa").val("");
    $("#IdUsuarioMesa").val("");
    $("#NombreMesa").val("");
    $("#DescripcionMesa").val("");
    $("#EstadoMesa").val("");
}
function EditarUsuario(id, cedula, nombre, codigo, perfil) {
    $("#IdUsuario").val(id);
    $("#CedulaUsuario").val(cedula);
    $("#NombreUsuario").val(nombre);
    $("#CodigoUsuario").val(codigo);
    $("#IdPerfilUsuario").val(perfil);
    
}
function LimpiaCamposUsuario() {
    $("#IdUsuario").val("");
    $("#CedulaUsuario").val("");
    $("#NombreUsuario").val("");
    $("#CodigoUsuario").val("");
    $("#IdPerfilUsuario").val("");
}
function EditarImpresora(id, nombreImpresora, descripcion) {
    $("#IdImpresora").val(id);
    $("#NombreImpresora").val(nombreImpresora);
    $("#DescripcionImpresora").val(descripcion);
}
function LimpiaCamposImpresora() {
    $("#IdImpresora").val("");
    $("#NombreImpresora").val("");
    $("#DescripcionImpresora").val("");
}
function EditarImpuesto(id, nombre, porcentaje, estado) {
    $("#IdImpuesto").val(id);
    $("#NombreImpuesto").val(nombre);
    $("#PorcentajeImpuesto").val(porcentaje);
    $("#EstadoImpuesto").val(estado);
}
function LimpiaCamposImpuesto() {
    $("#IdImpuesto").val("");
    $("#NombreImpuesto").val("");
    $("#PorcentajeImpuesto").val("");
    $("#EstadoImpuesto").val("");
}
function EditarPerfil(id, nombrePerfil, porcentaje) {    
    $("#IdPerfil").val(id);
    $("#NombrePerfil").val(nombrePerfil);
    $("#PorcentajePropinaPerfil").val(porcentaje);
}
function LimpiaCamposPerfil() {
    $("#IdPerfil").val("");
    $("#NombrePerfil").val("");
    $("#PorcentajePropinaPerfil").val("");
}
function EditarNomina(id, idusuariosistema, idperfil, cedula, nombre, cargo, fechanacimiento, direccion, telefono, estado, ultimoPago) {
    $("#IdNomina").val(id);
    $("#IdUsuarioSistemaNomina").val(idusuariosistema);
    $("#IdPerfilNomina").val(idperfil);
    $("#CedulaNomina").val(cedula);
    $("#NombreNomina").val(nombre);
    $("#CargoNomina").val(cargo);
    $("#FechaNacimientoDiaNomina").val(fechanacimiento);
    $("#DireccionResidenciaNomina").val(direccion);
    $("#TelefonoNomina").val(telefono);
    $("#EstadoNomina").val(estado);
    $("#FechaUltimoPagoNomina").val(ultimoPago);
}
function LimpiaCamposNomina() {
    $("#IdNomina").val("");
    $("#IdUsuarioSistemaNomina").val("");
    $("#IdPerfilNomina").val("");
    $("#CedulaNomina").val("");
    $("#NombreNomina").val("");
    $("#CargoNomina").val("");
    $("#FechaNacimientoDiaNomina").val("");
    $("#DireccionResidenciaNomina").val("");
    $("#TelefonoNomina").val("");
    $("#EstadoNomina").val("");
    $("#FechaUltimoPagoNomina").val("");
}

function ValidaCedula() {
    if ($("#CedulaUsuario").val() != "") {
        $.ajax({
            type: "POST",
            url: urlConsultaCedulaExistente,
            data: JSON.stringify({ Cedula: $("#CedulaUsuario").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "JSON",
            success: function (result) {                
                var json = JSON.parse(result);
                if (json) {
                    $("#CedulaUsuario").val('');
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-times',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'red',
                        title: 'Cedula repetida !',
                        content: 'Esta cedula ya existe en el sistema. Por favor digite otra',
                        buttons: {
                            Continuar: {
                                btnClass: 'btn btn-danger btn2',
                                action: function () {
                                }
                            }
                        }
                    });
                }
            },
            error: function (request, status, error) {
                console.log(error);
            }

        });
    }
}

function ValidaCodigo() {
    if ($("#CodigoUsuario").val() != "") {
        $.ajax({
            type: "POST",
            url: urlConsultaCodigoExistente,
            data: JSON.stringify({ Codigo: $("#CodigoUsuario").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "JSON",
            success: function (result) {                
                var json = JSON.parse(result);
                if (json) {
                    $("#CodigoUsuario").val('');
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-times',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'red',
                        title: 'Codigo repetido !',
                        content: 'Este código ya esta asignado a otro usuario. Por favor digite otro',
                        buttons: {
                            Continuar: {
                                btnClass: 'btn btn-danger btn2',
                                action: function () {
                                }
                            }
                        }
                    });
                }
            },
            error: function (request, status, error) {
                console.log(error);
            }

        });
    }
}
function CambiaParametrosSistema(id, valor) {
    $("#IdSistema").val(id);
    $("#SistemaValor").val(valor);
    $("#GuardarSistema").click();
}
