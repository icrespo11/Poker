/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="jquery-3.1.1.intellisense.js" />

$(document).ready(function () {

    var clickedCard = $(".playingCard");
    var discardField = $(".shouldDiscard");
    
    clickedCard.on("click", function (event) {

        clickedCard = $(this);
        clickedCard.toggleClass("selectedToDiscard");

        var selectedDiscardField = $(".selectedToDiscard .shouldDiscard");
        
        $("#discardForm").children(".card").remove();

        var discardList = $(".selectedToDiscard");

        for (var i = 0; i < discardList.length; i++) {
            var selectedSuit = $(discardList[i]).attr("data-suit");
            var selectedNumber = $(discardList[i]).attr("data-number");

            var suit = $("<input>").addClass("card").attr("type", "hidden").attr("name", "Discards[" + i + "].Suit").val(selectedSuit);
            var number = $("<input>").addClass("card").attr("type", "hidden").attr("name", "Discards[" + i + "].Number").val(selectedNumber);

            $("#discardForm").append(suit);
            $("#discardForm").append(number);
        }
    });

    setInterval(function () {
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