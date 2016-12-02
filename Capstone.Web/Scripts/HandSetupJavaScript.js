/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="jquery-3.1.1.intellisense.js" />

$(document).ready(function () {

    var clickedCard = $(".playingCard");
    var discardField = $(".shouldDiscard");
    
    clickedCard.on("click", function (event) {

        clickedCard = $(this);
        clickedCard.toggleClass("selectedToDiscard");
        discardField.val(false);
        var selectedDiscardField = $(".selectedToDiscard .shouldDiscard");
        selectedDiscardField.val(true);
        
    });
})