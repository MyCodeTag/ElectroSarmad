SabtRepairComplet_15 = function (RES) {
    SpinnerHideForm();
    if (isJsonString(RES.responseText)) {
        const JS = JSON.parse(RES.responseJSON);
        SpinnerHideForm(JS.message);
        if (JS.success == "OK") {
            ClickBtn('MsgFrom_BtnClose');

            if (EleValue('FormSabtRepairBtn') == 0) {
                // اگر پذیرش جدید است چاپ بگیرد
                ClickBtnSimpl('PrintRepairBtn', JS.id);
            }
        }
    }
}

RepairDeletComplet_15 = function (RES) {
    AreaHide('RepairCHNG_' + EleValue('RepairBtn'));
    SpinnerHideForm();
    SpinnerHideForm('حذف قبض پذیرش', RES.responseText);

}



RepairChangComplet_15 = function (RES) {
    ClickBtnSimpl('BtnKartabl', 'Open');
    SpinnerHideForm();
    ClickBtn('MsgFrom_BtnClose');

}
