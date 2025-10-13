
let connectPSR;
let ProductosSolicitudVector;
let descripcion;
let ProductosPedido = [];
let IdProd;
let DatosClienteDIAN = [];
let PagosDIAN = [];
let DatosSolicitud = [];
var BorradorDian = "";

$(function PedidoSignalR() {

    CargaCategorias();
    CargaMeseros();
    connectPSR = $.connection.solicitudhub;

    Llama_MetodosPSR(connectPSR);

    $.connection.hub.start().done(function () {
        Registra_EventosPSR(connectPSR);
    });

});

//SE ENVIAN LOS METODOS AL HUB
function Registra_EventosPSR(connectpsr) {
    //BOTON DE GUARDAR PRODUCTOS
    $('#AgregaProductos').click(function () {
        cargando();
        if (ProductosPedido.length > 0) {
            $("#AgregaProductos").attr("disabled", "disabled");
            //ENVIA AL METODO INSERTAR
            connectpsr.server.insertaProductosSolicitud(ProductosPedido, $('#ID_MESA').val());
        }
        else {
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-times',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'red',
                title: 'Oops',
                content: 'Debe seleccionar al menos un producto antes de enviar',
                buttons: {
                    Ok: {
                        btnClass: 'btn-danger btn2',
                        action: function () {

                        }
                    }
                }
            });
        }
        cerrar();
    });

    //CONsULTA MESA ABIERTA EN PANTALLA
    connectpsr.server.consultaMesaAbierta($('#ID_MESA').val());
}

//SE EJECUTAN LOS METODOS QUE ENVIAN DESDE EL HUB
function Llama_MetodosPSR(connectpsr) {
    cargando();
    //LISTA LOS DETALLES DE LA MESA
    connectpsr.client.ListaDetallesMesa = function (data) {
        if (data[0].IdMesa == $('#ID_MESA').val()) {
            if (data.length > 0) {

                DatosSolicitud = data;
                ProductosSolicitudVector = data[0].ProductosSolicitud;
                ActualizaInfoMesa(data);
                ActualizaInfoPrecios(data);
                ActualizaInfoProductos(data);
                $("#ID").val(data[0].Id);
            }
            $("#GuardaDatosCliente").removeAttr("disabled");
        }

    }

    //RECIBE DEL METODO CUANDO GUARDA PRODUCTOS
    connectpsr.client.GuardoProductos = function (data) {
        if (data == "Productos Insertados Exitosamente !") {
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-check',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'green',
                title: 'Super !',
                content: data,
                buttons: {
                    Continuar: {
                        btnClass: 'btn-success btn2',
                        action: function () {
                            CargaCategorias();
                            $("#setProductosElegidos").empty();
                            ProductosPedido = [];
                            $("#AgregaProductos").removeAttr("disabled");
                        }
                    }
                }
            });
        }
        else {
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-question',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'red',
                title: 'Error !',
                content: data,
                buttons: {
                    Continuar: {
                        btnClass: 'btn-success btn2',
                        action: function () {
                            CargaCategorias();
                            $("#AgregaProductos").removeAttr("disabled");
                        }
                    }
                }
            });
        }

    }

    //RECIBE EL METODO CUANDO NO HAY EXISTENCIAS DE ALGUNOS PRODUCTOS
    connectpsr.client.FaltaExistencias = function (data) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-question',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Oops !',
            content: "Al parecer no hay suficientes existencias para <br/><br/><b>" + data.toString() + "</b>.<br/><br/> Prueba seleccionando menos o consulta con el administrador.",
            buttons: {
                Continuar: {
                    btnClass: 'btn-danger btn2',
                    action: function () {
                        CargaCategorias();
                        $("#AgregaProductos").removeAttr("disabled");
                    }
                }
            }
        });


    }

    //RECIBE DEL METODO CUANDO GUARDA CLIENTE
    connectpsr.client.GuardoCliente = function (data) {
        CargaCategorias();
    }

    ////RECIBE METODOD PARA REDIRECCIONAR
    //connectpsr.client.ListaMesas = function (listamesas, Redirecciona, idmesa) {
    //    if (Redirecciona == "SI" && idmesa == $("#ID_MESA").val()) {
    //        window.location.href = '../Solicitud/SeleccionarMesa';
    //    };
    //}

    //
    connectpsr.client.CambiaIdMesa = function (idmesa, idmesaAnterior) {
        if ($("#ID_MESA").val() == idmesaAnterior) {
            $("#ID_MESA").val(idmesa);
        }
    }
    //RECIBE EL METODO DE CAMBIO DE MESA
    connectpsr.client.ListaMesas = function (data, Redirecciona, idmesa, ruta) {
        $("#ListaMesas").empty();
        for (var i = 0; i < data.length; i++) {
            switch (data[i].ESTADO) {
                case "LIBRE":
                    $("#ListaMesas").append('<div id=' + data[i].ID + ' onclick="CambioMesa(this.id, \'OCUPADO\');" class="card text-white bg-success estilo" style="width: 120px; text-align: center; float: left; margin: 5px; cursor: pointer; ">' +
                        '<div class="card-header">' +
                        '<h2 style="font-size: 22px; overflow: hidden; text-overflow: ellipsis; white-space: pre;">' + data[i].NOMBRE_MESA + '</h2>' +
                        '</div>' +
                        '<br/><i class="fa fa-2x fa-check"></i><br/>' +
                        '</div>');
                    break;
                case "OCUPADO":
                    $("#ListaMesas").append('<div id=' + data[i].ID + ' class="card text-white bg-danger estilo" style="width: 120px; text-align: center; float: left; margin: 5px; cursor: not-allowed;">' +
                        '<div class="card-header">' +
                        '<h2 style="font-size: 22px; overflow: hidden; text-overflow: ellipsis; white-space: pre;">' + data[i].NOMBRE_MESA + '</h2>' +
                        '</div>' +
                        '<br/><i class="fa fa-2x fa-arrow-down"></i><br/>' +
                        '</div>');
                    break;
                case "ESPERA":
                    $("#ListaMesas").append('<div id=' + data[i].ID + ' class="card text-dark bg-warning estilo" style="width: 120px; text-align: center; float: left; margin: 5px; cursor: not-allowed; ">' +
                        '<div class="card-header">' +
                        '<h2 style="font-size: 22px; overflow: hidden; text-overflow: ellipsis; white-space: pre;">' + data[i].NOMBRE_MESA + '</h2>' +
                        '</div>' +
                        '<br/><i class="fa fa-2x fa-clock-o"></i><br/>' +
                        '</div>');
                    break;
                case "NO DISPONIBLE":
                    $("#ListaMesas").append('<div id=' + data[i].ID + ' class="card text-white bg-secondary" style="width: 120px; text-align: center; float: left; margin: 5px; cursor: not-allowed; ">' +
                        '<div class="card-header">' +
                        '<h2 style="font-size: 22px; overflow: hidden; text-overflow: ellipsis; white-space: pre;">' + data[i].NOMBRE_MESA + '</h2>' +
                        '</div>' +
                        '<br/><i class="fa fa-2x fa-times-circle"></i><br/>' +
                        '</div>');
                    break;

                default:
                    break;
            }
        }
        // SE REALIZA EL REDIRECCIONAMIENTO EN ESTE PUNTO YA QUE SI SE DEJA SOBRE ONCLICK DE CADA MESA, NO ACTUALIZA LA MESA
        if (Redirecciona == "SI" && idmesa == $("#ID_MESA").val()) {

            window.location.href = ruta;
        }
    }

    connectpsr.client.DividioCuenta = function (idSolicitudDividida, idSolicitudPrincipal, subTotalDiv, servicioTotalDiv, totalDiv, cantidadProdActual) {
        if (idSolicitudPrincipal == $('#ID').val()) {
            $('#CerrarModalDC').click();
            $('#ID_SOLICITUD_DIVIDIDA').val(idSolicitudDividida);
            $('#SUBTOTAL_DIVIDIDA').val(subTotalDiv);
            $('#PORC_SERVICIO_DIVIDIDA').val(servicioTotalDiv);
            $('#TOTAL_DIVIDIDA').val(totalDiv);
            PagarFacturaDC();
        }
    }
}


//METODOS DE ACTUALIZACION DE MESA, PRECIOS Y SOLICITUD EN GENERAL
function ActualizaInfoMesa(data) {
    if (data[0].EstadoSolicitud == "ABIERTA") {
        $("#DivLlevar").css("display", "block");
        $("#DivAsignar").css("display", "none");
        $("#InfoMesa").empty();
        $("#InfoMesa").append('<div class="col-lg-12">' +
            '<input class="" type="checkbox" id="DianSistema" checked >' +
            '<div class="small-box bg-danger">' +
            '<div class="inner">' +
            '<h3>' +
            '#' + data[0].NumeroMesa +
            '</h3>' +
            '<h3><b>' + data[0].NombreMesa + '</b></h3>' +
            '<p><b>Mesero:<b/> ' + data[0].NombreMesero + '</p>' +
            '<p style ="float:right;"><b>Factura Electronica <b/>' +
            '<input class="form-check-input" type="checkbox" id="EnvioDian" onchange="EnviaDian(this.checked)" ></p>' +
            '<select id="PersonaDian" class="form-select" style="" onchange="CambiaPersona(this.value)">' +
            '<option value="">** Tipo Persona **</option>' +
            '<option value="Person">Persona</option>' +
            '<option value="Company">Empresa</option>' +
            '</select > <br/>' +
            '<select id="TipoDocumentoDian" class="form-select" style="" onchange="">' +
            '<option value="">** Tipo Documento **</option>' +
            '<option value="13">Cédula ciudadania</option>' +
            '<option value="31">NIT</option>' +
            '<option value="22">Cédula extranjeria</option>' +
            '<option value="42">Documento de identificación extranjero</option>' +
            '<option value="50">Nit Otro País</option>' +
            '<option value="R-00-PN">No obligado a registrarse en el RUT PN</option>' +
            '<option value="91">NUIP</option>' +
            '<option value="41">Pasaporte</option>' +
            '<option value="47">Permiso especial de permanencia PEP</option>' +
            '<option value="11">Registro civil</option>' +
            '<option value="43">Sin identificación del exterior o para uso definido por la DIAN</option>' +
            '<option value="21">Tarjeta de extranjería</option>' +
            '<option value="12">Tarjeta de identidad</option>' +
            '</select ><br/>' +
            '<p><b># Identificación: </b><input id="CCCliente" type="text" autocomplete="off" class="form-control" value="' + data[0].cliente.NumeroIdentificacion + '" onchange="ConsultaClienteDian(this.value)" name="CCCliente"/></p>' +
            '<p><b># Digito Verificacion: </b><input id="CDigitVerif" type="text" autocomplete="off" class="form-control" value="' + data[0].cliente.DigitoVerif + '"/></p>' +
            '<div id="SecciondianPersona">' +
            '</div>' +
            '<p><b>Telefono: </b><input id="TelefonoDian" type="text" autocomplete="off" class="form-control" name="TelefonoDian"/></p>' +
            '<p><b>Correo Electronico: </b><input id="CorreoClienteDian" type="text" autocomplete="off" class="form-control" name="CorreoClienteDian"/></p>' +
            '<select id="TipoPersonaDian" class="form-select">' +
            '<option value="">** Tipo Regimen IVA **</option>' +
            '<option value="false">NO responsable de IVA</option>' +
            '<option value="true">Responsable de IVA</option>' +
            '</select > <br/>' +
            '<p><b>Responsabilidad fiscal:</b><small style = "font-weight: 200;"> (Verifica la responsabilidad en el RUT de tu cliente, mínimo asignar R-99-PN)</small> <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-13" > O-13 Gran Contribuyente <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-15" > O-15 Autorretenedor <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-23" > O-23 Agente de retencion IVA <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-47" > O-47 Regimen simple de tributacion <br/>' +
            '<input class="form-check-input" type="checkbox" id="R-99-PN" checked> R-99-PN No aplica - Otros <br/>' +
            '</p> ' +
            '</div>' +
            '<div class="icon">' +
            '<i class="fa fa-arrow-down"></i>' +
            '</div>' +
            '</div>' +
            '</div>');
    }
    if (data[0].EstadoSolicitud == "LLEVAR") {
        $("#DivLlevar").css("display", "none");
        $("#DivAsignar").css("display", "block");
        $("#InfoMesa").empty();
        $("#InfoMesa").append('<div class="col-lg-12">' +
            '<input class="" type="checkbox" id="DianSistema" checked >' +
            '<div class="small-box bg-warning" style="background-color: #e3a200 !important; ">' +
            '<div class="inner">' +
            '<h3>' +
            '#' + data[0].NumeroMesa +
            '</h3>' +
            '<h3><b>' + data[0].NombreMesa + '</b></h3>' +
            '<p><b>Mesero:<b/> ' + data[0].NombreMesero + '</p>' +
            '<p style ="float:right;"><b>Factura Electronica <b/>' +
            '<input class="form-check-input" type="checkbox" id="EnvioDian" onchange="EnviaDian(this.checked)" ></p>' +
            '<select id="PersonaDian" class="form-select" style="" onchange="CambiaPersona(this.value)">' +
            '<option value="">** Tipo Persona **</option>' +
            '<option value="Person">Persona</option>' +
            '<option value="Company">Empresa</option>' +
            '</select > <br/>' +
            '<select id="TipoDocumentoDian" class="form-select" style="" onchange="">' +
            '<option value="">** Tipo Documento **</option>' +
            '<option value="13">Cédula ciudadania</option>' +
            '<option value="31">NIT</option>' +
            '<option value="22">Cédula extranjeria</option>' +
            '<option value="42">Documento de identificación extranjero</option>' +
            '<option value="50">Nit Otro País</option>' +
            '<option value="R-00-PN">No obligado a registrarse en el RUT PN</option>' +
            '<option value="91">NUIP</option>' +
            '<option value="41">Pasaporte</option>' +
            '<option value="47">Permiso especial de permanencia PEP</option>' +
            '<option value="11">Registro civil</option>' +
            '<option value="43">Sin identificación del exterior o para uso definido por la DIAN</option>' +
            '<option value="21">Tarjeta de extranjería</option>' +
            '<option value="12">Tarjeta de identidad</option>' +
            '</select ><br/>' +
            '<p><b># Identificación: </b><input id="CCCliente" type="text" autocomplete="off" class="form-control" value="' + data[0].cliente.NumeroIdentificacion + '" onchange="ConsultaClienteDian(this.value)" name="CCCliente"/></p>' +
            '<p><b># Digito Verificacion: </b><input id="CDigitVerif" type="text" autocomplete="off" class="form-control" value="' + data[0].cliente.DigitoVerif + '"/></p>' +
            '<div id="SecciondianPersona">' +
            '</div>' +
            '<p><b>Telefono: </b><input id="TelefonoDian" type="text" autocomplete="off" class="form-control" name="TelefonoDian"/></p>' +
            '<p><b>Correo Electronico: </b><input id="CorreoClienteDian" type="text" autocomplete="off" class="form-control" name="CorreoClienteDian"/></p>' +
            '<select id="TipoPersonaDian" class="form-select">' +
            '<option value="">** Tipo Regimen IVA **</option>' +
            '<option value="false">NO responsable de IVA</option>' +
            '<option value="true">Responsable de IVA</option>' +
            '</select > <br/>' +
            '<p><b>Responsabilidad fiscal:</b><small style = "font-weight: 200;"> (Verifica la responsabilidad en el RUT de tu cliente, mínimo asignar R-99-PN)</small> <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-13" > O-13 Gran Contribuyente <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-15" > O-15 Autorretenedor <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-23" > O-23 Agente de retencion IVA <br/>' +
            '<input class="form-check-input" type="checkbox" id="O-47" > O-47 Regimen simple de tributacion <br/>' +
            '<input class="form-check-input" type="checkbox" id="R-99-PN" checked> R-99-PN No aplica - Otros <br/>' +
            '</p> ' +
            '</div>' +
            '<div class="icon">' +
            '<i class="fa fa-clock-o"></i>' +
            '</div>' +
            '</div>' +
            '</div>');
    }

    $("#ID_MESERO").val(data[0].IdMesero)
    $("#ESTADO_SOLICITUD").val(data[0].EstadoSolicitud)
    $("#OBSERVACIONES").val(data[0].Observaciones);
    $("#ID_CLIENTE").val(data[0].IdCliente);
    $("#FACTURACION_ELECTRONICA").val(data[0].FactracionElectronica);

    //Carga Datos Facturacion Electronica
    if (data[0].FactracionElectronica == "1") {
        if (data[0].FactracionElectronica == "1") {
            $('#EnvioDian').prop("checked", true);
            EnviaDian(true);
        }
        else {
            $('#EnvioDian').prop("checked", true);
            EnviaDian(false);
        }
        $('#PersonaDian option[value="' + data[0].cliente.TipoPersona + '"]').prop('selected', true);
        CambiaPersona(data[0].cliente.TipoPersona);
        $('#TipoDocumentoDian option[value="' + data[0].cliente.CodigoDocumento + '"]').prop('selected', true);
        $("#CCCliente").val(data[0].cliente.NumeroIdentificacion);
        $('#CDigitVerif').val(data[0].cliente.DigitoVerif);
        $("#TelefonoDian").val(data[0].cliente.Telefono);
        $("#CorreoClienteDian").val(data[0].cliente.Email);
        $('#TipoPersonaDian option[value="' + data[0].cliente.ResponsableIva + '"]').prop('selected', true);
        var RFiscals = data[0].cliente.CodigoRFiscal.split(";");
        for (var i = 0; i < RFiscals.length; i++) {
            $('#' + RFiscals[i]).prop("checked", true);
        }
        $("#NombreCliente").val(data[0].cliente.Nombres);
        $("#ApellidosCliente").val(data[0].cliente.Apellidos);
        $("#RazonSocialCliente").val(data[0].cliente.RazonSocial);
        $("#NComercialCliente").val(data[0].cliente.NombreComercial);
        $("#DireccionCliente").val(data[0].cliente.Direccion);
    }

    if (IdPerfil == "1" || IdPerfil == "2") {
        //Carga Si esta dividiendo factura
        if (data[0].SolicitudDividida != null) {
            $("#ID_SOLICITUD_DIVIDIDA").val(data[0].SolicitudDividida.Id);
            $("#SUBTOTAL_DIVIDIDA").val(data[0].SolicitudDividida.Subtotal);
            $("#PORC_SERVICIO_DIVIDIDA").val(data[0].SolicitudDividida.PorcentajeServicio);
            $("#TOTAL_DIVIDIDA").val(data[0].SolicitudDividida.Total);
            PagarFacturaDC();
        }
        else if (data[0].MesaDividida == "1" && data[0].Total != 0) {
            document.getElementById('BotonDCModal').style.display = 'active';
            $('#BotonDCModal').click();
        }
        else if (data[0].MesaDividida == "1" && data[0].Total == 0) {
            DatosClienteDIAN = [];
            DatosClienteDIAN.push("false");
            DatosClienteDIAN.push(token);
            connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), "0",
                "FINALIZADA", $("#ID_MESA").val(), 0, "N/A", PagosDIAN, "0",
                $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
                false, 0, BorradorDian, "0");
            connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");
        }
    }
    else if (data[0].MesaDividida == "1") {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-money',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Contacte Cajero -Admin !',
            content: 'La cuenta esta siendo dividida por Cajero/Admin.',
            buttons: {
                Ok: {
                    btnClass: 'btn btn-default',
                    action: function () {
                        window.location.href = '../Solicitud/SeleccionarMesa';
                    }
                }
            }

        });
    }
}
function ActualizaInfoPrecios(data) {
    console.log(data);
    var IVA = '';
    var ICONSUMO = '';
    var SERVICIO = '';
    if (data[0].Impuestos[0].Estado == "ACTIVO")
        IVA = '<tr> ' +
            '<td>' +
            '<small><b>I.V.A (' + data[0].PorcentajeIVA + '%) : <b></small>' +
            '<input id="Iva" type="text" class="form-control input-sm" name="Iva" value="' + data[0].IVATotal + '" ReadOnly="true"/>' +
            '</td>' +
            '</tr>';
    if (data[0].Impuestos[1].Estado == "ACTIVO") {
        var sumaSubtotalConsumo = data[0].Subtotal + data[0].IConsumoTotal;
        ICONSUMO = '<tr> ' +
            '<td>' +
            '<small><b>Impuesto Consumo (' + data[0].PorcentajeIConsumo + '%) : <b></small>' +
            '<input id="ImConsumo" type="text" class="form-control input-sm" name="ImConsumo" value="' + data[0].IConsumoTotal + '" ReadOnly="true"/>' +
            '</td>' +
            '</tr>' +
            '<tr> ' +
            '<td>' +
            '<small style = "color: #dc3545;"><b>DATAFONO: <b></small>' +
            '<input style="font-weight:bolder; font-size: 25px;" type="text" class="form-control input-sm" value="' + sumaSubtotalConsumo + '" ReadOnly="true" />' +
            '</td>' +
            '</tr>';
    }
    if (data[0].Impuestos[2].Estado == "ACTIVO")
        SERVICIO = '<tr>' +
            '<td>' +
            '<small><b>Servicio (' + data[0].Impuestos[2].Porcentaje + '% Máx.) :</b></small>&nbsp;&nbsp;&nbsp;<small style="color: white; font-weight: bold; font-size: 20px; padding: 5px; border-radius: 5px; background-color: #30a630;">    $' + data[0].ServicioTotal + '</small><br>' +
            '<span class="input-group-btn" style="float: left;">' +
            '<button class="btn btn-success" id="menosServicio" type="button" onclick="menosServicio()"><b>-</b></button>' +
            '</span>' +
            '<input type="text" style="width:62px;text-align: center; float: left; margin-left: 2%;" id="servicio" class="form-control" value="' + data[0].PorcentajeServicio + '" readonly />' +
            '<span class="input-group-btn" style="float: left; margin-left: 2%;">' +
            '<button class="btn btn-success" id="masServicio" type="button" onclick="masServicio(' + data[0].Impuestos[2].Porcentaje + ')"><b>+</b></button>' +
            '</span>' +
            '<br/><br/><small><b>Digitar valor (Opcional):<b/><small/>' +
            '<input type="text" " style="background-color: #30a630c7; font-size: 24px; color: white; font-weight: bolder;" ' +
            'id="servicioDig" class="form-control" value="' + data[0].ServicioTotal + '" onkeypress = "return soloNum(event)" onchange="ValidarValores(' + data[0].Subtotal + ', ' + data[0].ServicioTotal + ')" />' +
            '</td>' +
            '</tr>';

    $("#InfoPrecios").empty();
    $("#InfoPrecios").append('<table class="table table-hover" style="font-size: 18px;">' +
        '<tbody>' +
        '<tr>' +
        '<td>' +
        '<small><b>Otros Cobros: </b></small>' +
        '<input id="OtrosCobros" type="text" class="form-control input-sm" name="OtrosCobros" value="' + data[0].OtrosCobros + '" onkeypress = "return soloNum(event)" onpaste="return false" ReadOnly="true"/>' +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>' +
        '<small><b>Descuentos: </b></small>' +
        '<input id="Descuentos" type="text" class="form-control input-sm" name="Descuentos" value="' + data[0].Descuentos + '" onkeypress = "return soloNum(event)" onpaste="return false"/>' +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>' +
        '<small><b>SubTotal: <b></small>' +
        '<input id="SubTotal" type="text" class="form-control input-sm" name="SubTotal" value="' + data[0].Subtotal + '" ReadOnly="true"/>' +
        '</td>' +
        '</tr>' +
        IVA +

        ICONSUMO +
        SERVICIO +
        '<tr>' +
        '<td>' +
        '<small><b>Total: <b></small>' +
        '<input id="Total" type="text" class="form-control input-sm" name="Total" value="' + data[0].Total + '" ReadOnly="true"/>' +
        '</td>' +
        '</tr>' +
        '</tbody>' +
        '</table>');
    //TotalGeneral = data[0].Total;
    //SumaTotal();

}
function ActualizaInfoProductos(data) {
    $("#InfoProductos").empty();
    $("#InfoProductos").append('<table id="Tabla2" class="table table-bordered table-hover">' +
        '<thead>' +
        '<tr>' +
        '<th>Editar</th>' +
        '<th>Producto</th>' +
        '<th>Descripcion</th>' +
        '<th>Precio</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody id="BodyProductos">' +

        '</tbody>' +
        '</table>');

    $("#BodyProductos").empty();

    for (var i = 0; i < data[0].ProductosSolicitud.length; i++) {
        var code = '';
        var color = '#a90000';
        if (data[0].ProductosSolicitud[i].EstadoProducto == "ENTREGADO")
            color = '#5cb85c';
        descripcion = data[0].ProductosSolicitud[i].Descripcion.toString();
        if (IdPerfil == 1) {
            code = '<i class="fa fa-2x fa-minus-square" style="color: #a90000; cursor:pointer;" onclick="CancelaProductoxId(' + data[0].ProductosSolicitud[i].Id + ',' + data[0].ProductosSolicitud[i].Id + ')"></i>' +
                '<i id="' + descripcion + '" class="fa fa-2x fa-print" style="color: ' + color + '; cursor:pointer; margin-left: 5px;" onclick="ReEnviaProducto(' + data[0].ProductosSolicitud[i].IdProducto + ', this.id, ' + data[0].IdMesa + ', ' + data[0].ProductosSolicitud[i].Id+')"></i >';
        }
        else {
            code = '<i id="' + descripcion + '" class="fa fa-2x fa-print" style="color: ' + color + '; cursor:pointer; margin-left: 5px;" onclick="ReEnviaProducto(' + data[0].ProductosSolicitud[i].IdProducto + ', this.id, ' + data[0].IdMesa + ', ' + data[0].ProductosSolicitud[i].Id +')"></i >';
        }
        $("#BodyProductos").append('<tr>' +
            '<td>' +
            code +
            '</<td>' +
            '<td>' +
            data[0].ProductosSolicitud[i].NombreProducto +
            '</<td>' +
            '<td>' +
            data[0].ProductosSolicitud[i].Descripcion +
            '</<td>' +
            '<td>' +
            data[0].ProductosSolicitud[i].PrecioProducto +
            '</<td>' +
            '</tr>');
    }
    cerrar();
    //$('#Tabla2').DataTable({
    //    "language": {
    //        "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
    //    }
    //});
}
//FUNCION DE CANCELAR PRODUCTO POR ID
function CancelaProductoxId(idProducto) {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-times',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'red',
        title: 'Eliminar !',
        content: 'Esta seguro que desea eliminar este producto ?',
        buttons: {
            Si: {
                btnClass: 'btn btn-danger btn2',
                action: function () {
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-warning',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'red',
                        title: 'Cancelar !',
                        content: 'Desea retornar el elemento al inventario ?',
                        buttons: {
                            Si: {
                                btnClass: 'btn btn-danger',
                                action: function () {
                                    connectPSR.server.cancelaPedidoXId(idProducto, true);
                                    $.alert({
                                        theme: 'Modern',
                                        icon: 'fa fa-check',
                                        boxWidth: '500px',
                                        useBootstrap: false,
                                        type: 'green',
                                        title: 'Super !',
                                        content: 'El producto fue eliminado !',
                                        buttons: {
                                            Continuar: {
                                                btnClass: 'btn btn-success',
                                                action: function () {
                                                    connectPSR.server.consultaMesaAbierta($('#ID_MESA').val());
                                                }
                                            },
                                        }
                                    });
                                }
                            },
                            No: {
                                btnClass: 'btn btn-danger',
                                action: function () {
                                    connectPSR.server.cancelaPedidoXId(idProducto, false);
                                    $.alert({
                                        theme: 'Modern',
                                        icon: 'fa fa-check',
                                        boxWidth: '500px',
                                        useBootstrap: false,
                                        type: 'green',
                                        title: 'Super !',
                                        content: 'El producto fue eliminado !',
                                        buttons: {
                                            Continuar: {
                                                btnClass: 'btn btn-success btn2',
                                                action: function () {
                                                    connectPSR.server.consultaMesaAbierta($('#ID_MESA').val());
                                                }
                                            },
                                        }
                                    });
                                }
                            },
                        }
                    });
                }
            },
            Cancelar: {
                btnClass: 'btn btn-danger',
                action: function () {

                }
            },
        }
    });
}

//REENVIA PRODUCTOS A IMPRESORAS
function ReEnviaProducto(idproducto, description, idmesa, idprodsolic) {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-question',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'Gray',
        title: ' Cuidado !',
        content: 'Esta seguro que desea reimprimir el producto ?',
        buttons: {
            Continuar: {
                btnClass: 'btn btn-default btn2',
                action: function () {
                    connectPSR.server.imprimeProductos(1, idproducto, description, idmesa);
                }
            },
            Cancelar: {
                btnClass: 'btn btn-default',
                action: function () {

                }
            }
        }
    });

}


//METODOS DE CARGA DE CATEGORIA, PRODUCTO Y ADICIONES. 
function CargaCategorias() {
    $("#setCategoria").empty();
    $("#setProducto").empty();
    $("#tableProductos").css("display", "none");
    $("#tableAdiciones").css("display", "none");
    $(".Categ").css("background-color", "transparent");
    $("#Adiciones").val('');
    $("#contador").val('1');
    $("#ID_PRODUCTO").val('');
    $("#PRECIO_PRODUCTO").val('');

    $.ajax({
        type: "POST",
        url: urlListaCategorias,
        contentType: "application/json; charset=utf-8",
        dataType: "JSON",
        success: function (result) {
            var json = JSON.parse(result);
            var br = '<br/><br/><br/><br/><br/>';
            if (json.length > 0) {
                for (var index = 0, len = json.length; index < len; index++) {
                    $("#setCategoria").append('<div class="Categ" id="' + json[index].ID + '_Categ" style = "margin-left: 2%; margin-top: 2%; float:left; border: 2px solid; width: 100px; height: 100px; border-radius: 5px; display: flex; align-items: center; text-align: center; cursor: pointer; background-color: #ffc93163" onclick="CargaProducto(' + json[index].ID + ')">' +
                        '<div style="width: 100%; font-family: Copperplate Gothic Bold; font-size: 16px;"><b>' + json[index].CATEGORIA + '</b></div>' +
                        '</div >');
                    if (index == 6 || index == 13 || index == 20 || index == 27) {
                        $("#setCategoria").append(br);
                    }
                }
            }
        },
        error: function (request, status, error) {
            console.log(error);
        }

    });
}
function CargaMeseros() {
    $("#selectMeseros").append("<option value=''>--SELECCIONE--</option>");
    $.ajax({
        type: "POST",
        url: urlListaMeseros,
        contentType: "application/json; charset=utf-8",
        dataType: "JSON",
        success: function (result) {
            var json = JSON.parse(result);
            if (json.length > 0) {
                for (var index = 0, len = json.length; index < len; index++) {
                    $('#selectMeseros').append($('<option>', {
                        value: json[index].ID,
                        text: json[index].NOMBRE
                    }));
                }
            }
        },
        error: function (request, status, error) {
            console.log(error);
        }
    });
}
function CargaProducto(id) {
    IdProd = id;
    $("#setProducto").empty();
    $("#tableProductos").css("display", "block");
    $("#tableAdiciones").css("display", "none");
    $(".Categ").css("background-color", "#ffc93163");
    $("#" + id + "_Categ").css("background-color", "#d4d4d4");
    $("#Adiciones").val('');
    $("#contador").val('1');
    $("#ID_PRODUCTO").val('');
    $("#PRECIO_PRODUCTO").val('');

    $.ajax({
        type: "POST",
        url: urlListaProductos,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ IdProducto: id }),
        dataType: "JSON",
        success: function (result) {
            var json = JSON.parse(result);
            var br = '<br/><br/><br/><br/><br/>';
            if (json.length > 0) {
                for (var index = 0, len = json.length; index < len; index++) {
                    if (json[index].CANTIDAD >= 1) {
                        $("#setProducto").append('<div class="Prod" id="' + json[index].ID + '_Producto" precio="' + json[index].PRECIO + '" cantidad="' + json[index].CANTIDAD + '" style = "margin-left: 2%; margin-top: 2%; float:left; border: 2px solid; width: 100px; height: 100px; border-radius: 5px; display: flex; align-items: center; text-align: center; cursor: pointer; background-color: #f4a1247d;" onclick="CargaAdiciones(' + json[index].ID + ', ' + json[index].PRECIO + ', \'' + json[index].NOMBRE_PRODUCTO + '\')">' +
                            '<div style="width: 100%; font-family: Copperplate Gothic Bold; font-size: 16px; background-color: transparent;"><b>' + json[index].NOMBRE_PRODUCTO + '</b></div>' +
                            '</div >');
                    }
                    else {
                        $("#setProducto").append('<div id="' + json[index].ID + '_Producto" precio="' + json[index].PRECIO + '" cantidad="' + json[index].CANTIDAD + '" style = "margin-left: 2%; margin-top: 2%; float:left; border: 2px solid; width: 100px; height: 100px; border-radius: 5px; display: flex; align-items: center; text-align: center; cursor: not-allowed; background-color: #aa020273;">' +
                            '<div style="width: 100%; font-family: Copperplate Gothic Bold; font-size: 16px; background-color: transparent;"><b>' + json[index].NOMBRE_PRODUCTO + '</b></div>' +
                            '</div >');
                    }
                    if (index == 6 || index == 13 || index == 20 || index == 27) {
                        $("#setProducto").append(br);
                    }
                }
            }
        },
        error: function (request, status, error) {
            console.log(error);
        }

    });
}
function CargaAdiciones(id, precio, nomproducto) {
    $("#tableAdiciones").css("display", "block");
    $(".Prod").css("background-color", "#f4a1247d");
    $("#" + id + "_Producto").css("background-color", "#d4d4d4");
    $("#Adiciones").val('');
    $("#contador").val('1');

    //ASIGNA A VARIABLES PARA GUARDAR
    $("#ID_PRODUCTO").val(id);
    $("#PRECIO_PRODUCTO").val(precio);
    $("#NOMBREPRODUCTO").val(nomproducto);


}

//METODO SOLO GUARDA DATOS DEL CLIENTE
function GuardarDatosCliente() {
    cargando();
    $("#GuardaDatosCliente").attr("disabled", "true");
    if ($('#EnvioDian').prop("checked")) {
        if ($('#PersonaDian').val() != "" && $('#TipoDocumentoDian').val() != "" && $("#CCCliente").val() != "" && (($("#NombreCliente").val() != "" && $('#ApellidosCliente').val() != "") || $('#RazonSocialCliente').val())
            && $('#TelefonoDian').val() != "" && $('#CorreoClienteDian').val() != "" && $('#TipoPersonaDian').val() != "") {
            DatosClienteDIAN = [];
            DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
            DatosClienteDIAN.push($('#PersonaDian').val());
            DatosClienteDIAN.push($('#TipoDocumentoDian').val());
            DatosClienteDIAN.push($('#ApellidosCliente').val());
            DatosClienteDIAN.push($('#NComercialCliente').val());
            DatosClienteDIAN.push($('#DireccionCliente').val());
            DatosClienteDIAN.push($('#RazonSocialCliente').val());
            DatosClienteDIAN.push($('#TelefonoDian').val());
            DatosClienteDIAN.push($('#CorreoClienteDian').val());
            DatosClienteDIAN.push($('#TipoPersonaDian').val());
            DatosClienteDIAN.push($('#O-13').prop("checked"));
            DatosClienteDIAN.push($('#O-15').prop("checked"));
            DatosClienteDIAN.push($('#O-23').prop("checked"));
            DatosClienteDIAN.push($('#O-47').prop("checked"));
            DatosClienteDIAN.push($('#R-99-PN').prop("checked"));
            DatosClienteDIAN.push(token);
            DatosClienteDIAN.push($('#CDigitVerif').val());
            connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), $("#OBSERVACIONES").val(),
                $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(), $("#ESTADO_SOLICITUD").val(), $("#ID_MESA").val(),
                $("#servicio").val(), "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
                $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-check',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'green',
                title: 'Super!',
                content: "Datos Guardados !",
                buttons: {
                    Continuar: {
                        btnClass: 'btn-success btn2',
                        action: function () {

                        }
                    }
                }
            });
        }
        else {
            $.alert({
                theme: 'Modern',
                icon: 'fa fa-times',
                boxWidth: '500px',
                useBootstrap: false,
                type: 'red',
                title: 'Campos Nulos !',
                content: 'Debe digitar todos los campos',
                buttons: {
                    Ok: {
                        btnClass: 'btn btn-danger',
                        action: function () {
                            $("#GuardaDatosCliente").removeAttr("disabled");
                            cerrar();
                        }
                    }
                }
            });
        }
    }
    else {
        DatosClienteDIAN = [];
        DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
        connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), $("#OBSERVACIONES").val(),
            $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(), $("#ESTADO_SOLICITUD").val(), $("#ID_MESA").val(),
            $("#servicio").val(), "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
            $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-check',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'green',
            title: 'Super!',
            content: "Datos Guardados !",
            buttons: {
                Continuar: {
                    btnClass: 'btn-success btn2',
                    action: function () {
                        DatosClienteDIAN = [];
                    }
                }
            }
        });
    }

}

//METODO IMPRIME Y PAGA FACTURA
function PagarFactura() {
    var Imprime;
    cargando();
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-question',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'orange',
        title: 'Vale !',
        content: '<br/> <label><input type="checkbox" name="CheckEnvDian" Value="SI"> Desea guardar borrador y NO enviar a la DIAN ? </label><br> ',
        buttons: {
            Continuar: {
                btnClass: 'btn btn-warning btn2',
                action: function () {
                    DatosClienteDIAN = [];
                    BorradorDian = $('input:checkbox[name=CheckEnvDian]:checked').val();
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-money',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'orange',
                        title: 'Medio de pago !',
                        content: 'Seleccione el medio de pago <br/> <label><input type="checkbox" name="Check1" Value="SI"> Desea Imprimir factura ? </label><br> ',
                        buttons: {
                            Tarjeta: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    Imprime = $('input:checkbox[name=Check1]:checked').val();
                                    PagosDIAN = [];
                                    DatosClienteDIAN = [];
                                    $.alert({
                                        theme: 'Modern',
                                        icon: 'fa fa-credit-card',
                                        boxWidth: '700px',
                                        useBootstrap: false,
                                        type: 'orange',
                                        title: '# Valores Aprobación !',
                                        content: 'Digite el numero de Aprobacion del Voucher, Valor y Tipo de Pago' +
                                            '<br/><br/>' +
                                            '<div>' +
                                            'Pagos Agregados:<br/><table id="InfoMetodosPago" style="width: 60%; margin-left: 20%; font-weight: 700;"> </table>' +
                                            '<br/><table id="TablePagoTarjeta"><td style="padding: 5px;"><input style="" id="numAprobacionVoucher" type="text" class="form-control input-sm" onkeypress = "return soloNum(event)" autocomplete="off" placeholder="# Voucher" /></td>' +
                                            '<td style="padding: 5px;"><input style="" id="ValorVoucher" type="text" class="form-control input-sm" onkeypress = "return soloNum(event)" autocomplete="off" placeholder="Valor Transaccion"/></td>' +
                                            '<td style="padding: 5px;"><select id="OpcionPago" class="form-select"><option value="">** Tipo Pago **</option ><option value="Debito">Tarjeta Debito</option><option value="Credito">Tarjeta Credito</option></select></td></table>' +
                                            '</div>' +
                                            '<label style="cursor: pointer; font-weight: bold;"><input type="checkbox" id="AgregaPagoCredit" onchange="AgregaPagoT()" > Desea Agregar Pago ? </label> <br/><br/>',
                                        buttons: {
                                            Continuar: {
                                                btnClass: 'btn btn-warning',
                                                action: function () {
                                                    var totalSuma = 0;
                                                    for (let i = 0; i < PagosDIAN.length; i++)
                                                        totalSuma += PagosDIAN[i].value;
                                                    if (PagosDIAN.length > 0 && totalSuma == parseInt($('#Total').val())) {
                                                        if (Imprime == "SI")
                                                            connectPSR.server.imprimirFactura($("#ID_MESA").val());
                                                        DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                                        DatosClienteDIAN.push(token);
                                                        connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), $("#OBSERVACIONES").val(),
                                                            $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(), "FINALIZADA", $("#ID_MESA").val(),
                                                            $("#servicio").val(), "TARJETA", PagosDIAN, "0", $("#ID_MESERO").val(),
                                                            DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"), $("#Total").val(), BorradorDian, "0");
                                                        connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");
                                                    }
                                                    else {
                                                        PagarFactura();
                                                    }
                                                }
                                            },
                                            Cancelar: {
                                                btnClass: 'btn btn-warning CierraPago',
                                                action: function () {
                                                    PagosDIAN = [];
                                                    cerrar();
                                                }
                                            }
                                        }
                                    });
                                }
                            },
                            Efectivo: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    Imprime = $('input:checkbox[name=Check1]:checked').val();
                                    DatosClienteDIAN = [];
                                    $.alert({
                                        theme: 'Modern',
                                        icon: 'fa fa-check',
                                        boxWidth: '500px',
                                        useBootstrap: false,
                                        type: 'orange',
                                        title: ':) Confirmación !',
                                        content: 'Esta a punto de pagar la cuenta y salir, seguro que desea continuar ?',
                                        buttons: {
                                            Continuar: {
                                                btnClass: 'btn btn-warning',
                                                action: function () {
                                                    if (Imprime == "SI")
                                                        connectPSR.server.imprimirFactura($("#ID_MESA").val());
                                                    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                                    DatosClienteDIAN.push(token);
                                                    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                                                        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                                                        "FINALIZADA", $("#ID_MESA").val(), $("#servicio").val(), "EFECTIVO", PagosDIAN, $("#SubTotal").val(),
                                                        $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
                                                        $('#DianSistema').prop("checked"), $("#Total").val(), BorradorDian, "0");
                                                    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");
                                                }
                                            },
                                            Cancelar: {
                                                btnClass: 'btn btn-warning',
                                                action: function () {
                                                    cerrar();
                                                }
                                            }
                                        }
                                    });
                                }
                            },
                            Ambas: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    Imprime = $('input:checkbox[name=Check1]:checked').val();
                                    DatosClienteDIAN = [];
                                    $.alert({
                                        theme: 'Modern',
                                        icon: 'fa fa-money',
                                        boxWidth: '500px',
                                        useBootstrap: false,
                                        type: 'orange',
                                        title: '$ Efectivo !',
                                        content: 'Digite la cantidad en efectivo <br/> <div><input style="witdh:60%; margin-left: 20%;" id="cantEfectivo" type="text" class="form-control input-sm" onchange="validaValor()" onkeypress = "return soloNum(event)" required /></div>',
                                        buttons: {
                                            Continuar: {
                                                btnClass: 'btn btn-warning',
                                                action: function () {
                                                    var cantEfectivo = $("#cantEfectivo").val();
                                                    PagosDIAN = [];
                                                    if (cantEfectivo != "") {
                                                        $.alert({
                                                            theme: 'Modern',
                                                            icon: 'fa fa-credit-card',
                                                            boxWidth: '700px',
                                                            useBootstrap: false,
                                                            type: 'orange',
                                                            title: '# Valores Aprobación !',
                                                            content: 'Digite el numero de Aprobacion del Voucher, Valor y Tipo de Pago' +
                                                                '<br/><br/>' +
                                                                '<div>' +
                                                                'Pagos Agregados:<br/><table id="InfoMetodosPago" style="width: 60%; margin-left: 20%; font-weight: 700;"> </table>' +
                                                                '<br/><table id="TablePagoTarjeta"><td style="padding: 5px;"><input style="" id="numAprobacionVoucher" type="text" class="form-control input-sm" onkeypress = "return soloNum(event)" autocomplete="off" placeholder="# Voucher" /></td>' +
                                                                '<td style="padding: 5px;"><input style="" id="ValorVoucher" type="text" class="form-control input-sm" onkeypress = "return soloNum(event)" autocomplete="off" placeholder="Valor Transaccion"/></td>' +
                                                                '<td style="padding: 5px;"><select id="OpcionPago" class="form-select"><option value="">** Tipo Pago **</option ><option value="Debito">Tarjeta Debito</option><option value="Credito">Tarjeta Credito</option></select></td></table>' +
                                                                '</div>' +
                                                                '<label style="cursor: pointer; font-weight: bold;"><input type="checkbox" id="AgregaPagoCredit" onchange="AgregaPagoT()" > Desea Agregar Pago ? </label> <br/><br/>',
                                                            buttons: {
                                                                Continuar: {
                                                                    btnClass: 'btn btn-warning',
                                                                    action: function () {
                                                                        var totalSuma = 0;
                                                                        for (let i = 0; i < PagosDIAN.length; i++)
                                                                            totalSuma += PagosDIAN[i].value;
                                                                        var sumaParcial = parseInt($('#Total').val()) - cantEfectivo;
                                                                        if (PagosDIAN.length > 0 && totalSuma == sumaParcial) {
                                                                            if (Imprime == "SI")
                                                                                connectPSR.server.imprimirFactura($("#ID_MESA").val());
                                                                            DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                                                            DatosClienteDIAN.push(token);
                                                                            connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                                                                                $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                                                                                "FINALIZADA", $("#ID_MESA").val(), $("#servicio").val(), "AMBAS", PagosDIAN,
                                                                                cantEfectivo, $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
                                                                                $('#DianSistema').prop("checked"), $("#Total").val(), BorradorDian, "0");
                                                                            connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");
                                                                        }
                                                                        else {
                                                                            PagarFactura();
                                                                        }
                                                                    }
                                                                },
                                                                Cancelar: {
                                                                    btnClass: 'btn btn-warning CierraPago',
                                                                    action: function () {
                                                                        PagosDIAN = [];
                                                                        cerrar();
                                                                    }
                                                                }
                                                            }
                                                        });
                                                    }
                                                    else {
                                                        PagarFactura();
                                                    }
                                                }
                                            },
                                            Cancelar: {
                                                btnClass: 'btn btn-warning',
                                                action: function () {
                                                    PagosDIAN = [];
                                                    cerrar();
                                                }
                                            }
                                        }
                                    });
                                }
                            },
                            Cancelar: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    PagosDIAN = [];
                                    cerrar();
                                }
                            }
                        }
                    });
                }
            },
            Cancelar: {
                btnClass: 'btn btn-warning',
                action: function () {
                    cerrar();
                }
            }
        }
    });

}

//METODO IMPRIME FACTURA NADA MAS
function GeneraFactura() {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-list-alt',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'gray',
        title: 'Factura !',
        content: 'Desea imprimir la factura ?',
        buttons: {
            Si: {
                btnClass: 'btn btn-default btn2',
                action: function () {
                    //IMPRIME FACTURA
                    connectPSR.server.imprimirFactura($("#ID_MESA").val());
                }
            },
            Cancelar: {
                btnClass: 'btn btn-default',
                action: function () {

                }
            },
        }
    });
}

//METODO CANCELA PEDIDO
function CancelaPedido() {
    cargando();
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-question',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'red',
        title: 'Cancelar !',
        content: 'Desea cancelar todo el pedido ?',
        buttons: {
            Si: {
                btnClass: 'btn btn-danger',
                action: function () {
                    DatosClienteDIAN = [];
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-times',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'red',
                        title: 'Cancelar !',
                        content: 'Desea retornar todos los productos al inventario ?',
                        buttons: {
                            Si: {
                                btnClass: 'btn btn-danger',
                                action: function () {
                                    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                                        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                                        "CANCELA PEDIDO", $("#ID_MESA").val(), $("#servicio").val(), "N/A", "", "0", $("#ID_MESERO").val(),
                                        DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), false,
                                        $("#Total").val(), BorradorDian, "0");
                                    connectPSR.server.cancelaPedido($("#ID").val(), true);
                                    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");

                                }
                            },
                            No: {
                                btnClass: 'btn btn-danger',
                                action: function () {
                                    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                    DatosClienteDIAN.push(token);
                                    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                                        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                                        "CANCELA PEDIDO", $("#ID_MESA").val(), $("#servicio").val(), "N/A", "", "0", $("#ID_MESERO").val(),
                                        DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), false,
                                        $("#Total").val(), BorradorDian, "0");
                                    connectPSR.server.cancelaPedido($("#ID").val(), false);
                                    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "SI", "../Solicitud/SeleccionarMesa");

                                }
                            },
                        }
                    });
                }
            },
            Cancelar: {
                btnClass: 'btn btn-danger',
                action: function () {

                }
            },
        }
    });
}

//METODO DE LLEVAR ORDEN
function AsignarLlevar() {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-arrow-up',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'orange',
        title: 'Llevar Orden !',
        content: 'Desea convertir la orden para llevar ?',
        buttons: {
            Si: {
                btnClass: 'btn btn-warning btn2',
                action: function () {
                    DatosClienteDIAN = [];
                    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                        "LLEVAR", $("#ID_MESA").val(), $("#servicio").val(), "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN,
                        $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
                    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "ESPERA", User, "NO", "");
                }
            },
            Cancelar: {
                btnClass: 'btn btn-warning',
                action: function () {

                }
            },
        }
    });
}

//METODO DE ASIGNAR ORDEN
function AsignarAsignaMesa() {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-arrow-down',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'green',
        title: 'Asignar Orden !',
        content: 'Desea asignar la orden a la mesa ?',
        buttons: {
            Si: {
                btnClass: 'btn btn-success btn2',
                action: function () {
                    DatosClienteDIAN = [];
                    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                        "ABIERTA", $("#ID_MESA").val(), $("#servicio").val(), "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN,
                        $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
                    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "OCUPADO", User, "NO", "");
                }
            },
            Cancelar: {
                btnClass: 'btn btn-success',
                action: function () {

                }
            },
        }
    });
}

//METODO BOTON CONSUMO INTERNO
function ConsumoInterno() {
    if ($("#CCCliente").val() != "") {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-users',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'blue',
            title: 'Consumo Interno !',
            content: 'Desea pasar esta cuenta a consumo interno ?',
            buttons: {
                Si: {
                    btnClass: 'btn btn-primary btn2',
                    action: function () {
                        DatosClienteDIAN = [];
                        DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                        connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                            $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                            "CONSUMO INTERNO", $("#ID_MESA").val(), $("#servicio").val(), "N/A", "", "0", $("#ID_MESERO").val(),
                            DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"),
                            $("#Total").val(), "", "0");
                        connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "NO", "");
                        $.alert({
                            theme: 'Modern',
                            icon: 'fa fa-users',
                            boxWidth: '500px',
                            useBootstrap: false,
                            type: 'blue',
                            title: 'Inhabilitar Mesa !',
                            content: 'Para ser redireccionado de clic en Continuar !',
                            buttons: {
                                Continuar: {
                                    btnClass: 'btn btn-primary btn2',
                                    action: function () {
                                        connectPSR.server.listarEstadoMesas("SI", $("#ID_MESA").val(), "../Solicitud/SeleccionarMesa");
                                    }
                                }
                            }
                        });
                    }
                },
                Cancelar: {
                    btnClass: 'btn btn-primary',
                    action: function () {

                    }
                },
            }
        });
    }
    else {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-warning',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Consumo Interno !',
            content: 'Debe digitar un numero de cedula del cliente para pasarlo como consumo interno !',
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {

                    }
                }
            }
        });
    }

}

//METODO PARA INHABILITAR MESA
function InhabilitarMesa() {
    if (ProductosSolicitudVector.length == 0 && $("#Total").val() == "0") {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'default',
            title: 'Inhabilitar Mesa !',
            content: 'Desea Inhabilitar esta mesa ?',
            buttons: {
                Si: {
                    btnClass: 'btn btn-default btn2',
                    action: function () {
                        DatosClienteDIAN = [];
                        DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                        connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
                            $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
                            "INHABILITAR", $("#ID_MESA").val(), $("#servicio").val(), "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN,
                            $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
                        connectPSR.server.actualizaMesa($("#ID_MESA").val(), "NO DISPONIBLE", User, "NO", "");
                        connectPSR.server.listarEstadoMesas("SI", $("#ID_MESA").val(), "../Solicitud/SeleccionarMesa");
                    }
                },
                Cancelar: {
                    btnClass: 'btn btn-default',
                    action: function () {

                    }
                },
            }
        });
    }
    else {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Inhabilitar Mesa Prohibido !',
            content: 'Ud. ya no puede inhabilitar esta mesa. Revise con el Cajero/Administrador',
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                    }
                }
            }
        });
    }

}

//METODOS PARA HACER CAMBIO DE MESA
function CargaMesas() {
    connectPSR.server.listarEstadoMesas("NO", 0, "");
}
function CierraModalCM() {
    $("#ListaMesas").empty();
}
function CambioMesa(id, Estado) {
    DatosClienteDIAN = [];
    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), $("#OBSERVACIONES").val(),
        $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(), $("#ESTADO_SOLICITUD").val(), id, $("#servicio").val(),
        "", "", "0", $("#ID_MESERO").val(), DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(),
        $('#DianSistema').prop("checked"), $("#Total").val(), "", "0");
    connectPSR.server.actualizaMesa(id, Estado, User, "NO", "");
    connectPSR.server.actualizaMesa($("#ID_MESA").val(), "LIBRE", User, "NO", "");
    connectPSR.server.actualizaIdmesaHTML(id, $("#ID_MESA").val());
    Encriptar(id).then(r => {
        connectPSR.server.listarEstadoMesas("SI", id, "../Solicitud/Pedido?Id=" + encodeURIComponent(r));
    }).catch(() => {
        console.log('error');
    });

}


//FUNCIONES ADICIONALES
function soloNum(e) {
    var key = window.Event ? e.which : e.keyCode
    return (key >= 48 && key <= 57)
}
function menos() {
    if ($("#contador").val() > 1) {
        $("#contador").val(Number($("#contador").val()) - 1);
    }
}
function mas() {
    if ($("#contador").val() < 10) {
        $("#contador").val(Number($("#contador").val()) + 1);
    }
}
function menosServicio() {
    var numActual = Math.trunc($("#servicio").val());
    if (numActual > 0)
        $("#servicio").val(numActual - 1);
    else
        $("#servicio").val(numActual);
}
function masServicio(porcentajeMaximo) {
    var numActual = Math.trunc($("#servicio").val());
    if (numActual < porcentajeMaximo)
        $("#servicio").val(numActual + 1);
    else
        $("#servicio").val(numActual);

}
function menosServicioDC(subtotal) {
    var numActual = Math.trunc($("#servicioDC").val());
    if (numActual > 0) {
        $("#servicioDC").val(numActual - 1);
        $("#servicioDigDC").val(((numActual - 1) * subtotal) / 100);
    }
    else {
        $("#servicioDC").val(numActual);
        $("#servicioDigDC").val(((numActual) * subtotal) / 100);
    }

}
function masServicioDC(porcentajeMaximo, subtotal) {
    var numActual = Math.trunc($("#servicioDC").val());
    if (numActual < porcentajeMaximo) {
        $("#servicioDC").val(numActual + 1);
        $("#servicioDigDC").val(((numActual + 1) * subtotal) / 100);
    }
    else {
        $("#servicioDC").val(numActual);
        $("#servicioDigDC").val(((numActual) * subtotal) / 100);
    }


}
function Encriptar(texto) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: "POST",
            url: urlEncriptar,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ Texto: texto }),
            dataType: "JSON",
            success: function (result) {
                var json = JSON.parse(result);
                if (json != "") {
                    resolve(json);
                }
                else {
                    reject();
                }

            },
            error: function (request, status, error) {
                console.log(error);
            }

        });
    })
}
function CambiaMesero(idMesero) {
    DatosClienteDIAN = [];
    DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
    connectPSR.server.guardaDatosCliente($("#ID").val(), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(),
        $("#OBSERVACIONES").val(), $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SubTotal").val(),
        $("#ESTADO_SOLICITUD").val(), $("#ID_MESA").val(), $("#servicio").val(), "", "", "0", idMesero,
        DatosClienteDIAN, $("#ID_CLIENTE").val(), $("#FACTURACION_ELECTRONICA").val(), $('#DianSistema').prop("checked"),
        $("#Total").val(), "", "0");
}
function AgregaProductosPedido() {
    if ($('#ID_PRODUCTO').val() != "" && $('#PRECIO_PRODUCTO').val() != "") {
        $("#AgregaPedido").attr("disabled", "disabled");
        let descripcion;
        if ($('#Adiciones').val() != "")
            descripcion = $('#Adiciones').val().toUpperCase();
        else
            descripcion = "";
        var model = {
            ID: parseInt($("#contador").val()),
            ID_SOLICITUD: $('#ID').val(),
            ID_PRODUCTO: $('#ID_PRODUCTO').val(),
            ID_MESERO: User,
            PRECIO_PRODUCTO: $('#PRECIO_PRODUCTO').val(),
            DESCRIPCION: descripcion,
            ESTADO_PRODUCTO: $('#NOMBREPRODUCTO').val()
        };
        ProductosPedido.push(model);
        $("#setProductosElegidos").empty();
        for (var i = 0; i < ProductosPedido.length; i++) {
            $("#setProductosElegidos").append("<tr><td>" + ProductosPedido[i].ID + "</td><td>" + ProductosPedido[i].ESTADO_PRODUCTO + "</td><td>" + ProductosPedido[i].DESCRIPCION +
                "</td ><td><i class=\"fa fa-2x fa-minus-square\" style=\"color: #a90000; cursor: pointer; \" onclick=\"EliminaProductoLista('" + i + "')\"></i></td></tr> ");
        }
        //console.log(ProductosPedido);
        CargaProducto(IdProd);
        $("#AgregaPedido").removeAttr("disabled");
    }
    else {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Oops',
            content: 'Debe seleccionar un producto antes de agregar a pedido',
            buttons: {
                Ok: {
                    btnClass: 'btn-danger btn2',
                    action: function () {

                    }
                }
            }
        });
    }
}
function EliminaProductoLista(idElemento) {
    ProductosPedido.splice(idElemento, 1);
    $("#setProductosElegidos").empty();
    for (var i = 0; i < ProductosPedido.length; i++) {
        $("#setProductosElegidos").append("<tr><td>" + ProductosPedido[i].ID + "</td><td>" + ProductosPedido[i].ESTADO_PRODUCTO + "</td><td>" + ProductosPedido[i].DESCRIPCION +
            "</td ><td><i class=\"fa fa-2x fa-minus-square\" style=\"color: #a90000; cursor: pointer; \" onclick=\"EliminaProductoLista('" + i + "')\"></i></td></tr> ");
    }
    //console.log(ProductosPedido);
}
function ValidarValores(subTotal, servicioTot) {
    var propinaMaxima = (parseInt(subTotal) * 10) / 100;

    if ($("#servicioDig").val() > propinaMaxima) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Ops!',
            content: "El valor maximo de propina es <b>" + propinaMaxima + "</b>",
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                        $("#servicioDig").val(servicioTot)
                    }
                }
            }
        });
    }
    else if ($("#servicioDig").val() < 0) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Ops!',
            content: "El valor no puede ser negativo",
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                        $("#servicioDig").val(servicioTot)
                    }
                }
            }
        });
    }
    else {
        var calculo = (parseInt($("#servicioDig").val()) * 100) / (parseInt($("#SubTotal").val()));
        $("#servicio").val(calculo);
    }
}
function ValidarValoresDC(subTotal, servicioTot) {
    var propinaMaxima = (parseInt(subTotal) * 10) / 100;

    if ($("#servicioDigDC").val() > propinaMaxima) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Ops!',
            content: "El valor maximo de propina es <b>" + propinaMaxima + "</b>",
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                        $("#servicioDigDC").val(servicioTot)
                    }
                }
            }
        });
    }
    else if ($("#servicioDigDC").val() < 0) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Ops!',
            content: "El valor no puede ser negativo",
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                        $("#servicioDigDC").val(servicioTot)
                    }
                }
            }
        });
    }
    else {
        var calculo = (parseInt($("#servicioDigDC").val()) * 100) / (parseInt($("#SubTotal").val()));
        $("#servicioDC").val(calculo);
    }
}
function validaValor() {
    var cantDigitada = $("#cantEfectivo").val();
    if (cantDigitada > (parseInt($("#Total").val())) - 1) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Ops!',
            content: "El valor no puede ser mayor o igual al total de la cuenta",
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-danger btn2',
                    action: function () {
                        $("#cantEfectivo").val('');
                    }
                }
            }
        });

    }
}

function EnviaDian(valor) {
    if (valor) {
        //$("#SecciondianPersona").empty(); 

    }
    else {
        $('#PersonaDian option:eq(0)').prop('selected', true);
        $("#TipoDocumentoDian option:eq(0)").prop("selected", true);
        $("#CCCliente").val("0")
        $('#TelefonoDian').val("");
        $('#CorreoClienteDian').val("");
        $("#TipoPersonaDian option:eq(0)").prop("selected", true);
        $('#O-13').prop("checked", false);
        $('#O-15').prop("checked", false);
        $('#O-23').prop("checked", false);
        $('#O-47').prop("checked", false);
        $('#R-99-PN').prop("checked", true);
        $("#SecciondianPersona").empty();
        $("#ID_CLIENTE").val("0");
        $("#FACTURACION_ELECTRONICA").val("0");
        $('#CDigitVerif').val("");
    }
}
function CambiaPersona(valor) {
    if (valor == "Person") {
        $("#SecciondianPersona").empty();
        $("#SecciondianPersona").append('<p><b>Nombres: </b><input id="NombreCliente" type="text" autocomplete="off" class="form-control" name="NombreCliente"/></p > ' +
            '<p><b>Apellidos: </b><input id="ApellidosCliente" type="text" autocomplete="off" class="form-control" name="NombreCliente" /></p>' +
            '<p><b>Nombre Comercial: </b><input id="NComercialCliente" type="text" autocomplete="off" class="form-control" name="NComercialCliente" /></p>' +
            '<p><b>Direccion (Incluir Ciudad): </b><input id="DireccionCliente" type="text" autocomplete="off" class="form-control" name="DireccionCliente" /></p>');
    }

    if (valor == "Company") {
        $("#SecciondianPersona").empty();
        $("#SecciondianPersona").append('<p><b>Razon Social: </b><input id="RazonSocialCliente" type="text" autocomplete="off" class="form-control" name="RazonSocialCliente"/></p > ' +
            '<p><b>Nombre Comercial: </b><input id="NComercialCliente" type="text" autocomplete="off" class="form-control" name="NComercialCliente" /></p>' +
            '<p><b>Direccion (Incluir Ciudad): </b><input id="DireccionCliente" type="text" autocomplete="off" class="form-control" name="DireccionCliente" /></p>');
    }

    if (valor == "") {
        $("#SecciondianPersona").empty();
    }
}
function ConsultaClienteDian(valor) {
    if (valor != "null" && valor != "0" && $('#EnvioDian').prop("checked")) {
        $.ajax({
            type: "POST",
            url: urlConsultaClienteDian,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ Cedula: valor }),
            dataType: "JSON",
            success: function (result) {
                var json = JSON.parse(result);
                if (json != null) {
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-user',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'green',
                        title: 'Usuario Encontrado!',
                        content: "Se ha encontrado el usuario " + json.NUMERO_IDENTIFICACION + ", se cargara automaticamente !",
                        buttons: {
                            OK: {
                                btnClass: 'btn-success btn2',
                                action: function () {
                                    $('#PersonaDian option[value="' + json.TIPO_PERSONA + '"]').prop('selected', true);
                                    CambiaPersona(json.TIPO_PERSONA);
                                    $('#TipoDocumentoDian option[value="' + json.CODIGO_DOCUMENTO + '"]').prop('selected', true);
                                    $("#CCCliente").val(json.NUMERO_IDENTIFICACION);
                                    $('#CDigitVerif').val(json.DIGITO_VERIFI);
                                    $("#TelefonoDian").val(json.TELEFONO);
                                    $("#CorreoClienteDian").val(json.EMAIL);
                                    $('#TipoPersonaDian option[value="' + json.RESPONSABLE_IVA + '"]').prop('selected', true);
                                    var RFiscals = json.CODIGO_R_FISCAL.split(";");
                                    for (var i = 0; i < RFiscals.length; i++) {
                                        $('#' + RFiscals[i]).prop("checked", true);
                                    }
                                    $("#NombreCliente").val(json.NOMBRES);
                                    $("#ApellidosCliente").val(json.APELLIDOS);
                                    $("#RazonSocialCliente").val(json.RAZON_SOCIAL);
                                    $("#NComercialCliente").val(json.NOMBRE_COMERCIAL);
                                    $("#DireccionCliente").val(json.DIRECCION);
                                    $("#ID_CLIENTE").val(json.ID);
                                    $("#FACTURACION_ELECTRONICA").val("1");
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
function AgregaPagoT() {
    if ($("#numAprobacionVoucher").val() != "" && $("#OpcionPago").val() != "" && $("#ValorVoucher").val() != "") {
        var model = {
            id: $("#numAprobacionVoucher").val(),
            name: $('#OpcionPago').val(),
            value: parseInt($('#ValorVoucher').val())
        };
        PagosDIAN.push(model);
        $("#InfoMetodosPago").empty();
        for (var i = 0; i < PagosDIAN.length; i++) {
            $("#InfoMetodosPago").append("<tr><td style='padding: 10px;'>#" + PagosDIAN[i].id + "</td><td style='padding: 10px;color: green;'>$" + PagosDIAN[i].value + "</td><td style='padding: 10px;'>" + PagosDIAN[i].name + "</td></tr>");
        }
        $("#numAprobacionVoucher").val("");
        $("#OpcionPago").val("");
        $("#ValorVoucher").val("");
        $('#AgregaPagoCredit').prop("checked", false);
        //console.log(PagosDIAN);
    }
    else {
        $(".CierraPago").click();
        PagarFactura();
    }

}

function ListaProductosDC() {
    $("#setProductosDC").empty();
    for (var i = 0; i < ProductosSolicitudVector.length; i++) {
        var code = '';
        var color = '#a90000';

        /*code = '<i class="fa fa-2x fa-minus-square" style="color: #a90000; cursor:pointer;" onclick="CancelaProductoxId(' + data[0].ProductosSolicitud[i].Id + ',' + data[0].ProductosSolicitud[i].Id + ')"></i>' +
            '<i id="' + descripcion + '" class="fa fa-2x fa-print" style="color: ' + color + '; cursor:pointer; margin-left: 5px;" onclick="ReEnviaProducto(' + data[0].ProductosSolicitud[i].IdProducto + ', this.id, ' + data[0].IdMesa + ')"></i >';
        */
        $("#setProductosDC").append('<tr>' +
            '<td style="text-align: center;">' +
            '<input class="form-check-input" style="border-color: black;" id=' + ProductosSolicitudVector[i].Id + ' type="checkbox" >' +
            '</<td>' +
            '<td>' +
            ProductosSolicitudVector[i].NombreProducto +
            '</<td>' +
            '<td>' +
            ProductosSolicitudVector[i].Descripcion +
            '</<td>' +
            '<td>' +
            ProductosSolicitudVector[i].PrecioProducto +
            '</<td>' +
            '</tr>');
        //console.log(ProductosSolicitudVector[i]);
    }
}
function DivideCuentaProceso() {
    cargando();
    //ATRAPA PRODUCTOS A DIVIDIR
    var table = document.getElementById('ProductosDC');
    var inputCheck = table.querySelectorAll('input[type="checkbox"]');
    var productosTemp = [];
    for (var i = 0; i < inputCheck.length; i++) {
        if (inputCheck[i].checked) {
            var model = {
                ID: ProductosSolicitudVector.find((p) => p.Id == inputCheck[i].id).Id,
                PRECIO_PRODUCTO: ProductosSolicitudVector.find((p) => p.Id == inputCheck[i].id).PrecioProducto,
            };
            productosTemp.push(model);
        }
    }
    if (productosTemp.length > 0) {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-list-alt',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'gray',
            title: 'Dividir Cuenta !',
            content: 'Esta seguro que desea dividir la cuenta con los productos seleccionados ? <br/> Por favor validar bien ;) ',
            buttons: {
                Si: {
                    btnClass: 'btn btn-default btn2',
                    action: function () {

                        //CALCULOS DE SERVICIO
                        var sumaProductos = 0;
                        for (var i = 0; i < productosTemp.length; i++) {
                            sumaProductos += productosTemp[i].PRECIO_PRODUCTO;
                        }
                        var servicioTotalDC = (sumaProductos * 10) / 100;

                        $.alert({
                            theme: 'Modern',
                            icon: 'fa fa-list-alt',
                            boxWidth: '500px',
                            useBootstrap: false,
                            type: 'gray',
                            title: 'Servicio !',
                            content: 'Desea agregar servicio ? <br/><br/> ' +
                                '<table style="width: 100%;"><td>' +
                                '<small><b>Servicio (' + DatosSolicitud[0].PorcentajeServicio + '% Máx.) :<br/>' +
                                '<span class="input-group-btn" style="float: left; margin-left: 35%;">' +
                                '<button class="btn btn-success" id="menosServicioDC" type="button" onclick="menosServicioDC(' + sumaProductos + ')"><b>-</b></button>' +
                                '</span>' +
                                '<input type="text" style="width:62px;text-align: center; float: left; margin-left: 2%;" id="servicioDC" class="form-control" value="' + DatosSolicitud[0].PorcentajeServicio + '" readonly />' +
                                '<span class="input-group-btn" style="float: left; margin-left: 2%;">' +
                                '<button class="btn btn-success" id="masServicioDC" type="button" onclick="masServicioDC(' + DatosSolicitud[0].Impuestos[2].Porcentaje + ', ' + sumaProductos + ')"><b>+</b></button>' +
                                '</span>' +
                                /*'<br/><br/><small><b>Digitar valor (Opcional):<b/><small/>' +*/
                                '<br/><br/><input type="text" " style="background-color: #30a630c7; font-size: 24px; color: white; font-weight: bolder; margin-left: 20%; text-align: center;" ' +
                                'id="servicioDigDC" class="form-control" value="' + servicioTotalDC + '" onkeypress = "return soloNum(event)" />' + //onchange="ValidarValoresDC(' + sumaProductos + ', ' + servicioTotalDC + ')"
                                '</td></table>',
                            buttons: {
                                Continuar: {
                                    btnClass: 'btn btn-default btn2',
                                    action: function () {
                                        //DIVIDE CUENTA
                                        connectPSR.server.divideCuenta(DatosSolicitud[0].Id, $("#ID_MESERO").val(), productosTemp, $("#servicioDC").val());
                                    }
                                },
                                Cancelar: {
                                    btnClass: 'btn btn-default',
                                    action: function () {
                                        cerrar();
                                    }
                                },
                            }
                        });
                    }
                },
                Cancelar: {
                    btnClass: 'btn btn-default',
                    action: function () {
                        cerrar();
                    }
                },
            }
        });
    }
    else {
        $.alert({
            theme: 'Modern',
            icon: 'fa fa-times',
            boxWidth: '500px',
            useBootstrap: false,
            type: 'red',
            title: 'Dividir Cuenta !',
            content: 'Seleccione primero como minimo un producto',
            buttons: {
                Continuar: {
                    btnClass: 'btn btn-default',
                    action: function () {
                        cerrar();
                    }
                },
            }
        });
    }

}

function PagarFacturaDC() {
    cargando();
    var subt = $('#SUBTOTAL_DIVIDIDA').val();
    var porcservdv = $('#PORC_SERVICIO_DIVIDIDA').val();
    var totaldivid = $('#TOTAL_DIVIDIDA').val();
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-money',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'orange',
        title: 'Medio de pago !',
        content: 'Seleccione el medio de pago -> Cuenta Dividida <br/>' +
            '<label style="font-weight: bolder; color: #ff781a; font-size: 20px;"> Subtotal: $' + subt + ' </label><br/>' +
            '<label style="font-weight: bolder; color: #ffc91a; font-size: 18px;"> Imp. Consumo: $' + (parseInt(subt * 8 / 100)) + ' </label> <br/>' +
            '<label style="font-weight: bolder; color: gray; text-decoration: underline; font-size: 14px;"> Datafono: $' + (parseInt(subt * 8 / 100) + parseInt(subt)) + ' </label> <br/>' +
            '<label style="font-weight: bolder; color: #ffc91a; font-size: 18px;"> Servicio: $' + parseInt(subt * porcservdv / 100) + '</label> <br/>' +
            '<label style="font-weight: bolder; color: #005c30; font-size: 22px;"> TOTAL: $' + totaldivid + ' </label> <br/>',
        buttons: {
            Tarjeta: {
                btnClass: 'btn btn-warning',
                action: function () {
                    PagosDIAN = [];
                    DatosClienteDIAN = [];
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-credit-card',
                        boxWidth: '700px',
                        useBootstrap: false,
                        type: 'orange',
                        title: 'Tipo Pago !',
                        content: 'Seleccione el tipo de pago !<br/><br/>' +
                            '<div><select id="OpcionPagoDC" class="form-select" style="margin-left: 27%;"><option value="">** Tipo Pago **</option ><option value="Debito">Tarjeta Debito</option><option value="Credito">Tarjeta Credito</option></select></div>',
                        buttons: {
                            Continuar: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    if ($('#OpcionPagoDC').val() != "") {
                                        cargando();
                                        DatosClienteDIAN.push("false");
                                        DatosClienteDIAN.push(token);
                                        var model = {
                                            id: "0000",
                                            name: $('#OpcionPagoDC').val(),
                                            value: parseInt(totaldivid)
                                        };
                                        PagosDIAN.push(model);
                                        connectPSR.server.guardaDatosCliente(Number($("#ID_SOLICITUD_DIVIDIDA").val()), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), "CUENTA DIVIDIDA POR EL CLIENTE",
                                            $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SUBTOTAL_DIVIDIDA").val(), "FINALIZADA", "999999",
                                            $("#PORC_SERVICIO_DIVIDIDA").val(), "TARJETA", PagosDIAN, "0", $("#ID_MESERO").val(),
                                            DatosClienteDIAN, $("#ID_CLIENTE").val(), "0", true, $("#TOTAL_DIVIDIDA").val(), "SI", $("#ID_MESA").val());
                                    }
                                    else {
                                        PagarFacturaDC();
                                    }
                                }
                            }
                        }
                    });
                }
            },
            Efectivo: {
                btnClass: 'btn btn-warning',
                action: function () {
                    DatosClienteDIAN = [];
                    PagosDIAN = [];
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-check',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'orange',
                        title: ':) Confirmación !',
                        content: 'Seguro que desea enviar el pago con EFECTIVO ?',
                        buttons: {
                            Continuar: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    cargando();
                                    DatosClienteDIAN.push("false");
                                    DatosClienteDIAN.push(token);
                                    connectPSR.server.guardaDatosCliente(Number($("#ID_SOLICITUD_DIVIDIDA").val()), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), "CUENTA DIVIDIDA POR EL CLIENTE",
                                        $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SUBTOTAL_DIVIDIDA").val(), "FINALIZADA", "999999",
                                        $("#PORC_SERVICIO_DIVIDIDA").val(), "EFECTIVO", PagosDIAN, $("#SUBTOTAL_DIVIDIDA").val(), $("#ID_MESERO").val(),
                                        DatosClienteDIAN, $("#ID_CLIENTE").val(), "0", true, $("#TOTAL_DIVIDIDA").val(), "SI", $("#ID_MESA").val());
                                }
                            }
                        }
                    });
                }
            },
            Ambas: {
                btnClass: 'btn btn-warning',
                action: function () {
                    DatosClienteDIAN = [];
                    PagosDIAN = [];
                    $.alert({
                        theme: 'Modern',
                        icon: 'fa fa-money',
                        boxWidth: '500px',
                        useBootstrap: false,
                        type: 'orange',
                        title: '$ Efectivo !',
                        content: 'Digite la cantidad en efectivo <br/> <div><input style="witdh:60%; margin-left: 20%;" id="cantEfectivoDC" type="text" class="form-control input-sm" onchange="validaValor()" onkeypress = "return soloNum(event)" required /></div>',
                        buttons: {
                            Continuar: {
                                btnClass: 'btn btn-warning',
                                action: function () {
                                    var cantEfectivoDC = $("#cantEfectivoDC").val();
                                    if (cantEfectivoDC != "" && cantEfectivoDC < totaldivid) {
                                        $.alert({
                                            theme: 'Modern',
                                            icon: 'fa fa-credit-card',
                                            boxWidth: '700px',
                                            useBootstrap: false,
                                            type: 'orange',
                                            title: '# Valores Aprobación !',
                                            content: 'Seleccione el Tipo de Pago' +
                                                '<br/><br/>' +
                                                '<div>' +
                                                '<select id="OpcionPagoDC" class="form-select" style="margin-left: 27%;"><option value="">** Tipo Pago **</option ><option value="Debito">Tarjeta Debito</option><option value="Credito">Tarjeta Credito</option></select></td></table>' +
                                                '</div>' +
                                                '<br/><br/>',
                                            buttons: {
                                                Continuar: {
                                                    btnClass: 'btn btn-warning',
                                                    action: function () {
                                                        if ($('#OpcionPagoDC').val() != "") {
                                                            cargando();
                                                            var model = {
                                                                id: "0000",
                                                                name: $('#OpcionPagoDC').val(),
                                                                value: parseInt($('#TOTAL_DIVIDIDA').val() - cantEfectivoDC)
                                                            };
                                                            PagosDIAN.push(model);
                                                            DatosClienteDIAN.push($('#EnvioDian').prop("checked"));
                                                            DatosClienteDIAN.push(token);
                                                            connectPSR.server.guardaDatosCliente(Number($("#ID_SOLICITUD_DIVIDIDA").val()), $("#CCCliente").val(), $("#NombreCliente").val() + " " + $("#ApellidosCliente").val(), "CUENTA DIVIDIDA POR EL CLIENTE",
                                                                $("#OtrosCobros").val(), $("#Descuentos").val(), $("#SUBTOTAL_DIVIDIDA").val(), "FINALIZADA", "999999",
                                                                $("#PORC_SERVICIO_DIVIDIDA").val(), "AMBAS", PagosDIAN, cantEfectivoDC, $("#ID_MESERO").val(),
                                                                DatosClienteDIAN, $("#ID_CLIENTE").val(), "0", true, $("#TOTAL_DIVIDIDA").val(), "SI", $("#ID_MESA").val());
                                                        }
                                                        else {
                                                            PagarFacturaDC();
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        $.alert({
                                            theme: 'Modern',
                                            icon: 'fa fa-times',
                                            boxWidth: '500px',
                                            useBootstrap: false,
                                            type: 'red',
                                            title: 'Ops!',
                                            content: "El valor no puede ser vacio, mayor o igual al total de la cuenta",
                                            buttons: {
                                                Continuar: {
                                                    btnClass: 'btn btn-danger btn2',
                                                    action: function () {
                                                        PagarFacturaDC();
                                                    }
                                                }
                                            }
                                        });

                                    }
                                }
                            }
                        }
                    });
                }
            }
        }

    });
}

function ReImprimeProductosMasivo(idproducto, description, idmesa) {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-print',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'orange',
        title: ' Re - Imprimir !',
        content: 'Desea imprimir todos los productos o solo los faltantes ?',
        buttons: {
            Todos: {
                btnClass: 'btn btn-default btn2',
                action: function () {
                    connectPSR.server.imprimeProductosMasivo(DatosSolicitud[0].ProductosSolicitud, DatosSolicitud[0].IdMesa);
                }
            },
            Faltantes: {
                btnClass: 'btn btn-default btn2',
                action: function () {
                    const prdoFaltantes = DatosSolicitud[0].ProductosSolicitud.filter(item => item.EstadoProducto === "NO ENTREGADO");
                    if (prdoFaltantes.length > 0)
                        connectPSR.server.imprimeProductosMasivo(prdoFaltantes, DatosSolicitud[0].IdMesa);
                }
            },
            Cancelar: {
                btnClass: 'btn btn-default',
                action: function () {

                }
            }
        }
    });

}