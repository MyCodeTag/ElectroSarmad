
function PersonGrp_2(G) {
    // کنترل اینکه آیا گروهی انتخاب شده است یا نه
    let STR = EleValue('PersonGrp');
    if (STR.includes(G) == true) {
        STR = STR.replace(G, '');
    } else {
        STR += G;
    };
    SetValueSimpl('PersonGrp', STR);
}

function SetPersonGrp_2(GRP) {
    if (GRP.includes('A')) { $("#PersonGrpImg_A").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_A', 'Y'); };
    if (GRP.includes('B')) { $("#PersonGrpImg_B").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_B', 'Y'); };
    if (GRP.includes('C')) { $("#PersonGrpImg_C").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_C', 'Y'); };
    if (GRP.includes('D')) { $("#PersonGrpImg_D").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_D', 'Y'); };
    if (GRP.includes('E')) { $("#PersonGrpImg_E").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_E', 'Y'); };
    if (GRP.includes('F')) { $("#PersonGrpImg_F").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_F', 'Y'); };
    if (GRP.includes('G')) { $("#PersonGrpImg_G").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_G', 'Y'); };
    if (GRP.includes('H')) { $("#PersonGrpImg_H").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_H', 'Y'); };
    if (GRP.includes('I')) { $("#PersonGrpImg_I").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_I', 'Y'); };
    if (GRP.includes('J')) { $("#PersonGrpImg_J").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_J', 'Y'); };
    if (GRP.includes('K')) { $("#PersonGrpImg_K").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_K', 'Y'); };
    if (GRP.includes('L')) { $("#PersonGrpImg_L").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_L', 'Y'); };
    if (GRP.includes('M')) { $("#PersonGrpImg_M").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_M', 'Y'); };
    if (GRP.includes('N')) { $("#PersonGrpImg_N").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_N', 'Y'); };
    if (GRP.includes('O')) { $("#PersonGrpImg_O").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_O', 'Y'); };
    if (GRP.includes('P')) { $("#PersonGrpImg_P").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_P', 'Y'); };
    if (GRP.includes('Q')) { $("#PersonGrpImg_Q").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_Q', 'Y'); };
    if (GRP.includes('R')) { $("#PersonGrpImg_R").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_R', 'Y'); };
    if (GRP.includes('S')) { $("#PersonGrpImg_S").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_S', 'Y'); };
    if (GRP.includes('T')) { $("#PersonGrpImg_T").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_T', 'Y'); };
    if (GRP.includes('U')) { $("#PersonGrpImg_U").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_U', 'Y'); };
    if (GRP.includes('V')) { $("#PersonGrpImg_V").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_V', 'Y'); };
    if (GRP.includes('W')) { $("#PersonGrpImg_W").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_W', 'Y'); };
    if (GRP.includes('X')) { $("#PersonGrpImg_X").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_X', 'Y'); };
    if (GRP.includes('Y')) { $("#PersonGrpImg_Y").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_Y', 'Y'); };
    if (GRP.includes('Z')) { $("#PersonGrpImg_Z").attr('src', '/images/Btn_Yes.jpg'); SetValueSimplMain('PersonGrp_Z', 'Y'); };
}
