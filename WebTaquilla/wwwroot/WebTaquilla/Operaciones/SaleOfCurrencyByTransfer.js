var ficha;
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

    $("#smartwizard").on("leaveStep", function (e, anchorObject, currentStepIndex, nextStepIndex, stepDirection) {

        if (currentStepIndex == GlobalConstant.swSearchClient) {
            Searchfichas(currentStepIndex);
            return ficha;
        }
        else if (currentStepIndex == GlobalConstant.swStatusClient && nextStepIndex == GlobalConstant.swBeneficiaryData) {
            if ($('#StatusId').val() == GlobalConstant.FichaActivo) {
                return true;
            }
            else {
                window.utils.showMessageWarning("Estatus Cuenta", "El número de Identidad Consultado no posee una ficha activa");
                return false;
            }
        }
        else if (nextStepIndex == GlobalConstant.swSearchClient) {
            CleanFilters();
            $('#smartwizard').smartWizard("reset");

        }
        else if (currentStepIndex == GlobalConstant.swBeneficiaryData && nextStepIndex == GlobalConstant.swDestinationData) {
            if ($('#BeneficiaryType').val() == "" || $('#cedulaBeneficiario').val() == "" || $('#nombreBeneficiario').val() == "" ||
                $('#EmailBeneficiario').val() == "" || $('#TelBeneficiaio').val() == "" || validateEmail($('#EmailBeneficiario').val()) == false) {
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "BeneficiaryType");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "cedulaBeneficiario");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "nombreBeneficiario");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "EmailBeneficiario");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "TelBeneficiaio");
                return false;
            }

        }
        else if (currentStepIndex == GlobalConstant.swDestinationData && nextStepIndex == GlobalConstant.swDestinationIntData) {
            if ($('#PaisDestino').val() == "" || $('#nombreBancoDestino').val() == "" || $('#numeroCuentaDestino').val() == "" ||
                $('#direccionBancoDestino').val() == "" || $('#aba').val() == "" || $('#swift').val() == "") {
                $('#DataTransfer').formValidation('revalidateField', "PaisDestino");
                $('#DataTransfer').formValidation('revalidateField', "nombreBancoDestino");
                $('#DataTransfer').formValidation('revalidateField', "numeroCuentaDestino");
                $('#DataTransfer').formValidation('revalidateField', "aba");
                $('#DataTransfer').formValidation('revalidateField', "swift");
                $('#DataTransfer').formValidation('revalidateField', "direccionBancoDestino");
                return false;
            }

        }
        else if (currentStepIndex == GlobalConstant.swDestinationIntData && nextStepIndex == GlobalConstant.swOperationsData.Transf) {
            if ($('#bancoIntermediario').val() == "" || $('#numeroCuentaIntermediario').val() == "" || $('#direccionBancoIntermediario').val() == "" ||
                $('#abaIntermediario').val() == "" || $('#swiftIntermediario').val() == "") {
                $('#DataBankIntermediary').formValidation('revalidateField', "bancoIntermediario");
                $('#DataBankIntermediary').formValidation('revalidateField', "numeroCuentaIntermediario");
                $('#DataBankIntermediary').formValidation('revalidateField', "swiftIntermediario");
                $('#DataBankIntermediary').formValidation('revalidateField', "abaIntermediario");
                $('#DataBankIntermediary').formValidation('revalidateField', "direccionBancoIntermediario");
                return false;
            }

        }
        else if (nextStepIndex == GlobalConstant.swSearchClient) {
            CleanFilters();
            // Reset wizard
            $('#smartwizard').smartWizard("reset");

        }
        else if (currentStepIndex == GlobalConstant.swOperationsData.Transf && nextStepIndex == GlobalConstant.swOperationsDetail.Transf) {
            return SearchFinancialSummary(currentStepIndex);
        }
        else if (currentStepIndex == GlobalConstant.swOperationsData.Transf) {
            $(".sw-btn-next").css("display", "");
            $(".Payment").remove();
        }
        else if (currentStepIndex != GlobalConstant.swOperationsData.Transf && nextStepIndex == GlobalConstant.swOperationsDetail.Transf) {
            if ($("#montoCliente").val() == formatCurrencyVE($("#montoOrden").val())) {
                $(".sw-btn-next").css("display", "none");
                $(".sw-btn-next").after("<button class='btn btn-primary Payment' type='button' onclick='GenerateOrden()'>Generar Orden</button>")
            }
            else {
                return SearchFinancialSummary(currentStepIndex);
            }

        }
    });

    $('#PaisDestino').select2({
        language: 'es',
        dropdownAutoWidth: true,
        placeholder: 'Seleccione',
        allowClear: true
    }).on('change', function () {

        if ($('#PaisDestino').val() != "") {
            SearchCity();
        }
    });

    $('#CiudadDestino').on("change", function () {

        if ($('#CiudadDestino').val() != "") {
            OficinasPorCiudad();
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

    $("#RecipientDataForTheRequest").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'BeneficiaryType': {
                validators: {
                    notEmpty: {
                        message: 'El tipo de identificación es requerido'
                    }
                }
            },
            'cedulaBeneficiario': {
                validators: {
                    notEmpty: {
                        message: 'La Identificación del beneficiario es requerido'
                    }
                }
            },
            'nombreBeneficiario': {
                validators: {
                    notEmpty: {
                        message: 'El nombre del beneficiario es requerido'
                    }
                }
            },
            'EmailBeneficiario': {
                validators: {
                    notEmpty: {
                        message: 'El email es requerido'
                    }
                }
            },
            'TelBeneficiaio': {
                validators: {
                    notEmpty: {
                        message: 'El telefono es requerido'
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

    $("#DataTransfer").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'PaisDestino': {
                validators: {
                    notEmpty: {
                        message: 'El pais es requerido'
                    }
                }
            },
            'nombreBancoDestino': {
                validators: {
                    notEmpty: {
                        message: 'El Banco es requerido'
                    }
                }
            },
            'numeroCuentaDestino': {
                validators: {
                    notEmpty: {
                        message: 'El Numero de Cuenta es requerido'
                    }
                }
            },
            'aba': {
                validators: {
                    notEmpty: {
                        message: 'El codigo ABA es requerido'
                    }
                }
            },
            'swift': {
                validators: {
                    notEmpty: {
                        message: 'El Codigo SWIFT es requerido'
                    }
                }
            }, 
            'direccionBancoDestino': {
                validators: {
                    notEmpty: {
                        message: 'La direccion del Banco es requerido'
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

    $("#DataBankIntermediary").formValidation({
        framework: 'bootstrap',
        excluded: ':disabled',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'disabled',
        fields: {
            'bancoIntermediario': {
                validators: {
                    notEmpty: {
                        message: 'El Banco es requerido'
                    }
                }
            },
            'numeroCuentaIntermediario': {
                validators: {
                    notEmpty: {
                        message: 'El Banco es requerido'
                    }
                }
                
            },
            'abaIntermediario': {
                validators: {
                    notEmpty: {
                        message: 'El Codigo ABA es requerido'
                    }
                }
            },
            'swiftIntermediario': {
                validators: {
                    notEmpty: {
                        message: 'El Codigo SWIFT es requerido'
                    }
                }
            },
            'direccionBancoIntermediario': {
                validators: {
                    notEmpty: {
                        message: 'La dirección del Banco es requerido'
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

    window.utils.showLoading('Buscando Información... Por favor espere...');
    GlobalApi.ExecuteAsync('get', url, model, "application/json; charset=utf-8", "json", function (result) {
        window.utils.hideLoading();
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
    // Show the loader
    $('#smartwizard').smartWizard("loader", "show");

    if (currentStepIndex == 0) {
        if ($("#tasaCambio").val() == '0,0') {
            // Hide the loader
            $('#smartwizard').smartWizard("loader", "hide");
            window.utils.showMessageWarning("Tasa de Cambio", "No hay tasa registrada para hacer calculos cambiarios");
            ficha = false;
            return;
        }
        else if ($("#clienteTipo").val() == "" || $("#Number").val() == "") {
            // Hide the loader
            $('#smartwizard').smartWizard("loader", "hide");
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

            GlobalApi.ExecuteSimple(GlobalVariables.Searchfichas, model, function (result) {
                if (result.length > 0) {
                    if (result[0].error) {
                        // Hide the loader
                        $('#smartwizard').smartWizard("loader", "hide");
                        window.utils.showMessageError("Error", (result[0].clientErrorDetail + ". " + result[0].apiDetail + ". " + result[0].errorDetail));
                        ficha = false;
                        return;
                    }
                }

                if (result.length == 0) {
                    $('#smartwizard').smartWizard("loader", "hide");
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
                    $('#smartwizard').smartWizard("loader", "hide");
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
        keyword: GlobalConstant.keywordVentaTrf,
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

    // Show the loader
    $('#smartwizard').smartWizard("loader", "show");
    var montoOrden = $('#montoOrden').val().replace(".", "").replace(",", ".").replace("-", ",");;
    var minOrden = $('#minOrden').val();
    var maxOrden = $('#maxOrden').val();

    if (currentStepIndex == GlobalConstant.swStatusClient || currentStepIndex == GlobalConstant.swBeneficiaryData || currentStepIndex == GlobalConstant.swDestinationData
        || currentStepIndex == GlobalConstant.swDestinationIntData || currentStepIndex == GlobalConstant.swOperationsData.Transf || currentStepIndex == GlobalConstant.swOperationsDetail.Transf) {
        if ($("#motivoOferta").val() == "" || $("#MonedaVenta").val() == "0" || $("#MonedaVenta").val() == "" || $("#montoOrden").val() == "") {
            $('#DataForTheRequest').formValidation('revalidateField', "motivoOferta");
            $('#DataForTheRequest').formValidation('revalidateField', "MonedaVenta");
            $('#DataForTheRequest').formValidation('revalidateField', "montoOrden");
            // Hide the loader
            $('#smartwizard').smartWizard("loader", "hide");
            return false;
        }
        else if (parseInt(montoOrden) < parseInt(minOrden) || parseInt(montoOrden) > parseInt(maxOrden)) {

            // Hide the loader
            $('#smartwizard').smartWizard("loader", "hide");
            window.utils.showMessageWarning("Monto Operación", "El monto ingresado no cumple con los limites establecidos ");
            return false;
        }
        else {
            $('#smartwizard').smartWizard("loader", "show");
            var IdMoneda = $("#MonedaVenta option:selected").val();
            var req = null;

            GlobalApi.ExecuteSimple(GlobalVariables.urlSearchMonedas, req, function (result) {
                if (result.error) {
                    // Hide the loader
                    $('#smartwizard').smartWizard("loader", "hide");
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
                idPais: $('#PaisDestino').val(),
                idCorresp:'',
                MonedaOperacion: $("#MonedaVenta").val(),
                OperationType: GlobalConstant.OperationTypeTranf,
                montoEnviar: $("#montoOrden").val(),
                MonedaConversion: GlobalConstant.Bolivar,
                tipoId: $("#clienteTipo").val(),
                tipoOperacion: $("#tipoId").val(),
                tasa: $("#tasaCambio").val(),
            };
            GlobalApi.ExecuteSimple(GlobalVariables.SearchFinancialSummary, model, function (result) {
                if (result.error) {
                    // Hide the loader
                    $('#smartwizard').smartWizard("loader", "hide");
                    window.utils.showMessageError("Error", (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail));
                    return false;
                }

                $("#montoCliente").val(formatCurrencyVE(result.OperationAmmount));
                $('#MontoPagar').val(result.SimbolConversion + " " + formatCurrencyVE(result.AmmountConversion));
                $("#ComisionOperacion").val(formatCurrencyVE(result.TotalCommissionBs));
                $('#montoOperacion').val(result.SimbolConversion + " " + formatCurrencyVE(result.AmmountConversionWithoutRate));
                $('#MontoTotal').val(result.SimbolConversion + " " + formatCurrencyVE(result.AmmountConversion));
                $('#TasaConversion').val(formatCurrencyVE(result.RateConversion));

                $(".sw-btn-next").css("display", "none");
                $(".sw-btn-next").after("<button class='btn btn-primary Payment' type='button' onclick='GenerateOrden()'>Generar Orden</button>")

                // Hide the loader
                $('#smartwizard').smartWizard("loader", "hide");
                return true;

            });
        }
    }
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
            if (v.que == GlobalConstant.MinVentTran) {
                if (v.cuanto == null) {
                    $("#minOrden").val(100);
                    $("#montoOrden").prop('min', 100);
                }
                else {
                    $("#minOrden").val(v.cuanto);
                    $("#montoOrden").prop('min', v.cuanto);
                }
            }
            if (v.que == GlobalConstant.MaxVentTran) {
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
    $("#numeroCuentaDestino").val("");
    $("#nombreBancoDestino").val("");
    $("#direccionBancoDestino").val("");
    $("#aba").val("");
    $("#swift").val("");
    $("#bancoIntermediario").val("");
    $("#numeroCuentaIntermediario").val("");
    $("#abaIntermediario").val("");
    $("#swiftIntermediario").val("");
    $("#direccionBancoIntermediario").val("");
    $("#observaciones").val("");

    $('#MonedaVenta').val(0);
    $('#motivoOferta').val("");
    $('#montoOrden').val("");
    $('#CodMoneda').val("");
};

function SearchCity() {

    var model = {
        CountryId: $('#PaisDestino').val(),
        ForShipping: true,
    };

    GlobalApi.ExecuteSimple(GlobalVariables.SearchCity, model, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $("#CiudadDestino").empty();
        $("#CiudadDestino").val("");
        $("#CiudadDestino").append('<option value="">Seleccione</option>');
        $.each(result, function (i, v) {
            $("#CiudadDestino").append('<option value="' + v.ID_CIUDAD + '">' + v.NOMBRE_CIUDAD + '</option>');
        });
    });
}

function OficinasPorCiudad() {

    var model = {
        idCiudad: $("#CiudadDestino").val(),
        tipoOperacion: GlobalConstant.keywordVentaEnc,
        monto: 300,
    };

    GlobalApi.ExecuteSimple(GlobalVariables.OficinasPorCiudad, model, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $("#OficinaDestino").empty();
        $("#OficinaDestino").val("");
        $("#OficinaDestino").append('<option value="">Seleccione</option>');
        $.each(result, function (i, v) {
            $("#OficinaDestino").append('<option value="' + v.id + '">' + v.nombre + '</option>');
        });
    });
}

function GenerateOrden() {
    var tasa = $("#Tasa").val().replace('.', ',');

    var model = {
        FechaOperacion: $("#Fecha_Operacion").val(),
        TipoDocumentoREM: $("#clienteTipo").val(),
        CIREM: $("#Number").val(),
        ///////////////////////////////
        TipoDocumentoDES: $("#BeneficiaryType option:selected").val(),
        CIDES: $("#cedulaBeneficiario").val(),
        NOMDES: $("#nombreBeneficiario").val(),
        DIRDES: "",
        TELDES: $("#TelBeneficiaio").val(),
        EMAILDES: $("#EmailBeneficiario").val(),
        PAISDES: $('#PaisDestino').val(),
        CIUDADDES: "",
        OFICIANDES:"",
        NUNCUENTADES: $("#numeroCuentaDestino").val(),
        BANCODES: $("#nombreBancoDestino").val(),
        DIRBANCODES: $("#direccionBancoDestino").val(),
        ABA: $("#aba").val(),
        SWIFT: $("#swift").val(),
        NOMBANCOINTER: $("#bancoIntermediario").val(),
        NUMCUENTAINTER: $("#numeroCuentaIntermediario").val(),
        ABAINTER: $("#abaIntermediario").val(),
        SWIFTINTER: $("#swiftIntermediario").val(),
        DIRBANCOINTER: $("#direccionBancoIntermediario").val(),
        Observacion: $("#observaciones").val(),
        OperationType: 0,
        IdMonedaOperacion: $("#MonedaVenta option:selected").val(),
        montoOrden: $("#montoOrden").val(),
        IdMotivosOperacion: $("#motivoOferta  option:selected").val(),
        CodigoTipoOperacion: GlobalConstant.keywordVentaTrf,
        TasaCambio: tasa,
        TasaConversion: $('#TasaConversion').val(),
        IdMonedaConversion: GlobalConstant.Bolivar,
        Comision: $("#ComisionOperacion").val(),
        MontoOperacion: $('#montoOperacion').val().replace('Bs', ''),
        MontoTotalCompra: $('#MontoTotal').val().replace('Bs', ''),
        MontoPagarCCAL: $('#MontoTotal').val().replace('Bs', '')
    };
    Loader("Espere por Favor....");
    $('#smartwizard').smartWizard("loader", "show");

    GlobalApi.ExecuteSimple(GlobalVariables.InsertOperacionesPorCobrar, model, function (result) {
        Swal.close();
        if (result.error) {
            Swal.fire(
                {
                    title: "Error",
                    text: (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail),
                    icon: "error",
                });
            $('#smartwizard').smartWizard("loader", "hide");
            return;
        }
        Swal.fire(
            {
                title: "Operación Exitosa!!!",
                text: "Se ha procesado de manera exitosa.",
                icon: "success",
            });
        CleanFilters();
        $('#smartwizard').smartWizard("loader", "hide");
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