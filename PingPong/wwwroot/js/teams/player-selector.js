$(document).ready(function () {
    $("input[name$='NSelectedPlayers']").click(function () {
        var nPlayers = $(this).val();
        if (nPlayers == 1) {
            $("#player-two-dropdown").hide();
        } else {
            $("#player-two-dropdown").show();
        }
    });
});
