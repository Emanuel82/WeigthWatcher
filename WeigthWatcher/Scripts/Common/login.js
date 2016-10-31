(function ($) {

    var authorizeUri = 'http://localhost:64971/';
    var tokenUri = 'http://localhost:64971/token';

    window.login = {};

    window.login.popup = function showFieldsChecklist(idButton, idPopup) {
        var dropdown = $('.dropdown-content');
        dropdown.hide();

        if (idButton === '' || idPopup === '') return;

        dropdown = $('#' + idPopup);
        var el = $('#' + idButton);

        var pos = el.position();
        var left = el.outerWidth() + el.offset().left - 350;

        if (dropdown.is(":visible")) {
            dropdown.hide();
        }
        else {
            dropdown.css('left', left + 'px');
            dropdown.css('top', pos.top + 'px');
            dropdown.show();
        }

        this.filterColumnName = "";
        this.filterGroupColumnName = "";

        $('.columnName').parent().parent().show();
        $('.groupColumnName').parent().parent().show();
    }

    $('#Login').click(function () {
        var name = $("#name").val();
        var password = $("#password").val();

        var sendInfo = {
            grant_type: "password",
            username: name,
            password: password
        };

        debugger;

        $.ajax({
            url: tokenUri,
            type: 'POST',
            data: sendInfo,
            dataType: 'json',
            success: function (data) {
                var result = data;
                debugger;
                $.cookie("ticket", data.access_token, { path: '/', expires: 7 });
                debugger;
                window.location.href = window.location.href;
            },
            error: function (err) {
                //
                // {readyState: 4, responseText: "{"error":"invalid_grant","error_description":"The user name or password is incorrect."}", responseJSON: Object, status: 400, statusText: "Bad Request"}
                // 

                $('#errorLabel').css("visibility", "visible");
                $('#errorLabel').val(err.responseText.error_description);
            }
        });


        //$.ajax(tokenUri, {
        //    beforeSend: function (xhr) {
        //        xhr.setRequestHeader('Authorization', 'Bearer ' + $('#AccessToken').val());
        //    },
        //    dataType: 'text',
        //    cache: false,
        //    success: function (data) {
        //        console.log(data);
        //        $('#output').text(data);
        //    }
        //});

    });


})(jQuery);