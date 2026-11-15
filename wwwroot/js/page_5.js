
UpdateOrderComplete_5 = function (RES) {
    SpinnerHideForm('ویرایش ردیف سفارش', RES.responseText);
    ClickBtn('FormViewOrderBtn');
    ClickBtn('MsgFrom_B_BtnClose');
}