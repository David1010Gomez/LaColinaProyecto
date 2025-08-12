$(document).ready(function () {
    $(".TablaList").DataTable({
        "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json",
    });
});

function AsignarDia(idnomina) {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-calendar',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'orange',
        title: 'Fecha trabajada !',
        content: 'Por favor seleccione una fecha de trabajo: <br/><label><input id="FechaTrabajada" type="date" class="form-control input-sm"></label><br>',
        buttons: {
            Continuar: {
                btnClass: 'btn btn-warning btn2',
                action: function () {
                    if ($("#FechaTrabajada").val() != "") {
                        const fechaTrabaj = $("#FechaTrabajada").val();
                        $.alert({
                            theme: 'Modern',
                            icon: 'fa fa-money',
                            boxWidth: '500px',
                            useBootstrap: false,
                            type: 'orange',
                            title: 'Valor día !',
                            content: 'Por favor digite el sueldo del día: <br/><label><input id="SueldoDiarioI" type="number" class="form-control input-sm"></label><br>',
                            buttons: {
                                Continuar: {
                                    btnClass: 'btn btn-warning btn2',
                                    action: function () {
                                        if ($("#SueldoDiarioI").val() != "") {
                                            const sueldoDiarioI = $("#SueldoDiarioI").val();
                                            $.alert({
                                                theme: 'Modern',
                                                icon: 'fa fa-Question',
                                                boxWidth: '500px',
                                                useBootstrap: false,
                                                type: 'orange',
                                                title: 'Cargo del dia !',
                                                content: 'Por favor, seleccione el cargo del dia: <br/><select id="CargoDia" class="form-control input-sm" style="margin-left: 20%;"><option value="" >-- Seleccione --</option><option value="3" > MESERO </option><option value="4" > PARRILLA / COCINA / BAR </option><option value="5" > OTROS </option></select>',
                                                buttons: {
                                                    Continuar: {
                                                        btnClass: 'btn btn-warning btn2',
                                                        action: function () {
                                                            if ($("#CargoDia").val() != "") {
                                                                $.ajax({
                                                                    type: "POST",
                                                                    url: urlAsignarDiaTrabajo,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    data: JSON.stringify({ IdNomina: idnomina, FechaTrabajada: fechaTrabaj, SueldoDiario: sueldoDiarioI, IdPerfil: $("#CargoDia").val() }),
                                                                    dataType: "JSON",
                                                                    success: function (result) {
                                                                        var json = JSON.parse(result);
                                                                        if (json == true) {
                                                                            $.alert({
                                                                                theme: 'Modern',
                                                                                icon: 'fa fa-check',
                                                                                boxWidth: '500px',
                                                                                useBootstrap: false,
                                                                                type: 'green',
                                                                                title: 'Exitoso !',
                                                                                content: 'Trabajador asignado a fecha seleccionada',
                                                                                buttons: {
                                                                                    Continuar: {
                                                                                        btnClass: 'btn btn-success btn2',
                                                                                        action: function () {
                                                                                            location.reload();
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
                                                                                title: 'Error Actualización !',
                                                                                content: 'La fecha seleccionada puede que ya se haya ingresado',
                                                                                buttons: {
                                                                                    Si: {
                                                                                        btnClass: 'btn btn-danger btn2',
                                                                                        action: function () {
                                                                                            location.reload();
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
                                                            else {
                                                                AsignarDia(idnomina);
                                                            }
                                                        }
                                                    },
                                                    Cancelar: {
                                                        btnClass: 'btn btn-warning btn2',
                                                        action: function () {

                                                        }
                                                    }
                                                }
                                            });
                                        }
                                        else {
                                            AsignarDia(idnomina);
                                        }
                                    }
                                },
                                Cancelar: {
                                    btnClass: 'btn btn-warning btn2',
                                    action: function () {

                                    }
                                }
                            }
                        });
                    }
                    else {
                        AsignarDia(idnomina);
                    }
                }
            },
            Cancelar: {
                btnClass: 'btn btn-warning btn2',
                action: function () {

                }
            }
        }
    });

}

function CalcularSueldo(idUsuarioNomina) {
    $.ajax({
        type: "POST",
        url: urlCalcularPago,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ IdNomina: idUsuarioNomina }),
        dataType: "JSON",
        success: function (result) {
            var json = JSON.parse(result);
            if (json == true) {
                $.alert({
                    theme: 'Modern',
                    icon: 'fa fa-check',
                    boxWidth: '500px',
                    useBootstrap: false,
                    type: 'green',
                    title: 'Exitoso !',
                    content: 'Calculo exitoso',
                    buttons: {
                        Continuar: {
                            btnClass: 'btn btn-success btn2',
                            action: function () {
                                location.reload();
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
                    title: 'Error Calculando !',
                    content: 'Hubo un error inesperado, por favor intentelo nuevamente',
                    buttons: {
                        Si: {
                            btnClass: 'btn btn-danger btn2',
                            action: function () {
                                location.reload();
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
function Liquidar(idUsuarioNomina) {
    $.ajax({
        type: "POST",
        url: urlLiquidarUsuario,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ IdNomina: idUsuarioNomina }),
        dataType: "JSON",
        success: function (result) {
            var json = JSON.parse(result);
            if (json == true) {
                $.alert({
                    theme: 'Modern',
                    icon: 'fa fa-check',
                    boxWidth: '500px',
                    useBootstrap: false,
                    type: 'green',
                    title: 'Exitoso !',
                    content: 'Trabajador Liquidado',
                    buttons: {
                        Continuar: {
                            btnClass: 'btn btn-success btn2',
                            action: function () {
                                location.reload();
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
                    title: 'Error Liquidando !',
                    content: 'Hubo un error inesperado, por favor intentelo nuevamente',
                    buttons: {
                        Si: {
                            btnClass: 'btn btn-danger btn2',
                            action: function () {
                                location.reload();
                            }
                        }
                    }
                });
            }
        },        
        Cancelar: {
            btnClass: 'btn btn-warning btn2',
            action: function () {

            }
        }
    });
}

function PropinasTotal() {
    $.alert({
        theme: 'Modern',
        icon: 'fa fa-money',
        boxWidth: '500px',
        useBootstrap: false,
        type: 'blue',
        title: 'Propinas !',
        content: 'Por favor seleccione una fecha inicial y final: <br/><label><input id="FechaIniPropinas" type="date" class="form-control input-sm"></label>'+
                    '<br/><label><input id="FechaFinPropinas" type="date" class="form-control input-sm"></label><br>',
        buttons: {
            Continuar: {
                btnClass: 'btn btn-primary btn2',
                action: function () {
                    if ($("#FechaIniPropinas").val() != "" && $("#FechaFinPropinas").val() != "" && ($("#FechaIniPropinas").val() < $("#FechaFinPropinas").val())) {
                        const fechaInicial = $("#FechaIniPropinas").val();
                        const fechaFinal = $("#FechaFinPropinas").val();
                        $("#Fecha1").val(fechaInicial);
                        $("#Fecha2").val(fechaFinal);
                        $("#EnviaDescargaPropinas").click();

                    }
                    else {
                        $.alert({
                            theme: 'Modern',
                            icon: 'fa fa-times',
                            boxWidth: '500px',
                            useBootstrap: false,
                            type: 'red',
                            title: 'Error Fechas !',
                            content: 'No pueden ser vacias las fechas y/o la fecha inicial debe ser menor a la fecha final',
                            buttons: {
                                Continuar: {
                                    btnClass: 'btn btn-danger btn2',
                                    action: function () {
                                        PropinasTotal();
                                    }
                                }
                            }
                        });
                    }
                    
                }
            },
            Cancelar: {
                btnClass: 'btn btn-secondary btn2',
                action: function () {

                }
            }
        }
    });
    
}