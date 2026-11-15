SabtFactorComp_4 = function (RES) {
    SpinnerHideForm();
    if (isJsonString(RES.responseText)) {
        var JS = JSON.parse(RES.responseJSON);
        if (JS.res != "OK")
            ShowMsg('خطا در ثبت فاکتور', JS.restp);
        else {
            ShowMsg('ثبت فاکتور', JS.restp);

            SetHtml('AreaSabt', '<div class="alert alert-primary">ابتدا مشتری را انتخاب کنید<input type="hidden" name="Customer" value="0" /></div>')
        }

    }
}

SabtProductComp_4 = function (RES) {
    SpinnerHideForm();
    if (isJsonString(RES.responseText)) {
        var JS = JSON.parse(RES.responseJSON);
        if (JS.res != "OK")
            ShowMsg('خطا در ثبت فاکتور', JS.restp);
        else {
            ShowMsg('ثبت فاکتور', JS.restp);

            ClickBtn('RefreshProductBtn');
        }

    }
}


SabtPishFactorComplet_4 = function (RES) {
    if (isJsonString(RES.responseText)) {
        const JS = JSON.parse(RES.responseJSON);
        ShowMsgArea('ListKalaPishFactor', JS.message);
        if (JS.success == "OK") {
            ClickBtnSimpl('PishFactorListKalaBtn', JS.id
                , 'PishFactorId', JS.id
                , 'PrintPishFactorBtn', JS.id
            );

            AreaUp('AreaExtended');
            AreaDown('AreaExtendedBtn');

        }
    }
}

function CalculatePrice_4() {
    const NQ = Number(numberClearCommas(EleValue('Pre_Nq')));
    const FEE = Number(numberClearCommas(EleValue('Pre_Fee')));
    const DISC = Number(numberClearCommas(EleValue('Pre_Disc')));
    const TAX = ((NQ * FEE) - DISC) * 0.10;

    
    const Price = (NQ * FEE) - DISC + TAX;

    SetValueSimplMain('Pre_Tax', numberWithCommas(TAX));
    SetValueSimplMain('Pre_Price', numberWithCommas(Price));
}

PreFactorCopyComplet_4 = function (RES) {
    SpinnerHideForm();
    const ID = RES.responseText;
    ClickBtnSimpl('FormSabtPishFactorBtn', ID);
}


SelectCustomerForPre_4 = function (RES) {
    SpinnerHideForm();
    ClickBtn('MsgFrom_B_BtnClose');
    if (isJsonString(RES.responseText)) {
        const JS = JSON.parse(RES.responseJSON);

        SetValueSimpl('PreCustomer', JS.firstname + ' ' + JS.lastname
            , 'PrePhone', JS.mobileno
            , 'PreCityName', ''
            , 'PreEcoNumber', ''
            , 'PreCodeMeli', JS.nationalcode
            , 'SabtPerson_City', JS.id_city
            , 'PreCodePost', JS.post
            , 'PreAddress', JS.adres
            , 'PreAboutCustomer', ''
        );

    }
}
 
