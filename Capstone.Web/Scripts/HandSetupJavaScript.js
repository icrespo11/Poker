/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="jquery-3.1.1.intellisense.js" />
var interval = 0;

$(document).ready(function () {

    interval = setInterval(function () {
        var tableId = $("#TableID").val();

        $.ajax({
            url: serviceUrl + "Game/RefreshTable",
            type: "GET",
            data: {
                tableId: tableId
            },
            dataType: "html"
        }).done(function (htmlData) {
            console.log("Received response");
            $("#gameContainer").html(htmlData);
        }).fail(function (xhr, statusCode, statusMessage) {
            console.log(xhr);
            console.log(statusCode);
            console.log(statusMessage);
        });
    }, 5000);
})