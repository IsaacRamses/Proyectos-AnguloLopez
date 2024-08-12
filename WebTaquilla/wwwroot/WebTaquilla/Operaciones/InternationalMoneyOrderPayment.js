var f = $("#frmAccountPayment");
$(document).ready(function () {
    SearchPaymentOrderVentanilla();

    $('#AccountPaymentBank').select2({
        language: 'es',
        dropdownAutoWidth: true,
        placeholder: 'Seleccione'
    });

    $("#btnConfirmar").on("click", function () {
        ModalConfirmation(true);
    });

    $("#btnRechazar").on("click", function () {
        ModalConfirmation(false);
    });

    $('#AccountPayment').mask('000.000.000.000.000.000,00', { reverse: true });
});

function SearchPaymentOrderVentanilla() {
    window.utils.showLoading('Consultando Información... Por favor espere...');
    var url = getFilterFichas();
    var urlFinal = GlobalVariables.SearchInternationalMoneyOrderPayment + '/' + url;

    $('#PaymentOrderVentanilla').bootstrapTable('refresh', {
        url: urlFinal,
        type: 'GET',
        striped: false,
        pagination: true,
        showFooter: false,
        showRefresh: true,
        search: false,
        showExport: true,
        exportTypes: ['excel'],
        exportOptions: {
            fileName: 'custom_file_name'
        }
    }).on('expand-row.bs.table', function (index, element, row) {
    });
    window.utils.hideLoading();
};

function getFilterFichas() {
    var url = '';

    if ($("#Number").val() !== "" && $("#Number").val() !== undefined) {

        if (url === "") {
            url += "?IDENTIFICACION_REMITENTE=" + $("#Number").val();
        }
        else {
            url += "&IDENTIFICACION_REMITENTE=" + $("#Number").val();
        }
    }

    return url;
};

function responseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });
    return res;
};

function operateFormatter(value, row, index) {
    return [
        '<button type="button" class="btn btn-primary ver"><i class="fa fa-eye hvr-icon"></i></button>'
    ].join('')
}

function FormatMonto(value, row) {
    symbolCurrency = row.simboloMonedaPago;
    let monto = row.simbolo + " " + formatCurrencyVE(row.monto);
    return monto;
}

window.operateEvents = {
    'click .ver': function (e, value, row, index) {

        if (row.statusFicha != "Activo") {
            Swal.fire(
                {
                    title: "Error",
                    text: "La ficha se encuentra inactiva",
                    icon: "error",
                });
            return;
        }

        $("#DetalleRemesa").addClass("active");
        $("#BitacoraRemesa").removeClass("active");

        $('#nombre').val(row.nombres_remitente);
        $('#numeroidentificacion').val(row.identificacion_cliente);
        $('#nombreRemitente').val(row.nombres_benef);
        $('#nameStatusFicha').val(row.statusFicha.toUpperCase());
        $('#fechaVencimientoCI').val(convertDate(row.FechaVencimientoCI));

        InitFormVentanilla();
        $('#AccountTotalAmmountDetail').val(row.simbolo + " " + formatCurrencyVE(row.monto));
        $("#AmountPayment").val(formatCurrencyVE(row.monto));
        $("#Tipo_Operation").val("ventanilla")
        $('.banco').css("display", "none");
        $("#MONEDA").val("VEB")

        $("#ID_ORDEN").val(row.id_orden);
        $("#ID_SOLICITUD").val(row.id_detalle_operacion);
        $("#NUMERO").val(row.numero_orden);
        $("#id_corresponsal").val(row.id_corresponsal);
        $("#REFERENCIA_ORDEN").val(row.referencia);
        $("#FECHA_FACTURADO").val(row.aprobado);
        $("#FACTURADO_POR").val(row.observaciones);
        $("#MONTO").val(row.monto)
        SearchCallCenterLog(row.id_orden);

        $("#tabsPrincipal").hide("slow");
        $("#FilterPaymentOrder").hide("slow");
        $('#FormPaymentOrder').show("slow");
        $("#btnGenerateOrden").prop('disabled', false)
        if (row.idstatusFicha != GlobalConstant.FichaActivo ) {
            $("#btnGenerateOrden").prop('disabled',true)
        }

        if (row.FechaVencimientoCI == null || row.FechaVencimientoCI == '' ) {
            if (convertDate(row.FechaVencimientoCI) <= convertDate(new Date())) {
                $("#btnGenerateOrden").prop('disabled', true)
            }
        }
9
    },
}

function CleanFilters() {
    $("#OrdenID").val("");
    $("#Number").val("");
    $("#btnGenerateOrden").prop('disabled', false)
    SearchPaymentOrderVentanilla();
};

function Volver() {
    CleanFilters();
    $("#tabsPrincipal").show("slow");
    $("#FilterPaymentOrder").show("slow");
    $('#FormPaymentOrder').hide("slow");
    $("#btnGenerateOrden").prop('disabled', false)
}

function SearchCallCenterLog(RawId) {
    window.utils.showLoading('Consultando... Por favor espere...');

    var model = {
        RawId: RawId,
        TableId: 1,
    };

    GlobalApi.ExecuteSimple(GlobalVariables.SearchCallCenterLog, model, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $("#TableBitacoraOperations").empty();
        $.each(result, function (i, v) {
            $("#TableBitacoraOperations").append("<div class='chat-element'>" +
                "<a href='#' class='float-left'><img alt='image' class='rounded-circle' src='/images/corporativas/fotousuario.jpg'></a>" +
                "<div class='media-body'>" +
                "<strong>" + v.CreationUser + "</strong>" +
                "<p class='m-b-xs'><strong>Llamada: " + v.OperationName + "</strong></p>" +
                "<p class='m-b-xs'><strong>Duración: " + v.Diferencia + "</strong></p>" +
                "<p class='m-b-xs'>" + v.Observation + "</p>" +
                "<small class='text-muted'>" + convertDateTime(v.CreationDate) + "</small>" +
                "</div>" +
                "</div > ");
        });
    });

    window.utils.hideLoading();
};

function ValidateDate(campo) {
    var id = $(campo).attr('id');
    var Fecha = $('#' + id).val();
    if (isDate(Fecha) == false) {
        $('#' + id).val("");
        Swal.fire(
            {
                title: "Fecha",
                text: "La fecha ingresada es invalida.",
                icon: "error",
            });
    }
}

function isDate(txtDate) {
    var currVal = txtDate;
    if (currVal == '')
        return false;
    var rxDatePattern = /^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/;
    var dtArray = currVal.match(rxDatePattern);

    if (dtArray == null)
        return false;

    dtMonth = dtArray[3];
    dtDay = dtArray[5];
    dtYear = dtArray[1];

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

//function ChargePayment() {
//    $(".remove").attr("disabled", true).addClass("disabled btn-light").removeClass("btn-danger");
//    $('#AccountPaymentReference').val("");
//    $('#AccountPaymentDate').val("");
//    $('#AccounPaymentObservation').val("");
//    $("#AccountPaymentBank").val("").trigger('change');
//    $('#AccountPayment').val("");
//    f.formValidation('resetForm', true);
//    $('#DetailDataPayment').modal('show');
//}

function InitFormVentanilla() {

    f.formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'AccountPaymentReference': {
                validators: {
                    notEmpty: {
                        message: 'La referencia de pago es requerida'
                    }
                }
            },
            'AccountPaymentDate': {
                validators: {
                    notEmpty: {
                        message: 'La fecha de pago es requerida'
                    }
                }
            },
            'AccounPaymentObservation': {
                validators: {
                    notEmpty: {
                        message: 'La observación es requerida'
                    }
                }
            },
            'AccountPayment': {
                validators: {
                    notEmpty: {
                        message: 'El monto del pago es requerido '
                    },
                    callback: {
                        message: ' El monto ingresado no concuerda con el monto de la orden',
                        callback: function (input) {
                            return ValidateAmountPayment(input);
                        }
                    },
                }
            },
        }
    }).on('err.field.fv', function (e, data) {
        //Esto es para que el Boton de Submit no se Deshabilite
        if (data.fv.getSubmitButton()) {
            data.fv.disableSubmitButtons(false);
        }
    }).on('success.field.fv', function (e, data) {
        //Esto es para que el Boton de Submit no se Deshabilite
        if (data.fv.getSubmitButton()) {
            data.fv.disableSubmitButtons(false);
        }
    }).on('success.form.fv', function (e) {
        // Esto Previne el Submission
        e.preventDefault();
        $('frmAccountPayment').formValidation('revalidateField');
        var $form = $(e.target);
        var fv = $form.data('formValidation');
        SaveLot();
    });

}

function ValidateAmountPayment(value) {
    var amountOrder = $('#AmountPayment').val();
    if (value == amountOrder) {
        return true;
    }
    return false;
};

function GenerarOperacionTemp() {
    Loader("Espere por Favor....");
    var model = {
        Id_RemesaEntrante: $("#ID_ORDEN").val(),
    };


    GlobalApi.ExecuteSimple(GlobalVariables.InsertOperacionesPorCobrar, model, function (result) {
        Swal.close();
        if (result.error) {
            Swal.fire(
                {
                    title: "Error",
                    text: (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail),
                    icon: "error",
                });
            return;
        }
        Swal.fire(
            {
                title: "Operación Exitosa!!!",
                text: "Se ha generado la operacion de manera exitosa.",
                icon: "success",
            });
        Volver();
    });
}

function Loader(message) {
    return Swal.fire({
        title: message,
        html: 'Espere...',
        timer: 100000,
        timerProgressBar: true,
        onBeforeOpen: () => {
            Swal.showLoading()
            timerInterval = setInterval(() => {
                const content = Swal.getContent()
                if (content) {
                    const b = content.querySelector('b')
                    if (b) {
                        b.textContent = Swal.getTimerLeft()
                    }
                }
            }, 100)
        },
        onClose: () => {
            clearInterval(timerInterval)
        }
    });
}


