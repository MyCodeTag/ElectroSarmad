//-------- 3 مرکز پیام

NewMsgComplete_3 = function () {
    SetText('FileName', 'پیوست را انتخاب کنید');
    $("#FileUpload").val(null);
    var BTN = document.getElementById('FileKnd');
    $(BTN).removeClass('icofont-upload');
    $(BTN).removeClass('icofont-image');
    $(BTN).removeClass('icofont-file-text');
    $(BTN).removeClass('icofont-file-bmp');
    $(BTN).removeClass('icofont-file-word');
    $(BTN).removeClass('icofont-file-pdf');
    $(BTN).removeClass('icofont-file-gif');
    $(BTN).removeClass('icofont-file-excel');
    $(BTN).removeClass('icofont-exclamation');

    $(BTN).addClass('icofont-upload');
}

SetDateTimeYaddasht_3 = function (RES) {
    if (isJsonString(RES.responseText)) {
        var RD = JSON.parse(RES.responseText);
        SetValueSimpl('YadDate', RD.a
            , 'YadTime', RD.b
        );
    }
}

SetDateTimeYaddasht_3 = function (RES) {
    if (isJsonString(RES.responseText)) {
        var RD = JSON.parse(RES.responseText);
        SetValueSimpl('YadDate', RD.a
            , 'YadTime', RD.b
        );
    }
}
ForwardMsgComp_3 = function (RES) {
    SpinnerHideForm();

    if (isJsonString(RES.responseText)) {
        const JS = JSON.parse(RES.responseText);


        if (JS.a.toUpperCase() == "OK") {
            ClickBtn('MsgFrom_A_BtnClose');
            ClickBtnSimpl('MsgDtListBtn', JS.b, 'MsgDtOrder', 'Desc');
        }
        else {
            ShowMsg(JS.a);
        }
    }
}
function SelectPerson_3(KND, ID, PIC, NAME, SEMAT) {
    var IDValue = ',' + ID + ',';
    var IdsValue = EleValue('Mokhataban' + KND);


    if (IdsValue.includes(IDValue) == true)
        return;

    var fnTxt = "ReturnSelectPerson_3('" + KND + "','" + ID + "');";

    SetValueSimplMain('Mokhataban' + KND, IdsValue + ID + ',');



    var NewItem = '<div class="m-1 areaKarbar w-min-300" id="ListMokhataban' + KND + '_' + ID + '">';

    if (PIC == "") {
        NewItem += '<div class="PicKarbar colorSecondary"><img src="/Uploads/Persons/P_0.jpg" class="rounded rounded-5 w-fix-50 h-fix-50" /></div>';
    } else {
        NewItem += '<div class="PicKarbar colorSecondary"><img src="/Uploads/Persons/' + PIC + '" class="rounded rounded-5 w-fix-50 h-fix-50" /></div>';
    }
    NewItem += '<div class="InfoKarbar"> <span class="control-label"><span class="text-white-50">سمت: </span><span>' + SEMAT + '</span></span>';
    NewItem += '<div class="colorSecondary">' + NAME;
    NewItem += '<span class="icofont-00x icofont-delete text-danger float-end btn-hover" onclick = "' + fnTxt + '" ></span>';
    NewItem += '</div></div></div>';

    $('#ListMokhataban' + KND).append(NewItem);

}
function AddPerson_3(ID, PIC, NAME, SEMAT) {

    var fnTxt = "ClickBtnSimpl('DeletMokhatabAddBtn','" + ID + "')";

    var NewItem = '<div id="FormAddMokhatabNow_' + ID + '">';
    NewItem += '<div class="areaKarbar cursor-pointer w-min-330">';
    NewItem += '<div class="PicKarbar colorSecondary">';
    NewItem += '<img src="/Uploads/Persons/"' + PIC + ' "class="rounded rounded-5 w-fix-50 h-fix-50" />';
    NewItem += '</div>';
    NewItem += '<div class="InfoKarbar">';
    NewItem += '<span class="control-label">';
    NewItem += '<span class="text-white-50">سمت: </span><span> </span>';
    NewItem += '</span>';
    NewItem += '<div class="colorSecondary text-start">';
    NewItem += '<span>' + NAME + '</span>';
    NewItem += '<button type="button" class="btn_Icon btn_Pill Icon_Red icofont-00x icofont-delete float-end" title="حذف مخاطب" onclick="' + fnTxt + '"></button>';
    NewItem += '</div></div></div></div>';

    SetHtml('FormAddMokhatabNow', EleHtml('FormAddMokhatabNow') + NewItem);
    SetHtml('MokhatabReplyList', EleHtml('MokhatabReplyList') + NewItem);

    ClickBtnSimpl('AddMokhatabBtn', ID)
}

function ReturnSelectPerson_3(KND, ID) {
    var IDValue = ',' + ID + ',';
    var IdsValue = EleValue('Mokhataban' + KND);
    IdsValue = IdsValue.replace(IDValue, ',');
    SetValueSimplMain('Mokhataban' + KND, IdsValue);
    $('#ListMokhataban' + KND + '_' + ID).remove();
}


function SelectAllMokhatab_3(CNT) {
    for (let i = 0; i < CNT; i++) {
        ClickBtn('ListMokhatab_' + i);
    }
    AreaUp('ModalForm_A'); AreaClear('MsgForm_A_MATN');
}
