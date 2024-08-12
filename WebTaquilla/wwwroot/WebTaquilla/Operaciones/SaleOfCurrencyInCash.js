var ficha;
var ClientUniqueId = "";
var ClientType = "";
$(document).ready(function () {
    $('#montoOrden').mask('000.000.000.000.000.000,00', { reverse: true });
    $("#TasaCambio").val(parseFloat($("#TasaCambio").val().replace(',', '.')).toFixed(4).replace('.', ','))
    InitEvent();
    InitForm();
});
function smartwizard() {
    // SmartWizard initialize
    $('#smartwizard').smartWizard({
        selected: 0,
        theme: 'dots',
        justified: true,
        darkMode: false,
        autoAdjustHeight: true,
        cycleSteps: false,
        backButtonSupport: true,
        enableURLhash: false,
        transition: {
            animation: 'none',
            speed: '400',
            easing: ''
        },
        toolbarSettings: {
            toolbarPosition: 'bottom',
            toolbarButtonPosition: 'right',
            showNextButton: true,
            showPreviousButton: true,
            toolbarExtraButtons: []
        },
        anchorSettings: {
            anchorClickable: true,
            enableAllAnchors: false,
            markDoneStep: true,
            markAllPreviousStepsAsDone: true,
            removeDoneStepOnNavigateBack: false,
            enableAnchorOnDoneStep: true
        },
        keyboardSettings: {
            keyNavigation: true,
            keyLeft: [37],
            keyRight: [39]
        },
        lang: {
            next: 'Siguiente',
            previous: 'Anterior'
        },
        disabledSteps: [],
        errorSteps: [],
        hiddenSteps: []
    });

}

function InitEvent() {

    // Initialize the leaveStep event
    $("#smartwizard").on("leaveStep", function (e, anchorObject, currentStepIndex, nextStepIndex, stepDirection) {
        
        if (currentStepIndex == GlobalConstant.swSearchClient) {
            Loader("Espere por Favor....");
            Searchfichas(currentStepIndex);
            return ficha;
        }
        else if (currentStepIndex == GlobalConstant.swStatusClient && nextStepIndex == GlobalConstant.swOperationsData.Default) {
            Loader("Espere por Favor....");
            if ($('#StatusId').val() == GlobalConstant.FichaActivo) {
                Swal.close();
                return true;
            }
            else {
                Swal.close();
                window.utils.showMessageWarning("Estatus Cuenta", "El número de Identidad Consultado no posee una ficha activa");
                return false;
            }
        }
        else if (nextStepIndex == GlobalConstant.swSearchClient) {
            Loader("Espere por Favor....");
            CleanFilters();
            // Reset wizard
            $('#smartwizard').smartWizard("reset");

        }
        else if (currentStepIndex == GlobalConstant.swOperationsData.Default && nextStepIndex == GlobalConstant.swOperationsDetail.Default) {
            Loader("Espere por Favor....");
            if (ValidateRequestAmount(currentStepIndex)) {
                return SearchFinancialSummary(currentStepIndex);
            } else {
                return false;
            }
            
        }
        else if (currentStepIndex == GlobalConstant.swOperationsDetail.Default) {
            Loader("Espere por Favor....");
            $(".sw-btn-next").css("display", "");
            $(".Payment").remove();
        }
        else if (currentStepIndex != GlobalConstant.swOperationsData.Default && nextStepIndex == GlobalConstant.swOperationsDetail.Default) {
            Loader("Espere por Favor....");
            if ($("#montoCliente").val() == formatCurrencyVE($("#montoOrden").val())) {
                $(".sw-btn-next").css("display", "none");
                $(".sw-btn-next").after("<button class='btn btn-primary Payment' type='button' onclick='GenerateOrden()'>Generar Orden</button>")
                Swal.close();
            }
            else {
                return SearchFinancialSummary(currentStepIndex);
            }

        }
    });
}

function InitForm() {

    smartwizard();

    $("#FilterData").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
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
        //InsertPaymentTemp();
    });

    $("#DataForTheRequest").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'motivoOferta': {
                validators: {
                    notEmpty: {
                        message: 'El Motivo de la Operación es requerido'
                    }
                }
            },
            'MonedaVenta': {
                validators: {
                    callback: {
                        message: 'La Moneda de Compra es requerida',
                        callback: function (input) {
                            return ValidateSelectBack(input);
                        },
                    }
                }
            },
            'montoOrden': {
                validators: {
                    notEmpty: {
                        message: 'El Monto de Operación es requerido'
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
        //InsertPaymentTemp();
    });

}


function GetImage(path) {

    url = GlobalVariables.GetImage;
    var model = {
        path: path
    };

    GlobalApi.ExecuteAsync('get', url, model, "application/json; charset=utf-8", "json", function (result) {
        if (result.Error != undefined) {
            Swal.fire(
                {
                    title: "No se encontro información",
                    text: result.MessageError,
                    icon: "info",
                });
        }
        if (result != null || result != "") {
            $("#imfFoto").prop("src", result);
            $("#imfFotoAmpliada").prop("src", result);
        }
    })
}

function Searchfichas(currentStepIndex) {

    if (currentStepIndex == 0) {
        if ($("#tasaCambio").val() == '0,0') {
            // Hide the loader
            Swal.close();
            window.utils.showMessageWarning("Tasa de Cambio", "No hay tasa registrada para hacer calculos cambiarios");
            ficha = false;
            return;
        }
        else if ($("#clienteTipo").val() == "" || $("#Number").val() == "") {
            // Hide the loader
            Swal.close();
            $('#FilterData').formValidation('revalidateField', "clienteTipo");
            $('#FilterData').formValidation('revalidateField', "Number");
            ficha = false;
            return;
        }
        else {

            var model = {
                id_cedula: $("#Number").val(),
                clienteTipo: $("#clienteTipo").val(),
            }; 

            GlobalApi.ExecuteSimpleAsync(GlobalVariables.Searchfichas, model, function (result) {
                //window.utils.hideLoading();
                if (result.length > 0) {
                    if (result[0].error) {
                        // Hide the loader
                        Swal.close();
                        window.utils.showMessageError("Error", (result[0].clientErrorDetail + ". " + result[0].apiDetail + ". " + result[0].errorDetail));
                        ficha = false;
                        return;
                    }
                }

                if (result.length == 0) {
                    // Hide the loader
                    Swal.close();
                    window.utils.showMessageWarning("Consulta de Idetificación", "No se encontro ficha para este número de identificación suministrado");
                    ficha = false;
                    return;
                }
                else {
                    const d = new Date();
                    GetImage(result[0].Foto);
                    ParametrosOrden();
                    TiposMovimientos(result[0].ClienteTipo);
                    $('#cedulaBeneficiario').val(result[0].id_cedula);
                    $('#nombreBeneficiario').val(result[0].NombreCompleto);
                    $('#statusNameFicha').val(result[0].statusName);
                    $('#Fecha_Operacion').val(convertDate(d));
                    $('#StatusId').val(result[0].Estatus);
                    $('#TELREM').val(result[0].TelfMovil1);
                    $('#DIRDES').val(result[0].direccion);
                    ClientUniqueId = result[0].PersitentObjectId;
                    ClientType = result[0].ClienteTipo;
                    // Hide the loader
                    Swal.close();
                    ficha = true;
                    return;
                }
            });
        }
    }
};

function TiposMovimientos(TipoIdentidad) {

    var model = {
        idTipoIdentidad: TipoIdentidad,
        keyword: GlobalConstant.keywordVentaEfc,
    };

    GlobalApi.ExecuteSimple(GlobalVariables.TiposMovimientos, model, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $("#tipoId").val(result[0].ID_TIPO)
    });
}

function SearchFinancialSummary(currentStepIndex) {

            var IdMoneda = $("#MonedaVenta option:selected").val();
            var req = null;

            GlobalApi.ExecuteSimpleAsync(GlobalVariables.urlSearchMonedas, req, function (result)
            {
                if (result.error) {
                    // Hide the loader
                    Swal.close();
                    window.utils.showMessageError("Error", result.ErrorMessage);
                    return false;
                }

                result = result.filter(function (codmoneda) {
                    return (codmoneda.MonedaId == IdMoneda);
                });

                $('#CodMoneda').val(result[0].MonedaCodigoInt);
                return true;

            });
           
            var model = {
                moneda: $('#CodMoneda').val(),
                idPais: GlobalConstant.paisVzla,
                idCorresp: GlobalConstant.correspCCAL,
                MonedaOperacion: $("#MonedaVenta").val(),
                OperationType: GlobalConstant.OperationTypeVentaEfc,
                montoEnviar: $("#montoOrden").val(),
                MonedaConversion: GlobalConstant.Bolivar,
                tipoId: $("#clienteTipo").val(),
                tipoOperacion: $("#tipoId").val(),
                tasa: $("#tasaCambio").val(),
            };

            GlobalApi.ExecuteSimpleAsync(GlobalVariables.SearchFinancialSummary, model, function (result) {
                if (result.error) {
                    // Hide the loader
                    $('#smartwizard').smartWizard("loader", "hide");
                    window.utils.showMessageError("Error", (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail));
                    return false;
                }

                $("#montoCliente").val(result.SimbolConversion + " " + formatCurrencyVE(Math.round(result.AmmountConversion)));
                $('#MontoPagar').val(formatCurrencyVE(result.OperationAmmount));
                $("#ComisionOperacion").val(formatCurrencyVE(result.TotalCommissionBs));
                $('#montoOperacion').val(result.SimbolConversion + " " + formatCurrencyVE(result.AmmountConversionWithoutRate));
                $('#MontoTotal').val(result.SimbolConversion + " " + formatCurrencyVE(Math.round(result.AmmountConversion)));
                $('#TasaConversion').val(formatCurrencyVE(result.RateConversion));

                $(".sw-btn-next").css("display", "none");
                $(".sw-btn-next").after("<button class='btn btn-primary Payment' type='button' onclick='GenerateOrden()'>Generar Orden</button>")

                // Hide the loader
                Swal.close();
                return true;
            });

}

function ParametrosOrden() {

    GlobalApi.ExecuteSimple(GlobalVariables.ParametrosOrden, null, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }

        $.each(result, function (i, v) {
            if (v.que == GlobalConstant.MinVentEfec) {
                if (v.cuanto == null) {
                    $("#minOrden").val(100);
                    $("#montoOrden").prop('min', 1);
                }
                else {
                    $("#minOrden").val(v.cuanto);
                    $("#montoOrden").prop('min', v.cuanto);
                }
            }
            if (v.que == GlobalConstant.MaxVentEfec) {
                if (v.cuanto == null) {
                    $("#maxOrden").val(300);
                    $("#montoOrden").prop('max', 300);
                }
                else {
                    $("#maxOrden").val(v.cuanto);
                    $("#montoOrden").prop('max', v.cuanto);
                }
            }
        });
    });
}

function ValidateSelectBack(value) {

    if (value != '' && value != 0 && value != null) {
        return true;
    }
    return false;
};

function CleanFilters() {
    $("#Number").val("");
    $("#imfFotoAmpliada").val("");
    $("#imfFoto").val("");
    $("#cedulaBeneficiario").val("");
    $("#nombreBeneficiario").val("");
    $("#statusNameFicha").val("");
    $("#clienteTipo").val("");
    $("#montoCliente").val("");
    $('#MontoPagar').val("");
    $("#ComisionOperacion").val("");
    $('#montoOperacion').val("");
    $('#MontoTotal').val("");
    $('#TasaConversion').val("");
    ClientUniqueId = "";
    ClientType = "";

    $('#MonedaVenta').val(0);
    $('#motivoOferta').val("");
    $('#montoOrden').val("");
    $('#CodMoneda').val("");
};

function GenerateOrden() {
    Loader("Espere por Favor....");

    var tasa = $("#Tasa").val().replace('.', ',');

    var model = {
        FechaOperacion:$("#Fecha_Operacion").val(),
        TipoDocumentoREM:$("#clienteTipo").val(),
        CIREM: $("#Number").val(),
        ///////////////////////////////
        TipoDocumentoDES: "",
        CIDES: "",
        NOMDES: "",
        DIRDES: "",
        TELDES: "",
        EMAILDES: "",
        PAISDES: "",
        CIUDADDES:"", 
        OFICIANDES: "",
        Observacion: "",
        OperationType: 0,
        IdMonedaOperacion: $("#MonedaVenta option:selected").val(),
        montoOrden: $("#montoOrden").val(),
        IdMotivosOperacion: $("#motivoOferta  option:selected").val(),
        CodigoTipoOperacion: GlobalConstant.keywordVentaEfc, 
        TasaCambio: tasa,
        TasaConversion: $('#TasaConversion').val(),
        IdMonedaConversion: GlobalConstant.Bolivar,
        Comision: $("#ComisionOperacion").val(),
        MontoOperacion: $('#montoOperacion').val().replace('Bs', ''),
        MontoTotalCompra: $('#MontoTotal').val().replace('Bs', ''),
        MontoPagarCCAL: $('#MontoTotal').val().replace('Bs', '')
    };

    GlobalApi.ExecuteSimpleAsync(GlobalVariables.InsertOperacionesPorCobrar, model, function (result) {
        Swal.close();
        if (result.error) {
            Swal.fire(
                {
                    title: "Error",
                    text: (result.clientErrorDetail + ". " + result.apiDetail +". " + result.errorDetail),
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
        CleanFilters();
        $(".sw-btn-next").css("display", "");
        $(".Payment").remove();
        // Reset wizard
        $('#smartwizard').smartWizard("reset");

    });
}

function Loader(message) {
    return Swal.fire({
        title: message,
        html: 'Espere...',
        timer: 100000,
        timerProgressBar: true,
        showConfirmButton: false,
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

function ValidateRequestAmount(currentStepIndex) {

    var montoOrden = $('#montoOrden').val().replace(".", "").replace(",", ".").replace("-", ",");
    var minOrden = $('#minOrden').val();
    var maxOrden = $('#maxOrden').val();
    var resultBit = false;

    if (currentStepIndex == GlobalConstant.swOperationsData.Default || currentStepIndex == GlobalConstant.swStatusClient) {
        if ($("#motivoOferta").val() == "" || $("#MonedaVenta").val() == "0" || $("#MonedaVenta").val() == "" || $("#montoOrden").val() == "") {
            $('#DataForTheRequest').formValidation('revalidateField', "motivoOferta");
            $('#DataForTheRequest').formValidation('revalidateField', "MonedaVenta");
            $('#DataForTheRequest').formValidation('revalidateField', "montoOrden");
            // Hide the loader
            Swal.close();
            return resultBit;
        }
        else if (parseInt(montoOrden) < parseInt(minOrden) || parseInt(montoOrden) > parseInt(maxOrden)) {
            // Hide the loader
            Swal.close();
            window.utils.showMessageWarning("Monto Operación", "El monto ingresado no cumple con los limites establecidos ");
            return resultBit;
        }
        else {

                var model = {
                    ClientId: ClientUniqueId,
                    IdentificationType: ClientType,
                    Ammount: $('#montoOrden').val(),
                    CurrencyId: $("#MonedaVenta option:selected").val(),
                    OperationTypeId: GlobalConstant.Venta,
                };

                GlobalApi.ExecuteSimple(GlobalVariables.ValidateRequestAmount, model, function (result) {

                    if (result.Error) {
                        Swal.fire(
                            {
                                title: "Error",
                                text: result.ErrorMessage,
                                icon: "error",
                            });
                        return;
                    }

                    if (!result.Valid) {
                        Swal.fire(
                            {
                                title: "Error",
                                text: result.Message.replace("Usted", "El cliente"),
                                icon: "error",
                            });

                        resultBit = false;
                    } else {
                        resultBit =  true;
                    }
                });
             }
    }

    return resultBit;
}