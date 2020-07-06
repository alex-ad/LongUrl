$("#InSingleUrl").click(function () {
    $(".urlMulti").val("");
    $("#TextUrlSingle").toggleClass("hidden");
    $("#TextUrlMulti").toggleClass("hidden");
});

$("#InMultiUrl").click(function () {
    $(".urlSingle").val("");
    $("#TextUrlSingle").toggleClass("hidden");
    $("#TextUrlMulti").toggleClass("hidden");
});

$("button[type=submit").click(function () {
    if ($(".urlMulti").val().length < 5 && $(".urlSingle").val().length < 5) return false;
    $("#blockProgress").removeClass("hidden");
    $("#blockResponse > .col").html("");
    return true;
});
