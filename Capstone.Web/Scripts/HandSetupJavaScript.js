/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="jquery-3.1.1.intellisense.js" />

$(document).ready(function () {

    var clickedCard = $(".playingCard");
    var discardField = $(".shouldDiscard");
    
    clickedCard.on("click", function (event) {

        clickedCard = $(this);
        clickedCard.toggleClass("selectedToDiscard");

        var selectedDiscardField = $(".selectedToDiscard .shouldDiscard");
        
        var selectedSuit = clickedCard.attr("data-suit");
        var selectedNumber = clickedCard.attr("data-number");
        $("#Suit").val(selectedSuit);
        $("#Number").val(selectedNumber);
    });
})