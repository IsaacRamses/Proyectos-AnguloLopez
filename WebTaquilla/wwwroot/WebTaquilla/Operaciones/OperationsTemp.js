$(document).ready(function () {
    SearchOperationsTempPending();
});

function SearchOperationsTempPending() {

    var urlFinal = GlobalVariables.SearchOperationsTempPending;

    $('#tblOperationsTemp').bootstrapTable('refresh', {
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
    }).on('load-success.bs.table', function (index, element, row) {
        Swal.close();
    }).on('load-error.bs.table', function (index, element, row) {
        Swal.close();
    });
}


function responseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });
    return res;
};

function runningFormatter(value, row, index) {
    return index + 1;
}

function operateFormatter(value, row, index) {
    return [
        '<button type="button" class="btn btn-primary hability"><i class="fa fa-eye hvr-icon"></i></button>'
    ].join('')
}

window.operateEvents = {
    'click .hability': function (e, value, row, index) {
        UpdateStatusOperationsTempEntity(row.Id_OPERACION);
    },
}

function UpdateStatusOperationsTempEntity(id_operacion) {
    $(".hability").attr("disabled", "disabled");
    var model = {
        Id_OPERACION: id_operacion,
        Status: GlobalConstant.PendienteDeCobro
    };
    window.utils.LoaderNotConfirm("Espere por Favor....");
    GlobalApi.ExecuteSimple2("POST", GlobalVariables.UpdateStatusOperationsTempEntity, model, function (result) {
        Swal.close();
        $(".hability").removeAttr('disabled');
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        Swal.fire(
            {
                title: "Operación Exitosa!!!",
                text: "Se ha procesado de manera exitosa.",
                icon: "success",
            });

        $('#tblOperationsTemp').bootstrapTable('refresh')
    });
}
