$(document).ready(function () {
    InitForm();
});

function InitForm() {

    $("#formCashierAnnulmentRequest").formValidation({
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

function SearchCashierOperationsAnnulment() {

    var IdOrden = 0;
    if ($("#clienteTipo").val() == "" || $("#Number").val() == "") {
        $('#formCashierAnnulmentRequest').formValidation('revalidateField', "clienteTipo");
        $('#formCashierAnnulmentRequest').formValidation('revalidateField', "Number");
        return;
    }
    else {

        if ($("#IdOrden").val().trim() != '') {
            IdOrden = $("#IdOrden").val();
        }
        var url = "?CIREM=" + $("#clienteTipo").val() + $("#Number").val() + "&ID_ORDEN=" + IdOrden ;

        window.utils.showLoading('Consultando Información... Por favor espere...');

        var urlFinal = GlobalVariables.SearchCashierAnnulment + '/' + url;

        $('#TbAvailableOperationsAnnulment').bootstrapTable('refresh', {
            url: urlFinal,
            type: 'GET',
            striped: false,
            pagination: true,
            showFooter: false,
            showRefresh: true,
            search: false,


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
        ProcesedOperation(row, 'rejected');
    },
};

function ProcesedOperation(row) {
    window.utils.showLoading('Por favor espere...');
    $("#IdOrden").val(row.ID_ORDEN);
    $("#IdOperacion").val(row.ID_OPERACION);
    $("#StatusOrderId").val(row.STATUS_ORDEN);
    $("#StatusOperacionId").val(row.STATUS_OPERACION);
    $("#TableId").val(row.TableId);
    $("#ListMotivosAnulacion").val('0');
    window.utils.hideLoading();
    $("#modalOperation").modal("show");
}

function SaveAnnulment() {
    window.utils.showLoading('Por favor espere...');

    if ($("#ListMotivosAnulacion").val() == 0 || $("#AnnulObservacion").val() == '') {
        $('#formAnnulment').formValidation('revalidateField', "ListMotivosAnulacion");
        $('#formAnnulment').formValidation('revalidateField', "AnnulObservacion");
        window.utils.hideLoading();
    }
    else {

        var RowId = $("#IdOrden").val();
        var StatusRowId = $("#StatusOrderId").val();
        var TableId = $("#TableId").val()
        //if (TableId != GlobalConstant.Ordenes ) {

        //    RowId = $("#IdOperacion").val();
        //    StatusRowId = $("#StatusOperacionId").val();
        //}

        var model = {
            TableId: TableId,
            RowId: RowId,
            StatusRowId: StatusRowId,
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
            $('#TbAvailableOperationsAnnulment').bootstrapTable('refresh');
        });

    }

}

function CleanInput() {
    $("#IdOrden").val("");
    $("#IdOperacion").val("");
    $("#StatusOrderId").val("");
    $("#StatusOperacionId").val("")
    $("#TableId").val("");
    $("#ListMotivosAnulacion").val('0');
    $("#AnnulObservacion").val("");
}