$("#InSingleUrl").change(function () {
    $(".urlMulti").val("");
    $("#TextUrlSingle").removeClass("hidden");
    $("#TextUrlMulti").addClass("hidden");
    $("#checkIsAntivirusCheck").removeClass("hidden");
});

$("#InMultiUrl").change(function () {
    $(".urlSingle").val("");
    $("#TextUrlSingle").addClass("hidden");
    $("#TextUrlMulti").removeClass("hidden");
    $("#checkIsAntivirusCheck").addClass("hidden");
});

$("button[type=submit").click(function () {
    if ($(".urlMulti").val().length < 5 && $(".urlSingle").val().length < 5) return;
    $("#blockProgress").removeClass("hidden");
    $("#blockResponse > .col").html("");
});
