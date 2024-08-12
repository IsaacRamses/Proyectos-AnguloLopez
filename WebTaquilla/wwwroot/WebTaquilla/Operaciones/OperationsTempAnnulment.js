$(document).ready(function () {
    InitForm();
});

function InitForm() {

    $("#formOperationsTempAnnulmentRequest").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'clienteTipo': {
                validators: {
                    notEmpty: {
                        message: 'El tipo de Cliente es requerido'
                    }
                }
            },
            'Number': {
                validators: {
                    notEmpty: {
                        message: 'El número de identificación es requerido'
                    }
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
    });

    $("#formAnnulment").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'ListMotivosAnulacion': {
                validators: {
                    callback: {
                        message: 'El Motivo de la Anulacion es requerido',
                        callback: function (input) {
                            return ValidateSelect(input);
                        },
                    }
                }
            },
            'AnnulObservacion': {
                validators: {
                    notEmpty: {
                        message: 'La observacion de la anulacion es requerido'
                    }
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
    });

}

function ValidateSelect(value) {
    if (value != '' && value != 0 && value != null) {
        return true;
    }
    return false;
};

function SearchOperationsTempAnnulment() {

    if ($("#clienteTipo").val() == "" || $("#Number").val() == "") {
        // Hide the loader
        $('#formOperationsTempAnnulmentRequest').formValidation('revalidateField', "clienteTipo");
        $('#formOperationsTempAnnulmentRequest').formValidation('revalidateField', "Number");
        return;
    }
    else {

        var url = "?CIREM=" + $("#clienteTipo").val() + $("#Number").val();
       
        window.utils.showLoading('Consultando Información... Por favor espere...');
       
        var urlFinal = GlobalVariables.SearchOperationsTempAnnulment + '/' + url;

        $('#TbAvailableOperationsTempAnnulment').bootstrapTable('refresh', {
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
    }
}

function responseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });
    return res;
};

function OperacionDescripcion(value, row) {
    if (row.Tipo_Operacion != GlobalConstant.Venta && row.Tipo_Operacion != GlobalConstant.Compra) {
        return row.CORRESPONSAL;
    }
    return row.DetalleOperacion;
}

function OperacionMontoRecibir(value, row) {
    var monto = (formatCurrencyVE(row.Total_Cobrar) + ' ' + row.MonedaConSymb);
    if (row.Tipo_Operacion != GlobalConstant.Venta) { 
        monto = (formatCurrency(row.Dolares) + ' ' + row.MonedaSymb);
        return monto;
    }
    return monto;
}

function OperacionMontoPagar(value, row) {
    var monto = formatCurrency(row.Dolares) + ' ' + row.MonedaSymb;

    if (row.Tipo_Operacion != GlobalConstant.Venta && row.Tipo_Operacion == GlobalConstant.Compra) {
        monto = (formatCurrencyVE(row.Total_Cobrar) + ' ' + row.MonedaConSymb);
        return monto;
    }
    return monto;
}

function operateFormatter(value, row, index) {
    var action = [];
            action.push(
                '<button class="Rejected btn btn-danger btn-xs" title="Rechazar">',
                '<i class="fa fa-minus-circle"></i>',
                '</button>&nbsp;'
            );
    return action.join('');
}

window.operateEvents = {
    'click .Rejected': function (e, value, row, index) {
        ProcesedOperation(row);
    },
};

function ProcesedOperation(row) {
    window.utils.showLoading('Por favor espere...');
    $("#IdOrden").val(row.Id_OPERACION);
    $("#IdOperacion").val(row.Id_RemesaEntrante);
    $("#StatusOperacionId").val(row.Status);
    $("#ListMotivosAnulacion").val('0');
    window.utils.hideLoading();
    $("#modalOperation").modal("show");
}

function SaveAnnulment() {
    window.utils.showLoading('Por favor espere...');

    if ($("#ListMotivosAnulacion").val() == 0 || $("#AnnulObservacion").val() == '')
    {
        $('#formAnnulment').formValidation('revalidateField', "ListMotivosAnulacion");
        $('#formAnnulment').formValidation('revalidateField', "AnnulObservacion");
        window.utils.hideLoading();
    }
    else
    {

        var model = {
            TableId: GlobalConstant.OperacionesPorCobrar,
            RowId: $("#IdOrden").val(),
            StatusRowId: $("#StatusOperacionId").val(),
            ReasonAnnulmentId: $("#ListMotivosAnulacion option:selected").val(),
            AnnulmentObservation: $("#AnnulObservacion").val(),
        }

        GlobalApi.ExecuteSimple(GlobalVariables.InsertAnnulment, model, function (result) {
            window.utils.hideLoading();
            if (result.Error) {
                Swal.fire(
                    {
                        title: "Error",
                        text: (result.ErrorMessage),
                        icon: "error",
                    });
                return;
            }


            Swal.fire(
                {
                    title: "Operación Exitosa!",
                    text: "Se ha generado la solicitud de Anulacion exitosamente.",
                    icon: "success",
                }
            );
            $("#modalOperation").modal("hide")
            CleanInput();
            $('#TbAvailableOperationsTempAnnulment').bootstrapTable('refresh');
        });

    }
   
}

function CleanInput() {
    $("#IdIdOrden").val("");
    $("#IdOperacion").val("");
    $("#StatusOperacionId").val("");
    $("#ListMotivosAnulacion").val('0');
    $("#AnnulObservacion").val("");

}