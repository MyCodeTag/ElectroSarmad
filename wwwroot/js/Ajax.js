let CheckConfirm = true;
let ActiveForm = "";
//var ActiveButton = undefined;
function SetConfirm() {
    CheckConfirm = true;
    SetValueSimpl('ConfirmFormBtn', '');
}
document.addEventListener("keydown", function (e) {
    // <form data-allow-enter="true">  اگر میخواهیم Enter برای دکمه submit در همه جای فرم کار کند این تنظیم اضافه شود
    if (e.key !== "Enter")
        return;

    const target = e.target;

    if (!target)
        return;

    if (target.tagName === "TEXTAREA")
        return;

    if (target.type === "submit")
        return;

    const form = target.closest("form");

    if (!form)
        return;

    if (form.dataset.allowEnter === "true")
        return;

    e.preventDefault();
});

(function ($) {
    var data_click = "unobtrusiveAjaxClick",
        data_target = "unobtrusiveAjaxClickTarget",
        data_validation = "unobtrusiveValidation",
        data_submitbtn = "unobtrusiveAjaxSubmitBtn";

    function getFunction(code, argNames) {
        var fn = window, parts = (code || "").split(".");
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof (fn) === "function") {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }

    function isMethodProxySafe(method) {
        return method === "GET" || method === "POST";
    }

    function asyncOnBeforeSend(xhr, method) {
        if (!isMethodProxySafe(method)) {
            xhr.setRequestHeader("X-HTTP-Method-Override", method);
        }
    }

    function asyncOnSuccess(element, data, contentType) {
        var mode;

        if (contentType.indexOf("application/x-javascript") !== -1) {  // jQuery already executes JavaScript for us
            return;
        }

        mode = (element.getAttribute("data-ajax-mode") || "").toUpperCase();
        $(element.getAttribute("data-ajax-update")).each(function (i, update) {
            var top;

            switch (mode) {
                case "BEFORE":
                    $(update).prepend(data);
                    break;
                case "AFTER":
                    $(update).append(data);
                    break;
                case "REPLACE-WITH":
                    $(update).replaceWith(data);
                    break;
                default:
                    $(update).html(data);
                    break;
            }
        });
    }
    ////////////////
    // --- Button disable / restore helpers ---
    function disableSubmitButton(btn) {
        if (!btn) return;
        try {
            if (!btn.disabled) {
                btn.disabled = true;
                btn.classList.add("btn-loading");
                btn.classList.add("ajax-submitted"); // marker we can find later
            }
        } catch (e) {
            // ignore
        }
    }

    function restoreSubmitButtonFromXhr(xhr) {
        if (!xhr) return;
        try {
            var btn = xhr._btn;
            if (!btn && xhr.form) {
                try {
                    btn = $(xhr.form).data(data_submitbtn) || xhr.form.querySelector(".ajax-submitted");
                } catch (e) { btn = xhr.form.querySelector(".ajax-submitted"); }
            }

            if (btn && document.contains(btn)) {
                btn.disabled = false;
                btn.classList.remove("btn-loading");
                btn.classList.remove("ajax-submitted");
            }

            // cleanup: remove stored ref on form to avoid stale references
            try {
                if (xhr.form) {
                    $(xhr.form).removeData(data_submitbtn);
                }
            } catch (e) { /* ignore */ }

            if (xhr._reEnableTimeout) {
                clearTimeout(xhr._reEnableTimeout);
                xhr._reEnableTimeout = null;
            }

        } catch (e) {
            // ignore
        }
    }
    ////////////////

    function asyncRequest(element, options) {
//        var confirm, loading, method, duration;
        var confirm, method;

        confirm = element.getAttribute("data-ajax-confirm");
        if (confirm && CheckConfirm) {
            CheckConfirm = false;

            SetTextMain('ConfirmMsg', confirm);
            SetValueSimpl('ConfirmFormBtn', element.getAttribute("data-ajax-Btn"));
            $('#ConfirmModal').css('display', 'flex');
            // منتظر بلی یا خیر کاربر هستیم، فعلا دکمه فعال شود و بعد از کلیک روی بلی یا خیر، دوباره این تابع فراخوانی می شود

            return;
        }
        else
        {
            CheckConfirm = true;

            // mark submit button and disable it immediately
            try {
                var btn = evt.currentTarget;
                // also store on form data for immediate retrieval if needed
                $(form).data(data_submitbtn, btn);
                // غیرفعال کردن دکمه در tick بعدی تا ارسال فرم مختل نشود
                setTimeout(function () {
                    try {
                        disableSubmitButton(btn);
                    } catch (e) { /* ignore */ }
                }, 0);

            } catch (e) { /* ignore */ }


            //            loading = $(element.getAttribute("data-ajax-loading"));
//            duration = parseInt(element.getAttribute("data-ajax-loading-duration"), 10) || 0;
            //////////////////
            // If a submit button was marked on the form (see click handler),
            // keep reference on options so we can attach it to the xhr/jq later.
            try {
                if (element instanceof HTMLFormElement) {
                    // the click handler marks the clicked button with class 'ajax-submitted'
                    var sb = element.querySelector(".ajax-submitted");
                    if (sb) options._submitBtn = sb;
                }
            } catch (e) { /* ignore */ }

            //////////////////

            $.extend(options, {
                type: element.getAttribute("data-ajax-method") || undefined,
                url: element.getAttribute("data-ajax-url") || undefined,
                cache: (element.getAttribute("data-ajax-cache") || "").toLowerCase() === "true",
                beforeSend: function (xhr) {
                    var result;
                    asyncOnBeforeSend(xhr, method);
                    ///////////////
                    // Prefer submit button stored on form data (set by click handler)
                    try {
                        var storedBtn = $(element).data(data_submitbtn);
                        if (storedBtn && !xhr._btn) {
                            xhr._btn = storedBtn;
                        }
                    } catch (e) { /* ignore */ }

                    // If options had a prepared submitBtn, attach to xhr immediately so fallback works
                    if (options._submitBtn && !xhr._btn) {
                        xhr._btn = options._submitBtn;
                    }

                    //////////////

                    result = getFunction(element.getAttribute("data-ajax-begin"), ["xhr"]).apply(element, arguments);

                    if (result !== false) {
                        //     loading.show(duration);
                        xhr.form = element;   // فرم مربوط به این درخواست
                        // پاک کردن اشاره به خطاهای قبل
                        element.querySelectorAll(".error-border")
                            .forEach(e => e.classList.remove("error-border"));

                        // غیر فعال کردن دکمه سابمیت برای جلوگیری از دوبار کلیک
                        // If we still don't have the btn reference, try to find the marked one
                        if (!xhr._btn) {
                            var btn = element.querySelector(".ajax-submitted") || element.querySelector("button[type='submit'], input[type='submit']");
                            if (btn) {
                                xhr._btn = btn;
                                // ensure it is disabled (in case click didn't run)
                                disableSubmitButton(btn);
                            }
                        }

                        // fallback: اگر پس از زمان مشخصی هنوز re-enable نشده بود، دوباره فعال کن (محافظت در برابر hung requests)
                        if (xhr._btn && !xhr._reEnableTimeout) {
                            xhr._reEnableTimeout = setTimeout(function () {
                                restoreSubmitButtonFromXhr(xhr);
                            }, 30000); // 30s fallback — مقدار را بنا بر نیاز تنظیم کن
                        }
                    } else {
                        // اگر شروعِ درخواست توسط data-ajax-begin کنسل شد، بلافاصله دکمه را بازگردان
                        restoreSubmitButtonFromXhr(xhr);
                    }

                    return result;
                },
                complete: function (xhr, status) {

                    try {
                        getFunction(element.getAttribute("data-ajax-complete"), ["xhr", "status"])
                            .apply(element, arguments);
                    } catch (e) {
                        // ignore handler errors
                    }

                    // restore in complete for compatibility
                    restoreSubmitButtonFromXhr(xhr);

                },
                success: function (data, status, xhr) {
                    asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
                    try {
                        getFunction(element.getAttribute("data-ajax-success"), ["data", "status", "xhr"]).apply(element, arguments);
                    } catch (e) { /* ignore */ }
                },
                error: function (xhr, status, error) {
                    try {
                        getFunction(element.getAttribute("data-ajax-failure"), ["xhr", "status", "error"]).apply(element, arguments);
                    } catch (e) { /* ignore */ }
                }
            });

            options.data.push({ name: "X-Requested-With", value: "XMLHttpRequest" });

            method = options.type.toUpperCase();
            if (!isMethodProxySafe(method)) {
                options.type = "POST";
                options.data.push({ name: "X-HTTP-Method-Override", value: method });
            }

            // change here:
            // Check for a Form POST with enctype=multipart/form-data
            // add the input file that were not previously included in the serializeArray()
            // set processData and contentType to false
            var $element = $(element);
            if ($element.is("form") && $element.attr("enctype") == "multipart/form-data") {
                var formdata = new FormData();
                $.each(options.data, function (i, v) {
                    formdata.append(v.name, v.value);
                });
                $("input[type=file]", $element).each(function () {
                    var file = this;
                    $.each(file.files, function (n, v) {
                        formdata.append(file.name, v);
                    });
                });
                $.extend(options, {
                    processData: false,
                    contentType: false,
                    data: formdata
                });
            }
            // end change

            // perform ajax and attach always handler to ensure restore in every outcome
            var jq = $.ajax(options);

            // ensure jq has reference to submit btn and a fallback timeout
            if (options._submitBtn) {
                jq._btn = options._submitBtn;
            } else {
                // try to find on element as fallback
                try {
                    jq._btn = element.querySelector(".ajax-submitted");
                } catch (e) { /* ignore */ }
            }

            // set a secondary fallback on the jq object (in case beforeSend never executed)
            jq._reEnableTimeout = setTimeout(function () {
                restoreSubmitButtonFromXhr(jq);
            }, 30000);

            jq.always(function () {
                restoreSubmitButtonFromXhr(jq);
            });
        }

    }


    function validate(form) {
        var validationInfo = $(form).data(data_validation);
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    // link ajax
    $(document).on("click", "a[data-ajax=true]", function (evt) {
        evt.preventDefault();
        asyncRequest(this, {
            url: this.href,
            type: "GET",
            data: []
        });
    });

    // image input click coordinates capture
    $(document).on("click", "form[data-ajax=true] input[type=image]", function (evt) {
        var name = evt.target.name,
            target = $(evt.target),
            form = $(target.parents("form")[0]),
            offset = target.offset();

        form.data(data_click, [
            { name: name + ".x", value: Math.round(evt.pageX - offset.left) },
            { name: name + ".y", value: Math.round(evt.pageY - offset.top) }
        ]);

        setTimeout(function () {
            form.removeData(data_click);
        }, 0);
    });

    // mark clicked submit button and disable immediately to prevent double-click
    $(document).on("click", "form[data-ajax=true] :submit", function (evt) {
        var name = evt.currentTarget.name,
            target = $(evt.target),
            form = $(target.parents("form")[0]);

        // mark which button was used for submission (used later by asyncRequest)
        form.data(data_click, name ? [{ name: name, value: evt.currentTarget.value }] : []);
        form.data(data_target, target);

        setTimeout(function () {
            form.removeData(data_click);
            form.removeData(data_target);
            // keep submit btn marker until ajax finishes; do not remove data_submitbtn here
        }, 0);
    });

    // on form submit, validate and call asyncRequest
    $(document).on("submit", "form[data-ajax=true]", function (evt) {
        var clickInfo = $(this).data(data_click) || [],
            clickTarget = $(this).data(data_target),
            isCancel = clickTarget && (clickTarget.hasClass("cancel") || clickTarget.attr('formnovalidate') !== undefined);
        evt.preventDefault();
        if (!isCancel && !validate(this)) {
            // validation failed -> re-enable submit button if we disabled it in click
            try {
                var btn = this.querySelector(".ajax-submitted");
                if (btn) restoreSubmitButtonFromXhr({ _btn: btn });
            } catch (e) { /* ignore */ }
            return;
        }
        asyncRequest(this, {
            url: this.action,
            type: this.method || "GET",
            data: clickInfo.concat($(this).serializeArray())
        });
    });

}(jQuery));

function FindListGrpAjax(GrpKnd, resSelect) {
    if (GrpKnd.length == 0) {
        document.getElementById(resSelect).innerHTML = '<option value="0">گروه کالا انتخاب شود</option>';
        return;
    } else {
        document.getElementById(resSelect).innerHTML = '<option class="text-danger">چند لحظه صبر کنید ...</option>';

        const xmlhttp = new XMLHttpRequest();

        xmlhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                document.getElementById(resSelect).innerHTML = this.responseText;
                $('#' + resSelect).change();
            }
            else
                document.getElementById(resSelect).innerHTML = '<option class="text-danger">خطا در جستجوی گروه ها</option>';;
        };
        xmlhttp.open("GET", "../ApiCenter?handler=FindListGrp&GrpKnd=" + GrpKnd);
        xmlhttp.send();
    }
}

function FindListKalaAllAjax(resSelect, GrpKala, FilterA, FilterB, Sort) {
    document.getElementById(resSelect).innerHTML = '<option value="0" class="text-danger">چند لحظه صبر کنید ...</option>';
    const xmlhttp = new XMLHttpRequest();
    xmlhttp.onload = function () {
        document.getElementById(resSelect).innerHTML = this.responseText;
    }
    xmlhttp.open("GET", "../ApiCenter?handler=FindListKalaAll"
        + "&Group=" + document.getElementById(GrpKala).value
        + "&FilterA=" + document.getElementById(FilterA).value
        + "&FilterB=" + document.getElementById(FilterB).value
        + "&Sort=" + Sort);
    xmlhttp.send();
}
function FindListKalaAjax(resSelect, GrpKala, FilterA, FilterB, Sort) {
    document.getElementById(resSelect).innerHTML = '<option value="0" class="text-danger">چند لحظه صبر کنید ...</option>';
    const xmlhttp = new XMLHttpRequest();
    xmlhttp.onload = function () {
        document.getElementById(resSelect).innerHTML = this.responseText;
    }
    xmlhttp.open("GET", "../ApiCenter?handler=FindListKala"
        + "&Group=" + document.getElementById(GrpKala).value
        + "&FilterA=" + document.getElementById(FilterA).value
        + "&FilterB=" + document.getElementById(FilterB).value
        + "&Sort=" + Sort);
    xmlhttp.send();
}


function FastReportAjax(LimitDateKnd, BtnAct, indexDate, LD) {
    const xmlhttp = new XMLHttpRequest();
    if (LD != '')
        document.getElementById(LD).value = LimitDateKnd;

    SpinnerShow('waitLimitDate' + indexDate);

    xmlhttp.onload = function () {
        const RD = JSON.parse(this.responseText);
        // ست کردن تاریخ
        document.getElementById('FromDate' + indexDate).value = RD.fromDate;
        document.getElementById('ToDate' + indexDate).value = RD.toDate;
        // اجرای گزارش
        if (BtnAct != '') {
            document.getElementById(BtnAct).value = '0';
            document.getElementById(BtnAct).click();
        }
        AreaClear('waitLimitDate' + indexDate);
    }
    xmlhttp.open("GET", "../ApiCenter?handler=LimitDate"
        + "&LimitDateKnd=" + LimitDateKnd);
    xmlhttp.send();
}
function LimitDateAjax(select, indexDate) {
    const xmlhttp = new XMLHttpRequest();
    SpinnerShow('waitLimitDate' + indexDate);

    xmlhttp.onload = function () {
        var RD = JSON.parse(this.responseText);
        // ست کردن تاریخ
        document.getElementById('FromDate' + indexDate).value = RD.fromDate;
        document.getElementById('ToDate' + indexDate).value = RD.toDate;

        AreaClear('waitLimitDate' + indexDate);
    }
    xmlhttp.open("GET", "../ApiCenter?handler=LimitDate"
        + "&LimitDateKnd=" + select.value);
    xmlhttp.send();
}

function HelpAjax(ID) {
    if (document.getElementById('HelpArea').innerHTML != "")
        return;

    const xmlhttp = new XMLHttpRequest();

    GoToPosArea("HelpArea");
    SpinnerShow('HelpArea');

    xmlhttp.onload = function () {
        document.getElementById('HelpArea').innerHTML = this.responseText;
    }
    xmlhttp.open("GET", "../ApiCenter?handler=HelpShow"
        + "&ID=" + ID);
    xmlhttp.send();
}
function CheckNewMsgAjax(ID) {
    const xmlhttp = new XMLHttpRequest();
    setInterval(function () {
        xmlhttp.onload = function () {
            if (this.responseText != "0") {
                document.getElementById('FastAlarmMsg').innerText = 'توجه: ' + this.responseText + ' پیام جدید داری';
                document.getElementById('FastAlarm').classList.remove('dis-none');
                ScrollFastAlarm();
            }
            else {
                document.getElementById('FastAlarm').classList.add('dis-none');
            }
        }
        xmlhttp.open("GET", "../ApiCenter?handler=CheckNewMsg&UserId=" + ID);
        xmlhttp.send();
    }, 60000);
}

