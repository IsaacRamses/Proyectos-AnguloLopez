var ficha;
var validBankAccount = false;
var Correspondent = '';
var IdOficina = 0;
var IdCiudad = 0;
var ClientUniqueId = "";
var ClientType = "";
$("#divBancoCuenta").css("display", "none");
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
            Loader("Espere por Favor....");
            Searchfichas(currentStepIndex);
            return ficha;
        }
        else if (currentStepIndex == GlobalConstant.swStatusClient && nextStepIndex == GlobalConstant.swBeneficiaryData) {
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
            $('#smartwizard').smartWizard("reset");

        }
        else if (currentStepIndex == GlobalConstant.swBeneficiaryData && nextStepIndex == GlobalConstant.swDestinationData) {
            if ($('#BeneficiaryType').val() == "" || $('#cedulaDestinatario').val() == "" || $('#nombreDestinatario1').val() == "" || $('#ApellidoDestinatario1').val() == "" ||
                $('#EmailBeneficiario').val() == "" || $('#TelBeneficiaio').val() == "" || validateEmail($('#EmailBeneficiario').val()) == false) {
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "BeneficiaryType");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "cedulaDestinatario");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "nombreDestinatario1");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "ApellidoDestinatario1");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "EmailBeneficiario");
                $('#RecipientDataForTheRequest').formValidation('revalidateField', "TelBeneficiaio");
                return false;
            }

        }
        else if (currentStepIndex == GlobalConstant.swDestinationData && nextStepIndex == GlobalConstant.swOperationsData.ElectronicParcel) {
            if (validBankAccount) {
                if ($('#PaisDestino').val() == "" || $('#BancoCuenta option:selected').val() == 0 || $("#numeroCuentaDestino").val() == "" || $("#TypeAccount").val() == "" || $("#DireccionBanco").val() == "") {

                    $('#DataDestino').formValidation('revalidateField', "BancoCuenta");
                    $('#DataDestino').formValidation('revalidateField', "numeroCuentaDestino");
                    $('#DataDestino').formValidation('revalidateField', "TypeAccount");
                    $('#DataDestino').formValidation('revalidateField', "DireccionBanco");
                    return false;
                }
            } else {                          
                if ($('#PaisDestino').val() == "" || $('#CiudadDestino').val() == "" || $('#OficinaDestino').val() == "") {

                     $('#DataDestino').formValidation('revalidateField', "PaisDestino");
                     $('#DataDestino').formValidation('revalidateField', "CiudadDestino");
                     $('#DataDestino').formValidation('revalidateField', "OficinaDestino");
                     return false;
                    }
            }
        }
        else if (nextStepIndex == GlobalConstant.swSearchClient) {
            Loader("Espere por Favor....");
            CleanFilters();
            // Reset wizard
            $('#smartwizard').smartWizard("reset");

        }
        else if (currentStepIndex == GlobalConstant.swOperationsData.ElectronicParcel && nextStepIndex == GlobalConstant.swOperationsDetail.ElectronicParcel) {
            Loader("Espere por Favor....");
            if (ValidateRequestAmount(currentStepIndex)) {
                return SearchFinancialSummary(currentStepIndex);
            } else {
                return false;
            }
        }
        else if (currentStepIndex == GlobalConstant.swOperationsDetail.ElectronicParcel) {
            $(".sw-btn-next").css("display", "");
            $(".Payment").remove();
        }
        else if (currentStepIndex != GlobalConstant.swOperationsData.ElectronicParcel && nextStepIndex == GlobalConstant.swOperationsDetail.ElectronicParcel) {
            if ($("#montoCliente").val() == formatCurrencyVE($("#montoOrden").val())) {
                $(".sw-btn-next").css("display", "none");
                $(".sw-btn-next").after("<button class='btn btn-primary Payment' type='button' onclick='GenerateOrden()'>Generar Orden</button>")
            }
            else {
                Loader("Espere por Favor....");
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

            window.utils.showLoading('Cargando Corresponsales....');
            validBankAccount = false;
            IdOficina = 0;
            IdCiudad = 0;
            $("#divCiudadDestino").hide();
            $("#divOficinaDestino").hide();
            $("#divBancoCuenta").hide();
            $("#divTypeAccount").hide();
            $("#divNumeroCuentaDes").hide();
            $("#divDireccionBanco").hide();
            $("#divModalidadPago").hide();

            $("#CiudadDestino").val("");
            $("#OficinaDestino").val("");
            $("#BancoCuenta option[value='0']").attr("selected", true);
            $("#TypeAccount option[value='']").attr("selected", true);
            $("#ModalidadPago option[value='']").attr("selected", true);
            $("#idCorresponsal option[value='']").attr("selected", true);
            $("#numeroCuentaDestino").val("");
            $("#DireccionBanco").val(""); 

            var model = {};
            model.pais = $('#PaisDestino').val();
            model.deposito = true;
            model.tipo_remesa = GlobalConstant.keywordVentaTrf;
            GlobalApi.ExecuteSimple(GlobalVariables.urlGetCorresponsalPais, model, function (result) {
                if (result[0].error) {
                    Swal.fire(
                        {
                            title: "Error",
                            text: result.ErrorMessage,
                            icon: "error",
                        });
                    return;

                }

                if (result.length > 1) {
                    /*loadCorresponsalPais(result);*/
                    $("#idCorresponsal").empty();
                    $("#idCorresponsal").val("");
                    $("#idCorresponsal").append('<option value="">Seleccione</option>');
                    $.each(result, function (i, v) {
                        $("#idCorresponsal").append('<option value="' + v.ID_CORRESPONSAL + '">' + v.CORRESPONSAL + '</option>');
                    });
                    $('#divCorresponsal').show();
                    window.utils.hideLoading();
                } else {
                    Correspondent = result[0].ID_CORRESPONSAL;
                    $('#divCorresponsal').hide();
                    SearchModalityPaymentByCorrespondent();
                    $("#divModalidadPago").show();
                    window.utils.hideLoading();
                }
            });
            window.utils.hideLoading();
        }
        else {
            validBankAccount = false;
            IdOficina = 0;
            IdCiudad = 0;
            Correspondent = '';
            $("#divCiudadDestino").hide();
            $("#divOficinaDestino").hide();
            $("#divBancoCuenta").hide();
            $("#divTypeAccount").hide();
            $("#divNumeroCuentaDes").hide();
            $("#divDireccionBanco").hide();
            $("#divModalidadPago").hide();
            $('#divCorresponsal').hide();

            $("#CiudadDestino").val("");
            $("#OficinaDestino").val("");
            $("#BancoCuenta option[value='0']").attr("selected", true);
            $("#TypeAccount option[value='']").attr("selected", true);
            $("#ModalidadPago option[value='']").attr("selected", true);
            $("#idCorresponsal option[value='']").attr("selected", true);
            $("#numeroCuentaDestino").val("");
            $("#DireccionBanco").val(""); 
        }
    });

    $('#ModalidadPago').change(function () {
        if ($('#PaisDestino').val() != '' && $('#PaisDestino').val() != null && $('#ModalidadPago').val() != 0 && $('#ModalidadPago').val() != 0 != null) {
            if ($('#ModalidadPago').val() == GlobalConstant.DepositoCuenta) {
                validBankAccount = true;
                var model = {
                    tipo_remesa : GlobalConstant.keywordVentaTrf,
                    deposito : true,
                    Pais : $('#PaisDestino').val(),
                    Corresponsal : Correspondent
                };

                IdOficina = 0;
                IdCiudad = 0;
                Loader("Espere por Favor....");
                GlobalApi.ExecuteSimpleAsync( GlobalVariables.SearchInfoByCorresponsal, model, function (result) {
                    Swal.close();
                    if (result.error) {
                        Swal.fire(
                            {
                                title: "Error",
                                text: result.ErrorMessage,
                                icon: "error",
                            });
                        return;

                    }

                    if (result != null) {
                        IdOficina = result.ID_OFICINA;
                        IdCiudad = result.ID_CIUDAD;
                    }
                    


                });
                loadBank();
                $("#divCiudadDestino").hide();
                $("#divOficinaDestino").hide();
                $("#divBancoCuenta").show(); 
                $("#divTypeAccount").show(); 
                $("#divNumeroCuentaDes").show(); 
                $("#divDireccionBanco").show(); 
                
            }
            else {
                validBankAccount = false;
                SearchCity();
                $("#divCiudadDestino").show();
                $("#divOficinaDestino").show();
                $("#divBancoCuenta").hide();
                $("#divTypeAccount").hide();
                $("#divNumeroCuentaDes").hide();
                $("#divDireccionBanco").hide();   

                $("#CiudadDestino").val("");
                $("#OficinaDestino").val("");
                $("#BancoCuenta option[value='0']").attr("selected", true);
                $("#TypeAccount option[value='']").attr("selected", true);
                $("#numeroCuentaDestino").val("");
                $("#DireccionBanco").val(""); 
            }
        }
        else {
            validBankAccount = false;
            IdOficina = 0;
            IdCiudad = 0;
            $("#divCiudadDestino").hide();
            $("#divOficinaDestino").hide();
            $("#divBancoCuenta").hide();
            $("#divTypeAccount").hide();
            $("#divNumeroCuentaDes").hide();
            $("#divDireccionBanco").hide();   

            $("#CiudadDestino").val("");
            $("#OficinaDestino").val("");
            $("#BancoCuenta option[value='0']").attr("selected", true);
            $("#TypeAccount option[value='']").attr("selected", true);
            $("#numeroCuentaDestino").val("");
            $("#DireccionBanco").val(""); 
  
        }
    });

    $('#CiudadDestino').on("change", function () {

        if ($('#CiudadDestino').val() != "") {
            OficinasPorCiudad();
            BancosRequeridosCiudad();
        }
    });

    $('#idCorresponsal').on("change", function () {
        if ($('#idCorresponsal').val() == "") {
            Correspondent = '';
            $("#divModalidadPago").hide();
            $("#divCiudadDestino").hide();
            $("#divOficinaDestino").hide();
            $("#divBancoCuenta").hide();
            $("#divTypeAccount").hide();
            $("#divNumeroCuentaDes").hide();
            $("#divDireccionBanco").hide();

            $("#CiudadDestino").val("");
            $("#OficinaDestino").val("");
            $("#BancoCuenta option[value='0']").attr("selected", true);
            $("#TypeAccount option[value='']").attr("selected", true);
            $("#numeroCuentaDestino").val("");
            $("#DireccionBanco").val(""); 

        } else {
            window.utils.showLoading('Cargando Modalidad de Pago....');
            Correspondent = $('#idCorresponsal').val();
            SearchModalityPaymentByCorrespondent();
            $("#divModalidadPago").show();
            window.utils.hideLoading();
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
            'cedulaDestinatario': {
                validators: {
                    notEmpty: {
                        message: 'La Identificación del beneficiario es requerido'
                    }
                }
            },
            'nombreDestinatario1': {
                validators: {
                    notEmpty: {
                        message: 'El primer nombre del beneficiario es requerido'
                    }
                }
            },
            'ApellidoDestinatario1': {
                validators: {
                    notEmpty: {
                        message: 'El primer apellido del beneficiario es requerido'
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

    $("#DataDestino").formValidation({
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
            'CiudadDestino': {
                validators: {
                    notEmpty: {
                        message: 'La ciudad es requerido'
                    }
                }
            },
            'OficinaDestino': {
                validators: {
                    notEmpty: {
                        message: 'La oficina es requerido'
                    }
                }
            },
            'BancoCuenta': {
                validators: {
                    callback: {
                        message: 'El Banco de destino es requerido',
                        callback: function (input) {
                            return ValidateSelectBanco(input);
                        },
                    }
                }
            },
            'numeroCuentaDestino': {
                validators: {
                    callback: {
                        message: 'El número de cuenta de destino es requerido',
                        callback: function (input) {
                            return ValidateNroCuenta(input);
                        },
                    }
                }
            },
            'TypeAccount': {
                validators: {
                    callback: {
                        message: 'El tipo de la cuenta es requerido',
                        callback: function (input) {
                            return ValidateSelectBanco(input);
                        },
                    }
                }
            },
            'DireccionBanco': {
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
;
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
                }
            });
        }
    }
};

function TiposMovimientos(TipoIdentidad) {

    var model = {
        idTipoIdentidad: TipoIdentidad,
        keyword: GlobalConstant.keywordVentaEnc,
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

            GlobalApi.ExecuteSimpleAsync(GlobalVariables.urlSearchMonedas, req, function (result) {
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

            var idCorresp = IdOficina;
            if (!validBankAccount) {
                idCorresp = $("#OficinaDestino option:selected").val();
            }

            var model = {
                moneda: $('#CodMoneda').val(),
                idPais: $('#PaisDestino').val(),
                idCorresp: idCorresp,
                MonedaOperacion: $("#MonedaVenta").val(),
                OperationType: GlobalConstant.OperationTypeVentaEnc,
                montoEnviar: $("#montoOrden").val(),
                MonedaConversion: GlobalConstant.Bolivar,
                tipoId: $("#clienteTipo").val(),
                tipoOperacion: $("#tipoId").val(),
                tasa: $("#tasaCambio").val(),
            };
            GlobalApi.ExecuteSimpleAsync(GlobalVariables.SearchFinancialSummary, model, function (result) {
                if (result.error) {
                    // Hide the loader
                    Swal.close();
                    window.utils.showMessageError("Error", (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail));
                    return false;
                }


                $("#montoCliente").val(formatCurrencyVE(result.OperationAmmount));
                result.RateOperation = result.RateOperation.filter(function (tarifas) {
                    return (tarifas.moneda != null && tarifas.moneda == $('#CodMoneda').val());
                });
                var TarifaUS = 0;
                $.each(result.RateOperation, function () {
                    if (this.valor < 1) {
                        TarifaUS = TarifaUS + (item.valor * result.OperationAmmount);
                    }
                    else {
                        TarifaUS = TarifaUS + this.valor;
                    }
                });
                var TarifaUS_ = TarifaUS.toString();
                $("#MontoRecibirCorres").val(result.AmmountUsdFormat);
                $("#TarifaUS").val(TarifaUS_.replace('.', ','));
                $('#MontoPagar').val(result.SimbolConversion + " " + formatCurrencyVE(Math.round(result.AmmountConversion)));
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
                    $("#montoOrden").prop('min', 100);
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

function ValidateSelectBanco(value) {
    if (validBankAccount) {
        if (value != '' && value != 0 && value != null) {
            return true;
        }
        return false;
    } else {
        return true;
    }

};

function ValidateNroCuenta(value) {
    if (validBankAccount) {
        if (value != '' && value != 0 && value != null) {
            return true;
        }
        return false;
    } else {
        return true;
    }

};

function CleanFilters() {
    validBankAccount = false;
    IdOficina = 0;
    IdCiudad = 0;
    ClientUniqueId = "";
    ClientType = "";
    $("#Number").val("");
    $("#imfFotoAmpliada").val("");
    $("#imfFoto").val("");
    $("#cedulaBeneficiario").val("");
    $("#nombreBeneficiario").val("");
    $("#EmailBeneficiario").val("");
    $("#TelBeneficiaio").val("");
    $("#statusNameFicha").val("");
    $("#clienteTipo").val("");
    $("#montoCliente").val("");
    $('#MontoPagar').val("");
    $("#ComisionOperacion").val("");
    $('#montoOperacion').val("");
    $('#MontoTotal').val("");
    $('#TasaConversion').val("");

    $("#CiudadDestino").val("");
    $("#OficinaDestino").val("");
    $("#BancoCuenta option[value='0']").attr("selected", true);
    $("#TypeAccount option[value='']").attr("selected", true);
    $("#numeroCuentaDestino").val("");
    $("#DireccionBanco").val(""); 


    $("#divCiudadDestino").hide();
    $("#divOficinaDestino").hide();
    $("#divBancoCuenta").hide();
    $("#divTypeAccount").hide();
    $("#divNumeroCuentaDes").hide();
    $("#divDireccionBanco").hide();

    $('#MonedaVenta').val(0);
    $('#motivoOferta').val("");
    $('#montoOrden').val("");
    $('#CodMoneda').val("");

    $("#BeneficiaryType option[value='']").attr("selected", true);
    $("#cedulaDestinatario").val("");
    $("#nombreDestinatario1").val("");
    $("#nombreDestinatario2").val("");
    $("#ApellidoDestinatario1").val("");
    $("#ApellidoDestinatario2").val("");

    Swal.close();
};

function SearchModalityPaymentByCorrespondent() {
    window.utils.showLoading('Por favor espere...');
    var req = {
        Country: $('#PaisDestino').val(),
        StatusId: GlobalConstant.CorresStatusActivo,
        Corresponsal: Correspondent
    };
    GlobalApi.ExecuteSimple2('POST', GlobalVariables.SearchModalityPaymentByCorrespondent, req, function (result) {
        if (result.length > 0) {
            $("#ModalidadPago").empty();
            $("#ModalidadPago").val("");
            $("#ModalidadPago").append('<option value="">Seleccione</option>');
            $.each(result, function (i, v) {
                $("#ModalidadPago").append('<option value="' + v.DetailId + '">' + v.DetailName + '</option>');
            });
        }

        window.utils.hideLoading();
    });
}

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

function BancosRequeridosCiudad() {
    validBankAccount = false;
    var req = {
        CityId: $("#CiudadDestino option:selected").val()
    };

    GlobalApi.ExecuteSimple(GlobalVariables.SearchBankAccountRequiredCity, req, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $('#BancoCuenta').empty();
        $('#BancoCuenta').val("");
        if (result.length > 0) {
            loadBank();
            validBankAccount = true;
        }
    });
}

function loadBank() {

    var req = { PAIS: $("#PaisDestino option:selected").val() };
    GlobalApi.ExecuteSimple(GlobalVariables.SearchBancos, req, function (result) {
        if (result.Error) {
            Swal.fire(
                {
                    title: "Error",
                    text: result.ErrorMessage,
                    icon: "error",
                });
            return;
        }
        $('#BancoCuenta').empty();
        $.each(result, function (i, v) {
            var Operation = '<option value="' + v.ID_BANCO + '">' + v.BANCO + '</option>'
            $('#BancoCuenta').append(Operation);
        });

    });
}

function GenerateOrden() {
    Loader("Espere por Favor....");
    var tasa = $("#Tasa").val().replace('.', ',');
    var BancoCuentaDestino = "";
    if ($("#BancoCuenta  option:selected").val() != 0) {
        BancoCuentaDestino = $("#BancoCuenta  option:selected").text()
    }
    var idOficina = IdOficina
    if (!validBankAccount) {
        idOficina = $("#OficinaDestino option:selected").val();
    }

    var model = {
        FechaOperacion: $("#Fecha_Operacion").val(),
        TipoDocumentoREM: $("#clienteTipo").val(),
        CIREM: $("#Number").val(),
        ///////////////////////////////
        TipoDocumentoDES: $("#BeneficiaryType option:selected").val(),
        CIDES: $("#cedulaDestinatario").val(),
        NOMDES1: $("#nombreDestinatario1").val(),
        NOMDES2: $("#nombreDestinatario2").val(),
        APEDES1: $("#ApellidoDestinatario1").val(),
        APEDES2: $("#ApellidoDestinatario2").val(),
        NOMDES: $("#nombreDestinatario1").val() + ' ' + $("#nombreDestinatario2").val() + ' ' + $("#ApellidoDestinatario1").val() + $("#ApellidoDestinatario2").val(),
        DIRDES: "",
        TELDES: $("#TelBeneficiaio").val(),
        EMAILDES: $("#EmailBeneficiario").val(),
        PAISDES: $('#PaisDestino').val(),
        CIUDADDES: $("#CiudadDestino option:selected").val(),
        OFICIANDES: idOficina,
        BANCODES: BancoCuentaDestino,
        NUNCUENTADES: $("#numeroCuentaDestino").val(),
        DIRBANCODES: $("#DireccionBanco").val(),
        TIPOCUENTADES: $("#TypeAccount").val(),
        Observacion: "",
        OperationType: 0,
        IdMonedaOperacion: $("#MonedaVenta option:selected").val(),
        montoOrden: $("#montoOrden").val(),
        IdMotivosOperacion: $("#motivoOferta  option:selected").val(),
        CodigoTipoOperacion: GlobalConstant.keywordVentaEnc,
        TasaCambio: tasa,
        TasaConversion: $('#TasaConversion').val(),
        IdMonedaConversion: GlobalConstant.Bolivar,
        Comision: $("#ComisionOperacion").val(),
        MontoOperacion: $('#montoOperacion').val().replace('Bs', ''),
        MontoTotalCompra: $('#MontoTotal').val().replace('Bs', ''),
        MontoPagarCCAL: $('#MontoTotal').val().replace('Bs', '')
    };
 
    GlobalApi.ExecuteSimpleAsync(GlobalVariables.InsertOperacionesPorCobrar, model, function (result) {
        if (result.error) {
            Swal.fire(
                {
                    title: "Error",
                    text: (result.clientErrorDetail + ". " + result.apiDetail + ". " + result.errorDetail),
                    icon: "error",
                });
            return;
        }
        CleanFilters();
        Swal.fire(
            {
                title: "Operación Exitosa!!!",
                text: "Se ha procesado de manera exitosa.",
                icon: "success",
            });
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

    if (currentStepIndex == GlobalConstant.swStatusClient || currentStepIndex == GlobalConstant.swBeneficiaryData || currentStepIndex == GlobalConstant.swDestinationData || currentStepIndex == GlobalConstant.swOperationsData.ElectronicParcel) {
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
                    resultBit = true;
                }
            });
        }
    }

    return resultBit;
}