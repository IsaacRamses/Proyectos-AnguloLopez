var RowAnnulment = null;

$(document).ready(function () {
    SearchAnnulmentPending();
});

function SearchAnnulmentPending() {

    window.utils.showLoading('Consultando Información... Por favor espere...');

    var model = { BranchOfficeId: 0 }
    GlobalApi.ExecuteSimple(GlobalVariables.SearchAnnulmentPending, model, function (result) {
        window.utils.hideLoading();
        if (result.length == 0) {
            Swal.fire(
                {
                    title: "No se encontro información",
                    text: "No se han encontrado solicitudes de Anulaciones pendientes por verificar",
                    icon: "info",
                });
            $('#TbAvailableAnnulment').bootstrapTable('removeAll')

            return;
        } else {
            $('#TbAvailableAnnulment').bootstrapTable('load', result).on('click-row.bs.table', function (index, row, element) {
                LoadDetailAnnulment(row);
            });
        }
    });

}

function RefreshTableAnnulment() {
    SearchAnnulmentPending();
}

function FechaHora(value, row) {
    var FechaHora = ((convertDate(row.CreationDate)) + ' ' + (hourUTC(row.CreationDate)));
    return FechaHora;
}

function LoadDetailAnnulment(row) {
    window.utils.showLoading('Por favor espere...');
    $("#NomRem").val(row.NOMBRE_REMITENTE);
    $("#Cirem").val(row.CIREM);
    $("#Operacion").val(row.OPERACION);
    $("#Monto").val(row.MONTO_DIVISAS +'  '+ row.SimMoneda );
    $("#ReasonAnnulment").val(row.ReasonAnnulment);
    $("#AnnulmentObservation").val(row.AnnulmentObservation);

    window.utils.hideLoading();
    RowAnnulment = row;
    $("#modalDetailAnnulment").modal("show");
}

function AproveAnnulment(Validate) {
    window.utils.showLoading('Por favor espere...');
    var model = RowAnnulment;
    var Observacion = model.AnnulmentObservation + ' - ' + $("#Observacion").val();
    model.AnnulmentObservation = Observacion;
    if (Validate) {
        model.StatusId = GlobalConstant.AnnulAprobada;
    } else {
        model.StatusId = GlobalConstant.AnnulRechazada;
    }

    GlobalApi.ExecuteSimple(GlobalVariables.ValidateAproveAnnulment, model, function (result) {
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
                    text: "Se ha procesado la solicitud de Anulacion exitosamente.",
                    icon: "success",
                }
        );
        $("#modalDetailAnnulment").modal("hide");
        CleanInput();
        SearchAnnulmentPending();
    });
}

function CleanInput() {
    $("#NomRem").val('');
    $("#Cirem").val('');
    $("#Operacion").val('');
    $("#Monto").val('');
    $("#ReasonAnnulment").val('');
    $("#AnnulmentObservation").val('');
    RowAnnulment = null;
}
